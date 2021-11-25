/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Agrirouter.Api.Definitions;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Api.Exception;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Api.Service.Parameters.Inner;
using Agrirouter.Common;
using Agrirouter.Common.AgrirouterApi;
using Agrirouter.Common.Extensions;
using Agrirouter.Feed.Request;
using Agrirouter.Impl.Service.Common;
using Agrirouter.Impl.Service.Messaging;
using Agrirouter.Impl.Service.Onboard;
using Agrirouter.Models;
using Agrirouter.Repositories.OnboardingResponse;
using Agrirouter.Repositories.StatusInformation;
using Agrirouter.Repositories.UserSettings;
using Agrirouter.Request;
using Agrirouter.Request.Payload.Account;
using Agrirouter.Request.Payload.Endpoint;
using Agrirouter.Response;
using Agrirouter.Services;
using Agrirouter.Services.Endpoints;
using Agrirouter.Services.Messages;
using Agrirouter.Services.UserInteractions;
using Agrirouter.Services.UserLocation;
using Agrirouter.Services.UserSettings;
using Agrirouter.Technicalmessagetype;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using Plugin.SimpleLogger;
using Xamarin.Forms;
using Timer = System.Timers.Timer;

namespace Agrirouter.Managers.Agrirouter
{
    public class AgrirouterManager : IAgrirouterManager, IDisposable
    {
        private readonly IOnboardingResponseRepository _onboardingResponseRepository;
        private readonly IUserInteractionService _userInteractionService;
        private readonly IGpsInfoMessagesService _gpsInfoMessagesService;
        private readonly IStatusInformationRepository _statusInformationRepository;
        private readonly IEndpointsService _endpointsService;
        private readonly IUserLocationService _userLocationService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly Timer _readOutboxTimer;

        private bool _isGpsPositionRecording;
        private string _teamSetContextId;
        private bool _hasMoreMessagesInFeedAvailable;
        private bool _isDisposed;
        private List<PendingMessageModel> _pendingMessageList;
        private bool _isAgrirouterServerAvailable;
        private RootOnboardError latestOnboardingError;

        public AgrirouterManager(
            IOnboardingResponseRepository onboardingResponseRepository,
            IStatusInformationRepository statusInformationRepository,
            IUserInteractionService userInteractionService,
            IGpsInfoMessagesService gpsInfoMessagesService,
            IUserLocationService userLocationService,
            IUserSettingsService userSettingsService,
            IEndpointsService endpointsService)
        {
            _onboardingResponseRepository = onboardingResponseRepository;
            _statusInformationRepository = statusInformationRepository;
            _userInteractionService = userInteractionService;
            _gpsInfoMessagesService = gpsInfoMessagesService;
            _userLocationService = userLocationService;
            _userSettingsService = userSettingsService;
            _endpointsService = endpointsService;

            _endpointsService.OnEndpointNotFound += EndpointsServiceOnOnEndpointNotFound;

            _teamSetContextId = Guid.NewGuid().ToString();
            _pendingMessageList = new List<PendingMessageModel>();

            _readOutboxTimer = new Timer(10000);
            _readOutboxTimer.Elapsed += OnReadOutboxTimerEvent;
            _readOutboxTimer.AutoReset = true;

            _isAgrirouterServerAvailable = true;
        }

        private async void EndpointsServiceOnOnEndpointNotFound(object sender, EventArgs e)
        {
            await SafeExecuteHandler(SendRequestEndpointsList);
        }

        public void StartReadOutbox()
        {
            _readOutboxTimer.Enabled = true;
        }

        public void StopReadOutbox()
        {
            _readOutboxTimer.Enabled = false;
        }

        public async Task<bool> Authorize(string registrationCode, BaseEnvironment environment)
        {
            try
            {
                var onboardResponse = await OnboardAsync(registrationCode, environment);
                if (onboardResponse is null)
                {
                    return false;
                }

                await _onboardingResponseRepository.SetAsync(onboardResponse);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool HasPendingMessages()
        {
            return _pendingMessageList.Count > 0;
        }

        public void AddToPendingList(string id, string pendingMessageType)
        {
            _pendingMessageList.Add(new PendingMessageModel()
            {
                TechnicalMessageType = pendingMessageType,
                Id = id
            });
        }

        public void RemoveFromPendingList(string id)
        {
            _pendingMessageList.RemoveAll(r => r.Id.Equals(id));
        }

        public string GetTechnicalMessageTypeByIdFromPendingMessagesList(string id)
        {
            var pendingMessage = _pendingMessageList.Find(r => r.Id.Equals(id));
            return pendingMessage == null ? TechnicalMessageTypes.Empty : pendingMessage.TechnicalMessageType;
        }

        public Task<OnboardResponse> GetOnboardResponse()
        {
            return _onboardingResponseRepository.GetAsync();
        }

        public Task<bool> UnAuthorize()
        {
            return Task.Run(() =>
            {
                _onboardingResponseRepository.Clear();
                _endpointsService.ClearEndpoints();
                _gpsInfoMessagesService.ClearList();
                return false;
            });
        }

        public async Task<bool> CheckIfAuthorized()
        {
            var onboardResponse = await _onboardingResponseRepository.GetAsync();
            if (onboardResponse is null)
            {
                return false;
            }

            return true;
        }

        public async Task<HttpClient> GetAuthorizedHttpClient()
        {
            var isAuthorized = await CheckIfAuthorized();
            if (!isAuthorized)
            {
                throw new Exception("Not authorized");
            }

            var onboardResponse = await _onboardingResponseRepository.GetAsync();

            var httpClient = HttpClientFactory.AuthenticatedHttpClient(onboardResponse);
            return httpClient;
        }

        private async Task<OnboardResponse> OnboardAsync(string registrationCode, BaseEnvironment environment)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var deviceInfoService = DependencyService.Get<IDeviceInfoService>();

                    var onboardService = new OnboardService(environment, new UtcDataService(), new HttpClient());
                    var result = onboardService.Onboard(new OnboardParameters
                    {
                        Uuid = deviceInfoService.GetUniqId(),
                        ApplicationId = environment.ApplicationId(),
                        ApplicationType = ApplicationTypeDefinitions.Application,
                        RegistrationCode = registrationCode,
                        CertificationType = Constants.CertificationType,
                        CertificationVersionId = environment.CertificationVersionId(),
                        GatewayId = Constants.GatewayId
                    });

                    return result;
                }
                catch (OnboardException e)
                {
                    var onboardError = JsonConvert.DeserializeObject<RootOnboardError>(e.ErrorMessage);
                    setOnboardingError(onboardError);
                    return null;
                }
                catch (Exception e)
                {
                    setOnboardingError(new RootOnboardError()
                    {
                        Error = new OnboardError()
                        {
                            Code = "0",
                            Message = "Connection Error"
                        }
                    });
                    return null;
                }
            });
        }

        private void setOnboardingError(RootOnboardError onboardError)
        {
            this.latestOnboardingError = onboardError;
        }

        public string getLastOnboardingErrorText()
        {
            if (this.latestOnboardingError != null)
            {
                return this.latestOnboardingError.Error.Message + "( Code: " + this.latestOnboardingError.Error.Code + ")";
            }
            else
            {
                return "No error";
            }
        }

        public async Task SendRequestMessagesFromFeed()
        {
            var queryMessagesService = new QueryMessagesService(new HttpMessagingService(await GetAuthorizedHttpClient()));
            var queryMessagesParameters = new QueryMessagesParameters
            {
                ApplicationMessageId = MessageIdService.ApplicationMessageId(),
                OnboardResponse = await GetOnboardResponse(),
                ValidityPeriod = new ValidityPeriod()
            };

            queryMessagesParameters.ValidityPeriod.SentFrom = UtcDataService.Timestamp(TimestampOffset.OneMinute * 10);
            queryMessagesParameters.ValidityPeriod.SentTo = UtcDataService.Timestamp(TimestampOffset.None);

            AddToPendingList(queryMessagesParameters.ApplicationMessageId, TechnicalMessageTypes.DkeFeedMessageQuery);
            await queryMessagesService.SendAsync(queryMessagesParameters);
        }

        public async Task DeleteOldMessagesFromFeed()
        {
            var feedDeleteService = new FeedDeleteService(new HttpMessagingService(await GetAuthorizedHttpClient()));
            var feedDeleteParameters = new FeedDeleteParameters
            {
                ApplicationMessageId = MessageIdService.ApplicationMessageId(),
                OnboardResponse = await GetOnboardResponse(),
                ValidityPeriod = new ValidityPeriod()
            };

            feedDeleteParameters.ValidityPeriod.SentFrom = UtcDataService.Timestamp(TimestampOffset.FourWeeks);
            feedDeleteParameters.ValidityPeriod.SentTo = UtcDataService.Timestamp(TimestampOffset.OneMinute * 10);

            AddToPendingList(feedDeleteParameters.ApplicationMessageId, TechnicalMessageTypes.DkeFeedDelete);
            await feedDeleteService.SendAsync(feedDeleteParameters);
        }

        public async Task SendCapability()
        {
            var environment = await _userSettingsService.GetCurrentEnvironment();

            var capabilities = new List<CapabilityParameter>();

            var sendReceiveGpsInfoCapabilityParameter = new CapabilityParameter
            {
                Direction = CapabilitySpecification.Types.Direction.SendReceive,
                TechnicalMessageType = TechnicalMessageTypes.GpsInfo
            };

            var receiveDeviceDescriptionCapabilityParameter = new CapabilityParameter
            {
                Direction = CapabilitySpecification.Types.Direction.Receive,
                TechnicalMessageType = TechnicalMessageTypes.Iso11783DeviceDescriptionProtobuf
            };

            var receiveTimeLogCapabilityParameter = new CapabilityParameter
            {
                Direction = CapabilitySpecification.Types.Direction.Receive,
                TechnicalMessageType = TechnicalMessageTypes.Iso11783TimeLogProtobuf
            };

            capabilities.Add(sendReceiveGpsInfoCapabilityParameter);
            capabilities.Add(receiveDeviceDescriptionCapabilityParameter);
            capabilities.Add(receiveTimeLogCapabilityParameter);

            var capabilitiesMessageId = MessageIdService.ApplicationMessageId();
            AddToPendingList(capabilitiesMessageId, TechnicalMessageTypes.DkeCapabilities);

            var capabilitiesParameters = new CapabilitiesParameters
            {
                ApplicationId = environment.Environment.ApplicationId(),
                CertificationVersionId = environment.Environment.CertificationVersionId(),
                OnboardResponse = await GetOnboardResponse().ConfigureAwait(false),
                EnablePushNotifications = CapabilitySpecification.Types.PushNotification.Enabled,
                CapabilityParameters = capabilities,
                TeamsetContextId = _teamSetContextId,
                ApplicationMessageId = capabilitiesMessageId
            };

            var capabilitiesService = new CapabilitiesService(new HttpMessagingService(await GetAuthorizedHttpClient()));
            await capabilitiesService.SendAsync(capabilitiesParameters).ConfigureAwait(false);
        }

        public void OnCapabilitiesResult(ResponseEnvelope envelope, ResponsePayloadWrapper payLoad)
        {
            switch (envelope.Type)
            {
                case ResponseEnvelope.Types.ResponseBodyType.Ack:
                    //All OK, message was sent
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithFailure:
                    //Some issue; to check if it should be mentioned
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithMessages:
                    //Minor issue, no message required
                    break;
            }
        }

        public async Task SendSubscriptions()
        {
            try
            {
                var subscriptionMessageId = MessageIdService.ApplicationMessageId();
                AddToPendingList(subscriptionMessageId, TechnicalMessageTypes.DkeSubscription);

                var subscriptionParameters = new SubscriptionParameters
                {
                    OnboardResponse = await GetOnboardResponse().ConfigureAwait(false),
                    TechnicalMessageTypes = new List<Subscription.Types.MessageTypeSubscriptionItem>(),
                    ApplicationMessageId = subscriptionMessageId
                };

                var technicalMessageType = new Subscription.Types.MessageTypeSubscriptionItem
                {
                    TechnicalMessageType = TechnicalMessageTypes.GpsInfo
                };

                subscriptionParameters.TechnicalMessageTypes.Add(technicalMessageType);

                var subscriptionService = new SubscriptionService(new HttpMessagingService(await GetAuthorizedHttpClient()));
                await subscriptionService.SendAsync(subscriptionParameters).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _userInteractionService.ShowAlert(e.Message);
            }
        }

        public void OnSubscriptionResult(ResponseEnvelope envelope, ResponsePayloadWrapper payLoad)
        {
            switch (envelope.Type)
            {
                case ResponseEnvelope.Types.ResponseBodyType.Ack:
                    //All OK, message was sent
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithFailure:
                    //Some issue; to check if it should be mentioned
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithMessages:
                    //Minor issue, no message required
                    break;
            }
        }

        public async Task SendGpsPosition()
        {
            try
            {
                var publishMessageService = new PublishMessageService(new HttpMessagingService(await GetAuthorizedHttpClient()));

                var messages = await _gpsInfoMessagesService.GetMessages();
                if (messages is null)
                {
                    return;
                }

                foreach (var message in messages)
                {
                    CrossSimpleLogger.Current.Info($"Send Gps Position: Message Id: {message.Id}");
                    try
                    {
                        var messageToSend = await BuildGpsMessage(message.Data);
                        if (messageToSend is null)
                        {
                            return;
                        }

                        var result = await publishMessageService.SendAsync(messageToSend).ConfigureAwait(false);
                        if (result.ApplicationMessageIds.Any())
                        {
                            await _gpsInfoMessagesService.RemoveMessage(message.Id);

                                var statusInformation = await _statusInformationRepository.GetAsync();
                                statusInformation.LastExportDateTime = DateTime.UtcNow;
                                await _statusInformationRepository.SetAsync(statusInformation);
                        }
                    }
                    catch (Exception e)
                    {
                        CrossSimpleLogger.Current.Error("Could not send message " + message.Id + "; Exception: " + e.Message, e);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void OnSendGPSInfoResult(ResponseEnvelope envelope, ResponsePayloadWrapper payLoad)
        {
            switch (envelope.Type)
            {
                case ResponseEnvelope.Types.ResponseBodyType.Ack:
                    //All OK, message was sent
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithFailure:
                    //Some issue; to check if it should be mentioned
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithMessages:
                    //Minor issue, no message required
                    break;
            }
        }

        public async void OnListEndpointsReceived(ResponseEnvelope envelope, ResponsePayloadWrapper payLoad)
        {
            switch (envelope.Type)
            {
                case ResponseEnvelope.Types.ResponseBodyType.EndpointsListing:
                    var listEndpointsService = new ListEndpointsService(new HttpMessagingService(await GetAuthorizedHttpClient()));
                    _ = _endpointsService.UpdateEndpointsList(listEndpointsService.Decode(payLoad.Details));
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.Ack:
                case ResponseEnvelope.Types.ResponseBodyType.AckWithFailure:
                case ResponseEnvelope.Types.ResponseBodyType.AckWithMessages:
                    break;
            }
        }

        public async void OnGetConfirmationResult(ResponseEnvelope envelope, ResponsePayloadWrapper payLoad)
        {
            switch (envelope.Type)
            {
                case ResponseEnvelope.Types.ResponseBodyType.Ack:
                case ResponseEnvelope.Types.ResponseBodyType.AckWithMessages:
                    if (_hasMoreMessagesInFeedAvailable)
                    {
                        await SendRequestMessagesFromFeed();
                    }

                    break;

                //If there is an error confirming the message, we'll just stop requesting.
                //This might lead to a missing position for old machines in very rare cases.
                //If a message cannot be confirmed, it will sooner or later be deleted by the "delete messages older than 20 minutes" function.
                case ResponseEnvelope.Types.ResponseBodyType.AckWithFailure:
                    break;
            }
        }

        public void OnDeleteMessagesFromFeedResult(ResponseEnvelope envelope, ResponsePayloadWrapper payLoad)
        {
            switch (envelope.Type)
            {
                case ResponseEnvelope.Types.ResponseBodyType.Ack:
                    //All OK, message was sent
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithFailure:
                    //Some issue; to check if it should be mentioned
                    break;
                case ResponseEnvelope.Types.ResponseBodyType.AckWithMessages:
                    //Minor issue, no message required
                    break;
            }
        }

        private void OnReadOutboxTimerEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                ReadOutbox();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception in reading Outbox: " + exception.Message);
            }
        }

        public async Task ReadOutbox()
        {
            try
            {
                if (!await CheckIfAuthorized())
                {
                    return;
                }

                _endpointsService.UpdateEndpointsStatus();

                var fetchMessageService = new FetchMessageService(await GetAuthorizedHttpClient());

                var resultList = await fetchMessageService.FetchAsync(await GetOnboardResponse());
                foreach (var result in resultList)
                {
                    var decodedMessage = DecodeMessageService.Decode(result.Command.Message);
                    var envelope = decodedMessage.ResponseEnvelope;
                    var payload = decodedMessage.ResponsePayloadWrapper;

                    //As push notifications do not have a corresponding request, there won't be any matching ID
                    if (envelope.Type == ResponseEnvelope.Types.ResponseBodyType.PushNotification)
                    {
                        OnPushNotificationReceived(envelope, payload);
                        continue;
                    }

                    var technicalMessageType = GetTechnicalMessageTypeByIdFromPendingMessagesList(envelope.ApplicationMessageId);

                    if (technicalMessageType.Equals(TechnicalMessageTypes.DkeCapabilities))
                    {
                        OnCapabilitiesResult(envelope, payload);
                    }

                    if (technicalMessageType.Equals(TechnicalMessageTypes.DkeSubscription))
                    {
                        OnSubscriptionResult(envelope, payload);
                    }

                    if (technicalMessageType.Equals(TechnicalMessageTypes.GpsInfo))
                    {
                        OnSendGPSInfoResult(envelope, payload);
                    }

                    if (technicalMessageType.Equals(TechnicalMessageTypes.DkeListEndpoints))
                    {
                        OnListEndpointsReceived(envelope, payload);
                    }

                    if (technicalMessageType.Equals(TechnicalMessageTypes.DkeFeedConfirm))
                    {
                        OnGetConfirmationResult(envelope, payload);
                    }

                    if (technicalMessageType.Equals(TechnicalMessageTypes.DkeFeedMessageQuery))
                    {
                        OnRequestMessagesForFeedResult(envelope, payload);
                    }

                    if (technicalMessageType.Equals(TechnicalMessageTypes.DkeFeedDelete))
                    {
                        OnDeleteMessagesFromFeedResult(envelope, payload);
                    }

                    RemoveFromPendingList(envelope.ApplicationMessageId);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("EEEERROR on ReadOutbox: " + exception.Message);
            }
        }

        public async void OnPushNotificationReceived(ResponseEnvelope envelope, ResponsePayloadWrapper payload)
        {
            var messageIdsList = new List<string>();

            try
            {
                var decodedMessage = DecodeMessageService.DecodePushNotification(payload.Details);
                foreach (var message in decodedMessage.Messages)
                {
                    try
                    {
                        messageIdsList.Add(message.Header.MessageId);

                        if (message.Header.TechnicalMessageType == TechnicalMessageTypes.GpsInfo)
                        {
                            var data = message.Content.Value.ToByteArray();
                            var index = data.Length - 1;

                            while (data[index] == 0)
                            {
                                index--;
                            }

                            var dataNew = Arrays.CopyOf(data, index + 1);

                            _endpointsService.UpdateGpsPositions(Guid.Parse(message.Header.SenderId), GPSList.Parser.ParseFrom(dataNew));
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            finally
            {
                await SafeExecuteHandler(() => SendConfirmation(messageIdsList));
            }
        }

        public async void OnRequestMessagesForFeedResult(ResponseEnvelope envelope, ResponsePayloadWrapper payload)
        {
            _hasMoreMessagesInFeedAvailable = false;

            var messageIdsList = new List<string>();

            try
            {
                var queryMessagesService = new QueryMessagesService(new HttpMessagingService(await GetAuthorizedHttpClient()));
                var decodedMessage = queryMessagesService.Decode(payload.Details);

                if (decodedMessage.Page != null && decodedMessage.Page.Total > 1)
                {
                    _hasMoreMessagesInFeedAvailable = true;
                }

                foreach (var message in decodedMessage.Messages)
                {
                    try
                    {
                        messageIdsList.Add(message.Header.MessageId);

                        if (message.Header.TechnicalMessageType == TechnicalMessageTypes.GpsInfo)
                        {
                            var data = message.Content.Value.ToByteArray();
                            var index = data.Length - 1;

                            while (data[index] == 0)
                            {
                                index--;
                            }

                            var dataNew = Arrays.CopyOf(data, index + 1);

                            _endpointsService.UpdateGpsPositions(Guid.Parse(message.Header.SenderId), GPSList.Parser.ParseFrom(dataNew));
                        }
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            finally
            {
                await SafeExecuteHandler(() => SendConfirmation(messageIdsList));
            }
        }

        public async Task SendConfirmation(List<string> messageIds)
        {
            var feedConfirmService = new FeedConfirmService(new HttpMessagingService(await GetAuthorizedHttpClient()));
            var feedConfirmParameters = new FeedConfirmParameters()
            {
                ApplicationMessageId = MessageIdService.ApplicationMessageId(),
                MessageIds = messageIds,
                OnboardResponse = await GetOnboardResponse(),
                TeamsetContextId = _teamSetContextId
            };

            AddToPendingList(feedConfirmParameters.ApplicationMessageId, TechnicalMessageTypes.DkeFeedConfirm);
            await feedConfirmService.SendAsync(feedConfirmParameters);
        }

        public async Task SendRequestEndpointsList()
        {
            var listEndpointsService = new ListEndpointsService(new HttpMessagingService(await GetAuthorizedHttpClient()));
            var listEndpointsParameters = new ListEndpointsParameters
            {
                ApplicationMessageId = MessageIdService.ApplicationMessageId(),
                OnboardResponse = await GetOnboardResponse(),
                Direction = ListEndpointsQuery.Types.Direction.SendReceive,
                TechnicalMessageType = TechnicalMessageTypes.GpsInfo
            };

            AddToPendingList(listEndpointsParameters.ApplicationMessageId, TechnicalMessageTypes.DkeListEndpoints);
            await listEndpointsService.SendAsync(listEndpointsParameters);
        }

        public async Task<bool> SafeExecuteHandler(Func<Task> action, int tryingCount = 20)
        {
            try
            {
                tryingCount = 0;

                await action();
                _isAgrirouterServerAvailable = true;
                return true;
            }
            catch (Exception e)
            {
                if (e is CouldNotSendHttpMessageException || e is HttpRequestException)
                {
                    tryingCount--;

                    if (e is CouldNotSendHttpMessageException)
                    {
                        _isAgrirouterServerAvailable = false;
                    }

                    if (tryingCount <= 0)
                    {
                        return false;
                    }

                    await Task.Delay(1000);

                    return await SafeExecuteHandler(action, tryingCount);
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task RecordGpsPosition()
        {
            try
            {
                if (_isGpsPositionRecording)
                {
                    return;
                }

                _isGpsPositionRecording = true;

                var location = await _userLocationService.GetCurrentLocation(false);
                if (location != null)
                {
                    var gpsEntry = new GPSList.Types.GPSEntry();

                    gpsEntry.PositionUp = (long) (location.Altitude ?? 0);
                    gpsEntry.PositionNorth = location.Latitude;
                    gpsEntry.PositionEast = location.Longitude;
                    gpsEntry.PositionStatus = GPSList.Types.GPSEntry.Types.PositionStatus.DGnss;
                    gpsEntry.GpsUtcTimestamp = location.Timestamp.ToTimestamp();
                    await _gpsInfoMessagesService.AddGpsEntry(gpsEntry);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                _isGpsPositionRecording = false;
            }
        }

        private async Task<SendProtobufMessageParameters> BuildGpsMessage(GPSList gpsList)
        {
            if (gpsList.GpsEntries.Count == 0)
            {
                return null;
            }

            var appMessageId = MessageIdService.ApplicationMessageId();
            AddToPendingList(appMessageId, TechnicalMessageTypes.GpsInfo);

            var parameters = new SendProtobufMessageParameters();
            parameters.OnboardResponse = await GetOnboardResponse().ConfigureAwait(false);
            parameters.TypeUrl = GPSList.Descriptor.FullName;
            parameters.TechnicalMessageType = TechnicalMessageTypes.GpsInfo;
            parameters.ApplicationMessageId = appMessageId;
            parameters.TeamsetContextId = _teamSetContextId;

            var encodeMessageHeaderParameters = new MessageHeaderParameters();
            encodeMessageHeaderParameters.Mode = RequestEnvelope.Types.Mode.Publish;
            encodeMessageHeaderParameters.TeamSetContextId = _teamSetContextId;
            encodeMessageHeaderParameters.TechnicalMessageType = TechnicalMessageTypes.GpsInfo;

            var content = gpsList.ToByteString();

            var encodePayloadParameters = new MessagePayloadParameters();
            encodePayloadParameters.TypeUrl = GPSList.Descriptor.FullName;
            encodePayloadParameters.Value = content;

            parameters.ProtobufMessageContent = content;

            return parameters;
        }

        public async Task CheckOutbox(bool silentMode = false)
        {
            do
            {
                await Task.Delay(100);
                var requestOutboxSucceeded = await SafeExecuteHandler(ReadOutbox);
                if (!requestOutboxSucceeded)
                {
                    if (silentMode)
                    {
                        if (HasPendingMessages())
                        {
                            await Task.Delay(500);
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        await _userInteractionService.ShowError("App could be onboarded but initialization process failed! Please check your internet connection and restart the app");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
            } while (HasPendingMessages());
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _endpointsService.OnEndpointNotFound -= EndpointsServiceOnOnEndpointNotFound;
            _readOutboxTimer.Elapsed -= OnReadOutboxTimerEvent;
            _readOutboxTimer?.Dispose();

            _isDisposed = true;
        }
    }
}
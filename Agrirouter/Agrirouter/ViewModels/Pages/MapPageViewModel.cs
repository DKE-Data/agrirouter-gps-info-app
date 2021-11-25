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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Agrirouter.Common;
using Agrirouter.Common.Extensions;
using Agrirouter.Factories;
using Agrirouter.Managers.Agrirouter;
using Agrirouter.Models;
using Agrirouter.Services.AppPermissions;
using Agrirouter.Services.Endpoints;
using Agrirouter.Services.UserInteractions;
using Agrirouter.Services.UserLocation;
using Agrirouter.Services.UserLocation.Models;
using Agrirouter.Services.UserSettings;
using Agrirouter.UI.Controls;
using Agrirouter.UI.Pages;
using Agrirouter.ViewModels.Pages.Abstract;
using Plugin.SimpleLogger;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Shiny;
using Shiny.Notifications;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Position = Xamarin.Forms.Maps.Position;

namespace Agrirouter.ViewModels.Pages
{
    public class MapPageViewModel : BasePageViewModel
    {
        private readonly IAgrirouterManager _agrirouterManager;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IUserInteractionService _userInteractionService;
        private readonly INotificationManager _notificationManager;
        private readonly IEndpointsService _endpointsService;
        private readonly IPinsFactory _pinsFactory;
        private readonly IPermissionsService _permissionsService;
        private readonly IUserLocationService _userLocationService;

        public MapType MapType { get; set; }

        public bool IsLegendVisible { get; set; }

        public bool IsConnected { get; set; }

        public bool IsDataSending { get; set; }

        public bool IsShowingUser { get; set; }

        public MapViewTypeEnum MapViewType { get; set; }

        public bool HasStatusMessage { get; set; }

        public string StatusText { get; set; }

        public EndpointPin SelectedEndpointPin { get; set; }

        public bool IsPinInformationVisible => SelectedEndpointPin != null;

        public Position? CurrentPosition { get; set; }

        public ObservableRangeCollection<EndpointPin> Endpoints { get; set; }

        public int AllEndpointsCount => Endpoints?.Count() ?? 0;

        public int ActiveEndpointsCount => Endpoints?.Where(endpoint => endpoint.IsActive).Count() ?? 0;

        public ICommand SettingsCommand { get; }
        public ICommand StartStopDataCommand { get; }
        public ICommand ClosePinInformationCommand { get; }
        public ICommand PinTappedCommand { get; }
        public ICommand ShowMyPositionCommand { get; }
        public ICommand MapLayersCommand { get; }
        public ICommand ChangeLegendVisibilityCommand { get; }
        public ICommand ShowAllEndpointsCommand { get; }

        public MapPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IAgrirouterManager agrirouterManager,
            IUserInteractionService userInteractionService,
            IEndpointsService endpointsService,
            IUserSettingsService userSettingsService,
            IPinsFactory pinsFactory,
            IUserLocationService userLocationService,
            IPermissionsService permissionsService) : base(navigationService, pageDialogService)
        {
            _agrirouterManager = agrirouterManager;
            _userSettingsService = userSettingsService;
            _userInteractionService = userInteractionService;
            _endpointsService = endpointsService;
            _pinsFactory = pinsFactory;
            _userLocationService = userLocationService;
            _permissionsService = permissionsService;
            _notificationManager = ShinyHost.Resolve<INotificationManager>();

            _userLocationService.OnLocationChanged += OnLocationChanged;
            _endpointsService.OnEndpointsChanged += OnEndpointsChanged;

            Endpoints = new ObservableRangeCollection<EndpointPin>();
            SettingsCommand = new DelegateCommand(async () => await SettingsCommandExecute());
            StartStopDataCommand = new DelegateCommand(async () => await StartStopDataCommandExecute());
            ClosePinInformationCommand = new DelegateCommand(ClosePinInformationCommandExecute);
            PinTappedCommand = new DelegateCommand<EndpointPin>(PinTappedCommandExecute);
            ShowMyPositionCommand = new DelegateCommand(ShowMyPositionCommandExecute);
            MapLayersCommand = new DelegateCommand(MapLayersCommandExecute);
            ChangeLegendVisibilityCommand = new DelegateCommand(ChangeLegendVisibilityCommandExecute);
            ShowAllEndpointsCommand = new DelegateCommand(ShowAllEndpoints);
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            var settings = await _userSettingsService.GetSettings();
            
            DeviceDisplay.KeepScreenOn = settings.IsDisplayOn;
            MapType = settings.MapType;
            MapViewType = settings.MapViewType;
            HasStatusMessage = true;
            IsDataSending = await CheckIfDataSending();

            if (IsDataSending)
            {
                ShowGpsTrackingNotification();
            }

            if (MapViewType != MapViewTypeEnum.Default)
            {
                _userLocationService.SetLocationAutoChecking(true);
            }

            var endpoints = await _endpointsService.GetEndpoints();

            // endpoints = new List<EndpointModel>
            // {
            //     new EndpointModel
            //     {
            //         Name = "Lutsk",
            //         IsActive = true,
            //         IsVisible = true,
            //         Latitude = 50.7412908,
            //         Longitude = 25.3230167
            //     },
            // new EndpointModel
            // {
            //     Name = "Left",
            //     IsActive = true,
            //     IsVisible = true,
            //     Latitude = 49.597066,
            //     Longitude = 22.656340
            // },
            // new EndpointModel
            // {
            //     Name = "Top",
            //     IsActive = true,
            //     IsVisible = true,
            //     Latitude = 52.381597,
            //     Longitude = 32.580481
            // },
            // new EndpointModel
            // {
            //     Name = "Right",
            //     IsActive = true,
            //     IsVisible = true,
            //     Latitude = 48.655391,
            //     Longitude = 39.983171
            // },
            // new EndpointModel
            // {
            //     Name = "Bottom",
            //     IsActive = true,
            //     IsVisible = true,
            //     Latitude = 41.957041,
            //     Longitude = 33.521618
            // },
            //};

            Endpoints = new ObservableRangeCollection<EndpointPin>(_pinsFactory.ProducePins(endpoints, PinTappedCommand));

            await SetCurrentLocation();
            await InitializeCommunication();
            HasStatusMessage = false;
        }

        public override async void OnAppearing()
        {
            IsConnected = await _agrirouterManager.CheckIfAuthorized();
            IsDataSending = await CheckIfDataSending();
        }

        public override void Destroy()
        {
            _endpointsService.OnEndpointsChanged -= OnEndpointsChanged;
            _userLocationService.OnLocationChanged -= OnLocationChanged;
        }

        private async Task SetCurrentLocation()
        {
            var location = await _userLocationService.GetCurrentLocation(true);
            if (location is null)
            {
                return;
            }

            CurrentPosition = null;
            CurrentPosition = new Position(location.Latitude, location.Longitude);
            IsShowingUser = true;
        }

        private async Task InitializeCommunication()
        {
            if (!await _agrirouterManager.CheckIfAuthorized())
            {
                return;
            }

            var result = true;

            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                StatusText = "Device Offline!";
                await Task.Delay(2000);
                await InitializeCommunication();
                return;
            }

            StatusText = "Clean inbox...";
            await _agrirouterManager.CheckOutbox(true);

            StatusText = "Send Capabilities...";
            var sendCapabilityResult = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.SendCapability, 2);
            if (!sendCapabilityResult)
            {
                result = false;
            }

            await _agrirouterManager.CheckOutbox(true);

            StatusText = "Send Subscriptions...";
            var sendSubscriptionResult = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.SendSubscriptions, 2);
            if (!sendSubscriptionResult)
            {
                result = false;
            }

            await _agrirouterManager.CheckOutbox(true);

            StatusText = "Delete outdated messages...";
            var deleteOldMessagesResult = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.DeleteOldMessagesFromFeed, 2);
            if (!deleteOldMessagesResult)
            {
                result = false;
            }

            await _agrirouterManager.CheckOutbox(true);
            StatusText = "Request List of Endpoints...";
            var sendRequestEndpointList = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.SendRequestEndpointsList, 2);
            if (!sendRequestEndpointList)
            {
                result = false;
            }

            await _agrirouterManager.CheckOutbox(true);

            StatusText = "Load old GPS positions...";
            var sendRequestMessagesResult = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.SendRequestMessagesFromFeed, 2);
            if (!sendRequestMessagesResult)
            {
                result = false;
            }

            await _agrirouterManager.CheckOutbox(true);

            if (!result)
            {
                await _userInteractionService.ShowError("App could be onboarded but initialization process failed! Please check your internet connection and try again", "", "Retry");
                await InitializeCommunication();
                return;
            }

            _agrirouterManager.StartReadOutbox();
        }

        private Task SettingsCommandExecute()
        {
            return NavigationService.NavigateAsync(nameof(SettingsPage));
        }

        private async Task StartStopDataCommandExecute()
        {
            var hasLocationPermission = await _permissionsService.AskingForLocationPermission();
            if (!hasLocationPermission)
            {
                return;
            }

            if (!IsConnected)
            {
                await PageDialogService.DisplayAlertAsync("Information", "Please connect first, to start telemetry", "Ok");
                await NavigationService.NavigateAsync(nameof(SettingsPage));
                return;
            }

            var settings = await _userSettingsService.GetSettings();
            settings.IsDataSending = !IsDataSending;
            await _userSettingsService.SaveSettings(settings);
            IsDataSending = await CheckIfDataSending();

            await _notificationManager.Clear();

            if (IsDataSending)
            {
                ShowGpsTrackingNotification();
            }
        }

        private async void ShowGpsTrackingNotification()
        {
            var notification = new Notification()
            {
                Title = "Telemetry",
                Message = "GPS tracking is active",
                Sound = NotificationSound.None,
                Android = new AndroidOptions()
                {
                    AutoCancel = false,
                    NotificationImportance = AndroidNotificationImportance.Max,
                    OnGoing = true
                }
            };

            if (Device.RuntimePlatform == Device.Android && DeviceInfo.Version.Major >= 8 || Device.RuntimePlatform == Device.iOS)
            {
                await _notificationManager.Clear();
                await _notificationManager.Send(notification);
            }

            CrossSimpleLogger.Current.Info($"GPS tracking is active");
        }

        private async Task<bool> CheckIfDataSending()
        {
            var settings = await _userSettingsService.GetSettings();
            return settings.IsDataSending;
        }

        private void OnEndpointsChanged(object sender, List<EndpointModel> endpoints)
        {
            Endpoints = new ObservableRangeCollection<EndpointPin>(_pinsFactory.ProducePins(endpoints, PinTappedCommand));
        }

        private void PinTappedCommandExecute(EndpointPin pin)
        {
            SelectedEndpointPin = pin;
        }

        private void ClosePinInformationCommandExecute()
        {
            SelectedEndpointPin = null;
        }

        private async void ShowMyPositionCommandExecute()
        {
            var hasLocationPermission = await _permissionsService.AskingForLocationPermission();
            if (!hasLocationPermission)
            {
                return;
            }

            if (MapViewType == MapViewTypeEnum.AlwaysShowMyPosition)
            {
                _userLocationService.SetLocationAutoChecking(false);
                MapViewType = MapViewTypeEnum.Default;
            }
            else
            {
                _userLocationService.SetLocationAutoChecking(true);
                MapViewType = MapViewTypeEnum.AlwaysShowMyPosition;
            }

            await SetCurrentLocation();

            var settings = await _userSettingsService.GetSettings();
            settings.MapViewType = MapViewType;
            await _userSettingsService.SaveSettings(settings);
        }

        private async void MapLayersCommandExecute()
        {
            var mapTypes = ((MapType[]) Enum.GetValues(typeof(MapType))).Select(mapType => mapType.ToString()).ToArray();

            var mapTypeName = await App.Current.MainPage.DisplayActionSheet("Map Type", "Cancel", null, mapTypes);
            if (!string.IsNullOrEmpty(mapTypeName) && mapTypeName != "Cancel")
            {
                MapType = mapTypeName.ToEnum(MapType.Street);

                var settings = await _userSettingsService.GetSettings();
                settings.MapType = MapType;
                await _userSettingsService.SaveSettings(settings);
            }
        }

        private void ChangeLegendVisibilityCommandExecute()
        {
            IsLegendVisible = !IsLegendVisible;
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs eventArgs)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var location = eventArgs.Location;
                if (location is null)
                {
                    return;
                }

                CurrentPosition = null;
                CurrentPosition = new Position(location.Latitude, location.Longitude);
            });
        }

        private async void ShowAllEndpoints()
        {
            var hasLocationPermission = await _permissionsService.AskingForLocationPermission();
            if (!hasLocationPermission)
            {
                return;
            }

            if (MapViewType == MapViewTypeEnum.AlwaysShowAllPinsAndMyPosition)
            {
                _userLocationService.SetLocationAutoChecking(false);
                MapViewType = MapViewTypeEnum.Default;
            }
            else
            {
                _userLocationService.SetLocationAutoChecking(true);
                MapViewType = MapViewTypeEnum.AlwaysShowAllPinsAndMyPosition;
            }

            await SetCurrentLocation();

            var settings = await _userSettingsService.GetSettings();
            settings.MapViewType = MapViewType;

            await _userSettingsService.SaveSettings(settings);
        }
    }
}
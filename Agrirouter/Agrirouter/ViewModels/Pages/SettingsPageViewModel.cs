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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Agrirouter.Managers.Agrirouter;
using Agrirouter.Models;
using Agrirouter.Services.Messages;
using Agrirouter.Services.UserInteractions;
using Agrirouter.Services.UserSettings;
using Agrirouter.UI.Pages;
using Agrirouter.Validators;
using Agrirouter.ViewModels.Items;
using Agrirouter.ViewModels.Pages.Abstract;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Essentials;

namespace Agrirouter.ViewModels.Pages
{
    public class SettingsPageViewModel : BasePageViewModel
    {
        private const string Cancel = "Cancel";

        private readonly IAgrirouterManager _agrirouterManager;
        private readonly IUserInteractionService _userInteractionService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IGpsInfoMessagesService _gpsInfoMessagesService;
        private readonly SettingsPageViewModelValidator _validator;

        private UserSettingsModel _settings;

        public string RegistrationCode { get; set; }

        public int CredentialsValidUntil { get; set; }
        
        public string UniqId { get; set; }

        public bool IsConnected { get; set; }

        private bool _isDisplayOn;

        public bool IsDisplayOn
        {
            get => _isDisplayOn;
            set => SetProperty(ref _isDisplayOn, value, IsDisplayOnChanged);
        }

        public EnvironmentViewModel Environment { get; set; }

        public CycleViewModel RecordCycle { get; set; }

        public CycleViewModel SendCycle { get; set; }

        public ExportInformationModel ExportInformation { get; set; }

        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand EnvironmentCommand { get; }
        public ICommand RecordCycleCommand { get; }
        public ICommand SendCycleCommand { get; }
        
        public ICommand ExternalIdCommand { get; }
        public ICommand GoToLogsCommand { get; }
        public ICommand GoToAboutApplicationCommand { get; }

        public SettingsPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IAgrirouterManager agrirouterManager,
            IUserInteractionService userInteractionService,
            IGpsInfoMessagesService gpsInfoMessagesService,
            IUserSettingsService userSettingsService) : base(navigationService, pageDialogService)
        {
            _agrirouterManager = agrirouterManager;
            _userInteractionService = userInteractionService;
            _gpsInfoMessagesService = gpsInfoMessagesService;
            _userSettingsService = userSettingsService;

            _validator = new SettingsPageViewModelValidator();

            _gpsInfoMessagesService.GpsInfoMessagesRepository.OnDataChanged += GpsInfoMessagesRepositoryOnSaveValue;

            ConnectCommand = new DelegateCommand(async () => await ConnectCommandAsync());
            DisconnectCommand = new DelegateCommand(async () => await DisconnectCommandAsync());
            EnvironmentCommand = new DelegateCommand(async () => await EnvironmentCommandAsync());
            RecordCycleCommand = new DelegateCommand(async () => await RecordCycleCommandAsync());
            SendCycleCommand = new DelegateCommand(async () => await SendCycleCommandAsync());
            ExternalIdCommand = new DelegateCommand(async () => await ExternalIdTappedAsync());
            GoToLogsCommand = new DelegateCommand(async () => await GoToLogsCommandAsync());
            GoToAboutApplicationCommand = new DelegateCommand(async () => await GoToAboutApplicationCommandAsync());
        }
        
        public override async void Initialize(INavigationParameters parameters)
        {
            _settings = await _userSettingsService.GetSettings();
            IsConnected = await _agrirouterManager.CheckIfAuthorized();
            CredentialsValidUntil = _settings.CredentialsValidUntil;
            IsDisplayOn = _settings.IsDisplayOn;
            UniqId = _userSettingsService.GetUniqId();
            Environment = _userSettingsService.GetEnvironmentByName(_settings.EnvironmentName);
            RecordCycle = _userSettingsService.RecordCycleIntervals.FirstOrDefault(item => item.Value == _settings.RecordCycle);
            SendCycle = _userSettingsService.SendCycleIntervals.FirstOrDefault(item => item.Value == _settings.SendCycle);
            ExportInformation = await _gpsInfoMessagesService.GetExportInformation();
        }

        public override void Destroy()
        {
            _gpsInfoMessagesService.GpsInfoMessagesRepository.OnDataChanged -= GpsInfoMessagesRepositoryOnSaveValue;
        }

        private async void GpsInfoMessagesRepositoryOnSaveValue(object sender, EventArgs e)
        {
            ExportInformation = await _gpsInfoMessagesService.GetExportInformation();
        }

        private async Task ConnectCommandAsync()
        {
            var validationResult = _validator.Validate(this);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => $"{e.ErrorMessage}");
                await _userInteractionService.ShowError(string.Join(".\n", errors));
                return;
            }

            _userInteractionService.ShowLoading("Connecting...");

            IsConnected = await _agrirouterManager.Authorize(RegistrationCode, Environment.Environment);

            if (IsConnected)
            {
                RegistrationCode = string.Empty;

                _settings = await _userSettingsService.GetSettings();
                _settings.IsDataSending = false;
                _settings.OnboardingDateTime = DateTime.UtcNow;
                await _userSettingsService.SaveSettings(_settings);

                _userInteractionService.ShowLoading("Send capabilities...");
                var sendCapabilityResult = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.SendCapability, 10);
                if (!sendCapabilityResult)
                {
                    await _userInteractionService.ShowError("App could be onboarded but initialization process failed! Please check your internet connection and restart the app");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                await _agrirouterManager.CheckOutbox(true);

                _userInteractionService.ShowLoading("Send subscriptions...");
                var sendSubscriptionResult = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.SendSubscriptions, 10);
                if (!sendSubscriptionResult)
                {
                    await _userInteractionService.ShowError("App could be onboarded but initialization process failed! Please check your internet connection and restart the app");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                await _agrirouterManager.CheckOutbox(true);

                _userInteractionService.ShowLoading("Send request endpoints list...");
                var sendRequestEndpointList = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.SendRequestEndpointsList, 10);
                if (!sendRequestEndpointList)
                {
                    await _userInteractionService.ShowError("App could be onboarded but initialization process failed! Please check your internet connection and restart the app");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                await _agrirouterManager.CheckOutbox(true);

                _userInteractionService.ShowLoading("Delete old messages from feed...");
                var deleteOldMessagesResult = await _agrirouterManager.SafeExecuteHandler(_agrirouterManager.DeleteOldMessagesFromFeed, 2);
                if (!deleteOldMessagesResult)
                {
                    await _userInteractionService.ShowError("App could be onboarded but initialization process failed! Please check your internet connection and restart the app");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                await _agrirouterManager.CheckOutbox(true);

                _agrirouterManager.StartReadOutbox();

                _userInteractionService.HideLoading();
                await _userInteractionService.ShowAlert("Connected!");
            }
            else
            {
                await _userInteractionService.ShowError("Could not onboard \n" + _agrirouterManager.getLastOnboardingErrorText());
                //TODO Add Error message from Agrirouter Onboarding Result
                _userInteractionService.HideLoading();
            }

            CredentialsValidUntil = _settings.CredentialsValidUntil;
            IsConnected = await _agrirouterManager.CheckIfAuthorized();
        }
        
        private async Task ExternalIdTappedAsync()
        {
            try
            {
                await Clipboard.SetTextAsync(UniqId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _userInteractionService.ShowToast("ExternalID copied to clipboard");
            }
        }

        private async Task DisconnectCommandAsync()
        {
            IsConnected = await _agrirouterManager.UnAuthorize();

            if (!IsConnected)
            {
                _agrirouterManager.StopReadOutbox();
                RegistrationCode = string.Empty;
                SecureStorage.RemoveAll();
                await _userInteractionService.ShowAlert("The endpoint needs to be deleted in the agrirouter UI as well. \n App will end now");
                Process.GetCurrentProcess().Kill();
            }
        }

        private async Task EnvironmentCommandAsync()
        {
            var environmentName = await App.Current.MainPage.DisplayActionSheet("Environment", "Cancel", null, _userSettingsService.Environments.Select(item => item.Name).ToArray());
            if (!string.IsNullOrEmpty(environmentName) && environmentName != Cancel)
            {
                Environment = _userSettingsService.Environments.FirstOrDefault(item => item.Name == environmentName);

                if (Environment is null)
                {
                    return;
                }

                _settings.EnvironmentName = Environment.Name;
                await _userSettingsService.SaveSettings(_settings);
            }
        }

        private async Task SendCycleCommandAsync()
        {
            var sendCycle = await App.Current.MainPage.DisplayActionSheet("Send Cycle", "Cancel", null, _userSettingsService.SendCycleIntervals.Select(item => item.Name.ToString()).ToArray());
            if (!string.IsNullOrEmpty(sendCycle) && sendCycle != Cancel)
            {
                SendCycle = _userSettingsService.SendCycleIntervals.FirstOrDefault(item => item.Name == sendCycle);

                if (SendCycle is null)
                {
                    return;
                }

                _settings.SendCycle = SendCycle.Value;
                await _userSettingsService.SaveSettings(_settings);
            }
        }

        private async void IsDisplayOnChanged()
        {
            DeviceDisplay.KeepScreenOn = IsDisplayOn;
            _settings.IsDisplayOn = IsDisplayOn;
            await _userSettingsService.SaveSettings(_settings);
        }

        private async Task RecordCycleCommandAsync()
        {
            var recordCycle = await App.Current.MainPage.DisplayActionSheet("Record Cycle", "Cancel", null, _userSettingsService.RecordCycleIntervals.Select(item => item.Name.ToString()).ToArray());
            if (!string.IsNullOrEmpty(recordCycle) && recordCycle != Cancel)
            {
                RecordCycle = _userSettingsService.RecordCycleIntervals.FirstOrDefault(item => item.Name == recordCycle);

                if (RecordCycle is null)
                {
                    return;
                }

                _settings.RecordCycle = RecordCycle.Value;
                await _userSettingsService.SaveSettings(_settings);
            }
        }

        private Task GoToLogsCommandAsync()
        {
            return NavigationService.NavigateAsync(nameof(LogsPage));
        }

        private Task GoToAboutApplicationCommandAsync()
        {
            return NavigationService.NavigateAsync(nameof(AboutPage));
        }
    }
}
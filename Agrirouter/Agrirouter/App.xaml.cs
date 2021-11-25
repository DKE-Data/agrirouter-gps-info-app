using Agrirouter.Factories;
using Agrirouter.Managers.Agrirouter;
using Agrirouter.Repositories.Endpoints;
using Agrirouter.Repositories.Messages;
using Agrirouter.Repositories.OnboardingResponse;
using Agrirouter.Repositories.StatusInformation;
using Agrirouter.Repositories.UserSettings;
using Agrirouter.Services;
using Agrirouter.Services.AppPermissions;
using Agrirouter.Services.Endpoints;
using Agrirouter.Services.Localization;
using Agrirouter.Services.Messages;
using Agrirouter.Services.Rest;
using Agrirouter.Services.Rest.Abstract;
using Agrirouter.Services.Shiny.Jobs;
using Agrirouter.Services.Shiny.Listeners;
using Agrirouter.Services.UserInteractions;
using Agrirouter.Services.UserLocation;
using Agrirouter.Services.UserSettings;
using Agrirouter.UI.Pages;
using Agrirouter.ViewModels.Pages;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Xamarin.Essentials;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace Agrirouter
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            AppCenter.Start("android=b2a12299-153b-4d7d-b421-88bccfd33503;ios=445468ab-474a-420e-9c40-4af29be8a773", typeof(Analytics), typeof(Crashes));
            Application.Current.UserAppTheme = OSAppTheme.Light;
            
            if (VersionTracking.IsFirstLaunchEver)
            {
                SecureStorage.RemoveAll();

                if (Device.RuntimePlatform == Device.Android)
                {
                    await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(BatteryOptimizationsPage)}");
                    return;
                }
                
                await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(MapPage)}");
            }
            else
            {
                await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(MapPage)}");
            }
            
            if (Device.RuntimePlatform == Device.iOS)
            {
                var locationService = DependencyService.Get<ILocationService>();
                locationService.StartLocationUpdates();
                
                var gpsRecordingJob = new GpsRecordingJob();
                _ = gpsRecordingJob.Run();
                
                var gpsSendingJob = new GpsSendingJob();
                _ = gpsSendingJob.Run();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<LogsPage, LogsPageViewModel>();
            containerRegistry.RegisterForNavigation<AboutPage, AboutPageViewModel>();
            containerRegistry.RegisterForNavigation<BatteryOptimizationsPage, BatteryOptimizationsPageViewModel>();

            containerRegistry.RegisterSingleton<IGpsListener, GpsListener>();
            containerRegistry.RegisterSingleton<IRestService, RestService>();

            containerRegistry.RegisterSingleton<IOnboardingResponseRepository, OnboardingResponseRepository>();
            containerRegistry.RegisterSingleton<IUserSettingsRepository, UserSettingsRepository>();
            containerRegistry.RegisterSingleton<IUserInteractionService, UserInteractionService>();
            containerRegistry.RegisterSingleton<IEndpointsRepository, EndpointsRepository>();

            containerRegistry.RegisterSingleton<IEndpointsService, EndpointsService>();
            containerRegistry.RegisterSingleton<IAgrirouterManager, AgrirouterManager>();
            containerRegistry.RegisterSingleton<IUserSettingsService, UserSettingsService>();
            containerRegistry.RegisterSingleton<IUserLocationService, UserLocationService>();


            containerRegistry.RegisterSingleton<IStatusInformationRepository, StatusInformationRepository>();
            containerRegistry.RegisterSingleton<IGpsInfoMessagesRepository, GpsInfoMessagesRepository>();
            containerRegistry.RegisterSingleton<IGpsInfoMessagesService, GpsInfoMessagesService>();
            
            containerRegistry.RegisterSingleton<IPinsFactory, PinsFactory>();
            containerRegistry.RegisterSingleton<IPermissionsService, PermissionsService>();
            containerRegistry.RegisterSingleton<ILocalizationService, LocalizationService>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
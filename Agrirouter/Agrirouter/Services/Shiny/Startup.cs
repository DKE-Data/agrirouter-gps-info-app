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
using Agrirouter.Services.Shiny.Delegates;
using Agrirouter.Services.Shiny.Jobs;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using Prism.Ioc;
using Shiny;
using Shiny.Jobs;
using Shiny.Logging;

namespace Agrirouter.Services.Shiny
{
    public class Startup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            Log.UseConsole();
            Log.UseDebug();

            services.UseGps<GpsDelegate>();
            services.UseNotifications(true);

            var gpsRecordingJob = new JobInfo(typeof(GpsRecordingJob), nameof(GpsRecordingJob))
            {
                BatteryNotLow = true,
                DeviceCharging = false,
                RunOnForeground = true,
                Repeat = true,
                IsSystemJob = true,
            };
            
            var gpsSendingJob = new JobInfo(typeof(GpsSendingJob), nameof(GpsSendingJob))
            {
                BatteryNotLow = true,
                DeviceCharging = false,
                RunOnForeground = true,
                RequiredInternetAccess = InternetAccess.Any,
                Repeat = true,
                IsSystemJob = true,
            };
            
            services.RegisterJob(gpsRecordingJob);
            services.RegisterJob(gpsSendingJob);
        }
        
        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            ContainerLocator.SetContainerExtension(() => new DryIocContainerExtension());
            var container = ContainerLocator.Container.GetContainer();
            DryIocAdapter.Populate(container, services);
            return container.GetServiceProvider();
        }
    }
}
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
using System.Threading;
using System.Threading.Tasks;
using Agrirouter.Managers.Agrirouter;
using Agrirouter.Services.UserSettings;
using Shiny;
using Shiny.Jobs;

namespace Agrirouter.Services.Shiny.Jobs
{
    public class GpsRecordingJob : IJob
    {
        private readonly IAgrirouterManager _agrirouterManager;
        private readonly IUserSettingsService _userSettingsService;

        public GpsRecordingJob()
        {
            _userSettingsService = ShinyHost.Resolve<IUserSettingsService>();
            _agrirouterManager = ShinyHost.Resolve<IAgrirouterManager>();
        }

        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            try
            {
                while (true)
                {
                    var settings = await _userSettingsService.GetSettings();

                    await Task.Delay(settings.RecordCycle);

                    if (settings.IsDataSending)
                    {
                        await _agrirouterManager.RecordGpsPosition();
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Run()
        {
            while (true)
            {
                try
                {
                    var settings = await _userSettingsService.GetSettings();

                    await Task.Delay(settings.RecordCycle);

                    if (settings.IsDataSending)
                    {
                        await _agrirouterManager.RecordGpsPosition();
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }
}
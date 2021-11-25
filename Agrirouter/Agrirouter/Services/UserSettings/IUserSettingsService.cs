/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using Agrirouter.Models;
using Agrirouter.ViewModels.Items;

namespace Agrirouter.Services.UserSettings
{
    public interface IUserSettingsService
    {
        public List<EnvironmentViewModel> Environments { get; }
        
        public List<CycleViewModel> RecordCycleIntervals { get; }

        public List<CycleViewModel> SendCycleIntervals { get; }
        
        Task<UserSettingsModel> GetSettings();

        EnvironmentViewModel GetEnvironmentByName(string name);

        Task<EnvironmentViewModel> GetCurrentEnvironment();

        Task SaveSettings(UserSettingsModel userSettingsModel);

        string GetUniqId();
    }
}
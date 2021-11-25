/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using Agrirouter.Common;
using Agrirouter.Models;
using Agrirouter.Repositories.Abstract;
using Agrirouter.UI.Controls;
using Xamarin.Forms.Maps;

namespace Agrirouter.Repositories.UserSettings
{
    public class UserSettingsRepository : Repository<UserSettingsModel>, IUserSettingsRepository
    {
        public override UserSettingsModel Initialize()
        {
            return new UserSettingsModel
            {
                EnvironmentName = "QA",
                IsDataSending = false,
                RecordCycle = 5000,
                SendCycle = 10000,
                MapType = MapType.Street,
                MapViewType = MapViewTypeEnum.Default,
                CurrentCulture = Constants.LocalizationConstants.Default
            };
        }
    }
}
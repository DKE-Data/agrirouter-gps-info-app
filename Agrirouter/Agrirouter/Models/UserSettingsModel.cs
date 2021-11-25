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
using Agrirouter.UI.Controls;
using Xamarin.Forms.Maps;

namespace Agrirouter.Models
{
    public class UserSettingsModel
    {
        public int RecordCycle { get; set; }
        
        public int SendCycle { get; set; }
        
        public string EnvironmentName { get; set; }
        
        public MapType MapType { get; set; }
        
        public bool IsDataSending { get; set; }
        
        public bool IsDisplayOn { get; set; }
        
        public MapViewTypeEnum MapViewType { get; set; }
        
        public DateTime? OnboardingDateTime { get; set; }
        
        public string CurrentCulture { get; set; }

        public int CredentialsValidUntil
        {
            get
            {
                var value = OnboardingDateTime.HasValue ? (OnboardingDateTime.Value.AddYears(1) - DateTime.UtcNow).Days : 0;
                return value < 0 ? 0 : value;
            }
        }
    }
}
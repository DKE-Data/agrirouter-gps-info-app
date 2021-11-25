/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using Agrirouter.Common.Extensions;
using Agrirouter.Droid.Services;
using Agrirouter.Services;
using Android.Provider;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInfoService))]
namespace Agrirouter.Droid.Services
{
    public class DeviceInfoService : IDeviceInfoService
    {
        public string GetUniqId()
        {
            try
            {
                return Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Settings.Secure.AndroidId).GetMd5Hash();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
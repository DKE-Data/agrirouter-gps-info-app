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
using Agrirouter.iOS.Services;
using Agrirouter.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInfoService))]

namespace Agrirouter.iOS.Services
{
    public class DeviceInfoService : IDeviceInfoService
    {
        public string GetUniqId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString().GetMd5Hash();
        }
    }
}
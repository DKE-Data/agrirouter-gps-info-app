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
using System.Threading.Tasks;
using Agrirouter.Services.Shiny.Listeners;
using Shiny;
using Shiny.Locations;
using Shiny.Notifications;

namespace Agrirouter.Services.Shiny.Delegates
{
    public class GpsDelegate : IGpsDelegate
    {
        IGpsListener _gpsListener;

        public GpsDelegate()
        {
            //   _gpsListener = gpsListener;
        }

        public async Task OnReading(IGpsReading reading)
        {
            var notifications = ShinyHost.Resolve<INotificationManager>();
            var access = await notifications.RequestAccess();
            if (access == AccessState.Available)
            {
                Debug.WriteLine(reading.Position);
               // await notifications.Send(new Notification());
            }
        }
    }
}
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
using Agrirouter.iOS.Services;
using Agrirouter.Services;
using CoreLocation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationService))]
namespace Agrirouter.iOS.Services
{
    public class LocationService : ILocationService
    {
        protected CLLocationManager locationManager;

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public LocationService()
        {
            locationManager = new CLLocationManager();
            locationManager.PausesLocationUpdatesAutomatically = false;

            // iOS 8 has additional permissions requirements
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locationManager.RequestAlwaysAuthorization(); // works in background
                //locMgr.RequestWhenInUseAuthorization (); // only in foreground
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locationManager.AllowsBackgroundLocationUpdates = true;
            }
        }

        public void StartLocationUpdates()
        {
            // We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
            // the popover when the app is first launched, or by changing the permissions for the app in Settings
            if (CLLocationManager.LocationServicesEnabled)
            {
                LocationManager.DesiredAccuracy = 1;

                LocationManager.LocationsUpdated += (sender, e) =>
                {
                    LocationUpdated?.Invoke(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                };

                LocationManager.StartUpdatingLocation();
            }
        }
        
        public CLLocationManager LocationManager => locationManager;
    }

    public class LocationUpdatedEventArgs : EventArgs
    {
        private CLLocation _location;

        public LocationUpdatedEventArgs(CLLocation location)
        {
            _location = location;
        }

        public CLLocation Location => _location;
    }
}
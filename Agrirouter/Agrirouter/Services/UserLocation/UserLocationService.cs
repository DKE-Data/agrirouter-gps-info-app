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
using System.Timers;
using Agrirouter.Services.UserLocation.Models;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using Timer = System.Timers.Timer;

namespace Agrirouter.Services.UserLocation
{
    public class UserLocationService : IUserLocationService
    {
        private readonly Timer _checkLocationTimer;

        private const int UpdateInterval = 3000;

        public UserLocationService()
        {
            _checkLocationTimer = new Timer(UpdateInterval);
            _checkLocationTimer.Elapsed += OnCheckLocation;
            _checkLocationTimer.AutoReset = true;
        }

        public event EventHandler<LocationChangedEventArgs> OnLocationChanged;

        public async Task<Location> GetCurrentLocation(bool isGetLastKnownLocation)
        {
            Location location = null;

            try
            {
                if (isGetLastKnownLocation)
                {
                    location = await Geolocation.GetLastKnownLocationAsync();
                }

                if (location is null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    var cancellationTokenSource = new CancellationTokenSource();
                    location = await Geolocation.GetLocationAsync(request, cancellationTokenSource.Token);
                }

                return location;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }

            return location;
        }

        public void SetLocationAutoChecking(bool isEnabled)
        {
            _checkLocationTimer.Enabled = isEnabled;
        }

        private async void OnCheckLocation(object sender, ElapsedEventArgs e)
        {
            var location = await GetCurrentLocation(true);
            OnLocationChanged?.Invoke(this, new LocationChangedEventArgs(location));
        }
    }
}
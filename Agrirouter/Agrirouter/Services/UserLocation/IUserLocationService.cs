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
using System.Threading.Tasks;
using Agrirouter.Services.UserLocation.Models;
using Xamarin.Essentials;

namespace Agrirouter.Services.UserLocation
{
    public interface IUserLocationService
    {
        Task<Location> GetCurrentLocation(bool isGetLastKnownLocation);

        void SetLocationAutoChecking(bool isEnabled);
        
        event EventHandler<LocationChangedEventArgs> OnLocationChanged;
    }
}
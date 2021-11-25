/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium f�r Ern�hrung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using System;
using Xamarin.Essentials;

namespace Agrirouter.Services.UserLocation.Models
{
    public class LocationChangedEventArgs: EventArgs
    {
        public Location Location { get; }

        public LocationChangedEventArgs(Location location)
        {
            Location = location;
        }
    }
}
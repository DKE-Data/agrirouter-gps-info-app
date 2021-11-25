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
using Shiny.Locations;

namespace Agrirouter.Services.Shiny.Listeners
{
    public class GpsListener : IGpsListener
    {
        public event EventHandler<GpsReadingEventArgs> OnReadingReceived;

        public void UpdateReading(IGpsReading reading)
        {
            OnReadingReceived?.Invoke(this, new GpsReadingEventArgs(reading));
        }
    }
}
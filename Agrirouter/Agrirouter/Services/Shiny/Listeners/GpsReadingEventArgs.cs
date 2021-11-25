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
using Shiny.Locations;

namespace Agrirouter.Services.Shiny.Listeners
{
    public class GpsReadingEventArgs : EventArgs
    {
        public IGpsReading Reading { get; }

        public GpsReadingEventArgs(IGpsReading reading)
        {
            Reading = reading;
        }

    }
}
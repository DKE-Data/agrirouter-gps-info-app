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

namespace Agrirouter.Services.Shiny.Listeners
{
    public interface IGpsListener
    {
        event EventHandler<GpsReadingEventArgs> OnReadingReceived;
    }
}
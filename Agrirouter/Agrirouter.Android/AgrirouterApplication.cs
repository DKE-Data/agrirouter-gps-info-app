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
using Agrirouter.Services.Shiny;
using Android.App;
using Android.Runtime;
using Shiny;

namespace Agrirouter.Droid
{
    [Application]
    public class AgrirouterApplication : ShinyAndroidApplication<Startup>
    {
        public AgrirouterApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }
    }
}
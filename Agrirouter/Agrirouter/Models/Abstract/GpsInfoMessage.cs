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
using Agrirouter.Technicalmessagetype;

namespace Agrirouter.Models.Abstract
{
    public class GpsInfoMessage : MessageModel<GPSList>
    {
        public GpsInfoMessage()
        {
            Data = new GPSList();
        }
        
        public GpsInfoMessage(Guid id, DateTime dateTime)
        {
            Id = id;
            CreatingDate = dateTime;
            Data = new GPSList();
        }
    }
}
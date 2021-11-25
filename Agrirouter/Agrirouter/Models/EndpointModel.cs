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

namespace Agrirouter.Models
{
    public class EndpointModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }

        public bool HasPosition => Longitude != 0 || Latitude != 0; 
        
        public DateTime LastUpdateDateTime { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsVisible { get; set; }
    }
}
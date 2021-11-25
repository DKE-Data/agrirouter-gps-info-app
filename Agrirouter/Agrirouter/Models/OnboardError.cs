/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Agrirouter.Models
{
    public class OnboardError
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("details")]
        public List<object> Details { get; set; }   
    }
    
    public class RootOnboardError
    {
        [JsonProperty("error")]
        public OnboardError Error { get; set; }
    }
}
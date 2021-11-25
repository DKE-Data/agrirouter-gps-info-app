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
using Agrirouter.Common;

namespace Agrirouter.Models
{
    public class ExportInformationModel
    {
        public int? NotSentMessagesCount { get; set; }
        
        public int? NotSentEntriesCount { get; set; }
        
        public DateTime? LastExportDateTime { get; set; }

        public string LastExportDateTimeString => LastExportDateTime?.ToString(Constants.DateTimeFormat);
    }
}
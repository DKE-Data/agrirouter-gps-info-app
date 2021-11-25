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

namespace Agrirouter.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static int ConvertToUnixTimestamp(this DateTime date)
        {
            return (int)(date - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
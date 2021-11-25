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
    public static class EnumExtensions
    {
        public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
        {
            if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
            {
                return defaultValue;
            }

            return (TEnum) Enum.Parse(typeof(TEnum), strEnumValue);
        }
    }
}
/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System.IO;
using System.Reflection;

namespace Agrirouter.Resources
{
    public static class AppResources
    {
        private static string ReadResource(string fileName)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppResources)).Assembly;
            var stream = assembly.GetManifestResourceStream($"{nameof(Agrirouter)}.{nameof(Resources)}.{fileName}");
            var result = string.Empty;

            if (stream is null)
            {
                return result;
            }
            
            using (var reader = new StreamReader(stream))
            {  
                result = reader.ReadToEnd ();
            }

            return result;
        }

        public static string About => ReadResource("About.html");
    }
}
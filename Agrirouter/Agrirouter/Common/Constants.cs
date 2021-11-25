/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

namespace Agrirouter.Common
{
    public static class Constants
    {
        public const int GpsDataSendingPeriod = 5000;

        public const string ApiUrl = "";

        public const string CertificationType = "P12";

        public const string GatewayId = "3";

        public const int EndpointMaxActiveTime = 10;

        public const int EndpointRemoveTime = 15;

        public const string DateTimeFormat = "dd.MM.yyyy HH:mm";
        
        public static class LocalizationConstants
        {
            public const string Default = "default";
            
            public const string EnUs = "en-US";
            
            public const string DeDe = "de-DE";
        }

        public static class QualityAssurance
        {
            public const string ApplicationId = "{Enter UUID here}";

            public const string CertificationVersionId = "{Enter UUID here}";
        }
        
        public static class Production
        {
            public const string ApplicationId = "{Enter UUID here}";

            public const string CertificationVersionId = "{Enter UUID here}";
        }
    }
}
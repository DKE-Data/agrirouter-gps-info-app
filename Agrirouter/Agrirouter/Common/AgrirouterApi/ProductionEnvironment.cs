/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using static Agrirouter.Common.Constants;

namespace Agrirouter.Common.AgrirouterApi
{
    public class ProductionEnvironment : BaseEnvironment
    {
        protected override string ApiPrefix() => "/api/v1.0";

        protected override string RegistrationServiceUrl() => "https://onboard.my-agrirouter.com";

        protected override string AuthorizationServiceUrl() => "https://goto.my-agrirouter.com";
        
        public override string PublicKey()
        {
            return string.Empty;
        }

        public override string ApplicationId() => Production.ApplicationId;

        public override string CertificationVersionId() => Production.CertificationVersionId;
    }
}
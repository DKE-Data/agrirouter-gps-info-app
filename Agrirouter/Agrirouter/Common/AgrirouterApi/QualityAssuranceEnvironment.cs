/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

namespace Agrirouter.Common.AgrirouterApi
{
    public class QualityAssuranceEnvironment : BaseEnvironment
    {
        protected override string ApiPrefix() => "/api/v1.0";

        protected override string RegistrationServiceUrl() => "https://agrirouter-registration-service-hubqa-eu10.cfapps.eu10.hana.ondemand.com";

        protected override string AuthorizationServiceUrl() => "https://agrirouter-qa.cfapps.eu10.hana.ondemand.com";

        public override string PublicKey() => "-----BEGIN PUBLIC KEY-----\n\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAy8xF9661acn+iS+QS+9Y\n\n3HvTfUVcismzbuvxHgHA7YeoOUFxyj3lkaTnXm7hzQe4wDEDgwpJSGAzxIIYSUXe\n\n8EsWLorg5O0tRexx5SP3+kj1i83DATBJCXP7k+bAF4u2FVJphC1m2BfLxelGLjzx\n\nVAS/v6+EwvYaT1AI9FFqW/a2o92IsVPOh9oM9eds3lBOAbH/8XrmVIeHofw+XbTH\n\n1/7MLD6IE2+HbEeY0F96nioXArdQWXcjUQsTch+p0p9eqh23Ak4ef5oGcZhNd4yp\n\nY8M6ppvIMiXkgWSPJevCJjhxRJRmndY+ajYGx7CLePx7wNvxXWtkng3yh+7WiZ/Y\n\nqwIDAQAB\n\n-----END PUBLIC KEY-----";

        public override string ApplicationId() => Constants.QualityAssurance.ApplicationId;

        public override string CertificationVersionId() => Constants.QualityAssurance.CertificationVersionId;
    }
}
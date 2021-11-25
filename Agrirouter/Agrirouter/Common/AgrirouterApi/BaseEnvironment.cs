/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using Agrirouter.Api.Env;

namespace Agrirouter.Common.AgrirouterApi
{
    public abstract class BaseEnvironment : Environment
    {
        public abstract string ApplicationId();

        public abstract string CertificationVersionId();
    }
}
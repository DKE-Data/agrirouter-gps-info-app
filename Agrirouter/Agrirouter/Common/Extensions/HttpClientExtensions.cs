/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System.Net.Http;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Services.Rest.Handlers;
using ModernHttpClient;

namespace Agrirouter.Common.Extensions
{
    public class HttpClientFactory
    {
        public static HttpClient AuthenticatedHttpClient(OnboardResponse onboardResponse)
        {
            var httpClientHandler = new NativeMessageHandler(false, new TLSConfig
            {
                ClientCertificate = new ClientCertificate()
                {
                    RawData = onboardResponse.Authentication.Certificate,
                    Passphrase = onboardResponse.Authentication.Secret
                }
            });
            
            return new HttpClient(new LoggingMessageHandler(httpClientHandler));
        }
    }
}
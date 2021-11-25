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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Agrirouter.Services.Rest.Handlers
{
    public class AuthenticatedHandler : HttpClientHandler
    {
        private readonly string _token;

        public AuthenticatedHandler(string token)
        {
            _token = token;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var auth = request.Headers.Authorization;
            if (auth != null && _token != null)
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, _token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
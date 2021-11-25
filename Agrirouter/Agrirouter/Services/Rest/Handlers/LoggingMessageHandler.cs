/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Agrirouter.Services.Rest.Handlers
{
    public class LoggingMessageHandler : DelegatingHandler
    {
        public LoggingMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler ?? new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("Request:");
            Debug.WriteLine(request.ToString());
            Debug.WriteLine(request.Headers.ToString());
            if (request.Content != null)
            {
                Debug.WriteLine(await request.Content.ReadAsStringAsync());
            }

            var response = await base.SendAsync(request, cancellationToken);

            Debug.WriteLine("Response:");
            Debug.WriteLine(response.ToString());
            if (response.Content == null)
                return response;

            var bytes = await response.Content.ReadAsByteArrayAsync();
            Debug.WriteLine(bytes.Length);
            Debug.WriteLine(await response.Content.ReadAsStringAsync());

            return response;
        }
    }
}
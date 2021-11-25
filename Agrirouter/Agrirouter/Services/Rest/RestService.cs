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
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Agrirouter.Common;
using Agrirouter.Services.Rest.Abstract;
using Agrirouter.Services.Rest.Handlers;

namespace Agrirouter.Services.Rest
{
    public class RestService : IRestService
    {
        public IAgrirouterApi WebApi { get; private set; }
        private HttpClient _client;

        public RestService()
        {
            //UpdateToken(UserSettings.Token);
        }

        protected void UpdateToken(string token = "")
        {
            if (token == null)
            {
                token = string.Empty;
            }

            try
            {
#if (DEBUG || Test)
                _client = new HttpClient(new LoggingMessageHandler(new AuthenticatedHandler(token)));
#else
                _client = new HttpClient(new AuthenticatedHandler(token));
#endif
                _client.BaseAddress = new Uri(Constants.ApiUrl);

                WebApi = Refit.RestService.For<IAgrirouterApi>(_client);
            }
            catch (Exception ex)
            {
#if (DEBUG || Test)
                Debug.WriteLine("TODO Handle Exception:" + ex.Message);
#endif
            }
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                await action.Invoke().ConfigureAwait(false);
            }
            catch (Exception e)
            {
#if (DEBUG || Test)
                throw e;
#endif
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
        {
            TResult result = default;

            if (action == null)
            {
                return result;
            }

            try
            {
                result = await action.Invoke();
            }
            catch (Exception e)
            {
#if (DEBUG || Test)
                throw e;
#endif
            }

            return result;
        }
    }
}
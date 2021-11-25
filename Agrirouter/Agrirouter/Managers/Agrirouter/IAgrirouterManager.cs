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
using System.Net.Http;
using System.Threading.Tasks;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Common.AgrirouterApi;
using Agrirouter.Models;
using Environment = Agrirouter.Api.Env.Environment;

namespace Agrirouter.Managers.Agrirouter
{
    public interface IAgrirouterManager
    {
        Task<bool> Authorize(string registrationCode, BaseEnvironment environment);
        
        Task<bool> UnAuthorize();
        
        Task<bool> CheckIfAuthorized();
        
        Task<HttpClient> GetAuthorizedHttpClient();

        Task<OnboardResponse> GetOnboardResponse();

        string getLastOnboardingErrorText();

        Task SendCapability();

        Task SendGpsPosition();

        Task RecordGpsPosition();

        Task SendSubscriptions();

        void StartReadOutbox();
        
        void StopReadOutbox();
        
        Task ReadOutbox();

        Task CheckOutbox(bool silentMode = false);
        
        bool HasPendingMessages();

        Task SendRequestEndpointsList();

        Task DeleteOldMessagesFromFeed();

        Task SendRequestMessagesFromFeed();

        Task<bool> SafeExecuteHandler(Func<Task> action, int tryingCount = 20);
    }
}
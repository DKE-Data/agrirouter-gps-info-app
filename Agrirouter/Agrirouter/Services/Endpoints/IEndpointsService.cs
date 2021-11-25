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
using System.Collections.Generic;
using System.Threading.Tasks;
using Agrirouter.Models;
using Agrirouter.Response.Payload.Account;
using Agrirouter.Technicalmessagetype;

namespace Agrirouter.Services.Endpoints
{
    public interface IEndpointsService
    {
        event EventHandler<List<EndpointModel>> OnEndpointsChanged; 
        
        event EventHandler OnEndpointNotFound;
        
        void UpdateGpsPositions(Guid endpointId, GPSList gpsList);

        Task UpdateEndpointsList(ListEndpointsResponse response);

        Task<List<EndpointModel>> GetEndpoints();

        void ClearEndpoints();
        
        void UpdateEndpointsStatus();
    }
}
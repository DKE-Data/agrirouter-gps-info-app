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
using System.Linq;
using System.Threading.Tasks;
using Agrirouter.Common;
using Agrirouter.Models;
using Agrirouter.Repositories.Endpoints;
using Agrirouter.Response.Payload.Account;
using Agrirouter.Technicalmessagetype;

namespace Agrirouter.Services.Endpoints
{
    public class EndpointsService : IEndpointsService
    {
        private readonly IEndpointsRepository _endpointsRepository;
        private List<EndpointModel> _endpoints;

        public EndpointsService(IEndpointsRepository endpointsRepository)
        {
            _endpointsRepository = endpointsRepository;
            _endpoints = new List<EndpointModel>();
        }

        public event EventHandler<List<EndpointModel>> OnEndpointsChanged;

        public event EventHandler OnEndpointNotFound;

        public void UpdateGpsPositions(Guid endpointId, GPSList gpsList)
        {
            if (gpsList is null)
            {
                return;
            }

            var selectedEndpoint = _endpoints.FirstOrDefault(endpoint => endpoint.Id == endpointId);
            if (selectedEndpoint is null)
            {
                OnEndpointNotFound?.Invoke(this, new EventArgs());
                return;
            }

            foreach( var gpsPosition in gpsList.GpsEntries)
            {
                if (gpsPosition.GpsUtcTimestamp != null)
                {
                    if (DateTime.Compare(gpsPosition.GpsUtcTimestamp.ToDateTime(), selectedEndpoint.LastUpdateDateTime) > 0 )
                    {
                        selectedEndpoint.LastUpdateDateTime = gpsPosition.GpsUtcTimestamp.ToDateTime();
                        selectedEndpoint.Latitude = gpsPosition.PositionNorth;
                        selectedEndpoint.Longitude = gpsPosition.PositionEast;
                    }
                }
            }

            _endpointsRepository.SetAsync(_endpoints);

            OnEndpointsChanged?.Invoke(this, _endpoints);
        }

        public void ClearEndpoints()
        {
            _endpointsRepository.Clear();
        }

        public void UpdateEndpointsStatus()
        {
            foreach (var endpoint in _endpoints)
            {
                TimeSpan diff = (DateTime.UtcNow - endpoint.LastUpdateDateTime);
                endpoint.IsActive = diff.TotalMinutes < Constants.EndpointMaxActiveTime;
                endpoint.IsVisible = diff.TotalMinutes < Constants.EndpointRemoveTime;
            }

            OnEndpointsChanged?.Invoke(this, _endpoints);
        }

        public async Task UpdateEndpointsList(ListEndpointsResponse response)
        {
            if (response is null)
            {
                return;
            }

            var savedEndpoints = await _endpointsRepository.GetAsync();

            _endpoints.Clear();

            foreach (var endpoint in response.Endpoints)
            {
                var newEndpoint = new EndpointModel
                {
                    Id = Guid.Parse(endpoint.EndpointId),
                    Name = endpoint.EndpointName,
                    Type = endpoint.EndpointType
                };

                var selectedEndpoint = savedEndpoints.FirstOrDefault(e => e.Id.ToString() == endpoint.EndpointId);
                if (selectedEndpoint != null)
                {
                    newEndpoint.Latitude = selectedEndpoint.Latitude;
                    newEndpoint.Longitude = selectedEndpoint.Longitude;
                    newEndpoint.LastUpdateDateTime = selectedEndpoint.LastUpdateDateTime;
                    newEndpoint.IsActive = selectedEndpoint.IsActive;
                }

                _endpoints.Add(newEndpoint);
            }

            await _endpointsRepository.SetAsync(_endpoints);
        }

        public Task<List<EndpointModel>> GetEndpoints()
        {
            return _endpointsRepository.GetAsync();
        }
    }
}
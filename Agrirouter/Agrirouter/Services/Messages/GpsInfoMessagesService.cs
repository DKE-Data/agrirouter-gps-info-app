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
using Agrirouter.Models;
using Agrirouter.Models.Abstract;
using Agrirouter.Repositories.Messages;
using Agrirouter.Repositories.StatusInformation;
using Agrirouter.Technicalmessagetype;
using Plugin.SimpleLogger;

namespace Agrirouter.Services.Messages
{
    public class GpsInfoMessagesService : IGpsInfoMessagesService
    {
        private const int MaxEntriesCountInMessage = 500;

        public IGpsInfoMessagesRepository GpsInfoMessagesRepository { get; }
        public IStatusInformationRepository StatusInformationRepository { get; }

        public GpsInfoMessagesService(IGpsInfoMessagesRepository gpsInfoMessagesRepository, IStatusInformationRepository statusInformationRepository)
        {
            StatusInformationRepository = statusInformationRepository;
            GpsInfoMessagesRepository = gpsInfoMessagesRepository;
        }

        public async Task AddGpsEntry(GPSList.Types.GPSEntry entry)
        {
            var messages = await GetMessages().ConfigureAwait(false);
            if (messages.Any())
            {
                var lastMessage = messages.LastOrDefault();
                if (lastMessage is null)
                {
                    return;
                }

                if (lastMessage.Data.GpsEntries.Count < MaxEntriesCountInMessage)
                {
                    lastMessage.Data.GpsEntries.Add(entry);
                }
                else
                {
                    var message = new GpsInfoMessage(Guid.NewGuid(), DateTime.UtcNow);
                    message.Data.GpsEntries.Add(entry);
                    messages.Add(message);
                }
            }
            else
            {
                var message = new GpsInfoMessage(Guid.NewGuid(), DateTime.UtcNow);
                message.Data.GpsEntries.Add(entry);
                messages.Add(message);
            }

            CrossSimpleLogger.Current.Info($"Added GPS Entry: Longitude: {entry.PositionEast}, Latitude {entry.PositionNorth}");

            await GpsInfoMessagesRepository.SetAsync(messages);
        }


        public async Task<List<GpsInfoMessage>> GetMessages()
        {
            var messages = await GpsInfoMessagesRepository.GetAsync();
            return messages.Where(message => message.Data.GpsEntries.Any()).ToList();
        }

        public async Task<ExportInformationModel> GetExportInformation()
        {
            var exportInformation = new ExportInformationModel();

            var messages = await GpsInfoMessagesRepository.GetAsync();
            var statusInformation = await StatusInformationRepository.GetAsync();

            exportInformation.LastExportDateTime = statusInformation.LastExportDateTime;
            exportInformation.NotSentMessagesCount = messages.Count;
            exportInformation.NotSentEntriesCount = messages.Sum(message => message.Data.GpsEntries.Count);
            return exportInformation;
        }

        public void ClearList()
        {
            GpsInfoMessagesRepository.Clear();
        }

        public async Task<bool> RemoveMessage(Guid id)
        {
            var messages = await GpsInfoMessagesRepository.GetAsync().ConfigureAwait(false);
            if (messages is null)
            {
                return false;
            }

            var message = messages.FirstOrDefault(item => item.Id == id);
            var result = messages.Remove(message);
            if (result)
            {
                await GpsInfoMessagesRepository.SetAsync(messages);
                CrossSimpleLogger.Current.Info($"RemoveMessage: Id: {id}");
                return true;
            }

            return false;
        }
    }
}
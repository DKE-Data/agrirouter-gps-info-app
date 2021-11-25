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
using Agrirouter.Models.Abstract;
using Agrirouter.Repositories.Messages;
using Agrirouter.Repositories.StatusInformation;
using Agrirouter.Technicalmessagetype;

namespace Agrirouter.Services.Messages
{
    public interface IGpsInfoMessagesService
    {
        IGpsInfoMessagesRepository GpsInfoMessagesRepository { get; }
        
        IStatusInformationRepository StatusInformationRepository { get; }
        
        Task AddGpsEntry(GPSList.Types.GPSEntry entry);

        Task<List<GpsInfoMessage>> GetMessages();

        Task<bool> RemoveMessage(Guid id);


        void ClearList();

        Task<ExportInformationModel> GetExportInformation();
    }
}
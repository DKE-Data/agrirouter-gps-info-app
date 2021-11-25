/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System.Collections.Generic;
using Agrirouter.Models.Abstract;
using Agrirouter.Repositories.Abstract;

namespace Agrirouter.Repositories.Messages
{
    public interface IGpsInfoMessagesRepository : IRepository<List<GpsInfoMessage>>
    {
        
    }
}
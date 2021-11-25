/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using Agrirouter.Models;
using Agrirouter.Repositories.Abstract;

namespace Agrirouter.Repositories.StatusInformation
{
    public interface IStatusInformationRepository : IRepository<StatusInformationModel>
    {
        
    }
}
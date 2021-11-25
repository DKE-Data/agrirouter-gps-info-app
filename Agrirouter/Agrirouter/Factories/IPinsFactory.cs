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
using System.Windows.Input;
using Agrirouter.Models;
using Agrirouter.UI.Controls;

namespace Agrirouter.Factories
{
    public interface IPinsFactory
    {
        IEnumerable<EndpointPin> ProducePins(IEnumerable<EndpointModel> endpointModels, ICommand tappedCommand);
    }
}
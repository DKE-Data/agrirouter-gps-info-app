/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using Agrirouter.Common.AgrirouterApi;

namespace Agrirouter.ViewModels.Items
{
    public class EnvironmentViewModel
    {
        public EnvironmentViewModel(string name, BaseEnvironment environment)
        {
            Name = name;
            Environment = environment;
        }
        
        public string Name { get; }
        
        public BaseEnvironment Environment { get; }
    }
}
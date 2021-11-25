/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
namespace Agrirouter.Services.Localization
{
    public class LanguageModel
    {
        public LanguageModel(string name, string culture)
        {
            Name = name;
            Culture = culture;
        }

        public string Name { get; }
        
        public string Culture { get; }
    }
}
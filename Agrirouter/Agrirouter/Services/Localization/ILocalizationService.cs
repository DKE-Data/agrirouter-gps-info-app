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
    public interface ILocalizationService
    {
        LanguageModel[] SupportedCultures { get; }

        string GetCultureByLanguageName(string languageName);

        string GetLanguageNameByCulture(string culture);

        void InitApplicationCulture();

        void ChangeCulture(string languageName);

        string GetSystemCulture();
    }
}
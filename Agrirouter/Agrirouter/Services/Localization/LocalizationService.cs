/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using System.Globalization;
using System.Linq;
using Agrirouter.Common;
using Agrirouter.Repositories.UserSettings;
using Agrirouter.Resources.Localization;

namespace Agrirouter.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        private string _defaultCulture = Constants.LocalizationConstants.EnUs;
        private string _defaultLanguage = Strings.English;

        public LocalizationService(IUserSettingsRepository userSettingsRepository)
        {
            _userSettingsRepository = userSettingsRepository;
        }

        public LanguageModel[] SupportedCultures => new[]
        {
            new LanguageModel(Strings.English, Constants.LocalizationConstants.EnUs),
            new LanguageModel(Strings.Deutsch, Constants.LocalizationConstants.DeDe),
        };

        public async void ChangeCulture(string languageName)
        {
            var language = SupportedCultures.FirstOrDefault(culture => culture.Name == languageName);
            if (language is null)
            {
                return;
            }

            var currentCulture = CultureInfo.GetCultureInfo(language.Culture);

            Strings.Culture = currentCulture;

            var userSettings = await _userSettingsRepository.GetAsync();
            userSettings.CurrentCulture = currentCulture.Name;
            await _userSettingsRepository.SetAsync(userSettings);
        }

        public string GetCultureByLanguageName(string languageName)
        {
            var language = SupportedCultures.FirstOrDefault(culture => culture.Name == languageName);
            if (language is null)
            {
                return null;
            }

            return language.Culture;
        }

        public string GetLanguageNameByCulture(string cultureName)
        {
            if (cultureName == Constants.LocalizationConstants.Default)
            {
                return _defaultLanguage;
            }

            var language = SupportedCultures.FirstOrDefault(culture => culture.Culture == cultureName);
            if (language is null)
            {
                return null;
            }

            return language.Name;
        }

        public string GetSystemCulture()
        {
            var systemCulture = CultureInfo.CurrentCulture.Name;
            if (!SupportedCultures.Any(culture => culture.Culture == systemCulture))
            {
                return _defaultCulture;
            }

            return systemCulture;
        }

        public async void InitApplicationCulture()
        {
            var userSettings = await _userSettingsRepository.GetAsync();

            var culture = userSettings.CurrentCulture;

            if (culture == Constants.LocalizationConstants.Default)
            {
                culture = GetSystemCulture();
            }

            Strings.Culture = CultureInfo.GetCultureInfo(culture);
        }
    }
}
using UnityEngine;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for handling game localization and translations.
    /// Replaces LocalizationSystem singleton.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the localized text value for a given key.
        /// </summary>
        string GetLocalizedValue(string key);

        /// <summary>
        /// Sets the current language for localization.
        /// </summary>
        void SetLanguage(SystemLanguage language);

        /// <summary>
        /// Gets the currently active language.
        /// </summary>
        SystemLanguage GetCurrentLanguage();

        /// <summary>
        /// Checks if a localization key exists in the current language.
        /// </summary>
        bool HasKey(string key);
    }
}

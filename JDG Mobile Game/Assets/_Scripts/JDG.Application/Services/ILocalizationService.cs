namespace JDG.Application.Services
{
    /// <summary>
    /// Supported languages for localization.
    /// </summary>
    public enum GameLanguage
    {
        Unknown,
        French,
        English,
        Spanish,
        German,
        Japanese
    }

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
        void SetLanguage(GameLanguage language);

        /// <summary>
        /// Gets the currently active language.
        /// </summary>
        GameLanguage GetCurrentLanguage();

        /// <summary>
        /// Checks if a localization key exists in the current language.
        /// </summary>
        bool HasKey(string key);

        #region Card Localization (Phase 126)

        /// <summary>
        /// Gets the localized title for a card by its ID.
        /// Returns null if not found (caller should use fallback).
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        /// <param name="cardId">The card identifier (e.g., "alpha-man").</param>
        /// <returns>Localized title or null if not found.</returns>
        string GetCardTitle(string cardId);

        /// <summary>
        /// Gets the localized description for a card by its ID.
        /// Returns null if not found (caller should use fallback).
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        /// <param name="cardId">The card identifier (e.g., "alpha-man").</param>
        /// <returns>Localized description or null if not found.</returns>
        string GetCardDescription(string cardId);

        /// <summary>
        /// Gets the localized detailed description for a card by its ID.
        /// Returns null if not found (caller should use fallback).
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        /// <param name="cardId">The card identifier (e.g., "alpha-man").</param>
        /// <returns>Localized detailed description or null if not found.</returns>
        string GetCardDetailedDescription(string cardId);

        /// <summary>
        /// Checks if card localization exists for the given card ID.
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        /// <param name="cardId">The card identifier (e.g., "alpha-man").</param>
        /// <returns>True if card has localization data, false otherwise.</returns>
        bool HasCardLocalization(string cardId);

        #endregion
    }
}

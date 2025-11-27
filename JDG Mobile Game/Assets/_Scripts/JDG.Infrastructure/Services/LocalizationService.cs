using JDG.Application.Services;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of ILocalizationService.
    /// Wraps the existing LocalizationSystem singleton during migration.
    /// Uses Strangler Fig pattern - delegates to LocalizationSystem.Instance temporarily.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly LocalizationSystem _localizationSystem;
        private GameLanguage _currentLanguage;

        public LocalizationService()
        {
            // During migration, get the existing singleton
            // TODO: Later, inject localization dependencies directly
            _localizationSystem = LocalizationSystem.Instance;
            _currentLanguage = GameLanguage.French; // Default from original system
        }

        public string GetLocalizedValue(string key)
        {
            // For now, we need to convert string keys to the enum-based system
            // Try to parse the string as a LocalizationKeys enum
            if (System.Enum.TryParse<LocalizationKeys>(key, true, out var enumKey))
            {
                return _localizationSystem.GetLocalizedValue(enumKey);
            }
            else
            {
                Debug.LogWarning($"LocalizationService: Key '{key}' not found in LocalizationKeys enum");
                return $"[{key}]"; // Return key in brackets as fallback
            }
        }

        public void SetLanguage(GameLanguage language)
        {
            // LocalizationSystem currently only supports French (hardcoded)
            // Store for future use when proper multi-language support is added
            _currentLanguage = language;
            Debug.LogWarning($"LocalizationService: Multi-language support not yet implemented. Current: {language}");
        }

        public GameLanguage GetCurrentLanguage()
        {
            return _currentLanguage;
        }

        public bool HasKey(string key)
        {
            // Check if the key exists as a LocalizationKeys enum value
            return System.Enum.IsDefined(typeof(LocalizationKeys), key);
        }
    }
}

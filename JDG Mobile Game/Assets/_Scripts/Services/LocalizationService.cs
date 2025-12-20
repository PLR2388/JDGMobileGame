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
        private GameLanguage _currentLanguage;

        // Lazy access to LocalizationSystem.Instance - avoids constructor timing issues
        private LocalizationSystem LocalizationSystem => LocalizationSystem.Instance;

        public LocalizationService()
        {
            _currentLanguage = GameLanguage.French; // Default from original system
        }

        public string GetLocalizedValue(string key)
        {
            // Lazily access the singleton
            var locSystem = LocalizationSystem;
            if (locSystem == null)
            {
                Debug.LogWarning($"LocalizationService: LocalizationSystem.Instance not yet available for key '{key}'");
                return $"[{key}]";
            }

            // For now, we need to convert string keys to the enum-based system
            // Try to parse the string as a LocalizationKeys enum
            if (System.Enum.TryParse<LocalizationKeys>(key, true, out var enumKey))
            {
                return locSystem.GetLocalizedValue(enumKey);
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
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            return System.Enum.IsDefined(typeof(LocalizationKeys), key);
        }
    }
}

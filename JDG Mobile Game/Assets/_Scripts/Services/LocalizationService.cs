using System.Collections.Generic;
using JDG.Application.Services;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of ILocalizationService.
    /// Phase 8: Created to replace LocalizationSystem.Instance direct access.
    /// Phase 84: Now loads JSON directly (LocalizationSystem deleted).
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private Dictionary<string, string> _localizedText;
        private GameLanguage _currentLanguage;
        private const string MissingTextString = "Localized text not found";
        private const string DefaultLanguageFile = "Localization/fr";

        public LocalizationService()
        {
            _currentLanguage = GameLanguage.French;
            LoadLocalizedText(DefaultLanguageFile);
        }

        /// <summary>
        /// Loads localized text data from a JSON file.
        /// Phase 84: Moved from LocalizationSystem.
        /// </summary>
        /// <returns>True if file was loaded successfully, false otherwise.</returns>
        private bool LoadLocalizedText(string fileName)
        {
            TextAsset fileData = Resources.Load<TextAsset>(fileName);

            if (fileData == null)
            {
                Debug.LogWarning($"LocalizationService: Cannot find localization file: {fileName}");
                return false;
            }

            _localizedText = new Dictionary<string, string>();
            JObject jsonObject = JObject.Parse(fileData.text);
            ProcessJsonObject(jsonObject);

            Debug.Log($"LocalizationService: Loaded localization data from: {fileName}");
            return true;
        }

        /// <summary>
        /// Processes a JSON object containing localized text.
        /// Phase 84: Moved from LocalizationSystem.
        /// </summary>
        private void ProcessJsonObject(JObject jsonObject, string parentKey = "")
        {
            foreach (var property in jsonObject.Properties())
            {
                string currentKey = string.IsNullOrEmpty(parentKey)
                    ? property.Name
                    : $"{parentKey}.{property.Name}";

                if (property.Value.Type == JTokenType.Object)
                {
                    ProcessJsonObject((JObject)property.Value, currentKey);
                }
                else
                {
                    _localizedText[currentKey] = property.Value.ToString();
                }
            }
        }

        public string GetLocalizedValue(string key)
        {
            // Try to parse the string as a LocalizationKeys enum
            if (System.Enum.TryParse<LocalizationKeys>(key, true, out var enumKey))
            {
                return GetLocalizedValue(enumKey);
            }

            // Fallback: try direct key lookup
            if (_localizedText != null && _localizedText.TryGetValue(key, out var value))
            {
                return value;
            }

            Debug.LogWarning($"LocalizationService: Key '{key}' not found");
            return $"[{key}]";
        }

        /// <summary>
        /// Gets localized value using the enum key.
        /// Phase 84: Uses LocalizationKeyStrings for key mapping.
        /// </summary>
        public string GetLocalizedValue(LocalizationKeys enumKey)
        {
            string key = LocalizationKeyStrings.KeyMappings[enumKey];

            if (_localizedText != null && _localizedText.TryGetValue(key, out var value))
            {
                return value;
            }

            return MissingTextString;
        }

        public void SetLanguage(GameLanguage language)
        {
            // Load appropriate language file
            string fileName = language switch
            {
                GameLanguage.French => "Localization/fr",
                GameLanguage.English => "Localization/en",
                _ => DefaultLanguageFile
            };

            if (LoadLocalizedText(fileName))
            {
                _currentLanguage = language;
            }
            else
            {
                // Fall back to default language if requested file doesn't exist
                Debug.LogWarning($"LocalizationService: Falling back to {_currentLanguage} (requested: {language})");
                // Keep current language and dictionary unchanged
            }
        }

        public GameLanguage GetCurrentLanguage()
        {
            return _currentLanguage;
        }

        public bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            // Check enum first
            if (System.Enum.IsDefined(typeof(LocalizationKeys), key))
            {
                return true;
            }

            // Check direct key
            return _localizedText != null && _localizedText.ContainsKey(key);
        }
    }
}

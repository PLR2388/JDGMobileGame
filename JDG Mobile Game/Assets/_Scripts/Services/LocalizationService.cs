using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JDG.Application.Services;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Holds localized strings for a single card.
    /// Phase 126: Added for card multilanguage support.
    /// </summary>
    public class CardLocalizedStrings
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Detailed { get; set; }
    }

    /// <summary>
    /// Infrastructure implementation of ILocalizationService.
    /// Phase 8: Created to replace LocalizationSystem.Instance direct access.
    /// Phase 84: Now loads JSON directly (LocalizationSystem deleted).
    /// Phase 126: Added card-specific localization support.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private Dictionary<string, string> _localizedText;
        private Dictionary<string, CardLocalizedStrings> _cardStrings;
        private GameLanguage _currentLanguage;
        private const string MissingTextString = "Localized text not found";
        private const string DefaultLanguageFile = "Localization/fr";
        private const string DefaultCardLanguageFile = "Localization/cards_fr";

        public LocalizationService()
        {
            _currentLanguage = GameLanguage.French;
            _cardStrings = new Dictionary<string, CardLocalizedStrings>();
            LoadLocalizedText(DefaultLanguageFile);
            LoadCardLocalizedText(DefaultCardLanguageFile);
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

            // Phase 126: Also load card localization for the language
            string cardFileName = language switch
            {
                GameLanguage.French => "Localization/cards_fr",
                GameLanguage.English => "Localization/cards_en",
                GameLanguage.Spanish => "Localization/cards_es",
                GameLanguage.German => "Localization/cards_de",
                GameLanguage.Japanese => "Localization/cards_ja",
                _ => DefaultCardLanguageFile
            };

            if (LoadLocalizedText(fileName))
            {
                _currentLanguage = language;
                // Load card localizations (will fallback to French if file doesn't exist)
                LoadCardLocalizedText(cardFileName);
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

        #region Card Localization (Phase 126)

        /// <summary>
        /// Loads card localization data from a JSON file.
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        /// <param name="fileName">The file name without extension (e.g., "Localization/cards_fr").</param>
        /// <returns>True if file was loaded successfully, false otherwise.</returns>
        private bool LoadCardLocalizedText(string fileName)
        {
            TextAsset fileData = Resources.Load<TextAsset>(fileName);

            if (fileData == null)
            {
                // Card localization is optional - silently fall back to ScriptableObject text
                Debug.Log($"LocalizationService: Card localization file not found: {fileName} (using ScriptableObject fallback)");
                _cardStrings = new Dictionary<string, CardLocalizedStrings>();
                return false;
            }

            try
            {
                JObject jsonObject = JObject.Parse(fileData.text);
                JToken cardToken = jsonObject["card"];

                if (cardToken == null)
                {
                    Debug.LogWarning($"LocalizationService: Card localization file {fileName} missing 'card' root element");
                    _cardStrings = new Dictionary<string, CardLocalizedStrings>();
                    return false;
                }

                _cardStrings = new Dictionary<string, CardLocalizedStrings>();
                foreach (var property in ((JObject)cardToken).Properties())
                {
                    var cardId = property.Name;
                    var cardData = property.Value;

                    _cardStrings[cardId] = new CardLocalizedStrings
                    {
                        Title = cardData["title"]?.ToString(),
                        Description = cardData["description"]?.ToString(),
                        Detailed = cardData["detailed"]?.ToString()
                    };
                }

                Debug.Log($"LocalizationService: Loaded {_cardStrings.Count} card localizations from: {fileName}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LocalizationService: Error parsing card localization file {fileName}: {ex.Message}");
                _cardStrings = new Dictionary<string, CardLocalizedStrings>();
                return false;
            }
        }

        /// <summary>
        /// Generates a card ID from an asset name for localization lookup.
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        /// <param name="assetName">The ScriptableObject asset name (e.g., "Alpha Man").</param>
        /// <returns>Normalized card ID (e.g., "alpha-man").</returns>
        public static string GenerateCardId(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                return string.Empty;

            // Normalize to decompose accented characters, then filter out combining marks
            var normalized = assetName
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("!", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("?", "");
        }

        /// <inheritdoc />
        public string GetCardTitle(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
                return null;

            if (_cardStrings != null && _cardStrings.TryGetValue(cardId, out var strings))
                return strings.Title;

            return null; // Caller should use ScriptableObject fallback
        }

        /// <inheritdoc />
        public string GetCardDescription(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
                return null;

            if (_cardStrings != null && _cardStrings.TryGetValue(cardId, out var strings))
                return strings.Description;

            return null; // Caller should use ScriptableObject fallback
        }

        /// <inheritdoc />
        public string GetCardDetailedDescription(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
                return null;

            if (_cardStrings != null && _cardStrings.TryGetValue(cardId, out var strings))
                return strings.Detailed;

            return null; // Caller should use ScriptableObject fallback
        }

        /// <inheritdoc />
        public bool HasCardLocalization(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
                return false;

            return _cardStrings != null && _cardStrings.ContainsKey(cardId);
        }

        #endregion
    }
}

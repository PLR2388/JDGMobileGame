using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JDG.Editor
{
    /// <summary>
    /// Editor tool for exporting card localizations to JSON files.
    /// Phase 126c: Creates cards_fr.json from existing ScriptableObject card text.
    /// Uses SerializedObject to access card fields without direct type references.
    /// </summary>
    public static class CardLocalizationExporter
    {
        private const string CardsResourcePath = "Cards";
        private const string LocalizationPath = "Assets/Resources/Localization";

        /// <summary>
        /// Exports all card localizations to cards_fr.json.
        /// Accessible via Tools > Export Card Localizations menu.
        /// </summary>
        [MenuItem("Tools/Localization/Export Card Localizations (French)")]
        public static void ExportCardLocalizations()
        {
            var cards = Resources.LoadAll<ScriptableObject>(CardsResourcePath);

            if (cards.Length == 0)
            {
                Debug.LogError($"No cards found in Resources/{CardsResourcePath}");
                return;
            }

            // Build card strings dictionary
            var cardStrings = new Dictionary<string, object>();

            foreach (var card in cards)
            {
                var serializedObject = new SerializedObject(card);

                var titleProp = serializedObject.FindProperty("title");
                var descriptionProp = serializedObject.FindProperty("description");
                var detailedDescriptionProp = serializedObject.FindProperty("detailedDescription");

                // Skip if not a card (doesn't have expected properties)
                if (titleProp == null)
                    continue;

                var cardId = GenerateCardId(card.name);
                cardStrings[cardId] = new
                {
                    title = titleProp?.stringValue ?? "",
                    description = descriptionProp?.stringValue ?? "",
                    detailed = detailedDescriptionProp?.stringValue ?? ""
                };
            }

            // Create wrapper with "card" key for nested JSON structure
            var wrapper = new { card = cardStrings };

            // Serialize with proper formatting
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                StringEscapeHandling = StringEscapeHandling.Default
            };
            var json = JsonConvert.SerializeObject(wrapper, settings);

            // Ensure directory exists
            if (!Directory.Exists(LocalizationPath))
            {
                Directory.CreateDirectory(LocalizationPath);
            }

            // Write to file
            var outputPath = Path.Combine(LocalizationPath, "cards_fr.json");
            File.WriteAllText(outputPath, json, Encoding.UTF8);

            AssetDatabase.Refresh();

            Debug.Log($"Exported {cardStrings.Count} cards to {outputPath}");
        }

        /// <summary>
        /// Creates an empty template for English translations.
        /// </summary>
        [MenuItem("Tools/Localization/Create English Card Template")]
        public static void CreateEnglishTemplate()
        {
            var cards = Resources.LoadAll<ScriptableObject>(CardsResourcePath);

            if (cards.Length == 0)
            {
                Debug.LogError($"No cards found in Resources/{CardsResourcePath}");
                return;
            }

            // Build card strings dictionary with empty values for translation
            var cardStrings = new Dictionary<string, object>();

            foreach (var card in cards)
            {
                var serializedObject = new SerializedObject(card);
                var titleProp = serializedObject.FindProperty("title");

                // Skip if not a card
                if (titleProp == null)
                    continue;

                var cardId = GenerateCardId(card.name);
                cardStrings[cardId] = new
                {
                    title = "", // Empty for translator to fill
                    description = "",
                    detailed = ""
                };
            }

            var wrapper = new { card = cardStrings };

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(wrapper, settings);

            if (!Directory.Exists(LocalizationPath))
            {
                Directory.CreateDirectory(LocalizationPath);
            }

            var outputPath = Path.Combine(LocalizationPath, "cards_en.json");
            File.WriteAllText(outputPath, json, Encoding.UTF8);

            AssetDatabase.Refresh();

            Debug.Log($"Created English template with {cardStrings.Count} card entries at {outputPath}");
        }

        /// <summary>
        /// Validates that all cards have corresponding entries in cards_fr.json.
        /// </summary>
        [MenuItem("Tools/Localization/Validate Card Localizations")]
        public static void ValidateCardLocalizations()
        {
            var cards = Resources.LoadAll<ScriptableObject>(CardsResourcePath);
            var frJsonPath = Path.Combine(LocalizationPath, "cards_fr.json");

            if (!File.Exists(frJsonPath))
            {
                Debug.LogError($"cards_fr.json not found at {frJsonPath}. Run 'Export Card Localizations' first.");
                return;
            }

            var json = File.ReadAllText(frJsonPath);
            var root = JObject.Parse(json);
            var cardData = root["card"] as JObject;

            if (cardData == null)
            {
                Debug.LogError("Invalid JSON structure: missing 'card' object");
                return;
            }

            var missingCards = new List<string>();
            var emptyFields = new List<string>();
            var cardCount = 0;

            foreach (var card in cards)
            {
                var serializedObject = new SerializedObject(card);
                var titleProp = serializedObject.FindProperty("title");

                // Skip if not a card
                if (titleProp == null)
                    continue;

                cardCount++;
                var cardId = GenerateCardId(card.name);

                if (!cardData.ContainsKey(cardId))
                {
                    missingCards.Add($"{card.name} (id: {cardId})");
                    continue;
                }

                var cardEntry = cardData[cardId] as JObject;
                if (cardEntry == null) continue;

                if (string.IsNullOrEmpty(cardEntry["title"]?.ToString()))
                    emptyFields.Add($"{cardId}: title is empty");
                if (string.IsNullOrEmpty(cardEntry["description"]?.ToString()))
                    emptyFields.Add($"{cardId}: description is empty");
            }

            if (missingCards.Count > 0)
            {
                Debug.LogWarning($"Missing {missingCards.Count} cards in localization:\n" +
                    string.Join("\n", missingCards));
            }

            if (emptyFields.Count > 0)
            {
                Debug.LogWarning($"Empty fields found:\n" + string.Join("\n", emptyFields));
            }

            if (missingCards.Count == 0 && emptyFields.Count == 0)
            {
                Debug.Log($"Validation passed! All {cardCount} cards have complete localization entries.");
            }
        }

        /// <summary>
        /// Generates a normalized card ID from the asset name.
        /// Must match LocalizationService.GenerateCardId for consistency.
        /// </summary>
        /// <param name="assetName">The ScriptableObject asset name</param>
        /// <returns>Normalized card ID (lowercase, hyphens instead of spaces, no accents)</returns>
        public static string GenerateCardId(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                return "";

            // Normalize to decomposed form (separates base chars from accents)
            var normalized = assetName.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                // Skip combining marks (accents, diacritics)
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category == UnicodeCategory.NonSpacingMark)
                    continue;

                sb.Append(c);
            }

            // Convert back to composed form, then apply transformations
            return sb.ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace("\"", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "");
        }
    }
}

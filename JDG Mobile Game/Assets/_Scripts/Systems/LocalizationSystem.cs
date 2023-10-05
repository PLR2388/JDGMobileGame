using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Manages the localization of text resources based on the provided language.
/// </summary>
public class LocalizationSystem : StaticInstance<LocalizationSystem>
{
    private Dictionary<string, string> localizedText;
    private const string MissingTextString = "Localized text not found";

    /// <summary>
    /// Starts the localization system by loading the default language text.
    /// </summary>
    private void Start()
    {
        LoadLocalizedText("Localization/fr");
    }
    
    /// <summary>
    /// Loads localized text data from a file into the system.
    /// </summary>
    /// <param name="fileName">The name of the file containing the localized text data.</param>
    private void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        TextAsset fileData = Resources.Load<TextAsset>(fileName);

        if (fileData == null)
        {
            Debug.LogError("Cannot find localization file: " + fileName);
            return;
        }

        JObject jsonObject = JObject.Parse(fileData.text);

        ProcessJsonObject(jsonObject);

        Debug.Log("Loaded localization data from: " + fileName);
    }

    /// <summary>
    /// Processes a JSON object containing localized text, converting it into a usable format for the system.
    /// </summary>
    /// <param name="jsonObject">The JSON object containing localized text data.</param>
    /// <param name="parentKey">The parent key in the JSON structure, used for nested objects.</param>
    private void ProcessJsonObject(JObject jsonObject, string parentKey = "")
    {
        foreach (var property in jsonObject.Properties())
        {
            string currentKey = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}.{property.Name}";

            if (property.Value.Type == JTokenType.Object)
            {
                ProcessJsonObject((JObject)property.Value, currentKey);
            }
            else
            {
                localizedText.Add(currentKey, property.Value.ToString());
            }
        }
    }

    /// <summary>
    /// Retrieves the localized text associated with a specific key.
    /// </summary>
    /// <param name="enumKey">The key of the text to be localized.</param>
    /// <returns>The localized text corresponding to the provided key. If not found, returns a default "text not found" string.</returns>
    public string GetLocalizedValue(LocalizationKeys enumKey)
    {
        string key = LocalizationKeyStrings.KeyMappings[enumKey];
        string result = MissingTextString;
        if (localizedText.TryGetValue(key, out var value))
        {
            result = value;
        }

        return result;
    }
}
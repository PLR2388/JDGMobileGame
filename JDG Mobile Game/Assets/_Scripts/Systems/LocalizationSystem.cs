using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class LocalizationSystem : StaticInstance<LocalizationSystem>
{
    private Dictionary<string, string> localizedText;
    private static bool isInitialized = false;
    private const string missingTextString = "Localized text not found";

    private void Start()
    {
        LoadLocalizedText("Localization/fr");
    }
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

    public string GetLocalizedValue(LocalizationKeys enumKey)
    {
        string key = LocalizationKeyStrings.KeyMappings[enumKey];
        string result = missingTextString;
        if (localizedText.TryGetValue(key, out var value))
        {
            result = value;
        }

        return result;
    }

    public static bool IsInitialized()
    {
        return isInitialized;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class LocalizationSystem : StaticInstance<LocalizationSystem>
{
    private Dictionary<string, string> localizedText;
    private static bool isInitialized = false;
    private const string missingTextString = "Localized text not found";
    
    public void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        TextAsset fileData = Resources.Load<TextAsset>(fileName);

        if (fileData == null)
        {
            Debug.LogError("Cannot find localization file: " + fileName);
            return;
        }

        Dictionary<string, object> dataAsJson = JsonUtility.FromJson<Dictionary<string, object>>(fileData.ToString());

        foreach (var key in dataAsJson.Keys)
        {
            localizedText.Add(key, dataAsJson[key].ToString());
        }

        Debug.Log("Loaded localization data from: " + fileName);
    }

    public string GetLocalizedValue(string key)
    {
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

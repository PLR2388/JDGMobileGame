using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// Responsible for generating enums and associated mappings from JSON localization data.
/// </summary>
public class EnumGenerator: MonoBehaviour
{
    StringBuilder enumBuilder = new StringBuilder();
    StringBuilder mappingBuilder = new StringBuilder();

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        GenerateEnumFromJson();
    }
    
    /// <summary>
    /// Generates an enum and its associated mappings from the JSON localization data.
    /// </summary>
    private void GenerateEnumFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Localization/fr");
        JObject jsonObject = JObject.Parse(jsonFile.text);

        GenerateEnumAndMapping(jsonObject);
        SaveToFile();
    }

    /// <summary>
    /// Recursively generates enum values and associated mappings for a given JSON object.
    /// </summary>
    /// <param name="jsonObject">The JSON object to parse.</param>
    /// <param name="parentKey">The parent key (if any) for nested properties.</param>
    private void GenerateEnumAndMapping(JObject jsonObject, string parentKey = "")
    {
        foreach (var property in jsonObject.Properties())
        {
            string currentKey = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}.{property.Name}";

            if (property.Value.Type == JTokenType.Object)
            {
                GenerateEnumAndMapping((JObject)property.Value, currentKey);
            }
            else
            {
                enumBuilder.AppendLine($"    {currentKey.ToUpper().Replace(".", "_")},");
                mappingBuilder.AppendLine($"        {{ LocalizationKeys.{currentKey.ToUpper().Replace(".", "_")}, \"{currentKey}\" }},");
            }
        }
    }
    
    /// <summary>
    /// Saves the generated enum and its associated mappings to their respective .cs files.
    /// </summary>
    private void SaveToFile()
    {
        enumBuilder.Insert(0, "public enum LocalizationKeys\n{");
        enumBuilder.AppendLine("}");

        mappingBuilder.Insert(0, "using System.Collections.Generic;\n\npublic static class LocalizationKeyStrings\n{\n    public static Dictionary<LocalizationKeys, string> KeyMappings = new Dictionary<LocalizationKeys, string>\n    {");
        mappingBuilder.AppendLine("    };\n}");

        // Save these strings to .cs files or use them however you prefer.
        File.WriteAllText("Assets/_Scripts/Systems/Generated/LocalizationKeys.cs", enumBuilder.ToString());
        File.WriteAllText("Assets/_Scripts/Systems/Generated/LocalizationKeyStrings.cs", mappingBuilder.ToString());
    }

}
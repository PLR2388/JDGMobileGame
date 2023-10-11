using UnityEngine;

/// <summary>
/// Decodes a JSON-based scenario from a text asset into a Scenario object.
/// </summary>
public class ScenarioDecoder : MonoBehaviour
{
    /// <summary>
    /// The JSON text asset containing scenario data.
    /// </summary>
    public TextAsset jsonFile;

    /// <summary>
    /// Retrieves a Scenario object parsed from the associated JSON text asset.
    /// </summary>
    public Scenario Scenario
    {
        get
        {
            // Convert the JSON text into a JsonScenario object
            JsonScenario jsonScenario = JsonUtility.FromJson<JsonScenario>(jsonFile.text);

            // Convert the JsonScenario object into a Scenario object and return
            Scenario scenario = jsonScenario.ToScenario();
            return scenario;
        }
    }
}

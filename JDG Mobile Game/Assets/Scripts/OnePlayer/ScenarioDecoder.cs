using UnityEngine;

public class ScenarioDecoder : MonoBehaviour
{
    public TextAsset jsonFile;
    public DialogueObject dialogueObject;

    public Scenario Scenario
    {
        get
        {
            JsonScenario jsonScenario = JsonUtility.FromJson<JsonScenario>(jsonFile.text);

            Scenario scenario = jsonScenario.ToScenario();
            return scenario;
        }
    }
}
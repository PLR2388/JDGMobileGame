using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioDecoder : MonoBehaviour
{
    public TextAsset jsonFile;

    public Scenario Scenario
    {
        get
        {
            JsonScenario scenario = JsonUtility.FromJson<JsonScenario>(jsonFile.text);

            return scenario.ToScenario();
        }
    }
}
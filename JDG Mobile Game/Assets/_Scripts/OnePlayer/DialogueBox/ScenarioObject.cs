using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioObject
{
    [SerializeField] private List<DialogueObject> scenario;

    public List<DialogueObject> Scenario => scenario;
}

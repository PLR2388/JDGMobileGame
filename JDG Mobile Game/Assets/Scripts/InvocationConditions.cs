using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationConditions")]
public class InvocationConditions : ScriptableObject
{
    [SerializeField] private List<Condition> keys;
    [SerializeField] private List<string> values;

    public List<Condition> Keys => keys;

    public List<string> Values => values;
}

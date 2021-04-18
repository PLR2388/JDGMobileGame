using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationActionEffect")]
public class InvocationActionEffect : ScriptableObject
{
    [SerializeField] private List<ActionEffect> keys;
    [SerializeField] private List<string> values;
    
    public List<ActionEffect> Keys => keys;

    public List<string> Values => values;
}

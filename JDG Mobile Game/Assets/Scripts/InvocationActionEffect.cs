using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationActionEffect")]
public class InvocationActionEffect : ScriptableObject
{
    [SerializeField] private List<ActionEffect> keys;
    [SerializeField] private List<string> values;

    public List<ActionEffect> Keys
    {
        get
        {
            return keys;
        }
        set
        {
            keys = value;
        }
    } 

    public List<string> Values {
        get
        {
            return values;
        }
        set
        {
            values = value;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationActionEffect")]
public class InvocationActionEffect : ScriptableObject
{
    [SerializeField] private List<ActionEffect> keys;
    [SerializeField] private List<string> values;
}

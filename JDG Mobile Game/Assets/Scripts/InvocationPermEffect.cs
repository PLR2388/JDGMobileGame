using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationPermEffect")]
public class InvocationPermEffect : ScriptableObject
{
    [SerializeField] private List<PermEffect> keys;
    [SerializeField] private List<string> values;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationDeathEffect")]
public class InvocationDeathEffect : ScriptableObject
{
    [SerializeField] private List<DeathEffect> keys;
    [SerializeField] private List<string> values;
    
    public List<DeathEffect> Keys => keys;

    public List<string> Values => values;
}
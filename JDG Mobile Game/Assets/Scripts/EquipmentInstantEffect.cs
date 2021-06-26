using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentCondition", menuName = "EquipmentInstantEffect")]
public class EquipmentInstantEffect : ScriptableObject
{
    [SerializeField] private List<InstantEffect> keys;
    [SerializeField] private List<string> values;

    public IEnumerable<InstantEffect> Keys => keys;

    public IEnumerable<string> Values => values;
}
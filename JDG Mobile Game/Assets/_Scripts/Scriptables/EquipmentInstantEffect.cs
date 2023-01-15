using System.Collections.Generic;
using Cards.InvocationCards;
using UnityEngine;

namespace Cards.EquipmentCards
{
    [CreateAssetMenu(fileName = "EquipmentCondition", menuName = "EquipmentInstantEffect")]
    public class EquipmentInstantEffect : ScriptableObject
    {
        [SerializeField] private List<InstantEffect> keys;
        [SerializeField] private List<string> values;

        public List<InstantEffect> Keys => keys;

        public List<string> Values => values;
    }
}
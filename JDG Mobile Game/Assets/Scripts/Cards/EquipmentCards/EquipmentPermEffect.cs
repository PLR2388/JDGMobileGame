using System.Collections.Generic;
using UnityEngine;

namespace Cards.EquipmentCards
{
    [CreateAssetMenu(fileName = "EquipmentCondition", menuName = "EquipmentPermEffect")]
    public class EquipmentPermEffect : ScriptableObject
    {
        [SerializeField] private List<PermanentEffect> keys;
        [SerializeField] private List<string> values;

        public List<PermanentEffect> Keys => keys;

        public List<string> Values => values;
    }
}
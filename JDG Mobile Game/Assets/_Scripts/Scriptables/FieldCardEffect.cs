using System.Collections.Generic;
using UnityEngine;

namespace Cards.FieldCards
{
    [CreateAssetMenu(fileName = "FieldCardEffect", menuName = "FieldCardEffect")]
    public class FieldCardEffect : ScriptableObject
    {
        [SerializeField] private List<FieldEffect> keys;
        [SerializeField] private List<string> values;

        public List<FieldEffect> Keys => keys;

        public List<string> Values => values;
    }
}
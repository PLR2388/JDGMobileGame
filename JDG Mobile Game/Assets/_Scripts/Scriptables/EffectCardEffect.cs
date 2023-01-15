using System.Collections.Generic;
using UnityEngine;

namespace Cards.EffectCards
{
    [CreateAssetMenu(menuName = "EffectCardEffect")]
    public class EffectCardEffect : ScriptableObject
    {
        [SerializeField] private List<Effect> keys;
        [SerializeField] private List<string> values;

        public List<Effect> Keys => keys;

        public List<string> Values => values;
    }
}
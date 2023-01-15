using System.Collections.Generic;
using UnityEngine;

namespace Cards.InvocationCards
{
    [CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationDeathEffect")]
    public class InvocationDeathEffect : ScriptableObject
    {
        [SerializeField] private List<DeathEffect> keys;
        [SerializeField] private List<string> values;

        public List<DeathEffect> Keys => keys;

        public List<string> Values => values;
    }
}
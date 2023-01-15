using System.Collections.Generic;
using UnityEngine;

namespace Cards.InvocationCards
{
    [CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationStartEffect")]
    public class InvocationStartEffect : ScriptableObject
    {
        [SerializeField] private List<StartEffect> keys;
        [SerializeField] private List<string> values;

        public List<StartEffect> Keys => keys;

        public List<string> Values => values;
    }
}
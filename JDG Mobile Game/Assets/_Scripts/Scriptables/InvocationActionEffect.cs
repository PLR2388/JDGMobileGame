using System.Collections.Generic;
using UnityEngine;

namespace Cards.InvocationCards
{
    [CreateAssetMenu(fileName = "InvocationCondition", menuName = "InvocationActionEffect")]
    public class InvocationActionEffect : ScriptableObject
    {
        [SerializeField] private List<ActionEffect> keys;
        [SerializeField] private List<string> values;

        public List<ActionEffect> Keys
        {
            get => keys;
            set => keys = value;
        }

        public List<string> Values
        {
            get => values;
            set => values = value;
        }
    }
}
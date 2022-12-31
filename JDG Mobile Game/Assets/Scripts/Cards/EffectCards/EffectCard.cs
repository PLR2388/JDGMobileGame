using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.EffectCards
{
    [CreateAssetMenu(fileName = "NewEffectCard", menuName = "EffectCard")]
    public class EffectCard : Card
    {
        [SerializeField] private EffectCardEffect effectCardEffect;
        [SerializeField] private int lifeTime; // Time in laps before the effect stop
        [SerializeField] public bool checkTurn;

        [FormerlySerializedAs("affectPV")] [SerializeField]
        public float affectPv;

        public int LifeTime => lifeTime;

        public EffectCardEffect EffectCardEffect => effectCardEffect;

        private void Awake()
        {
            type = CardType.Effect;
        }
    }
}
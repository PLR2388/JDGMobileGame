using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.EffectCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "EffectCard")]
    public class EffectCard : Card
    {
        [SerializeField] private EffectCardEffect effectCardEffect;
        [SerializeField] private int lifeTime; // Time in laps before the effect stop
        [SerializeField] public bool checkTurn;
        [FormerlySerializedAs("affectPV")] [SerializeField] public float affectPv;

        public EffectCardEffect GetEffectCardEffect()
        {
            return effectCardEffect;
        }

        private void Awake()
        {
            type = CardType.Effect;
        }

        public int GetLifeTime()
        {
            return lifeTime;
        }

        public void SetLifeTime(int life)
        {
            lifeTime = life;
        }

        public void DecrementLifeTime()
        {
            lifeTime -= 1;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

namespace Cards.EffectCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "EffectCard")]
    public class EffectCard : Card
    {
        [SerializeField] private EffectCardEffect effectCardEffect;
        [SerializeField] private int lifeTime; // Time in laps before the effect stop
        [SerializeField] public bool checkTurn;

        [FormerlySerializedAs("affectPV")] [SerializeField]
        public float affectPv;

        private int currentLifeTime;

        public void Init()
        {
            ResetLifeTime();
        }

        public EffectCardEffect GetEffectCardEffect()
        {
            return effectCardEffect;
        }

        private void Awake()
        {
            type = CardType.Effect;
            currentLifeTime = lifeTime;
        }

        public int GetLifeTime()
        {
            return currentLifeTime;
        }

        public void SetLifeTime(int life)
        {
            lifeTime = life;
            currentLifeTime = life;
        }

        public void DecrementLifeTime()
        {
            currentLifeTime -= 1;
        }

        private void ResetLifeTime()
        {
            currentLifeTime = lifeTime;
        }
    }
}
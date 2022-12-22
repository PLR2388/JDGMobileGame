using UnityEngine;

namespace Cards.EffectCards
{
    public class InGameEffectCard : InGameCard
    {
        public EffectCard baseEffectCard;
        [SerializeField] private EffectCardEffect effectCardEffect;
        [SerializeField] private int lifeTime;
        [SerializeField] public bool checkTurn;

        [SerializeField]
        public float affectPv;

        private int currentLifeTime;

        public static InGameEffectCard Init(EffectCard effectCard)
        {
            InGameEffectCard inGameEffectCard = new InGameEffectCard
            {
                baseEffectCard = effectCard
            };
            inGameEffectCard.Reset();
            return inGameEffectCard;
        }

        private void Awake()
        {
            type = CardType.Effect;
            currentLifeTime = lifeTime;
        }

        private void Reset()
        {
            title = baseEffectCard.Nom;
            description = baseEffectCard.Description;
            baseCard = baseEffectCard;
            detailedDescription = baseEffectCard.DetailedDescription;
            type = baseEffectCard.Type;
            materialCard = baseEffectCard.MaterialCard;
            collector = baseEffectCard.Collector;
            effectCardEffect = baseEffectCard.GetEffectCardEffect();
            lifeTime = baseEffectCard.GetLifeTime();
            checkTurn = baseEffectCard.checkTurn;
            affectPv = baseEffectCard.affectPv;
        }

        public void Init()
        {
            ResetLifeTime();
        }

        public EffectCardEffect GetEffectCardEffect()
        {
            return effectCardEffect;
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

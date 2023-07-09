using System.Collections.Generic;
using System.Linq;

namespace Cards.EffectCards
{
    public class InGameEffectCard : InGameCard
    {
        private EffectCard baseEffectCard;
        private int lifeTime;
        public bool checkTurn;

        public float affectPv;

        public List<EffectAbility> EffectAbilities = new List<EffectAbility>();


        public int LifeTime
        {
            get => lifeTime;
            set => lifeTime = value;
        }

        public static InGameEffectCard Init(EffectCard effectCard, CardOwner cardOwner)
        {
            InGameEffectCard inGameEffectCard = new InGameEffectCard
            {
                baseEffectCard = effectCard,
                CardOwner = cardOwner
            };
            inGameEffectCard.Reset();
            return inGameEffectCard;
        }

        private void Reset()
        {
            title = baseEffectCard.Title;
            description = baseEffectCard.Description;
            baseCard = baseEffectCard;
            detailedDescription = baseEffectCard.DetailedDescription;
            type = baseEffectCard.Type;
            materialCard = baseEffectCard.MaterialCard;
            collector = baseEffectCard.Collector;
            lifeTime = baseEffectCard.LifeTime;
            checkTurn = baseEffectCard.checkTurn;
            affectPv = baseEffectCard.affectPv;
            EffectAbilities = baseEffectCard.EffectAbilities.Select(
                effectAbilityName => EffectAbilityLibrary.Instance.effectAbilityDictionary[effectAbilityName]
            ).ToList();
        }

        public void DecrementLifeTime()
        {
            lifeTime -= 1;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Cards.EffectCards
{
    public class InGameEffectCard : InGameCard
    {
        private EffectCard baseEffectCard;

        public List<EffectAbility> EffectAbilities = new List<EffectAbility>();

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
            EffectAbilities = baseEffectCard.EffectAbilities.Select(
                effectAbilityName => EffectAbilityLibrary.Instance.effectAbilityDictionary[effectAbilityName]
            ).ToList();
        }
    }
}
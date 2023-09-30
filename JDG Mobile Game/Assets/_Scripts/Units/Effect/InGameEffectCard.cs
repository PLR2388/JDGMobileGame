using System.Collections.Generic;
using System.Linq;

namespace Cards.EffectCards
{
    public class InGameEffectCard : InGameCard
    {
        private EffectCard baseEffectCard;

        public List<EffectAbility> EffectAbilities = new List<EffectAbility>();

        public InGameEffectCard(EffectCard effectCard, CardOwner cardOwner)
        {
            baseEffectCard = effectCard;
            CardOwner = cardOwner;
            Reset();
        }

        private void Reset()
        {
            title = baseEffectCard.Title;
            Description = baseEffectCard.Description;
            BaseCard = baseEffectCard;
            DetailedDescription = baseEffectCard.DetailedDescription;
            type = baseEffectCard.Type;
            materialCard = baseEffectCard.MaterialCard;
            collector = baseEffectCard.Collector;
            EffectAbilities = baseEffectCard.EffectAbilities.Select(
                effectAbilityName => EffectAbilityLibrary.Instance.effectAbilityDictionary[effectAbilityName]
            ).ToList();
        }
    }
}
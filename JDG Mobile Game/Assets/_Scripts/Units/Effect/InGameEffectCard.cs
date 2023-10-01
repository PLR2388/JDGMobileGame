using System.Collections.Generic;
using System.Linq;

namespace Cards.EffectCards
{
    /// <summary>
    /// Represents an in-game effect card, which is derived from a base effect card and has additional in-game properties and behaviors.
    /// </summary>
    public class InGameEffectCard : InGameCard
    {
        private readonly EffectCard baseEffectCard;

        /// <summary>
        /// List of effect abilities associated with this card.
        /// </summary>
        public List<EffectAbility> EffectAbilities = new List<EffectAbility>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InGameEffectCard"/> class.
        /// </summary>
        /// <param name="effectCard">The base effect card from which the in-game card is derived.</param>
        /// <param name="cardOwner">The owner of the card.</param>
        public InGameEffectCard(EffectCard effectCard, CardOwner cardOwner)
        {
            baseEffectCard = effectCard;
            CardOwner = cardOwner;
            Reset();
        }

        /// <summary>
        /// Resets the in-game card properties to match those of the base effect card.
        /// </summary>
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
using System.Collections.Generic;
using System.Linq;
using JDG.Application.Cards;

namespace Cards.EffectCards
{
    /// <summary>
    /// Represents an in-game effect card, which is derived from a base effect card and has additional in-game properties and behaviors.
    /// Phase 49: Implements IInGameEffectCard for complete abstraction.
    /// </summary>
    public class InGameEffectCard : InGameCard, IInGameEffectCard
    {
        private readonly EffectCard baseEffectCard;
        private readonly IEffectAbilityProvider _abilityProvider;

        /// <summary>
        /// List of effect abilities associated with this card.
        /// </summary>
        public List<EffectAbility> EffectAbilities = new List<EffectAbility>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InGameEffectCard"/> class.
        /// Phase 48: Added optional abilityProvider parameter for DI.
        /// </summary>
        /// <param name="effectCard">The base effect card from which the in-game card is derived.</param>
        /// <param name="cardOwner">The owner of the card.</param>
        /// <param name="abilityProvider">Optional ability provider (uses legacy library if null).</param>
        public InGameEffectCard(EffectCard effectCard, CardOwner cardOwner, IEffectAbilityProvider abilityProvider = null)
        {
            baseEffectCard = effectCard;
            CardOwner = cardOwner;
            _abilityProvider = abilityProvider;
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

            // Phase 48: Use injected provider if available, fallback to legacy library
            if (_abilityProvider != null)
            {
                EffectAbilities = baseEffectCard.EffectAbilities
                    .Select(name => _abilityProvider.GetAbility(name))
                    .Where(ability => ability != null)
                    .ToList();
            }
            else
            {
                // Fallback to legacy singleton for backward compatibility
                EffectAbilities = baseEffectCard.EffectAbilities.Select(
                    effectAbilityName => EffectAbilityLibrary.Instance.EffectAbilityDictionary[effectAbilityName]
                ).ToList();
            }
        }

        #region IInGameEffectCard Implementation

        /// <summary>
        /// Gets the effect abilities as a read-only list of objects.
        /// Phase 49: Explicit implementation for IInGameEffectCard interface.
        /// </summary>
        IReadOnlyList<object> IInGameEffectCard.EffectAbilities =>
            EffectAbilities.Cast<object>().ToList().AsReadOnly();

        #endregion
    }
}
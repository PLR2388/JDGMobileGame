using System.Collections.Generic;
using System.Linq;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using DomainEffectAbilityName = JDG.Domain.Enums.EffectAbilityName;

namespace Cards.EffectCards
{
    /// <summary>
    /// Represents an in-game effect card, which is derived from a base effect card and has additional in-game properties and behaviors.
    /// Phase 49: Implements IInGameEffectCard for complete abstraction.
    /// Phase 105: Added ModernEffectAbilities for IAbility migration.
    /// </summary>
    public class InGameEffectCard : InGameCard, IInGameEffectCard
    {
        private readonly EffectCard baseEffectCard;
        private readonly IEffectAbilityProvider _abilityProvider;

        /// <summary>
        /// List of legacy effect abilities associated with this card.
        /// Phase 105: Marked for deprecation - use ModernEffectAbilities instead.
        /// </summary>
        public List<EffectAbility> EffectAbilities = new List<EffectAbility>();

        /// <summary>
        /// List of modern IAbility implementations for this card.
        /// Phase 105: New property for clean architecture migration.
        /// </summary>
        public List<IAbility> ModernEffectAbilities { get; private set; } = new List<IAbility>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InGameEffectCard"/> class.
        /// Phase 48: Added abilityProvider parameter for DI.
        /// Phase 61: Made abilityProvider required (removed fallback to legacy singleton).
        /// </summary>
        /// <param name="effectCard">The base effect card from which the in-game card is derived.</param>
        /// <param name="cardOwner">The owner of the card.</param>
        /// <param name="abilityProvider">Provider for effect abilities (required).</param>
        public InGameEffectCard(EffectCard effectCard, CardOwner cardOwner, IEffectAbilityProvider abilityProvider)
        {
            baseEffectCard = effectCard;
            CardOwner = cardOwner;
            _abilityProvider = abilityProvider;
            Reset();
        }

        /// <summary>
        /// Resets the in-game card properties to match those of the base effect card.
        /// Phase 105: Now populates both legacy and modern ability lists.
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

            // Phase 61: Use injected provider (fallback removed)
            // Legacy abilities (for backward compatibility)
            EffectAbilities = baseEffectCard.EffectAbilities
                .Select(name => _abilityProvider.GetAbility(name))
                .Where(ability => ability != null)
                .ToList();

            // Phase 105: Populate modern abilities
            ModernEffectAbilities = baseEffectCard.EffectAbilities
                .Select(name => _abilityProvider.GetModernAbility(ConvertToDomainEnum(name)))
                .Where(ability => ability != null)
                .ToList();
        }

        /// <summary>
        /// Converts legacy EffectAbilityName to domain enum.
        /// Phase 105: Bridge method for enum conversion during migration.
        /// </summary>
        private static DomainEffectAbilityName ConvertToDomainEnum(EffectAbilityName legacyName)
        {
            // Both enums have identical orderings, so integer cast works
            return (DomainEffectAbilityName)(int)legacyName;
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
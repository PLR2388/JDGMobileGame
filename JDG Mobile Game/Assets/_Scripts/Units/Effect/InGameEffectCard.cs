using System.Collections.Generic;
using System.Linq;
using JDG.Application.Abilities;
using JDG.Application.Cards;

namespace Cards.EffectCards
{
    /// <summary>
    /// Represents an in-game effect card, which is derived from a base effect card and has additional in-game properties and behaviors.
    /// Phase 49: Implements IInGameEffectCard for complete abstraction.
    /// Phase 105: Added ModernEffectAbilities for IAbility migration.
    /// Phase 114: Removed legacy EffectAbilities - now uses only modern IAbility.
    /// </summary>
    public class InGameEffectCard : InGameCard, IInGameEffectCard
    {
        private readonly EffectCard baseEffectCard;
        private readonly IEffectAbilityProvider _abilityProvider;

        /// <summary>
        /// List of modern IAbility implementations for this card.
        /// Phase 114: Now the primary (and only) ability storage.
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
        /// Phase 114: Now populates only modern ability list.
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

            // Phase 114: Only populate modern abilities (legacy removed)
            // Phase 118: ScriptableObjects now use domain enums directly, no conversion needed
            // Phase 146: Added warning for missing abilities
            ModernEffectAbilities = baseEffectCard.EffectAbilities
                .Select(name => {
                    var ability = _abilityProvider.GetModernAbility(name);
                    if (ability == null)
                        UnityEngine.Debug.LogWarning($"[InGameEffectCard] Ability '{name}' not found for card '{title}'");
                    return ability;
                })
                .Where(ability => ability != null)
                .ToList();
        }

        #region IInGameEffectCard Implementation

        /// <summary>
        /// Gets the effect abilities as a read-only list.
        /// Phase 114: Now returns modern IAbility instances directly.
        /// </summary>
        IReadOnlyList<IAbility> IInGameEffectCard.EffectAbilities => ModernEffectAbilities.AsReadOnly();

        #endregion
    }
}
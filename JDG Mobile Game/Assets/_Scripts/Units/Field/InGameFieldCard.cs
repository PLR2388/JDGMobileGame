using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.FieldCards;
using JDG.Application.Abilities;
using JDG.Application.Cards;

/// <summary>
/// Represents a card on the field in the game with additional runtime behaviors.
/// Phase 49: Implements IInGameFieldCard for complete abstraction.
/// Phase 105: Added ModernFieldAbilities for IAbility migration.
/// Phase 116: Removed legacy FieldAbilities - now uses only modern IAbility.
/// </summary>
public class InGameFieldCard : InGameCard, IInGameFieldCard
{
    private readonly FieldCard baseFieldCard;
    private readonly IFieldAbilityProvider _abilityProvider;

    public CardFamily Family { get; private set; }

    /// <summary>
    /// List of modern IAbility implementations for this card.
    /// Phase 116: Now the primary (and only) ability storage.
    /// </summary>
    public List<IAbility> ModernFieldAbilities { get; private set; } = new List<IAbility>();

    /// <summary>
    /// Initializes a new instance of <see cref="InGameFieldCard"/> using the base <see cref="FieldCard"/> data.
    /// Phase 48: Added abilityProvider parameter for DI.
    /// Phase 61: Made abilityProvider required (removed fallback to legacy singleton).
    /// </summary>
    /// <param name="fieldCard">The base field card data.</param>
    /// <param name="cardOwner">The owner of the card.</param>
    /// <param name="abilityProvider">Provider for field abilities (required).</param>
    public InGameFieldCard(FieldCard fieldCard, CardOwner cardOwner, IFieldAbilityProvider abilityProvider)
    {
        baseFieldCard = fieldCard;
        CardOwner = cardOwner;
        _abilityProvider = abilityProvider;
        Reset();
    }

    /// <summary>
    /// Resets the card's properties based on the underlying base field card.
    /// Phase 116: Now populates only modern ability list.
    /// </summary>
    private void Reset()
    {
        title = baseFieldCard.Title;
        Description = baseFieldCard.Description;
        BaseCard = baseFieldCard;
        DetailedDescription = baseFieldCard.DetailedDescription;
        type = baseFieldCard.Type;
        materialCard = baseFieldCard.MaterialCard;
        collector = baseFieldCard.Collector;
        Family = baseFieldCard.Family;

        // Phase 116: Only populate modern abilities (legacy removed)
        // Phase 118: ScriptableObjects now use domain enums directly, no conversion needed
        // Phase 146: Added warning for missing abilities
        ModernFieldAbilities = baseFieldCard.FieldAbilities
            .Select(name => {
                var ability = _abilityProvider.GetModernAbility(name);
                if (ability == null)
                    UnityEngine.Debug.LogWarning($"[InGameFieldCard] Ability '{name}' not found for card '{title}'");
                return ability;
            })
            .Where(ability => ability != null)
            .ToList();
    }

    #region IInGameFieldCard Implementation

    /// <summary>
    /// Gets the field abilities as a read-only list.
    /// Phase 116: Now returns modern IAbility instances directly.
    /// </summary>
    IReadOnlyList<IAbility> IInGameFieldCard.FieldAbilities => ModernFieldAbilities.AsReadOnly();

    #endregion
}
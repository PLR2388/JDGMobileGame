using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.FieldCards;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using DomainFieldAbilityName = JDG.Domain.Enums.FieldAbilityName;

/// <summary>
/// Represents a card on the field in the game with additional runtime behaviors.
/// Phase 49: Implements IInGameFieldCard for complete abstraction.
/// Phase 105: Added ModernFieldAbilities for IAbility migration.
/// </summary>
public class InGameFieldCard : InGameCard, IInGameFieldCard
{
    private readonly FieldCard baseFieldCard;
    private readonly IFieldAbilityProvider _abilityProvider;

    public CardFamily Family { get; private set; }

    /// <summary>
    /// List of legacy abilities associated with the field card.
    /// Phase 105: Marked for deprecation - use ModernFieldAbilities instead.
    /// </summary>
    public List<FieldAbility> FieldAbilities = new List<FieldAbility>();

    /// <summary>
    /// List of modern IAbility implementations for this card.
    /// Phase 105: New property for clean architecture migration.
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
    /// Phase 105: Now populates both legacy and modern ability lists.
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

        // Phase 61: Use injected provider (fallback removed)
        // Legacy abilities (for backward compatibility)
        FieldAbilities = baseFieldCard.FieldAbilities
            .Select(name => _abilityProvider.GetAbility(name))
            .Where(ability => ability != null)
            .ToList();

        // Phase 105: Populate modern abilities
        ModernFieldAbilities = baseFieldCard.FieldAbilities
            .Select(name => _abilityProvider.GetModernAbility(ConvertToDomainEnum(name)))
            .Where(ability => ability != null)
            .ToList();
    }

    /// <summary>
    /// Converts legacy FieldAbilityName to domain enum.
    /// Phase 105: Bridge method for enum conversion during migration.
    /// </summary>
    private static DomainFieldAbilityName ConvertToDomainEnum(FieldAbilityName legacyName)
    {
        // Both enums have identical orderings, so integer cast works
        return (DomainFieldAbilityName)(int)legacyName;
    }

    #region IInGameFieldCard Implementation

    /// <summary>
    /// Gets the field abilities as a read-only list of objects.
    /// Phase 49: Explicit implementation for IInGameFieldCard interface.
    /// </summary>
    IReadOnlyList<object> IInGameFieldCard.FieldAbilities =>
        FieldAbilities.Cast<object>().ToList().AsReadOnly();

    #endregion
}
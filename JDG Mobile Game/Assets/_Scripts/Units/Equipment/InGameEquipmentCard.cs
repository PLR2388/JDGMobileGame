using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using DomainEquipmentAbilityName = JDG.Domain.Enums.EquipmentAbilityName;

/// <summary>
/// Represents an in-game version of an equipment card with its abilities.
/// Phase 49: Implements IInGameEquipmentCard for complete abstraction.
/// Phase 105: Added ModernEquipmentAbilities for IAbility migration.
/// </summary>
public class InGameEquipmentCard : InGameCard, IInGameEquipmentCard
{
    private readonly EquipmentCard baseEquipmentCard;
    private readonly IEquipmentAbilityProvider _abilityProvider;

    /// <summary>
    /// List of legacy abilities associated with the equipment card.
    /// Phase 105: Marked for deprecation - use ModernEquipmentAbilities instead.
    /// </summary>
    public List<EquipmentAbility> EquipmentAbilities = new List<EquipmentAbility>();

    /// <summary>
    /// List of modern IAbility implementations for this card.
    /// Phase 105: New property for clean architecture migration.
    /// </summary>
    public List<IAbility> ModernEquipmentAbilities { get; private set; } = new List<IAbility>();

    /// <summary>
    /// Initializes a new instance of the <see cref="InGameEquipmentCard"/> class.
    /// Phase 48: Added abilityProvider parameter for DI.
    /// Phase 61: Made abilityProvider required (removed fallback to legacy singleton).
    /// </summary>
    /// <param name="equipmentCard">The base equipment card this in-game card is based on.</param>
    /// <param name="cardOwner">The owner of this card.</param>
    /// <param name="abilityProvider">Provider for equipment abilities (required).</param>
    public InGameEquipmentCard(EquipmentCard equipmentCard, CardOwner cardOwner, IEquipmentAbilityProvider abilityProvider)
    {
        baseEquipmentCard = equipmentCard;
        CardOwner = cardOwner;
        _abilityProvider = abilityProvider;
        Reset();
    }

    /// <summary>
    /// Resets the in-game equipment card's details to match its base equipment card.
    /// Phase 105: Now populates both legacy and modern ability lists.
    /// </summary>
    private void Reset()
    {
        title = baseEquipmentCard.Title;
        Description = baseEquipmentCard.Description;
        DetailedDescription = baseEquipmentCard.DetailedDescription;
        BaseCard = baseEquipmentCard;
        type = baseEquipmentCard.Type;
        materialCard = baseEquipmentCard.MaterialCard;
        collector = baseEquipmentCard.Collector;

        // Phase 61: Use injected provider (fallback removed)
        // Legacy abilities (for backward compatibility)
        EquipmentAbilities = baseEquipmentCard.EquipmentAbilities
            .Select(name => _abilityProvider.GetAbility(name))
            .Where(ability => ability != null)
            .ToList();

        // Phase 105: Populate modern abilities
        ModernEquipmentAbilities = baseEquipmentCard.EquipmentAbilities
            .Select(name => _abilityProvider.GetModernAbility(ConvertToDomainEnum(name)))
            .Where(ability => ability != null)
            .ToList();
    }

    /// <summary>
    /// Converts legacy EquipmentAbilityName to domain enum.
    /// Phase 105: Bridge method for enum conversion during migration.
    /// </summary>
    private static DomainEquipmentAbilityName ConvertToDomainEnum(EquipmentAbilityName legacyName)
    {
        // Both enums have identical orderings, so integer cast works
        return (DomainEquipmentAbilityName)(int)legacyName;
    }

    #region IInGameEquipmentCard Implementation

    /// <summary>
    /// Gets the equipment abilities as a read-only list of objects.
    /// Phase 49: Explicit implementation for IInGameEquipmentCard interface.
    /// </summary>
    IReadOnlyList<object> IInGameEquipmentCard.EquipmentAbilities =>
        EquipmentAbilities.Cast<object>().ToList().AsReadOnly();

    #endregion
}
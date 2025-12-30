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
/// Phase 117: Removed legacy EquipmentAbilities - now uses only ModernEquipmentAbilities.
/// </summary>
public class InGameEquipmentCard : InGameCard, IInGameEquipmentCard
{
    private readonly EquipmentCard baseEquipmentCard;
    private readonly IEquipmentAbilityProvider _abilityProvider;

    /// <summary>
    /// List of modern IAbility implementations for this card.
    /// Phase 117: Now the primary (and only) ability storage.
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
    /// Phase 117: Now populates only modern ability list.
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

        // Phase 117: Populate only modern abilities
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
    /// Gets the equipment abilities as a read-only list.
    /// Phase 117: Now returns ModernEquipmentAbilities directly.
    /// </summary>
    IReadOnlyList<IAbility> IInGameEquipmentCard.EquipmentAbilities =>
        ModernEquipmentAbilities.AsReadOnly();

    /// <summary>
    /// Whether this equipment can be placed on cards that already have equipment.
    /// Phase 117: Checks if any ability has CanAlwaysBePlaced = true.
    /// </summary>
    public bool CanAlwaysBePlaced =>
        ModernEquipmentAbilities.OfType<IEquipmentAbility>().Any(a => a.CanAlwaysBePlaced);

    #endregion
}
using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;

/// <summary>
/// Represents an in-game version of an equipment card with its abilities.
/// </summary>
public class InGameEquipmentCard : InGameCard
{
    private readonly EquipmentCard baseEquipmentCard;
    private readonly IEquipmentAbilityProvider _abilityProvider;

    /// <summary>
    /// List of abilities associated with the equipment card.
    /// </summary>
    public List<EquipmentAbility> EquipmentAbilities = new List<EquipmentAbility>();

    /// <summary>
    /// Initializes a new instance of the <see cref="InGameEquipmentCard"/> class.
    /// Phase 48: Added optional abilityProvider parameter for DI.
    /// </summary>
    /// <param name="equipmentCard">The base equipment card this in-game card is based on.</param>
    /// <param name="cardOwner">The owner of this card.</param>
    /// <param name="abilityProvider">Optional ability provider (uses legacy library if null).</param>
    public InGameEquipmentCard(EquipmentCard equipmentCard, CardOwner cardOwner, IEquipmentAbilityProvider abilityProvider = null)
    {
        baseEquipmentCard = equipmentCard;
        CardOwner = cardOwner;
        _abilityProvider = abilityProvider;
        Reset();
    }

    /// <summary>
    /// Resets the in-game equipment card's details to match its base equipment card.
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

        // Phase 48: Use injected provider if available, fallback to legacy library
        if (_abilityProvider != null)
        {
            EquipmentAbilities = baseEquipmentCard.EquipmentAbilities
                .Select(name => _abilityProvider.GetAbility(name))
                .Where(ability => ability != null)
                .ToList();
        }
        else
        {
            // Fallback to legacy singleton for backward compatibility
            EquipmentAbilities = baseEquipmentCard.EquipmentAbilities.Select(
                equipmentAbilityName => EquipmentAbilityLibrary.Instance.EquipmentAbilityDictionary[equipmentAbilityName]
            ).ToList();
        }
    }
}
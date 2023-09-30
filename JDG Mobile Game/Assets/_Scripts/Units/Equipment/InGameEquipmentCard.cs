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

    /// <summary>
    /// List of abilities associated with the equipment card.
    /// </summary>
    public List<EquipmentAbility> EquipmentAbilities = new List<EquipmentAbility>();

    /// <summary>
    /// Initializes a new instance of the <see cref="InGameEquipmentCard"/> class.
    /// </summary>
    /// <param name="equipmentCard">The base equipment card this in-game card is based on.</param>
    /// <param name="cardOwner">The owner of this card.</param>
    public InGameEquipmentCard(EquipmentCard equipmentCard, CardOwner cardOwner)
    {
        baseEquipmentCard = equipmentCard;
        CardOwner = cardOwner;
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
        EquipmentAbilities = baseEquipmentCard.EquipmentAbilities.Select(
            equipmentAbilityName => EquipmentAbilityLibrary.Instance.EquipmentAbilityDictionary[equipmentAbilityName]
        ).ToList();
    }
}
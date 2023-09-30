using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;

public class InGameEquipmentCard : InGameCard
{
    private EquipmentCard baseEquipmentCard;

    public List<EquipmentAbility> EquipmentAbilities = new List<EquipmentAbility>();


    public InGameEquipmentCard(EquipmentCard equipmentCard, CardOwner cardOwner)
    {
        baseEquipmentCard = equipmentCard;
        CardOwner = cardOwner;
        Reset();
    }

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
            equipmentAbilityName => EquipmentAbilityLibrary.Instance.equipmentAbilityDictionary[equipmentAbilityName]
        ).ToList();
    }
}
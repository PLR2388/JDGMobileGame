using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;

public class InGameEquipmentCard : InGameCard
{
    private EquipmentCard baseEquipmentCard;

    public List<EquipmentAbility> EquipmentAbilities = new List<EquipmentAbility>();


    public static InGameEquipmentCard Init(EquipmentCard equipmentCard, CardOwner cardOwner)
    {
        InGameEquipmentCard inGameEquipmentCard = new InGameEquipmentCard
        {
            baseEquipmentCard = equipmentCard,
            CardOwner = cardOwner
        };
        inGameEquipmentCard.Reset();
        return inGameEquipmentCard;
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
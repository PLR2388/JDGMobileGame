using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;

public class InGameEquipementCard : InGameCard
{
    private EquipmentCard baseEquipmentCard;

    public List<EquipmentAbility> EquipmentAbilities = new List<EquipmentAbility>();


    public static InGameEquipementCard Init(EquipmentCard equipmentCard, CardOwner cardOwner)
    {
        InGameEquipementCard inGameEquipmentCard = new InGameEquipementCard
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
        description = baseEquipmentCard.Description;
        detailedDescription = baseEquipmentCard.DetailedDescription;
        baseCard = baseEquipmentCard;
        type = baseEquipmentCard.Type;
        materialCard = baseEquipmentCard.MaterialCard;
        collector = baseEquipmentCard.Collector;
        EquipmentAbilities = baseEquipmentCard.EquipmentAbilities.Select(
            equipmentAbilityName => EquipmentAbilityLibrary.Instance.equipmentAbilityDictionary[equipmentAbilityName]
        ).ToList();
    }
}
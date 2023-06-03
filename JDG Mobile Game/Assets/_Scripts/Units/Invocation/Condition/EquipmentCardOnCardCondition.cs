using System.Linq;

public class EquipmentCardOnCardCondition : Condition
{

    private string equipmentCardName;
    private string invocationCardName;
    public EquipmentCardOnCardCondition(ConditionName name, string description, string equipmentCardName,
        string invocationCardName)
    {
        Name = name;
        Description = description;
        this.equipmentCardName = equipmentCardName;
        this.invocationCardName = invocationCardName;
    }

    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.invocationCards.Any(card =>
            card.Title == invocationCardName && card.EquipmentCard != null &&
            card.EquipmentCard.Title == equipmentCardName);
    }
}

using _Scripts.Units.Invocation;

public class ProtectFromDestructionAbility : EquipmentAbility
{
    private int numberProtect;

    public ProtectFromDestructionAbility(EquipmentAbilityName name, string description, int numberProtect = 1)
    {
        Name = name;
        Description = description;
        this.numberProtect = numberProtect;
    }

    public override bool OnInvocationPreDestroy(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.OnInvocationPreDestroy(invocationCard, playerCards);
        numberProtect--;
        if (numberProtect <= 0)
        {
            playerCards.YellowCards.Add(invocationCard.EquipmentCard);
            invocationCard.EquipmentCard = null;
        }

        return false;
    }
}

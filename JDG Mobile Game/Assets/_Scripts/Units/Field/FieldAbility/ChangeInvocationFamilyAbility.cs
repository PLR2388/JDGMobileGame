using System.Linq;
using _Scripts.Units.Invocation;
using Cards;

public class ChangeInvocationFamilyAbility : FieldAbility
{
    private string invocationName;
    private CardFamily family;

    public ChangeInvocationFamilyAbility(FieldAbilityName name, string description, string invocationName,
        CardFamily family)
    {
        Name = name;
        Description = description;
        this.invocationName = invocationName;
        this.family = family;
    }

    private static void NotifyChangeFamilyFieldCard(PlayerCards playerCards, CardFamily[] previousFamilies,
        InGameInvocationCard invocationCard)
    {
        if (playerCards.FieldCard == null) return;
        foreach (var fieldCardFieldAbility in playerCards.FieldCard.FieldAbilities)
        {
            fieldCardFieldAbility.OnInvocationChangeFamily(previousFamilies, invocationCard);
        }
    }

    public override void ApplyEffect(PlayerCards playerCards)
    {
        base.ApplyEffect(playerCards);
        var invocationCard = playerCards.invocationCards.FirstOrDefault(card => card.Title == invocationName);
        if (invocationCard != null)
        {
            var previousFamilies = invocationCard.Families;
            invocationCard.Families = new[] { family };

            NotifyChangeFamilyFieldCard(playerCards, previousFamilies, invocationCard);
        }
    }


    public override void OnInvocationCardAdded(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.OnInvocationCardAdded(invocationCard, playerCards);
        if (invocationCard.Title == invocationName)
        {
            var previousFamilies = invocationCard.Families;
            invocationCard.Families = new[] { family };
            NotifyChangeFamilyFieldCard(playerCards, previousFamilies, invocationCard);
        }
    }

    public override void OnFieldCardRemoved(PlayerCards playerCards)
    {
        base.OnFieldCardRemoved(playerCards);
        var invocationCard = playerCards.invocationCards.FirstOrDefault(card => card.Title == invocationName);
        if (invocationCard != null)
        {
            var previousFamilies = invocationCard.Families;
            invocationCard.Families = invocationCard.baseInvocationCard.BaseInvocationCardStats.Families;
            NotifyChangeFamilyFieldCard(playerCards, previousFamilies, invocationCard);
        }
    }
}
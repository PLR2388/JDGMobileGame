using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;

public class EarnATKDEFForFamilyAbility : FieldAbility
{
    private float bonusAttack;
    private float bonusDefense;
    private CardFamily family;

    public EarnATKDEFForFamilyAbility(FieldAbilityName name, string description, float atk, float def,
        CardFamily family)
    {
        Name = name;
        Description = description;
        bonusAttack = atk;
        bonusDefense = def;
        this.family = family;
    }

    private List<InGameInvocationCard> BuildValidInvocationCards(PlayerCards playerCards)
    {
        var invocationsList = family == CardFamily.Any
            ? playerCards.invocationCards
            : playerCards.invocationCards.Where(card => card.Families.Contains(family));
        return invocationsList.ToList();
    }

    public override void ApplyEffect(PlayerCards playerCards)
    {
        base.ApplyEffect(playerCards);
        var invocationCards = BuildValidInvocationCards(playerCards);
        foreach (var invocationCard in invocationCards)
        {
            invocationCard.Attack += bonusAttack;
            invocationCard.Defense += bonusDefense;
        }
    }

    public override void OnInvocationCardAdded(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.OnInvocationCardAdded(invocationCard, playerCards);
        if (family != CardFamily.Any && !invocationCard.Families.Contains(family)) return;
        invocationCard.Attack += bonusAttack;
        invocationCard.Defense += bonusDefense;
    }

    public override void OnFieldCardRemoved(PlayerCards playerCards)
    {
        base.OnFieldCardRemoved(playerCards);
        var invocationCards = BuildValidInvocationCards(playerCards);
        foreach (var invocationCard in invocationCards)
        {
            invocationCard.Attack -= bonusAttack;
            invocationCard.Defense -= bonusDefense;
        }
    }

    public override void OnInvocationChangeFamily(CardFamily[] previousFamilies, InGameInvocationCard invocationCard)
    {
        base.OnInvocationChangeFamily(previousFamilies, invocationCard);

        if (family != CardFamily.Any)
        {
            if (invocationCard.Families.Contains(family) && !previousFamilies.Contains(family))
            {
                invocationCard.Attack += bonusAttack;
                invocationCard.Defense += bonusDefense;
            }

            if (!invocationCard.Families.Contains(family) && previousFamilies.Contains(family))
            {
                invocationCard.Attack -= bonusAttack;
                invocationCard.Defense -= bonusDefense;
            }
        }
    }
}
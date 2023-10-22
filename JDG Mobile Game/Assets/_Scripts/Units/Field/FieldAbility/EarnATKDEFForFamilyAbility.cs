using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;

/// <summary>
/// Represents a field ability that modifies the attack and defense of invocations based on their family.
/// </summary>
public class EarnATKDEFForFamilyAbility : FieldAbility
{
    private readonly float bonusAttack;
    private readonly float bonusDefense;
    private readonly CardFamily family;

    /// <summary>
    /// Initializes a new instance of the <see cref="EarnATKDEFForFamilyAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the field ability.</param>
    /// <param name="description">The description of the field ability.</param>
    /// <param name="atk">Bonus attack to be applied.</param>
    /// <param name="def">Bonus defense to be applied.</param>
    /// <param name="family">The family of invocation cards this ability applies to.</param>
    public EarnATKDEFForFamilyAbility(FieldAbilityName name, string description, float atk, float def,
        CardFamily family)
    {
        Name = name;
        Description = description;
        bonusAttack = atk;
        bonusDefense = def;
        this.family = family;
    }

    /// <summary>
    /// Builds a list of valid invocation cards based on the family.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    /// <returns>A list of valid invocation cards.</returns>
    private List<InGameInvocationCard> BuildValidInvocationCards(PlayerCards playerCards)
    {
        var invocationsList = family == CardFamily.Any
            ? playerCards.InvocationCards
            : playerCards.InvocationCards.Where(card => card.Families.Contains(family));
        return invocationsList.ToList();
    }

    /// <summary>
    /// Applies the effect of this ability to the player's cards.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    public override void ApplyEffect(PlayerCards playerCards)
    {
        base.ApplyEffect(playerCards);
        var invocationCards = BuildValidInvocationCards(playerCards);
        foreach (var invocationCard in invocationCards)
        {
            IncrementStats(invocationCard);
        }
    }
    
    /// <summary>
    /// Increases the stats of the provided invocation card based on the ability.
    /// </summary>
    /// <param name="invocationCard">The invocation card.</param>
    private void IncrementStats(InGameInvocationCard invocationCard)
    {
        invocationCard.Attack += bonusAttack;
        invocationCard.Defense += bonusDefense;
    }
    
    /// <summary>
    /// Decreases the stats of the provided invocation card based on the ability.
    /// </summary>
    /// <param name="invocationCard">The invocation card.</param>
    private void DecrementStats(InGameInvocationCard invocationCard)
    {
        invocationCard.Attack -= bonusAttack;
        invocationCard.Defense -= bonusDefense;
    }
    
    /// <summary>
    /// Handles the logic when a new invocation card is added.
    /// </summary>
    /// <param name="invocationCard">The added invocation card.</param>
    /// <param name="playerCards">The player's cards.</param>
    public override void OnInvocationCardAdded(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.OnInvocationCardAdded(invocationCard, playerCards);
        if (family != CardFamily.Any && !invocationCard.Families.Contains(family)) return;
        IncrementStats(invocationCard);
    }

    /// <summary>
    /// Handles the logic when the field card is removed from the field.
    /// </summary>
    /// <param name="playerCards">The player's cards.</param>
    public override void OnFieldCardRemoved(PlayerCards playerCards)
    {
        base.OnFieldCardRemoved(playerCards);
        var invocationCards = BuildValidInvocationCards(playerCards);
        foreach (var invocationCard in invocationCards)
        {
            DecrementStats(invocationCard);
        }
    }

    /// <summary>
    /// Handles the logic when an invocation changes its family.
    /// </summary>
    /// <param name="previousFamilies">The previous families of the invocation card.</param>
    /// <param name="invocationCard">The invocation card that changed families.</param>
    public override void OnInvocationChangeFamily(CardFamily[] previousFamilies, InGameInvocationCard invocationCard)
    {
        base.OnInvocationChangeFamily(previousFamilies, invocationCard);

        if (family != CardFamily.Any)
        {
            if (invocationCard.Families.Contains(family) && !previousFamilies.Contains(family))
            {
                IncrementStats(invocationCard);
            }

            if (!invocationCard.Families.Contains(family) && previousFamilies.Contains(family))
            {
                DecrementStats(invocationCard);
            }
        }
    }
}
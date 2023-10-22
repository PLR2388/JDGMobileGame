using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability to increment or decrement the attack and defense 
/// of an invocation card based on the number of other cards in the same family present.
/// </summary>
public class WinAtkDefFamilyAbility : Ability
{
    /// <summary>
    /// The card family associated with the ability.
    /// </summary>
    protected readonly CardFamily Family;
    
    /// <summary>
    /// The attack value associated with the ability.
    /// </summary>
    private readonly float attack;
    
    /// <summary>
    /// The defense value associated with the ability.
    /// </summary>
    private readonly float defense;

    /// <summary>
    /// Initializes a new instance of the <see cref="WinAtkDefFamilyAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="family">The card family associated with the ability.</param>
    /// <param name="atk">The attack value associated with the ability.</param>
    /// <param name="def">The defense value associated with the ability.</param>
    public WinAtkDefFamilyAbility(AbilityName name, string description, CardFamily family, float atk,
        float def)
    {
        Name = name;
        Description = description;
        Family = family;
        attack = atk;
        defense = def;
    }
    
    /// <summary>
    /// Increments the attack and defense of the invocation card.
    /// </summary>
    /// <param name="numberCardInvocation">The number of invocation cards to consider for increment.</param>
    protected void IncrementAtkDefInvocationCard(int numberCardInvocation)
    {
        invocationCard.Attack += numberCardInvocation * attack;
        invocationCard.Defense += numberCardInvocation * defense;
    }
    
    /// <summary>
    /// Decrements the attack and defense of the invocation card.
    /// </summary>
    /// <param name="numberCardInvocation">The number of invocation cards to consider for decrement.</param>
    protected void DecrementAtkDefInvocationCard(int numberCardInvocation)
    {
        invocationCard.Attack -= numberCardInvocation * attack;
        invocationCard.Defense -= numberCardInvocation * defense;
    }

    
    /// <summary>
    /// Applies the effect of the ability.
    /// </summary>
    /// <param name="canvas">The transform of the canvas.</param>
    /// <param name="playerCards">The player cards associated with the player.</param>
    /// <param name="opponentPlayerCards">The player cards associated with the opponent.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }
    
    /// <summary>
    /// Calculates the number of invocation cards in the same family.
    /// </summary>
    /// <param name="playerCards">The player cards to consider.</param>
    /// <returns>The number of invocation cards in the same family.</returns>
    private int CalculateNumberCardInvocation(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Count(card => card.Title != invocationCard.Title && card.Families.Contains(Family));
    }

    /// <summary>
    /// Applies the ability power based on the number of invocation cards.
    /// </summary>
    /// <param name="playerCards">The player cards to consider.</param>
    private void ApplyPower(PlayerCards playerCards)
    {
        int numberCardInvocation = CalculateNumberCardInvocation(playerCards);
        IncrementAtkDefInvocationCard(numberCardInvocation);
    }

    /// <summary>
    /// Cancels the effect of the ability and reverts any changes made to the invocation card.
    /// </summary>
    /// <param name="playerCards">The player cards to consider for reverting the ability effect.</param>
    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        int numberCardInvocation = CalculateNumberCardInvocation(playerCards);
        DecrementAtkDefInvocationCard(numberCardInvocation);
    }

    /// <summary>
    /// Reactivates the effect of the ability on the invocation card.
    /// </summary>
    /// <param name="playerCards">The player cards to consider for reactivating the ability effect.</param>
    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyPower(playerCards);
    }

    /// <summary>
    /// Handles the addition of a new card and applies the ability effect if necessary.
    /// </summary>
    /// <param name="newCard">The newly added invocation card.</param>
    /// <param name="playerCards">The player cards to consider for applying the ability effect.</param>
    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (newCard.Title != invocationCard.Title && newCard.Families.Contains(Family))
        {
            IncrementAtkDefInvocationCard(1);
        }
    }

    /// <summary>
    /// Handles the removal of a card and reverts the ability effect if necessary.
    /// </summary>
    /// <param name="removeCard">The removed invocation card.</param>
    /// <param name="playerCards">The player cards to consider for reverting the ability effect.</param>
    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (removeCard.Title != invocationCard.Title && removeCard.Families.Contains(Family))
        {
            DecrementAtkDefInvocationCard(1);
        }
    }

    /// <summary>
    /// Handles the death of a card and resets its attack and defense to base values.
    /// </summary>
    /// <param name="canvas">The transform of the canvas.</param>
    /// <param name="deadCard">The dead invocation card.</param>
    /// <param name="playerCards">The player cards to consider for handling the card death.</param>
    /// <param name="opponentPlayerCards">The player cards associated with the opponent.</param>
    /// <returns>A boolean indicating whether the base OnCardDeath method should be executed.</returns>
    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
        }
        deadCard.Attack = deadCard.BaseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.BaseInvocationCard.BaseInvocationCardStats.Defense;
        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}

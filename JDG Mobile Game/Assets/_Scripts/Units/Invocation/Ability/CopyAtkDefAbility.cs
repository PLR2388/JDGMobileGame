using System;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability to copy the attack and defense stats of a specified card.
/// If the target card (with stats to be copied) is present in the player's invocation cards, 
/// the ability will replace the ability holder's attack and defense with the target's stats.
/// </summary>
public class CopyAtkDefAbility : Ability
{
    private readonly string cardToCopyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CopyAtkDefAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="copiedCard">The name of the card whose stats are to be copied.</param>
    public CopyAtkDefAbility(AbilityName name, string description, string copiedCard)
    {
        Name = name;
        Description = description;
        cardToCopyName = copiedCard;
    }

    /// <summary>
    /// Copies the attack and defense stats of the specified card if present.
    /// </summary>
    /// <param name="playerCards">The player's current set of cards.</param>
    private void ApplyCopy(PlayerCards playerCards)
    {
        try
        {
            InGameInvocationCard invocationCardToCopy =
                playerCards.InvocationCards.First(card => card.Title == cardToCopyName);
            invocationCard.Attack = invocationCardToCopy.Attack;
            invocationCard.Defense = invocationCardToCopy.Defense;
        }
        catch (Exception e)
        {
            // Desired card is not found among the invocation cards
            Console.WriteLine(e);
        }
    }

    /// <summary>
    /// Applies the effect to copy attack and defense stats.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <param name="opponentPlayerCards">The opponent's collection of cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyCopy(playerCards);
    }

    /// <summary>
    /// Resets the invocation card's attack and defense stats.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        ResetInvocationAttackDefense();
    }
    
    /// <summary>
    /// Resets the invocation card's attack and defense to base stats.
    /// </summary>
    private void ResetInvocationAttackDefense()
    {

        invocationCard.Attack = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Attack;
        invocationCard.Defense = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Defense;
    }

    /// <summary>
    /// Reactivates the copying effect.
    /// </summary>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyCopy(playerCards);
    }

    /// <summary>
    /// Applies the copying effect when the turn starts.
    /// </summary>
    /// <param name="canvas">The UI canvas.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    /// <param name="opponentPlayerCards">The opponent's collection of cards.</param>
    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }

        ApplyCopy(playerCards);
    }

    /// <summary>
    /// Reactivates the copy effect when a card is added.
    /// </summary>
    /// <param name="newCard">The new card added.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }

        ApplyCopy(playerCards);
    }

    /// <summary>
    /// Resets the invocation card stats if the specified card to copy is removed.
    /// </summary>
    /// <param name="removeCard">The card that was removed.</param>
    /// <param name="playerCards">The player's collection of cards.</param>
    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }

        if (removeCard.Title == cardToCopyName)
        {
            ResetInvocationAttackDefense();
        }
    }
    
}
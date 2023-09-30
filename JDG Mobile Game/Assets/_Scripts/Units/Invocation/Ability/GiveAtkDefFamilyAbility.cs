using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Represents an ability to give attack and defense values to all cards in the specified family.
/// </summary>
public class GiveAtkDefFamilyAbility : Ability
{
    private readonly CardFamily family;
    private readonly float attack;
    private readonly float defense;

    /// <summary>
    /// Initializes a new instance of the <see cref="GiveAtkDefFamilyAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="cardFamily">The card family that the ability applies to.</param>
    /// <param name="attack">The attack value to give.</param>
    /// <param name="defense">The defense value to give.</param>
    public GiveAtkDefFamilyAbility(AbilityName name, string description, CardFamily cardFamily, float attack, float defense)
    {
        Name = name;
        Description = description;
        family = cardFamily;
        this.attack = attack;
        this.defense = defense;
    }

    /// <summary>
    /// Increases the attack and defense values of the specified card.
    /// </summary>
    /// <param name="inGameInvocationCard">The card to modify.</param>
    private void IncreaseAtkDef(InGameInvocationCard inGameInvocationCard)
    {
        inGameInvocationCard.Defense += defense;
        inGameInvocationCard.Attack += attack;
    }
    
    /// <summary>
    /// Decreases the attack and defense values of the specified card.
    /// </summary>
    /// <param name="inGameInvocationCard">The card to modify.</param>
    private void DecreaseAtkDef(InGameInvocationCard inGameInvocationCard)
    {
        inGameInvocationCard.Defense -= defense;
        inGameInvocationCard.Attack -= attack;
    }
    
    /// <summary>
    /// Applies the ability effect on the player's cards.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyPower(playerCards);
    }
    
    /// <summary>
    /// Retrieves a list of cards from the specified player that belong to the ability's target family.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    /// <returns>A list of cards that match the criteria.</returns>
    private IEnumerable<InGameInvocationCard> BuildInGameInvocationCardsList(PlayerCards playerCards)
    {
        return playerCards.InvocationCards.Where(card => card.Title != invocationCard.Title && card.Families.Contains(family));
    }
    
    /// <summary>
    /// Applies the power increase to the applicable player's cards.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    private void ApplyPower(PlayerCards playerCards)
    {
        IEnumerable<InGameInvocationCard> invocationCards = BuildInGameInvocationCardsList(playerCards);
        foreach (var inGameInvocationCard in invocationCards)
        {
            IncreaseAtkDef(inGameInvocationCard);
        }
    }

    /// <summary>
    /// Reverts the ability's effect on the specified player's cards.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    public override void CancelEffect(PlayerCards playerCards)
    {
        base.CancelEffect(playerCards);
        IEnumerable<InGameInvocationCard> invocationCards = BuildInGameInvocationCardsList(playerCards);
        foreach (var inGameInvocationCard in invocationCards)
        {
            DecreaseAtkDef(inGameInvocationCard);
        }
    }

    /// <summary>
    /// Re-applies the ability's effect on the specified player's cards.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    public override void ReactivateEffect(PlayerCards playerCards)
    {
        base.ReactivateEffect(playerCards);
        ApplyPower(playerCards);
    }

    /// <summary>
    /// Checks if the specified card belongs to the ability's target family.
    /// </summary>
    /// <param name="card">The card to check.</param>
    /// <returns><c>true</c> if the card belongs to the target family; otherwise, <c>false</c>.</returns>
    private bool IsCardFromFamily(InGameInvocationCard card)
    {
        return card.Title != invocationCard.Title && card.Families.Contains(family);
    }

    /// <summary>
    /// Handles the event when a new card is added to the player's collection.
    /// </summary>
    /// <param name="newCard">The card that was added.</param>
    /// <param name="playerCards">The player's current cards.</param>
    public override void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
        base.OnCardAdded(newCard, playerCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        if (IsCardFromFamily(newCard))
        {
            IncreaseAtkDef(newCard);
        }
    }

    /// <summary>
    /// Handles the event when a card is removed from the player's collection.
    /// </summary>
    /// <param name="removeCard">The card that was removed.</param>
    /// <param name="playerCards">The player's current cards.</param>
    public override void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
        base.OnCardRemove(removeCard, playerCards);
        if (IsCardFromFamily(removeCard))
        {
            DecreaseAtkDef(removeCard);
        }
    }

    /// <summary>
    /// Handles the event when a card is defeated.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="deadCard">The card that was defeated.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    /// <returns>Result from the base method.</returns>
    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        var invocationCards = BuildInGameInvocationCardsList(playerCards);
        foreach (var inGameInvocationCard in invocationCards)
        {
            DecreaseAtkDef(inGameInvocationCard);
        }
        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}

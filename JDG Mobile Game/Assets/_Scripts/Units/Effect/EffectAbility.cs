using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Enumeration of names for various effect abilities in the game.
/// </summary>
public enum EffectAbilityName
{
    LimitHandCardTo5,
    Lose2Point5StarsByInvocations,
    ApplyFamilyFieldToInvocations,
    DestroyAllCardsUnderManyConditions,
    GetHPFor1Sacrifice3ATKDEFCondition,
    DirectAttackIfUnder5HP,
    ChangeFieldCardFromDeck,
    DestroyOneCardByRemovingOneHandCard,
    DestroyFieldFor7HalfCost,
    Get7HalfHPFor1Sacrifice,
    GetCardFromYellowDeck,
    ManiabilitePourrieSkipAttackForOpponent,
    SwitchAtkDef,
    LookAndOrderDeckCards,
    LooseHPBasedOnNumberInvocation,
    DestroyEquipmentCard,
    LookOpponentHandCardsAndChangeIt,
    DoubleAttackPerTurn,
    InvokeCardFromYellowTrash,
    DivideDEFOpponentBy2,
    Add3ShieldsForUser,
    DestroyOpponentInvocationCard,
    Loose1HPPerOpponentHandCards,
    GetBackAllHPBySacrifice5AtkDef,
    Control1OpponentInvocationCard
}

/// <summary>
/// Base class for effect abilities that can be applied in the game.
/// EffectAbilities can influence gameplay by modifying card behaviors, player statuses, etc.
/// </summary>
public abstract class EffectAbility
{
    /// <summary>
    /// Gets or sets the name of the effect ability.
    /// </summary>
    public EffectAbilityName Name { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the effect ability.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Indicates how many turns the effect lasts.
    /// </summary>
    protected int NumberOfTurn = 1;

    /// <summary>
    /// Counts the number of turns since the effect was applied.
    /// </summary>
    protected int Counter;

    /// <summary>
    /// Determines if the effect can be applied given the current game state.
    /// </summary>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <returns>True if the effect can be applied, otherwise false.</returns>
    public virtual bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return true;
    }

    /// <summary>
    /// Applies the effect ability.
    /// </summary>
    /// <param name="canvas">Reference to the game canvas.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="playerStatus">The status of the current player.</param>
    /// <param name="opponentStatus">The status of the opponent player.</param>
    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentStatus)
    {
        
    }

    /// <summary>
    /// Logic to execute at the start of a turn.
    /// </summary>
    /// <param name="canvas">Reference to the game canvas.</param>
    /// <param name="playerStatus">The status of the current player.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    public virtual void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        Counter++;
        if (Counter >= NumberOfTurn && NumberOfTurn > 0)
        {
            var effectCard = playerCards.EffectCards.First(elt => elt.EffectAbilities.Any(ability => ability.Name == Name));
            playerCards.EffectCards.Remove(effectCard);
            playerCards.YellowCards.Add(effectCard);
        }
    }

    /// <summary>
    /// Logic to execute when an invocation card is added.
    /// </summary>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="invocationCard">The invocation card being added.</param>
    public virtual void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }

    /// <summary>
    /// Logic to execute when an invocation card is removed.
    /// </summary>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="invocationCard">The invocation card being removed.</param>
    public virtual void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }
}
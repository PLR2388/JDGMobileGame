using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents an ability that switches the attack and defense values of invocation cards.
/// This effect can be applied immediately, on each turn start, or when an invocation card is added or removed.
/// </summary>
public class SwitchAtkDefEffectAbility : EffectAbility
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchAtkDefEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    public SwitchAtkDefEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }
    
    /// <summary>
    /// Switches the attack and defense values of the provided invocation card.
    /// </summary>
    /// <param name="invocationCard">The invocation card whose attack and defense values will be switched.</param>
    private static void SwitchAttackAndDefense(InGameInvocationCard invocationCard)
    {
        (invocationCard.Attack, invocationCard.Defense) = (invocationCard.Defense, invocationCard.Attack);
    }

    
    /// <summary>
    /// Applies the effect, switching the attack and defense values of all the player's invocation cards.
    /// </summary>
    /// <param name="canvas">The transform component of the game canvas.</param>
    /// <param name="playerCards">The collection of cards belonging to the player.</param>
    /// <param name="opponentPlayerCard">The collection of cards belonging to the opponent.</param>
    /// <param name="playerStatus">The status of the player.</param>
    /// <param name="opponentStatus">The status of the opponent.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        foreach (var invocationCard in playerCards.InvocationCards)
        {
            SwitchAttackAndDefense(invocationCard);
        }
    }

    /// <summary>
    /// At the start of the turn, switches the attack and defense values of all the player's invocation cards.
    /// </summary>
    /// <param name="canvas">The transform component of the game canvas.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    /// <param name="playerCards">The collection of cards belonging to the player.</param>
    /// <param name="opponentPlayerStatus">The current status of the opponent.</param>
    /// <param name="opponentPlayerCards">The collection of cards belonging to the opponent.</param>
    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        base.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCards);
        foreach (var invocationCard in playerCards.InvocationCards)
        {
            SwitchAttackAndDefense(invocationCard);
        }
    }

    /// <summary>
    /// When an invocation card is added, switches its attack and defense values.
    /// </summary>
    /// <param name="playerCards">The collection of cards belonging to the player.</param>
    /// <param name="invocationCard">The newly added invocation card.</param>
    public override void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        SwitchAttackAndDefense(invocationCard);
    }

    /// <summary>
    /// When an invocation card is removed, switches its attack and defense values.
    /// </summary>
    /// <param name="playerCards">The collection of cards belonging to the player.</param>
    /// <param name="invocationCard">The invocation card that was removed.</param>
    public override void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        SwitchAttackAndDefense(invocationCard);
    }
}

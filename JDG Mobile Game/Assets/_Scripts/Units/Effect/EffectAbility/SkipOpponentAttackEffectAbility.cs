using System.Linq;
using UnityEngine;

/// <summary>
/// Represents an ability that skips the opponent's attack for a specified number of turns.
/// </summary>
public class SkipOpponentAttackEffectAbility : EffectAbility
{
    private readonly string cardName;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SkipOpponentAttackEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The unique identifier for the effect ability.</param>
    /// <param name="description">A description of what the effect ability does.</param>
    /// <param name="cardName">The name of the card associated with this ability.</param>
    /// <param name="numberTurn">The number of turns the opponent's attack is skipped. Default value is 1.</param>
    public SkipOpponentAttackEffectAbility(EffectAbilityName name, string description, string cardName, int numberTurn = 1)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        NumberOfTurn = numberTurn;
    }

    /// <summary>
    /// Applies the effect, blocking the opponent from attacking.
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
        opponentStatus.EnableBlockAttack();
    }

    /// <summary>
    /// At the start of each turn, checks if the effect duration has expired. If expired, it re-enables the opponent's ability to attack.
    /// </summary>
    /// <param name="canvas">The transform component of the game canvas.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    /// <param name="playerCards">The collection of cards belonging to the player.</param>
    /// <param name="opponentPlayerStatus">The current status of the opponent.</param>
    /// <param name="opponentPlayerCards">The collection of cards belonging to the opponent.</param>
    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        Counter++;
        if (Counter > NumberOfTurn)
        {
            Counter = 0;
            opponentPlayerStatus.DisableBlockAttack();
            var card = playerCards.EffectCards.First(effectCard => effectCard.Title == cardName);
            playerCards.EffectCards.Remove(card);
            playerCards.YellowCards.Add(card);
        }
    }
}

using System.Linq;
using Cards;
using UnityEngine;

/// <summary>
/// Represents a field ability that allows players to earn health points (HP) based on the number
/// of cards from a specified family at the start of a turn.
/// </summary>
public class EarnHPPerFamilyOnTurnStartAbility : FieldAbility
{
    private readonly float hp;
    private readonly CardFamily family;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EarnHPPerFamilyOnTurnStartAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the field ability.</param>
    /// <param name="description">The description of the field ability.</param>
    /// <param name="hpPerInvocation">The amount of HP earned per card of the specified family.</param>
    /// <param name="family">The card family related to this ability.</param>
    public EarnHPPerFamilyOnTurnStartAbility(FieldAbilityName name, string description, float hpPerInvocation,
        CardFamily family)
    {
        Name = name;
        Description = description;
        hp = hpPerInvocation;
        this.family = family;
    }

    /// <summary>
    /// The behavior to execute at the start of a turn, where the player earns HP based on the number of cards 
    /// from the specified family they possess.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's current set of cards.</param>
    /// <param name="playerStatus">The current player's status.</param>
    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        base.OnTurnStart(canvas, playerCards, playerStatus);
        var numberCard = playerCards.InvocationCards.Count(card => card.Families.Contains(family));
        playerStatus.ChangePv(numberCard * hp);
    }
}

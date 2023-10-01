using _Scripts.Units.Invocation;

/// <summary>
/// Represents an ability that modifies an invocation card's attack and defense based on given bonus values.
/// The bonus can be static or dependent on the number of hand cards.
/// </summary>
public class EarnAtkDefAbility : EquipmentAbility
{
    private readonly float bonusDefense;
    private readonly float bonusAttack;

    private readonly bool dependOnHandCardNumber;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EarnAtkDefAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    /// <param name="bonusAtk">The bonus attack value to be applied.</param>
    /// <param name="bonusDef">The bonus defense value to be applied.</param>
    /// <param name="dependOnHandCardNumber">If true, the bonus is multiplied by the number of hand cards.</param>
    public EarnAtkDefAbility(EquipmentAbilityName name, string description, float bonusAtk, float bonusDef, bool dependOnHandCardNumber = false)
    {
        Name = name;
        Description = description;
        bonusDefense = bonusDef;
        bonusAttack = bonusAtk;
        this.dependOnHandCardNumber = dependOnHandCardNumber;
    }

    /// <summary>
    /// Applies the attack and defense bonuses to the invocation card.
    /// </summary>
    /// <param name="invocationCard">The target invocation card.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    public override void ApplyEffect(InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        base.ApplyEffect(invocationCard, playerCards, opponentPlayerCards);
        if (dependOnHandCardNumber)
        {
            var numberOfCards = playerCards.HandCards.Count;
            invocationCard.Attack += bonusAttack * numberOfCards;
            invocationCard.Defense += bonusDefense * numberOfCards;
        }
        else
        {
            invocationCard.Attack += bonusAttack;
            invocationCard.Defense += bonusDefense;
        }
    }

    /// <summary>
    /// Adjusts the attack and defense of the invocation card when the number of hand cards changes.
    /// </summary>
    /// <param name="invocationCard">The target invocation card.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="delta">The change in the number of hand cards.</param>
    public override void OnHandCardsChange(InGameInvocationCard invocationCard, PlayerCards playerCards, int delta)
    {
        base.OnHandCardsChange(invocationCard, playerCards, delta);
        if (dependOnHandCardNumber)
        {
            invocationCard.Attack += bonusAttack * delta;
            invocationCard.Defense += bonusDefense * delta;
        }
    }

    /// <summary>
    /// Removes the applied effects from the invocation card.
    /// </summary>
    /// <param name="invocationCard">The target invocation card.</param>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    public override void RemoveEffect(InGameInvocationCard invocationCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        base.RemoveEffect(invocationCard, playerCards, opponentPlayerCards);
        if (dependOnHandCardNumber)
        {
            var numberOfCards = playerCards.HandCards.Count;
            invocationCard.Attack -= bonusAttack * numberOfCards;
            invocationCard.Defense -= bonusDefense * numberOfCards;
        }
        else
        {
            invocationCard.Attack -= bonusAttack;
            invocationCard.Defense -= bonusDefense;
        }
    }
}


/// <summary>
/// Represents an ability to perform a direct attack based on the opponent's health condition.
/// </summary>
public class DirectAttackEffectAbility : EffectAbility
{
    private readonly float limitHpOpponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectAttackEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="limitHpOpponent">The health limit of the opponent to check if the effect can be used.</param>
    public DirectAttackEffectAbility(EffectAbilityName name, string description, float limitHpOpponent)
    {
        Name = name;
        Description = description;
        this.limitHpOpponent = limitHpOpponent;
    }

    /// <summary>
    /// Determines if the effect can be used based on the player and opponent's cards and status.
    /// </summary>
    /// <param name="playerCards">The player's current cards.</param>
    /// <param name="opponentPlayerCards">The opponent's current cards.</param>
    /// <param name="opponentPlayerStatus">The opponent's current status.</param>
    /// <returns><c>true</c> if the opponent's current health is below the set limit and the effect can be used; otherwise, <c>false</c>.</returns>
    public override bool CanUseEffect(PlayerCards playerCards,PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerStatus.GetCurrentHealth() < limitHpOpponent;
    }
}

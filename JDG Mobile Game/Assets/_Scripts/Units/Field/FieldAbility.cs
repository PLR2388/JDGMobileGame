using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Enumeration representing different types of field abilities.
/// </summary>
public enum FieldAbilityName
{
    Earn1DEFForSpatialFamily,
    Earn1HalfDEFAndMinusHalfATKForDevFamily,
    ChangePatronInfogramFamilyToDev,
    ChangeJMBruitagesFamilyToDev,
    Earn2DEFAndMinusOneATKForIncarnationFamily,
    EarnHalfHPPerWizardInvocationEachTurn,
    Earn1ATKForJapanFamily,
    Earn1HalfATKAndMinusHalfDEFForHCFamily,
    DrawOneMoreCard,
    EarnHalfATKAndDefForRpgFamily,
    SkipDrawToGetFistilandInvocation,
    Earn2ATKAndMinus1DEFForComicsFamily
}

/// <summary>
/// Abstract class representing a field ability.
/// </summary>
public abstract class FieldAbility
{
    /// <summary>
    /// Gets or sets the name of the field ability.
    /// </summary>
    public FieldAbilityName Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the field ability.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Applies the field ability's effect to the given player's cards.
    /// </summary>
    /// <param name="playerCards">The player's cards to apply the effect on.</param>
    public virtual void ApplyEffect(PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Method called when an invocation card is added.
    /// </summary>
    /// <param name="invocationCard">The invocation card that was added.</param>
    /// <param name="playerCards">The player's cards.</param>
    public virtual void OnInvocationCardAdded(InGameInvocationCard invocationCard,  PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Method called when a field card is removed.
    /// </summary>
    /// <param name="playerCards">The player's cards affected by the removal.</param>
    public virtual void OnFieldCardRemoved(PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Method called when an invocation card changes its family.
    /// </summary>
    /// <param name="previousFamilies">The previous families of the invocation card.</param>
    /// <param name="invocationCard">The invocation card that changed its family.</param>
    public virtual void OnInvocationChangeFamily(CardFamily[]  previousFamilies, InGameInvocationCard invocationCard)
    {
        
    }

    /// <summary>
    /// Method called at the start of a turn.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's cards for the current turn.</param>
    /// <param name="playerStatus">The current player's status.</param>
    public virtual void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        
    }

}
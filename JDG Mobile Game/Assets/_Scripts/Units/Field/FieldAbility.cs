using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

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
    SkipDrawToGetFistilandInvocation
}

public abstract class FieldAbility
{
    public FieldAbilityName Name { get; set; }

    protected string Description { get; set; }

    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards)
    {
        
    }

    public virtual void OnInvocationCardAdded(InGameInvocationCard invocationCard,  PlayerCards playerCards)
    {
        
    }

    public virtual void OnFieldCardRemoved(PlayerCards playerCards)
    {
        
    }

    public virtual void OnInvocationChangeFamily(CardFamily[]  previousFamilies, InGameInvocationCard invocationCard)
    {
        
    }

    public virtual void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        
    }

}
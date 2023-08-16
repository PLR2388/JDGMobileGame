using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

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

public abstract class EffectAbility
{
    public EffectAbilityName Name { get; set; }
    protected string Description { get; set; }

    protected int NumberOfTurn = 1;

    protected int counter = 0;

    public virtual bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return true;
    }

    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentStatus)
    {
        
    }

    public virtual void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        counter++;
        if (counter >= NumberOfTurn && NumberOfTurn > 0)
        {
            var effectCard = playerCards.effectCards.First(elt => elt.EffectAbilities.Any(ability => ability.Name == Name));
            playerCards.effectCards.Remove(effectCard);
            playerCards.yellowCards.Add(effectCard);
        }
    }

    public virtual void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }

    public virtual void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }
}
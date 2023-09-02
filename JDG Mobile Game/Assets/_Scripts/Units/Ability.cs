using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public enum AbilityName
{
    CanOnlyAttackItself,
    AddSpatialFromDeck,
    SacrificeArchibaldVonGrenier,
    CantBeAttackIfComics,
    CantLiveWithoutBenzaieOrBenzaieJeune,
    GiveAtkDefToComics,
    SendAllCardToHands,
    SacrificeBenzaieJeune,
    GetNounoursFromDeck,
    SacrificeJoueurDuGrenier,
    GetPetitePortionDeRizFromDeck,
    InvokeTentacules,
    GetLycéeMagiqueGeorgesPompidouFromDeck,
    SacrificeSebDuGrenierOnHardCornerForAtkDef,
    Win1Atk1DefDeveloper,
    Sacrifice3Atk3Def,
    ChangeFieldWithFieldFromDeck,
    Win1ATK1DefJaponWith2ATK2DEFCondition,
    InvokeDresseurBidulmon,
    GetZozanKebabFromDeck,
    SacrificeWizard,
    GetConvocationAuLyceeFromDeck,
    ProtectedBehindStarlightUnicorn,
    GetCanardSignal,
    SacrificeDeveloper3Atk3Def,
    SacrificeHardCorner3Atk3Def,
    CantBeAttackKill,
    ComesBackFromDeath,
    Sacrifice2Japan,
    DestroyFieldATK,
    KillOpponentInvocation,
    CantLiveWithoutJDG,
    GetForetElfesSylvains,
    InvokeSebOrJDG,
    CantLiveWithoutComics,
    Sacrifice2Incarnation,
    DestroyFieldDEF,
    GetBenzaieJeuneFromDeck,
    GetEquipmentCardWithoutAttack,
    SacrificeGranolax,
    SacrificeJDGOnStudioDevForAtkDef,
    CantLiveWithoutHuman,
    CopyBenzaieJeune,
    SurviveOneTurn,
    GiveDeathWhenDie,
    ProtectBehindGreaterDef,
    SacrificeSebDuGrenier,
    Win1Atk1DefFistiland,
    SacrificeClicheRaciste,
    KillEnemyIfDestroy,
    SacrificeToInvoke,
    GetPatronInfogramesFromDeckYellowTrash,
    CantLiveWithoutGranolaxOrMechaGranolax,
    SkipOpponentAttackEveryTurn,
    ComesBackFromDeath5Times,
    CantLiveWithoutJapon,
    Draw2Cards,
    GiveAktDefToRpgMember,
    GiveAktDefToFistilandMember,
    Default
}

public abstract class Ability
{
    public AbilityName Name { get; set; }
    protected string Description { get; set; }

    public bool IsAction { get; protected set; }

    protected InGameInvocationCard invocationCard;

    public InGameInvocationCard InvocationCard
    {
        set => invocationCard = value;
    }

    // Called when invocation card is put on field without special invocation
    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    public virtual void CancelEffect(PlayerCards playerCards)
    {
        
    }

    public virtual void ReactivateEffect(PlayerCards playerCards)
    {
        
    }

    // Called when a turn start
    public virtual void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    // Called when an invocation is added to field
    public virtual void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
    }

    // Called when an invocation is removed from field
    public virtual void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
    }

    public virtual void OnAttackCard(InGameInvocationCard attackedCard, InGameInvocationCard attacker, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        // Only for Sandrine (KillBothCardsIfAttackAbility)
    }

    // Called before an attack on a card (attacker belong to playerCards)
    public virtual void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        float resultAttack = attackedCard.Defense - attacker.Attack;
        if (resultAttack > 0)
        {
            var isProtected = IsEquipmentCardProtect(attacker, playerCards);

            if (isProtected == false)
            {
                var value = OnCardDeath(canvas, attacker, playerCards, opponentPlayerCards);
                if (value)
                {
                    currentPlayerStatus.ChangePv(-resultAttack);
                }
            }
        }
        else if (resultAttack == 0)
        {
            var isProtectedAttacker = IsEquipmentCardProtect(attacker, playerCards);
            var isProtectedAttacked = IsEquipmentCardProtect(attackedCard, opponentPlayerCards);
            if (!isProtectedAttacked)
            {
                OnCardDeath(canvas, attackedCard, opponentPlayerCards, playerCards);
            }

            if (!isProtectedAttacker)
            {
                OnCardDeath(canvas, attacker, playerCards, opponentPlayerCards);
            }
        }
        else
        {
            var isProtected = IsEquipmentCardProtect(attackedCard, opponentPlayerCards);
            if (!isProtected)
            {
                var value = OnCardDeath(canvas, attackedCard, opponentPlayerCards, playerCards);
                if (value)
                {
                    opponentPlayerStatus.ChangePv(resultAttack);
                }
            }
        }
    }

    private bool IsEquipmentCardProtect(InGameInvocationCard attacker, PlayerCards playerCards)
    {
        var equipmentCard = attacker.EquipmentCard;
        var isProtected = false;
        if (equipmentCard != null)
        {
            isProtected = equipmentCard.EquipmentAbilities.Any(ability => ability.OnInvocationPreDestroy(attacker, playerCards) == false);
        }

        return isProtected;
    }

    // Call when the current card having a ability die
    public virtual bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (playerCards.YellowCards.Contains(deadCard)) return false;
        var equipmentCard = deadCard.EquipmentCard;
        if (equipmentCard != null)
        {
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.RemoveEffect(deadCard, playerCards, opponentPlayerCards);
            }
            playerCards.YellowCards.Add(equipmentCard);
            deadCard.EquipmentCard = null;
        }

        deadCard.IncrementNumberDeaths();

        playerCards.InvocationCards.Remove(deadCard);
        playerCards.YellowCards.Add(deadCard);
        return true;
    }

    public virtual bool IsActionPossible(PlayerCards playerCards)
    {
        return !invocationCard.CancelEffect;
    }

    public virtual void OnCardActionTouched(Transform canvas, PlayerCards playerCards,
        PlayerCards opponentCards)
    {
    }
}
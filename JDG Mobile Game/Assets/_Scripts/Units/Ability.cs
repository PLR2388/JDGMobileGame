using System;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public enum AbilityName
{
    CanOnlyAttackAlphaMan,
    AddSpatialFromDeck,
    SacrificeArchibaldVonGrenier,
    BabsCantBeAttackIfComics,
    CantLiveWithoutBenzaieOrBenzaieJeune,
    BabsGiveAtkDefToComics,
    BebeSendAllCardToHands,
    SacrificeBenzaieJeune,
    GetNounoursFromDeck,
    SacrificeJoueurDuGrenier,
    GetPetitePortionDeRizFromDeck,
    InvokeTentacules,
    GetLycéeMagiqueGeorgesPompidouFromDeck,
    SacrificeSebDuGrenierOnHardCornerForAtkDef,
    DavidGnoufWin1Atk1DefDeveloper,
    DictateurSympaSacrifice3Atk3Def,
    ChangeFieldWithFieldFromDeck,
    DresseurBidulmonWin1ATK1DefJaponWith2ATK2DEFCondition,
    InvokeDresseurBidulmon,
    GetZozanKebabFromDeck,
    GeorgesSacrificeWizard,
    GetConvocationAuLyceeFromDeck,
    GranolaxProtectedBehindStarlightUnicorn,
    GetCanardSignal,
    JeanClaudeSacrificeDeveloper3Atk3Def,
    JeanGuySacrificeHardCornerAtk3Def,
    JeanLouisCantBeAttackKill,
    JeanMarcComesBackFromDeath,
    Koaloutre2JapanSacrifice,
    KoaloutreDestroyField,
    KillOponentInvocation,
    CantLiveWithoutJDG,
    CanOnlyAttackPetiteFille,
    GetForetElfesSylvains,
    InvokeSebOrJDG,
    BananeCantLiveWithoutComics,
    Lolhitler2IncarnationSacrifice,
    LolhitlerDestroyField,
    GetBenzaieJeuneFromDeck,
    ManuelGetEquipmentCardWithoutAttack,
    SacrificeGranolax,
    SacrificeJDGOnStudioDevForAtkDef,
    MoiseCantLiveWithoutHuman,
    NounoursCopyBenzaieJeune,
    Papy1TurnSurvive,
    PapyGiveDeathWhenDie,
    PatateProtectBehindGreaterDef,
    SacrificeSebDuGrenier,
    ProprioFistiliandWin1Atk1DefFistiland,
    SacrificeClicheRaciste,
    SandrineKillEnemyIfDestroy,
    SheikPointSacrificeToInvoke,
    GetPatronInfogramesFromDeckYellowTrash,
    CantLiveWithoutGranolaxOrMechaGranolax,
    SkipOpponentAttackEveryTurn,
    ScenaristeCanadienComesBackFromDeath5times,
    TentaculesCantLiveWithoutJapon,
    CanOnlyAttackTentacules,
    Draw2Cards,
    GiveAktDefToRpgMember,
    GiveAktDefToFistilandMember,
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

    public virtual void CancelEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        
    }

    // Called when a turn start
    public virtual void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    // Called when an invocation is added to field
    public virtual void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }

    // Called when an invocation is removed from field
    public virtual void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
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
        if (playerCards.yellowCards.Contains(deadCard)) return false;
        var equipmentCard = deadCard.EquipmentCard;
        if (equipmentCard != null)
        {
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.RemoveEffect(deadCard, playerCards, opponentPlayerCards);
            }
            playerCards.yellowCards.Add(equipmentCard);
            deadCard.EquipmentCard = null;
        }

        deadCard.IncrementNumberDeaths();

        playerCards.invocationCards.Remove(deadCard);
        playerCards.yellowCards.Add(deadCard);
        return true;
    }

    public virtual bool IsActionPossible(InGameInvocationCard currentCard, PlayerCards playerCards,
        PlayerCards opponentCards)
    {
        return !invocationCard.CancelEffect;
    }

    public virtual void OnCardActionTouched(Transform canvas, InGameInvocationCard currentCard, PlayerCards playerCards,
        PlayerCards opponentCards)
    {
    }
}
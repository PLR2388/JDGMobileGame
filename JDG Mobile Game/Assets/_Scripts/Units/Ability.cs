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
    Draw2Cards
}

public abstract class Ability
{
    public AbilityName Name { get; set; }
    protected string Description { get; set; }

    // Called when invocation card is put on field without special invocation
    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards){}
    
    // Called when a turn start
    public virtual void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards){}

    // Called when an invocation is added to field
    public virtual void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards) {}

    // Called when an invocation is removed from field
    public virtual void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards) {}

    // Called before an attack on a card (attacker belong to playerCards)
    public virtual void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        float resultAttack = attackedCard.Defense - attacker.Attack;
        if (resultAttack > 0)
        {
            OnCardDeath(canvas, attacker, playerCards);
            currentPlayerStatus.ChangePv(-resultAttack);
        }
        else if (resultAttack == 0)
        {
            OnCardDeath(canvas, attacker, playerCards);
            OnCardDeath(canvas, attackedCard, opponentPlayerCards);
        }
        else
        {
            OnCardDeath(canvas, attackedCard, opponentPlayerCards);
            opponentPlayerStatus.ChangePv(resultAttack);
        }
    }

    // Call when the current card having a ability die
    public virtual void OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards)
    {
        if (playerCards.yellowTrash.Contains(deadCard)) return;
        if (deadCard.EquipmentCard != null)
        {
            playerCards.yellowTrash.Add(deadCard.EquipmentCard);
            deadCard.EquipmentCard = null;
        }
        playerCards.yellowTrash.Add(deadCard);
        playerCards.invocationCards.Remove(deadCard);
    }
}
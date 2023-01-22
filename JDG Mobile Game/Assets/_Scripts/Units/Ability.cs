

using _Scripts.Units.Invocation;
using Cards;
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
    InvokeTentacules
}

public abstract class Ability
{

    public AbilityName Name { get; set; }
    protected string Description { get; set; }
    
    // Called when invocation card is put on field without special invocation
    public abstract void ApplyEffect(Transform canvas, PlayerCards playerCards ,PlayerCards opponentPlayerCards);
    public abstract void OnTurnStart(Transform canvas, PlayerCards playerCards ,PlayerCards opponentPlayerCards);
    public abstract void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards ,PlayerCards opponentPlayerCards);
    public abstract void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards ,PlayerCards opponentPlayerCards);
}
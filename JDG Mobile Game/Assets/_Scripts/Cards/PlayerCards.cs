using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.InvocationCards;
using UnityEngine;

/// <summary>
/// Represent all the cards of a player
/// </summary>
public class PlayerCards : MonoBehaviour
{

    #region Properties

    [SerializeField] private InvocationCard playerInvocationCard;
    [SerializeField] private Transform canvas;
    [SerializeField] private CardLocation cardLocation;
    [SerializeField] private PlayerCards opponentPlayerCards;
    [SerializeField] private bool _isPlayerOne;
    private InGameFieldCard _fieldCard;

    public bool IsPlayerOne
    {
        get => _isPlayerOne;
        private set => _isPlayerOne = value;
    }
    public bool SkipCurrentDraw { get; set; }
    public List<InGameCard> Deck = new List<InGameCard>();
    public readonly ObservableCollection<InGameCard> HandCards = new ObservableCollection<InGameCard>();
    public readonly ObservableCollection<InGameEffectCard> EffectCards = new ObservableCollection<InGameEffectCard>();
    public readonly ObservableCollection<InGameInvocationCard> InvocationCards = new ObservableCollection<InGameInvocationCard>();
    public readonly ObservableCollection<InGameCard> YellowCards = new ObservableCollection<InGameCard>();
    private List<InGameInvocationCard> oldInvocations = new List<InGameInvocationCard>();

    public InGameFieldCard FieldCard
    {
        get => _fieldCard;
        set
        {
            if (_fieldCard != value && _fieldCard != null)
            {
                foreach (var fieldCardFieldAbility in _fieldCard.FieldAbilities)
                {
                    fieldCardFieldAbility.OnFieldCardRemoved(this);
                }
            }

            _fieldCard = value;
            CardLocation.UpdateLocation.Invoke();
        }
    }

    public InGameCard Player;

    #endregion

    public void BuildPlayer()
    {
        Player = IsPlayerOne ? InGameCard.CreateInGameCard(playerInvocationCard, CardOwner.Player1) : InGameCard.CreateInGameCard(playerInvocationCard, CardOwner.Player2);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Deck = IsPlayerOne ? GameState.Instance.deckP1 : GameState.Instance.deckP2;
        UnitManager.Instance.InitPhysicalCards(Deck, IsPlayerOne);
        for (var i = Deck.Count - GameState.InitialNumberOfHandCards; i < Deck.Count; i++)
        {
            HandCards.Add(Deck[i]);
        }

        Deck.RemoveRange(Deck.Count - GameState.InitialNumberOfHandCards, GameState.InitialNumberOfHandCards);
        cardLocation.HideCards(HandCards.ToList());

        InvocationCards.CollectionChanged += InvocationCards_CollectionChanged;
        YellowCards.CollectionChanged += YellowCards_CollectionChanged;
        HandCards.CollectionChanged += HandCards_CollectionChanged;
        EffectCards.CollectionChanged += EffectCards_CollectionChanged;
    }

    /// <summary>
    /// Reset the attack number of invocations during a new turn
    /// </summary>
    public void ResetInvocationCardNewTurn()
    {
        foreach (var invocationCard in InvocationCards.Where(invocationCard =>
                     invocationCard?.Title != null))
        {
            invocationCard.ResetNewTurn();
        }
    }

    /// <summary>
    /// Check if an invocationCard is on field (among Invocation Cards)
    /// </summary>
    /// <param name="invocationCard"></param>
    /// <returns></returns>
    public bool ContainsCardInInvocation(InGameInvocationCard invocationCard)
    {
        return InvocationCards.Any(invocation => invocation != null && invocation.Title == invocationCard.Title);
    }

    private void OnDestroy()
    {
        InvocationCards.CollectionChanged -= InvocationCards_CollectionChanged;
        YellowCards.CollectionChanged -= YellowCards_CollectionChanged;
        HandCards.CollectionChanged -= HandCards_CollectionChanged;
        EffectCards.CollectionChanged -= EffectCards_CollectionChanged;
    }

    #region Event Handlers

    /// <summary>
    /// Apply powers on a new invocation card on field
    /// </summary>
    /// <param name="newInvocationCard"></param>
    private void OnInvocationCardAdded(InGameInvocationCard newInvocationCard)
    {
        foreach (var inGameInvocationCard in opponentPlayerCards.InvocationCards)
        {
            var equipmentCard = inGameInvocationCard.EquipmentCard;
            if (equipmentCard == null) continue;
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.OnOpponentInvocationCardAdded(newInvocationCard);
            }
        }

        foreach (var inGameInvocationCard in InvocationCards)
        {
            foreach (var ability in inGameInvocationCard.Abilities)
            {
                ability.OnCardAdded(newInvocationCard, this);
            }
        }

        foreach (var effectAbility in EffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectAbility.OnInvocationCardAdded(this, newInvocationCard);
        }

        if (FieldCard?.FieldAbilities != null)
        {
            foreach (var fieldAbility in FieldCard.FieldAbilities)
            {
                fieldAbility.OnInvocationCardAdded(newInvocationCard, this);
            }
        }
    }

    /// <summary>
    /// Remove power on a invocation removed from field
    /// </summary>
    private void OnInvocationCardsRemoved()
    {
        var removedInvocationCard = oldInvocations.Except(InvocationCards).First();
        var cloneInvocationCards = InvocationCards.ToList();
        // Apply onCardRemove for invocation card that are still alive
        foreach (var ability in cloneInvocationCards.SelectMany(inGameInvocationCard => inGameInvocationCard.Abilities))
        {
            ability.OnCardRemove(removedInvocationCard, this);
        }

        foreach (var effectAbility in EffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectAbility.OnInvocationCardRemoved(this, removedInvocationCard);
        }
    }

    /// <summary>
    /// Update Invocation cards location
    /// </summary>
    private void OnInvocationCardsChanged()
    {
        CardLocation.UpdateLocation.Invoke();
    }

    /// <summary>
    /// React to changes among invocation cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void InvocationCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnInvocationCardAdded(InvocationCards.Last());
                break;
            case NotifyCollectionChangedAction.Remove:
                OnInvocationCardsRemoved();
                break;
        }

        OnInvocationCardsChanged();
        oldInvocations = InvocationCards.ToList();
    }

    /// <summary>
    /// Reset and update powers for Invocation that goes to Yellow trash
    /// </summary>
    private void OnYellowTrashAdded()
    {
        var newYellowTrashCard = YellowCards.Last();
        if (newYellowTrashCard is InGameInvocationCard invocationCard)
        {
            invocationCard.UnblockAttack();
            invocationCard.Attack = invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
            invocationCard.Defense = invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
            invocationCard.FreeCard();
            invocationCard.ResetNewTurn();
            foreach (var t in invocationCard.Abilities)
            {
                t.OnCardDeath(canvas, invocationCard, this, opponentPlayerCards);
            }
        }
    }

    /// <summary>
    /// React to changes among Yellow cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void YellowCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnYellowTrashAdded();
                break;
        }

        CardLocation.UpdateLocation.Invoke();
    }

    /// <summary>
    /// React to changes among Hand cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void HandCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnHandCardsChange(1);
                break;
            case NotifyCollectionChangedAction.Remove:
                OnHandCardsChange(-1);
                break;
        }
    }

    /// <summary>
    /// React to changes among Effect cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EffectCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        CardLocation.UpdateLocation.Invoke();
    }

    /// <summary>
    /// Update powers, location and display when Hand cards change
    /// </summary>
    /// <param name="delta"></param>
    private void OnHandCardsChange(int delta)
    {
        foreach (var inGameInvocationCard in InvocationCards)
        {
            var equipmentCard = inGameInvocationCard.EquipmentCard;
            if (equipmentCard == null) continue;
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.OnHandCardsChange(inGameInvocationCard, this, delta);
            }
        }
        CardLocation.UpdateLocation.Invoke();
        HandCardDisplay.HandCardChange.Invoke(HandCards);
    }

    #endregion

}
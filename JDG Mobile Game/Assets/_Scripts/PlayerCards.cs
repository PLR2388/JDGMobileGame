using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    public List<InGameCard> deck = new List<InGameCard>();

    [SerializeField] public bool isPlayerOne;

    [SerializeField] private Transform canvas;
    [SerializeField] private CardLocation cardLocation;

    private UnitManager unitManager;


    public bool SkipCurrentDraw { get; set; }

    public List<InGameCard> secretCards = new List<InGameCard>(); // Where combine card go

    private InGameFieldCard _fieldCard;

    public InGameFieldCard FieldCard
    {
        get { return _fieldCard; }
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

    public ObservableCollection<InGameCard> handCards = new ObservableCollection<InGameCard>();
    public ObservableCollection<InGameEffectCard> effectCards = new ObservableCollection<InGameEffectCard>();

    public ObservableCollection<InGameInvocationCard> invocationCards =
        new ObservableCollection<InGameInvocationCard>();

    public ObservableCollection<InGameCard> yellowCards = new ObservableCollection<InGameCard>();

    private List<InGameInvocationCard> oldInvocations = new List<InGameInvocationCard>();

    // Start is called before the first frame update
    private void Start()
    {
        invocationCards = new ObservableCollection<InGameInvocationCard>();
        effectCards = new ObservableCollection<InGameEffectCard>();
        deck = isPlayerOne ? GameState.Instance.deckP1 : GameState.Instance.deckP2;
        UnitManager.Instance.InitPhysicalCards(deck, isPlayerOne);

        for (var i = deck.Count - 5; i < deck.Count; i++)
        {
            handCards.Add(deck[i]);
        }

        deck.RemoveRange(deck.Count - 5, 5);
        cardLocation.HideCards(handCards.ToList());

        invocationCards.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnInvocationCardAdded(invocationCards.Last());
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removedInvocationCard = oldInvocations.Except(invocationCards).First();
                    OnInvocationCardsRemoved(removedInvocationCard);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnInvocationCardsChanged();
            oldInvocations = invocationCards.ToList();
        };

        yellowCards.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnYellowTrashAdded();
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CardLocation.UpdateLocation.Invoke();
        };

        handCards.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnHandCardsChange(1);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnHandCardsChange(-1);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };

        effectCards.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
        {
            CardLocation.UpdateLocation.Invoke();
        };
    }

    private void OnHandCardsChange(int delta)
    {
        foreach (var inGameInvocationCard in invocationCards)
        {
            var equipmentCard = inGameInvocationCard.EquipmentCard;
            if (equipmentCard == null) continue;
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.OnHandCardsChange(inGameInvocationCard, this, delta);
            }
        }
        CardLocation.UpdateLocation.Invoke();
        HandCardDisplay.HandCardChange.Invoke(handCards);
    }


    public void ResetInvocationCardNewTurn()
    {
        foreach (var invocationCard in invocationCards.Where(invocationCard =>
                     invocationCard != null && invocationCard.Title != null))
        {
            invocationCard.ResetNewTurn();
        }
    }

    public bool ContainsCardInInvocation(InGameInvocationCard invocationCard)
    {
        return invocationCards.Any(invocation => invocation != null && invocation.Title == invocationCard.Title);
    }

    private void OnInvocationCardAdded(InGameInvocationCard newInvocationCard)
    {
        var opponentPlayerCard = isPlayerOne
            ? GameObject.Find("Player2").GetComponent<PlayerCards>()
            : GameObject.Find("Player1").GetComponent<PlayerCards>();

        foreach (var inGameInvocationCard in opponentPlayerCard.invocationCards)
        {
            var equipmentCard = inGameInvocationCard.EquipmentCard;
            if (equipmentCard == null) continue;
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.OnOpponentInvocationCardAdded(newInvocationCard);
            }
        }

        foreach (var inGameInvocationCard in invocationCards)
        {
            foreach (var ability in inGameInvocationCard.Abilities)
            {
                ability.OnCardAdded(newInvocationCard, this);
            }
        }

        foreach (var effectAbility in effectCards.SelectMany(effectCard => effectCard.EffectAbilities))
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

    private void OnInvocationCardsRemoved(InGameInvocationCard removedInvocationCard)
    {
        var cloneInvocationCards = invocationCards.ToList();
        // Apply onCardRemove for invocation card that are still alive
        foreach (var ability in cloneInvocationCards.SelectMany(inGameInvocationCard => inGameInvocationCard.Abilities))
        {
            ability.OnCardRemove(removedInvocationCard, this);
        }

        foreach (var effectAbility in effectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectAbility.OnInvocationCardRemoved(this, removedInvocationCard);
        }
    }

    private void OnInvocationCardsChanged()
    {
        CardLocation.UpdateLocation.Invoke();
    }

    private void OnYellowTrashAdded()
    {
        var newYellowTrashCard = yellowCards.Last();
        if (newYellowTrashCard is InGameInvocationCard invocationCard)
        {
            invocationCard.UnblockAttack();
            invocationCard.Attack = invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
            invocationCard.Defense = invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
            invocationCard.FreeCard();
            invocationCard.ResetNewTurn();
            var opponentPlayerCard = isPlayerOne
                ? GameObject.Find("Player2").GetComponent<PlayerCards>()
                : GameObject.Find("Player1").GetComponent<PlayerCards>();
            foreach (var t in invocationCard.Abilities)
            {
                t.OnCardDeath(canvas, invocationCard, this, opponentPlayerCard);
            }
        }
    }
}
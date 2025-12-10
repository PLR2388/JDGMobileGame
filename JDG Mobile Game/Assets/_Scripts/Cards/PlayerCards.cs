using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using UnityEngine;
using VContainer;

/// <summary>
/// Represent all the cards of a player.
/// Phase 8: Removed singleton dependencies (GameState, UnitManager).
/// Phase 17-18: Now uses DeckConfiguration for constants.
/// Phase 21-22: Extracted BuildPlayer and ResetInvocationCardNewTurn to use cases.
/// Phase 23: Migrated static UnityEvents to EventBus (CardLocation.UpdateLocation).
/// Uses dependency injection for deck initialization.
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

    // Phase 8: Injected dependencies
    private IDeckInitializationService _deckInitService;

    // Phase 21-22: Injected use cases
    private SummonPlayerEntityUseCase _summonPlayerEntityUseCase;
    private ResetCardsForNewTurnUseCase _resetCardsForNewTurnUseCase;
    private HandleCardDeathUseCase _handleCardDeathUseCase;
    private HandleCardAddedToFieldUseCase _handleCardAddedToFieldUseCase;
    private HandleCardRemovedFromFieldUseCase _handleCardRemovedFromFieldUseCase;
    private HandleHandCardsChangeUseCase _handleHandCardsChangeUseCase;
    private HandleFieldCardChangedUseCase _handleFieldCardChangedUseCase;

    // Phase 23: EventBus for static UnityEvent migration
    private IEventBus _eventBus;

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
            // Phase 21-22: Delegate field card change handling to use case
            if (_fieldCard != value && _fieldCard != null)
            {
                _handleFieldCardChangedUseCase.HandleFieldCardRemoved(_fieldCard, this);
            }

            _fieldCard = value;
            // Phase 23: Publish to EventBus instead of static UnityEvent
            _eventBus.Publish(new CardLocationChangedEvent { Player = null });
        }
    }

    public InGameCard Player;

    #endregion

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 8: Inject IDeckInitializationService instead of using singletons.
    /// Phase 21-22: Inject use cases for business logic extraction.
    /// Phase 23: Inject IEventBus for static UnityEvent migration.
    /// </summary>
    [Inject]
    public void Construct(
        IDeckInitializationService deckInitService,
        SummonPlayerEntityUseCase summonPlayerEntityUseCase,
        ResetCardsForNewTurnUseCase resetCardsForNewTurnUseCase,
        HandleCardDeathUseCase handleCardDeathUseCase,
        HandleCardAddedToFieldUseCase handleCardAddedToFieldUseCase,
        HandleCardRemovedFromFieldUseCase handleCardRemovedFromFieldUseCase,
        HandleHandCardsChangeUseCase handleHandCardsChangeUseCase,
        HandleFieldCardChangedUseCase handleFieldCardChangedUseCase,
        IEventBus eventBus)
    {
        _deckInitService = deckInitService;
        _summonPlayerEntityUseCase = summonPlayerEntityUseCase;
        _resetCardsForNewTurnUseCase = resetCardsForNewTurnUseCase;
        _handleCardDeathUseCase = handleCardDeathUseCase;
        _handleCardAddedToFieldUseCase = handleCardAddedToFieldUseCase;
        _handleCardRemovedFromFieldUseCase = handleCardRemovedFromFieldUseCase;
        _handleHandCardsChangeUseCase = handleHandCardsChangeUseCase;
        _handleFieldCardChangedUseCase = handleFieldCardChangedUseCase;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Creates the player entity card.
    /// Phase 21-22: Delegates to SummonPlayerEntityUseCase.
    /// </summary>
    public void BuildPlayer()
    {
        Player = _summonPlayerEntityUseCase.Execute(playerInvocationCard, IsPlayerOne);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Phase 8: Use injected service instead of GameState.Instance
        Deck = _deckInitService.GetPlayerDeck(IsPlayerOne);
        var deckLocation = CardLocation.GetDeckLocation(IsPlayerOne);

        // Phase 8: Use injected service instead of UnitManager.Instance
        _deckInitService.InitializePhysicalCards(Deck, deckLocation, IsPlayerOne);

        // Phase 17-18: Use DeckConfiguration instead of GameState for constants
        for (var i = Deck.Count - DeckConfiguration.InitialNumberOfHandCards; i < Deck.Count; i++)
        {
            HandCards.Add(Deck[i]);
        }

        Deck.RemoveRange(Deck.Count - DeckConfiguration.InitialNumberOfHandCards, DeckConfiguration.InitialNumberOfHandCards);
        cardLocation.HideCards(HandCards.ToList());

        InvocationCards.CollectionChanged += InvocationCards_CollectionChanged;
        YellowCards.CollectionChanged += YellowCards_CollectionChanged;
        HandCards.CollectionChanged += HandCards_CollectionChanged;
        EffectCards.CollectionChanged += EffectCards_CollectionChanged;
    }

    /// <summary>
    /// Reset the attack number of invocations during a new turn.
    /// Phase 21-22: Delegates to ResetCardsForNewTurnUseCase.
    /// </summary>
    public void ResetInvocationCardNewTurn()
    {
        _resetCardsForNewTurnUseCase.Execute(InvocationCards);
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
    /// React to changes among invocation cards.
    /// Phase 21-22: Delegates to use cases for business logic.
    /// Phase 23: Publishes to EventBus instead of static UnityEvent.
    /// </summary>
    private void InvocationCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                _handleCardAddedToFieldUseCase.Execute(InvocationCards.Last(), this, opponentPlayerCards);
                break;
            case NotifyCollectionChangedAction.Remove:
                var removedCard = oldInvocations.Except(InvocationCards).First();
                _handleCardRemovedFromFieldUseCase.Execute(removedCard, this);
                break;
        }

        _eventBus.Publish(new CardLocationChangedEvent { Player = IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2 });
        oldInvocations = InvocationCards.ToList();
    }

    /// <summary>
    /// React to changes among Yellow cards (graveyard).
    /// Phase 21-22: Delegates to HandleCardDeathUseCase.
    /// Phase 23: Publishes to EventBus instead of static UnityEvent.
    /// </summary>
    private void YellowCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            _handleCardDeathUseCase.Execute(YellowCards.Last(), this, opponentPlayerCards, canvas);
        }
        _eventBus.Publish(new CardLocationChangedEvent { Player = IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2 });
    }

    /// <summary>
    /// React to changes among Hand cards.
    /// Phase 21-22: Delegates to HandleHandCardsChangeUseCase.
    /// Phase 23: Publishes to EventBus instead of static UnityEvents.
    /// </summary>
    private void HandCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        int delta = e.Action == NotifyCollectionChangedAction.Add ? 1 : -1;
        _handleHandCardsChangeUseCase.Execute(this, delta);

        var domainOwner = IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        _eventBus.Publish(new CardLocationChangedEvent { Player = domainOwner });
        _eventBus.Publish(new HandCardsDisplayChangedEvent { Player = domainOwner, HandCards = HandCards });
    }

    /// <summary>
    /// React to changes among Effect cards.
    /// Phase 23: Publishes to EventBus instead of static UnityEvent.
    /// </summary>
    private void EffectCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        _eventBus.Publish(new CardLocationChangedEvent { Player = IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2 });
    }

    #endregion

}
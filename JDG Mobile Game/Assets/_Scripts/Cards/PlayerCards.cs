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
using JDG.Application.Cards;
using JDG.Application.UseCases;
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
/// Phase 53: Implements IPlayerCardCollection for abstraction.
/// Uses dependency injection for deck initialization.
/// </summary>
public class PlayerCards : MonoBehaviour, IPlayerCardCollection
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
    // Phase 84: Updated to use JDG.Application.UseCases versions
    private JDG.Application.UseCases.SummonPlayerEntityUseCase _summonPlayerEntityUseCase;
    private JDG.Application.UseCases.ResetCardsForNewTurnUseCase _resetCardsForNewTurnUseCase;
    private JDG.Application.UseCases.HandleCardDeathUseCase _handleCardDeathUseCase;
    private JDG.Application.UseCases.HandleCardAddedToFieldUseCase _handleCardAddedToFieldUseCase;
    private JDG.Application.UseCases.HandleCardRemovedFromFieldUseCase _handleCardRemovedFromFieldUseCase;
    private JDG.Application.UseCases.HandleHandCardsChangeUseCase _handleHandCardsChangeUseCase;
    private JDG.Application.UseCases.HandleFieldCardChangedUseCase _handleFieldCardChangedUseCase;

    // Phase 23: EventBus for static UnityEvent migration
    private IEventBus _eventBus;

    // Initialization guard to prevent double initialization
    private bool _isInitialized;

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
            // Phase 84: Updated to use JDG.Application.UseCases version with opponent cards
            if (_fieldCard != value && _fieldCard != null)
            {
                _handleFieldCardChangedUseCase.HandleFieldCardRemoved(_fieldCard, this, opponentPlayerCards);
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
        JDG.Application.UseCases.SummonPlayerEntityUseCase summonPlayerEntityUseCase,
        JDG.Application.UseCases.ResetCardsForNewTurnUseCase resetCardsForNewTurnUseCase,
        JDG.Application.UseCases.HandleCardDeathUseCase handleCardDeathUseCase,
        JDG.Application.UseCases.HandleCardAddedToFieldUseCase handleCardAddedToFieldUseCase,
        JDG.Application.UseCases.HandleCardRemovedFromFieldUseCase handleCardRemovedFromFieldUseCase,
        JDG.Application.UseCases.HandleHandCardsChangeUseCase handleHandCardsChangeUseCase,
        JDG.Application.UseCases.HandleFieldCardChangedUseCase handleFieldCardChangedUseCase,
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

        // Guard against double initialization (can happen if VContainer injects twice)
        if (_isInitialized)
        {
            Debug.LogWarning($"PlayerCards.Construct() - Already initialized for IsPlayerOne={IsPlayerOne}, skipping");
            return;
        }
        _isInitialized = true;

        // Initialize deck immediately after injection, before any Start() runs.
        // This fixes the race condition where GameLoop.Start() tries to draw
        // before PlayerCards.Start() has initialized the deck.
        Deck = _deckInitService.GetPlayerDeck(IsPlayerOne);

        // Create physical card GameObjects immediately in Construct() so they exist
        // before any Start() methods run. This fixes the timing issue where
        // GameLoop.Start() or other Start() methods tried to access cards that
        // weren't yet created.
        var deckLocation = CardLocation.GetDeckLocation(IsPlayerOne);
        _deckInitService.InitializePhysicalCards(Deck, deckLocation, IsPlayerOne);

        Debug.Log($"PlayerCards.Construct() - IsPlayerOne={IsPlayerOne}, Deck={Deck?.Count ?? 0} cards, Physical cards created");
        // Note: BuildPlayer() is called in Start() to ensure SerializeFields are populated
    }

    /// <summary>
    /// Creates the player entity card.
    /// Phase 21-22: Delegates to SummonPlayerEntityUseCase.
    /// Phase 84: Updated to use JDG.Application.UseCases version.
    /// Phase 139: Added proper error handling and logging.
    /// </summary>
    public void BuildPlayer()
    {
        if (playerInvocationCard == null)
        {
            Debug.LogError($"PlayerCards.BuildPlayer() - playerInvocationCard SerializeField is NOT assigned in Inspector! IsPlayerOne={IsPlayerOne}. " +
                "Please assign the Player invocation card in the Unity Inspector.");
            return;
        }

        var result = _summonPlayerEntityUseCase.Execute(playerInvocationCard, IsPlayerOne);

        if (!result.IsSuccess)
        {
            Debug.LogError($"PlayerCards.BuildPlayer() - Failed to create player entity! IsPlayerOne={IsPlayerOne}, Message: {result.Message}");
            return;
        }

        Player = result.EntityCard as InGameCard;

        if (Player == null)
        {
            Debug.LogError($"PlayerCards.BuildPlayer() - EntityCard cast to InGameCard returned null! IsPlayerOne={IsPlayerOne}");
        }
        else
        {
            // Phase 140: Add success logging for debugging target building
            Debug.Log($"PlayerCards.BuildPlayer() - SUCCESS! IsPlayerOne={IsPlayerOne}, Player.Title='{Player.Title}', Player.GetType()={Player.GetType().Name}");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log($"PlayerCards.Start() - IsPlayerOne={IsPlayerOne}, Deck.Count={Deck?.Count ?? 0}");

        // Phase 137: Build player entity in Start() to ensure SerializeFields are populated
        // (Construct() is called by VContainer before Unity populates SerializeFields)
        BuildPlayer();
        Debug.Log($"PlayerCards.Start() - Player entity created: {Player?.Title ?? "null"}");

        // Note: Deck and physical cards are initialized in Construct() to ensure
        // they're ready before any Start() methods run.

        // Move initial hand cards from deck to hand
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
    /// Phase 84: Updated to use JDG.Application.UseCases version.
    /// </summary>
    public void ResetInvocationCardNewTurn()
    {
        _resetCardsForNewTurnUseCase.Execute(InvocationCards.Cast<IInGameInvocationCard>());
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
            // Phase 65: Removed canvas parameter - now injected via ICanvasProvider
            _handleCardDeathUseCase.Execute(YellowCards.Last(), this, opponentPlayerCards);
        }
        _eventBus.Publish(new CardLocationChangedEvent { Player = IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2 });
    }

    /// <summary>
    /// React to changes among Hand cards.
    /// Phase 21-22: Delegates to HandleHandCardsChangeUseCase.
    /// Phase 23: Publishes to EventBus instead of static UnityEvents.
    /// Phase 84: Updated to use JDG.Application.UseCases version with opponent cards.
    /// </summary>
    private void HandCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        int delta = e.Action == NotifyCollectionChangedAction.Add ? 1 : -1;
        _handleHandCardsChangeUseCase.Execute(this, opponentPlayerCards, delta);

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

    #region IPlayerCardCollection Implementation

    /// <summary>
    /// Gets the card owner for this collection.
    /// Phase 53: Added for IPlayerCardCollection interface.
    /// </summary>
    JDG.Domain.CardOwner IPlayerCardCollection.Owner =>
        IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;

    /// <summary>
    /// Gets the invocation cards as a read-only list of interface types.
    /// Phase 53: Explicit implementation for IPlayerCardCollection interface.
    /// </summary>
    IReadOnlyList<IInGameInvocationCard> IPlayerCardCollection.InvocationCards =>
        InvocationCards.Cast<IInGameInvocationCard>().ToList().AsReadOnly();

    /// <summary>
    /// Gets the effect cards as a read-only list of interface types.
    /// Phase 53: Explicit implementation for IPlayerCardCollection interface.
    /// </summary>
    IReadOnlyList<IInGameEffectCard> IPlayerCardCollection.EffectCards =>
        EffectCards.Cast<IInGameEffectCard>().ToList().AsReadOnly();

    /// <summary>
    /// Gets the current field card as an interface type.
    /// Phase 53: Explicit implementation for IPlayerCardCollection interface.
    /// </summary>
    IInGameFieldCard IPlayerCardCollection.FieldCard => _fieldCard;

    /// <summary>
    /// Gets the cards in hand as a read-only list of interface types.
    /// Phase 53: Explicit implementation for IPlayerCardCollection interface.
    /// </summary>
    IReadOnlyList<IInGameCard> IPlayerCardCollection.HandCards =>
        HandCards.Cast<IInGameCard>().ToList().AsReadOnly();

    /// <summary>
    /// Gets the count of cards in hand.
    /// Phase 53: Added for IPlayerCardCollection interface.
    /// </summary>
    int IPlayerCardCollection.HandCardCount => HandCards.Count;

    #endregion

}
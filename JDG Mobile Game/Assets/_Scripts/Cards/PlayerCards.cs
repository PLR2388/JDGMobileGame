using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Cards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Cards;
using JDG.Application.UseCases;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Cards;
using UnityEngine;
using VContainer;

/// <summary>
/// Represent all the cards of a player.
/// Phase 8: Removed singleton dependencies (GameState, UnitManager).
/// Phase 17-18: Now uses DeckConfiguration for constants.
/// Phase 21-22: Extracted BuildPlayer and ResetInvocationCardNewTurn to use cases.
/// Phase 23: Migrated static UnityEvents to EventBus (CardLocation.UpdateLocation).
/// Phase 53: Implements IPlayerCardCollection for abstraction.
/// Phase 144: Implements IPlayerCardCollectionMutable for mutation operations.
/// Uses dependency injection for deck initialization.
/// </summary>
public class PlayerCards : MonoBehaviour, IPlayerCardCollectionMutable
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

    // Phase 140: Card pool service for registering player entity
    private ICardPoolService _cardPoolService;

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
            // Phase 156: Fixed - was publishing null Player, now uses proper owner
            var domainOwner = IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
            _eventBus.Publish(new CardLocationChangedEvent { Player = domainOwner });
        }
    }

    public InGameCard Player;

    #endregion

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 8: Inject IDeckInitializationService instead of using singletons.
    /// Phase 21-22: Inject use cases for business logic extraction.
    /// Phase 23: Inject IEventBus for static UnityEvent migration.
    /// Phase 140: Inject ICardPoolService for player entity registration.
    /// </summary>
    /// <summary>
    /// Phase 156: Added null checks for critical dependencies.
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
        IEventBus eventBus,
        ICardPoolService cardPoolService)
    {
        // Phase 156: Validate critical dependencies
        _deckInitService = deckInitService ?? throw new System.ArgumentNullException(
            nameof(deckInitService), "PlayerCards requires IDeckInitializationService");
        _eventBus = eventBus ?? throw new System.ArgumentNullException(
            nameof(eventBus), "PlayerCards requires IEventBus for event publishing");
        _cardPoolService = cardPoolService ?? throw new System.ArgumentNullException(
            nameof(cardPoolService), "PlayerCards requires ICardPoolService for card pooling");

        // Use cases - store with null checks
        _summonPlayerEntityUseCase = summonPlayerEntityUseCase ?? throw new System.ArgumentNullException(
            nameof(summonPlayerEntityUseCase), "PlayerCards requires SummonPlayerEntityUseCase");
        _resetCardsForNewTurnUseCase = resetCardsForNewTurnUseCase ?? throw new System.ArgumentNullException(
            nameof(resetCardsForNewTurnUseCase), "PlayerCards requires ResetCardsForNewTurnUseCase");
        _handleCardDeathUseCase = handleCardDeathUseCase ?? throw new System.ArgumentNullException(
            nameof(handleCardDeathUseCase), "PlayerCards requires HandleCardDeathUseCase");
        _handleCardAddedToFieldUseCase = handleCardAddedToFieldUseCase ?? throw new System.ArgumentNullException(
            nameof(handleCardAddedToFieldUseCase), "PlayerCards requires HandleCardAddedToFieldUseCase");
        _handleCardRemovedFromFieldUseCase = handleCardRemovedFromFieldUseCase ?? throw new System.ArgumentNullException(
            nameof(handleCardRemovedFromFieldUseCase), "PlayerCards requires HandleCardRemovedFromFieldUseCase");
        _handleHandCardsChangeUseCase = handleHandCardsChangeUseCase ?? throw new System.ArgumentNullException(
            nameof(handleHandCardsChangeUseCase), "PlayerCards requires HandleHandCardsChangeUseCase");
        _handleFieldCardChangedUseCase = handleFieldCardChangedUseCase ?? throw new System.ArgumentNullException(
            nameof(handleFieldCardChangedUseCase), "PlayerCards requires HandleFieldCardChangedUseCase");

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

#if UNITY_EDITOR
        Debug.Log($"PlayerCards.Construct() - IsPlayerOne={IsPlayerOne}, Deck={Deck?.Count ?? 0} cards, Physical cards created");
#endif

        // Phase 155: Warn if deck is empty - may indicate TutoSceneInitializer order issue in tutorial scenes
        if (Deck == null || Deck.Count == 0)
        {
            Debug.LogWarning($"PlayerCards.Construct() - Deck is empty for IsPlayerOne={IsPlayerOne}. " +
                "In tutorial scenes, ensure TutoSceneInitializer is injected BEFORE PlayerCards (order matters in GameSceneScope). " +
                "In normal gameplay, verify DeckInitializationService and deck data.");
        }

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
#if UNITY_EDITOR
            Debug.Log($"PlayerCards.BuildPlayer() - SUCCESS! IsPlayerOne={IsPlayerOne}, Player.Title='{Player.Title}', Player.GetType()={Player.GetType().Name}");
#endif

            // Phase 140: Register Player entity with card pool so it can be displayed in attack target selector
            _cardPoolService?.AddCardToPool(Player);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log($"PlayerCards.Start() - IsPlayerOne={IsPlayerOne}, Deck.Count={Deck?.Count ?? 0}");
#endif

        // Phase 137: Build player entity in Start() to ensure SerializeFields are populated
        // (Construct() is called by VContainer before Unity populates SerializeFields)
        BuildPlayer();
#if UNITY_EDITOR
        Debug.Log($"PlayerCards.Start() - Player entity created: {Player?.Title ?? "null"}");
#endif

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
                var removedCard = oldInvocations.Except(InvocationCards).FirstOrDefault();
                if (removedCard == null)
                {
                    Debug.LogWarning("PlayerCards: No removed card found in collection difference");
                    break;
                }
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
    /// Phase 144: Note - Uses LINQ Cast which performs runtime type checking.
    /// Safe because InvocationCards only contains InGameInvocationCard instances
    /// which implement IInGameInvocationCard. Creates a new list each call.
    /// </summary>
    IReadOnlyList<IInGameInvocationCard> IPlayerCardCollection.InvocationCards =>
        InvocationCards.Cast<IInGameInvocationCard>().ToList().AsReadOnly();

    /// <summary>
    /// Gets the effect cards as a read-only list of interface types.
    /// Phase 53: Explicit implementation for IPlayerCardCollection interface.
    /// Phase 144: Note - Uses LINQ Cast which performs runtime type checking.
    /// Safe because EffectCards only contains InGameEffectCard instances
    /// which implement IInGameEffectCard. Creates a new list each call.
    /// </summary>
    IReadOnlyList<IInGameEffectCard> IPlayerCardCollection.EffectCards =>
        EffectCards.Cast<IInGameEffectCard>().ToList().AsReadOnly();

    /// <summary>
    /// Gets the current field card as an interface type.
    /// Phase 53: Explicit implementation for IPlayerCardCollection interface.
    /// </summary>
    IInGameFieldCard IPlayerCardCollection.FieldCard => _fieldCard;

    /// <summary>
    /// Gets the graveyard cards as a read-only list of interface types.
    /// Phase 165: Explicit implementation for IPlayerCardCollection interface.
    /// Wraps the legacy YellowCards collection.
    /// </summary>
    IReadOnlyList<IInGameCard> IPlayerCardCollection.GraveyardCards =>
        YellowCards.Cast<IInGameCard>().ToList().AsReadOnly();

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

    #region IPlayerCardCollectionMutable Implementation

    /// <summary>
    /// Adds a card to the player's hand.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    void IPlayerCardCollectionMutable.AddToHand(IInGameCard card)
    {
        if (card is InGameCard concreteCard)
        {
            HandCards.Add(concreteCard);
        }
        else
        {
            Debug.LogError($"[PlayerCards.AddToHand] Expected InGameCard but got {card?.GetType().Name ?? "null"}");
        }
    }

    /// <summary>
    /// Removes a card from the player's hand.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    bool IPlayerCardCollectionMutable.RemoveFromHand(IInGameCard card)
    {
        if (card is InGameCard concreteCard)
        {
            return HandCards.Remove(concreteCard);
        }
        Debug.LogError($"[PlayerCards.RemoveFromHand] Expected InGameCard but got {card?.GetType().Name ?? "null"}");
        return false;
    }

    /// <summary>
    /// Adds an invocation card to the field.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    void IPlayerCardCollectionMutable.AddToField(IInGameInvocationCard card)
    {
        if (card is InGameInvocationCard concreteCard)
        {
            InvocationCards.Add(concreteCard);
        }
        else
        {
            Debug.LogError($"[PlayerCards.AddToField] Expected InGameInvocationCard but got {card?.GetType().Name ?? "null"}");
        }
    }

    /// <summary>
    /// Removes an invocation card from the field.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    bool IPlayerCardCollectionMutable.RemoveFromField(IInGameInvocationCard card)
    {
        if (card is InGameInvocationCard concreteCard)
        {
            return InvocationCards.Remove(concreteCard);
        }
        Debug.LogError($"[PlayerCards.RemoveFromField] Expected InGameInvocationCard but got {card?.GetType().Name ?? "null"}");
        return false;
    }

    /// <summary>
    /// Adds an effect card to the field.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    void IPlayerCardCollectionMutable.AddEffectToField(IInGameEffectCard card)
    {
        if (card is InGameEffectCard concreteCard)
        {
            EffectCards.Add(concreteCard);
        }
        else
        {
            Debug.LogError($"[PlayerCards.AddEffectToField] Expected InGameEffectCard but got {card?.GetType().Name ?? "null"}");
        }
    }

    /// <summary>
    /// Removes an effect card from the field.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    bool IPlayerCardCollectionMutable.RemoveEffectFromField(IInGameEffectCard card)
    {
        if (card is InGameEffectCard concreteCard)
        {
            return EffectCards.Remove(concreteCard);
        }
        Debug.LogError($"[PlayerCards.RemoveEffectFromField] Expected InGameEffectCard but got {card?.GetType().Name ?? "null"}");
        return false;
    }

    /// <summary>
    /// Sets the field card (replaces any existing field card).
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    void IPlayerCardCollectionMutable.SetFieldCard(IInGameFieldCard card)
    {
        if (card == null)
        {
            FieldCard = null;
        }
        else if (card is InGameFieldCard concreteCard)
        {
            FieldCard = concreteCard;
        }
        else
        {
            Debug.LogError($"[PlayerCards.SetFieldCard] Expected InGameFieldCard but got {card.GetType().Name}");
        }
    }

    /// <summary>
    /// Adds a card to the graveyard.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    void IPlayerCardCollectionMutable.AddToGraveyard(IInGameCard card)
    {
        if (card is InGameCard concreteCard)
        {
            YellowCards.Add(concreteCard);
        }
        else
        {
            Debug.LogError($"[PlayerCards.AddToGraveyard] Expected InGameCard but got {card?.GetType().Name ?? "null"}");
        }
    }

    /// <summary>
    /// Removes a card from the graveyard (e.g., for resurrection abilities).
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    bool IPlayerCardCollectionMutable.RemoveFromGraveyard(IInGameCard card)
    {
        if (card is InGameCard concreteCard)
        {
            return YellowCards.Remove(concreteCard);
        }
        Debug.LogError($"[PlayerCards.RemoveFromGraveyard] Expected InGameCard but got {card?.GetType().Name ?? "null"}");
        return false;
    }

    /// <summary>
    /// Draws the top card from the deck.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    IInGameCard IPlayerCardCollectionMutable.DrawFromDeck()
    {
        if (Deck.Count == 0)
        {
            return null;
        }

        // Draw from end (last card is top of deck)
        var card = Deck[Deck.Count - 1];
        Deck.RemoveAt(Deck.Count - 1);
        return card;
    }

    /// <summary>
    /// Adds a card to the top of the deck.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    void IPlayerCardCollectionMutable.AddToDeck(IInGameCard card)
    {
        if (card is InGameCard concreteCard)
        {
            Deck.Add(concreteCard); // Add to end (top of deck)
        }
        else
        {
            Debug.LogError($"[PlayerCards.AddToDeck] Expected InGameCard but got {card?.GetType().Name ?? "null"}");
        }
    }

    /// <summary>
    /// Removes a specific card from the deck.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    bool IPlayerCardCollectionMutable.RemoveFromDeck(IInGameCard card)
    {
        if (card is InGameCard concreteCard)
        {
            return Deck.Remove(concreteCard);
        }
        Debug.LogError($"[PlayerCards.RemoveFromDeck] Expected InGameCard but got {card?.GetType().Name ?? "null"}");
        return false;
    }

    /// <summary>
    /// Gets the number of cards remaining in the deck.
    /// Phase 144: Explicit implementation for IPlayerCardCollectionMutable interface.
    /// </summary>
    int IPlayerCardCollectionMutable.DeckCount => Deck.Count;

    #endregion

}
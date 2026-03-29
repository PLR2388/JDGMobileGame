using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cards;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Cards;
using JDG.Infrastructure.Services;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

/// <summary>
/// Phase 23: Migrated from static UnityEvent to EventBus subscription.
/// Phase 24-25: Removed ServiceLocator, using VContainer DI.
/// </summary>
public class HandCardDisplay : MonoBehaviour
{
    [SerializeField] protected GameObject prefabCard;

    protected readonly List<GameObject> CreatedCards = new List<GameObject>();

    // Phase 24-25: Injected via VContainer
    private GameStateService _gameStateService;
    private bool IsP1Turn => _gameStateService.CurrentPlayer == PlayerId.Player1;

    // Phase 23: EventBus for static UnityEvent migration
    private IEventBus _eventBus;
    private IDisposable _handCardsSubscription;

    // Phase 132: VContainer resolver for injecting dynamically created components
    protected VContainer.IObjectResolver _container;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 23: Inject IEventBus for static UnityEvent migration.
    /// Phase 24-25: Inject GameStateService instead of ServiceLocator.
    /// Phase 132: Inject IObjectResolver to inject dynamically created OnHover components.
    /// Phase 156: Added null checks to fail early if DI is not properly configured.
    /// </summary>
    [Inject]
    public void Construct(IEventBus eventBus, GameStateService gameStateService, VContainer.IObjectResolver container)
    {
        _eventBus = eventBus ?? throw new System.ArgumentNullException(
            nameof(eventBus),
            "HandCardDisplay requires IEventBus for event subscriptions.");
        _gameStateService = gameStateService ?? throw new System.ArgumentNullException(
            nameof(gameStateService),
            "HandCardDisplay requires GameStateService for turn state.");
        _container = container ?? throw new System.ArgumentNullException(
            nameof(container),
            "HandCardDisplay requires IObjectResolver for injecting dynamically created components.");

        // Subscribe immediately after injection since Awake/OnEnable may have already run
        SubscribeToEvents();
#if UNITY_EDITOR
        Debug.Log("HandCardDisplay.Construct: Dependencies injected and subscribed");
#endif
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Note: VContainer injection happens AFTER Awake(), so subscription is done in Construct().
    /// </summary>
    private void Awake()
    {
        // Don't subscribe here - _eventBus is not yet injected
        // Subscription is done in Construct() after injection
    }

    /// <summary>
    /// Displays hand cards if they belong to the current player.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void DisplayHandCard(ObservableCollection<InGameCard> handCards)
    {
#if UNITY_EDITOR
        Debug.Log($"HandCardDisplay.DisplayHandCard: Received {handCards.Count} cards, " +
            $"IsP1Turn={IsP1Turn}, " +
            $"FirstCardOwner={(handCards.Count > 0 ? handCards[0].CardOwner.ToString() : "N/A")}");
#endif

        if (handCards.Count == 0 || IsCurrentPlayerTurn(handCards[0]))
        {
#if UNITY_EDITOR
            Debug.Log($"HandCardDisplay.DisplayHandCard: Building {handCards.Count} cards");
#endif
            BuildCards(handCards);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"HandCardDisplay.DisplayHandCard: Skipping - not current player's turn");
#endif
        }
    }
    
    /// <summary>
    /// Checks if the provided card belongs to the current player.
    /// </summary>
    /// <param name="card">In-game card to check.</param>
    /// <returns>True if card belongs to current player; otherwise, false.</returns>
    protected bool IsCurrentPlayerTurn(InGameCard card)
    {
        return IsP1Turn == (card.CardOwner == CardOwner.Player1);
    }
    
    /// <summary>
    /// Clears and then creates visual representations for the provided cards.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void BuildCards(ObservableCollection<InGameCard> handCards)
    {
        ClearCreatedCards();
        CreateCards(handCards);
    }
    
    /// <summary>
    /// Creates visual representations for the provided cards.
    /// Protected virtual to allow tutorial-specific overrides for card highlighting.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    protected virtual void CreateCards(ObservableCollection<InGameCard> handCards)
    {
        if (prefabCard == null)
        {
            Debug.LogError("HandCardDisplay.CreateCards: prefabCard is not assigned in Inspector!");
            return;
        }

        foreach (var handCard in handCards)
        {
            var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
            newCard.transform.SetParent(transform, true);

            var cardDisplay = newCard.GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.InGameCard = handCard;
            }
            else
            {
                Debug.LogError($"HandCardDisplay.CreateCards: CardDisplay component not found on prefab for card '{handCard.Title}'");
            }

            var onHover = newCard.GetComponent<OnHover>();
            if (onHover != null)
            {
                onHover.bIsInGame = true;
                // Phase 132: Inject IEventBus and ICardSelectionService into dynamically created OnHover
                _container?.Inject(onHover);
            }

            CreatedCards.Add(newCard);
#if UNITY_EDITOR
            Debug.Log($"HandCardDisplay.CreateCards: Created card '{handCard.Title}'");
#endif
        }

        AdjustRectTransformSize(handCards.Count);
    }
    
    /// <summary>
    /// Adjusts the RectTransform size based on the number of cards.
    /// </summary>
    /// <param name="cardCount">Number of cards.</param>
    protected void AdjustRectTransformSize(int cardCount)
    {
        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("HandCardDisplay.AdjustRectTransformSize: RectTransform component not found on GameObject");
            return;
        }
        rectTransform.sizeDelta = new UnityEngine.Vector2(420 * cardCount, rectTransform.sizeDelta.y);
    }
    
    /// <summary>
    /// Destroys created card game objects and clears the list.
    /// </summary>
    protected void ClearCreatedCards()
    {
        foreach (var createdCard in CreatedCards)
        {
            Destroy(createdCard);
        }
        CreatedCards.Clear();
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Phase 156: Only resubscribes if previously injected (handles re-enable after OnDisable).
    /// Initial subscription happens in Construct() after DI injection.
    /// </summary>
    private void OnEnable()
    {
        // Only resubscribe if _eventBus was already injected (re-enable scenario)
        // Initial subscription is handled in Construct() after injection
        if (_eventBus != null)
        {
            SubscribeToEvents();
        }
    }

    /// <summary>
    /// Called when the behaviour becomes disabled.
    /// Unsubscribes from events and clears created cards.
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromEvents();
        ClearCreatedCards();
    }
    
    /// <summary>
    /// Subscribes to hand card change events.
    /// Phase 23: Subscribes to EventBus instead of static UnityEvent.
    /// </summary>
    protected void SubscribeToEvents()
    {
        // Guard: Only subscribe if not already subscribed (prevents double subscription
        // when both Construct() and OnEnable() call this method)
        if (_handCardsSubscription != null)
        {
            return;
        }
        _handCardsSubscription = _eventBus?.Subscribe<HandCardsDisplayChangedEvent>(OnHandCardsDisplayChanged);
    }

    /// <summary>
    /// Unsubscribes from hand card change events.
    /// Phase 23: Disposes EventBus subscription.
    /// </summary>
    protected void UnsubscribeFromEvents()
    {
        _handCardsSubscription?.Dispose();
        _handCardsSubscription = null; // Clear reference so SubscribeToEvents() can resubscribe
    }

    /// <summary>
    /// Event handler for HandCardsDisplayChangedEvent.
    /// Phase 23: Replaces static UnityEvent listener.
    /// </summary>
    private void OnHandCardsDisplayChanged(HandCardsDisplayChangedEvent evt)
    {
#if UNITY_EDITOR
        Debug.Log($"HandCardDisplay.OnHandCardsDisplayChanged: Event received, Player={evt.Player}, " +
            $"HandCards type={(evt.HandCards?.GetType().Name ?? "null")}, " +
            $"gameObject.activeInHierarchy={gameObject.activeInHierarchy}");
#endif

        if (evt.HandCards is ObservableCollection<InGameCard> handCards)
        {
            DisplayHandCard(handCards);
        }
        else
        {
            Debug.LogWarning($"HandCardDisplay: HandCards is not ObservableCollection<InGameCard>, " +
                $"actual type={(evt.HandCards?.GetType().Name ?? "null")}");
        }
    }
}
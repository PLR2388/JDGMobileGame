using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cards;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Services;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

/// <summary>
/// Phase 23: Migrated from static UnityEvent to EventBus subscription.
/// </summary>
public class HandCardDisplay : MonoBehaviour
{
    [SerializeField] protected GameObject prefabCard;

    protected readonly List<GameObject> CreatedCards = new List<GameObject>();

    // Phase 2: Temporary bridge to GameStateService during migration
    // This will be removed when HandCardDisplay is refactored in Phase 6
    private GameStateService GameStateService => ServiceLocator.Get<GameStateService>();
    private bool IsP1Turn => GameStateService.CurrentPlayer == PlayerId.Player1;

    // Phase 23: EventBus for static UnityEvent migration
    private IEventBus _eventBus;
    private IDisposable _handCardsSubscription;

    /// <summary>
    /// VContainer method injection for EventBus.
    /// Phase 23: Inject IEventBus for static UnityEvent migration.
    /// </summary>
    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Subscribes to relevant events.
    /// </summary>
    private void Awake()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Displays hand cards if they belong to the current player.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void DisplayHandCard(ObservableCollection<InGameCard> handCards)
    {
        if (handCards.Count == 0 || IsCurrentPlayerTurn(handCards[0]))
        {
            BuildCards(handCards);
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
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void CreateCards(ObservableCollection<InGameCard> handCards)
    {
        foreach (var handCard in handCards)
        {
            var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
            newCard.transform.SetParent(transform, true);
            newCard.GetComponent<CardDisplay>().InGameCard = handCard;
            newCard.GetComponent<OnHover>().bIsInGame = true;

            CreatedCards.Add(newCard);
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
    /// Subscribes to relevant events.
    /// </summary>
    private void OnEnable()
    {
        SubscribeToEvents();
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
        _handCardsSubscription = _eventBus?.Subscribe<HandCardsDisplayChangedEvent>(OnHandCardsDisplayChanged);
    }

    /// <summary>
    /// Unsubscribes from hand card change events.
    /// Phase 23: Disposes EventBus subscription.
    /// </summary>
    protected void UnsubscribeFromEvents()
    {
        _handCardsSubscription?.Dispose();
    }

    /// <summary>
    /// Event handler for HandCardsDisplayChangedEvent.
    /// Phase 23: Replaces static UnityEvent listener.
    /// </summary>
    private void OnHandCardsDisplayChanged(HandCardsDisplayChangedEvent evt)
    {
        if (evt.HandCards is ObservableCollection<InGameCard> handCards)
        {
            DisplayHandCard(handCards);
        }
    }
}
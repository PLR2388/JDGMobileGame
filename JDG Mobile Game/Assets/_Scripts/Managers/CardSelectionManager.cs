using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using VContainer;

/// <summary>
/// Adapter MonoBehaviour that bridges legacy UnityEvent-based code to new ICardSelectionService.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// Phase 28: Migrated to service pattern - delegates to CardSelectionService, maintains UnityEvents for backward compatibility.
/// </summary>
public class CardSelectionManager : MonoBehaviour
{
    // Legacy UnityEvents - intentionally maintained for backward compatibility with older UI code
    // Phase 144: These will be removed once all subscribers migrate to EventBus
    public UnityEvent<InGameCard> CardSelected = new UnityEvent<InGameCard>();
    public UnityEvent<InGameCard> CardDeselected = new UnityEvent<InGameCard>();
    public UnityEvent SelectionChanged = new UnityEvent();

    // Phase 28: Delegate to service
    private ICardSelectionService _cardSelectionService;
    private IEventBus _eventBus;

    // Phase 144: Store subscriptions for proper disposal
    private IDisposable _cardAddedSubscription;
    private IDisposable _cardRemovedSubscription;
    private IDisposable _selectionChangedSubscription;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 28: Injects service and event bus.
    /// </summary>
    [Inject]
    public void Construct(ICardSelectionService cardSelectionService, IEventBus eventBus)
    {
        _cardSelectionService = cardSelectionService;
        _eventBus = eventBus;

        // Subscribe to domain events and forward to UnityEvents for backward compatibility
        // Phase 144: Store subscriptions for disposal in OnDestroy
        _cardAddedSubscription = _eventBus.Subscribe<CardAddedToSelectionEvent>(OnCardAddedToSelectionEvent);
        _cardRemovedSubscription = _eventBus.Subscribe<CardRemovedFromSelectionEvent>(OnCardRemovedFromSelectionEvent);
        _selectionChangedSubscription = _eventBus.Subscribe<CardSelectionChangedEvent>(OnCardSelectionChangedEvent);
    }

    private void OnCardAddedToSelectionEvent(CardAddedToSelectionEvent evt)
    {
        if (evt.Card is InGameCard card)
        {
            CardSelected.Invoke(card);
        }
    }

    private void OnCardRemovedFromSelectionEvent(CardRemovedFromSelectionEvent evt)
    {
        if (evt.Card is InGameCard card)
        {
            CardDeselected.Invoke(card);
        }
    }

    private void OnCardSelectionChangedEvent(CardSelectionChangedEvent evt)
    {
        SelectionChanged.Invoke();
    }

    /// <summary>
    /// Gets the list of currently selected cards.
    /// Phase 28: Delegates to service.
    /// </summary>
    public List<InGameCard> SelectedCards =>
        _cardSelectionService.SelectedCards.OfType<InGameCard>().ToList();

    /// <summary>
    /// Gets or sets whether multiple cards can be selected simultaneously.
    /// Phase 28: Delegates to service.
    /// </summary>
    public bool MultipleCardSelection
    {
        get => _cardSelectionService.MultipleCardSelection;
        set => _cardSelectionService.MultipleCardSelection = value;
    }

    /// <summary>
    /// Gets or sets the maximum number of cards that can be selected.
    /// Phase 28: Delegates to service.
    /// </summary>
    public int MultipleSelectionLimit
    {
        get => _cardSelectionService.MultipleSelectionLimit;
        set => _cardSelectionService.MultipleSelectionLimit = value;
    }

    /// <summary>
    /// Selects a card.
    /// Phase 28: Delegates to service.
    /// </summary>
    public void SelectCard(InGameCard card)
    {
        _cardSelectionService.SelectCard(card);
    }

    /// <summary>
    /// Unselects a card.
    /// Phase 28: Delegates to service.
    /// </summary>
    public void UnselectCard(InGameCard card)
    {
        _cardSelectionService.UnselectCard(card);
    }

    /// <summary>
    /// Clears all selected cards.
    /// Phase 28: Delegates to service.
    /// </summary>
    public void ClearSelection()
    {
        _cardSelectionService.ClearSelection();
    }

    /// <summary>
    /// Checks if a card is currently selected.
    /// Phase 28: Delegates to service.
    /// </summary>
    public bool IsCardSelected(InGameCard card)
    {
        return _cardSelectionService.IsCardSelected(card);
    }

    /// <summary>
    /// Phase 144: Fixed - dispose EventBus subscriptions along with UnityEvents.
    /// </summary>
    private void OnDestroy()
    {
        // Phase 144: Dispose EventBus subscriptions
        _cardAddedSubscription?.Dispose();
        _cardRemovedSubscription?.Dispose();
        _selectionChangedSubscription?.Dispose();

        // Clean up legacy UnityEvents
        CardSelected.RemoveAllListeners();
        CardDeselected.RemoveAllListeners();
        SelectionChanged.RemoveAllListeners();
    }
}
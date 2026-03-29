using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Infrastructure.Cards;
using VContainer;

/// <summary>
/// Adapter MonoBehaviour that bridges legacy code to ICardSelectionService.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// Phase 28: Migrated to service pattern - delegates to CardSelectionService.
/// Phase 146: Removed legacy UnityEvents (no remaining subscribers).
/// </summary>
public class CardSelectionManager : MonoBehaviour
{
    // Phase 28: Delegate to service
    private ICardSelectionService _cardSelectionService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 28: Injects service.
    /// Phase 146: Removed EventBus forwarding (no UnityEvent subscribers remain).
    /// </summary>
    [Inject]
    public void Construct(ICardSelectionService cardSelectionService)
    {
        _cardSelectionService = cardSelectionService;
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

    // Phase 146: OnDestroy removed - no subscriptions or UnityEvents to clean up
}
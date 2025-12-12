using System.Collections.Generic;
using Cards;
using UnityEngine.Events;

/// <summary>
/// LEGACY adapter service that bridges ICardSelectionService to CardSelectionManager.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Phase 19-20: Now injects CardSelectionManager instead of using .Instance.
///
/// Phase 32: This implements the LEGACY ICardSelectionService (global namespace).
/// For new code, use JDG.Infrastructure.Services.CardSelectionService which implements
/// the clean JDG.Application.Services.ICardSelectionService.
/// </summary>
[System.Obsolete("Legacy adapter. Use JDG.Infrastructure.Services.CardSelectionService for new code.")]
public class CardSelectionService : ICardSelectionService
{
    private readonly CardSelectionManager _cardSelectionManager;

    public CardSelectionService(CardSelectionManager cardSelectionManager)
    {
        _cardSelectionManager = cardSelectionManager;
    }

    public UnityEvent<InGameCard> CardSelected => _cardSelectionManager.CardSelected;
    public UnityEvent<InGameCard> CardDeselected => _cardSelectionManager.CardDeselected;
    public UnityEvent SelectionChanged => _cardSelectionManager.SelectionChanged;

    public List<InGameCard> SelectedCards => _cardSelectionManager.SelectedCards;

    public bool MultipleCardSelection
    {
        get => _cardSelectionManager.MultipleCardSelection;
        set => _cardSelectionManager.MultipleCardSelection = value;
    }

    public int MultipleSelectionLimit
    {
        get => _cardSelectionManager.MultipleSelectionLimit;
        set => _cardSelectionManager.MultipleSelectionLimit = value;
    }

    public void SelectCard(InGameCard card)
    {
        _cardSelectionManager.SelectCard(card);
    }

    public void UnselectCard(InGameCard card)
    {
        _cardSelectionManager.UnselectCard(card);
    }

    public void ClearSelection()
    {
        _cardSelectionManager.ClearSelection();
    }

    public bool IsCardSelected(InGameCard card)
    {
        return _cardSelectionManager.IsCardSelected(card);
    }
}

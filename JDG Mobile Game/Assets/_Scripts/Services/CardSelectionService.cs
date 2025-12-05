using System.Collections.Generic;
using Cards;
using UnityEngine.Events;

/// <summary>
/// Implementation of ICardSelectionService.
/// Bridges to legacy CardSelectionManager singleton during migration.
///
/// Part of Phase 9 - Removes singleton dependencies from UI components.
/// This service will be refactored once CardSelectionManager lifecycle is managed by DI.
/// </summary>
public class CardSelectionService : ICardSelectionService
{
    public UnityEvent<InGameCard> CardSelected => CardSelectionManager.Instance?.CardSelected;
    public UnityEvent<InGameCard> CardDeselected => CardSelectionManager.Instance?.CardDeselected;
    public UnityEvent SelectionChanged => CardSelectionManager.Instance?.SelectionChanged;

    public List<InGameCard> SelectedCards => CardSelectionManager.Instance?.SelectedCards ?? new List<InGameCard>();

    public bool MultipleCardSelection
    {
        get => CardSelectionManager.Instance?.MultipleCardSelection ?? false;
        set
        {
            if (CardSelectionManager.Instance != null)
                CardSelectionManager.Instance.MultipleCardSelection = value;
        }
    }

    public int MultipleSelectionLimit
    {
        get => CardSelectionManager.Instance?.MultipleSelectionLimit ?? 1;
        set
        {
            if (CardSelectionManager.Instance != null)
                CardSelectionManager.Instance.MultipleSelectionLimit = value;
        }
    }

    public void SelectCard(InGameCard card)
    {
        CardSelectionManager.Instance?.SelectCard(card);
    }

    public void UnselectCard(InGameCard card)
    {
        CardSelectionManager.Instance?.UnselectCard(card);
    }

    public void ClearSelection()
    {
        CardSelectionManager.Instance?.ClearSelection();
    }

    public bool IsCardSelected(InGameCard card)
    {
        return CardSelectionManager.Instance?.IsCardSelected(card) ?? false;
    }
}

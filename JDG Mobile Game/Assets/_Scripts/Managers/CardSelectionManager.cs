using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cards;

/// <summary>
/// Manages card selection state and events.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// TODO Phase 23: Migrate UnityEvents to EventBus.
/// </summary>
public class CardSelectionManager : MonoBehaviour
{
    // Events
    public UnityEvent<InGameCard> CardSelected = new UnityEvent<InGameCard>();
    public UnityEvent<InGameCard> CardDeselected = new UnityEvent<InGameCard>();
    public UnityEvent SelectionChanged = new UnityEvent();

    public List<InGameCard> SelectedCards => selectedCards;

    private List<InGameCard> selectedCards = new List<InGameCard>();
    public bool MultipleCardSelection { get; set; } = false;
    public int MultipleSelectionLimit { get; set; } = 1;

    public void SelectCard(InGameCard card)
    {
        if (!MultipleCardSelection && selectedCards.Count > 0)
        {
            ClearSelection();
        }

        if (selectedCards.Count >= MultipleSelectionLimit)
        {
            selectedCards.RemoveAt(0);
            // Handle according to your preference, e.g., remove the first card, prevent further selection, etc.
            return;
        }

        if (!selectedCards.Contains(card))
        {
            selectedCards.Add(card);
            CardSelected.Invoke(card);
            SelectionChanged.Invoke();
        }
    }

    public void UnselectCard(InGameCard card)
    {
        if (selectedCards.Remove(card))
        {
            CardDeselected.Invoke(card);
            SelectionChanged.Invoke();
        }
    }

    public void ClearSelection()
    {
        while (selectedCards.Count > 0)
        {
            UnselectCard(selectedCards[0]);
        }
    }

    public bool IsCardSelected(InGameCard card)
    {
        return selectedCards.Contains(card);
    }

    private void OnDestroy()
    {
        CardDeselected.RemoveAllListeners();
        CardDeselected.RemoveAllListeners();
    }
}
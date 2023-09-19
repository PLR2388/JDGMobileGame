using System.Collections.Generic;
using UnityEngine.Events;
using Cards;

public class CardSelectionManager: StaticInstance<CardSelectionManager>
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
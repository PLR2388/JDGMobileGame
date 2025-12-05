using System.Collections.Generic;
using Cards;
using UnityEngine.Events;

/// <summary>
/// Service for managing card selection state.
/// Replaces CardSelectionManager singleton access.
/// Part of Phase 9 - Remaining singleton elimination.
///
/// Note: This interface is in the default assembly because it references legacy types
/// (InGameCard, UnityEvent). Will be migrated once InGameCard is replaced with domain Card entity.
/// </summary>
public interface ICardSelectionService
{
    /// <summary>
    /// Event fired when a card is selected.
    /// </summary>
    UnityEvent<InGameCard> CardSelected { get; }

    /// <summary>
    /// Event fired when a card is deselected.
    /// </summary>
    UnityEvent<InGameCard> CardDeselected { get; }

    /// <summary>
    /// Event fired when selection changes.
    /// </summary>
    UnityEvent SelectionChanged { get; }

    /// <summary>
    /// Gets the list of currently selected cards.
    /// </summary>
    List<InGameCard> SelectedCards { get; }

    /// <summary>
    /// Gets or sets whether multiple card selection is enabled.
    /// </summary>
    bool MultipleCardSelection { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of cards that can be selected.
    /// </summary>
    int MultipleSelectionLimit { get; set; }

    /// <summary>
    /// Selects a card.
    /// </summary>
    void SelectCard(InGameCard card);

    /// <summary>
    /// Unselects a card.
    /// </summary>
    void UnselectCard(InGameCard card);

    /// <summary>
    /// Clears all selected cards.
    /// </summary>
    void ClearSelection();

    /// <summary>
    /// Checks if a card is selected.
    /// </summary>
    bool IsCardSelected(InGameCard card);
}

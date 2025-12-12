using System.Collections.Generic;
using Cards;
using UnityEngine.Events;

/// <summary>
/// LEGACY service interface for managing card selection state.
/// Part of Phase 9 - Replaces CardSelectionManager singleton access.
///
/// IMPORTANT: This is a LEGACY interface in the global namespace.
/// There is also a CLEAN interface at JDG.Application.Services.ICardSelectionService.
///
/// Phase 32: Both interfaces coexist during migration:
/// - This interface: Uses InGameCard and UnityEvents, for legacy code (OnHover, CardChoice, etc.)
/// - Clean interface: Uses object and EventBus, for new code following Clean Architecture
///
/// Migration path: Gradually update code to use JDG.Application.Services.ICardSelectionService,
/// then delete this legacy interface once no code depends on it.
/// </summary>
[System.Obsolete("Legacy interface. Use JDG.Application.Services.ICardSelectionService for new code.")]
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

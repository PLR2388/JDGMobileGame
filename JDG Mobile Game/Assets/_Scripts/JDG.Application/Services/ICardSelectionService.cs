using System.Collections.Generic;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service interface for managing card selection state.
    /// Part of Phase 28 - MonoBehaviour Wave 1 service extraction.
    /// Replaces legacy CardSelectionManager MonoBehaviour.
    /// </summary>
    public interface ICardSelectionService
    {
        /// <summary>
        /// Gets the list of currently selected cards.
        /// </summary>
        IReadOnlyList<object> SelectedCards { get; }

        /// <summary>
        /// Gets or sets whether multiple cards can be selected simultaneously.
        /// Default: false (single selection mode).
        /// </summary>
        bool MultipleCardSelection { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of cards that can be selected in multiple selection mode.
        /// Default: 1.
        /// </summary>
        int MultipleSelectionLimit { get; set; }

        /// <summary>
        /// Selects a card. Publishes CardSelectedEvent.
        /// In single selection mode, clears previous selection first.
        /// </summary>
        /// <param name="card">The card to select</param>
        void SelectCard(object card);

        /// <summary>
        /// Unselects a card. Publishes CardDeselectedEvent if card was selected.
        /// </summary>
        /// <param name="card">The card to unselect</param>
        void UnselectCard(object card);

        /// <summary>
        /// Clears all selected cards.
        /// Publishes CardDeselectedEvent for each card.
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// Checks if a card is currently selected.
        /// </summary>
        /// <param name="card">The card to check</param>
        /// <returns>True if the card is selected, false otherwise</returns>
        bool IsCardSelected(object card);
    }
}

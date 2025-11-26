using System;
using System.Collections.Generic;
using JDG.Application.DTOs;

namespace JDG.Presentation.Views
{
    /// <summary>
    /// View interface for displaying cards in a player's hand.
    /// </summary>
    public interface ICardHandView
    {
        /// <summary>
        /// Event fired when a card in hand is clicked/selected.
        /// </summary>
        event Action<Guid> OnCardSelected;

        /// <summary>
        /// Updates the hand display with the current cards.
        /// </summary>
        void UpdateHand(IEnumerable<CardDTO> cards);

        /// <summary>
        /// Highlights a specific card in hand.
        /// </summary>
        void HighlightCard(Guid cardId);

        /// <summary>
        /// Clears any card highlights.
        /// </summary>
        void ClearHighlights();

        /// <summary>
        /// Enables or disables card interaction.
        /// </summary>
        void SetInteractable(bool interactable);
    }
}

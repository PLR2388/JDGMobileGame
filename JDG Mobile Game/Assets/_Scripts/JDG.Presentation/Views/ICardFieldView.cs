using System;
using System.Collections.Generic;
using JDG.Application.DTOs;

namespace JDG.Presentation.Views
{
    /// <summary>
    /// View interface for displaying cards on the field (play area).
    /// </summary>
    public interface ICardFieldView
    {
        /// <summary>
        /// Event fired when a card on the field is clicked/selected.
        /// </summary>
        event Action<Guid> OnCardSelected;

        /// <summary>
        /// Updates the field display with current cards.
        /// </summary>
        void UpdateField(IEnumerable<CardDTO> playerCards, IEnumerable<CardDTO> opponentCards);

        /// <summary>
        /// Animates a card being played to the field.
        /// </summary>
        void AnimateCardPlayed(CardDTO card);

        /// <summary>
        /// Animates a card being destroyed/removed.
        /// </summary>
        void AnimateCardDestroyed(Guid cardId);

        /// <summary>
        /// Highlights valid attack targets.
        /// </summary>
        void HighlightAttackTargets(IEnumerable<Guid> targetCardIds);

        /// <summary>
        /// Clears all highlights.
        /// </summary>
        void ClearHighlights();

        /// <summary>
        /// Animates an attack between two cards.
        /// </summary>
        void AnimateAttack(Guid attackerId, Guid defenderId);
    }
}

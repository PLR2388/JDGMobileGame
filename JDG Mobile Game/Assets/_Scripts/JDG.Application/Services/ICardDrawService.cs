using UnityEngine.Events;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for card drawing operations.
    /// Replaces CardManager's card drawing responsibilities.
    /// Part of Phase 4 - CardManager decomposition.
    /// </summary>
    public interface ICardDrawService
    {
        /// <summary>
        /// Draws a card for the current player.
        /// </summary>
        /// <param name="onNoCard">Callback invoked when there are no cards left to draw.</param>
        void DrawCard(UnityAction onNoCard);
    }
}

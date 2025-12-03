using Cards;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for accessing player card collections.
    /// Replaces CardManager's card access responsibilities.
    /// Part of Phase 4 - CardManager decomposition.
    /// </summary>
    public interface ICardCollectionService
    {
        /// <summary>
        /// Gets the card collection for the current player.
        /// </summary>
        PlayerCards GetCurrentPlayerCards();

        /// <summary>
        /// Gets the card collection for the opponent player.
        /// </summary>
        PlayerCards GetOpponentPlayerCards();
    }
}

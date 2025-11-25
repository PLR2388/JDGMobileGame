using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Repositories
{
    /// <summary>
    /// Repository interface for managing player entities.
    /// Defines operations for accessing and persisting player state.
    /// </summary>
    public interface IPlayerRepository
    {
        /// <summary>
        /// Gets a player by their ID.
        /// </summary>
        Player GetPlayer(PlayerId playerId);

        /// <summary>
        /// Saves the current state of a player.
        /// </summary>
        void SavePlayer(Player player);

        /// <summary>
        /// Creates a new player with the specified deck.
        /// </summary>
        Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30);

        /// <summary>
        /// Resets a player to initial state with their deck.
        /// </summary>
        void ResetPlayer(PlayerId playerId);
    }
}

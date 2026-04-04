using System.Collections.Generic;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Repositories
{
    /// <summary>
    /// Repository interface for managing deck configurations.
    /// Handles loading and saving deck compositions.
    /// </summary>
    public interface IDeckRepository
    {
        /// <summary>
        /// Gets a deck configuration by name.
        /// Returns array of CardIds representing the deck.
        /// </summary>
        CardId[] GetDeck(string deckName);

        /// <summary>
        /// Saves a deck configuration.
        /// </summary>
        void SaveDeck(string deckName, CardId[] cardIds);

        /// <summary>
        /// Gets all available deck names.
        /// </summary>
        IEnumerable<string> GetAllDeckNames();

        /// <summary>
        /// Deletes a deck configuration.
        /// </summary>
        void DeleteDeck(string deckName);

        /// <summary>
        /// Gets the default deck for a player.
        /// </summary>
        CardId[] GetDefaultDeck(PlayerId playerId);
    }
}

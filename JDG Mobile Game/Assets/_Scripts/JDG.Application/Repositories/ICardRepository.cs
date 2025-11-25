using System.Collections.Generic;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Repositories
{
    /// <summary>
    /// Repository interface for accessing card data.
    /// Provides read-only access to card definitions from ScriptableObjects.
    /// </summary>
    public interface ICardRepository
    {
        /// <summary>
        /// Gets a card entity by its ID.
        /// </summary>
        Card GetCard(CardId cardId);

        /// <summary>
        /// Creates a new card instance from a card definition (ScriptableObject).
        /// Generates a new unique CardId for the instance.
        /// </summary>
        Card CreateCardInstance(string cardDefinitionName);

        /// <summary>
        /// Gets all available card definitions.
        /// </summary>
        IEnumerable<Card> GetAllCardDefinitions();

        /// <summary>
        /// Searches for card definitions by type.
        /// </summary>
        IEnumerable<Card> GetCardsByType(CardType type);

        /// <summary>
        /// Searches for card definitions by family.
        /// </summary>
        IEnumerable<Card> GetCardsByFamily(CardFamily family);

        /// <summary>
        /// Gets a card definition by its title.
        /// </summary>
        Card GetCardByTitle(string title);
    }
}

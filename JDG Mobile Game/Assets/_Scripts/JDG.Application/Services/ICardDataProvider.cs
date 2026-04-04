using System.Collections.Generic;

namespace JDG.Application.Services
{
    /// <summary>
    /// Provides access to card data loaded from resources.
    /// Phase 8: Replaces ResourceSystem.Instance singleton access.
    /// </summary>
    public interface ICardDataProvider
    {
        /// <summary>
        /// Gets all available cards loaded from resources.
        /// </summary>
        List<object> GetAllCards();

        /// <summary>
        /// Gets a card by its title/name.
        /// </summary>
        /// <param name="title">The title of the card.</param>
        /// <returns>The card with the given title, or null if not found.</returns>
        object GetCardByName(string title);

        /// <summary>
        /// Gets whether the card data has been loaded.
        /// </summary>
        bool IsLoaded { get; }
    }
}

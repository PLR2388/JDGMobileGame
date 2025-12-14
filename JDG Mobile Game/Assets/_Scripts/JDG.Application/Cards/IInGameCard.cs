using JDG.Domain;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing a card in the game.
    /// Phase 41: Created to abstract card dependencies for use cases.
    /// Enables use cases to be moved to Application layer and tested without Unity.
    /// </summary>
    public interface IInGameCard
    {
        /// <summary>
        /// Gets the title of the card.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the card owner (Player1, Player2, or NotDefined).
        /// </summary>
        CardOwner CardOwner { get; }
    }
}

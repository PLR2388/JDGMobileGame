using JDG.Domain;
using JDG.Domain.Enums;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing a card in the game.
    /// Phase 41: Created to abstract card dependencies for use cases.
    /// Phase 39: Extended with additional properties for presenter migration.
    /// Phase 126: Added CardId for multilanguage localization support.
    /// Enables use cases and presenters to be moved to their proper assemblies and tested without Unity.
    /// </summary>
    public interface IInGameCard
    {
        /// <summary>
        /// Gets the card ID for localization lookup.
        /// Generated from the ScriptableObject asset name (e.g., "alpha-man").
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        string CardId { get; }

        /// <summary>
        /// Gets the title of the card.
        /// Phase 126: Now supports lazy localization with fallback.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the card owner (Player1, Player2, or NotDefined).
        /// </summary>
        CardOwner CardOwner { get; }

        /// <summary>
        /// Gets the type classification of the card (Invocation, Equipment, Field, Effect, Contre).
        /// Phase 39: Added for presenter and use case migration.
        /// </summary>
        CardType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the card is a collector's item.
        /// Phase 39: Added for presenter migration.
        /// </summary>
        bool Collector { get; }

        /// <summary>
        /// Gets the brief description of the card.
        /// Phase 39: Added for UI display purposes.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the detailed description or lore of the card.
        /// Phase 39: Added for UI display purposes.
        /// </summary>
        string DetailedDescription { get; }

        /// <summary>
        /// Gets the unique identifier for the card's visual (material/image).
        /// This is an abstract identifier - use ICardVisualService to resolve to actual visuals.
        /// Phase 39: Added to abstract Unity Material dependency.
        /// </summary>
        string VisualId { get; }
    }
}

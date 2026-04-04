using System.Collections.Generic;
using JDG.Domain;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing a player's card collection.
    /// Phase 41: Created to abstract PlayerCards dependencies for use cases.
    /// Enables use cases to be tested without coupling to the concrete PlayerCards class.
    /// </summary>
    public interface IPlayerCardCollection
    {
        /// <summary>
        /// Gets whether this collection belongs to Player 1.
        /// </summary>
        bool IsPlayerOne { get; }

        /// <summary>
        /// Gets the card owner for this collection.
        /// </summary>
        CardOwner Owner { get; }

        /// <summary>
        /// Gets the invocation cards on the field.
        /// </summary>
        IReadOnlyList<IInGameInvocationCard> InvocationCards { get; }

        /// <summary>
        /// Gets the effect cards on the field.
        /// </summary>
        IReadOnlyList<IInGameEffectCard> EffectCards { get; }

        /// <summary>
        /// Gets the current field card, if any.
        /// </summary>
        IInGameFieldCard FieldCard { get; }

        /// <summary>
        /// Gets the cards in the graveyard (yellow cards / dead cards).
        /// Phase 165: Added for Condition migration to JDG.Cards assembly.
        /// </summary>
        IReadOnlyList<IInGameCard> GraveyardCards { get; }

        /// <summary>
        /// Gets the cards in hand.
        /// </summary>
        IReadOnlyList<IInGameCard> HandCards { get; }

        /// <summary>
        /// Gets the count of cards in hand.
        /// </summary>
        int HandCardCount { get; }
    }
}

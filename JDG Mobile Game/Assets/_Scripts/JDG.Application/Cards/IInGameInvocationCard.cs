using System.Collections.Generic;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing an invocation card in the game.
    /// Phase 41: Created to abstract InGameInvocationCard dependencies for use cases.
    /// Extends IInGameCard with invocation-specific properties and methods.
    /// </summary>
    public interface IInGameInvocationCard : IInGameCard
    {
        /// <summary>
        /// Gets or sets the current attack value.
        /// </summary>
        float Attack { get; set; }

        /// <summary>
        /// Gets or sets the current defense value.
        /// </summary>
        float Defense { get; set; }

        /// <summary>
        /// Gets the base attack value (from card definition).
        /// </summary>
        float BaseAttack { get; }

        /// <summary>
        /// Gets the base defense value (from card definition).
        /// </summary>
        float BaseDefense { get; }

        /// <summary>
        /// Gets the abilities associated with this card.
        /// Uses object to avoid coupling to legacy Ability type.
        /// </summary>
        IReadOnlyList<object> Abilities { get; }

        /// <summary>
        /// Gets the equipment card attached to this invocation, if any.
        /// Returns null if no equipment is attached.
        /// </summary>
        IInGameEquipmentCard EquipmentCard { get; }

        /// <summary>
        /// Resets the card state for a new turn.
        /// Resets attack counts and turn-based state.
        /// </summary>
        void ResetNewTurn();

        /// <summary>
        /// Unblocks the card's ability to attack.
        /// </summary>
        void UnblockAttack();

        /// <summary>
        /// Frees the card from any control or equipment effects.
        /// </summary>
        void FreeCard();
    }
}

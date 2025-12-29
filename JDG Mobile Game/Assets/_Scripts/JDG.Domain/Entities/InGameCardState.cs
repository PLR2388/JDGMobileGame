using System;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Entities
{
    /// <summary>
    /// Domain entity representing the runtime state of a card in play.
    /// Pure C# - no Unity dependencies.
    /// ECS-ready: Structured to easily convert to ECS components.
    ///
    /// Phase 67: Created as part of UseCase migration to enable pure domain use cases.
    ///
    /// This separates:
    /// - Card definition (Card entity, ScriptableObject) - immutable card data
    /// - Card runtime state (InGameCardState) - mutable gameplay state
    /// - Card presentation (InGameCard MonoBehaviour) - Unity visuals
    /// </summary>
    public class InGameCardState
    {
        /// <summary>
        /// Unique identifier for this card instance.
        /// </summary>
        public CardId Id { get; }

        /// <summary>
        /// Reference to the card definition (for accessing Title, Type, etc.).
        /// </summary>
        public CardId CardDefinitionId { get; }

        /// <summary>
        /// The card type (Invocation, Equipment, Field, Effect, Contre).
        /// Cached from card definition for quick access.
        /// </summary>
        public CardType Type { get; }

        /// <summary>
        /// The owner of this card (Player1, Player2, NotDefined).
        /// </summary>
        public CardOwner Owner { get; set; }

        /// <summary>
        /// Whether the card's effects are canceled (e.g., by equipment ability).
        /// </summary>
        public bool CancelEffect { get; set; }

        /// <summary>
        /// Whether this card is currently blocked from attacking.
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Whether this card is currently controlled by the opponent.
        /// </summary>
        public bool IsControlled { get; set; }

        /// <summary>
        /// Number of turns this card has been on the field.
        /// Reset when card leaves the field.
        /// </summary>
        public int TurnsOnField { get; set; }

        /// <summary>
        /// Protected constructor - use factory methods or derived classes.
        /// </summary>
        protected InGameCardState(
            CardId id,
            CardId cardDefinitionId,
            CardType type,
            CardOwner owner)
        {
            Id = id;
            CardDefinitionId = cardDefinitionId;
            Type = type;
            Owner = owner;
            CancelEffect = false;
            IsBlocked = false;
            IsControlled = false;
            TurnsOnField = 0;
        }

        /// <summary>
        /// Creates a new InGameCardState for a generic card.
        /// Use derived class factory methods for specific card types.
        /// </summary>
        public static InGameCardState Create(
            CardId cardDefinitionId,
            CardType type,
            CardOwner owner)
        {
            return new InGameCardState(
                CardId.New(),
                cardDefinitionId,
                type,
                owner);
        }

        /// <summary>
        /// Increments the turn counter for this card on the field.
        /// </summary>
        public void IncrementTurnCount()
        {
            TurnsOnField++;
        }

        /// <summary>
        /// Resets field-specific state when card leaves the field.
        /// </summary>
        public virtual void ResetFieldState()
        {
            TurnsOnField = 0;
            IsBlocked = false;
            IsControlled = false;
        }

        /// <summary>
        /// Controls this card (transfers control to opponent).
        /// </summary>
        public void Control()
        {
            IsControlled = true;
        }

        /// <summary>
        /// Frees this card from opponent control.
        /// </summary>
        public void Free()
        {
            IsControlled = false;
        }

        public override string ToString()
        {
            return $"InGameCardState[{Id}] Type={Type}, Owner={Owner}";
        }
    }
}

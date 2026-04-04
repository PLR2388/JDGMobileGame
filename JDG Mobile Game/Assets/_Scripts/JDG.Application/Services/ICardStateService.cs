using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service interface for managing card state operations.
    /// Phase 71: Created as part of UseCase migration.
    ///
    /// Provides domain logic for common card state operations.
    /// Use cases can depend on this service to avoid duplicating logic.
    ///
    /// Responsibilities:
    /// - Reset card state for new turns
    /// - Apply damage and stat modifications
    /// - Handle card death preparation
    /// </summary>
    public interface ICardStateService
    {
        #region Turn Management

        /// <summary>
        /// Resets an invocation card's state for a new turn.
        /// Restores attack count, unblocks if needed.
        /// </summary>
        void ResetForNewTurn(InvocationCardState state);

        /// <summary>
        /// Increments the turn counter for a card on the field.
        /// </summary>
        void IncrementTurnOnField(InGameCardState state);

        #endregion

        #region Combat

        /// <summary>
        /// Applies damage to an invocation card.
        /// Returns true if the card is destroyed (defense <= 0).
        /// </summary>
        bool ApplyDamage(InvocationCardState state, float damage);

        /// <summary>
        /// Checks if an invocation card can attack.
        /// </summary>
        bool CanAttack(InvocationCardState state);

        /// <summary>
        /// Records that an invocation card performed an attack.
        /// </summary>
        void RecordAttack(InvocationCardState state);

        #endregion

        #region Stats

        /// <summary>
        /// Modifies an invocation card's stats by delta values.
        /// </summary>
        void ModifyStats(InvocationCardState state, float attackDelta, float defenseDelta);

        /// <summary>
        /// Resets an invocation card's stats to base values.
        /// </summary>
        void ResetToBaseStats(InvocationCardState state);

        /// <summary>
        /// Sets an invocation card's stats to specific values.
        /// </summary>
        void SetStats(InvocationCardState state, float attack, float defense);

        #endregion

        #region Death/Field Removal

        /// <summary>
        /// Prepares a card for death (graveyard transfer).
        /// Resets state, unblocks, frees from control.
        /// Does NOT handle ability triggers - use HandleCardDeathUseCase for that.
        /// </summary>
        void PrepareForDeath(InvocationCardState state);

        /// <summary>
        /// Prepares a card for field removal.
        /// Similar to death but doesn't increment death counter.
        /// </summary>
        void PrepareForFieldRemoval(InGameCardState state);

        #endregion

        #region Control

        /// <summary>
        /// Takes control of an invocation card (transfers to opponent).
        /// </summary>
        void TakeControl(InvocationCardState state);

        /// <summary>
        /// Frees an invocation card from opponent control.
        /// </summary>
        void ReleaseControl(InvocationCardState state);

        #endregion
    }
}

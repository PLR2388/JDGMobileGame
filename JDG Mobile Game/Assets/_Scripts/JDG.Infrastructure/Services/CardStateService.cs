using JDG.Application.Services;
using JDG.Domain.Entities;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Implementation of ICardStateService for managing card state operations.
    /// Phase 71: Created as part of UseCase migration.
    /// Phase 96: Moved to JDG.Infrastructure for proper testability.
    ///
    /// Provides domain logic for common card state operations.
    /// Pure C# logic - no Unity dependencies.
    /// </summary>
    public class CardStateService : ICardStateService
    {
        #region Turn Management

        public void ResetForNewTurn(InvocationCardState state)
        {
            if (state == null) return;
            state.ResetForNewTurn();
        }

        public void IncrementTurnOnField(InGameCardState state)
        {
            if (state == null) return;
            state.IncrementTurnCount();
        }

        #endregion

        #region Combat

        public bool ApplyDamage(InvocationCardState state, float damage)
        {
            if (state == null) return false;

            state.CurrentDefense -= damage;
            return state.IsDestroyed;
        }

        public bool CanAttack(InvocationCardState state)
        {
            if (state == null) return false;
            return state.CanAttack();
        }

        public void RecordAttack(InvocationCardState state)
        {
            if (state == null) return;
            state.AttackPerformed();
        }

        #endregion

        #region Stats

        public void ModifyStats(InvocationCardState state, float attackDelta, float defenseDelta)
        {
            if (state == null) return;
            state.ModifyStats(attackDelta, defenseDelta);
        }

        public void ResetToBaseStats(InvocationCardState state)
        {
            if (state == null) return;
            state.ResetToBaseStats();
        }

        public void SetStats(InvocationCardState state, float attack, float defense)
        {
            if (state == null) return;
            state.CurrentAttack = attack;
            state.CurrentDefense = defense;
        }

        #endregion

        #region Death/Field Removal

        public void PrepareForDeath(InvocationCardState state)
        {
            if (state == null) return;

            // Reset stats to base values
            state.ResetToBaseStats();

            // Unblock attacks
            state.UnblockAttack();

            // Free from control
            state.Free();

            // Reset attack counter
            state.ResetForNewTurn();

            // Increment death count
            state.IncrementDeathCount();

            // Detach equipment
            state.EquippedCardId = null;

            // Reset field state
            state.ResetFieldState();
        }

        public void PrepareForFieldRemoval(InGameCardState state)
        {
            if (state == null) return;

            // Reset field-specific state
            state.ResetFieldState();

            // For invocation cards, also reset stats
            if (state is InvocationCardState invocationState)
            {
                invocationState.ResetToBaseStats();
                invocationState.UnblockAttack();
                invocationState.Free();
                invocationState.EquippedCardId = null;
            }
        }

        #endregion

        #region Control

        public void TakeControl(InvocationCardState state)
        {
            if (state == null) return;
            state.Control();
        }

        public void ReleaseControl(InvocationCardState state)
        {
            if (state == null) return;
            state.Free();
        }

        #endregion
    }
}

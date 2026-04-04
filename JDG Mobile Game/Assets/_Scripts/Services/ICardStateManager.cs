using System;
using JDG.Domain;

/// <summary>
/// Service interface for managing card state during gameplay.
/// Phase 21-22: Created as part of InGameInvocationCard extraction pattern.
///
/// NOTE: This interface is in the default assembly (Services folder) because it depends
/// on legacy types (Cards namespace, InGameInvocationCard) that haven't been migrated
/// to the Domain layer yet. Once card types are migrated, this can move to JDG.Application.
///
/// LONG-TERM GOAL: Full implementation requires InGameInvocationCard refactoring (267+ lines).
/// This interface defines the contract for potential future extraction:
/// - Attack counting and limits
/// - Blocking state
/// - Control state (who controls the card)
/// - Combat flags (CanDirectAttack, CantBeAttack, Aggro)
/// - Turn-based state (NumberOfTurnOnField, etc.)
/// - Condition tracking
///
/// Deferred due to:
/// 1. Complexity of card system (267+ lines of interrelated state)
/// 2. Deep integration with ability system
/// 3. Requires comprehensive testing to avoid breaking gameplay
/// 4. Current InGameInvocationCard works correctly - no urgent need
///
/// This is a design document for future architecture evolution, not a blocking item.
/// </summary>
public interface ICardStateManager
    {
        // Attack Management
        bool CanAttack(Guid cardId);
        void IncrementAttackCount(Guid cardId);
        void ResetAttackCount(Guid cardId);
        int GetRemainingAttacks(Guid cardId);

        // Blocking State
        void BlockAttack(Guid cardId);
        void UnblockAttack(Guid cardId);
        bool IsAttackBlocked(Guid cardId);

        // Control State
        void TakeControl(Guid cardId, CardOwner newController);
        void ReleaseControl(Guid cardId);
        bool IsControlled(Guid cardId);
        CardOwner? GetController(Guid cardId);

        // Combat Flags
        void SetCanDirectAttack(Guid cardId, bool value);
        void SetCantBeAttacked(Guid cardId, bool value);
        void SetAggro(Guid cardId, bool value);
        bool CanDirectAttack(Guid cardId);
        bool CantBeAttacked(Guid cardId);
        bool HasAggro(Guid cardId);

        // Turn-based State
        void IncrementTurnOnField(Guid cardId);
        int GetTurnsOnField(Guid cardId);
        void ResetForNewTurn(Guid cardId);

        // Equipment
        void AttachEquipment(Guid cardId, object equipmentCard); // Using object until equipment types migrated
        void DetachEquipment(Guid cardId);
        object GetEquipment(Guid cardId);

    // Lifecycle
    void RegisterCard(Guid cardId);
    void UnregisterCard(Guid cardId);
}

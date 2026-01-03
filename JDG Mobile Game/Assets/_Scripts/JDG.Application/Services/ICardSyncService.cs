using JDG.Application.Cards;
using JDG.Domain.Entities;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for synchronizing domain Card state changes back to InGameInvocationCard.
    ///
    /// Phase 141: Fixes the equipment ability sync issue where domain Card modifications
    /// were lost because ConvertToCard() created a disconnected temporary Card.
    ///
    /// Usage pattern:
    /// 1. Call CreateLinkedCard() before ability execution
    /// 2. Pass returned Card to abilities via AbilityContext.TargetCard
    /// 3. After abilities execute, call SyncCardState() to apply changes
    /// 4. Call ClearMapping() to clean up
    /// </summary>
    public interface ICardSyncService
    {
        /// <summary>
        /// Creates a domain Card linked to an InGameInvocationCard for ability execution.
        /// Uses Card.CreateInvocation with current stats (not CreateEffect).
        /// The returned Card maintains a mapping for later synchronization.
        /// </summary>
        /// <param name="inGameCard">The InGameInvocationCard to link.</param>
        /// <returns>A domain Card with current stats that is mapped to the InGameCard.</returns>
        Card CreateLinkedCard(IInGameInvocationCard inGameCard);

        /// <summary>
        /// Syncs all state changes from the domain Card back to its linked InGameInvocationCard.
        /// Should be called after ability execution completes.
        ///
        /// Syncs: Attack, Defense, CancelEffect, CanDirectAttack, CantBeAttacked
        /// </summary>
        /// <param name="domainCard">The domain Card whose state should be synced.</param>
        void SyncCardState(Card domainCard);

        /// <summary>
        /// Clears the mapping for a specific card.
        /// Should be called after SyncCardState to prevent memory leaks.
        /// </summary>
        /// <param name="domainCard">The domain Card to unmap.</param>
        void ClearMapping(Card domainCard);
    }
}

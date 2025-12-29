using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.UseCases.Results;
using JDG.Domain;
using JDG.Domain.Events;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for handling hand card count changes.
    /// Phase 21-22: Originally extracted from PlayerCards.OnHandCardsChange().
    /// Phase 78: Migrated to JDG.Application with interface-based abstraction.
    ///
    /// This use case handles triggering equipment abilities when hand card count changes.
    /// Equipment abilities can react to hand size (e.g., stat bonuses based on hand count).
    /// </summary>
    public class HandleHandCardsChangeUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IAbilityExecutor _abilityExecutor;

        public HandleHandCardsChangeUseCase(IEventBus eventBus, IAbilityExecutor abilityExecutor)
        {
            _eventBus = eventBus;
            _abilityExecutor = abilityExecutor;
        }

        /// <summary>
        /// Handles hand card count changes by triggering equipment abilities.
        /// </summary>
        /// <param name="playerCards">The player's card collection whose hand changed.</param>
        /// <param name="opponentCards">The opponent's card collection (for abilities that affect opponent).</param>
        /// <param name="oldCount">The previous hand card count.</param>
        /// <param name="newCount">The new hand card count.</param>
        /// <returns>Result indicating success and number of abilities triggered.</returns>
        public HandCardsChangeResult Execute(
            IPlayerCardCollection playerCards,
            IPlayerCardCollection opponentCards,
            int oldCount,
            int newCount)
        {
            if (playerCards == null)
                return HandCardsChangeResult.Failure("Player cards collection is null");

            // Delegate to ability executor which handles the equipment ability triggers
            _abilityExecutor.ExecuteOnHandCardsChanged(playerCards, opponentCards, oldCount, newCount);

            // Publish event to notify other systems
            _eventBus.Publish(new HandCardsChangedEvent
            {
                Owner = playerCards.Owner,
                NewHandCount = newCount,
                Delta = newCount - oldCount
            });

            return HandCardsChangeResult.Success(
                playerCards.Owner,
                oldCount,
                newCount,
                abilitiesTriggered: CountEquippedInvocations(playerCards));
        }

        /// <summary>
        /// Legacy overload for backward compatibility with delta-based calls.
        /// Phase 78: Maintained for callers that don't track old/new counts.
        /// </summary>
        public void Execute(IPlayerCardCollection playerCards, IPlayerCardCollection opponentCards, int delta)
        {
            int currentCount = playerCards?.HandCardCount ?? 0;
            int oldCount = currentCount - delta;
            Execute(playerCards, opponentCards, oldCount, currentCount);
        }

        /// <summary>
        /// Counts invocation cards with equipment for result reporting.
        /// </summary>
        private int CountEquippedInvocations(IPlayerCardCollection playerCards)
        {
            int count = 0;
            foreach (var invocation in playerCards.InvocationCards)
            {
                if (invocation.EquipmentCard != null)
                    count++;
            }
            return count;
        }
    }
}

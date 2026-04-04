using System.Collections.Generic;
using System.Linq;
using JDG.Application.Cards;
using JDG.Application.UseCases.Results;
using JDG.Domain;
using JDG.Domain.Events;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for resetting card state at the start of a new turn.
    /// Phase 21-22: Originally extracted from PlayerCards.ResetInvocationCardNewTurn().
    /// Phase 77: Migrated to JDG.Application with interface-based abstraction.
    ///
    /// This use case handles resetting attack counts and other turn-based state
    /// for all invocation cards at the start of each turn.
    /// </summary>
    public class ResetCardsForNewTurnUseCase
    {
        private readonly IEventBus _eventBus;

        public ResetCardsForNewTurnUseCase(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        /// <summary>
        /// Resets all invocation cards for a new turn.
        /// Resets attack counts, unblocks cards, and applies turn-based ability effects.
        /// </summary>
        /// <param name="invocationCards">The collection of invocation cards to reset.</param>
        /// <param name="owner">The owner of the cards being reset.</param>
        /// <returns>Result indicating success and number of cards reset.</returns>
        public CardsResetResult Execute(IEnumerable<IInGameInvocationCard> invocationCards, CardOwner owner)
        {
            if (invocationCards == null)
                return CardsResetResult.Failure("Invocation cards collection is null");

            var cardsList = invocationCards.Where(card => card != null).ToList();
            int resetCount = 0;

            foreach (var invocationCard in cardsList)
            {
                invocationCard.ResetNewTurn();
                resetCount++;
            }

            // Publish event to notify UI and other systems
            _eventBus.Publish(new CardsResetForNewTurnEvent());

            return CardsResetResult.Success(owner, resetCount);
        }

        /// <summary>
        /// Legacy overload for backward compatibility.
        /// Phase 77: Maintained for callers that don't need the result.
        /// </summary>
        public void Execute(IEnumerable<IInGameInvocationCard> invocationCards)
        {
            Execute(invocationCards, CardOwner.NotDefined);
        }
    }
}

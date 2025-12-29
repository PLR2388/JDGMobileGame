using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using JDG.Application;
using JDG.Domain.Events;

/// <summary>
/// Use case for resetting card state at the start of a new turn.
/// Phase 21-22: Extracted from PlayerCards.ResetInvocationCardNewTurn().
/// Phase 77: OBSOLETE - Use JDG.Application.UseCases.ResetCardsForNewTurnUseCase instead.
///
/// This legacy version remains for backward compatibility with code that
/// passes concrete InGameInvocationCard types. New code should use the
/// Application layer version with IInGameInvocationCard interfaces.
/// </summary>
[Obsolete("Use JDG.Application.UseCases.ResetCardsForNewTurnUseCase with IInGameInvocationCard instead. Phase 77.")]
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
    public void Execute(IEnumerable<InGameInvocationCard> invocationCards)
    {
        foreach (var invocationCard in invocationCards.Where(card => card?.Title != null))
        {
            invocationCard.ResetNewTurn();
        }

        // Publish event to notify UI and other systems
        _eventBus.Publish(new CardsResetForNewTurnEvent());
    }
}

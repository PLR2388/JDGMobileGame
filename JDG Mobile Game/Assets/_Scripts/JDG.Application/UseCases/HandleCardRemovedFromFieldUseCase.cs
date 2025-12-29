using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.UseCases.Results;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for handling an invocation card being removed from the field.
    /// Phase 21-22: Originally extracted from PlayerCards.OnInvocationCardsRemoved().
    /// Phase 81: Migrated to JDG.Application with interface-based abstraction.
    ///
    /// This use case handles triggering abilities when a card is removed from field:
    /// - Remaining invocation card abilities (OnCardRemove)
    /// - Effect card abilities (OnInvocationCardRemoved)
    /// </summary>
    public class HandleCardRemovedFromFieldUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IAbilityExecutor _abilityExecutor;

        public HandleCardRemovedFromFieldUseCase(IEventBus eventBus, IAbilityExecutor abilityExecutor)
        {
            _eventBus = eventBus;
            _abilityExecutor = abilityExecutor;
        }

        /// <summary>
        /// Handles an invocation card being removed from the field.
        /// Triggers all relevant abilities for the removed card.
        /// </summary>
        /// <param name="removedCard">The card that was removed from the field.</param>
        /// <param name="ownerCards">The player's card collection.</param>
        /// <param name="opponentCards">The opponent's card collection (optional).</param>
        /// <returns>Result indicating success and abilities triggered.</returns>
        public CardFieldChangeResult Execute(
            IInGameInvocationCard removedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards = null)
        {
            if (removedCard == null)
                return CardFieldChangeResult.Failure("Removed card is null");

            if (ownerCards == null)
                return CardFieldChangeResult.Failure("Owner cards collection is null");

            // Delegate to ability executor which handles:
            // - All remaining invocation cards' OnCardRemove abilities
            // - Effect card abilities' OnInvocationCardRemoved
            _abilityExecutor.ExecuteOnCardRemovedFromField(removedCard, ownerCards, opponentCards);

            // Publish event to notify UI and other systems
            _eventBus.Publish(new CardRemovedFromFieldEvent
            {
                CardId = System.Guid.Empty, // Legacy cards don't have Guid IDs yet
                Owner = ownerCards.Owner
            });

            return CardFieldChangeResult.Success(
                CardId.Empty,
                ownerCards.Owner,
                FieldChangeType.Removed,
                abilitiesTriggered: CountRemainingAbilities(ownerCards));
        }

        /// <summary>
        /// Counts abilities on remaining invocation cards for result reporting.
        /// </summary>
        private int CountRemainingAbilities(IPlayerCardCollection ownerCards)
        {
            int count = 0;
            foreach (var card in ownerCards.InvocationCards)
            {
                foreach (var _ in card.Abilities)
                {
                    count++;
                }
            }
            return count;
        }
    }
}

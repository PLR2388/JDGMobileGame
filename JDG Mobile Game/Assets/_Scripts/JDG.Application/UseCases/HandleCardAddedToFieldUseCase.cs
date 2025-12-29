using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.UseCases.Results;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for handling a new invocation card being added to the field.
    /// Phase 21-22: Originally extracted from PlayerCards.OnInvocationCardAdded().
    /// Phase 80: Migrated to JDG.Application with interface-based abstraction.
    ///
    /// This use case handles triggering abilities when a new card enters the field:
    /// - Opponent's equipment card abilities (OnOpponentInvocationCardAdded)
    /// - Existing invocation card abilities (OnCardAdded)
    /// - Effect card abilities (OnInvocationCardAdded)
    /// - Field card abilities (OnInvocationCardAdded)
    /// </summary>
    public class HandleCardAddedToFieldUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IAbilityExecutor _abilityExecutor;

        public HandleCardAddedToFieldUseCase(IEventBus eventBus, IAbilityExecutor abilityExecutor)
        {
            _eventBus = eventBus;
            _abilityExecutor = abilityExecutor;
        }

        /// <summary>
        /// Handles a new invocation card being added to the field.
        /// Triggers all relevant abilities for both players.
        /// </summary>
        /// <param name="addedCard">The new invocation card that was added.</param>
        /// <param name="ownerCards">The player's card collection.</param>
        /// <param name="opponentCards">The opponent's card collection.</param>
        /// <returns>Result indicating success and abilities triggered.</returns>
        public CardFieldChangeResult Execute(
            IInGameInvocationCard addedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (addedCard == null)
                return CardFieldChangeResult.Failure("Added card is null");

            if (ownerCards == null)
                return CardFieldChangeResult.Failure("Owner cards collection is null");

            // Delegate to ability executor which handles all ability triggers:
            // - Opponent equipment abilities
            // - Existing invocation abilities
            // - Effect card abilities
            // - Field card abilities
            _abilityExecutor.ExecuteOnCardAddedToField(addedCard, ownerCards, opponentCards);

            // Publish event to notify UI and other systems
            _eventBus.Publish(new CardAddedToFieldEvent
            {
                AddedCard = addedCard,
                Owner = ownerCards.Owner
            });

            return CardFieldChangeResult.Success(
                CardId.Empty, // Legacy cards don't have Guid IDs yet
                ownerCards.Owner,
                FieldChangeType.Added,
                abilitiesTriggered: CountAbilities(addedCard));
        }

        /// <summary>
        /// Counts abilities on the added card for result reporting.
        /// </summary>
        private int CountAbilities(IInGameInvocationCard card)
        {
            int count = 0;
            foreach (var _ in card.Abilities)
            {
                count++;
            }
            return count;
        }
    }
}

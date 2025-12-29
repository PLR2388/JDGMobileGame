using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.UseCases.Results;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for handling card death and moving cards to the graveyard.
    /// Phase 21-22: Originally extracted from PlayerCards.OnYellowTrashAdded().
    /// Phase 82: Migrated to JDG.Application with interface-based abstraction.
    ///
    /// This use case handles:
    /// - Resetting card stats to base values
    /// - Unblocking the card
    /// - Freeing the card from control/equipment
    /// - Triggering OnCardDeath abilities
    /// </summary>
    public class HandleCardDeathUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IAbilityExecutor _abilityExecutor;

        public HandleCardDeathUseCase(IEventBus eventBus, IAbilityExecutor abilityExecutor)
        {
            _eventBus = eventBus;
            _abilityExecutor = abilityExecutor;
        }

        /// <summary>
        /// Handles a card's death, resetting its state and triggering death abilities.
        /// </summary>
        /// <param name="deadCard">The invocation card that died.</param>
        /// <param name="ownerCards">The player's card collection.</param>
        /// <param name="opponentCards">The opponent's card collection.</param>
        /// <returns>Result indicating success and abilities triggered.</returns>
        public CardDeathResult Execute(
            IInGameInvocationCard deadCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (deadCard == null)
                return CardDeathResult.Failure("Dead card is null");

            if (ownerCards == null)
                return CardDeathResult.Failure("Owner cards collection is null");

            // Reset card state to base values
            deadCard.UnblockAttack();
            deadCard.Attack = deadCard.BaseAttack;
            deadCard.Defense = deadCard.BaseDefense;
            deadCard.FreeCard();
            deadCard.ResetNewTurn();

            // Delegate to ability executor which handles death abilities
            _abilityExecutor.ExecuteOnCardDeath(deadCard, ownerCards, opponentCards);

            // Publish event to notify UI and other systems
            _eventBus.Publish(new CardDiedEvent
            {
                DeadCard = deadCard,
                Owner = ownerCards.Owner
            });

            return CardDeathResult.Success(
                CardId.Empty, // Legacy cards don't have Guid IDs yet
                ownerCards.Owner,
                abilitiesTriggered: CountAbilities(deadCard));
        }

        /// <summary>
        /// Overload for handling any IInGameCard (checks if it's an invocation).
        /// Maintains backward compatibility with code that handles generic cards.
        /// </summary>
        public CardDeathResult Execute(
            IInGameCard deadCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (deadCard is IInGameInvocationCard invocationCard)
            {
                return Execute(invocationCard, ownerCards, opponentCards);
            }

            // Non-invocation cards don't have death abilities
            return CardDeathResult.Success(CardId.Empty, ownerCards?.Owner ?? CardOwner.NotDefined, 0);
        }

        /// <summary>
        /// Counts abilities on the dead card for result reporting.
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

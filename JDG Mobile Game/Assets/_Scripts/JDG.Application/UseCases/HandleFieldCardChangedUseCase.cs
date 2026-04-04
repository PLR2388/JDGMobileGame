using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.UseCases.Results;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for handling field card changes.
    /// Phase 21-22: Originally extracted from PlayerCards.FieldCard property setter.
    /// Phase 79: Migrated to JDG.Application with interface-based abstraction.
    ///
    /// This use case handles triggering abilities when a field card is changed:
    /// - Triggers OnFieldCardRemoved on the old field card's abilities
    /// - Triggers any field change effects on the new card
    /// </summary>
    public class HandleFieldCardChangedUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IAbilityExecutor _abilityExecutor;

        public HandleFieldCardChangedUseCase(IEventBus eventBus, IAbilityExecutor abilityExecutor)
        {
            _eventBus = eventBus;
            _abilityExecutor = abilityExecutor;
        }

        /// <summary>
        /// Handles a field card being changed (replaced or removed).
        /// </summary>
        /// <param name="oldFieldCard">The field card being removed (null if none).</param>
        /// <param name="newFieldCard">The new field card (null if field is being cleared).</param>
        /// <param name="ownerCards">The player's card collection.</param>
        /// <param name="opponentCards">The opponent's card collection.</param>
        /// <returns>Result indicating success and abilities triggered.</returns>
        public CardFieldChangeResult Execute(
            IInGameFieldCard oldFieldCard,
            IInGameFieldCard newFieldCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (ownerCards == null)
                return CardFieldChangeResult.Failure("Owner cards collection is null");

            // Delegate to ability executor which handles field ability triggers
            _abilityExecutor.ExecuteOnFieldCardChanged(oldFieldCard, newFieldCard, ownerCards, opponentCards);

            // Publish event to notify other systems
            _eventBus.Publish(new FieldCardReplacedEvent
            {
                OldFieldCardId = null, // Legacy cards don't have Guid IDs yet
                NewFieldCardId = null,
                Owner = ownerCards.Owner
            });

            return CardFieldChangeResult.Success(
                CardId.Empty,
                ownerCards.Owner,
                newFieldCard != null ? FieldChangeType.FieldCardChanged : FieldChangeType.Removed,
                abilitiesTriggered: oldFieldCard != null ? CountFieldAbilities(oldFieldCard) : 0);
        }

        /// <summary>
        /// Legacy overload for backward compatibility.
        /// Phase 79: Maintained for callers that only handle removal.
        /// </summary>
        public void HandleFieldCardRemoved(
            IInGameFieldCard oldFieldCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            Execute(oldFieldCard, null, ownerCards, opponentCards);
        }

        /// <summary>
        /// Gets the title of a field card safely.
        /// </summary>
        private string GetCardTitle(IInGameFieldCard card)
        {
            return card?.Title ?? string.Empty;
        }

        /// <summary>
        /// Counts field abilities for result reporting.
        /// </summary>
        private int CountFieldAbilities(IInGameFieldCard fieldCard)
        {
            int count = 0;
            foreach (var _ in fieldCard.FieldAbilities)
            {
                count++;
            }
            return count;
        }
    }
}

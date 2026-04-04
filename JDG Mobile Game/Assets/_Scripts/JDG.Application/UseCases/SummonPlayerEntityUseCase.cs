using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Application.UseCases.Results;
using JDG.Domain;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for summoning/creating a player entity card.
    /// Phase 21-22: Originally extracted from PlayerCards.BuildPlayer().
    /// Phase 62: Uses ICardFactory instead of individual ability providers.
    /// Phase 83: Migrated to JDG.Application with interface-based return type.
    ///
    /// This use case handles the creation of the player entity card (the player avatar)
    /// which is different from regular invocation cards.
    ///
    /// Note: The input parameter (base card definition) remains as object because
    /// InvocationCard is a Unity ScriptableObject that would require significant
    /// architectural changes to abstract. The important abstraction is the output type.
    /// </summary>
    public class SummonPlayerEntityUseCase
    {
        private readonly ICardFactory _cardFactory;

        public SummonPlayerEntityUseCase(ICardFactory cardFactory)
        {
            _cardFactory = cardFactory;
        }

        /// <summary>
        /// Creates a player entity card for the specified player.
        /// </summary>
        /// <param name="playerInvocationCard">The base invocation card for the player entity (ScriptableObject).</param>
        /// <param name="owner">The owner of this player entity.</param>
        /// <returns>Result containing the created player entity card.</returns>
        public SummonPlayerEntityResult Execute(object playerInvocationCard, CardOwner owner)
        {
            if (playerInvocationCard == null)
                return SummonPlayerEntityResult.Failure("Player invocation card is null");

            var entityCard = _cardFactory.CreateInvocationCard(playerInvocationCard, owner);

            if (entityCard == null)
                return SummonPlayerEntityResult.Failure("Failed to create player entity card");

            return SummonPlayerEntityResult.Success(
                owner,
                CardId.Empty, // Legacy cards don't have Guid IDs yet
                entityCard);
        }

        /// <summary>
        /// Legacy overload for backward compatibility with bool-based player identification.
        /// </summary>
        public SummonPlayerEntityResult Execute(object playerInvocationCard, bool isPlayerOne)
        {
            var owner = isPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
            return Execute(playerInvocationCard, owner);
        }

        /// <summary>
        /// Legacy overload that returns the interface type directly.
        /// Maintained for callers that don't need the result pattern.
        /// </summary>
        public IInGameInvocationCard CreatePlayerEntity(object playerInvocationCard, CardOwner owner)
        {
            var result = Execute(playerInvocationCard, owner);
            return result.IsSuccess ? result.EntityCard : null;
        }
    }
}

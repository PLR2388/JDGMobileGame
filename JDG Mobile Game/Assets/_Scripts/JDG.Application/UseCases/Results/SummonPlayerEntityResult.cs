using JDG.Application.Cards;
using JDG.Domain;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases.Results
{
    /// <summary>
    /// Result of summoning a player entity card.
    /// Phase 76: Created for UseCase migration.
    /// </summary>
    public class SummonPlayerEntityResult
    {
        public bool IsSuccess { get; private set; }
        public CardOwner Owner { get; private set; }
        public CardId? EntityCardId { get; private set; }
        public IInGameInvocationCard EntityCard { get; private set; }
        public string Message { get; private set; }

        public static SummonPlayerEntityResult Success(
            CardOwner owner,
            CardId entityCardId,
            IInGameInvocationCard entityCard) => new SummonPlayerEntityResult
        {
            IsSuccess = true,
            Owner = owner,
            EntityCardId = entityCardId,
            EntityCard = entityCard,
            Message = "Player entity summoned successfully"
        };

        public static SummonPlayerEntityResult Failure(string message) => new SummonPlayerEntityResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

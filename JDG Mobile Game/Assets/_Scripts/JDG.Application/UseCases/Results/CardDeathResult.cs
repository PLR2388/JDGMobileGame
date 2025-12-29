using JDG.Domain;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases.Results
{
    /// <summary>
    /// Result of a card death operation.
    /// Phase 76: Created for UseCase migration.
    /// </summary>
    public class CardDeathResult
    {
        public bool IsSuccess { get; private set; }
        public CardId CardId { get; private set; }
        public CardOwner Owner { get; private set; }
        public int AbilitiesTriggered { get; private set; }
        public string Message { get; private set; }

        public static CardDeathResult Success(CardId cardId, CardOwner owner, int abilitiesTriggered = 0) => new CardDeathResult
        {
            IsSuccess = true,
            CardId = cardId,
            Owner = owner,
            AbilitiesTriggered = abilitiesTriggered,
            Message = "Card death handled successfully"
        };

        public static CardDeathResult Failure(string message) => new CardDeathResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

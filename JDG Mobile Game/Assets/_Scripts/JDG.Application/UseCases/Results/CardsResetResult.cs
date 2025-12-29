using JDG.Domain;

namespace JDG.Application.UseCases.Results
{
    /// <summary>
    /// Result of resetting cards for a new turn.
    /// Phase 76: Created for UseCase migration.
    /// </summary>
    public class CardsResetResult
    {
        public bool IsSuccess { get; private set; }
        public CardOwner Owner { get; private set; }
        public int CardsReset { get; private set; }
        public string Message { get; private set; }

        public static CardsResetResult Success(CardOwner owner, int cardsReset) => new CardsResetResult
        {
            IsSuccess = true,
            Owner = owner,
            CardsReset = cardsReset,
            Message = $"Reset {cardsReset} cards for new turn"
        };

        public static CardsResetResult Failure(string message) => new CardsResetResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

using JDG.Domain;

namespace JDG.Application.UseCases.Results
{
    /// <summary>
    /// Result of handling hand cards change.
    /// Phase 76: Created for UseCase migration.
    /// </summary>
    public class HandCardsChangeResult
    {
        public bool IsSuccess { get; private set; }
        public CardOwner Owner { get; private set; }
        public int OldCount { get; private set; }
        public int NewCount { get; private set; }
        public int AbilitiesTriggered { get; private set; }
        public string Message { get; private set; }

        public static HandCardsChangeResult Success(
            CardOwner owner,
            int oldCount,
            int newCount,
            int abilitiesTriggered = 0) => new HandCardsChangeResult
        {
            IsSuccess = true,
            Owner = owner,
            OldCount = oldCount,
            NewCount = newCount,
            AbilitiesTriggered = abilitiesTriggered,
            Message = $"Hand cards changed from {oldCount} to {newCount}"
        };

        public static HandCardsChangeResult Failure(string message) => new HandCardsChangeResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

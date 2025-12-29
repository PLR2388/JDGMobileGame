using JDG.Domain;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases.Results
{
    /// <summary>
    /// Result of a card field change operation (added/removed).
    /// Phase 76: Created for UseCase migration.
    /// </summary>
    public class CardFieldChangeResult
    {
        public bool IsSuccess { get; private set; }
        public CardId CardId { get; private set; }
        public CardOwner Owner { get; private set; }
        public FieldChangeType ChangeType { get; private set; }
        public int AbilitiesTriggered { get; private set; }
        public string Message { get; private set; }

        public static CardFieldChangeResult Success(
            CardId cardId,
            CardOwner owner,
            FieldChangeType changeType,
            int abilitiesTriggered = 0) => new CardFieldChangeResult
        {
            IsSuccess = true,
            CardId = cardId,
            Owner = owner,
            ChangeType = changeType,
            AbilitiesTriggered = abilitiesTriggered,
            Message = $"Card {changeType.ToString().ToLower()} successfully"
        };

        public static CardFieldChangeResult Failure(string message) => new CardFieldChangeResult
        {
            IsSuccess = false,
            Message = message
        };
    }

    /// <summary>
    /// Type of field change operation.
    /// </summary>
    public enum FieldChangeType
    {
        Added,
        Removed,
        FieldCardChanged
    }
}

using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Context passed to abilities containing all necessary game state information.
    /// </summary>
    public class AbilityContext
    {
        public PlayerId CurrentPlayerId { get; }
        public PlayerId OpponentPlayerId { get; }
        public Card SourceCard { get; }
        public AbilityName AbilityName { get; }

        // Optional target information
        public Card TargetCard { get; set; }
        public PlayerId TargetPlayerId { get; set; }

        public AbilityContext(
            PlayerId currentPlayerId,
            PlayerId opponentPlayerId,
            Card sourceCard,
            AbilityName abilityName)
        {
            CurrentPlayerId = currentPlayerId;
            OpponentPlayerId = opponentPlayerId;
            SourceCard = sourceCard;
            AbilityName = abilityName;
        }
    }

    /// <summary>
    /// Result returned by ability execution.
    /// </summary>
    public class AbilityResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public bool RequiresUserInput { get; }

        private AbilityResult(bool isSuccess, string message, bool requiresUserInput = false)
        {
            IsSuccess = isSuccess;
            Message = message;
            RequiresUserInput = requiresUserInput;
        }

        public static AbilityResult Success(string message = "")
            => new AbilityResult(true, message);

        public static AbilityResult Failure(string message)
            => new AbilityResult(false, message);

        public static AbilityResult NeedsUserInput(string message)
            => new AbilityResult(true, message, requiresUserInput: true);
    }

    /// <summary>
    /// Base interface for all card abilities.
    /// Abilities are pure logic - they execute game actions through use cases.
    /// </summary>
    public interface IAbility
    {
        /// <summary>
        /// The unique name of this ability.
        /// </summary>
        AbilityName Name { get; }

        /// <summary>
        /// User-friendly description of what the ability does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Checks if the ability can be activated given the current context.
        /// </summary>
        bool CanActivate(AbilityContext context);

        /// <summary>
        /// Executes the ability's effect.
        /// Pure logic - no Unity dependencies. Uses use cases for all game actions.
        /// </summary>
        AbilityResult Execute(AbilityContext context);
    }

    /// <summary>
    /// Base interface for passive abilities that trigger automatically.
    /// </summary>
    public interface IPassiveAbility : IAbility
    {
        /// <summary>
        /// Determines when this passive ability triggers.
        /// </summary>
        AbilityTrigger Trigger { get; }
    }

    /// <summary>
    /// Defines when a passive ability triggers.
    /// </summary>
    public enum AbilityTrigger
    {
        OnSummon,           // When card is summoned
        OnDeath,            // When card dies
        OnAttack,           // When card attacks
        OnDefend,           // When card is attacked
        OnTurnStart,        // At start of turn
        OnTurnEnd,          // At end of turn
        OnCardDrawn,        // When a card is drawn
        OnCardPlayed,       // When a card is played
        Continuous          // Always active while card is on field
    }
}

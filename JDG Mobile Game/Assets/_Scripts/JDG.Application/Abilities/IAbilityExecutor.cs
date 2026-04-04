using JDG.Application.Cards;
using JDG.Domain.Enums;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Interface for executing ability triggers without coupling to legacy Ability classes.
    /// Phase 74: Created as part of UseCase migration.
    ///
    /// Use cases depend on this interface to trigger abilities without knowing
    /// about the concrete Ability implementation.
    ///
    /// The implementation (AbilityExecutorAdapter) bridges to the legacy Ability system,
    /// allowing gradual migration to pure domain ability implementations.
    /// </summary>
    public interface IAbilityExecutor
    {
        #region Death Triggers

        /// <summary>
        /// Executes OnCardDeath abilities for an invocation card.
        /// Called when a card is moved to the graveyard.
        /// </summary>
        void ExecuteOnCardDeath(
            IInGameInvocationCard deadCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards);

        #endregion

        #region Field Entry/Exit Triggers

        /// <summary>
        /// Executes OnCardAdded abilities when a card is added to the field.
        /// Triggers abilities on:
        /// - The added card itself
        /// - Existing field cards that react to new cards
        /// </summary>
        void ExecuteOnCardAddedToField(
            IInGameInvocationCard addedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards);

        /// <summary>
        /// Executes OnCardRemoved abilities when a card is removed from the field.
        /// </summary>
        void ExecuteOnCardRemovedFromField(
            IInGameInvocationCard removedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards);

        /// <summary>
        /// Executes abilities when the field card is changed.
        /// </summary>
        void ExecuteOnFieldCardChanged(
            IInGameFieldCard oldFieldCard,
            IInGameFieldCard newFieldCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards);

        #endregion

        #region Turn Triggers

        /// <summary>
        /// Executes OnTurnStart abilities for all applicable cards.
        /// Called at the beginning of a player's turn.
        /// </summary>
        void ExecuteOnTurnStart(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards);

        /// <summary>
        /// Executes OnTurnEnd abilities for all applicable cards.
        /// Called at the end of a player's turn.
        /// </summary>
        void ExecuteOnTurnEnd(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards);

        #endregion

        #region Hand Change Triggers

        /// <summary>
        /// Executes abilities that trigger when hand card count changes.
        /// Some equipment abilities react to hand size changes.
        /// </summary>
        void ExecuteOnHandCardsChanged(
            IPlayerCardCollection playerCards,
            IPlayerCardCollection opponentCards,
            int oldCount,
            int newCount);

        #endregion

        #region Equipment Triggers

        /// <summary>
        /// Executes abilities when equipment is attached to an invocation card.
        /// </summary>
        void ExecuteOnEquipmentAttached(
            IInGameEquipmentCard equipment,
            IInGameInvocationCard target,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards);

        /// <summary>
        /// Executes abilities when equipment is detached from an invocation card.
        /// </summary>
        void ExecuteOnEquipmentDetached(
            IInGameEquipmentCard equipment,
            IInGameInvocationCard previousTarget,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards);

        #endregion

        #region Effect Card Triggers

        /// <summary>
        /// Executes abilities when an effect card is played to the field.
        /// Phase 114: Added for effect card ability migration.
        /// </summary>
        void ExecuteOnEffectCardPlayed(
            IInGameEffectCard effectCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards);

        #endregion
    }
}

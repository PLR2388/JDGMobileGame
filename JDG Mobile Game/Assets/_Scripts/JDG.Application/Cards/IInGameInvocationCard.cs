using System.Collections.Generic;
using JDG.Domain.Enums;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing an invocation card in the game.
    /// Phase 41: Created to abstract InGameInvocationCard dependencies for use cases.
    /// Phase 72: Extended with additional members for full UseCase abstraction.
    /// Extends IInGameCard with invocation-specific properties and methods.
    /// </summary>
    public interface IInGameInvocationCard : IInGameCard
    {
        #region Stats

        /// <summary>
        /// Gets or sets the current attack value.
        /// </summary>
        float Attack { get; set; }

        /// <summary>
        /// Gets or sets the current defense value.
        /// </summary>
        float Defense { get; set; }

        /// <summary>
        /// Gets the base attack value (from card definition).
        /// </summary>
        float BaseAttack { get; }

        /// <summary>
        /// Gets the base defense value (from card definition).
        /// </summary>
        float BaseDefense { get; }

        /// <summary>
        /// Gets or sets the current families (may be changed by field abilities).
        /// </summary>
        CardFamily[] Families { get; set; }

        #endregion

        #region Combat State

        /// <summary>
        /// Gets or sets whether this card can attack the player directly.
        /// </summary>
        bool CanDirectAttack { get; set; }

        /// <summary>
        /// Gets or sets whether this card cannot be attacked.
        /// </summary>
        bool CantBeAttack { get; set; }

        /// <summary>
        /// Gets or sets whether this card has aggro (must be attacked first).
        /// </summary>
        bool Aggro { get; set; }

        /// <summary>
        /// Checks if the card can attack this turn.
        /// </summary>
        bool CanAttack();

        /// <summary>
        /// Blocks the card's attack for the next turn.
        /// </summary>
        void BlockAttack();

        /// <summary>
        /// Records that the card performed an attack this turn.
        /// </summary>
        void AttackTurnDone();

        /// <summary>
        /// Sets the number of remaining attacks for this turn.
        /// </summary>
        void SetRemainedAttackThisTurn(int number);

        #endregion

        #region Ability State

        /// <summary>
        /// Gets the abilities associated with this card.
        /// Uses object to avoid coupling to legacy Ability type.
        /// </summary>
        IReadOnlyList<object> Abilities { get; }

        /// <summary>
        /// Gets or sets whether the card's effect is canceled.
        /// </summary>
        bool CancelEffect { get; set; }

        /// <summary>
        /// Gets or sets whether this card is affected by effect cards.
        /// </summary>
        bool IsAffectedByEffectCard { get; set; }

        /// <summary>
        /// Checks if the card has an action ability available.
        /// </summary>
        bool HasAction();

        #endregion

        #region Equipment

        /// <summary>
        /// Gets the equipment card attached to this invocation, if any.
        /// Returns null if no equipment is attached.
        /// </summary>
        IInGameEquipmentCard EquipmentCard { get; }

        /// <summary>
        /// Sets the equipment card attached to this invocation.
        /// </summary>
        void SetEquipmentCard(IInGameEquipmentCard card);

        #endregion

        #region Control

        /// <summary>
        /// Gets whether this card is currently controlled by the opponent.
        /// </summary>
        bool IsControlled { get; }

        /// <summary>
        /// Takes control of this card (marks as controlled).
        /// </summary>
        void ControlCard();

        /// <summary>
        /// Frees the card from any control or equipment effects.
        /// </summary>
        void FreeCard();

        #endregion

        #region Turn/Field Tracking

        /// <summary>
        /// Gets the number of turns this card has been on the field.
        /// </summary>
        int NumberOfTurnOnField { get; }

        /// <summary>
        /// Increments the turn counter for this card on the field.
        /// </summary>
        void IncrementNumberTurnOnField();

        /// <summary>
        /// Gets the number of times this card has died.
        /// </summary>
        int NumberOfDeaths { get; }

        /// <summary>
        /// Increments the death counter for this card.
        /// </summary>
        void IncrementNumberDeaths();

        /// <summary>
        /// Gets or sets the number of times this card has been revived.
        /// Used by resurrection abilities with limited revive counts.
        /// Phase 142: Added for domain Card sync.
        /// </summary>
        int TimesRevived { get; set; }

        /// <summary>
        /// Gets or sets the bonus attacks granted for this turn.
        /// Added on top of base attack count when syncing from domain.
        /// Phase 142: Added for domain Card sync.
        /// </summary>
        int BonusAttacks { get; set; }

        /// <summary>
        /// Resets the card state for a new turn.
        /// Resets attack counts and turn-based state.
        /// </summary>
        void ResetNewTurn();

        /// <summary>
        /// Unblocks the card's ability to attack.
        /// </summary>
        void UnblockAttack();

        #endregion
    }
}

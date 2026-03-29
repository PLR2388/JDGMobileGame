using System.Collections.Generic;
using JDG.Application.Cards;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for managing combat operations (attacks, targeting, validation).
    /// Replaces CardManager's combat responsibilities.
    /// Part of Phase 4 - CardManager decomposition.
    /// Phase 40: Now extends ICombatQueryService from JDG.Application.
    /// Phase 166: Refactored to use IInGameCard/IInGameInvocationCard interfaces
    /// instead of concrete types. Moved to JDG.Application.Services.
    /// </summary>
    public interface ICombatService : ICombatQueryService
    {
        /// <summary>
        /// Gets or sets the current attacker card.
        /// </summary>
        IInGameInvocationCard Attacker { get; set; }

        /// <summary>
        /// Gets or sets the current opponent/target card.
        /// </summary>
        IInGameInvocationCard Opponent { get; set; }

        /// <summary>
        /// Computes the damage from the current attacker attacking the current opponent.
        /// </summary>
        /// <returns>Damage value (negative means opponent survives with remaining defense).</returns>
        float ComputeDamageAttack();

        /// <summary>
        /// Executes the attack from the current attacker to the current opponent.
        /// </summary>
        void HandleAttack();

        /// <summary>
        /// Builds a list of valid target cards for the current attacker.
        /// </summary>
        IReadOnlyList<IInGameCard> BuildValidTargets();

        /// <summary>
        /// Executes the special action for the current attacker.
        /// </summary>
        void UseSpecialAction();

        // Note: CanAttackerAttack(), HasAttackerAction(), and IsSpecialActionPossible()
        // are inherited from ICombatQueryService
    }
}

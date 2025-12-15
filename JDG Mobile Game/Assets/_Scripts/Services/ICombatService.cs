using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Services;

/// <summary>
/// Service for managing combat operations (attacks, targeting, validation).
/// Replaces CardManager's combat responsibilities.
/// Part of Phase 4 - CardManager decomposition.
/// Phase 40: Now extends ICombatQueryService from JDG.Application.
///
/// Note: This interface is in the default assembly because it references legacy types
/// (InGameCard, InGameInvocationCard). It will be moved to JDG.Application once
/// these types are refactored.
/// </summary>
public interface ICombatService : ICombatQueryService
{
    /// <summary>
    /// Gets or sets the current attacker card.
    /// </summary>
    InGameInvocationCard Attacker { get; set; }

    /// <summary>
    /// Gets or sets the current opponent/target card.
    /// </summary>
    InGameInvocationCard Opponent { get; set; }

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
    List<InGameCard> BuildValidTargets();

    /// <summary>
    /// Executes the special action for the current attacker.
    /// </summary>
    void UseSpecialAction();

    // Note: CanAttackerAttack(), HasAttackerAction(), and IsSpecialActionPossible()
    // are inherited from ICombatQueryService
}

using System;
using JDG.Application.Cards;
using JDG.Domain.Enums;

namespace Cards
{

/// <summary>
/// Represents an abstract condition that can be evaluated against player cards.
/// </summary>
/// <remarks>
/// DEPRECATED: Use ICondition from JDG.Application.Abilities instead.
/// This class is kept for backward compatibility - concrete implementations still inherit from it.
/// Phase 165: Changed parameter from PlayerCards to IPlayerCardCollection
/// to decouple from MonoBehaviour and enable migration to JDG.Cards assembly.
/// Phase 166: Now uses JDG.Domain.Enums.ConditionName (legacy enum removed).
/// </remarks>
[Obsolete("Use ICondition from JDG.Application.Abilities for new code. Kept for backward compatibility.")]
public abstract class Condition
{
    /// <summary>
    /// Gets or sets the name of the condition.
    /// </summary>
    public ConditionName Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the condition.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Determines whether the condition can be met with the given player cards.
    /// </summary>
    /// <param name="playerCards">The player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the condition can be met; otherwise, <c>false</c>.</returns>
    public abstract bool CanBeSummoned(IPlayerCardCollection playerCards);
}

} // namespace Cards

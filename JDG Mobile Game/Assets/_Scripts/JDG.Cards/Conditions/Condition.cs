using System;
using JDG.Application.Cards;

namespace Cards
{

/// <summary>
/// Enumerates the names of various conditions in the game.
/// </summary>
/// <remarks>
/// DEPRECATED: Use JDG.Domain.Enums.ConditionName instead for clean architecture compatibility.
/// This enum is kept for Unity ScriptableObject serialization - existing card assets reference these values.
/// </remarks>
[Obsolete("Use JDG.Domain.Enums.ConditionName for new code. This enum is kept for Unity serialization compatibility.")]
public enum ConditionName
{
    BenzaieJeuneOrBenzaieOnField,
    ArchibalVonGrenierOnField,
    ZozanKebabOnField,
    BenzaieJeuneCassetteVhsEquiped,
    JoueurDuGrenierCanarangEquiped,
    ThreeAtk3Def,
    JoueurDuGrenierOnFieldCondition,
    ForetDesElfesSylvainsOnField,
    WizardOnField,
    LyceeMagiqueGeorgesPompidouOnField,
    Developer3Atk3Def2Cards,
    HardCorner3Atk3Def2Cards,
    Japan2Cards,
    TenDeathYellowTrash,
    ComicsOnField,
    Incarnation2Cards,
    GranolaxAlreadyDead,
    HumanOnField,
    SebDuGrenierMerdePlastiqueBleuEquiped,
    SebDuGrenierOnField,
    ClicheRacisteMerdeRoseEquiped,
    MechaGronolaxOrGranolaxOnField,
    JapanOnField
}

/// <summary>
/// Represents an abstract condition that can be evaluated against player cards.
/// </summary>
/// <remarks>
/// DEPRECATED: Use ICondition from JDG.Application.Abilities instead.
/// This class is kept for backward compatibility - concrete implementations still inherit from it.
/// Phase 165: Changed parameter from PlayerCards to IPlayerCardCollection
/// to decouple from MonoBehaviour and enable migration to JDG.Cards assembly.
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

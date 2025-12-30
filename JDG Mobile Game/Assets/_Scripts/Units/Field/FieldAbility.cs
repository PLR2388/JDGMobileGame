using System;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Enumeration representing different types of field abilities.
/// </summary>
/// <remarks>
/// DEPRECATED: Use JDG.Domain.Enums.FieldAbilityName instead for clean architecture compatibility.
/// This enum is kept for Unity ScriptableObject serialization - existing card assets reference these values.
/// </remarks>
[Obsolete("Use JDG.Domain.Enums.FieldAbilityName for new code. This enum is kept for Unity serialization compatibility.")]
public enum FieldAbilityName
{
    Earn1DEFForSpatialFamily,
    Earn1HalfDEFAndMinusHalfATKForDevFamily,
    ChangePatronInfogramFamilyToDev,
    ChangeJMBruitagesFamilyToDev,
    Earn2DEFAndMinusOneATKForIncarnationFamily,
    EarnHalfHPPerWizardInvocationEachTurn,
    Earn1ATKForJapanFamily,
    Earn1HalfATKAndMinusHalfDEFForHCFamily,
    DrawOneMoreCard,
    EarnHalfATKAndDefForRpgFamily,
    SkipDrawToGetFistilandInvocation,
    Earn2ATKAndMinus1DEFForComicsFamily
}

/// <summary>
/// Abstract class representing a field ability.
/// </summary>
/// <remarks>
/// DEPRECATED: Use IAbility from JDG.Application.Abilities instead.
/// This class is kept for backward compatibility - concrete implementations still inherit from it.
/// Phase 66: Added instance properties with constructor injection for testability.
/// </remarks>
[Obsolete("Use IAbility from JDG.Application.Abilities for new code. Kept for backward compatibility.")]
public abstract class FieldAbility
{
    /// <summary>
    /// Static localization service for legacy abilities.
    /// Phase 38: Provides DI-compatible localization without changing ability constructors.
    /// Phase 66: Marked obsolete - use instance property Localization instead.
    /// </summary>
    [Obsolete("Use instance property Localization instead. Will be removed in future phase.")]
    public static JDG.Application.Services.ILocalizationService LocalizationService { get; set; }

    /// <summary>
    /// Static dialog service for legacy abilities.
    /// Phase 38: Provides DI-compatible dialogs without changing ability constructors.
    /// Phase 66: Marked obsolete - use instance property Dialog instead.
    /// </summary>
    [Obsolete("Use instance property Dialog instead. Will be removed in future phase.")]
    public static JDG.Application.Services.IDialogService DialogService { get; set; }

    // Phase 66: Instance properties for dependency injection
    #pragma warning disable CS0618 // Using obsolete static properties for backward compatibility
    /// <summary>
    /// Instance-level localization service. Falls back to static property for backward compatibility.
    /// </summary>
    protected JDG.Application.Services.ILocalizationService Localization { get; }

    /// <summary>
    /// Instance-level dialog service. Falls back to static property for backward compatibility.
    /// </summary>
    protected JDG.Application.Services.IDialogService Dialog { get; }

    /// <summary>
    /// Constructor with optional dependency injection.
    /// Phase 66: Enables testability while maintaining backward compatibility.
    /// </summary>
    protected FieldAbility(
        JDG.Application.Services.ILocalizationService localization = null,
        JDG.Application.Services.IDialogService dialog = null)
    {
        Localization = localization ?? LocalizationService;
        Dialog = dialog ?? DialogService;
    }

    /// <summary>
    /// Parameterless constructor for backward compatibility.
    /// </summary>
    protected FieldAbility() : this(null, null) { }
    #pragma warning restore CS0618

    /// <summary>
    /// Gets or sets the name of the field ability.
    /// </summary>
    public FieldAbilityName Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the field ability.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Applies the field ability's effect to the given player's cards.
    /// </summary>
    /// <param name="playerCards">The player's cards to apply the effect on.</param>
    public virtual void ApplyEffect(PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Method called when an invocation card is added.
    /// </summary>
    /// <param name="invocationCard">The invocation card that was added.</param>
    /// <param name="playerCards">The player's cards.</param>
    public virtual void OnInvocationCardAdded(InGameInvocationCard invocationCard,  PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Method called when a field card is removed.
    /// </summary>
    /// <param name="playerCards">The player's cards affected by the removal.</param>
    public virtual void OnFieldCardRemoved(PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Method called when an invocation card changes its family.
    /// </summary>
    /// <param name="previousFamilies">The previous families of the invocation card.</param>
    /// <param name="invocationCard">The invocation card that changed its family.</param>
    public virtual void OnInvocationChangeFamily(CardFamily[]  previousFamilies, InGameInvocationCard invocationCard)
    {
        
    }

    /// <summary>
    /// Method called at the start of a turn.
    /// </summary>
    /// <param name="canvas">The game canvas.</param>
    /// <param name="playerCards">The player's cards for the current turn.</param>
    /// <param name="playerStatus">The current player's status.</param>
    public virtual void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        
    }

}
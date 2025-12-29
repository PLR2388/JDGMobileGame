using System;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Enumeration of names for various effect abilities in the game.
/// </summary>
/// <remarks>
/// DEPRECATED: Use JDG.Domain.Enums.EffectAbilityName instead for clean architecture compatibility.
/// This enum will be removed in a future version.
/// </remarks>
[Obsolete("Use JDG.Domain.Enums.EffectAbilityName instead. This enum will be removed in Phase 54.")]
public enum EffectAbilityName
{
    LimitHandCardTo5,
    Lose2Point5StarsByInvocations,
    ApplyFamilyFieldToInvocations,
    DestroyAllCardsUnderManyConditions,
    GetHPFor1Sacrifice3ATKDEFCondition,
    DirectAttackIfUnder5HP,
    ChangeFieldCardFromDeck,
    DestroyOneCardByRemovingOneHandCard,
    DestroyFieldFor7HalfCost,
    Get7HalfHPFor1Sacrifice,
    GetCardFromYellowDeck,
    ManiabilitePourrieSkipAttackForOpponent,
    SwitchAtkDef,
    LookAndOrderDeckCards,
    LooseHPBasedOnNumberInvocation,
    DestroyEquipmentCard,
    LookOpponentHandCardsAndChangeIt,
    DoubleAttackPerTurn,
    InvokeCardFromYellowTrash,
    DivideDEFOpponentBy2,
    Add3ShieldsForUser,
    DestroyOpponentInvocationCard,
    Loose1HPPerOpponentHandCards,
    GetBackAllHPBySacrifice5AtkDef,
    Control1OpponentInvocationCard
}

/// <summary>
/// Base class for effect abilities that can be applied in the game.
/// EffectAbilities can influence gameplay by modifying card behaviors, player statuses, etc.
/// Phase 38: Added static service properties for legacy abilities (eliminates singleton calls).
/// Phase 66: Added instance properties with constructor injection for testability.
/// </summary>
/// <remarks>
/// DEPRECATED: Use IAbility from JDG.Application.Abilities instead.
/// Legacy abilities are being replaced with clean architecture implementations.
/// </remarks>
[Obsolete("Use IAbility from JDG.Application.Abilities instead. This class will be removed in Phase 54.")]
public abstract class EffectAbility
{
    /// <summary>
    /// Shared ILocalizationService instance for legacy effect abilities.
    /// Phase 38: Set once at initialization to remove LocalizationSystem.Instance calls.
    /// Phase 66: Marked obsolete - use instance property Localization instead.
    /// </summary>
    [Obsolete("Use instance property Localization instead. Will be removed in future phase.")]
    public static JDG.Application.Services.ILocalizationService LocalizationService { get; set; }

    /// <summary>
    /// Shared IDialogService instance for legacy effect abilities.
    /// Phase 38: Set once at initialization to remove MessageBox.Instance and CardSelector.Instance calls.
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
    protected EffectAbility(
        JDG.Application.Services.ILocalizationService localization = null,
        JDG.Application.Services.IDialogService dialog = null)
    {
        Localization = localization ?? LocalizationService;
        Dialog = dialog ?? DialogService;
    }

    /// <summary>
    /// Parameterless constructor for backward compatibility.
    /// </summary>
    protected EffectAbility() : this(null, null) { }
    #pragma warning restore CS0618

    /// <summary>
    /// Gets or sets the name of the effect ability.
    /// </summary>
    public EffectAbilityName Name { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the effect ability.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Indicates how many turns the effect lasts.
    /// </summary>
    protected int NumberOfTurn = 1;

    /// <summary>
    /// Counts the number of turns since the effect was applied.
    /// </summary>
    protected int Counter;

    /// <summary>
    /// Determines if the effect can be applied given the current game state.
    /// </summary>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <returns>True if the effect can be applied, otherwise false.</returns>
    public virtual bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return true;
    }

    /// <summary>
    /// Applies the effect ability.
    /// </summary>
    /// <param name="canvas">Reference to the game canvas.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    /// <param name="playerStatus">The status of the current player.</param>
    /// <param name="opponentStatus">The status of the opponent player.</param>
    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentStatus)
    {
        
    }

    /// <summary>
    /// Logic to execute at the start of a turn.
    /// </summary>
    /// <param name="canvas">Reference to the game canvas.</param>
    /// <param name="playerStatus">The status of the current player.</param>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent player.</param>
    public virtual void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        Counter++;
        if (Counter >= NumberOfTurn && NumberOfTurn > 0)
        {
            var effectCard = playerCards.EffectCards.First(elt => elt.EffectAbilities.Any(ability => ability.Name == Name));
            playerCards.EffectCards.Remove(effectCard);
            playerCards.YellowCards.Add(effectCard);
        }
    }

    /// <summary>
    /// Logic to execute when an invocation card is added.
    /// </summary>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="invocationCard">The invocation card being added.</param>
    public virtual void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }

    /// <summary>
    /// Logic to execute when an invocation card is removed.
    /// </summary>
    /// <param name="playerCards">The cards of the current player.</param>
    /// <param name="invocationCard">The invocation card being removed.</param>
    public virtual void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        
    }
}
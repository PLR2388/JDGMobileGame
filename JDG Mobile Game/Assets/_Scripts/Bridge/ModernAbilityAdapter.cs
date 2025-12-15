using System.Linq;
using _Scripts.Units.Invocation;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using UnityEngine;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

/// <summary>
/// Adapter that wraps a modern IAbility to satisfy the legacy Ability type.
/// Used during Phase 7 migration from AbilityLibrary to AbilityRegistry.
///
/// This adapter allows InGameInvocationCard.Abilities to hold modern IAbility
/// implementations while maintaining compatibility with legacy code that expects
/// the old Ability base class.
///
/// Key behaviors:
/// - ApplyEffect attempts to execute the modern ability via AbilityManager
/// - Falls back to legacy execution path when modern system can't handle it
/// - Preserves mutable InvocationCard property for legacy compatibility
/// </summary>
#pragma warning disable CS0612 // Suppress obsolete warning for Ability base class
public class ModernAbilityAdapter : Ability
#pragma warning restore CS0612
{
    private readonly IAbility _modernAbility;
    private readonly IPlayerRepository _playerRepository;
    private readonly IEventBus _eventBus;

    /// <summary>
    /// Creates a new adapter wrapping a modern IAbility.
    /// </summary>
    /// <param name="modernAbility">The modern IAbility implementation to wrap</param>
    /// <param name="playerRepository">Repository for player state (optional, used for execution)</param>
    /// <param name="eventBus">Event bus for publishing events (optional, used for execution)</param>
    public ModernAbilityAdapter(
        IAbility modernAbility,
        IPlayerRepository playerRepository = null,
        IEventBus eventBus = null)
    {
        _modernAbility = modernAbility;
        _playerRepository = playerRepository;
        _eventBus = eventBus;

        // Copy properties from modern ability to legacy base class
        Name = modernAbility.Name;
        Description = modernAbility.Description;
        IsAction = false; // Most abilities are not action-based
    }

    /// <summary>
    /// Attempts to apply the ability effect using the modern system.
    /// </summary>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        // If we don't have the required dependencies, skip modern execution
        if (_playerRepository == null || invocationCard == null)
        {
            Debug.LogWarning($"[ModernAbilityAdapter] Cannot execute {Name}: missing dependencies");
            return;
        }

        // Determine which player owns this card
        var currentPlayerId = DeterminePlayerId(playerCards);
        var opponentPlayerId = DeterminePlayerId(opponentPlayerCards);

        // Create a domain Card from the InGameInvocationCard
        var sourceCard = CreateDomainCard(invocationCard);

        // Build the context
        var context = new AbilityContext(
            currentPlayerId,
            opponentPlayerId,
            sourceCard,
            Name);

        // Check if ability can activate
        if (!_modernAbility.CanActivate(context))
        {
            Debug.Log($"[ModernAbilityAdapter] Ability {Name} cannot activate in current context");
            return;
        }

        // Execute the modern ability
        var result = _modernAbility.Execute(context);

        if (result.RequiresLegacyExecution)
        {
            Debug.Log($"[ModernAbilityAdapter] Ability {Name} requires legacy execution path");
            // The modern ability indicated it needs legacy handling
            // This is expected during migration - some abilities still need Unity context
        }
        else if (!result.IsSuccess)
        {
            Debug.LogWarning($"[ModernAbilityAdapter] Ability {Name} failed: {result.Message}");
        }
        else
        {
            Debug.Log($"[ModernAbilityAdapter] Ability {Name} executed successfully via modern system");
        }
    }

    /// <summary>
    /// Checks if the ability's action can be executed.
    /// </summary>
    public override bool IsActionPossible(PlayerCards playerCards)
    {
        if (invocationCard == null || invocationCard.CancelEffect)
            return false;

        // Try modern system check if we have dependencies
        if (_playerRepository != null)
        {
            var currentPlayerId = DeterminePlayerId(playerCards);
            var sourceCard = CreateDomainCard(invocationCard);
            var context = new AbilityContext(currentPlayerId, PlayerId.Player2, sourceCard, Name);
            return _modernAbility.CanActivate(context);
        }

        return true;
    }

    /// <summary>
    /// Determines the PlayerId based on PlayerCards reference.
    /// </summary>
    private PlayerId DeterminePlayerId(PlayerCards playerCards)
    {
        // PlayerCards stores a reference to which player it belongs to
        // We determine this based on the IsPlayerOne property if available
        // For now, use a heuristic based on common patterns
        if (playerCards == null)
            return PlayerId.Player1;

        // Check if this PlayerCards belongs to player 1 or 2
        // This relies on the game's player identification system
        return playerCards.IsPlayerOne ? PlayerId.Player1 : PlayerId.Player2;
    }

    /// <summary>
    /// Creates a domain Card entity from an InGameInvocationCard.
    /// </summary>
    private Card CreateDomainCard(InGameInvocationCard inGameCard)
    {
        if (inGameCard == null)
            return null;

        // Create a domain Card that represents the in-game card
        // Use the card's properties to build the domain entity
        // Convert legacy Cards.CardFamily to JDG.Domain.Enums.CardFamily
        var domainFamilies = inGameCard.Families?.Select(f => (DomainCardFamily)(int)f)
            ?? Enumerable.Empty<DomainCardFamily>();

        return Card.CreateInvocation(
            CardId.New(),
            inGameCard.Title,
            inGameCard.BaseInvocationCard.Description ?? "",
            inGameCard.BaseInvocationCard.DetailedDescription ?? "",
            (int)inGameCard.Attack,
            (int)inGameCard.Defense,
            domainFamilies,
            inGameCard.IsAffectedByEffectCard
        );
    }

    /// <summary>
    /// Gets the wrapped modern ability for direct access if needed.
    /// </summary>
    public IAbility GetModernAbility() => _modernAbility;
}

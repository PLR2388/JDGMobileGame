using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using DomainCard = JDG.Domain.Entities.Card;
using JDG.Domain.ValueObjects;
using UnityEngine;

/// <summary>
/// Implementation of ICombatService.
/// Manages combat operations including attack validation, execution, and targeting.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (InGameCard, InGameInvocationCard, PlayerCards, etc.). It will be moved to
/// JDG.Infrastructure once these types are fully refactored.
///
/// Part of Phase 4 migration - decomposes CardManager god class.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 115: Updated to use modern EnableDirectAttackEffectAbility type.
/// Phase 118: Moved combat logic from legacy Ability class. Uses only ModernAbilities.
/// </summary>
public class CombatService : ICombatService
{
    private readonly ICardCollectionService _cardCollectionService;
    private readonly IPlayerStatusProvider _playerStatusProvider;
    private readonly IAbilityExecutor _abilityExecutor;
    private readonly ICardSyncService _cardSyncService;

    /// <summary>
    /// Phase 148: EventBus is intentionally optional to support test scenarios
    /// and legacy code paths where event publishing isn't needed. When null,
    /// attack events are not published. In production, EventBus is always
    /// injected via VContainer. Null-conditional access (?.) is used on publish.
    /// </summary>
    private readonly IEventBus _eventBus;
    private readonly Transform _canvas;

    public InGameInvocationCard Attacker { get; set; }
    public InGameInvocationCard Opponent { get; set; }

    /// <summary>
    /// Phase 144: Added IEventBus for publishing AttackExecutedEvent.
    /// Phase 144: Added null validation for required parameters.
    /// Phase 151: Added ICardSyncService for SourceCard conversion in ability contexts.
    /// </summary>
    public CombatService(
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        IAbilityExecutor abilityExecutor,
        ICardSyncService cardSyncService,
        IEventBus eventBus,
        Transform canvas)
    {
        // Phase 144: Validate required parameters
        _cardCollectionService = cardCollectionService ?? throw new System.ArgumentNullException(nameof(cardCollectionService));
        _playerStatusProvider = playerStatusProvider ?? throw new System.ArgumentNullException(nameof(playerStatusProvider));
        _abilityExecutor = abilityExecutor ?? throw new System.ArgumentNullException(nameof(abilityExecutor));
        _cardSyncService = cardSyncService; // Optional - null check on use (for test compatibility)
        _eventBus = eventBus; // Optional - null check on use
        _canvas = canvas; // Optional - can be null in tests
    }

    public bool CanAttackerAttack()
    {
        if (Attacker == null)
            return false;

        var currentPlayerCards = _cardCollectionService.GetCurrentPlayerCards();
        return Attacker.CanAttack() && currentPlayerCards.ContainsCardInInvocation(Attacker);
    }

    public bool HasAttackerAction()
    {
        return Attacker != null && Attacker.HasAction();
    }

    public float ComputeDamageAttack()
    {
        if (Opponent == null || Attacker == null)
            return 0f;

        return Opponent.GetCurrentDefense() - Attacker.GetCurrentAttack();
    }

    public void HandleAttack()
    {
        if (Attacker == null || Opponent == null)
            return;

        Attacker.AttackTurnDone();

        if (Opponent.Title == CardNameMappings.CardNameMap[CardNames.Player])
        {
            _playerStatusProvider.HandleAttackIfOpponentIsPlayer();
        }
        else
        {
            HandleAttackOverInvocation();
        }
    }

    public List<InGameCard> BuildValidTargets()
    {
        if (Attacker == null)
        {
            return new List<InGameCard>();
        }

        var opponentCards = _cardCollectionService.GetOpponentPlayerCards();
        var currentPlayerCards = _cardCollectionService.GetCurrentPlayerCards();

        var validTargets = FilterValidOpponentCards(opponentCards.InvocationCards);

        if (HasAggroCard(validTargets))
        {
            validTargets = GetOnlyAggroCards(validTargets);
        }
        else
        {
            RemoveCantBeAttackedCards(validTargets);

            // Phase 137: Add null check for Player entity
            if (opponentCards.Player == null)
            {
                Debug.LogWarning("CombatService.BuildValidTargets() - Opponent Player entity is NULL! " +
                    "Check that playerInvocationCard is assigned in Inspector for the opponent's PlayerCards.");
            }
            else
            {
                bool shouldAddPlayer = ShouldAddPlayerToTarget(currentPlayerCards.EffectCards, validTargets);
                if (shouldAddPlayer)
                {
                    validTargets.Add(opponentCards.Player);
                }
            }
        }

        // Phase 137: Add null check for Player entity
        bool canDirectAttack = AttackerCanDirectAttack();
        if (opponentCards.Player != null && canDirectAttack && !validTargets.Contains(opponentCards.Player))
        {
            validTargets.Add(opponentCards.Player);
        }

        return validTargets;
    }

    /// <summary>
    /// Executes special action abilities on the attacker card.
    /// Phase 118: Uses ModernAbilities with OnAction trigger.
    /// Phase 151: Now uses linked domain Card for proper ability context.
    /// </summary>
    public void UseSpecialAction()
    {
        if (Attacker == null)
            return;

        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        var opponentCards = _cardCollectionService.GetOpponentPlayerCards();

        // Phase 118: Execute modern abilities that are actions
        // Phase 151: Create linked domain Card for proper ability context
        var (context, domainCard) = CreateAbilityContext(Attacker, playerCards);
        // Phase 148: Added null-coalescing to prevent NullReferenceException
        foreach (var ability in Attacker.ModernAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
        {
            // Execute action-type abilities
            if (ability.CanActivate(context))
            {
                ability.Execute(context);
            }
        }

        // Phase 151: Sync state changes back to InGameInvocationCard
        SyncCardStateIfNeeded(domainCard);
    }

    /// <summary>
    /// Checks if the attacker has any actionable abilities.
    /// Phase 118: Uses ModernAbilities for action check.
    /// Phase 151: Now uses linked domain Card for proper ability context.
    /// </summary>
    public bool IsSpecialActionPossible()
    {
        if (Attacker == null)
            return false;

        if (Attacker.CancelEffect)
            return false;

        // Phase 118: Check if any modern ability can activate
        // Phase 151: Create linked domain Card for proper ability context
        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        var (context, _) = CreateAbilityContext(Attacker, playerCards);

        // Phase 148: Added null-safe check to prevent NullReferenceException
        return Attacker.ModernAbilities?.Any(ability => ability.CanActivate(context)) ?? false;
    }

    // Private helper methods

    /// <summary>
    /// Handles combat between two invocation cards.
    /// Phase 118: Moved combat logic from legacy Ability class.
    /// Phase 144: Added AttackExecutedEvent publishing.
    /// Phase 151: Now uses linked domain Cards for proper ability context.
    /// </summary>
    private void HandleAttackOverInvocation()
    {
        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        var opponentCards = _cardCollectionService.GetOpponentPlayerCards();
        var playerStatus = _playerStatusProvider.GetCurrentPlayerStatus();
        var opponentStatus = _playerStatusProvider.GetOpponentPlayerStatus();

        // Phase 118: Execute modern abilities with OnDefend trigger for defender
        // Phase 151: Create linked domain Cards for proper ability context
        var (defenderContext, defenderDomainCard) = CreateAbilityContext(Opponent, opponentCards);
        ExecuteModernAbilities(Opponent.ModernAbilities, AbilityTrigger.OnDefend, defenderContext);
        SyncCardStateIfNeeded(defenderDomainCard);

        // Phase 118: Execute modern abilities with OnAttack trigger for attacker
        var (attackerContext, attackerDomainCard) = CreateAbilityContext(Attacker, playerCards);
        ExecuteModernAbilities(Attacker.ModernAbilities, AbilityTrigger.OnAttack, attackerContext);
        SyncCardStateIfNeeded(attackerDomainCard);

        // Phase 118: Calculate and apply combat damage (moved from Ability.OnCardAttacked)
        float resultAttack = Opponent.Defense - Attacker.Attack;

        bool attackerDestroyed = false;
        bool defenderDestroyed = false;

        if (resultAttack > 0)
        {
            attackerDestroyed = HandlePositiveAttackResult(Attacker, playerCards, opponentCards, playerStatus, resultAttack);
        }
        else if (resultAttack == 0)
        {
            (attackerDestroyed, defenderDestroyed) = HandleNeutralAttackResult(Opponent, Attacker, playerCards, opponentCards);
        }
        else
        {
            defenderDestroyed = HandleNegativeAttackResult(Opponent, playerCards, opponentCards, opponentStatus, resultAttack);
        }

        // Phase 144: Publish AttackExecutedEvent for attack animations/UI feedback
        _eventBus?.Publish(new AttackExecutedEvent
        {
            AttackerId = Guid.NewGuid(), // Note: InGameInvocationCard doesn't have domain Id yet
            DefenderId = Guid.NewGuid(),
            Damage = (int)Mathf.Abs(resultAttack),
            DefenderDestroyed = defenderDestroyed,
            AttackerDestroyed = attackerDestroyed
        });
    }

    /// <summary>
    /// Creates an AbilityContext for modern ability execution.
    /// Phase 118: Added for modern ability migration.
    /// Phase 151: Now creates linked domain Card via CardSyncService for proper ability execution.
    /// </summary>
    private (AbilityContext context, DomainCard domainCard) CreateAbilityContext(InGameInvocationCard card, PlayerCards playerCards)
    {
        var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        var ownerId = PlayerId.FromCardOwner(owner);
        var opponentOwner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;
        var opponentId = PlayerId.FromCardOwner(opponentOwner);

        // Phase 151: Create linked domain Card for proper ability context
        DomainCard domainCard = null;
        if (_cardSyncService != null && card != null)
        {
            domainCard = _cardSyncService.CreateLinkedCard(card);
        }

        return (new AbilityContext(ownerId, opponentId, domainCard, JDG.Domain.AbilityName.Default), domainCard);
    }

    /// <summary>
    /// Syncs domain card state back to the InGame card after ability execution.
    /// Phase 151: Added to ensure ability modifications are reflected in game state.
    /// </summary>
    private void SyncCardStateIfNeeded(DomainCard domainCard)
    {
        if (_cardSyncService != null && domainCard != null)
        {
            _cardSyncService.SyncCardState(domainCard);
        }
    }

    /// <summary>
    /// Executes modern abilities with the specified trigger.
    /// Phase 118: Added for modern ability migration.
    /// Phase 152: Added null-coalescing to prevent NullReferenceException.
    /// </summary>
    private void ExecuteModernAbilities(List<IAbility> abilities, AbilityTrigger trigger, AbilityContext context)
    {
        // Phase 152: Added null-coalescing to prevent NullReferenceException
        foreach (var ability in abilities ?? System.Linq.Enumerable.Empty<IAbility>())
        {
            if (ability is IPassiveAbility passiveAbility && passiveAbility.Trigger == trigger)
            {
                if (ability.CanActivate(context))
                {
                    ability.Execute(context);
                }
            }
        }
    }

    /// <summary>
    /// Handles positive attack result (defender's DEF > attacker's ATK).
    /// Phase 118: Moved from legacy Ability class.
    /// Phase 144: Returns true if attacker was destroyed.
    /// </summary>
    private bool HandlePositiveAttackResult(
        InGameInvocationCard attacker,
        PlayerCards playerCards,
        PlayerCards opponentCards,
        PlayerStatus currentPlayerStatus,
        float resultAttack)
    {
        if (!IsEquipmentCardProtected(attacker, playerCards))
        {
            bool cardDied = HandleCardDeath(attacker, playerCards, opponentCards);
            if (cardDied)
            {
                currentPlayerStatus.ChangePv(-resultAttack);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Handles neutral attack result (defender's DEF == attacker's ATK).
    /// Phase 118: Moved from legacy Ability class.
    /// Phase 144: Returns tuple (attackerDestroyed, defenderDestroyed).
    /// </summary>
    private (bool attackerDestroyed, bool defenderDestroyed) HandleNeutralAttackResult(
        InGameInvocationCard attackedCard,
        InGameInvocationCard attacker,
        PlayerCards playerCards,
        PlayerCards opponentCards)
    {
        bool isProtectedAttacker = IsEquipmentCardProtected(attacker, playerCards);
        bool isProtectedAttacked = IsEquipmentCardProtected(attackedCard, opponentCards);
        bool defenderDestroyed = false;
        bool attackerDestroyed = false;

        if (!isProtectedAttacked)
        {
            defenderDestroyed = HandleCardDeath(attackedCard, opponentCards, playerCards);
        }

        if (!isProtectedAttacker)
        {
            attackerDestroyed = HandleCardDeath(attacker, playerCards, opponentCards);
        }

        return (attackerDestroyed, defenderDestroyed);
    }

    /// <summary>
    /// Handles negative attack result (defender's DEF < attacker's ATK).
    /// Phase 118: Moved from legacy Ability class.
    /// Phase 144: Returns true if defender was destroyed.
    /// </summary>
    private bool HandleNegativeAttackResult(
        InGameInvocationCard attackedCard,
        PlayerCards playerCards,
        PlayerCards opponentCards,
        PlayerStatus opponentPlayerStatus,
        float resultAttack)
    {
        if (!IsEquipmentCardProtected(attackedCard, opponentCards))
        {
            bool cardDied = HandleCardDeath(attackedCard, opponentCards, playerCards);
            if (cardDied)
            {
                opponentPlayerStatus.ChangePv(resultAttack);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a card is protected from destruction by equipment.
    /// Phase 118: Moved from legacy Ability class.
    /// Phase 151: Now uses linked domain Card for proper ability context.
    /// </summary>
    private bool IsEquipmentCardProtected(InGameInvocationCard card, PlayerCards playerCards)
    {
        var equipmentCard = card.EquipmentCard;
        if (equipmentCard == null)
            return false;

        // Check if any equipment ability prevents destruction
        // Phase 151: Create linked domain Card for proper ability context
        var (context, _) = CreateAbilityContext(card, playerCards);

        // Phase 148: Added null-coalescing to prevent NullReferenceException
        return (equipmentCard.ModernEquipmentAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
            .OfType<IEquipmentAbility>()
            .Any(ability => !ability.OnPreDestroy(context));
    }

    /// <summary>
    /// Handles card death logic including equipment removal and graveyard placement.
    /// Phase 118: Moved from legacy Ability class.
    /// Phase 151: Now uses linked domain Card for proper ability context.
    /// </summary>
    /// <returns>True if the card actually died, false if it was protected.</returns>
    private bool HandleCardDeath(
        InGameInvocationCard deadCard,
        PlayerCards ownerCards,
        PlayerCards opponentCards)
    {
        // Check if card is in yellow cards (already dead)
        if (ownerCards.YellowCards.Contains(deadCard))
            return false;

        // Handle equipment removal
        var equipmentCard = deadCard.EquipmentCard;
        if (equipmentCard != null)
        {
            // Execute OnUnequip abilities
            // Phase 151: Create linked domain Card for proper ability context
            var (context, domainCard) = CreateAbilityContext(deadCard, ownerCards);

            // Phase 148: Added null-coalescing to prevent NullReferenceException
            foreach (var ability in equipmentCard.ModernEquipmentAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
            {
                if (ability is IPassiveAbility passiveAbility && passiveAbility.Trigger == AbilityTrigger.OnUnequip)
                {
                    if (ability.CanActivate(context))
                    {
                        ability.Execute(context);
                    }
                }
            }

            // Phase 151: Sync state changes back to InGameInvocationCard
            SyncCardStateIfNeeded(domainCard);

            ownerCards.YellowCards.Add(equipmentCard);
            deadCard.EquipmentCard = null;
        }

        // Increment death counter
        deadCard.IncrementNumberDeaths();

        // Execute death abilities via IAbilityExecutor
        _abilityExecutor.ExecuteOnCardDeath(deadCard, ownerCards, opponentCards);

        // Move card to graveyard
        ownerCards.InvocationCards.Remove(deadCard);
        ownerCards.YellowCards.Add(deadCard);

        return true;
    }

    private List<InGameCard> FilterValidOpponentCards(System.Collections.Generic.IEnumerable<InGameCard> cards)
    {
        return cards.Where(card => card != null && card.Title != null).ToList();
    }

    private bool HasAggroCard(List<InGameCard> cards)
    {
        return cards.Any(card => card is InGameInvocationCard invocationCard && invocationCard.Aggro);
    }

    private List<InGameCard> GetOnlyAggroCards(List<InGameCard> cards)
    {
        return cards.Where(card => card is InGameInvocationCard invocationCard && invocationCard.Aggro).ToList();
    }

    private void RemoveCantBeAttackedCards(List<InGameCard> cards)
    {
        cards.RemoveAll(card => card is InGameInvocationCard invocationCard && invocationCard.CantBeAttack);
    }

    /// <summary>
    /// Phase 115: Updated to use modern EnableDirectAttackEffectAbility type.
    /// </summary>
    private bool ShouldAddPlayerToTarget(
        System.Collections.ObjectModel.ObservableCollection<InGameEffectCard> effectCards,
        List<InGameCard> validTargets)
    {
        return !validTargets.Any() ||
               effectCards.Any(card => card.ModernEffectAbilities.Any(ability => ability is EnableDirectAttackEffectAbility));
    }

    private bool AttackerCanDirectAttack()
    {
        return Attacker.CanDirectAttack;
    }
}

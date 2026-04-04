using System.Linq;
using Cards;
using JDG.Application.Abilities;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using DomainCard = JDG.Domain.Entities.Card;
using JDG.Infrastructure.Cards;
using JDG.Infrastructure.Services;
using UnityEngine;

/// <summary>
/// Implementation of ITurnService.
/// Manages turn lifecycle and processes card abilities at turn start/end.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (PlayerCardManager, PlayerCards, PlayerStatus). It will be moved to JDG.Infrastructure
/// once these types are fully refactored.
///
/// Part of Phase 4 migration - decomposes CardManager god class.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 115: Updated to use modern IAbility for effect cards.
/// Phase 118: Uses only ModernAbilities for all card types.
/// Phase 151: Added ICardSyncService for SourceCard conversion in ability contexts.
/// </summary>
public class TurnService : ITurnService
{
    private readonly GameStateService _gameStateService;
    private readonly PlayerCardManager _player1CardManager;
    private readonly PlayerCardManager _player2CardManager;
    private readonly IPlayerStatusProvider _playerStatusProvider;
    private readonly ICardSyncService _cardSyncService;
    private readonly Transform _canvas;

    /// <summary>
    /// Phase 157: Added null validation for critical dependencies to prevent runtime crashes.
    /// </summary>
    public TurnService(
        GameStateService gameStateService,
        PlayerCardManager player1CardManager,
        PlayerCardManager player2CardManager,
        IPlayerStatusProvider playerStatusProvider,
        ICardSyncService cardSyncService,
        Transform canvas)
    {
        // Phase 157: Validate critical dependencies that would cause NullReferenceException if null
        _gameStateService = gameStateService ?? throw new System.ArgumentNullException(
            nameof(gameStateService), "TurnService requires GameStateService for turn management");
        _player1CardManager = player1CardManager ?? throw new System.ArgumentNullException(
            nameof(player1CardManager), "TurnService requires Player1 CardManager");
        _player2CardManager = player2CardManager ?? throw new System.ArgumentNullException(
            nameof(player2CardManager), "TurnService requires Player2 CardManager");
        _playerStatusProvider = playerStatusProvider ?? throw new System.ArgumentNullException(
            nameof(playerStatusProvider), "TurnService requires IPlayerStatusProvider");

        // Optional dependencies (used with null-conditional operators)
        _cardSyncService = cardSyncService; // Phase 151: For ability context card conversion
        _canvas = canvas; // Can be null in test scenarios
    }

    public void OnTurnStart()
    {
        var (playerCards, opponentCards) = GetCurrentAndOpponentCards();
        var playerStatus = _playerStatusProvider.GetCurrentPlayerStatus();
        var opponentStatus = _playerStatusProvider.GetOpponentPlayerStatus();

        playerCards.ResetInvocationCardNewTurn();

        // Create copies to avoid collection modification during iteration
        var copyInvocationCards = playerCards.InvocationCards.ToList();
        var copyOpponentInvocationCards = opponentCards.InvocationCards.ToList();
        var copyEffectCards = playerCards.EffectCards.ToList();
        var copyOpponentEffectCards = opponentCards.EffectCards.ToList();

        // Apply abilities for both players
        ApplyInvocationOnTurnStart(copyInvocationCards, playerCards, opponentCards);
        ApplyInvocationOnTurnStart(copyOpponentInvocationCards, opponentCards, playerCards);

        ApplyEffectOnTurnStart(copyEffectCards, playerStatus, playerCards, opponentStatus, opponentCards);
        ApplyEffectOnTurnStart(copyOpponentEffectCards, opponentStatus, opponentCards, playerStatus, playerCards);

        ApplyFieldOnTurnStart(playerCards, playerStatus);
    }

    public void HandleEndTurn()
    {
        var currentCardManager = GetCurrentPlayerCardManager();
        currentCardManager.ProcessEndOfTurn();
    }

    private (PlayerCards current, PlayerCards opponent) GetCurrentAndOpponentCards()
    {
        var isP1Turn = _gameStateService.CurrentPlayer == PlayerId.Player1;
        if (isP1Turn)
        {
            return (_player1CardManager.PlayerCards, _player2CardManager.PlayerCards);
        }
        else
        {
            return (_player2CardManager.PlayerCards, _player1CardManager.PlayerCards);
        }
    }

    private PlayerCardManager GetCurrentPlayerCardManager()
    {
        var isP1Turn = _gameStateService.CurrentPlayer == PlayerId.Player1;
        return isP1Turn ? _player1CardManager : _player2CardManager;
    }

    /// <summary>
    /// Phase 118: Uses only ModernAbilities for invocation cards.
    /// Phase 151: Now uses linked domain Card for proper ability context.
    /// </summary>
    private void ApplyInvocationOnTurnStart(
        System.Collections.Generic.List<InGameInvocationCard> invocationCards,
        PlayerCards playerCards,
        PlayerCards opponentCards)
    {
        foreach (var invocationCard in invocationCards)
        {
            // Phase 151: Create linked domain Card for proper ability context
            var (context, domainCard) = CreateAbilityContext(invocationCard, playerCards);

            // Phase 118: Use modern abilities with OnTurnStart trigger for invocations
            // Phase 148: Added null-coalescing to prevent NullReferenceException
            foreach (var ability in invocationCard.ModernAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
            {
                if (ability is IPassiveAbility passiveAbility && passiveAbility.Trigger == AbilityTrigger.OnTurnStart)
                {
                    if (ability.CanActivate(context))
                    {
                        ability.Execute(context);
                    }
                }
            }

            // Phase 151: Sync state changes back to InGameInvocationCard
            SyncCardStateIfNeeded(domainCard);

            // Equipment abilities
            if (invocationCard.EquipmentCard != null)
            {
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var ability in invocationCard.EquipmentCard.ModernEquipmentAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
                {
                    if (ability is IPassiveAbility equipPassive && equipPassive.Trigger == AbilityTrigger.OnTurnStart)
                    {
                        if (ability.CanActivate(context))
                        {
                            ability.Execute(context);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates an AbilityContext for modern ability execution.
    /// Phase 151: Added to create linked domain Card for proper ability execution.
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
    /// Phase 115: Updated to use modern IAbility with AbilityTrigger.OnTurnStart.
    /// Phase 151: Effect abilities don't require SourceCard since they don't reference themselves.
    /// </summary>
    private void ApplyEffectOnTurnStart(
        System.Collections.Generic.List<InGameEffectCard> effectCards,
        PlayerStatus playerStatus,
        PlayerCards playerCards,
        PlayerStatus opponentStatus,
        PlayerCards opponentCards)
    {
        // Phase 151: Create simple context for effect cards (no SourceCard needed)
        // Effect abilities operate on game state, not on themselves
        var context = CreateSimpleContext(playerCards);

        foreach (var effectCard in effectCards)
        {
            // Phase 148: Added null-coalescing to prevent NullReferenceException
            foreach (var ability in effectCard.ModernEffectAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
            {
                // Only execute passive abilities with OnTurnStart trigger
                if (ability is IPassiveAbility passiveAbility && passiveAbility.Trigger == AbilityTrigger.OnTurnStart)
                {
                    if (ability.CanActivate(context))
                    {
                        ability.Execute(context);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Phase 116: Updated to use modern IAbility with AbilityTrigger.OnTurnStart.
    /// Phase 151: Field abilities don't require SourceCard since they don't reference themselves.
    /// </summary>
    private void ApplyFieldOnTurnStart(PlayerCards playerCards, PlayerStatus playerStatus)
    {
        if (playerCards.FieldCard == null)
            return;

        // Phase 151: Create simple context for field cards (no SourceCard needed)
        // Field abilities provide passive boosts, don't need to reference themselves
        var context = CreateSimpleContext(playerCards);

        // Phase 148: Added null-coalescing to prevent NullReferenceException
        foreach (var ability in playerCards.FieldCard.ModernFieldAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
        {
            // Only execute passive abilities with OnTurnStart trigger
            if (ability is IPassiveAbility passiveAbility && passiveAbility.Trigger == AbilityTrigger.OnTurnStart)
            {
                if (ability.CanActivate(context))
                {
                    ability.Execute(context);
                }
            }
        }
    }

    /// <summary>
    /// Creates a simple AbilityContext without a linked card.
    /// Phase 151: Used for effect/field abilities that don't need SourceCard.
    /// </summary>
    private AbilityContext CreateSimpleContext(PlayerCards playerCards)
    {
        var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        var ownerId = PlayerId.FromCardOwner(owner);
        var opponentOwner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;
        var opponentId = PlayerId.FromCardOwner(opponentOwner);
        return new AbilityContext(ownerId, opponentId, null, JDG.Domain.AbilityName.Default);
    }
}

using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using JDG.Application.Abilities;
using JDG.Domain;
using JDG.Domain.ValueObjects;
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
/// </summary>
public class TurnService : ITurnService
{
    private readonly GameStateService _gameStateService;
    private readonly PlayerCardManager _player1CardManager;
    private readonly PlayerCardManager _player2CardManager;
    private readonly IPlayerStatusProvider _playerStatusProvider;
    private readonly Transform _canvas;

    public TurnService(
        GameStateService gameStateService,
        PlayerCardManager player1CardManager,
        PlayerCardManager player2CardManager,
        IPlayerStatusProvider playerStatusProvider,
        Transform canvas)
    {
        _gameStateService = gameStateService;
        _player1CardManager = player1CardManager;
        _player2CardManager = player2CardManager;
        _playerStatusProvider = playerStatusProvider;
        _canvas = canvas;
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

    private void ApplyInvocationOnTurnStart(
        System.Collections.Generic.List<InGameInvocationCard> invocationCards,
        PlayerCards playerCards,
        PlayerCards opponentCards)
    {
        foreach (var invocationCard in invocationCards)
        {
            foreach (var ability in invocationCard.Abilities)
            {
                ability.OnTurnStart(_canvas, playerCards, opponentCards);
            }

            if (invocationCard.EquipmentCard != null)
            {
                foreach (var equipmentAbility in invocationCard.EquipmentCard.EquipmentAbilities)
                {
                    equipmentAbility.OnTurnStart(invocationCard);
                }
            }
        }
    }

    /// <summary>
    /// Phase 115: Updated to use modern IAbility with AbilityTrigger.OnTurnStart.
    /// </summary>
    private void ApplyEffectOnTurnStart(
        System.Collections.Generic.List<InGameEffectCard> effectCards,
        PlayerStatus playerStatus,
        PlayerCards playerCards,
        PlayerStatus opponentStatus,
        PlayerCards opponentCards)
    {
        // Phase 115: Use modern abilities with IPassiveAbility.Trigger check
        var owner = playerCards.IsPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
        var ownerId = PlayerId.FromCardOwner(owner);
        var opponentOwner = playerCards.IsPlayerOne ? CardOwner.Player2 : CardOwner.Player1;
        var opponentId = PlayerId.FromCardOwner(opponentOwner);

        foreach (var effectCard in effectCards)
        {
            var context = new AbilityContext(ownerId, opponentId, null, JDG.Domain.Enums.AbilityName.Default);

            foreach (var ability in effectCard.ModernEffectAbilities)
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

    private void ApplyFieldOnTurnStart(PlayerCards playerCards, PlayerStatus playerStatus)
    {
        if (playerCards.FieldCard != null)
        {
            foreach (var fieldAbility in playerCards.FieldCard.FieldAbilities)
            {
                fieldAbility.OnTurnStart(_canvas, playerCards, playerStatus);
            }
        }
    }
}

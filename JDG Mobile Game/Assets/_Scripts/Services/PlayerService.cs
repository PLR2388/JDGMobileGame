using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Events;
using System.Collections.Generic;

/// <summary>
/// Implementation of IPlayerService.
/// Manages player state (health, shields, block attack) and publishes events.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (PlayerStatus MonoBehaviour). It will be moved to JDG.Infrastructure once
/// PlayerStatus is fully refactored to use PlayerState entity.
///
/// Part of Phase 3 migration - replaces PlayerManager singleton with DI.
/// </summary>
public class PlayerService : IPlayerService
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<CardOwner, PlayerState> _playerStates;

    public PlayerService(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _playerStates = new Dictionary<CardOwner, PlayerState>
        {
            { CardOwner.Player1, PlayerState.CreateDefault(CardOwner.Player1) },
            { CardOwner.Player2, PlayerState.CreateDefault(CardOwner.Player2) }
        };
    }

    public PlayerState GetPlayerState(CardOwner playerId)
    {
        return _playerStates[playerId];
    }

    public void ChangeHealth(CardOwner playerId, int delta)
    {
        var currentState = _playerStates[playerId];
        var oldHealth = currentState.CurrentHealth;
        var newState = currentState.ChangeHealth(delta);
        _playerStates[playerId] = newState;

        // Publish event
        _eventBus.Publish(new PlayerHealthChangedEvent
        {
            Player = playerId,
            OldHealth = oldHealth,
            NewHealth = newState.CurrentHealth,
            Delta = delta
        });
    }

    public void SetHealth(CardOwner playerId, int health)
    {
        var currentState = _playerStates[playerId];
        var oldHealth = currentState.CurrentHealth;
        var newState = currentState.WithHealth(health);
        _playerStates[playerId] = newState;

        // Publish event
        _eventBus.Publish(new PlayerHealthChangedEvent
        {
            Player = playerId,
            OldHealth = oldHealth,
            NewHealth = newState.CurrentHealth,
            Delta = newState.CurrentHealth - oldHealth
        });
    }

    public void SetShieldCount(CardOwner playerId, int shieldCount)
    {
        var currentState = _playerStates[playerId];
        var oldShields = currentState.ShieldCount;
        var newState = currentState.WithShields(shieldCount);
        _playerStates[playerId] = newState;

        // Publish event
        _eventBus.Publish(new PlayerShieldChangedEvent
        {
            Player = playerId,
            OldShields = oldShields,
            NewShields = newState.ShieldCount
        });
    }

    public void DecrementShield(CardOwner playerId)
    {
        var currentState = _playerStates[playerId];
        var oldShields = currentState.ShieldCount;
        var newState = currentState.DecrementShield();
        _playerStates[playerId] = newState;

        // Publish event
        _eventBus.Publish(new PlayerShieldChangedEvent
        {
            Player = playerId,
            OldShields = oldShields,
            NewShields = newState.ShieldCount
        });
    }

    public void EnableBlockAttack(CardOwner playerId)
    {
        var currentState = _playerStates[playerId];
        var newState = currentState.EnableBlockAttack();
        _playerStates[playerId] = newState;
    }

    public void DisableBlockAttack(CardOwner playerId)
    {
        var currentState = _playerStates[playerId];
        var newState = currentState.DisableBlockAttack();
        _playerStates[playerId] = newState;
    }

    public bool IsPlayerDefeated(CardOwner playerId)
    {
        return _playerStates[playerId].IsDefeated;
    }

    public bool HasShields(CardOwner playerId)
    {
        return _playerStates[playerId].HasShields;
    }

    public void ResetPlayers()
    {
        _playerStates[CardOwner.Player1] = PlayerState.CreateDefault(CardOwner.Player1);
        _playerStates[CardOwner.Player2] = PlayerState.CreateDefault(CardOwner.Player2);
    }
}

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
    private readonly Dictionary<JDG.Domain.CardOwner, PlayerState> _playerStates;

    public PlayerService(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _playerStates = new Dictionary<JDG.Domain.CardOwner, PlayerState>
        {
            { JDG.Domain.CardOwner.Player1, PlayerState.CreateDefault(JDG.Domain.CardOwner.Player1) },
            { JDG.Domain.CardOwner.Player2, PlayerState.CreateDefault(JDG.Domain.CardOwner.Player2) }
        };
    }

    public PlayerState GetPlayerState(JDG.Domain.CardOwner playerId)
    {
        return _playerStates[playerId];
    }

    /// <summary>
    /// Phase 151: Changed delta to float for half-star damage support.
    /// </summary>
    public void ChangeHealth(JDG.Domain.CardOwner playerId, float delta)
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

    /// <summary>
    /// Phase 151: Changed health to float for half-star damage support.
    /// </summary>
    public void SetHealth(JDG.Domain.CardOwner playerId, float health)
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

    public void SetShieldCount(JDG.Domain.CardOwner playerId, int shieldCount)
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

    public void DecrementShield(JDG.Domain.CardOwner playerId)
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

    public void EnableBlockAttack(JDG.Domain.CardOwner playerId)
    {
        var currentState = _playerStates[playerId];
        var newState = currentState.EnableBlockAttack();
        _playerStates[playerId] = newState;
    }

    public void DisableBlockAttack(JDG.Domain.CardOwner playerId)
    {
        var currentState = _playerStates[playerId];
        var newState = currentState.DisableBlockAttack();
        _playerStates[playerId] = newState;
    }

    public bool IsPlayerDefeated(JDG.Domain.CardOwner playerId)
    {
        return _playerStates[playerId].IsDefeated;
    }

    public bool HasShields(JDG.Domain.CardOwner playerId)
    {
        return _playerStates[playerId].HasShields;
    }

    public void ResetPlayers()
    {
        _playerStates[JDG.Domain.CardOwner.Player1] = PlayerState.CreateDefault(JDG.Domain.CardOwner.Player1);
        _playerStates[JDG.Domain.CardOwner.Player2] = PlayerState.CreateDefault(JDG.Domain.CardOwner.Player2);
    }
}

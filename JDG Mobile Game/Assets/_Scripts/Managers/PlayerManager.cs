using JDG.Domain.ValueObjects;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Services;
using UnityEngine;

/// <summary>
/// Manages player-related functionalities such as retrieving the current player status or handling attacks on the opponent.
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private PlayerStatus playerStatus1;
    [SerializeField] private PlayerStatus playerStatus2;

    // Phase 2: Temporary bridge to GameStateService during migration
    // This will be removed when PlayerManager is replaced by PlayerService in Phase 3
    private GameStateService GameStateService => ServiceLocator.Get<GameStateService>();
    private bool IsP1Turn => GameStateService.CurrentPlayer == PlayerId.Player1;

    /// <summary>
    /// Retrieves the current player's status.
    /// </summary>
    /// <returns>PlayerStatus of the current player.</returns>
    public PlayerStatus GetCurrentPlayerStatus()
    {
        return IsP1Turn ? playerStatus1 : playerStatus2;
    }

    /// <summary>
    /// Retrieves the opponent player's status.
    /// </summary>
    /// <returns>PlayerStatus of the opponent player.</returns>
    public PlayerStatus GetOpponentPlayerStatus()
    {
        return IsP1Turn ? playerStatus2 : playerStatus1;
    }

    /// <summary>
    /// Awake method to initialize components and settings.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        InitShieldCount();
    }
    
    /// <summary>
    /// Initializes the shield count for both players to zero.
    /// </summary>
    private void InitShieldCount()
    {

        playerStatus1.SetShieldCount(0);
        playerStatus2.SetShieldCount(0);
    }

    /// <summary>
    /// Handles the attack on the opponent player. 
    /// If the opponent has a shield, it decrements the shield. Otherwise, it computes the damage and applies it.
    /// </summary>
    public void HandleAttackIfOpponentIsPlayer()
    {
        var opponentPlayerStatus = GetOpponentPlayerStatus();
        // Directly attack the player
        if (opponentPlayerStatus.NumberShield > 0)
        {
            opponentPlayerStatus.DecrementShield();
        }
        else
        {
            var diff = CardManager.Instance.ComputeDamageAttack();
            opponentPlayerStatus.ChangePv(diff);
        }
    }
}
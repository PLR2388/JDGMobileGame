using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Services;
using UnityEngine;

/// <summary>
/// Manages player-related functionalities such as retrieving the current player status or handling attacks on the opponent.
/// Phase 3: Now uses IPlayerService for state management. PlayerStatus MonoBehaviours are kept
/// temporarily to maintain UI compatibility during migration.
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private PlayerStatus playerStatus1;
    [SerializeField] private PlayerStatus playerStatus2;

    // Phase 2: Temporary bridge to GameStateService during migration
    private GameStateService GameStateService => ServiceLocator.Get<GameStateService>();
    private bool IsP1Turn => GameStateService.CurrentPlayer == PlayerId.Player1;

    // Phase 3: Temporary bridge to PlayerService during migration
    // This will be removed when PlayerManager singleton is fully replaced
    private IPlayerService PlayerService => ServiceLocator.Get<IPlayerService>();

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
    /// Phase 3: Now uses PlayerService to manage state.
    /// </summary>
    private void InitShieldCount()
    {
        PlayerService.SetShieldCount(JDG.Domain.CardOwner.Player1, 0);
        PlayerService.SetShieldCount(JDG.Domain.CardOwner.Player2, 0);

        // Sync legacy PlayerStatus MonoBehaviours with service state
        SyncPlayerStatusWithService(JDG.Domain.CardOwner.Player1);
        SyncPlayerStatusWithService(JDG.Domain.CardOwner.Player2);
    }

    /// <summary>
    /// Handles the attack on the opponent player.
    /// If the opponent has a shield, it decrements the shield. Otherwise, it computes the damage and applies it.
    /// Phase 3: Now uses PlayerService for state management and event publishing.
    /// </summary>
    public void HandleAttackIfOpponentIsPlayer()
    {
        var opponentId = IsP1Turn ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;

        // Directly attack the player
        if (PlayerService.HasShields(opponentId))
        {
            PlayerService.DecrementShield(opponentId);
        }
        else
        {
            var diff = CardManager.Instance.ComputeDamageAttack();
            PlayerService.ChangeHealth(opponentId, diff);
        }

        // Sync legacy PlayerStatus MonoBehaviours with service state
        SyncPlayerStatusWithService(opponentId);
    }

    /// <summary>
    /// Syncs the legacy PlayerStatus MonoBehaviour with the current PlayerService state.
    /// This bridge method maintains UI compatibility during migration.
    /// </summary>
    private void SyncPlayerStatusWithService(JDG.Domain.CardOwner playerId)
    {
        var state = PlayerService.GetPlayerState(playerId);
        var playerStatus = playerId == JDG.Domain.CardOwner.Player1 ? playerStatus1 : playerStatus2;

        // Update MonoBehaviour to match service state (without triggering events)
        playerStatus.SetHealthDirect((float)state.CurrentHealth);
        playerStatus.SetShieldCount(state.ShieldCount);
    }
}
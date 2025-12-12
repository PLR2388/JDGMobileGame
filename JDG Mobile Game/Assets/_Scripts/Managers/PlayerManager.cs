using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using UnityEngine;
using VContainer;

/// <summary>
/// Manages player-related functionalities such as retrieving the current player status or handling attacks on the opponent.
/// Phase 3: Now uses IPlayerService for state management. PlayerStatus MonoBehaviours are kept
/// temporarily to maintain UI compatibility during migration.
/// Phase 17-18: Removed CardManager singleton dependency via ICombatService.
/// Phase 24-25: Removed ServiceLocator, using VContainer DI.
/// Phase 28: Removed singleton pattern, implements IPlayerStatusProvider for DI.
/// </summary>
public class PlayerManager : MonoBehaviour, IPlayerStatusProvider
{
    [SerializeField] private PlayerStatus playerStatus1;
    [SerializeField] private PlayerStatus playerStatus2;

    // Phase 24-25: Injected via VContainer
    private GameStateService _gameStateService;
    private bool IsP1Turn => _gameStateService.CurrentPlayer == PlayerId.Player1;

    private IPlayerService _playerService;
    private ICombatService _combatService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 17-18: Inject ICombatService instead of CardManager.Instance.
    /// Phase 24-25: Inject GameStateService and IPlayerService instead of ServiceLocator.
    /// </summary>
    [Inject]
    public void Construct(ICombatService combatService, GameStateService gameStateService, IPlayerService playerService)
    {
        _combatService = combatService;
        _gameStateService = gameStateService;
        _playerService = playerService;
    }

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
    /// Phase 28: Removed Singleton base class, now regular MonoBehaviour.
    /// </summary>
    private void Awake()
    {
        InitShieldCount();
    }

    /// <summary>
    /// Initializes the shield count for both players to zero.
    /// Phase 3: Now uses PlayerService to manage state.
    /// Phase 24-25: Uses injected _playerService.
    /// </summary>
    private void InitShieldCount()
    {
        _playerService.SetShieldCount(JDG.Domain.CardOwner.Player1, 0);
        _playerService.SetShieldCount(JDG.Domain.CardOwner.Player2, 0);

        // Sync legacy PlayerStatus MonoBehaviours with service state
        SyncPlayerStatusWithService(JDG.Domain.CardOwner.Player1);
        SyncPlayerStatusWithService(JDG.Domain.CardOwner.Player2);
    }

    /// <summary>
    /// Handles the attack on the opponent player.
    /// If the opponent has a shield, it decrements the shield. Otherwise, it computes the damage and applies it.
    /// Phase 3: Now uses PlayerService for state management and event publishing.
    /// Phase 17-18: Now uses ICombatService instead of CardManager.Instance.
    /// Phase 24-25: Uses injected _playerService.
    /// </summary>
    public void HandleAttackIfOpponentIsPlayer()
    {
        var opponentId = IsP1Turn ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;

        // Directly attack the player
        if (_playerService.HasShields(opponentId))
        {
            _playerService.DecrementShield(opponentId);
        }
        else
        {
            // Phase 17-18: Use ICombatService instead of CardManager.Instance
            var diff = _combatService.ComputeDamageAttack();
            _playerService.ChangeHealth(opponentId, (int)diff);
        }

        // Sync legacy PlayerStatus MonoBehaviours with service state
        SyncPlayerStatusWithService(opponentId);
    }

    /// <summary>
    /// Syncs the legacy PlayerStatus MonoBehaviour with the current PlayerService state.
    /// This bridge method maintains UI compatibility during migration.
    /// Phase 24-25: Uses injected _playerService.
    /// </summary>
    private void SyncPlayerStatusWithService(JDG.Domain.CardOwner playerId)
    {
        var state = _playerService.GetPlayerState(playerId);
        var playerStatus = playerId == JDG.Domain.CardOwner.Player1 ? playerStatus1 : playerStatus2;

        // Update MonoBehaviour to match service state (without triggering events)
        playerStatus.SetHealthDirect((float)state.CurrentHealth);
        playerStatus.SetShieldCount(state.ShieldCount);
    }
}
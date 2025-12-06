using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using UnityEngine;
using VContainer;

/// <summary>
/// View component for displaying player health and shield status.
/// Phase 21-22: Refactored to be a thin view that delegates to PlayerService.
/// All business logic moved to PlayerService, this class only handles UI state.
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    public const float MaxHealth = 30f;

    [SerializeField]
    private float currentHealth = 30f;

    [SerializeField] private bool isP1;

    // Phase 21-22: Injected dependencies
    private IPlayerService _playerService;
    private IEventBus _eventBus;

    private JDG.Domain.CardOwner PlayerId => isP1 ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 21-22: Inject IPlayerService and IEventBus.
    /// </summary>
    [Inject]
    public void Construct(IPlayerService playerService, IEventBus eventBus)
    {
        _playerService = playerService;
        _eventBus = eventBus;
    }

    private void Start()
    {
        // Subscribe to EventBus events for this player
        _eventBus.Subscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
        _eventBus.Subscribe<PlayerShieldChangedEvent>(OnPlayerShieldChanged);

        // Initialize local state from PlayerService
        var playerState = _playerService.GetPlayerState(PlayerId);
        currentHealth = playerState.CurrentHealth;
    }

    private void OnDestroy()
    {
        // EventBus subscriptions are automatically cleaned up
    }

    /// <summary>
    /// EventBus handler for health changes.
    /// Updates local UI state when player health changes.
    /// </summary>
    private void OnPlayerHealthChanged(PlayerHealthChangedEvent evt)
    {
        if (evt.Player == PlayerId)
        {
            currentHealth = evt.NewHealth;
        }
    }

    /// <summary>
    /// EventBus handler for shield changes.
    /// Updates local UI state when player shields change.
    /// </summary>
    private void OnPlayerShieldChanged(PlayerShieldChangedEvent evt)
    {
        if (evt.Player == PlayerId)
        {
            // Shield count is tracked in PlayerService, not locally
        }
    }

    /// <summary>
    /// Gets the number of shields the player has.
    /// Phase 21-22: Delegates to PlayerService.
    /// </summary>
    public int NumberShield => _playerService?.GetPlayerState(PlayerId).ShieldCount ?? 0;

    /// <summary>
    /// Gets whether the player can block an attack.
    /// Phase 21-22: Delegates to PlayerService.
    /// </summary>
    public bool BlockAttack => _playerService?.GetPlayerState(PlayerId).CanBlockAttack ?? false;

    /// <summary>
    /// Changes the player's health by the given amount.
    /// Phase 21-22: Delegates to PlayerService, which publishes PlayerHealthChangedEvent.
    /// </summary>
    /// <param name="pv">Amount to change the health by (can be positive or negative).</param>
    public void ChangePv(float pv)
    {
        _playerService.ChangeHealth(PlayerId, (int)pv);
        // currentHealth will be updated by OnPlayerHealthChanged event handler
    }

    /// <summary>
    /// Gets the current health of the player.
    /// Phase 21-22: Returns local UI state (updated by EventBus).
    /// </summary>
    /// <returns>The current health value.</returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Sets the health directly.
    /// Phase 21-22: Delegates to PlayerService.SetHealth().
    /// </summary>
    /// <param name="health">The health value to set.</param>
    public void SetHealthDirect(float health)
    {
        _playerService.SetHealth(PlayerId, (int)health);
        // currentHealth will be updated by OnPlayerHealthChanged event handler
    }

    /// <summary>
    /// Sets the shield count for the player.
    /// Phase 21-22: Delegates to PlayerService.
    /// </summary>
    /// <param name="number">The number of shields to set.</param>
    public void SetShieldCount(int number)
    {
        _playerService.SetShieldCount(PlayerId, number);
    }

    /// <summary>
    /// Decreases the shield count by one.
    /// Phase 21-22: Delegates to PlayerService.
    /// </summary>
    public void DecrementShield()
    {
        _playerService.DecrementShield(PlayerId);
    }

    /// <summary>
    /// Enables the player to block an attack.
    /// Phase 21-22: Delegates to PlayerService.
    /// </summary>
    public void EnableBlockAttack()
    {
        _playerService.EnableBlockAttack(PlayerId);
    }

    /// <summary>
    /// Disables the player's ability to block an attack.
    /// Phase 21-22: Delegates to PlayerService.
    /// </summary>
    public void DisableBlockAttack()
    {
        _playerService.DisableBlockAttack(PlayerId);
    }
}
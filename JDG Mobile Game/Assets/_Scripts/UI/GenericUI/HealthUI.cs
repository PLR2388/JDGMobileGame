using System;
using JDG.Application;
using JDG.Domain.Events;
using TMPro;
using UnityEngine;
using VContainer;

/// <summary>
/// Represents the user interface for displaying player health.
/// Phase 21-22: Migrated from static PlayerStatus.OnHealthChanged to EventBus.
/// </summary>
public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthP1Text;
    [SerializeField] private TextMeshProUGUI healthP2Text;

    // Phase 21-22: Injected dependency
    private IEventBus _eventBus;

    // Phase 144: Store subscription for proper disposal
    private IDisposable _healthChangedSubscription;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 21-22: Inject IEventBus to subscribe to PlayerHealthChangedEvent.
    /// </summary>
    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Initialization logic for the health UI.
    /// Phase 21-22: Subscribes to EventBus instead of static UnityEvent.
    /// Phase 46: Moved EventBus subscription to Start to ensure VContainer injection is complete.
    /// </summary>
    private void Awake()
    {
        // Initialize the health text for both players at the start.
        SetHealthText(PlayerStatus.MaxHealth, true);
        SetHealthText(PlayerStatus.MaxHealth, false);
    }

    /// <summary>
    /// Subscribe to events after VContainer injection is complete.
    /// Phase 46: Moved from Awake to Start.
    /// </summary>
    private void Start()
    {
        // Phase 21-22: Subscribe to EventBus PlayerHealthChangedEvent
        // Phase 144: Store subscription for disposal in OnDestroy
        _healthChangedSubscription = _eventBus.Subscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
    }

    /// <summary>
    /// Updates the displayed health value for a given player.
    /// </summary>
    /// <param name="health">The health value to display.</param>
    /// <param name="isP1">Indicates if the health update is for Player 1. Otherwise, it's for Player 2.</param>
    private void SetHealthText(float health, bool isP1)
    {
        if (isP1)
        {
            healthP1Text.SetText($"{health} / {PlayerStatus.MaxHealth}");
        }
        else
        {
            healthP2Text.SetText($"{health} / {PlayerStatus.MaxHealth}");
        }
    }

    /// <summary>
    /// Handles the PlayerHealthChangedEvent from EventBus.
    /// Phase 21-22: Updated to use domain event instead of static UnityEvent.
    /// </summary>
    /// <param name="evt">The PlayerHealthChangedEvent containing health update data.</param>
    private void OnPlayerHealthChanged(PlayerHealthChangedEvent evt)
    {
        bool isP1 = (evt.Player == JDG.Domain.CardOwner.Player1);
        SetHealthText(evt.NewHealth, isP1);
    }

    /// <summary>
    /// Unregisters the event listener when the object is destroyed.
    /// Phase 144: Fixed - subscriptions must be manually disposed.
    /// </summary>
    private void OnDestroy()
    {
        _healthChangedSubscription?.Dispose();
    }
}
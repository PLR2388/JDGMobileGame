using System;
using JDG.Application;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Infrastructure.DI;
using UnityEngine.Events;

public enum Phase
{
    Draw,
    Choose,
    Attack,
    End,
    GameOver
}

/// <summary>
/// [OBSOLETE] Legacy game state manager using singleton pattern.
/// Use GameStateService with dependency injection instead.
/// GameStateService provides the same functionality with proper DI and EventBus integration.
///
/// Migration guide:
/// - GameStateManager.Instance.IsP1Turn → gameStateService.CurrentPlayer == PlayerId.Player1
/// - GameStateManager.Instance.Phase → gameStateService.CurrentPhase (note: JDG.Domain.Phase enum)
/// - GameStateManager.Instance.NumberOfTurn → gameStateService.TurnNumber
/// - GameStateManager.ChangePlayer event → Subscribe to PlayerTurnChangedEvent via EventBus
/// </summary>
[System.Obsolete("Use GameStateService with dependency injection instead. This singleton will be removed in Phase 2 cleanup.")]
public class GameStateManager : Singleton<GameStateManager>
{
    public bool IsP1Turn => isP1Turn;
    public Phase Phase => phase;
    public int NumberOfTurn => numberOfTurn;

    private bool isP1Turn = true;
    private Phase phase = Phase.Draw;
    private int numberOfTurn = 0;

    /// <summary>
    /// Toggles the current player's turn and publishes to EventBus.
    /// Phase 23: Static UnityEvent removed, only EventBus used.
    /// </summary>
    private void ToggleTurn()
    {
        isP1Turn = !isP1Turn;

        // Phase 23: Publish to EventBus
        var eventBus = ServiceLocator.Get<IEventBus>();
        eventBus.Publish(new PlayerTurnChangedEvent
        {
            NewPlayer = isP1Turn ? PlayerId.Player1 : PlayerId.Player2,
            TurnNumber = numberOfTurn
        });
    }

    /// <summary>
    /// Sets the current phase to a specified value.
    /// </summary>
    /// <param name="newPhaseId">The new phase to set.</param>
    public void SetPhase(Phase newPhaseId)
    {
        phase = newPhaseId;
    }

    /// <summary>
    /// Transitions to the next phase in the sequence. If the current phase is GameOver, no change occurs.
    /// </summary>
    public void NextPhase()
    {
        if (phase != Phase.GameOver)
        {
            phase = (Phase)(((int)phase + 1) % 4);
        }
    }

    /// <summary>
    /// Increments the turn counter by one.
    /// </summary>
    public void IncrementNumberOfTurn()
    {
        numberOfTurn++;
    }

    /// <summary>
    /// Handles the end of a turn by toggling the current player and resetting the phase to Draw.
    /// </summary>
    public void HandleEndTurn()
    {
        ToggleTurn();
        SetPhase(Phase.Draw);
    }
}
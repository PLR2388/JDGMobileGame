using System;
using UnityEngine.Events;

public enum Phase
{
    Draw,
    Choose,
    Attack,
    End,
    GameOver
}

public class GameStateManager : Singleton<GameStateManager>
{
    public bool IsP1Turn => isP1Turn;
    public Phase Phase => phase;
    public int NumberOfTurn => numberOfTurn;

    private bool isP1Turn = true;
    private Phase phase = Phase.Draw;
    private int numberOfTurn = 0;
    
    public static readonly UnityEvent ChangePlayer = new UnityEvent();
    
    /// <summary>
    /// Toggles the current player's turn and invokes a change player event.
    /// </summary>
    private void ToggleTurn()
    {
        isP1Turn = !isP1Turn;
        ChangePlayer.Invoke();
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
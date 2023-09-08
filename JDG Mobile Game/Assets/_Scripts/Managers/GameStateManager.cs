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

    protected override void Awake()
    {
        base.Awake();
        phase = 0;
        isP1Turn = true;
        numberOfTurn = 0;
    }

    private void ToggleTurn()
    {
        isP1Turn = !isP1Turn;
        ChangePlayer.Invoke();
    }

    public void SetPhase(Phase newPhaseId)
    {
        phase = newPhaseId;
    }

    public void NextPhase()
    {
        switch (phase)
        {
            case Phase.Draw:
                phase = Phase.Choose;
                break;
            case Phase.Choose:
                phase = Phase.Attack;
                break;
            case Phase.Attack:
                phase = Phase.End;
                break;
            case Phase.End:
                phase = Phase.Draw;
                break;
            case Phase.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void IncrementNumberOfTurn()
    {
        numberOfTurn++;
    }

    public void HandleEndTurn()
    {
        ToggleTurn();
        SetPhase(0);
    }
}
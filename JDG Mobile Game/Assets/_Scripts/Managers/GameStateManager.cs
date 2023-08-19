public class GameStateManager : Singleton<GameStateManager>
{
    public bool IsP1Turn => isP1Turn;
    public int PhaseId => phaseId;
    public int NumberOfTurn => numberOfTurn;

    private bool isP1Turn = true;
    private int phaseId = 0;
    private int numberOfTurn = 0;

    protected override void Awake()
    {
        base.Awake();
        phaseId = 0;
        isP1Turn = true;
        numberOfTurn = 0;
    }

    public void ToggleTurn()
    {
        isP1Turn = !isP1Turn;
    }

    public void SetPhase(int newPhaseId)
    {
        phaseId = newPhaseId;
    }

    public void IncrementNumberOfTurn()
    {
        numberOfTurn++;
    }
}
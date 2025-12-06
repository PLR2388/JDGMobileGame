using JDG.Application.Services;

/// <summary>
/// Adapter service that bridges IRoundDisplayService to RoundDisplayManager.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Phase 19-20: Now injects RoundDisplayManager instead of using .Instance.
/// </summary>
public class RoundDisplayService : IRoundDisplayService
{
    private readonly RoundDisplayManager _roundDisplayManager;

    public RoundDisplayService(RoundDisplayManager roundDisplayManager)
    {
        _roundDisplayManager = roundDisplayManager;
    }

    public void SetRoundText(string value)
    {
        _roundDisplayManager.SetRoundText(value);
    }

    public void AdaptUIToPhaseIdInNextRound(bool rotate)
    {
        _roundDisplayManager.AdaptUIToPhaseIdInNextRound(rotate);
    }
}

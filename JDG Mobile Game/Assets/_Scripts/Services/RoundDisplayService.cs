using JDG.Application.Services;

/// <summary>
/// Adapter service that bridges IRoundDisplayService to RoundDisplayManager singleton.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Will be removed once RoundDisplayManager is refactored into a presenter.
/// </summary>
public class RoundDisplayService : IRoundDisplayService
{
    public void SetRoundText(string value)
    {
        RoundDisplayManager.Instance.SetRoundText(value);
    }

    public void AdaptUIToPhaseIdInNextRound(bool rotate)
    {
        RoundDisplayManager.Instance.AdaptUIToPhaseIdInNextRound(rotate);
    }
}

using JDG.Application.Services;
using UnityEngine;

/// <summary>
/// Adapter service that bridges IRoundDisplayService to RoundDisplayManager.
/// Phase 9: Temporary bridge during migration from singleton to DI.
/// Phase 19-20: Now injects RoundDisplayManager instead of using .Instance.
/// Phase 135: Added null checks for defensive programming.
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
        // Phase 135: Add null check for defensive programming
        if (_roundDisplayManager == null)
        {
            Debug.LogError("RoundDisplayService: RoundDisplayManager is null! Cannot set round text.");
            return;
        }
        _roundDisplayManager.SetRoundText(value);
    }

    public void AdaptUIToPhaseIdInNextRound(bool rotate)
    {
        // Phase 135: Add null check for defensive programming
        if (_roundDisplayManager == null)
        {
            Debug.LogError("RoundDisplayService: RoundDisplayManager is null! Cannot adapt UI.");
            return;
        }
        _roundDisplayManager.AdaptUIToPhaseIdInNextRound(rotate);
    }
}

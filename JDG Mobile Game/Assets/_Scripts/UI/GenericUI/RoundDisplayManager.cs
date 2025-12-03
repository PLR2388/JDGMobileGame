using JDG.Domain.ValueObjects;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Services;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the round display, including round text, player indicators, and camera orientation.
/// </summary>
public class RoundDisplayManager : StaticInstance<RoundDisplayManager>
{
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] protected GameObject inHandButton;

    private readonly Vector3 cameraRotation = new Vector3(0, 0, 180);

    // Phase 2: Temporary bridge to GameStateService during migration
    // This will be removed when RoundDisplayManager is replaced by Presenter in Phase 5
    private GameStateService GameStateService => ServiceLocator.Get<GameStateService>();
    private bool IsP1Turn => GameStateService.CurrentPlayer == PlayerId.Player1;
    private Phase CurrentPhase => (Phase)(int)GameStateService.CurrentPhase; // Cast from JDG.Domain.Phase to global Phase

    /// <summary>
    /// Sets the displayed round text.
    /// </summary>
    /// <param name="value">The text value to set.</param>
    public void SetRoundText(string value)
    {
        if (roundText)
            roundText.text = value;
    }

    /// <summary>
    /// Updates the UI elements based on the game phase in the next round.
    /// </summary>
    public void AdaptUIToPhaseIdInNextRound(bool rotate)
    {
        var phaseId = CurrentPhase;
        switch (phaseId)
        {
            case Phase.End:
                inHandButton.SetActive(true);
                SetRoundText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PHASE_DRAW));
                if (rotate)
                {
                    RotateCameraForEndPhase();   
                }
                SetPlayerTurnText();
                break;
            case Phase.Attack:
                inHandButton.SetActive(false);
                SetRoundText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PHASE_ATTACK));
                break;
            // Optionally, add more phases as needed.
        }
    }

    /// <summary>
    /// Rotates the camera for the end phase of the game.
    /// </summary>
    private void RotateCameraForEndPhase()
    {
        if (playerCamera)
            playerCamera.transform.Rotate(cameraRotation);
    }

    /// <summary>
    /// Sets the text indicating whose turn it is.
    /// </summary>
    private void SetPlayerTurnText()
    {
        if (playerText)
        {
            playerText.text = IsP1Turn
                ? LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PLAYER_TWO)
                : LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PLAYER_ONE);
        }
    }
}
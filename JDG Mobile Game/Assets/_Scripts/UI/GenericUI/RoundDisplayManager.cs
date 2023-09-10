using TMPro;
using UnityEngine;

public class RoundDisplayManager : StaticInstance<RoundDisplayManager>
{
    [SerializeField] private GameObject playerText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] protected GameObject inHandButton;

    private readonly Vector3 cameraRotation = new Vector3(0, 0, 180);

    protected override void Awake()
    {
        base.Awake();
        // Ensure roundText is assigned
        if(!roundText && playerText)
            roundText = playerText.GetComponent<TextMeshProUGUI>();
    }

    public void SetRoundText(string value)
    {
        if (roundText)
            roundText.text = value;
    }

    public void AdaptUIToPhaseIdInNextRound()
    {
        var phaseId = GameStateManager.Instance.Phase;
        switch (phaseId)
        {
            case Phase.End:
                inHandButton.SetActive(true);
                SetRoundText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PHASE_DRAW));
                RotateCameraForEndPhase();
                SetPlayerTurnText();
                break;
            case Phase.Attack:
                inHandButton.SetActive(false);
                SetRoundText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PHASE_ATTACK));
                break;
            // Optionally, add more phases as needed.
        }
    }

    private void RotateCameraForEndPhase()
    {
        if (playerCamera)
            playerCamera.transform.Rotate(cameraRotation);
    }

    private void SetPlayerTurnText()
    {
        if (playerText && roundText)
        {
            roundText.text = GameStateManager.Instance.IsP1Turn 
                ? LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PLAYER_TWO) 
                : LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PLAYER_ONE);
        }
    }
}
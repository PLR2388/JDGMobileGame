using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    private readonly Vector3 cameraRotation = new Vector3(0, 0, 180);

    [SerializeField] protected GameObject playerText;
    [SerializeField] protected GameObject roundText;
    [SerializeField] protected TextMeshProUGUI healthP1Text;
    [SerializeField] protected TextMeshProUGUI healthP2Text;

    [SerializeField] protected GameObject bigImageCard;
    [SerializeField] protected GameObject invocationMenu;
    [SerializeField] protected GameObject nextPhaseButton;
    [SerializeField] protected Transform canvas;

    [SerializeField] protected GameObject inHandButton;

    [SerializeField] private GameObject playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        ChangeHealthText(PlayerStatus.MaxPv, true);
        ChangeHealthText(PlayerStatus.MaxPv, false);
        PlayerStatus.ChangePvEvent.AddListener(ChangeHealthText);
    }

    protected void ChangeHealthText(float pv, bool isP1)
    {
        if (isP1)
        {
            healthP1Text.SetText(
                $"{pv} / {PlayerStatus.MaxPv}"
            );
        }
        else
        {
            healthP2Text.SetText(
                $"{pv} / {PlayerStatus.MaxPv}"
            );
        }
    }

    public void SetRoundText(string value)
    {
        roundText.GetComponent<TextMeshProUGUI>().text = value;
    }

    public void DisplayCardInBigImage(InGameCard card)
    {
        bigImageCard.SetActive(true);
        bigImageCard.GetComponent<Image>().material = card.MaterialCard;
    }

    public void UpdateAttackButton()
    {
        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = CardManager.Instance.CanAttackerAttack();
    }

    public void DisplayInvocationMenu(bool isAttackPhase)
    {
        var mousePosition = InputManager.TouchPosition;
        invocationMenu.SetActive(true);
        var attackButton = invocationMenu.transform.GetChild(0);
        var actionButton = invocationMenu.transform.GetChild(1);
        attackButton.gameObject.SetActive(isAttackPhase);
        attackButton.GetComponent<Button>().interactable = CardManager.Instance.CanAttackerAttack();
        actionButton.gameObject.SetActive(CardManager.Instance.HasAttackerAction() && !isAttackPhase);
        actionButton.GetComponent<Button>().interactable =
            CardManager.Instance.IsSpecialActionPossible();
        invocationMenu.transform.position = mousePosition;
    }

    public void DisplayOpponentAvailableMessageBox(
        List<InGameCard> invocationCards,
        UnityAction<InGameInvocationCard> positiveAction,
        UnityAction negativeAction)
    {
        nextPhaseButton.SetActive(false);

        if (invocationCards.Count > 0)
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOOSE_OPPONENT),
                invocationCards,
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: (invocationCard) =>
                {
                    positiveAction(invocationCard as InGameInvocationCard);
                    nextPhaseButton.SetActive(true);
                },
                negativeAction: () =>
                {
                    negativeAction();
                    nextPhaseButton.SetActive(true);
                }
            );
            CardSelector.Instance.CreateCardSelection(
                canvas,
                config
            );
        }
        else
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_CANNOT_ATTACK_MESSAGE),
                showOkButton: true
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }

    public void HideInvocationMenu()
    {
        invocationMenu.SetActive(false);
    }

    public void AdaptUIToPhaseIdInNextRound()
    {
        var phaseId = GameStateManager.Instance.Phase;
        switch (phaseId)
        {
            case Phase.End:
                inHandButton.SetActive(true);
                SetRoundText(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PHASE_DRAW)
                );
                playerCamera.transform.Rotate(cameraRotation);
                playerText.GetComponent<TextMeshProUGUI>().text = GameStateManager.Instance.IsP1Turn
                    ? LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PLAYER_ONE)
                    : LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PLAYER_TWO);
                break;
            case Phase.Attack:
                inHandButton.SetActive(false);
                SetRoundText(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PHASE_ATTACK)
                );
                break;
        }
    }

    public void EnableInvocationMenu()
    {
        invocationMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
    }

    public void HideBigImage()
    {
        bigImageCard.SetActive(false);
    }

    public void DisplayPauseMenu(UnityAction onPositiveAction)
    {
        MessageBoxConfig config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PAUSE_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PAUSE_MESSAGE),
            showPositiveButton: true,
            showNegativeButton: true,
            positiveAction: onPositiveAction
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }
}
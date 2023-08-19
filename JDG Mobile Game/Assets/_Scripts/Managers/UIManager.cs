using System.Collections;
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
            healthP1Text.SetText(pv + "/" + PlayerStatus.MaxPv);
        }
        else
        {
            healthP2Text.SetText(pv + "/" + PlayerStatus.MaxPv);
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

    public void UpdateAttackButton(bool isP1Turn)
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
            var message =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis ton adversaire :", invocationCards);
            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var invocationCard =
                    (InGameInvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                positiveAction(invocationCard);
                nextPhaseButton.SetActive(true);
                Destroy(message);
            };
            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                negativeAction();
                nextPhaseButton.SetActive(true);
                Destroy(message);
            };
        }
        else
        {
            MessageBox.CreateOkMessageBox(canvas, "Attention",
                "Tu ne peux pas attaquer le joueur ni ses invocations");
        }
    }

    public void HideInvocationMenu()
    {
        invocationMenu.SetActive(false);
    }

    public void AdaptUIToPhaseIdInNextRound(Phase phaseId)
    {
        switch (phaseId)
        {
            case Phase.End:
                inHandButton.SetActive(true);
                SetRoundText("Phase de pioche");
                playerCamera.transform.Rotate(cameraRotation);
                playerText.GetComponent<TextMeshProUGUI>().text = GameStateManager.Instance.IsP1Turn ? "Joueur 1" : "Joueur 2";
                break;
            case Phase.Choose:
                inHandButton.SetActive(true);
                SetRoundText("Phase de pose");
                break;
            case Phase.Attack:
                inHandButton.SetActive(false);
                SetRoundText("Phase d'attaque");
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
        MessageBox.CreateSimpleMessageBox(canvas, "Pause", "Veux-tu quitter la partie ?", onPositiveAction);
    }
}
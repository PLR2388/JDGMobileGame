using System;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class TutoInGameMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject buttonText;
    [SerializeField] private GameObject handScreen;
    [SerializeField] private GameObject miniMenuCard;
    [SerializeField] private GameObject detailCardPanel;
    [SerializeField] private GameObject detailButtonText;
    [SerializeField] private GameObject putCardButtonText;
    [SerializeField] private GameObject putCardButton;
    [SerializeField] private GameObject inHandButton;

    [FormerlySerializedAs("backgroundInformations")] [SerializeField]
    private GameObject backgroundInformation;

    [SerializeField] private InGameCard currentSelectedCard;
    [SerializeField] private GameObject invocationMenu;

    [SerializeField] private GameObject gameLoop;

    public static readonly CardEvent EventClick = new CardEvent();
    public static readonly InvocationCardEvent InvocationCardEvent = new InvocationCardEvent();
    public static readonly FieldCardEvent FieldCardEvent = new FieldCardEvent();
    public static readonly EffectCardEvent EffectCardEvent = new EffectCardEvent();
    public static readonly EquipmentCardEvent EquipmentCardEvent = new EquipmentCardEvent();


    private readonly Vector3 buttonGroupPosition = new Vector3(-40, 40, 0);
    private readonly Vector3 padding = new Vector3(490, -350, 0);

    private int currentDialogIndex = 0;

    private void Awake()
    {
        DialogueUI.DialogIndex.AddListener(SavedIndexDialog);
    }

    private void SavedIndexDialog(int index)
    {
        currentDialogIndex = index;
    }

    private void Start()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        EventClick.AddListener(ClickOnCard);
    }

    private void ClickOnCard(InGameCard card)
    {
        currentSelectedCard = card;
        var cardType = currentSelectedCard.Type;
        var playerCard = GameStateManager.Instance.IsP1Turn
            ? GameObject.Find("Player1").GetComponent<PlayerCards>()
            : GameObject.Find("Player2").GetComponent<PlayerCards>();

        var authorizedCard = currentDialogIndex > 36 ? "Musique de Mega Drive" : "Cliché Raciste";

        if (card.Title == authorizedCard)
        {
            switch (cardType)
            {
                case CardType.Invocation:
                    putCardButtonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD);
                    var invocationCard = card as InGameInvocationCard;
                    putCardButton.GetComponent<Button>().interactable =
                        invocationCard?.IsInvocationPossible() == true && playerCard.InvocationCards.Count < 4;

                    break;
                case CardType.Contre:
                    putCardButtonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_CONTRE);
                    putCardButton.GetComponent<Button>().interactable = true;
                    break;
                case CardType.Effect:
                    putCardButtonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD);
                    break;
                case CardType.Equipment:
                    putCardButtonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_EQUIP_INVOCATION);
                    putCardButton.GetComponent<Button>().interactable = true;
                    break;
                case CardType.Field:
                    putCardButtonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD);
                    putCardButton.GetComponent<Button>().interactable = playerCard.FieldCard == null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#if UNITY_EDITOR
            var mousePosition = Input.mousePosition;
            DisplayMiniMenuCardAtPosition(mousePosition);
#elif UNITY_ANDROID
        var touch = Input.GetTouch(0);
        DisplayMiniMenuCardAtPosition(touch.position);
#endif
        }
    }

    private void DisplayMiniMenuCardAtPosition(Vector3 mousePosition)
    {
        putCardButton.GetComponent<HighLightButton>().isActivated = true;
        if (miniMenuCard.activeSelf)
        {
            miniMenuCard.transform.position = mousePosition + padding;
        }
        else
        {
            miniMenuCard.SetActive(true);
            miniMenuCard.transform.position = mousePosition + padding;
        }
    }

    public void ClickPutCard()
    {
        switch (currentSelectedCard.Type)
        {
            case CardType.Invocation:
            {
                var invocationCard = currentSelectedCard as InGameInvocationCard;
                InvocationCardEvent.Invoke(invocationCard);
                break;
            }
            case CardType.Field:
            {
                var fieldCard = currentSelectedCard as InGameFieldCard;
                FieldCardEvent.Invoke(fieldCard);
                break;
            }
            case CardType.Effect:
            {
                var effectCard = currentSelectedCard as InGameEffectCard;
                EffectCardEvent.Invoke(effectCard);
                break;
            }
            case CardType.Equipment:
            {
                var equipmentCard = currentSelectedCard as InGameEquipementCard;
                EquipmentCardEvent.Invoke(equipmentCard);
                break;
            }
        }

        putCardButton.GetComponent<HighLightButton>().isActivated = false;
        miniMenuCard.SetActive(false);
        inHandButton.SetActive(true);
        inHandButton.GetComponent<HighLightButton>().isActivated = true;
        if (!detailCardPanel.activeSelf) return;
        // The detail panel is visible. One must go back to card display
        detailCardPanel.SetActive(false);
        handScreen.SetActive(true);
    }

    public void DetailCardClick()
    {
        if (detailCardPanel.activeSelf)
        {
            detailButtonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_DETAILS);
            miniMenuCard.SetActive(false);
            detailCardPanel.SetActive(false);
            handScreen.SetActive(true);
            inHandButton.SetActive(true);
        }
        else
        {
            handScreen.SetActive(false);

            miniMenuCard.transform.position = buttonGroupPosition + new Vector3(640, 360);

            detailButtonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_BACK);
            detailCardPanel.transform.GetChild(0).gameObject.GetComponent<CardDisplay>().InGameCard = currentSelectedCard;
            detailCardPanel.SetActive(true);
            inHandButton.SetActive(false);
        }
    }

    public void ClickHandCard()
    {
        invocationMenu.SetActive(false);
        if (handScreen.activeSelf)
        {
            HideHand();
            inHandButton.GetComponent<Button>().interactable = false;
            inHandButton.GetComponent<HighLightButton>().isActivated = false;
            if (currentDialogIndex == 38)
            {
                DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.PutEffectCard);
            }
        }
        else
        {
            DisplayHand();
            inHandButton.GetComponent<Button>().interactable = false;
            inHandButton.GetComponent<HighLightButton>().isActivated = false;
        }
    }

    private void DisplayHand()
    {
        handScreen.SetActive(true);
        backgroundInformation.SetActive(false);
        buttonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_BACK);
    }

    private void HideHand()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        handScreen.SetActive(false);
        backgroundInformation.SetActive(true);
        buttonText.GetComponent<TextMeshProUGUI>().SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_HAND));
    }
}
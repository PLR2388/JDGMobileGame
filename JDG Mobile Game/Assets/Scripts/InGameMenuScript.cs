using System;
using Cards;
using Cards.EffectCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class CardEvent : UnityEvent<InGameCard>
{
}

[Serializable]
public class InvocationCardEvent : UnityEvent<InGameInvocationCard>
{
}

[Serializable]
public class FieldCardEvent : UnityEvent<InGameFieldCard>
{
}

[Serializable]
public class EffectCardEvent : UnityEvent<InGameEffectCard>
{
}


[Serializable]
public class EquipmentCardEvent : UnityEvent<InGameEquipementCard>
{
}

public class InGameMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject buttonText;
    [SerializeField] private GameObject handScreen;
    [SerializeField] private GameObject miniMenuCard;
    [SerializeField] private GameObject detailCardPanel;
    [SerializeField] private GameObject detailButtonText;
    [SerializeField] private GameObject putCardButtonText;
    [SerializeField] private GameObject putCardButton;
    [SerializeField] private GameObject inHandButton;

    [SerializeField]
    private GameObject backgroundInformation;

    private InGameCard currentSelectedCard;
    [SerializeField] private GameObject invocationMenu;

    [SerializeField] private GameObject gameLoop;

    public static readonly CardEvent EventClick = new CardEvent();
    public static readonly InvocationCardEvent InvocationCardEvent = new InvocationCardEvent();
    public static readonly FieldCardEvent FieldCardEvent = new FieldCardEvent();
    public static readonly EffectCardEvent EffectCardEvent = new EffectCardEvent();
    public static readonly EquipmentCardEvent EquipmentCardEvent = new EquipmentCardEvent();


    private readonly Vector3 buttonGroupPosition = new Vector3(-40, 40, 0);
    private readonly Vector3 padding = new Vector3(490, -350, 0);

    private EffectFunctions effectFunctions;

    private void Start()
    {
        effectFunctions = gameLoop.GetComponent<EffectFunctions>();
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        EventClick.AddListener(ClickOnCard);
    }

    private void ClickOnCard(InGameCard card)
    {
        currentSelectedCard = card;
        var cardType = currentSelectedCard.Type;
        var playerCard = GameLoop.IsP1Turn
            ? GameObject.Find("Player1").GetComponent<PlayerCards>()
            : GameObject.Find("Player2").GetComponent<PlayerCards>();
        switch (cardType)
        {
            case CardType.Invocation:
                putCardButtonText.GetComponent<TextMeshProUGUI>().text = "Poser la carte";
                var invocationCard = (InGameInvocationCard)card;
                putCardButton.GetComponent<Button>().interactable =
                    invocationCard.IsInvocationPossible() && playerCard.invocationCards.Count < 4;

                break;
            case CardType.Contre:
                putCardButtonText.GetComponent<TextMeshProUGUI>().text = "Contrer";
                putCardButton.GetComponent<Button>().interactable = true;
                break;
            case CardType.Effect:
                var effectCard = (InGameEffectCard)card;
                putCardButtonText.GetComponent<TextMeshProUGUI>().text = "Poser la carte";
                putCardButton.GetComponent<Button>().interactable =
                    effectFunctions.CanUseEffectCard(effectCard.GetEffectCardEffect());
                break;
            case CardType.Equipment:
                putCardButtonText.GetComponent<TextMeshProUGUI>().text = "Équiper une invocation";
                var equipmentCard = (InGameEquipementCard)card;
                putCardButton.GetComponent<Button>().interactable =
                    InGameEquipementCard.IsEquipmentPossible(equipmentCard.EquipmentInstantEffect);
                break;
            case CardType.Field:
                putCardButtonText.GetComponent<TextMeshProUGUI>().text = "Poser la carte";
                putCardButton.GetComponent<Button>().interactable = playerCard.field == null;
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

    private void DisplayMiniMenuCardAtPosition(Vector3 mousePosition)
    {
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
                var invocationCard = (InGameInvocationCard)currentSelectedCard;
                InvocationCardEvent.Invoke(invocationCard);
                break;
            }
            case CardType.Field:
            {
                var fieldCard = (InGameFieldCard)currentSelectedCard;
                FieldCardEvent.Invoke(fieldCard);
                break;
            }
            case CardType.Effect:
            {
                var effectCard = (InGameEffectCard)currentSelectedCard;
                EffectCardEvent.Invoke(effectCard);
                break;
            }
            case CardType.Equipment:
            {
                var equipmentCard = (InGameEquipementCard)currentSelectedCard;
                EquipmentCardEvent.Invoke(equipmentCard);
                break;
            }
        }

        miniMenuCard.SetActive(false);
        if (!detailCardPanel.activeSelf) return;
        // The detail panel is visible. One must go back to card display
        detailCardPanel.SetActive(false);
        handScreen.SetActive(true);
        inHandButton.SetActive(true);
    }

    public void DetailCardClick()
    {
        if (detailCardPanel.activeSelf)
        {
            detailButtonText.GetComponent<TextMeshProUGUI>().text = "Détails";
            miniMenuCard.SetActive(false);
            detailCardPanel.SetActive(false);
            handScreen.SetActive(true);
            inHandButton.SetActive(true);
        }
        else
        {
            handScreen.SetActive(false);

            miniMenuCard.transform.position = buttonGroupPosition + new Vector3(640, 360);

            detailButtonText.GetComponent<TextMeshProUGUI>().text = "Retour";
            detailCardPanel.transform.GetChild(0).gameObject.GetComponent<CardDisplay>().card = currentSelectedCard.baseCard;
            detailCardPanel.transform.GetChild(0).gameObject.GetComponent<CardDisplay>().inGameCard = currentSelectedCard;
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
        }
        else
        {
            DisplayHand();
        }
    }

    private void DisplayHand()
    {
        handScreen.SetActive(true);
        backgroundInformation.SetActive(false);
        buttonText.GetComponent<TextMeshProUGUI>().text = "Retour";
    }

    private void HideHand()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        handScreen.SetActive(false);
        backgroundInformation.SetActive(true);
        buttonText.GetComponent<TextMeshProUGUI>().SetText("Cartes en main");
    }
}
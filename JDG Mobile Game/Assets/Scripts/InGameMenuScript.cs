using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class CardEvent : UnityEvent<Card>
{
}

[System.Serializable]
public class InvocationCardEvent : UnityEvent<InvocationCard>
{
}

[System.Serializable]
public class FieldCardEvent : UnityEvent<FieldCard>
{
}

[System.Serializable]
public class EffectCardEvent : UnityEvent<EffectCard>
{
}


[System.Serializable]
public class EquipmentCardEvent : UnityEvent<EquipmentCard>
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

    [FormerlySerializedAs("backgroundInformations")] [SerializeField]
    private GameObject backgroundInformation;

    [SerializeField] private Card currentSelectedCard;
    [SerializeField] private GameObject invocationMenu;

    public static readonly CardEvent EventClick = new CardEvent();
    public static readonly InvocationCardEvent InvocationCardEvent = new InvocationCardEvent();
    public static readonly FieldCardEvent FieldCardEvent = new FieldCardEvent();
    public static readonly EffectCardEvent EffectCardEvent = new EffectCardEvent();
    public static readonly EquipmentCardEvent EquipmentCardEvent = new EquipmentCardEvent();


    private readonly Vector3 buttonGroupPosition = new Vector3(-40, 40, 0);
    private readonly Vector3 padding = new Vector3(490, -350, 0);

    private void Start()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        EventClick.AddListener(ClickOnCard);
    }

    private void ClickOnCard(Card card)
    {
        currentSelectedCard = card;
        var cardType = currentSelectedCard.Type;
        switch (cardType)
        {
            case CardType.Invocation:
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Poser la carte";
                var invocationCard = (InvocationCard) card;
                var playerCard = GameLoop.IsP1Turn
                    ? GameObject.Find("Player1").GetComponent<PlayerCards>()
                    : GameObject.Find("Player2").GetComponent<PlayerCards>();
                putCardButton.GetComponent<Button>().interactable = invocationCard.IsInvocationPossible() && playerCard.invocationCards.Count < 4;

                break;
            case CardType.Contre:
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Contrer";
                break;
            case CardType.Effect:
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Poser la carte";
                break;
            case CardType.Equipment:
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Équiper une invocation";
                var equipmentCard = (EquipmentCard) card;
                putCardButton.GetComponent<Button>().interactable = equipmentCard.IsEquipmentPossible();

                break;
            case CardType.Field:
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Poser la carte";
                break;
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
                var invocationCard = (InvocationCard) currentSelectedCard;
                InvocationCardEvent.Invoke(invocationCard);
                break;
            }
            case CardType.Field:
            {
                var fieldCard = (FieldCard) currentSelectedCard;
                FieldCardEvent.Invoke(fieldCard);
                break;
            }
            case CardType.Effect:
            {
                var effectCard = (EffectCard) currentSelectedCard;
                EffectCardEvent.Invoke(effectCard);
                break;
            }
            case CardType.Equipment:
            {
                var equipmentCard = (EquipmentCard) currentSelectedCard;
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
            detailButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Détails";
            miniMenuCard.SetActive(false);
            detailCardPanel.SetActive(false);
            handScreen.SetActive(true);
            inHandButton.SetActive(true);
        }
        else
        {
            handScreen.SetActive(false);

            miniMenuCard.transform.position = buttonGroupPosition + new Vector3(640, 360);

            detailButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Retour";
            detailCardPanel.transform.GetChild(0).gameObject.GetComponent<CardDisplay>().card = currentSelectedCard;
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
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
    [SerializeField] private GameObject messageBox;
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
#if UNITY_EDITOR
        var mousePosition = Input.mousePosition;

        var cardType = currentSelectedCard.Type;
        switch (cardType)
        {
            case "invocation":
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Poser la carte";
                var invocationCard = (InvocationCard) card;
                putCardButton.GetComponent<Button>().interactable = invocationCard.IsInvocationPossible();

                break;
            case "contre":
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Contrer";
                break;
            case "effect":
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Poser la carte";
                break;
            case "equipment":
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Équiper une invocation";
                var equipmentCard = (EquipmentCard) card;
                putCardButton.GetComponent<Button>().interactable = equipmentCard.IsEquipmentPossible();

                break;
            case "field":
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Poser la carte";
                break;
        }

        if (miniMenuCard.activeSelf)
        {
            miniMenuCard.transform.position = mousePosition + padding;
        }
        else
        {
            miniMenuCard.SetActive(true);
            miniMenuCard.transform.position = mousePosition + padding;
        }
#endif
    }

    public void ClickPutCard()
    {
        switch (currentSelectedCard.Type)
        {
            case "invocation":
            {
                var invocationCard = (InvocationCard) currentSelectedCard;
                InvocationCardEvent.Invoke(invocationCard);
                break;
            }
            case "field":
            {
                var fieldCard = (FieldCard) currentSelectedCard;
                FieldCardEvent.Invoke(fieldCard);
                break;
            }
            case "effect":
            {
                var effectCard = (EffectCard) currentSelectedCard;
                EffectCardEvent.Invoke(effectCard);
                break;
            }
            case "equipment":
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
        buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Retour";
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
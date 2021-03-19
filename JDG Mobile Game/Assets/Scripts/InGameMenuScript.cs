using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR;

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
    [SerializeField] private GameObject backgroundInformations;
    [SerializeField] private Card currentSelectedCard;
    [SerializeField] private GameObject messageBox;
    
    public static CardEvent EventClick = new CardEvent();
    public static InvocationCardEvent InvocationCardEvent = new InvocationCardEvent();
    public static FieldCardEvent FieldCardEvent = new FieldCardEvent();
    public static EffectCardEvent EffectCardEvent = new EffectCardEvent();
    public static EquipmentCardEvent EquipmentCardEvent = new EquipmentCardEvent();


    private Vector3 buttonGroupPosition = new Vector3(-40, 40, 0);
    private Vector3 padding = new Vector3(490,-350,0);
 private void Start()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        EventClick.AddListener(ClickOnCard);
    }

    public void ClickOnCard(Card card)
    {
        currentSelectedCard = card;
#if UNITY_EDITOR
        Vector3 mousePosition = Input.mousePosition;

        string cardType = currentSelectedCard.GetType();
        switch (cardType)
        {
            case "invocation":
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Poser la carte";
                InvocationCard invocationCard = (InvocationCard) card;
                if (invocationCard.isInvocationPossible())
                {
                    putCardButton.GetComponent<Button>().interactable = true;
                }
                else
                {
                    putCardButton.GetComponent<Button>().interactable = false;
                }
                break;
            case "contre" :
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Contrer";
                break;
            case "effect" :
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Poser la carte";
                break;
            case "equipment" :
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Équiper une invocation";
                EquipmentCard equipmentCard = (EquipmentCard) card;
                if (equipmentCard.isEquipmentPossible())
                {
                    putCardButton.GetComponent<Button>().interactable = true;
                }
                else
                {
                    putCardButton.GetComponent<Button>().interactable = false;
                }
                break;
            case "field" : 
                putCardButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Poser la carte";
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
        if (currentSelectedCard.GetType() == "invocation")
        {
            InvocationCard invocationCard = (InvocationCard)currentSelectedCard;
            InvocationCardEvent.Invoke(invocationCard);
        } else if (currentSelectedCard.GetType() == "field")
        {
            FieldCard fieldCard = (FieldCard) currentSelectedCard;
            FieldCardEvent.Invoke(fieldCard);
        } else if (currentSelectedCard.GetType() == "effect")
        {
            EffectCard effectCard = (EffectCard) currentSelectedCard;
            EffectCardEvent.Invoke(effectCard);
        } else if (currentSelectedCard.GetType() == "equipment")
        {
            EquipmentCard equipmentCard = (EquipmentCard) currentSelectedCard;
            EquipmentCardEvent.Invoke(equipmentCard);
        }
        
        miniMenuCard.SetActive(false);
        if (detailCardPanel.activeSelf)
        {
            // The detail panel is visible. One must go back to card display
            detailCardPanel.SetActive(false);
            handScreen.SetActive(true);
            inHandButton.SetActive(true);
        }
    }

    public void DetailCardClick()
    {
        if (detailCardPanel.activeSelf)
        {
           detailButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Détails";
           miniMenuCard.SetActive(false);
           detailCardPanel.SetActive(false);
           handScreen.SetActive(true);
           inHandButton.SetActive(true);
        }
        else
        {
            handScreen.SetActive(false);

            miniMenuCard.transform.position = buttonGroupPosition + new Vector3(640,360);
            
            detailButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Retour";
            detailCardPanel.transform.GetChild(0).gameObject.GetComponent<CardDisplay>().card = currentSelectedCard;
            detailCardPanel.SetActive(true);
            inHandButton.SetActive(false);
        }

    }

    public void ClickHandCard()
    {
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
        backgroundInformations.SetActive(false);
        buttonText.GetComponent<TMPro.TextMeshProUGUI>().text="Retour";
    }

    private void HideHand()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        handScreen.SetActive(false);
        backgroundInformations.SetActive(true);
        buttonText.GetComponent<TextMeshProUGUI>().SetText("Cartes en main");
    }
}

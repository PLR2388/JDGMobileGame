using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR;

[System.Serializable]
public class CardEvent : UnityEvent<Card>
{
}

public class InGameMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject buttonText;
    [SerializeField] private GameObject handScreen;
    [SerializeField] private GameObject miniMenuCard;
    [SerializeField] private GameObject detailCardPanel;
    [SerializeField] private GameObject detailButtonText;
    [SerializeField] private GameObject inHandButton;
    [SerializeField] private Card currentSelectedCard;
    
    public static CardEvent EventClick = new CardEvent();

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
        buttonText.GetComponent<TMPro.TextMeshProUGUI>().text="Retour";
    }

    private void HideHand()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        handScreen.SetActive(false);
        buttonText.GetComponent<TextMeshProUGUI>().SetText("Cartes en main");
    }
}

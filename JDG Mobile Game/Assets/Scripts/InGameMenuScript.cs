using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class InGameMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject ButtonText;
    [SerializeField] private GameObject HandScreen;

    public void Click()
    {
        if (HandScreen.activeSelf)
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
        HandScreen.SetActive(true);
        ButtonText.GetComponent<TMPro.TextMeshProUGUI>().text="Retour";
    }

    private void HideHand()
    {
        HandScreen.SetActive(false);
        ButtonText.GetComponent<TextMeshProUGUI>().SetText("Cartes en main");
    }
}

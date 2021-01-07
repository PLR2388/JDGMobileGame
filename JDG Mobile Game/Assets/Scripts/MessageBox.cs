using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public UnityAction positiveAction;
    public UnityAction negativeAction;
    public String title;
    public String description;
    public Boolean isInformation = false;
    public DisplayCards displayCardsScript;
    public Boolean displayCards = false;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI positiveButtonText;
    [SerializeField] private TextMeshProUGUI negativeButtonText;
    [SerializeField] private TextMeshProUGUI okButtonText;
    [SerializeField] private GameObject positiveButton;
    [SerializeField] private GameObject negativeButton;
    [SerializeField] private GameObject okButton;
    
    [SerializeField] private GameObject scrollCardDisplay;

    
    // Start is called before the first frame update
    void Start()
    {
        titleText.text = title;
        descriptionText.text = description;

        if (isInformation)
        {
            okButtonText.text = "Ok";
            positiveButton.SetActive(false);
            negativeButton.SetActive(false);
            okButton.SetActive(true);
        }
        else
        {
            positiveButtonText.text = "Oui";
            negativeButtonText.text = "Non";
            
           
            Button positiveBtn = positiveButton.GetComponent<Button>();
            Button negativeBtn = negativeButton.GetComponent<Button>();
            positiveBtn.onClick.AddListener(positiveAction);
            negativeBtn.onClick.AddListener(negativeAction);
            
            
            positiveButton.SetActive(true);
            negativeButton.SetActive(true);
            okButton.SetActive(false);
        }

        if (displayCards)
        {
            scrollCardDisplay.SetActive(true);
        }
        else
        {
            scrollCardDisplay.SetActive(false);
        }
    }
}

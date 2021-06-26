using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public UnityAction PositiveAction;
    public UnityAction NegativeAction;
    public UnityAction OkAction = null;
    public string title;
    public string description;
    public bool isInformation = false;
    public DisplayCards displayCardsScript;
    public bool displayCards = false;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI positiveButtonText;
    [SerializeField] private TextMeshProUGUI negativeButtonText;
    [SerializeField] private TextMeshProUGUI okButtonText;
    [SerializeField] private GameObject positiveButton;
    [SerializeField] private GameObject negativeButton;
    [SerializeField] private GameObject okButton;
    [SerializeField] private GameObject container;

    [SerializeField] private GameObject scrollCardDisplay;
    private Card currentSelectedCard;

    public Card GETSelectedCard()
    {
        return currentSelectedCard;
    }


    // Start is called before the first frame update
    private void Start()
    {
        OnHover.cardSelectedEvent.AddListener(ONCardSelected);
        titleText.text = title;
        descriptionText.text = description;

        if (isInformation)
        {
            okButtonText.text = "Ok";
            positiveButton.SetActive(false);
            negativeButton.SetActive(false);
            okButton.SetActive(true);

            var okBtn = okButton.GetComponent<Button>();
            if (OkAction != null)
            {
                okBtn.onClick.AddListener(OkAction);
            }
            else
            {
                okBtn.onClick.AddListener(() => { Destroy(gameObject); });
            }
        }
        else
        {
            positiveButtonText.text = "Oui";
            negativeButtonText.text = "Non";


            var positiveBtn = positiveButton.GetComponent<Button>();
            var negativeBtn = negativeButton.GetComponent<Button>();
            positiveBtn.onClick.AddListener(PositiveAction);
            negativeBtn.onClick.AddListener(NegativeAction);


            positiveButton.SetActive(true);
            negativeButton.SetActive(true);
            okButton.SetActive(false);
        }

        scrollCardDisplay.SetActive(displayCards);
    }

    private void ONCardSelected(Card card)
    {
        currentSelectedCard = card;
    }

    private void Update()
    {
        if (currentSelectedCard == null) return;
        var children = container.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            var childGameObject = child.gameObject;
            if (childGameObject.GetComponent<CardDisplay>() == null) continue;
            var cardNom = childGameObject.GetComponent<CardDisplay>().card.Nom;

            if (!(currentSelectedCard is null) && currentSelectedCard.Nom != cardNom)
            {
                childGameObject.GetComponent<OnHover>().bIsSelected = false;
            }
            else if (!childGameObject.GetComponent<OnHover>().bIsSelected)
            {
                // if the card is unselected
                currentSelectedCard = null;
            }
        }
    }

    public static GameObject CreateSimpleMessageBox(Transform canvas, string title, string description,
        UnityAction positiveAction = null,
        UnityAction negativeAction = null)
    {
        var messageBox = Resources.FindObjectsOfTypeAll<MessageBox>();
        var messageBoxGameObject = messageBox[0].gameObject;
        var message = Instantiate(messageBoxGameObject);
        Destroy(message.GetComponent<DDOL>());
        message.transform.SetParent(canvas); // Must set parent after removing DDOL to avoid errors
        message.GetComponent<MessageBox>().title = title;
        message.GetComponent<MessageBox>().description = description;
        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            positiveAction?.Invoke();
            Destroy(message);
        };
        message.GetComponent<MessageBox>().NegativeAction = () =>
        {
            negativeAction?.Invoke();
            Destroy(message);
        };
        message.SetActive(true);
        return message;
    }

    public static GameObject CreateOkMessageBox(Transform canvas, string title, string description,
        UnityAction okAction = null)
    {
        var messageBox = Resources.FindObjectsOfTypeAll<MessageBox>();
        var messageBoxGameObject = messageBox[0].gameObject;
        var message = Instantiate(messageBoxGameObject);
        Destroy(message.GetComponent<DDOL>());
        message.transform.SetParent(canvas); // Must set parent after removing DDOL to avoid errors
        message.GetComponent<MessageBox>().isInformation = true;
        message.GetComponent<MessageBox>().title = title;
        message.GetComponent<MessageBox>().description = description;
        message.GetComponent<MessageBox>().OkAction = () =>
        {
            okAction?.Invoke();
            Destroy(message);
        };
        message.SetActive(true);
        return message;
    }

    public static GameObject CreateMessageBoxWithCardSelector(Transform canvas, string title, List<Card> cards,
        UnityAction positiveAction = null, UnityAction negativeAction = null, bool okButton = false)
    {
        var messageBox = Resources.FindObjectsOfTypeAll<MessageBox>();
        var messageBoxGameObject = messageBox[0].gameObject;
        var message = Instantiate(messageBoxGameObject);
        Destroy(message.GetComponent<DDOL>());
        message.transform.SetParent(canvas); // Must set parent after removing DDOL to avoid errors
        message.GetComponent<MessageBox>().title = title;
        message.GetComponent<MessageBox>().displayCards = true;
        message.GetComponent<MessageBox>().isInformation = okButton;
        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            positiveAction?.Invoke();
            Destroy(message);
        };
        message.GetComponent<MessageBox>().NegativeAction = () =>
        {
            negativeAction?.Invoke();
            Destroy(message);
        };
        message.SetActive(true);
        return message;
    }
}
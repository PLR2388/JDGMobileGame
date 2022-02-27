using System.Collections.Generic;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class NumberedCardEvent : UnityEvent<Card, int>
{
}

public class MessageBox : MonoBehaviour
{
    public UnityAction PositiveAction;
    public UnityAction NegativeAction;
    public UnityAction OkAction;
    public string title;
    public string description;
    public bool isInformation;
    public DisplayCards displayCardsScript;
    private bool displayCards;
    private bool multipleCardSelection;
    private int numberCardInSelection = 2;
    private bool displayNumberOnCard = false;

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
    private readonly List<Card> multipleSelectedCards = new List<Card>();
    public static readonly NumberedCardEvent NumberedCardEvent = new NumberedCardEvent();

    public Card GetSelectedCard()
    {
        return currentSelectedCard;
    }

    public List<Card> GetMultipleSelectedCards()
    {
        return multipleSelectedCards;
    }


    // Start is called before the first frame update
    private void Start()
    {
        OnHover.CardSelectedEvent.AddListener(OnCardSelected);
        OnHover.CardUnselectedEvent.AddListener(OnCardUnselected);
        titleText.text = title;
        descriptionText.text = description;

        if (isInformation)
        {
            okButtonText.text = "Ok";
            positiveButton.SetActive(false);
            negativeButton.SetActive(false);
            okButton.SetActive(true);

            var okBtn = okButton.GetComponent<Button>();
            okBtn.onClick.AddListener(OkAction ?? (() => { Destroy(gameObject); }));
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

    private void OnCardSelected(Card card)
    {
        if (multipleCardSelection)
        {
            if (multipleSelectedCards.Contains(card)) return;
            if (numberCardInSelection == multipleSelectedCards.Count)
            {
                multipleSelectedCards.RemoveAt(0);
            }

            multipleSelectedCards.Add(card);

            if (displayNumberOnCard)
            {
                for (var i = 0; i < multipleSelectedCards.Count; i++)
                {
                    NumberedCardEvent.Invoke(multipleSelectedCards[i], i + 1);
                }
            }
        }
        else
        {
            currentSelectedCard = card;
        }
    }

    private void OnCardUnselected(Card card)
    {
        if (displayNumberOnCard)
        {
            multipleSelectedCards.Remove(card);
            for (var i = 0; i < multipleSelectedCards.Count; i++)
            {
                NumberedCardEvent.Invoke(multipleSelectedCards[i], i + 1);
            }
        }
    }

    private void Update()
    {
        if (currentSelectedCard == null && multipleSelectedCards.Count == 0) return;
        var children = container.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            var childGameObject = child.gameObject;
            if (childGameObject.GetComponent<CardDisplay>() == null) continue;
            var cardNom = childGameObject.GetComponent<CardDisplay>().card.Nom;

            if (multipleCardSelection)
            {
                if (multipleSelectedCards.Count > 0 && multipleSelectedCards.Find(card => card.Nom == cardNom) == null)
                {
                    childGameObject.GetComponent<OnHover>().bIsSelected = false;
                }
                else if (!childGameObject.GetComponent<OnHover>().bIsSelected)
                {
                    // if the card is unselected
                    var index = multipleSelectedCards.FindIndex(0, multipleSelectedCards.Count,
                        card => card.Nom == cardNom);
                    if (index > -1)
                    {
                        multipleSelectedCards.RemoveAt(index);
                    }
                }
            }
            else
            {
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
    }

    public static GameObject CreateSimpleMessageBox(Transform canvas, string title, string description,
        UnityAction positiveAction = null,
        UnityAction negativeAction = null,
        List<GameObject> gameObjectsToHide = null)
    {
        var messageBox = Resources.FindObjectsOfTypeAll<MessageBox>();
        var messageBoxGameObject = messageBox[messageBox.Length - 1].gameObject;
        var message = Instantiate(messageBoxGameObject);
        message.SetActive(true);
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

        return message;
    }

    public static GameObject CreateOkMessageBox(Transform canvas, string title, string description,
        UnityAction okAction = null)
    {
        var messageBox = Resources.FindObjectsOfTypeAll<MessageBox>();
        var messageBoxGameObject = messageBox[messageBox.Length - 1].gameObject;
        var message = Instantiate(messageBoxGameObject);
        message.SetActive(true);
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
        return message;
    }

    public static GameObject CreateMessageBoxWithCardSelector(Transform canvas, string title, List<Card> cards,
        UnityAction positiveAction = null, UnityAction negativeAction = null, bool okButton = false,
        bool multipleCardSelection = false, int numberCardInSelection = 2, bool displayOrder = false)

    {
        var messageBox = Resources.FindObjectsOfTypeAll<MessageBox>();
        var messageBoxGameObject = messageBox[messageBox.Length - 1].gameObject;
        var message = Instantiate(messageBoxGameObject);
        message.SetActive(true);
        Destroy(message.GetComponent<DDOL>());
        message.transform.SetParent(canvas); // Must set parent after removing DDOL to avoid errors
        message.GetComponent<MessageBox>().title = title;
        message.GetComponent<MessageBox>().displayCards = true;
        message.GetComponent<MessageBox>().multipleCardSelection = multipleCardSelection;
        message.GetComponent<MessageBox>().numberCardInSelection = numberCardInSelection;
        message.GetComponent<MessageBox>().displayNumberOnCard = displayOrder;
        message.GetComponent<MessageBox>().displayCardsScript.cardsList = cards;
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

        return message;
    }
}
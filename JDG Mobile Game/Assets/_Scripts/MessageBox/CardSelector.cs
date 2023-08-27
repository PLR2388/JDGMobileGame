using System.Collections.Generic;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSelector : StaticInstance<CardSelector>
{
    private bool multipleCardSelection;
    private int numberCardInSelection = 2;
    private bool displayNumberOnCard = false;

    [SerializeField] private GameObject prefab;

    private InGameCard currentSelectedCard;
    private readonly List<InGameCard> multipleSelectedCards = new List<InGameCard>();

    public static readonly NumberedCardEvent NumberedCardEvent = new NumberedCardEvent();

    void Start()
    {
        OnHover.CardSelectedEvent.AddListener(OnCardSelected);
        OnHover.CardUnselectedEvent.AddListener(OnCardUnselected);
    }

    private void OnCardSelected(InGameCard card)
    {
        if (multipleCardSelection)
        {
            if (multipleSelectedCards.Exists(selectedCard => card.Title == selectedCard.Title)) return;
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

    private void OnCardUnselected(InGameCard card)
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

    private void SetValueGameObject(GameObject newGameObject, CardSelectorConfig config)
    {
        var titleTextTransform = newGameObject.transform.GetChild(0).Find("TitleText");
        var descriptionTextTransform = newGameObject.transform.GetChild(0).Find("DescriptionText");
        var positiveButtonTransform = newGameObject.transform.GetChild(0).Find("PositiveButton");
        var negativeButtonTransform = newGameObject.transform.GetChild(0).Find("NegativeButton");
        var okButtonTransfrom = newGameObject.transform.GetChild(0).Find("OkButton");
        var containerTransform = newGameObject.transform.GetChild(0).GetChild(0).Find("Container");
        var cardsTransform = newGameObject.transform.GetChild(0).Find("Cards");

        var titleText = titleTextTransform.GetComponent<TextMeshProUGUI>();
        descriptionTextTransform.gameObject.SetActive(false);
        var positiveButtonText = positiveButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
        var negativeButtonText = negativeButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
        var okButtonText = okButtonTransfrom.GetComponentInChildren<TextMeshProUGUI>();
        var positiveButton = positiveButtonTransform.gameObject;
        var negativeButton = negativeButtonTransform.gameObject;
        var okButton = okButtonTransfrom.gameObject;
        var displayCardsScript = containerTransform.GetComponent<DisplayCards>();
        positiveButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_YES);
        negativeButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_NO);
        okButtonText.text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_OK);

        displayCardsScript.cardsList = config.Cards;
        displayNumberOnCard = config.DisplayOrder;

        numberCardInSelection = config.NumberCardSelection;
        multipleCardSelection = config.NumberCardSelection > 1;

        cardsTransform.gameObject.SetActive(true);


        titleText.text = config.Title;

        positiveButton.SetActive(config.ShowPositiveButton);
        okButton.SetActive(config.ShowOkButton);
        negativeButton.SetActive(config.ShowNegativeButton);

        var okBtn = okButton.GetComponent<Button>();
        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(() =>
        {
            config.OkAction?.Invoke(currentSelectedCard);
            config.OkMultipleAction?.Invoke(multipleSelectedCards);
            switch (config.NumberCardSelection)
            {
                case 1:
                {
                    if (currentSelectedCard != null)
                    {
                        Destroy(newGameObject);
                    }
                    break;
                }
                case 0:
                    Destroy(newGameObject);
                    break;
                default:
                {
                    if (multipleSelectedCards.Count == config.NumberCardSelection)
                    {
                        Destroy(newGameObject);
                    }
                    break;
                }
            }
        });

        var positiveBtn = positiveButton.GetComponent<Button>();
        positiveBtn.onClick.RemoveAllListeners();
        positiveBtn.onClick.AddListener(() =>
        {
            config.PositiveAction?.Invoke(currentSelectedCard);
            config.PositiveMultipleAction?.Invoke(multipleSelectedCards);
            if (config.NumberCardSelection == 1)
            {
                if (currentSelectedCard != null)
                {
                    Destroy(newGameObject);
                }
            }
            else
            {
                if (multipleSelectedCards.Count == config.NumberCardSelection)
                {
                    Destroy(newGameObject);
                }
            }

        });

        var negativeBtn = negativeButton.GetComponent<Button>();
        negativeBtn.onClick.RemoveAllListeners();
        negativeBtn.onClick.AddListener(() =>
        {
            config.NegativeAction?.Invoke();
            Destroy(newGameObject);
        });
    }
    public void CreateCardSelection(Transform canvas, CardSelectorConfig config)
    {
        var message = Instantiate(prefab);
        message.SetActive(true);
        message.transform.SetParent(canvas);

        SetValueGameObject(message, config);
    }
}
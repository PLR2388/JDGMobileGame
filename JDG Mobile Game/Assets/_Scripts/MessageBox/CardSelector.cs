using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class NumberedCardEvent : UnityEvent<InGameCard, int>
{
}

public class CardSelector : StaticInstance<CardSelector>, IMessageBoxBaseComponent
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

    private GameObject GetDescriptionTextGameObject(GameObject parent)
    {
        var descriptionTextTransform = parent.transform.GetChild(0).Find("DescriptionText");
        return descriptionTextTransform.gameObject;
    }

    private GameObject GetChildGameObject(GameObject parent, string childName)
    {
        return parent.transform.GetChild(0).Find(childName)?.gameObject;
    }

    private Button GetButton(GameObject parent, string childName)
    {
        var buttonTransform = parent.transform.GetChild(0).Find(childName);
        return buttonTransform.GetComponent<Button>();
    }

    private DisplayCards GetDisplayCards(GameObject parent)
    {
        var containerTransform = parent.transform.GetChild(0).GetChild(0).Find("Container");
        return containerTransform.GetComponent<DisplayCards>();
    }

    private void ConfigureButton(Button button, UnityAction action, GameObject gameObjectToDestroy)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            action?.Invoke();
            if (gameObjectToDestroy != null)
                Destroy(gameObjectToDestroy);
        });
    }


    public void SetNewValueGameObject(GameObject newGameObject, UIConfig config)
    {
        this.SetValueGameObject(newGameObject, config);
        var cardSelectorConfig = config as CardSelectorConfig;

        GetChildGameObject(newGameObject, "Cards").SetActive(true);
        GetDescriptionTextGameObject(newGameObject).SetActive(false);

        var displayCardsScript = GetDisplayCards(newGameObject);
        displayCardsScript.CardsList = cardSelectorConfig?.Cards;

        displayNumberOnCard = cardSelectorConfig?.DisplayOrder == true;
        numberCardInSelection = cardSelectorConfig?.NumberCardSelection ?? 0;
        multipleCardSelection = cardSelectorConfig?.NumberCardSelection > 1;


        var okBtn = GetButton(newGameObject, "OkButton");
        var positiveBtn = GetButton(newGameObject, "PositiveButton");
        var negativeBtn = GetButton(newGameObject, "NegativeButton");

        UnityAction okAction = () =>
        {
            cardSelectorConfig?.OkAction?.Invoke(currentSelectedCard);
            cardSelectorConfig?.OkMultipleAction?.Invoke(multipleSelectedCards);
            switch (cardSelectorConfig?.NumberCardSelection)
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
                    if (multipleSelectedCards.Count == cardSelectorConfig?.NumberCardSelection)
                    {
                        Destroy(newGameObject);
                    }
                    break;
                }
            }
        };
        ConfigureButton(okBtn, okAction, null);

        UnityAction positiveAction = () =>
        {
            cardSelectorConfig?.PositiveAction?.Invoke(currentSelectedCard);
            cardSelectorConfig?.PositiveMultipleAction?.Invoke(multipleSelectedCards);
            if (cardSelectorConfig?.NumberCardSelection == 1)
            {
                if (currentSelectedCard != null)
                {
                    Destroy(newGameObject);
                }
            }
            else
            {
                if (multipleSelectedCards.Count == cardSelectorConfig?.NumberCardSelection)
                {
                    Destroy(newGameObject);
                }
            }
        };
        ConfigureButton(positiveBtn, positiveAction, null);
        ConfigureButton(negativeBtn, config.NegativeAction, newGameObject);
    }
    public void CreateCardSelection(Transform canvas, CardSelectorConfig config)
    {
        var message = Instantiate(prefab);
        message.SetActive(true);
        message.transform.SetParent(canvas);

        SetNewValueGameObject(message, config);
    }
    
    private void OnDestroy()
    {
        OnHover.CardSelectedEvent.RemoveListener(OnCardSelected);
        OnHover.CardUnselectedEvent.RemoveListener(OnCardUnselected);
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
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
    #region Fields and Properties

    [SerializeField] private GameObject prefab;
    public static readonly NumberedCardEvent NumberedCardEvent = new NumberedCardEvent();

    private int numberCardInSelection = 2;
    private bool displayNumberOnCard = false;

    #endregion

    #region Unity Callbacks

    /// <summary>
    /// Initialization method called by Unity. It sets up card selection and unselection event listeners.
    /// </summary>
    void Start()
    {
        CardSelectionManager.Instance.SelectionChanged.AddListener(SelectionChanged);
    }

    #endregion

    #region Private Methods

    private void SelectionChanged()
    {
        InvokeNumberedEventIfRequired();
    }

    /// <summary>
    /// If the display number on card feature is enabled, it invokes the NumberedCardEvent 
    /// for each card in the list with its corresponding order.
    /// </summary>
    private void InvokeNumberedEventIfRequired()
    {
        if (displayNumberOnCard)
        {
            for (var i = 0; i < CardSelectionManager.Instance.SelectedCards.Count; i++)
            {
                NumberedCardEvent.Invoke(CardSelectionManager.Instance.SelectedCards[i], i + 1);
            }
        }
    }

    /// <summary>
    /// Returns the "DescriptionText" GameObject child of the provided parent GameObject.
    /// </summary>
    private GameObject GetDescriptionTextGameObject(GameObject parent)
    {
        var descriptionTextTransform = parent.transform.GetChild(0).Find("DescriptionText");
        return descriptionTextTransform.gameObject;
    }

    /// <summary>
    /// Returns the child GameObject of the provided parent GameObject by name.
    /// </summary>
    private GameObject GetChildGameObject(GameObject parent, string childName)
    {
        return parent.transform.GetChild(0).Find(childName)?.gameObject;
    }

    /// <summary>
    /// Returns the Button component of a child GameObject by name from the provided parent GameObject.
    /// </summary>
    private Button GetButton(GameObject parent, string childName)
    {
        var buttonTransform = parent.transform.GetChild(0).Find(childName);
        return buttonTransform.GetComponent<Button>();
    }


    /// <summary>
    /// Returns the DisplayCards component of the "Container" child GameObject from the provided parent GameObject.
    /// </summary>
    private DisplayCards GetDisplayCards(GameObject parent)
    {
        var containerTransform = parent.transform.GetChild(0).GetChild(0).Find("Container");
        return containerTransform.GetComponent<DisplayCards>();
    }

    /// <summary>
    /// Configures the provided button to invoke the specified action and optionally destroy the provided GameObject.
    /// </summary>
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

    /// <summary>
    /// Configures the OK, Positive, and Negative buttons for the card selector.
    /// </summary>
    private void ConfigureButtons(GameObject newGameObject, UIConfig config, CardSelectorConfig cardSelectorConfig)
    {
        var okBtn = GetButton(newGameObject, "OkButton");
        var positiveBtn = GetButton(newGameObject, "PositiveButton");
        var negativeBtn = GetButton(newGameObject, "NegativeButton");

        UnityAction okAction = () =>
        {
            var singleCard = CardSelectionManager.Instance.SelectedCards.Count > 0
                ? CardSelectionManager.Instance.SelectedCards[0]
                : null;
            var multipleCards = CardSelectionManager.Instance.SelectedCards;
            cardSelectorConfig?.OkActions.SingleAction?.Invoke(singleCard);
            cardSelectorConfig?.OkActions.MultipleAction?.Invoke(multipleCards);
            switch (cardSelectorConfig?.NumberCardSelection)
            {
                case 1:
                {
                    DestroyGameObjectSingleCard(newGameObject);
                    break;
                }
                case 0:
                    Destroy(newGameObject);
                    break;
                default:
                {
                    DestroyGameObjectMultipleSelectedCards(newGameObject, cardSelectorConfig);
                    break;
                }
            }
        };
        ConfigureButton(okBtn, okAction, null);

        UnityAction positiveAction = () =>
        {
            var singleCard = CardSelectionManager.Instance.SelectedCards.Count > 0
                ? CardSelectionManager.Instance.SelectedCards[0]
                : null;
            var multipleCards = CardSelectionManager.Instance.SelectedCards;
            cardSelectorConfig?.PositiveActions.SingleAction?.Invoke(singleCard);
            cardSelectorConfig?.PositiveActions.MultipleAction?.Invoke(multipleCards);
            if (cardSelectorConfig?.NumberCardSelection == 1)
            {
                DestroyGameObjectSingleCard(newGameObject);
            }
            else
            {
                DestroyGameObjectMultipleSelectedCards(newGameObject, cardSelectorConfig);
            }
        };
        ConfigureButton(positiveBtn, positiveAction, null);
        ConfigureButton(negativeBtn, config.NegativeAction, newGameObject);
    }

    /// <summary>
    /// Destroys the provided GameObject if a single card is selected.
    /// </summary>
    private void DestroyGameObjectSingleCard(GameObject newGameObject)
    {
        if (CardSelectionManager.Instance.SelectedCards.Count > 0)
        {
            CardSelectionManager.Instance.ClearSelection();
            Destroy(newGameObject);
        }
    }

    /// <summary>
    /// Destroys the provided GameObject if the count of multiple selected cards matches the specified configuration.
    /// </summary>
    private void DestroyGameObjectMultipleSelectedCards(GameObject newGameObject, CardSelectorConfig cardSelectorConfig)
    {
        if (CardSelectionManager.Instance.SelectedCards.Count == cardSelectorConfig?.NumberCardSelection)
        {
            CardSelectionManager.Instance.ClearSelection();
            Destroy(newGameObject);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Configures and sets values for the card selector using the provided UIConfig.
    /// </summary>
    public void SetNewValueGameObject(GameObject newGameObject, UIConfig config)
    {
        this.SetValueGameObject(newGameObject, config);
        var cardSelectorConfig = config as CardSelectorConfig;

        GetChildGameObject(newGameObject, "Cards").SetActive(true);
        GetDescriptionTextGameObject(newGameObject).SetActive(false);

        var displayCardsScript = GetDisplayCards(newGameObject);
        displayCardsScript.CardsList = cardSelectorConfig?.Cards;

        displayNumberOnCard = cardSelectorConfig?.ShowOrder == true;
        CardSelectionManager.Instance.MultipleSelectionLimit = cardSelectorConfig?.NumberCardSelection ?? 0;
        CardSelectionManager.Instance.MultipleCardSelection = cardSelectorConfig?.NumberCardSelection > 1;

        ConfigureButtons(newGameObject, config, cardSelectorConfig);
    }

    /// <summary>
    /// Creates and initializes a new card selection instance on the provided canvas using the specified configuration.
    /// </summary>
    public void CreateCardSelection(Transform canvas, CardSelectorConfig config)
    {
        var message = Instantiate(prefab);
        message.SetActive(true);
        message.transform.SetParent(canvas);

        SetNewValueGameObject(message, config);
    }

    #endregion
}
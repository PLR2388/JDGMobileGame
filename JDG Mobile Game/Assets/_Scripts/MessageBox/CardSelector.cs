using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;

[System.Serializable]
public class NumberedCardEvent : UnityEvent<InGameCard, int>
{
}

/// <summary>
/// Displays card selector dialogs for card selection.
/// Phase 9: Removed CardSelectionManager singleton dependency via DI.
/// Phase 41: Migrated to clean ICardSelectionService with EventBus.
/// Phase 52: Marked obsolete - use IDialogService via dependency injection instead.
/// Phase 94: Removed StaticInstance inheritance - now a regular MonoBehaviour.
/// </summary>
public class CardSelector : MonoBehaviour, IMessageBoxBaseComponent
{
    #region Fields and Properties

    [SerializeField] private GameObject prefab;
    public static readonly NumberedCardEvent NumberedCardEvent = new NumberedCardEvent();

    private bool displayNumberOnCard = false;

    // Phase 41: Migrated to clean ICardSelectionService with EventBus
    private ICardSelectionService _cardSelectionService;
    private IEventBus _eventBus;
    private IDisposable _selectionChangedSubscription;

    #endregion

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 9: Inject ICardSelectionService instead of using singleton.
    /// Phase 41: Migrated to clean JDG.Application.Services.ICardSelectionService.
    /// </summary>
    [Inject]
    public void Construct(ICardSelectionService cardSelectionService, IEventBus eventBus)
    {
        _cardSelectionService = cardSelectionService;
        _eventBus = eventBus;
    }

    #region Unity Callbacks

    /// <summary>
    /// Initialization method called by Unity. It sets up card selection and unselection event listeners.
    /// Phase 41: Subscribe to EventBus instead of UnityEvent.
    /// </summary>
    void Start()
    {
        _selectionChangedSubscription = _eventBus?.Subscribe<CardSelectionChangedEvent>(OnSelectionChangedEvent);
    }

    /// <summary>
    /// EventBus handler for selection changed.
    /// Phase 41: Replaces UnityEvent SelectionChanged listener.
    /// </summary>
    private void OnSelectionChangedEvent(CardSelectionChangedEvent evt)
    {
        SelectionChanged();
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
    /// Phase 41: Cast from object to InGameCard for clean interface.
    /// </summary>
    private void InvokeNumberedEventIfRequired()
    {
        if (displayNumberOnCard)
        {
            var selectedCards = _cardSelectionService.SelectedCards;
            for (var i = 0; i < selectedCards.Count; i++)
            {
                if (selectedCards[i] is InGameCard card)
                {
                    NumberedCardEvent.Invoke(card, i + 1);
                }
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
    /// Phase 41: Cast from object to InGameCard for clean interface.
    /// </summary>
    private void ConfigureButtons(GameObject newGameObject, UIConfig config, CardSelectorConfig cardSelectorConfig)
    {
        var okBtn = GetButton(newGameObject, "OkButton");
        var positiveBtn = GetButton(newGameObject, "PositiveButton");
        var negativeBtn = GetButton(newGameObject, "NegativeButton");

        UnityAction okAction = () =>
        {
            var selectedCards = _cardSelectionService.SelectedCards;
            var singleCard = selectedCards.Count > 0
                ? selectedCards[0] as InGameCard
                : null;
            var multipleCards = selectedCards.OfType<InGameCard>().ToList();
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
            var selectedCards = _cardSelectionService.SelectedCards;
            var singleCard = selectedCards.Count > 0
                ? selectedCards[0] as InGameCard
                : null;
            var multipleCards = selectedCards.OfType<InGameCard>().ToList();
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
        if (_cardSelectionService.SelectedCards.Count > 0)
        {
            _cardSelectionService.ClearSelection();
            Destroy(newGameObject);
        }
    }

    /// <summary>
    /// Destroys the provided GameObject if the count of multiple selected cards matches the specified configuration.
    /// </summary>
    private void DestroyGameObjectMultipleSelectedCards(GameObject newGameObject, CardSelectorConfig cardSelectorConfig)
    {
        if (_cardSelectionService.SelectedCards.Count == cardSelectorConfig?.NumberCardSelection)
        {
            _cardSelectionService.ClearSelection();
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
        _cardSelectionService.MultipleSelectionLimit = cardSelectorConfig?.NumberCardSelection ?? 0;
        _cardSelectionService.MultipleCardSelection = cardSelectorConfig?.NumberCardSelection > 1;

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

    /// <summary>
    /// Cleanup and dispose of EventBus subscriptions.
    /// Phase 41: Added disposal for EventBus subscription.
    /// </summary>
    private void OnDestroy()
    {
        _selectionChangedSubscription?.Dispose();
    }

    #endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using Menu;
using UnityEngine;
using VContainer;

/// <summary>
/// Phase 9: Removed CardSelectionManager singleton dependency via DI.
/// Phase 17-18: Removed GameState singleton dependency via IDeckManagementService.
/// Phase 23: Migrated from static UnityEvent to EventBus subscription.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 35: Uses IDialogService instead of MessageBox.Instance.
/// Phase 41: Migrated to clean ICardSelectionService with EventBus.
/// </summary>
public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private Transform canvas;

    private int numberOfSelectedCards;
    private int numberOfRareCards;

    private bool displayP1Card = true;
    private List<Card> deck1AllCards;
    private List<Card> deck2AllCards;

    // Phase 41: Migrated to clean ICardSelectionService
    private ICardSelectionService _cardSelectionService;

    // Phase 17-18: Injected dependencies
    private IDeckManagementService _deckManagementService;

    // Phase 23 & 41: EventBus for event subscriptions
    private IEventBus _eventBus;
    private IDisposable _choicePlayerSubscription;
    private IDisposable _cardSelectedSubscription;
    private IDisposable _cardDeselectedSubscription;

    // Phase 34: Injected dependencies
    private ILocalizationService _localizationService;

    // Phase 35: Injected dependencies
    private IDialogService _dialogService;

    private readonly string[] removeCardTitles =
    {
        CardNameMappings.CardNameMap[CardNames.AttaqueDeLaTourEiffel],
        CardNameMappings.CardNameMap[CardNames.BlagueInterdite],
        CardNameMappings.CardNameMap[CardNames.UnBonTuyau]
    };

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 9: Inject ICardSelectionService instead of using singleton.
    /// Phase 17-18: Inject IDeckManagementService instead of GameState.Instance.
    /// Phase 23: Inject IEventBus for static UnityEvent migration.
    /// Phase 34: Inject ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 35: Inject IDialogService instead of MessageBox.Instance.
    /// Phase 41: Migrated to clean JDG.Application.Services.ICardSelectionService.
    /// </summary>
    [Inject]
    public void Construct(
        ICardSelectionService cardSelectionService,
        IDeckManagementService deckManagementService,
        IEventBus eventBus,
        ILocalizationService localizationService,
        IDialogService dialogService)
    {
        _cardSelectionService = cardSelectionService;
        _deckManagementService = deckManagementService;
        _eventBus = eventBus;
        _localizationService = localizationService;
        _dialogService = dialogService;
    }

    // Start is called before the first frame update
    private void Start()
    {
        deck1AllCards = _deckManagementService.Deck1AllCards;
        deck2AllCards = _deckManagementService.Deck2AllCards;
        DisplayAvailableCards(deck1AllCards);

        // Phase 41: Use clean service with EventBus subscriptions
        _cardSelectionService.MultipleCardSelection = true;
        _cardSelectionService.MultipleSelectionLimit = DeckConfiguration.MaxDeckCards;
        _cardSelectedSubscription = _eventBus.Subscribe<CardAddedToSelectionEvent>(OnCardSelected);
        _cardDeselectedSubscription = _eventBus.Subscribe<CardRemovedFromSelectionEvent>(OnCardDeselected);

        // Phase 23: Subscribe to EventBus instead of static UnityEvent
        _choicePlayerSubscription = _eventBus.Subscribe<ChoicePlayerChangedEvent>(OnChoicePlayerChanged);
    }

    /// <summary>
    /// EventBus handler for card selection.
    /// Phase 41: Replaces UnityEvent CardSelected listener.
    /// </summary>
    private void OnCardSelected(CardAddedToSelectionEvent evt)
    {
        if (evt.Card is InGameCard card)
        {
            OnSelectCard(card);
        }
    }

    /// <summary>
    /// EventBus handler for card deselection.
    /// Phase 41: Replaces UnityEvent CardDeselected listener.
    /// </summary>
    private void OnCardDeselected(CardRemovedFromSelectionEvent evt)
    {
        if (evt.Card is InGameCard card)
        {
            OnUnSelectCard(card);
        }
    }


    /// <summary>
    /// Event handler for ChoicePlayerChangedEvent from EventBus.
    /// Phase 23: Replaces static UnityEvent listener.
    /// </summary>
    private void OnChoicePlayerChanged(ChoicePlayerChangedEvent evt)
    {
        displayP1Card = evt.PlayerIndex == 1;
        numberOfSelectedCards = 0;
        numberOfRareCards = 0;
        DisplayAvailableCards(displayP1Card ? deck1AllCards : deck2AllCards);
    }

    /// <summary>
    /// Card has been selected, count of card updated
    /// Display message if neccessary
    /// </summary>
    /// <param name="card"></param>
    private void OnSelectCard(InGameCard card)
    {
        numberOfSelectedCards++;
        if (card.Collector)
        {
            numberOfRareCards++;
        }

        CheckNumberOfSelectedCards(card);
        CheckNumberOfRareCards(card);
    }
    
    /// <summary>
    /// Checks if the number of rare cards selected exceeds the maximum allowed limit.
    /// If it does, the provided card is unselected, and a warning message is displayed.
    /// Phase 34: Uses injected ILocalizationService.
    /// </summary>
    /// <param name="card">The card to check and possibly unselect.</param>
    private void CheckNumberOfRareCards(InGameCard card)
    {
        // Phase 17-18: Use DeckConfiguration instead of GameState for constants
        if (numberOfRareCards > DeckConfiguration.MaxRare)
        {
            _cardSelectionService.UnselectCard(card);
            DisplayMessageBox(
                _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_COLLECTOR_CARD)
            );
        }
    }

    /// <summary>
    /// Checks if the total number of selected cards exceeds the maximum allowed limit.
    /// If it does, the provided card is unselected, and a warning message is displayed.
    /// Phase 34: Uses injected ILocalizationService.
    /// </summary>
    /// <param name="card">The card to check and possibly unselect.</param>
    private void CheckNumberOfSelectedCards(InGameCard card)
    {
        // Phase 17-18: Use DeckConfiguration instead of GameState for constants
        if (numberOfSelectedCards > DeckConfiguration.MaxDeckCards)
        {
            _cardSelectionService.UnselectCard(card);
            DisplayMessageBox(
                _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_NUMBER_CARDS)
            );
        }
    }

    /// <summary>
    /// Card has been removed from selection
    /// </summary>
    /// <param name="card"></param>
    private void OnUnSelectCard(InGameCard card)
    {
        numberOfSelectedCards--;
        if (card.Collector)
        {
            numberOfRareCards--;
        }
    }

    /// <summary>
    /// Display all available card to choose from for the user to choose
    /// </summary>
    /// <param name="allCards"></param>
    private void DisplayAvailableCards(List<Card> allCards)
    {
        foreach (var card in allCards.Where(card => card.Type != CardType.Contre && !removeCardTitles.Contains(card.Title)))
        {
            CreateCardDisplay(card);
        }
    }

    /// <summary>
    /// Create a display for a card
    /// </summary>
    /// <param name="card"></param>
    private void CreateCardDisplay(Card card)
    {
        var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
        newCard.GetComponent<OnHover>().bIsInGame = false;
        newCard.transform.SetParent(transform, true);
        newCard.GetComponent<CardDisplay>().Card = card;
    }

    /// <summary>
    /// Display a warning messageBox with a ok button and a custom message
    /// Phase 34: Uses injected ILocalizationService.
    /// Phase 35: Uses injected IDialogService.
    /// </summary>
    /// <param name="msg"></param>
    private void DisplayMessageBox(string msg)
    {
        _dialogService.ShowMessageBox(canvas, new MessageBoxOptions
        {
            Title = _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            Message = msg,
            ShowOkButton = true
        });
    }

    /// <summary>
    /// Remove Event listener attached
    /// Phase 23: Disposes EventBus subscription.
    /// Phase 41: Disposes card selection subscriptions.
    /// </summary>
    private void OnDestroy()
    {
        _choicePlayerSubscription?.Dispose();
        _cardSelectedSubscription?.Dispose();
        _cardDeselectedSubscription?.Dispose();
    }
}
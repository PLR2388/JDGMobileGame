using System;
using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using JDG.Infrastructure.Cards;
using JDG.Infrastructure.Services;
using OnePlayer;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// Represents the tutorial version of the in-game menu.
/// Phase 17-18: Inherits ICardCollectionService from base class.
/// Phase 34: Inherits ILocalizationService from base class.
/// Phase 90: Uses ITutorialStateService instead of DialogueTutoHandler.Instance.
/// </summary>
public class TutoInGameMenuScript : InGameMenuScript
{
    private const int CardDialogChangeIndex = 36;
    private const int PutCardIndex = 38;

    // Phase 90: Tutorial state service replaces DialogueTutoHandler singleton
    private ITutorialStateService _tutorialStateService;

    // Phase 109: EventBus subscription for card click events
    private IDisposable _tutoCardClickedSubscription;

    private TextMeshProUGUI buttonTextMeshProUGUI;
    private Button button;
    private HighLightButton highLightButton;
    private HighLightButton putCardHighLightButton;

    /// <summary>
    /// Phase 90: VContainer injection for tutorial-specific dependencies.
    /// Phase 133: Renamed from ConstructTutorial to Construct for VContainer compatibility.
    /// Phase 144: Fixed to inject ALL base class dependencies including _eventBus.
    /// Previously only injected ITutorialStateService, leaving _eventBus null.
    /// This caused DialogueTriggerCompletedEvent to never publish in ClickPutCard().
    /// </summary>
    [Inject]
    public void Construct(
        ITutorialStateService tutorialStateService,
        ICardCollectionService cardCollectionService,
        IEventBus eventBus,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService,
        GameStateService gameStateService)
    {
        _tutorialStateService = tutorialStateService;
        _cardCollectionService = cardCollectionService;
        _eventBus = eventBus;
        _playerStatusProvider = playerStatusProvider;
        _localizationService = localizationService;
        _gameStateService = gameStateService;
    }

    /// <summary>
    /// Awake method to cache component references.
    /// </summary>
    private void Awake()
    {
        buttonTextMeshProUGUI = buttonText.GetComponent<TextMeshProUGUI>();
        button = inHandButton.GetComponent<Button>();
        highLightButton = inHandButton.GetComponent<HighLightButton>();
        putCardHighLightButton = putCardButton.GetComponent<HighLightButton>();
    }

    /// <summary>
    /// Initializes the state of UI and card handlers.
    /// Phase 109: Uses EventBus instead of static EventClick UnityEvent.
    /// </summary>
    private void Start()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        _tutoCardClickedSubscription = _eventBus?.Subscribe<InGameCardClickedEvent>(OnTutoCardClicked);
        InitializeCardHandlers();
    }

    /// <summary>
    /// Event handler for InGameCardClickedEvent from EventBus.
    /// Phase 109: Replaces static UnityEvent listener.
    /// </summary>
    private void OnTutoCardClicked(InGameCardClickedEvent evt)
    {
        if (evt.Card is InGameCard card)
        {
            ClickOnCard(card);
        }
    }

    /// <summary>
    /// Cleanup EventBus subscription on destroy.
    /// Phase 109: Added for proper resource cleanup.
    /// </summary>
    private void OnDestroy()
    {
        _tutoCardClickedSubscription?.Dispose();
    }
    
    /// <summary>
    /// Handles the logic when a card is clicked.
    /// </summary>
    /// <param name="card">The in-game card that was clicked.</param>
    private void ClickOnCard(InGameCard card)
    {
        // Phase 90: Use injected service instead of singleton
        var currentIndex = _tutorialStateService?.CurrentDialogIndex ?? 0;
        var authorizedCard = currentIndex > CardDialogChangeIndex
            ? CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive]
            : CardNameMappings.CardNameMap[CardNames.ClichéRaciste];
        if (card.Title != authorizedCard) return;
        CurrentSelectedCard = card;
        if (CardHandlerMap.TryGetValue(card.Type, out var handler))
        {
            handler.HandleCard(card);
        }
        else
        {
            Debug.LogError($"Unexpected card type: {card.Type}");
        }

        var clickPosition = GetClickPosition();
        putCardHighLightButton.isActivated = true;
        DisplayMiniMenuCardAtPosition(clickPosition);
    }

    /// <summary>
    /// Handles the "Put Card" action, triggering the appropriate event based on the card's type.
    /// Phase 146: Changed from 'new' to 'override' to fix method hiding bug.
    /// Phase 148: Moved DialogueTriggerCompletedEvent to HideHand (triggered when closing highlighted hand).
    /// </summary>
    public override void ClickPutCard()
    {
        if (CardHandlerMap.TryGetValue(CurrentSelectedCard.Type, out var handler))
        {
            handler.HandleCardPut(CurrentSelectedCard);
        }
        else
        {
            Debug.LogError($"Unexpected card type: {CurrentSelectedCard.Type}");
        }

        miniMenuCard.SetActive(false);

        if (CurrentSelectedCard.Title == CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive])
        {
            // Phase 122: Publish via EventBus
            _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.InHandButton, IsActivated = true });
        }

        if (!detailCardPanel.activeSelf) return;
        // The detail panel is visible. One must go back to card display
        detailCardPanel.SetActive(false);
        handScreen.SetActive(true);
        inHandButton.SetActive(true);
    }
    
    /// <summary>
    /// Toggles between showing and hiding the hand card display.
    /// </summary>
    public new void ClickHandCard()
    {
        invocationMenu.SetActive(false);
        if (handScreen.activeSelf)
        {
            HideHand();
        }
        else
        {
            DisplayHand();
        }
    }
    
    /// <summary>
    /// Sets the visibility of the hand.
    /// </summary>
    /// <param name="isVisible">Whether the hand should be visible.</param>
    private void SetHandVisibility(bool isVisible)
    {
        handScreen.SetActive(isVisible);
        backgroundInformation.SetActive(!isVisible);
    }

    /// <summary>
    /// Updates the button text based on the given localization key.
    /// Phase 34: Uses inherited _localizationService instead of LocalizationSystem.Instance.
    /// </summary>
    /// <param name="key">Localization key for the button text.</param>
    private void UpdateButtonText(LocalizationKeys key)
    {
        buttonTextMeshProUGUI.text = _localizationService.GetLocalizedValue(key);
    }

    /// <summary>
    /// Deactivates the button interactivity and highlighting.
    /// </summary>
    private void UnselectButton()
    {
        button.interactable = false;
        highLightButton.isActivated = false;
    }

    /// <summary>
    /// Displays hand cards
    /// </summary>
    private void DisplayHand()
    {
        SetHandVisibility(true);
        UpdateButtonText(LocalizationKeys.BUTTON_BACK);
        UnselectButton();
        // Phase 17-18: Use ICardCollectionService from base class instead of CardManager.Instance
        // Phase 23: Publish to EventBus instead of static UnityEvent
        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        var domainOwner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        _eventBus.Publish(new JDG.Domain.Events.HandCardsDisplayChangedEvent
        {
            Player = domainOwner,
            HandCards = playerCards.HandCards
        });
    }

    /// <summary>
    /// Hides hand cards
    /// </summary>
    private void HideHand()
    {
        // Phase 148: Publish PutCard trigger when InHandButton was highlighted (player followed tutorial guidance)
        if (highLightButton.isActivated)
        {
            _eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.PutCard });
        }

        SetHandVisibility(false);
        UpdateButtonText(LocalizationKeys.BUTTON_HAND);
        UnselectButton();

        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);

        // Phase 90: Use injected service instead of singleton
        if (_tutorialStateService?.CurrentDialogIndex == PutCardIndex)
        {
            // Phase 123: Publish via EventBus instead of static TriggerDoneEvent
            _eventBus?.Publish(new DialogueTriggerCompletedEvent { TriggerType = (int)NextDialogueTrigger.PutEffectCard });
        }
        // Phase 17-18: Use ICardCollectionService from base class instead of CardManager.Instance
        if (_cardCollectionService.GetCurrentPlayerCards().InvocationCards.Count == 2)
        {
            // Phase 122: Publish via EventBus
            _eventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.NextPhaseButton, IsActivated = true });
        }
    }
}
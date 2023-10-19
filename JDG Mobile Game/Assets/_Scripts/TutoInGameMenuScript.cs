using Cards;
using OnePlayer;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the tutorial version of the in-game menu.
/// </summary>
public class TutoInGameMenuScript : InGameMenuScript
{
    private const int CardDialogChangeIndex = 36;
    private const int PutCardIndex = 38;

    private TextMeshProUGUI buttonTextMeshProUGUI;
    private Button button;
    private HighLightButton highLightButton;
    private HighLightButton putCardHighLightButton;

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
    /// </summary>
    private void Start()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        EventClick.AddListener(ClickOnCard);
        InitializeCardHandlers();
    }
    
    /// <summary>
    /// Handles the logic when a card is clicked.
    /// </summary>
    /// <param name="card">The in-game card that was clicked.</param>
    private void ClickOnCard(InGameCard card)
    {
        var authorizedCard = DialogueTutoHandler.Instance.CurrentDialogIndex > CardDialogChangeIndex
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
    /// </summary>
    public new void ClickPutCard()
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
            HighLightPlane.Highlight.Invoke(HighlightElement.InHandButton, true);
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
    /// </summary>
    /// <param name="key">Localization key for the button text.</param>
    private void UpdateButtonText(LocalizationKeys key)
    {
        buttonTextMeshProUGUI.text = LocalizationSystem.Instance.GetLocalizedValue(key);
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
        HandCardDisplay.HandCardChange.Invoke(CardManager.Instance.GetCurrentPlayerCards().HandCards);
    }

    /// <summary>
    /// Hides hand cards
    /// </summary>
    private void HideHand()
    {
        SetHandVisibility(false);
        UpdateButtonText(LocalizationKeys.BUTTON_HAND);
        UnselectButton();
        
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        
        if (DialogueTutoHandler.Instance.CurrentDialogIndex == PutCardIndex)
        {
            DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.PutEffectCard);
        }
        if (CardManager.Instance.GetCurrentPlayerCards().InvocationCards.Count == 2)
        {
            HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
        }
    }
}
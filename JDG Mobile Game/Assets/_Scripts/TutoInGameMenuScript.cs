using Cards;
using OnePlayer;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TutoInGameMenuScript : InGameMenuScript
{
    // Serialized fields for UI components

    private int currentDialogIndex = 0;

    private void Awake()
    {
        DialogueUI.DialogIndex.AddListener(SavedIndexDialog);
    }

    /// <summary>
    /// Unity's start method, called before the first frame update. Initializes UI states and card handlers.
    /// </summary>
    private void Start()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        EventClick.AddListener(ClickOnCard);
        InitializeCardHandlers();
    }

    private void SavedIndexDialog(int index)
    {
        currentDialogIndex = index;
    }

    private void ClickOnCard(InGameCard card)
    {
        var authorizedCard = currentDialogIndex > 36 ? CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive] : CardNameMappings.CardNameMap[CardNames.ClichéRaciste];
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
        putCardButton.GetComponent<HighLightButton>().isActivated = true;
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

    private void DisplayHand()
    {
        handScreen.SetActive(true);
        backgroundInformation.SetActive(false);
        buttonText.GetComponent<TextMeshProUGUI>().text = LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_BACK);
        
        inHandButton.GetComponent<Button>().interactable = false;
        inHandButton.GetComponent<HighLightButton>().isActivated = false;
        HandCardDisplay.HandCardChange.Invoke(CardManager.Instance.GetCurrentPlayerCards().HandCards);
    }

    private void HideHand()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        handScreen.SetActive(false);
        backgroundInformation.SetActive(true);
        buttonText.GetComponent<TextMeshProUGUI>().SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_HAND));
        
        inHandButton.GetComponent<Button>().interactable = false;
        inHandButton.GetComponent<HighLightButton>().isActivated = false;
        if (currentDialogIndex == 38)
        {
            DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.PutEffectCard);
        }
        if (CardManager.Instance.GetCurrentPlayerCards().InvocationCards.Count == 2)
        {
            HighLightPlane.Highlight.Invoke(HighlightElement.NextPhaseButton, true);
        }
    }
}
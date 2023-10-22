using System.Collections.ObjectModel;
using Cards;
using UnityEngine;

/// <summary>
/// Represents the display of cards in a tutorial scenario.
/// </summary>
public class TutoHandCardDisplay : HandCardDisplay
{

    private int currentDialogIndex = 0;

    private const int StartHighlightMusiqueDeMegadriveIndex = 35;

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Subscribes to relevant events.
    /// </summary>
    private void OnEnable()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Called when the behaviour becomes disabled.
    /// Unsubscribes from events and clears created cards.
    /// </summary>
    private void OnDisable()
    {
        ClearCreatedCards();
    }

    /// <summary>
    /// Called before the object is destroyed.
    /// Cleans up by clearing created cards and unsubscribing from events.
    /// </summary>
    private void OnDestroy()
    {
        ClearCreatedCards();
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Subscribes to necessary events for card display updates.
    /// </summary>
    private void SubscribeToEvents()
    {
        HandCardChange.AddListener(DisplayHandCard);
        DialogueUI.DialogIndex.AddListener(UpdateCurrentDialogIndex);
    }

    /// <summary>
    /// Unsubscribes from hand card change events.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        HandCardChange.RemoveListener(DisplayHandCard);
        DialogueUI.DialogIndex.RemoveListener(UpdateCurrentDialogIndex);
    }

    /// <summary>
    /// Updates the current dialog index and checks for highlighting conditions.
    /// </summary>
    /// <param name="index">The new dialog index.</param>
    private void UpdateCurrentDialogIndex(int index)
    {
        currentDialogIndex = index;
        HighlightCardOnDialogChange(index);
    }
    
    /// <summary>
    /// Highlights specific cards based on the current dialog index.
    /// </summary>
    /// <param name="index">The current dialog index.</param>
    private void HighlightCardOnDialogChange(int index)
    {
        if (index > StartHighlightMusiqueDeMegadriveIndex)
        {
            var card = CreatedCards.Find(card => card.GetComponent<CardDisplay>().Card.Title == CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive]);
            if (card != null)
            {
                card.AddComponent<HighLightCard>();
            }
        }
    }

    /// <summary>
    /// Determines if a specific in-game card should be highlighted.
    /// </summary>
    /// <param name="handCard">The in-game card to check.</param>
    /// <returns>True if the card should be highlighted, false otherwise.</returns>
    private bool ShouldHighlightCard(InGameCard handCard)
    {
        return handCard.Title == CardNameMappings.CardNameMap[CardNames.ClichéRaciste] ||
               (handCard.Title == CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive] && currentDialogIndex > StartHighlightMusiqueDeMegadriveIndex);
    }

    /// <summary>
    /// Creates visual representations for the provided cards.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void CreateCards(ObservableCollection<InGameCard> handCards)
    {
        foreach (var handCard in handCards)
        {
            var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
            newCard.transform.SetParent(transform, true);
            newCard.GetComponent<CardDisplay>().InGameCard = handCard;
            newCard.GetComponent<OnHover>().bIsInGame = true;

            if (ShouldHighlightCard(handCard))
            {
                newCard.AddComponent<HighLightCard>();
            }

            CreatedCards.Add(newCard);
        }

        AdjustRectTransformSize(handCards.Count);
    }

    /// <summary>
    /// Clears any existing card representations and then recreates them for the provided cards.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void RebuildHandDisplay(ObservableCollection<InGameCard> handCards)
    {
        ClearCreatedCards();
        CreateCards(handCards);
    }

    /// <summary>
    /// Determines if the hand cards should be displayed.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    /// <returns>True if the cards should be displayed, false otherwise.</returns>
    private bool ShouldDisplayHandCard(ObservableCollection<InGameCard> handCards)
    {
        return handCards.Count == 0 || IsCurrentPlayerTurn(handCards[0]);
    }

    /// <summary>
    /// Displays the hand cards based on certain conditions.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void DisplayHandCard(ObservableCollection<InGameCard> handCards)
    {
        if (ShouldDisplayHandCard(handCards))
        {
            RebuildHandDisplay(handCards);
        }
    }
}
using System;
using System.Collections.ObjectModel;
using Cards;
using UnityEngine;

public class TutoHandCardDisplay : HandCardDisplay
{

    private int currentDialogIndex = 0;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Subscribes to relevant events.
    /// </summary>
    private void Awake()
    {
        SubscribeToEvents();
    }
    
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

    private void OnDestroy()
    {
        ClearCreatedCards();
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        HandCardChange.AddListener(DisplayHandCard);
        DialogueUI.DialogIndex.AddListener(SavedIndexDialog);
    }

    /// <summary>
    /// Unsubscribes from hand card change events.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        HandCardChange.RemoveListener(DisplayHandCard);
        DialogueUI.DialogIndex.RemoveListener(SavedIndexDialog);
    }
    
    private void SavedIndexDialog(int index)
    {
        currentDialogIndex = index;
        if (index > 35)
        {
            var card = CreatedCards.Find(card => card.GetComponent<CardDisplay>().Card.Title == CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive]);
            if (card != null)
            {
                card.AddComponent<HighLightCard>();
            }
        }
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

            if (handCard.Title == CardNameMappings.CardNameMap[CardNames.ClichéRaciste])
            {
                newCard.AddComponent<HighLightCard>();
            }
            
            if (handCard.Title == CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive] && currentDialogIndex > 35)
            {
                newCard.AddComponent<HighLightCard>();
            }

            CreatedCards.Add(newCard);
        }

        AdjustRectTransformSize(handCards.Count);
    }
    
    /// <summary>
    /// Clears and then creates visual representations for the provided cards.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void BuildCards(ObservableCollection<InGameCard> handCards)
    {
        ClearCreatedCards();
        CreateCards(handCards);
    }

    private void DisplayHandCard(ObservableCollection<InGameCard> handCards)
    {
        if (handCards.Count == 0 || IsCurrentPlayerTurn(handCards[0]))
        {
            BuildCards(handCards);
        }
    }
}
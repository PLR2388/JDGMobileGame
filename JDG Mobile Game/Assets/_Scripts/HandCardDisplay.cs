using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cards;
using UnityEngine;using UnityEngine.Events;

[Serializable]
public class HandCardChangeEvent : UnityEvent<ObservableCollection<InGameCard>>
{
}

public class HandCardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    
    private List<GameObject> createdCards = new();

    public static readonly HandCardChangeEvent HandCardChange = new();

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Subscribes to relevant events.
    /// </summary>
    private void Awake()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Displays hand cards if they belong to the current player.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    private void DisplayHandCard(ObservableCollection<InGameCard> handCards)
    {
        if (handCards.Count == 0 || IsCurrentPlayerTurn(handCards[0]))
        {
            BuildCards(handCards);
        }
    }
    
    /// <summary>
    /// Checks if the provided card belongs to the current player.
    /// </summary>
    /// <param name="card">In-game card to check.</param>
    /// <returns>True if card belongs to current player; otherwise, false.</returns>
    private bool IsCurrentPlayerTurn(InGameCard card)
    {
        return GameStateManager.Instance.IsP1Turn == (card.CardOwner == CardOwner.Player1);
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

            createdCards.Add(newCard);
        }

        AdjustRectTransformSize(handCards.Count);
    }
    
    /// <summary>
    /// Adjusts the RectTransform size based on the number of cards.
    /// </summary>
    /// <param name="cardCount">Number of cards.</param>
    private void AdjustRectTransformSize(int cardCount)
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(420 * cardCount, rectTransform.sizeDelta.y);
    }
    
    /// <summary>
    /// Destroys created card game objects and clears the list.
    /// </summary>
    private void ClearCreatedCards()
    {
        foreach (var createdCard in createdCards)
        {
            Destroy(createdCard);
        }
        createdCards.Clear();
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
        UnsubscribeFromEvents();
        ClearCreatedCards();
    }
    
    /// <summary>
    /// Subscribes to hand card change events.
    /// </summary>
    private void SubscribeToEvents()
    {
        HandCardChange.AddListener(DisplayHandCard);
    }

    /// <summary>
    /// Unsubscribes from hand card change events.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        HandCardChange.RemoveListener(DisplayHandCard);
    }
}
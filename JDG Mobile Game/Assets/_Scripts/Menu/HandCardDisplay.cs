using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cards;
using UnityEngine;using UnityEngine.Events;

[Serializable]
public class HandCardChangeEvent : UnityEvent<ObservableCollection<InGameCard>>
{
}

public class HandCardDisplay : MonoBehaviour
{
    [SerializeField] protected GameObject prefabCard;
    
    protected readonly List<GameObject> CreatedCards = new List<GameObject>();

    public static readonly HandCardChangeEvent HandCardChange = new HandCardChangeEvent();

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
    protected bool IsCurrentPlayerTurn(InGameCard card)
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

            CreatedCards.Add(newCard);
        }

        AdjustRectTransformSize(handCards.Count);
    }
    
    /// <summary>
    /// Adjusts the RectTransform size based on the number of cards.
    /// </summary>
    /// <param name="cardCount">Number of cards.</param>
    protected void AdjustRectTransformSize(int cardCount)
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(420 * cardCount, rectTransform.sizeDelta.y);
    }
    
    /// <summary>
    /// Destroys created card game objects and clears the list.
    /// </summary>
    protected void ClearCreatedCards()
    {
        foreach (var createdCard in CreatedCards)
        {
            Destroy(createdCard);
        }
        CreatedCards.Clear();
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
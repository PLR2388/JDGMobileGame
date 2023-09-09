using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Generate GameObject to be used in CardSelector
/// It's used as a GameObject to store Image cards
/// when they are not used in a card selector
/// </summary>
public class CardPoolManager : StaticInstance<CardPoolManager>
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] public Transform cardPoolHolder;
    
    private readonly List<GameObject> pooledCards = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        // Must be done before PlayerCards
        BuildPooledCards();
    }
    
    private void BuildPooledCards()
    {
        foreach (var inGameCard in GameState.Instance.deckP1)
        {
            BuildNewCard(inGameCard);
        }
        foreach (var inGameCard in GameState.Instance.deckP2)
        {
            BuildNewCard(inGameCard);
        }
        var playerCards = FindObjectsOfType<PlayerCards>();
        foreach (var playerCard in playerCards)
        {
            playerCard.BuildPlayer();
            BuildNewCard(playerCard.Player);
        }
    }
    
    private void BuildNewCard(InGameCard inGameCard)
    {
        var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
        newCard.GetComponent<CardDisplay>().InGameCard = inGameCard;
        newCard.SetActive(false);
        newCard.transform.SetParent(cardPoolHolder, true);
        pooledCards.Add(newCard);
    }

    private void CleanPooledCards()
    {
        foreach (var card in pooledCards)
        {
            Destroy(card);
        }
    }
    
    public GameObject GetPooledObject(InGameCard inGameCard)
    {
        if (inGameCard == null)
        {
            return null;
        }
        else
        {
            return pooledCards.Find(cardGameObject =>
            {
                var card = cardGameObject.GetComponent<CardDisplay>().InGameCard;
                return inGameCard.CardOwner == card.CardOwner && inGameCard.Title == card.Title;
            });
        }
    }

    private void OnDestroy()
    {
        CleanPooledCards();
    }
}

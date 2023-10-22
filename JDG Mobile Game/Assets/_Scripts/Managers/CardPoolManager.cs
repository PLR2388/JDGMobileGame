using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Generates GameObjects to be used in CardSelector.
/// Serves as a pool to store Image card GameObjects when they are not actively used in a card selector.
/// </summary>
public class CardPoolManager : StaticInstance<CardPoolManager>
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] public Transform cardPoolHolder;
    
    private readonly List<GameObject> pooledCards = new List<GameObject>();

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the card pool.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        InitializeCardPool();
    }

    /// <summary>
    /// Initializes the card pool by adding cards from both decks and building player cards.
    /// </summary>
    private void InitializeCardPool()
    {
        AddCardsToPool(GameState.Instance.Player1DeckCards);
        AddCardsToPool(GameState.Instance.Player2DeckCards);
        BuildPlayerCards();
    }

    /// <summary>
    /// Adds all the cards from the given deck to the card pool.
    /// </summary>
    private void AddCardsToPool(IEnumerable<InGameCard> deck)
    {
        foreach (var inGameCard in deck)
        {
            BuildNewCard(inGameCard);
        }
    }

    /// <summary>
    /// Builds player cards and adds them to the card pool.
    /// </summary>
    private void BuildPlayerCards()
    {
        var playerCards = FindObjectsOfType<PlayerCards>();
        foreach (var playerCard in playerCards)
        {
            playerCard.BuildPlayer();
            BuildNewCard(playerCard.Player);
        }
    }

    /// <summary>
    /// Creates a new card GameObject from the provided InGameCard and adds it to the card pool.
    /// </summary>
    private void BuildNewCard(InGameCard inGameCard)
    {
        var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity, cardPoolHolder);
        newCard.GetComponent<CardDisplay>().InGameCard = inGameCard;
        newCard.SetActive(false);
        pooledCards.Add(newCard);
    }

    /// <summary>
    /// Destroys all the GameObjects in the card pool.
    /// </summary>
    private void CleanPooledCards()
    {
        foreach (var card in pooledCards)
        {
            Destroy(card);
        }
    }

    /// <summary>
    /// Retrieves a pooled card GameObject that matches the provided InGameCard.
    /// </summary>
    public GameObject GetPooledObject(InGameCard inGameCard)
    {
        return inGameCard == null 
            ? null 
            : pooledCards.Find(cardGameObject => CardMatches(inGameCard, cardGameObject));
    }

    /// <summary>
    /// Checks if the provided card GameObject matches the given InGameCard.
    /// </summary>
    private bool CardMatches(InGameCard inGameCard, GameObject cardGameObject)
    {
        var card = cardGameObject.GetComponent<CardDisplay>().InGameCard;
        return inGameCard.CardOwner == card.CardOwner && inGameCard.Title == card.Title;
    }

    /// <summary>
    /// Called when the MonoBehaviour will be destroyed.
    /// Cleans up the pooled card GameObjects.
    /// </summary>
    private void OnDestroy()
    {
        CleanPooledCards();
    }
}

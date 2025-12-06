using System.Collections.Generic;
using Cards;
using UnityEngine;
using VContainer;

/// <summary>
/// Generates GameObjects to be used in CardSelector.
/// Serves as a pool to store Image card GameObjects when they are not actively used in a card selector.
/// Phase 17-18: Removed GameState singleton and FindObjectsOfType dependencies.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// </summary>
public class CardPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] public Transform cardPoolHolder;

    private readonly List<GameObject> pooledCards = new List<GameObject>();

    // Phase 17-18: Injected dependencies
    private IDeckManagementService _deckManagementService;
    private IDeckInitializationService _deckInitializationService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 17-18: Inject IDeckManagementService instead of GameState.Instance.
    /// </summary>
    [Inject]
    public void Construct(IDeckManagementService deckManagementService, IDeckInitializationService deckInitializationService)
    {
        _deckManagementService = deckManagementService;
        _deckInitializationService = deckInitializationService;
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the card pool.
    /// Phase 19-20: No longer calls base.Awake() since not a singleton.
    /// </summary>
    private void Awake()
    {
        // Initialization moved to Start() to ensure DI has completed
    }

    /// <summary>
    /// Called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        InitializeCardPool();
    }

    /// <summary>
    /// Initializes the card pool by adding cards from both decks and building player cards.
    /// </summary>
    private void InitializeCardPool()
    {
        if (_deckManagementService == null)
        {
            Debug.LogError("CardPoolManager: DeckManagementService not injected!");
            return;
        }

        AddCardsToPool(_deckManagementService.Player1DeckCards);
        AddCardsToPool(_deckManagementService.Player2DeckCards);
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
    /// Phase 17-18: Temporarily disabled FindObjectsOfType pattern.
    /// TODO Phase 21: Refactor when PlayerCards business logic is extracted.
    /// </summary>
    private void BuildPlayerCards()
    {
        // Phase 17-18: FindObjectsOfType removed - this will be refactored in Phase 21
        // when PlayerCards MonoBehaviour business logic is extracted to services.
        // For now, the card pool is initialized with deck cards only.
        // Player entity cards (player avatars) will be handled differently after
        // PlayerCards refactoring is complete.
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

using System.Collections.Generic;
using Cards;
using JDG.Infrastructure.Cards;
using UnityEngine;
using VContainer;

/// <summary>
/// Generates GameObjects to be used in CardSelector.
/// Serves as a pool to store Image card GameObjects when they are not actively used in a card selector.
/// Phase 17-18: Removed GameState singleton and FindObjectsOfType dependencies.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// Phase 138: Added IObjectResolver to inject dynamically instantiated card components.
/// </summary>
public class CardPoolManager : MonoBehaviour
{
    // Resource paths for prefabs (avoid magic strings)
    private const string CardPrefabResourcePath = "Prefabs/Card";
    private const string PhysicalCardPrefabResourcePath = "Prefabs/PhysicalCard";

    [SerializeField] private GameObject prefabCard;
    [SerializeField] private Transform _cardPoolHolder;

    /// <summary>
    /// Transform to hold pooled card GameObjects.
    /// Phase 47: Added auto-find fallback for cardPoolHolder child.
    /// </summary>
    public Transform cardPoolHolder
    {
        get
        {
            if (_cardPoolHolder == null)
            {
                // Try to find a child named "CardPoolHolder"
                _cardPoolHolder = transform.Find("CardPoolHolder");
                if (_cardPoolHolder == null)
                {
                    // Create one if not found
                    var holder = new GameObject("CardPoolHolder");
                    holder.transform.SetParent(transform);
                    _cardPoolHolder = holder.transform;
                }
            }
            return _cardPoolHolder;
        }
    }

    [Header("Physical Card Prefab (for game board)")]
    [SerializeField] private GameObject physicalCardPrefab;

    /// <summary>
    /// Exposes the UI card prefab (with CardDisplay) for card pool/selection.
    /// Phase 47: Added Resources fallback if not assigned in Inspector.
    /// </summary>
    public GameObject PrefabCard
    {
        get
        {
            if (prefabCard == null)
            {
                prefabCard = Resources.Load<GameObject>(CardPrefabResourcePath);
                if (prefabCard == null)
                {
                    Debug.LogError($"CardPoolManager: PrefabCard not found in Resources/{CardPrefabResourcePath}. " +
                        "Please assign it in the Inspector or add to Resources folder.");
                }
            }
            return prefabCard;
        }
    }

    /// <summary>
    /// Exposes the physical card prefab (with PhysicalCardDisplay) for game board cards.
    /// Phase 46: Added separate prefab for physical cards on the game board.
    /// Phase 47: Added Resources fallback if not assigned in Inspector.
    /// </summary>
    public GameObject PhysicalCardPrefab
    {
        get
        {
            if (physicalCardPrefab == null)
            {
                physicalCardPrefab = Resources.Load<GameObject>(PhysicalCardPrefabResourcePath);
                if (physicalCardPrefab == null)
                {
                    Debug.LogError($"CardPoolManager: PhysicalCardPrefab not found in Resources/{PhysicalCardPrefabResourcePath}. " +
                        "Please assign it in the Inspector or add to Resources folder.");
                }
            }
            return physicalCardPrefab;
        }
    }

    private readonly List<GameObject> pooledCards = new List<GameObject>();

    // Phase 17-18: Injected dependencies
    private IDeckManagementService _deckManagementService;

    // Phase 138: Container reference for injecting dynamically instantiated components
    private IObjectResolver _container;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 17-18: Inject IDeckManagementService instead of GameState.Instance.
    /// Phase 46: Removed unused IDeckInitializationService to fix circular dependency.
    /// Phase 138: Added IObjectResolver for injecting dynamically instantiated card components.
    /// Phase 156: Added null checks to fail early if DI is not properly configured.
    /// </summary>
    [Inject]
    public void Construct(IDeckManagementService deckManagementService, IObjectResolver container)
    {
        _deckManagementService = deckManagementService ?? throw new System.ArgumentNullException(
            nameof(deckManagementService),
            "CardPoolManager requires IDeckManagementService. Ensure it is registered in the DI container.");
        _container = container ?? throw new System.ArgumentNullException(
            nameof(container),
            "CardPoolManager requires IObjectResolver for injecting dynamically instantiated card components.");
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
    /// Note: PlayerCards MonoBehaviour extraction deferred - current pattern works correctly.
    /// </summary>
    private void BuildPlayerCards()
    {
        // Phase 17-18: FindObjectsOfType removed.
        // Card pool is initialized with deck cards via GetDeckCards() in Start().
        // Player entity cards are handled by SummonPlayerEntityUseCase.
        // No further refactoring required - current architecture is stable.
    }

    /// <summary>
    /// Creates a new card GameObject from the provided InGameCard and adds it to the card pool.
    /// Phase 138: Injects OnHover and CardDisplay components after instantiation to fix DI.
    /// </summary>
    private void BuildNewCard(InGameCard inGameCard)
    {
        if (PrefabCard == null)
        {
            Debug.LogError("CardPoolManager.BuildNewCard: PrefabCard is null, cannot create card");
            return;
        }

        var newCard = Instantiate(PrefabCard, Vector3.zero, Quaternion.identity, cardPoolHolder);

        // Phase 138: Inject dynamically instantiated components
        // OnHover needs ICardSelectionService and IEventBus for card selection to work
        // CardDisplay needs ICardVisualService for proper rendering
        if (_container != null)
        {
            var onHover = newCard.GetComponent<OnHover>();
            if (onHover != null)
            {
                _container.Inject(onHover);
            }

            var cardDisplay = newCard.GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                _container.Inject(cardDisplay);
                cardDisplay.InGameCard = inGameCard;
            }
            else
            {
                Debug.LogError($"CardPoolManager.BuildNewCard: CardDisplay component not found on prefab for card '{inGameCard?.Title ?? "Unknown"}'");
            }
        }
        else
        {
            // Fallback: just set InGameCard without injection (legacy behavior)
            var cardDisplay = newCard.GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.InGameCard = inGameCard;
            }
            Debug.LogWarning("CardPoolManager.BuildNewCard: Container is null, components not injected");
        }

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
    /// Adds a card to the pool with a visual representation.
    /// Phase 140: Added for player entity registration.
    /// This method is called by PlayerCards.BuildPlayer() to ensure the Player entity
    /// is available in the pool for attack target selection.
    /// </summary>
    /// <param name="inGameCard">The card to add to the pool</param>
    public void AddCardToPool(InGameCard inGameCard)
    {
        if (inGameCard == null)
        {
            Debug.LogWarning("CardPoolManager.AddCardToPool: Cannot add null card to pool");
            return;
        }

        // Check if card is already in the pool
        if (GetPooledObject(inGameCard) != null)
        {
#if UNITY_EDITOR
            Debug.Log($"CardPoolManager.AddCardToPool: Card '{inGameCard.Title}' already in pool, skipping");
#endif
            return;
        }

#if UNITY_EDITOR
        Debug.Log($"CardPoolManager.AddCardToPool: Adding card '{inGameCard.Title}' (Owner: {inGameCard.CardOwner})");
#endif
        BuildNewCard(inGameCard);
    }

    /// <summary>
    /// Checks if the provided card GameObject matches the given InGameCard.
    /// </summary>
    private bool CardMatches(InGameCard inGameCard, GameObject cardGameObject)
    {
        if (cardGameObject == null) return false;

        var cardDisplay = cardGameObject.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.InGameCard == null) return false;

        var card = cardDisplay.InGameCard;
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

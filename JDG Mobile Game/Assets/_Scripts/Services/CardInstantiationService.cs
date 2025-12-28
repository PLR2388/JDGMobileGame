using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// Implementation of ICardInstantiationService that handles physical card GameObject creation.
/// Migrated from UnitManager singleton in Phase 17-18.
///
/// Phase 17-18: Migrated from UnitManager singleton.
/// Phase 19-20: Now injects CardPoolManager instead of using .Instance.
///
/// This service is responsible for:
/// - Instantiating card prefabs
/// - Setting correct rotation based on player (Player 1 cards are rotated 180°)
/// - Positioning cards in deck with slight offsets
/// - Maintaining a dictionary for card lookup by name
/// </summary>
public class CardInstantiationService : ICardInstantiationService
{
    private readonly GameObject _prefabCard;
    private readonly Dictionary<string, GameObject> _cardNameToGameObject;
    private readonly Quaternion _playerOneRotation = Quaternion.Euler(0, 180, 0);
    private const float PositionOffset = 0.1f;

    public CardInstantiationService(CardPoolManager cardPoolManager)
    {
        // Phase 46: Use PhysicalCardPrefab (with PhysicalCardDisplay) for game board cards
        if (cardPoolManager != null)
        {
            _prefabCard = cardPoolManager.PhysicalCardPrefab;
        }

        if (_prefabCard == null)
        {
            Debug.LogError("CardInstantiationService: PhysicalCardPrefab is null on CardPoolManager. " +
                "Assign the physical card prefab (with PhysicalCardDisplay component) in the Inspector.");
        }

        _cardNameToGameObject = new Dictionary<string, GameObject>();
        Debug.Log($"CardInstantiationService: Created instance (HashCode={GetHashCode()})");
    }

    /// <summary>
    /// Initializes physical card GameObjects for a deck.
    /// Each card is instantiated with correct rotation and stacked with slight Y offset.
    /// </summary>
    public void InitializePhysicalCards(
        List<InGameCard> deck,
        float deckLocationX,
        float deckLocationY,
        float deckLocationZ,
        bool isPlayerOne)
    {
        if (deck == null)
        {
            Debug.LogError("CardInstantiationService: Cannot initialize physical cards - deck is null");
            return;
        }

        if (_prefabCard == null)
        {
            Debug.LogError("CardInstantiationService: Cannot initialize physical cards - prefabCard is null");
            return;
        }

        var deckLocation = new Vector3(deckLocationX, deckLocationY, deckLocationZ);

        for (var i = 0; i < deck.Count; i++)
        {
            var card = deck[i];
            if (card == null)
            {
                Debug.LogWarning($"CardInstantiationService: Skipping null card at index {i}");
                continue;
            }

            var newPhysicalCard = Object.Instantiate(_prefabCard, deckLocation, Quaternion.identity);

            // Player 1 cards are rotated 180 degrees
            if (isPlayerOne)
            {
                newPhysicalCard.transform.rotation = _playerOneRotation;
            }

            // Stack cards with slight Y offset
            newPhysicalCard.transform.position = deckLocation + new Vector3(0, PositionOffset * i, 0);

            // Generate unique name and register
            var newPhysicalCardName = GenerateCardName(card, isPlayerOne);
            newPhysicalCard.name = newPhysicalCardName;

            var physicalCardDisplay = newPhysicalCard.GetComponent<PhysicalCardDisplay>();
            if (physicalCardDisplay != null)
            {
                physicalCardDisplay.Card = card;
            }
            else
            {
                Debug.LogError($"CardInstantiationService: PhysicalCardDisplay component missing on prefab for card {card.Title}");
            }

            if (_cardNameToGameObject.ContainsKey(newPhysicalCardName))
            {
                Debug.LogError($"CardInstantiationService: Duplicate card name '{newPhysicalCardName}'! Card already exists in dictionary.");
                Object.Destroy(newPhysicalCard);
                continue;
            }
            _cardNameToGameObject.Add(newPhysicalCardName, newPhysicalCard);
            Debug.Log($"CardInstantiationService: Added '{newPhysicalCardName}' (isPlayerOne={isPlayerOne}). Dictionary now has {_cardNameToGameObject.Count} entries.");
        }
    }

    /// <summary>
    /// Retrieves the GameObject for a specific card by name.
    /// </summary>
    public bool TryGetCardGameObject(string cardName, out GameObject cardGameObject)
    {
        bool found = _cardNameToGameObject.TryGetValue(cardName, out cardGameObject);
        if (!found)
        {
            // Log all entries to debug naming issues
            var allKeys = string.Join(", ", _cardNameToGameObject.Keys);
            Debug.LogWarning($"CardInstantiationService: Card '{cardName}' not found. " +
                $"Dictionary has {_cardNameToGameObject.Count} entries. Looking for P2 cards: " +
                string.Join(", ", System.Linq.Enumerable.Where(_cardNameToGameObject.Keys, k => k.EndsWith("P2"))));
        }
        return found;
    }

    /// <summary>
    /// Gets the count of cards currently registered in the dictionary.
    /// Useful for debugging to verify all cards were instantiated.
    /// </summary>
    public int GetDictionaryCount() => _cardNameToGameObject.Count;

    /// <summary>
    /// Generates a unique name for a card based on its title and player.
    /// Example: "Fisti" for Player 1 becomes "FistiP1"
    /// </summary>
    private string GenerateCardName(InGameCard card, bool isPlayerOne)
    {
        return card.Title + (isPlayerOne ? "P1" : "P2");
    }
}

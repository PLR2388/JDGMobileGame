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
        // Phase 19-20: Inject CardPoolManager and access prefab via reflection
        // We use reflection because prefabCard is a private serialized field that Unity sets from the scene
        if (cardPoolManager != null)
        {
            var field = typeof(CardPoolManager).GetField("prefabCard",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            _prefabCard = (GameObject)field?.GetValue(cardPoolManager);
        }

        if (_prefabCard == null)
        {
            Debug.LogError("CardInstantiationService: Could not access prefabCard from CardPoolManager");
        }

        _cardNameToGameObject = new Dictionary<string, GameObject>();
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
        var deckLocation = new Vector3(deckLocationX, deckLocationY, deckLocationZ);

        for (var i = 0; i < deck.Count; i++)
        {
            var newPhysicalCard = Object.Instantiate(_prefabCard, deckLocation, Quaternion.identity);

            // Player 1 cards are rotated 180 degrees
            if (isPlayerOne)
            {
                newPhysicalCard.transform.rotation = _playerOneRotation;
            }

            // Stack cards with slight Y offset
            newPhysicalCard.transform.position = deckLocation + new Vector3(0, PositionOffset * i, 0);

            // Generate unique name and register
            var newPhysicalCardName = GenerateCardName(deck[i], isPlayerOne);
            newPhysicalCard.name = newPhysicalCardName;
            newPhysicalCard.GetComponent<PhysicalCardDisplay>().Card = deck[i];
            _cardNameToGameObject.Add(newPhysicalCardName, newPhysicalCard);
        }
    }

    /// <summary>
    /// Retrieves the GameObject for a specific card by name.
    /// </summary>
    public bool TryGetCardGameObject(string cardName, out GameObject cardGameObject)
    {
        return _cardNameToGameObject.TryGetValue(cardName, out cardGameObject);
    }

    /// <summary>
    /// Generates a unique name for a card based on its title and player.
    /// Example: "Fisti" for Player 1 becomes "FistiP1"
    /// </summary>
    private string GenerateCardName(InGameCard card, bool isPlayerOne)
    {
        return card.Title + (isPlayerOne ? "P1" : "P2");
    }
}

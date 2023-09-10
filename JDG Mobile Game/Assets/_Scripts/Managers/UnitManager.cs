using System.Collections.Generic;
using Cards;
using UnityEngine;

/// <summary>
/// The UnitManager class is responsible for managing and instantiating units (cards) in the game.
/// It ensures that each card is placed and oriented correctly based on the player's side.
/// </summary>
public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] private GameObject prefabCard;

    private readonly Dictionary<string, GameObject> cardNameToGameObject = new Dictionary<string, GameObject>();

    public IReadOnlyDictionary<string, GameObject> CardNameToGameObject => cardNameToGameObject;

    private readonly Quaternion playerOneRotation = Quaternion.Euler(0, 180, 0);
    private const float PositionOffset = 0.1f;

    /// <summary>
    /// Initializes the physical card instances based on the provided deck.
    /// </summary>
    /// <param name="deck">A list of InGameCard objects representing the player's deck.</param>
    /// <param name="deckLocation">The starting position where the deck should be instantiated.</param>
    /// <param name="isPlayerOne">Indicates whether the cards belong to Player One. Affects card orientation.</param>
    public void InitPhysicalCards(List<InGameCard> deck, Vector3 deckLocation, bool isPlayerOne)
    {
        for (var i = 0; i < deck.Count; i++)
        {
            var newPhysicalCard = Instantiate(prefabCard, deckLocation, Quaternion.identity);

            if (isPlayerOne)
            {
                newPhysicalCard.transform.rotation = playerOneRotation;
            }

            newPhysicalCard.transform.position = deckLocation + new Vector3(0, PositionOffset * i, 0);

            var newPhysicalCardName = GenerateCardName(deck[i], isPlayerOne);
            newPhysicalCard.name = newPhysicalCardName;
            newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = deck[i];
            cardNameToGameObject.Add(newPhysicalCardName, newPhysicalCard);
        }
    }

    /// <summary>
    /// Generates a unique name for a card based on its title and the player it belongs to.
    /// </summary>
    /// <param name="card">The InGameCard object to generate a name for.</param>
    /// <param name="isPlayerOne">Indicates whether the card belongs to Player One. Affects the generated name.</param>
    /// <returns>A string representing the card's unique name.</returns>
    private string GenerateCardName(InGameCard card, bool isPlayerOne)
    {
        return card.Title + (isPlayerOne ? "P1" : "P2");
    }
}
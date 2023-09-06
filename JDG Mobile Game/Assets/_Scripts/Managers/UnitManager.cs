using System.Collections.Generic;
using Cards;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] private GameObject prefabCard;

    public readonly Dictionary<string, GameObject> CardNameToGameObject = new Dictionary<string, GameObject>();

    public void InitPhysicalCards(List<InGameCard> deck, bool isPlayerOne)
    {
        for (var i = 0; i < deck.Count; i++)
        {
            var deckLocation = CardLocation.GetDeckLocation(isPlayerOne);
            var newPhysicalCard = Instantiate(prefabCard, deckLocation, Quaternion.identity);
            if (isPlayerOne)
            {
                newPhysicalCard.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            newPhysicalCard.transform.position =
                new Vector3(deckLocation.x, deckLocation.y + 0.1f * i, deckLocation.z);
            var newPhysicalCardName = deck[i].Title + (isPlayerOne ? "P1" : "P2");
            newPhysicalCard.name = newPhysicalCardName;
            newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = deck[i];
            CardNameToGameObject.Add(newPhysicalCardName, newPhysicalCard);
        }
    }
}

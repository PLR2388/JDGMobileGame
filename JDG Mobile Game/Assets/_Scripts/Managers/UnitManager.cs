using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] private GameObject prefabCard;
    
    private List<GameObject> allPhysicalCards = new List<GameObject>();

    public List<GameObject> AllPhysicalCards => allPhysicalCards;

    public void InitPhysicalCards(List<InGameCard> deck, bool isPlayerOne)
    {
        for (var i = 0; i < deck.Count; i++)
        {
            var deckLocation = isPlayerOne ? CardLocation.deckLocationP1 : CardLocation.deckLocationP2;
            var newPhysicalCard = Instantiate(prefabCard, deckLocation, Quaternion.identity);
            if (isPlayerOne)
            {
                newPhysicalCard.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            newPhysicalCard.transform.position =
                new Vector3(deckLocation.x, deckLocation.y + 0.1f * i, deckLocation.z);
            newPhysicalCard.name = deck[i].Title + (isPlayerOne ? "P1" : "P2");
            newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = deck[i];
            AddPhysicalCard(newPhysicalCard);
        }
    }
        
    private void AddPhysicalCard(GameObject physicalCard)
    {
        allPhysicalCards.Add(physicalCard);
    }
}

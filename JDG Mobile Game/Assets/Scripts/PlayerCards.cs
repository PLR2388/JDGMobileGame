using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCards : MonoBehaviour
{
    [FormerlySerializedAs("Deck")] [SerializeField]
    public List<Card> deck;

    [SerializeField] public List<Card> handCards;

    [FormerlySerializedAs("YellowTrash")] [SerializeField]
    public List<Card> yellowTrash;

    [FormerlySerializedAs("Field")] [SerializeField]
    public FieldCard field;

    [FormerlySerializedAs("InvocationCards")] [SerializeField]
    public List<InvocationCard> invocationCards;

    [FormerlySerializedAs("EffectCards")] [SerializeField]
    public List<EffectCard> effectCards;

    [FormerlySerializedAs("IsPlayerOne")] [SerializeField]
    private bool isPlayerOne;

    [FormerlySerializedAs("PrefabCard")] [SerializeField]
    private GameObject prefabCard;

    [FormerlySerializedAs("AllPhysicalCards")] [SerializeField]
    private List<GameObject> allPhysicalCards;

    private readonly Vector3[] invocationCardsLocationP1 =
    {
        new Vector3(-53.5f, 0.5f, -21),
        new Vector3(-32, 0.5f, -21),
        new Vector3(-11.5f, 0.5f, -21),
        new Vector3(10, 0.5f, -21),
    };

    private readonly Vector3[] equipmentCardsLocationP1 =
    {
        new Vector3(-53.5f, 0.5f, -30),
        new Vector3(-32, 0.5f, -30),
        new Vector3(-11.5f, 0.5f, -30),
        new Vector3(10, 0.5f, -30),
    };

    private readonly Vector3[] effectCardsLocationP1 =
    {
        new Vector3(-53.5f, 0.5f, -46.7f),
        new Vector3(-32, 0.5f, -46.7f),
        new Vector3(-11.5f, 0.5f, -46.7f),
        new Vector3(10, 0.5f, -46.7f),
    };

    private readonly Vector3 fieldCardLocationP1 = new Vector3(31.5f, 0.5f, -21);
    private readonly Vector3 deckLocationP1 = new Vector3(52, 0.5f, -33);
    private readonly Vector3 yellowTrashLocationP1 = new Vector3(31.5f, 0.5f, -46.7f);

    private readonly Vector3[] invocationCardsLocationP2 =
    {
        new Vector3(53.5f, 0.5f, 21),
        new Vector3(32, 0.5f, 21),
        new Vector3(10.5f, 0.5f, 21),
        new Vector3(-11, 0.5f, 21),
    };

    private readonly Vector3[] equipmentCardsLocationP2 =
    {
        new Vector3(-53.5f, 0.5f, 30),
        new Vector3(-32, 0.5f, 30),
        new Vector3(-11.5f, 0.5f, 30),
        new Vector3(10, 0.5f, 30),
    };

    private readonly Vector3[] effectCardsLocationP2 =
    {
        new Vector3(53.5f, 0.5f, 46),
        new Vector3(32, 0.5f, 46),
        new Vector3(10.5f, 0.5f, 46),
        new Vector3(-11, 0.5f, 46),
    };

    private readonly Vector3 fieldCardLocationP2 = new Vector3(-32, 0.5f, 21);
    private readonly Vector3 deckLocationP2 = new Vector3(-53, 0.5f, 34);
    private readonly Vector3 yellowTrashLocationP2 = new Vector3(-32, 0.5f, 46);

    public string Tag => isPlayerOne ? "card1" : "card2";

    // Start is called before the first frame update
    private void Start()
    {
        invocationCards = new List<InvocationCard>(4);
        effectCards = new List<EffectCard>(4);
        var gameStateGameObject = GameObject.Find("GameState");
        var gameState = gameStateGameObject.GetComponent<GameState>();
        if (isPlayerOne)
        {
            deck = gameState.deckP1;
            for (var i = 0; i < deck.Count; i++)
            {
                var newPhysicalCard = Instantiate(prefabCard, deckLocationP1, Quaternion.identity);
                newPhysicalCard.transform.rotation = Quaternion.Euler(0, 180, 0);
                newPhysicalCard.transform.position =
                    new Vector3(deckLocationP1.x, deckLocationP1.y + 0.1f * i, deckLocationP1.z);
                newPhysicalCard.name = deck[i].Nom + "P1";
                newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = deck[i];
                allPhysicalCards.Add(newPhysicalCard);
            }
        }
        else
        {
            deck = gameState.deckP2;
            for (var i = 0; i < deck.Count; i++)
            {
                var newPhysicalCard = Instantiate(prefabCard, deckLocationP2, Quaternion.identity);
                newPhysicalCard.transform.position =
                    new Vector3(deckLocationP2.x, deckLocationP2.y + 0.1f * i, deckLocationP2.z);
                newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = deck[i];
                newPhysicalCard.name = deck[i].Nom + "P2";
                allPhysicalCards.Add(newPhysicalCard);
            }
        }

        for (var i = deck.Count - 5; i < deck.Count; i++)
        {
            handCards.Add(deck[i]);
            allPhysicalCards[i].transform.position = new Vector3(999, 999);
        }

        deck.RemoveRange(deck.Count - 5, 5);

        if (!isPlayerOne) return;
        {
            for (var i = 0; i < deck.Count; i++)
            {
                if (deck[i].Nom != "Le voisin") continue;
                handCards.Add(deck[i]);
                deck.Remove(deck[i]);
                break;
            }
        }
    }

    public void ResetInvocationCardNewTurn()
    {
        foreach (var invocationCard in invocationCards.Where(invocationCard =>
            invocationCard != null && invocationCard.Nom != null))
        {
            invocationCard.ResetNewTurn();
        }
    }

    public bool ContainsCardInInvocation(InvocationCard invocationCard)
    {
        return invocationCards.Any(invocation => invocation != null && invocation.Nom == invocationCard.Nom);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPlayerOne)
        {
            for (var i = 0; i < invocationCards.Count; i++)
            {
                if (!invocationCards[i]) continue;
                var invocationCard = invocationCards[i];
                var index = FindCard(invocationCard);
                allPhysicalCards[index].transform.position = invocationCardsLocationP1[i];
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card1";

                if (invocationCard.GETEquipmentCard() == null) continue;
                var equipmentCard = invocationCard.GETEquipmentCard();
                index = FindCard(equipmentCard);
                allPhysicalCards[index].transform.position = equipmentCardsLocationP1[i];
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card1";
            }

            for (var i = 0; i < effectCards.Count; i++)
            {
                if (!effectCards[i]) continue;
                var index = FindCard(effectCards[i]);
                allPhysicalCards[index].transform.position = effectCardsLocationP1[i];

                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card1";
            }

            for (var i = 0; i < deck.Count; i++)
            {
                if (!deck[i]) continue;
                var index = FindCard(deck[i]);
                allPhysicalCards[index].transform.position =
                    new Vector3(deckLocationP1.x, deckLocationP1.y + 0.1f * i, deckLocationP1.z);
            }

            for (var i = 0; i < yellowTrash.Count; i++)
            {
                if (!yellowTrash[i]) continue;
                var index = FindCard(yellowTrash[i]);
                allPhysicalCards[index].transform.position =
                    new Vector3(yellowTrashLocationP1.x, yellowTrashLocationP1.y + 0.1f * i, yellowTrashLocationP1.z);
                if (!allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = true;
                }
            }

            if (!field) return;
            {
                var index = FindCard(field);
                allPhysicalCards[index].transform.position = fieldCardLocationP1;
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card1";
            }
        }
        else
        {
            for (var i = 0; i < invocationCards.Count; i++)
            {
                if (!invocationCards[i]) continue;
                var invocationCard = invocationCards[i];
                var index = FindCard(invocationCards[i]);
                allPhysicalCards[index].transform.position = invocationCardsLocationP2[i];
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card2";
                if (invocationCard.GETEquipmentCard() == null) continue;
                var equipmentCard = invocationCard.GETEquipmentCard();
                index = FindCard(equipmentCard);
                allPhysicalCards[index].transform.position = equipmentCardsLocationP2[i];
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card2";
            }

            for (var i = 0; i < effectCards.Count; i++)
            {
                if (!effectCards[i]) continue;
                var index = FindCard(effectCards[i]);
                allPhysicalCards[index].transform.position = effectCardsLocationP2[i];

                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card2";
            }

            for (var i = 0; i < deck.Count; i++)
            {
                if (!deck[i]) continue;
                var index = FindCard(deck[i]);
                allPhysicalCards[index].transform.position =
                    new Vector3(deckLocationP2.x, deckLocationP2.y + 0.1f * i, deckLocationP2.z);
            }

            for (var i = 0; i < yellowTrash.Count; i++)
            {
                if (!yellowTrash[i]) continue;
                var index = FindCard(yellowTrash[i]);
                allPhysicalCards[index].transform.position =
                    new Vector3(yellowTrashLocationP2.x, yellowTrashLocationP2.y + 0.1f * i, yellowTrashLocationP2.z);
                if (!allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = true;
                }
            }

            if (!field) return;
            {
                var index = FindCard(field);
                allPhysicalCards[index].transform.position = fieldCardLocationP2;
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card2";
            }
        }
    }

    public void SendCardToYellowTrash(Card card)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Nom != card.Nom) continue;
            var invocationCard = card as InvocationCard;
            if (invocationCard == null) continue;
            if (invocationCard.GETEquipmentCard() != null)
            {
                var equipmentCard = invocationCard.GETEquipmentCard();
                yellowTrash.Add(equipmentCard);
                invocationCard.SetEquipmentCard(null);
            }

            invocationCards.Remove(card as InvocationCard);
        }

        for (var i = 0; i < effectCards.Count; i++)
        {
            if (effectCards[i].Nom == card.Nom)
            {
                effectCards.Remove(card as EffectCard);
            }
        }

        if (field != null && field.Nom == card.Nom)
        {
            field = null;
        }

        yellowTrash.Add(card);
    }

    public void SendCardToHand(Card card)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Nom == card.Nom)
            {
                invocationCards.Remove(card as InvocationCard);
            }
        }

        handCards.Add(card);
    }

    private int FindCard(Card card)
    {
        var cardName = card.Nom;
        if (isPlayerOne)
        {
            for (var i = 0; i < allPhysicalCards.Count; i++)
            {
                if ((cardName + "P1") == allPhysicalCards[i].name)
                {
                    return i;
                }
            }
        }
        else
        {
            for (var i = 0; i < allPhysicalCards.Count; i++)
            {
                if ((cardName + "P2") == (allPhysicalCards[i].name))
                {
                    return i;
                }
            }
        }

        return -1;
    }
}
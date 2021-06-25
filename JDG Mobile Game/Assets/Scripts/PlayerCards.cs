using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    [SerializeField] public List<Card> Deck;
    [SerializeField] public List<Card> handCards;
    [SerializeField] public List<Card> YellowTrash;
    [SerializeField] public FieldCard Field;
    [SerializeField] public List<InvocationCard> InvocationCards;
    [SerializeField] public List<EffectCard> EffectCards;
    [SerializeField] private bool IsPlayerOne;
    [SerializeField] private GameObject PrefabCard;
    [SerializeField] private List<GameObject> AllPhysicalCards;

    private Vector3[] invocationCardsLocationP1 =
    {
        new Vector3(-53.5f, 0.5f, -21),
        new Vector3(-32, 0.5f, -21),
        new Vector3(-11.5f, 0.5f, -21),
        new Vector3(10, 0.5f, -21),
    };

    private Vector3[] equipmentCardsLocationP1 =
    {
        new Vector3(-53.5f, 0.5f, -30),
        new Vector3(-32, 0.5f, -30),
        new Vector3(-11.5f, 0.5f, -30),
        new Vector3(10, 0.5f, -30),
    };

    private Vector3[] effectCardsLocationP1 =
    {
        new Vector3(-53.5f, 0.5f, -46.7f),
        new Vector3(-32, 0.5f, -46.7f),
        new Vector3(-11.5f, 0.5f, -46.7f),
        new Vector3(10, 0.5f, -46.7f),
    };

    private Vector3 FieldCardLocationP1 = new Vector3(31.5f, 0.5f, -21);
    private Vector3 DeckLocationP1 = new Vector3(52, 0.5f, -33);
    private Vector3 YellowTrashLocationP1 = new Vector3(31.5f, 0.5f, -46.7f);

    private Vector3[] invocationCardsLocationP2 =
    {
        new Vector3(53.5f, 0.5f, 21),
        new Vector3(32, 0.5f, 21),
        new Vector3(10.5f, 0.5f, 21),
        new Vector3(-11, 0.5f, 21),
    };

    private Vector3[] equipmentCardsLocationP2 =
    {
        new Vector3(-53.5f, 0.5f, 30),
        new Vector3(-32, 0.5f, 30),
        new Vector3(-11.5f, 0.5f, 30),
        new Vector3(10, 0.5f, 30),
    };

    private Vector3[] effectCardsLocationP2 =
    {
        new Vector3(53.5f, 0.5f, 46),
        new Vector3(32, 0.5f, 46),
        new Vector3(10.5f, 0.5f, 46),
        new Vector3(-11, 0.5f, 46),
    };

    private Vector3 FieldCardLocationP2 = new Vector3(-32, 0.5f, 21);
    private Vector3 DeckLocationP2 = new Vector3(-53, 0.5f, 34);
    private Vector3 YellowTrashLocationP2 = new Vector3(-32, 0.5f, 46);

    public string TAG
    {
        get
        {
            if (IsPlayerOne)
            {
                return "card1";
            }
            else
            {
                return "card2";
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvocationCards = new List<InvocationCard>(4);
        EffectCards = new List<EffectCard>(4);
        GameObject gameObject = GameObject.Find("GameState");
        GameState gameState = gameObject.GetComponent<GameState>();
        if (IsPlayerOne)
        {
            Deck = gameState.DeckP1;
            for (int i = 0; i < Deck.Count; i++)
            {
                GameObject newPhysicalCard = Instantiate(PrefabCard, DeckLocationP1, Quaternion.identity);
                newPhysicalCard.transform.rotation = Quaternion.Euler(0, 180, 0);
                newPhysicalCard.transform.position =
                    new Vector3(DeckLocationP1.x, DeckLocationP1.y + 0.1f * i, DeckLocationP1.z);
                newPhysicalCard.name = Deck[i].Nom + "P1";
                newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = Deck[i];
                AllPhysicalCards.Add(newPhysicalCard);
            }
        }
        else
        {
            Deck = gameState.DeckP2;
            for (int i = 0; i < Deck.Count; i++)
            {
                GameObject newPhysicalCard = Instantiate(PrefabCard, DeckLocationP2, Quaternion.identity);
                newPhysicalCard.transform.position =
                    new Vector3(DeckLocationP2.x, DeckLocationP2.y + 0.1f * i, DeckLocationP2.z);
                newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = Deck[i];
                newPhysicalCard.name = Deck[i].Nom + "P2";
                AllPhysicalCards.Add(newPhysicalCard);
            }
        }

        for (int i = Deck.Count - 5; i < Deck.Count; i++)
        {
            handCards.Add(Deck[i]);
            AllPhysicalCards[i].transform.position = new Vector3(999, 999);
        }

        Deck.RemoveRange(Deck.Count - 5, 5);

        if (IsPlayerOne)
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                if (Deck[i].Nom == "Le voisin")
                {
                    handCards.Add(Deck[i]);
                    Deck.Remove(Deck[i]);
                    break;
                }
            }
        }
    }

    public void resetInvocationCardNewTurn()
    {
        foreach (var invocationCard in InvocationCards)
        {
            if (invocationCard != null && invocationCard.Nom != null)
            {
                invocationCard.resetNewTurn();
            }
        }
    }

    public bool containsCardInInvocation(InvocationCard invocationCard)
    {
        foreach (var invocation in InvocationCards)
        {
            if (invocation != null && invocation.Nom == invocationCard.Nom)
            {
                return true;
            }
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerOne)
        {
            for (int i = 0; i < InvocationCards.Count; i++)
            {
                if (InvocationCards[i])
                {
                    InvocationCard invocationCard = InvocationCards[i];
                    int index = FindCard(invocationCard);
                    AllPhysicalCards[index].transform.position = invocationCardsLocationP1[i];
                    if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                    }
                    AllPhysicalCards[index].tag = "card1";

                    if (invocationCard.getEquipmentCard() != null)
                    {
                        EquipmentCard equipmentCard = invocationCard.getEquipmentCard();
                        index = FindCard(equipmentCard);
                        AllPhysicalCards[index].transform.position = equipmentCardsLocationP1[i];
                        if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                        {
                            AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                        }

                        AllPhysicalCards[index].tag = "card1";
                    }
                }
            }

            for (int i = 0; i < EffectCards.Count; i++)
            {
                if (EffectCards[i])
                {
                    int index = FindCard(EffectCards[i]);
                    AllPhysicalCards[index].transform.position = effectCardsLocationP1[i];

                    if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                    }

                    AllPhysicalCards[index].tag = "card1";
                }
            }

            for (int i = 0; i < Deck.Count; i++)
            {
                if (Deck[i])
                {
                    int index = FindCard(Deck[i]);
                    AllPhysicalCards[index].transform.position =
                        new Vector3(DeckLocationP1.x, DeckLocationP1.y + 0.1f * i, DeckLocationP1.z);
                }
            }

            for (int i = 0; i < YellowTrash.Count; i++)
            {
                if (YellowTrash[i])
                {
                    int index = FindCard(YellowTrash[i]);
                    AllPhysicalCards[index].transform.position =
                        new Vector3(YellowTrashLocationP1.x, YellowTrashLocationP1.y + 0.1f * i, YellowTrashLocationP1.z);
                    if (!AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = true;
                    }   
                }
            }

            if (Field)
            {
                int index = FindCard(Field);
                AllPhysicalCards[index].transform.position = FieldCardLocationP1;
                if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                AllPhysicalCards[index].tag = "card1";
            }
        }
        else
        {
            for (int i = 0; i < InvocationCards.Count; i++)
            {
                if (InvocationCards[i])
                {
                    InvocationCard invocationCard = InvocationCards[i];
                    int index = FindCard(InvocationCards[i]);
                    AllPhysicalCards[index].transform.position = invocationCardsLocationP2[i];
                    if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                    }

                    AllPhysicalCards[index].tag = "card2"; 
                    if (invocationCard.getEquipmentCard() != null)
                    {
                        EquipmentCard equipmentCard = invocationCard.getEquipmentCard();
                        index = FindCard(equipmentCard);
                        AllPhysicalCards[index].transform.position = equipmentCardsLocationP2[i];
                        if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                        {
                            AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                        }

                        AllPhysicalCards[index].tag = "card2";
                    }
                }
            }

            for (int i = 0; i < EffectCards.Count; i++)
            {
                if (EffectCards[i])
                {
                    int index = FindCard(EffectCards[i]);
                    AllPhysicalCards[index].transform.position = effectCardsLocationP2[i];

                    if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                    }

                    AllPhysicalCards[index].tag = "card2";
                }
            }

            for (int i = 0; i < Deck.Count; i++)
            {
                if (Deck[i])
                {
                    int index = FindCard(Deck[i]);
                    AllPhysicalCards[index].transform.position =
                        new Vector3(DeckLocationP2.x, DeckLocationP2.y + 0.1f * i, DeckLocationP2.z);
                }
            }

            for (int i = 0; i < YellowTrash.Count; i++)
            {
                if (YellowTrash[i])
                {
                    int index = FindCard(YellowTrash[i]);
                    AllPhysicalCards[index].transform.position =
                        new Vector3(YellowTrashLocationP2.x, YellowTrashLocationP2.y + 0.1f * i, YellowTrashLocationP2.z);
                    if (!AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = true;
                    }
                }
            }

            if (Field)
            {
                int index = FindCard(Field);
                AllPhysicalCards[index].transform.position = FieldCardLocationP2;
                if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                AllPhysicalCards[index].tag = "card2";
            }
        }
    }

    public void sendCardToYellowTrash(Card card)
    {
        for (int i = 0; i < InvocationCards.Count; i++)
        {
            if (InvocationCards[i].Nom == card.Nom)
            {
                InvocationCard invocationCard = card as InvocationCard;
                if (invocationCard != null)
                {
                    if (invocationCard.getEquipmentCard() != null)
                    {
                        EquipmentCard equipmentCard = invocationCard.getEquipmentCard();
                        YellowTrash.Add(equipmentCard);
                    }
                    InvocationCards.Remove(card as InvocationCard);
                }
 
                
            }
        }

        for (int i = 0; i < EffectCards.Count; i++)
        {
            if (EffectCards[i].Nom == card.Nom)
            {
                EffectCards.Remove(card as EffectCard);
            }
        }

        if (Field != null && Field.Nom == card.Nom)
        {
            Field = null;
        }

        YellowTrash.Add(card);
    }

    public void sendCardToHand(Card card)
    {
        for (int i = 0; i < InvocationCards.Count; i++)
        {
            if ( InvocationCards[i].Nom == card.Nom)
            {
                InvocationCards.Remove(card as InvocationCard);
            }
        }

        handCards.Add(card);
    }

    int FindCard(Card card)
    {
        string CardName = card.Nom;
        if (IsPlayerOne)
        {
            for (int i = 0; i < AllPhysicalCards.Count; i++)
            {
                if ((CardName + "P1") == AllPhysicalCards[i].name)
                {
                    return i;
                }
            }
        }
        else
        {
            for (int i = 0; i < AllPhysicalCards.Count; i++)
            {
                if ((CardName + "P2") == (AllPhysicalCards[i].name))
                {
                    return i;
                }
            }
        }

        return -1;
    }
}
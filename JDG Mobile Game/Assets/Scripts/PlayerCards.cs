using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    [SerializeField] public List<Card> Deck;
    [SerializeField] public List<Card> handCards;
    [SerializeField] public List<Card> YellowTrash;
    [SerializeField] public FieldCard Field;
    [SerializeField] public InvocationCard[] InvocationCards;
    [SerializeField] public EffectCard[] EffectCards;
    [SerializeField] public EquipmentCard[] EquipmentCards;
    [SerializeField] private bool IsPlayerOne;
    [SerializeField] private GameObject PrefabCard;
    [SerializeField] private List<GameObject> AllPhysicalCards;
    
    private Vector3[] invocationCardsLocationP1= {
        new Vector3(-53.5f,0.5f,-21),
        new Vector3(-32,0.5f,-21),
        new Vector3(-11.5f,0.5f,-21),
        new Vector3(10,0.5f,-21),    
    };
    
    private Vector3[] equipmentCardsLocationP1= {
        new Vector3(-53.5f,0.5f,-30),
        new Vector3(-32,0.5f,-30),
        new Vector3(-11.5f,0.5f,-30),
        new Vector3(10,0.5f,-30),    
    };

    private Vector3[] effectCardsLocationP1 =
    {
        new Vector3(-53.5f, 0.5f, -46.7f),
        new Vector3(-32, 0.5f, -46.7f),
        new Vector3(-11.5f, 0.5f, -46.7f),
        new Vector3(10, 0.5f, -46.7f),
    };
    private Vector3 FieldCardLocationP1=new Vector3(31.5f,0.5f,-21);
    private Vector3 DeckLocationP1=new Vector3(52,0.5f,-33);
    private Vector3 YellowTrashLocationP1=new Vector3(31.5f,0.5f,-46.7f);
    
    private Vector3[] invocationCardsLocationP2= {
        new Vector3(53.5f,0.5f,21),
        new Vector3(32,0.5f,21),
        new Vector3(10.5f,0.5f,21),
        new Vector3(-11,0.5f,21),    
    };
    
    private Vector3[] equipmentCardsLocationP2= {
        new Vector3(-53.5f,0.5f,30),
        new Vector3(-32,0.5f,30),
        new Vector3(-11.5f,0.5f,30),
        new Vector3(10,0.5f,30),    
    };

    private Vector3[] effectCardsLocationP2 =
    {
        new Vector3(53.5f,0.5f,46),
        new Vector3(32,0.5f,46),
        new Vector3(10.5f,0.5f,46),
        new Vector3(-11,0.5f,46),   
    };
    private Vector3 FieldCardLocationP2=new Vector3(-32,0.5f,21);
    private Vector3 DeckLocationP2=new Vector3(-53,0.5f,34);
    private Vector3 YellowTrashLocationP2=new Vector3(-32,0.5f,46);

    // Start is called before the first frame update
    void Start()
    {
        InvocationCards=new InvocationCard[4];
        EffectCards=new EffectCard[4];
        EquipmentCards=new EquipmentCard[4];
        GameObject gameObject=GameObject.Find("GameState");
        GameState gameState = gameObject.GetComponent<GameState>();
        if (IsPlayerOne)
        {
            Deck = gameState.DeckP1;
            for (int i = 0; i < Deck.Count; i++)
            {
                GameObject newPhysicalCard = Instantiate(PrefabCard, DeckLocationP1, Quaternion.identity);
                newPhysicalCard.transform.rotation=Quaternion.Euler(0,180,0);
                newPhysicalCard.transform.position=new Vector3(DeckLocationP1.x,DeckLocationP1.y+0.1f*i,DeckLocationP1.z);
                newPhysicalCard.name = Deck[i].GetNom()+"P1";
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
                newPhysicalCard.transform.position=new Vector3(DeckLocationP2.x,DeckLocationP2.y+0.1f*i,DeckLocationP2.z);
                newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = Deck[i];
                newPhysicalCard.name = Deck[i].GetNom()+"P2";
                AllPhysicalCards.Add(newPhysicalCard);
            }
        }

        for (int i = Deck.Count - 5; i < Deck.Count; i++)
        {
            handCards.Add(Deck[i]);
            AllPhysicalCards[i].transform.position=new Vector3(999,999);
        }
        Deck.RemoveRange(Deck.Count-5,5);

        if (IsPlayerOne)
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                if (Deck[i].GetNom() == "Le voisin")
                {
                    handCards.Add(Deck[i]);
                    Deck.Remove(Deck[i]);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerOne)
        {

            for (int i = 0; i < InvocationCards.Length; i++)
            {
                if (InvocationCards[i])
                {
                    int index = FindCard(InvocationCards[i]);
                    AllPhysicalCards[index].transform.position = invocationCardsLocationP1[i];
                    if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                    }
                }
            }
            for (int i = 0; i < EffectCards.Length; i++)
            {
                if (EffectCards[i])
                {
                    int index = FindCard(EffectCards[i]);
                    AllPhysicalCards[index].transform.position = effectCardsLocationP1[i];
                }
            }
            
            for (int i = 0; i < EquipmentCards.Length; i++)
            {
                if (EquipmentCards[i])
                {
                    int index = FindCard(EquipmentCards[i]);
                    AllPhysicalCards[index].transform.position = equipmentCardsLocationP1[i];
                }
            }

            for (int i = 0; i < Deck.Count; i++)
            {
                int index = FindCard(Deck[i]);
                AllPhysicalCards[index].transform.position =
                    new Vector3(DeckLocationP1.x, DeckLocationP1.y + 0.1f * i, DeckLocationP1.z);
            }

            for (int i = 0; i < YellowTrash.Count; i++)
            {
                int index = FindCard(YellowTrash[i]);
                AllPhysicalCards[index].transform.position =
                    new Vector3(YellowTrashLocationP1.x, YellowTrashLocationP1.y + 0.1f * i, YellowTrashLocationP1.z);
            }

            if (Field)
            {
                int index = FindCard(Field);
                AllPhysicalCards[index].transform.position = FieldCardLocationP1;
            }
            
        }
        else
        {
            for (int i = 0; i < InvocationCards.Length; i++)
            {
                if (InvocationCards[i])
                {
                    int index = FindCard(InvocationCards[i]);
                    AllPhysicalCards[index].transform.position = invocationCardsLocationP2[i];
                    if (AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                    }
                }
            }
            for (int i = 0; i < EffectCards.Length; i++)
            {
                if (EffectCards[i])
                {
                    int index = FindCard(EffectCards[i]);
                    AllPhysicalCards[index].transform.position = effectCardsLocationP2[i];
                }
            }
            
            for (int i = 0; i < EquipmentCards.Length; i++)
            {
                if (EquipmentCards[i])
                {
                    int index = FindCard(EquipmentCards[i]);
                    AllPhysicalCards[index].transform.position = equipmentCardsLocationP2[i];
                }
            }

            for (int i = 0; i < Deck.Count; i++)
            {
                int index = FindCard(Deck[i]);
                AllPhysicalCards[index].transform.position =
                    new Vector3(DeckLocationP2.x, DeckLocationP2.y + 0.1f * i, DeckLocationP2.z);
            }

            for (int i = 0; i < YellowTrash.Count; i++)
            {
                int index = FindCard(YellowTrash[i]);
                AllPhysicalCards[index].transform.position =
                    new Vector3(YellowTrashLocationP2.x, YellowTrashLocationP2.y + 0.1f * i, YellowTrashLocationP2.z);
            }

            if (Field)
            {
                int index = FindCard(Field);
                AllPhysicalCards[index].transform.position = FieldCardLocationP2;
            }
        }
   
    }

    int FindCard(Card card)
    {
        string CardName = card.GetNom();
        if (IsPlayerOne)
        {
            for (int i = 0; i < AllPhysicalCards.Count; i++)
            {
                if ((CardName+"P1") == AllPhysicalCards[i].name)
                {
                    return i;
                }
            }
        }
        else
        {
            for (int i = 0; i < AllPhysicalCards.Count; i++)
            {
                if ((CardName+"P2") == (AllPhysicalCards[i].name))
                {
                    return i;
                }
            }
        }
        return -1;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField] public List<InvocationCard> invocationCards;

    [FormerlySerializedAs("EffectCards")] [SerializeField]
    public List<EffectCard> effectCards;

    [FormerlySerializedAs("IsPlayerOne")] [SerializeField]
    private bool isPlayerOne;

    [FormerlySerializedAs("PrefabCard")] [SerializeField]
    private GameObject prefabCard;

    [FormerlySerializedAs("AllPhysicalCards")] [SerializeField]
    private List<GameObject> allPhysicalCards;

    [SerializeField] private GameObject nextPhaseButton;
    [SerializeField] private GameObject inHandButton;
    [SerializeField] private Transform canvas;


    private List<Card> secretCards = new List<Card>(); // Where combine card go

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

    private readonly Vector3 secretHide = new Vector3(1000.0f, 1000.0f, 1000.0f);

    private readonly Vector3 fieldCardLocationP2 = new Vector3(-32, 0.5f, 21);
    private readonly Vector3 deckLocationP2 = new Vector3(-53, 0.5f, 34);
    private readonly Vector3 yellowTrashLocationP2 = new Vector3(-32, 0.5f, 46);

    public string Tag => isPlayerOne ? "card1" : "card2";

    private List<InvocationCard> oldInvocationCards = new List<InvocationCard>();
    private FieldCard oldField = null;
    private List<EffectCard> oldEffectCards = new List<EffectCard>();
    private List<Card> oldHandCards = new List<Card>();

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

    public void AddPhysicalCard(Card card, string playerName)
    {
        var newPhysicalCard = Instantiate(prefabCard, deckLocationP2, Quaternion.identity);
        newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = card;
        newPhysicalCard.name = card.Nom + playerName;
        allPhysicalCards.Add(newPhysicalCard);
    }

    public void RemoveSuperInvocation(Card superInvocationCard)
    {
        var index = FindCard(superInvocationCard);
        if (index > -1)
        {
            invocationCards.Remove(superInvocationCard as InvocationCard);
            var gameobject = allPhysicalCards[index];
            Destroy(gameobject);
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
        if (invocationCards.Count != oldInvocationCards.Count)
        {
            if (invocationCards.Count > oldInvocationCards.Count)
            {
                OnInvocationCardAdded(invocationCards.Last());
            }
            else
            {
                var removedInvocationCard = oldInvocationCards.Except(invocationCards).First();
                OnInvocationCardsRemoved(removedInvocationCard);
            }

            oldInvocationCards = new List<InvocationCard>(invocationCards);
            OnInvocationCardsChanged();
        }

        if (effectCards.Count != oldEffectCards.Count)
        {
            if (effectCards.Count > oldEffectCards.Count)
            {
                OnEffectCardsAdded(effectCards.Last());
            }

            oldEffectCards = new List<EffectCard>(effectCards);
        }

        if (field != oldField)
        {
            for (var i = effectCards.Count - 1; i >= 0; i--)
            {
                var effectCard = effectCards[i];
                var effectCardEffect = effectCard.GetEffectCardEffect();
                if (effectCardEffect != null)
                {
                    if (effectCardEffect.Keys.Contains(Effect.SameFamily))
                    {
                        if (field != null)
                        {
                            foreach (var invocationCard in invocationCards)
                            {
                                invocationCard.SetCurrentFamily(field.GETFamily());
                            }
                        }
                        else
                        {
                            effectCards.Remove(effectCard);
                            yellowTrash.Add(effectCard);
                        }
                    }
                }
            }

            OnFieldCardChanged(oldField);
            oldField = field;
        }

        if (oldHandCards.Count != handCards.Count)
        {
            foreach (var invocationCard in invocationCards)
            {
                if (invocationCard.GETEquipmentCard() == null ||
                    invocationCard.GETEquipmentCard().EquipmentPermEffect == null) continue;
                var permEffect = invocationCard.GETEquipmentCard().EquipmentPermEffect;
                if (permEffect.Keys.Contains(PermanentEffect.AddAtkBaseOnHandCards))
                {
                    var value = float.Parse(permEffect.Values[
                        permEffect.Keys.FindIndex(key => key == PermanentEffect.AddAtkBaseOnHandCards)]);
                    var newBonusAttack = invocationCard.GetBonusAttack() +
                                         (handCards.Count - oldHandCards.Count) * value;
                    invocationCard.SetBonusAttack(newBonusAttack);
                }
                else if (permEffect.Keys.Contains(PermanentEffect.AddDefBaseOnHandCards))
                {
                    var value = float.Parse(permEffect.Values[
                        permEffect.Keys.FindIndex(key => key == PermanentEffect.AddDefBaseOnHandCards)]);
                    var newBonusDefense = invocationCard.GetBonusDefense() +
                                          (handCards.Count - oldHandCards.Count) * value;
                    invocationCard.SetBonusDefense(newBonusDefense);
                }
            }

            oldHandCards = new List<Card>(handCards);
        }

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
                var effectCard = effectCards[i];
                if (!effectCard) continue;
                var index = FindCard(effectCards[i]);
                allPhysicalCards[index].transform.position = effectCardsLocationP1[i];

                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                if (effectCard.GetEffectCardEffect().Keys.Contains(Effect.SameFamily))
                {
                    for (var j = 0; j < invocationCards.Count; j++)
                    {
                        var invocationCard = invocationCards[i];
                        if (!invocationCard) continue;
                        if (field != null)
                        {
                            invocationCard.SetCurrentFamily(field.GETFamily());
                        }
                    }
                }

                allPhysicalCards[index].tag = "card1";
            }

            for (var i = 0; i < deck.Count; i++)
            {
                if (!deck[i]) continue;
                var index = FindCard(deck[i]);
                if (index == -1)
                {
                    print("Cannot find card " + deck[i].Nom);
                }
                else
                {
                    allPhysicalCards[index].transform.position =
                        new Vector3(deckLocationP1.x, deckLocationP1.y + 0.1f * i, deckLocationP1.z);
                }
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

            if (field)
            {
                var index = FindCard(field);
                allPhysicalCards[index].transform.position = fieldCardLocationP1;
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card1";
            }

            for (var i = 0; i < secretCards.Count; i++)
            {
                var index = FindCard(secretCards[i]);
                allPhysicalCards[index].transform.position = secretHide;
                allPhysicalCards[index].tag = "card1";
            }

            for (var i = 0; i < handCards.Count; i++)
            {
                var index = FindCard(handCards[i]);
                if (index == -1)
                {
                    print("Cannot find Card " + handCards[i].Nom);
                }
                else
                {
                    allPhysicalCards[index].transform.position = secretHide;
                    allPhysicalCards[index].tag = "card1";
                }
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

            if (field)
            {
                var index = FindCard(field);
                allPhysicalCards[index].transform.position = fieldCardLocationP2;
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = "card2";
            }

            for (var i = 0; i < secretCards.Count; i++)
            {
                var index = FindCard(secretCards[i]);
                allPhysicalCards[index].transform.position = secretHide;
                allPhysicalCards[index].tag = "card2";
            }

            for (var i = 0; i < handCards.Count; i++)
            {
                var index = FindCard(handCards[i]);
                allPhysicalCards[index].transform.position = secretHide;
                allPhysicalCards[index].tag = "card2";
            }
        }
    }

    public void SendToSecretHide(Card card)
    {
        secretCards.Add(card);
    }

    public void RemoveFromSecretHide(Card card)
    {
        secretCards.Remove(card);
    }

    public void SendCardToYellowTrash(Card card)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Nom != card.Nom) continue;
            var invocationCard = card as InvocationCard;
            if (invocationCard == null) continue;
            if (invocationCard is SuperInvocationCard)
            {
                var superInvocationCard = invocationCard as SuperInvocationCard;
                var invocationListCards = superInvocationCard.invocationCards;
                foreach (var cardFromList in invocationListCards)
                {
                    secretCards.Remove(cardFromList);
                    yellowTrash.Add(cardFromList);
                }

                invocationCards.Remove(invocationCard);
            }
            else
            {
                if (invocationCard.GETEquipmentCard() != null)
                {
                    var equipmentCard = invocationCard.GETEquipmentCard();
                    yellowTrash.Add(equipmentCard);
                    invocationCard.SetEquipmentCard(null);
                }

                invocationCards.Remove((InvocationCard)card);
                yellowTrash.Add(card);
            }
        }

        for (var i = 0; i < effectCards.Count; i++)
        {
            if (effectCards[i].Nom == card.Nom)
            {
                effectCards.Remove(card as EffectCard);
                yellowTrash.Add(card);
            }
        }

        if (field != null && field.Nom == card.Nom)
        {
            field = null;
            yellowTrash.Add(card);
        }
    }

    public void sendInvocationCardToYellowTrash(InvocationCard specificCardFound)
    {
        var equipmentCard = specificCardFound.GETEquipmentCard();
        specificCardFound.SetEquipmentCard(null);
        specificCardFound.SetBonusAttack(0);
        specificCardFound.SetBonusDefense(0);
        specificCardFound.SetRemainedAttackThisTurn(1);
        invocationCards.Remove(specificCardFound);
        yellowTrash.Add(specificCardFound);
        if (equipmentCard != null)
        {
            yellowTrash.Add(equipmentCard);
        }

        if (specificCardFound.GetInvocationDeathEffect() != null)
        {
            var invocationDeathEffect = specificCardFound.GetInvocationDeathEffect();
            var keys = invocationDeathEffect.Keys;
            var values = invocationDeathEffect.Values;

            var cardName = "";
            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case DeathEffect.GetSpecificCard:
                        cardName = values[i];
                        break;
                    case DeathEffect.GetCardSource:
                        GetCardSourceDeathEffect(specificCardFound, isPlayerOne, values, i, cardName);
                        break;
                    case DeathEffect.ComeBackToHand:
                        ComeBackToHandDeathEffect(specificCardFound, values[i]);
                        break;
                    case DeathEffect.KillAlsoOtherCard:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void ComeBackToHandDeathEffect(InvocationCard invocationCard, string value)
    {
        var isParsed = int.TryParse(value, out var number);
        if (isParsed)
        {
            if (invocationCard.GETNumberDeaths() > number)
            {
                sendInvocationCardToYellowTrash(invocationCard);
            }
            else
            {
                SendCardToHand(invocationCard);
            }
        }
        else
        {
            SendCardToHand(invocationCard);
        }
    }

    private void AskUserToAddCardInHand(string cardName, Card cardFound, bool isFound)
    {
        if (!isFound) return;

        nextPhaseButton.SetActive(false);
        inHandButton.SetActive(false);

        UnityAction positiveAction = () =>
        {
            handCards.Add(cardFound);
            deck.Remove(cardFound);
            nextPhaseButton.SetActive(true);
            inHandButton.SetActive(true);
        };

        UnityAction negativeAction = () =>
        {
            nextPhaseButton.SetActive(true);
            inHandButton.SetActive(true);
        };

        MessageBox.CreateSimpleMessageBox(canvas, "Carte en main",
            "Voulez-vous aussi ajouter " + cardName + " à votre main ?", positiveAction, negativeAction);
    }

    private void GetCardSourceDeathEffect(Card invocationCard, bool isP1Card, IReadOnlyList<string> values, int i,
        string cardName)
    {
        Card cardFound = null;
        var source = values[i];

        SendCardToYellowTrash(invocationCard);
        switch (source)
        {
            case "deck":
            {
                if (cardName != "")
                {
                    var isFound = false;
                    var j = 0;
                    while (j < deck.Count && !isFound)
                    {
                        if (deck[j].Nom == cardName)
                        {
                            isFound = true;
                            cardFound = deck[j];
                        }

                        j++;
                    }

                    AskUserToAddCardInHand(cardName, cardFound, isFound);
                }

                break;
            }
            case "trash":
            {
                var trash = yellowTrash;
                if (cardName != "")
                {
                    var isFound = false;
                    var j = 0;
                    while (j < trash.Count && !isFound)
                    {
                        if (trash[j].Nom == cardName)
                        {
                            isFound = true;
                            cardFound = trash[j];
                        }

                        j++;
                    }

                    AskUserToAddCardInHand(cardName, cardFound, isFound);
                }

                break;
            }
        }
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

        for (var i = 0; i < allPhysicalCards.Count; i++)
        {
            if ((cardName + "P1") == allPhysicalCards[i].name)
            {
                return i;
            }
            else if ((cardName + "P2") == allPhysicalCards[i].name)
            {
                return i;
            }
        }


        return -1;
    }

    public int GetIndexInvocationCard(string nameCard)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i].Nom == nameCard)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnInvocationCardAdded(InvocationCard newInvocationCard)
    {
        for (var j = invocationCards.Count - 1; j >= 0; j--)
        {
            var invocationCard = invocationCards[j];
            var permEffect = invocationCard.InvocationPermEffect;
            if (permEffect != null)
            {
                var keys = permEffect.Keys;
                var values = permEffect.Values;

                List<InvocationCard> invocationCardsToChange = new List<InvocationCard>();
                List<InvocationCard> sameFamilyInvocationCards = new List<InvocationCard>();
                bool mustHaveMiminumATKDEF = false;

                for (var i = 0; i < keys.Count; i++)
                {
                    switch (keys[i])
                    {
                        case PermEffect.GiveStat:
                        {
                            if (Enum.TryParse(values[i], out CardFamily cardFamily))
                            {
                                if (newInvocationCard.Nom == invocationCard.Nom)
                                {
                                    // Must catch up old cards
                                    foreach (var invocationCardToCheck in invocationCards)
                                    {
                                        if (invocationCardToCheck.Nom != newInvocationCard.Nom)
                                        {
                                            if (invocationCardToCheck.GetFamily().Contains(cardFamily))
                                            {
                                                invocationCardsToChange.Add(invocationCardToCheck);
                                            }
                                        }
                                    }
                                }
                                else if (newInvocationCard.GetFamily().Contains(cardFamily))
                                {
                                    invocationCardsToChange.Add(newInvocationCard);
                                }
                            }
                        }
                            break;
                        case PermEffect.Family:
                        {
                            if (Enum.TryParse(values[i], out CardFamily cardFamily))
                            {
                                if (newInvocationCard.Nom == invocationCard.Nom)
                                {
                                    // Must catch up old cards
                                    foreach (var invocationCardToCheck in invocationCards)
                                    {
                                        if (invocationCardToCheck.Nom != newInvocationCard.Nom)
                                        {
                                            if (invocationCardToCheck.GetFamily().Contains(cardFamily))
                                            {
                                                sameFamilyInvocationCards.Add(invocationCardToCheck);
                                            }
                                        }
                                    }
                                }
                                else if (newInvocationCard.GetFamily().Contains(cardFamily))
                                {
                                    sameFamilyInvocationCards.Add(newInvocationCard);
                                }
                            }
                        }
                            break;
                        case PermEffect.Condition:
                        {
                            switch (values[i])
                            {
                                case "2 ATK 2 DEF":
                                {
                                    for (var k = sameFamilyInvocationCards.Count - 1; k >= 0; k--)
                                    {
                                        var invocationCardToCheck = sameFamilyInvocationCards[k];
                                        if (invocationCardToCheck.GetAttack() != 2f ||
                                            invocationCardToCheck.GetDefense() != 2f)
                                        {
                                            sameFamilyInvocationCards.Remove(invocationCardToCheck);
                                        }
                                    }
                                }
                                    break;
                                case "Benzaie jeune":
                                {
                                    if (invocationCards.Any(invocationCardToCheck =>
                                            invocationCardToCheck.Nom == values[i]))
                                    {
                                        mustHaveMiminumATKDEF = true;
                                    }
                                }
                                    break;
                                default:
                                    break;
                            }
                        }
                            break;
                        case PermEffect.IncreaseAtk:
                        {
                            if (invocationCardsToChange.Count > 0)
                            {
                                foreach (var invocationCardToChange in invocationCardsToChange)
                                {
                                    var value = invocationCardToChange.GETBonusAttack() + float.Parse(values[i]);
                                    invocationCardToChange.SetBonusAttack(value);
                                }
                            }
                            else if (sameFamilyInvocationCards.Count > 0)
                            {
                                var newValue = invocationCard.GETBonusAttack() +
                                               float.Parse(values[i]) * sameFamilyInvocationCards.Count;
                                invocationCard.SetBonusAttack(newValue);
                            }
                            else if (mustHaveMiminumATKDEF)
                            {
                                var minValue = float.Parse(values[i]);
                                if (invocationCard.GetCurrentAttack() < minValue)
                                {
                                    var value = minValue - invocationCard.GetCurrentAttack();
                                    invocationCard.SetBonusAttack(value);
                                }
                            }
                        }
                            break;
                        case PermEffect.IncreaseDef:
                        {
                            if (invocationCardsToChange.Count > 0)
                            {
                                foreach (var invocationCardToChange in invocationCardsToChange)
                                {
                                    var value = invocationCardToChange.GetBonusDefense() + float.Parse(values[i]);
                                    invocationCardToChange.SetBonusDefense(value);
                                }
                            }
                            else if (sameFamilyInvocationCards.Count > 0)
                            {
                                var newValue = invocationCard.GetBonusDefense() +
                                               float.Parse(values[i]) * sameFamilyInvocationCards.Count;
                                invocationCard.SetBonusDefense(newValue);
                            }
                            else if (mustHaveMiminumATKDEF)
                            {
                                var minValue = float.Parse(values[i]);
                                if (invocationCard.GetCurrentDefense() < minValue)
                                {
                                    var value = minValue - invocationCard.GetCurrentDefense();
                                    invocationCard.SetBonusDefense(value);
                                }
                            }
                        }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        for (var i = effectCards.Count - 1; i >= 0; i--)
        {
            var effectCard = effectCards[i];
            var effectCardEffect = effectCard.GetEffectCardEffect();
            if (effectCardEffect != null)
            {
                if (effectCardEffect.Keys.Contains(Effect.SameFamily))
                {
                    if (field != null)
                    {
                        newInvocationCard.SetCurrentFamily(field.GETFamily());
                    }
                }
                else if (effectCardEffect.Keys.Contains(Effect.NumberAttacks))
                {
                    var value = int.Parse(
                        effectCardEffect.Values[
                            effectCardEffect.Keys.FindIndex(effect => effect == Effect.NumberAttacks)]);
                    newInvocationCard.SetRemainedAttackThisTurn(value);
                }
            }
        }

        if (field != null)
        {
            var fieldCardEffect = field.FieldCardEffect;

            var fieldKeys = fieldCardEffect.Keys;
            var fieldValues = fieldCardEffect.Values;

            var family = field.GETFamily();
            for (var i = 0; i < fieldKeys.Count; i++)
            {
                switch (fieldKeys[i])
                {
                    case FieldEffect.ATK:
                    {
                        var atk = float.Parse(fieldValues[i]);
                        if (newInvocationCard.GetFamily().Contains(family))
                        {
                            var newBonusAttack = newInvocationCard.GetBonusAttack() + atk;
                            newInvocationCard.SetBonusAttack(newBonusAttack);
                        }
                    }

                        break;
                    case FieldEffect.DEF:
                    {
                        var def = float.Parse(fieldValues[i]);
                        if (newInvocationCard.GetFamily().Contains(family))
                        {
                            var newBonusDefense = newInvocationCard.GetBonusDefense() + def;
                            newInvocationCard.SetBonusDefense(newBonusDefense);
                        }
                    }
                        break;
                    case FieldEffect.Change:
                    {
                        var names = fieldValues[i].Split(';');
                        if (names.Contains(newInvocationCard.Nom))
                        {
                            newInvocationCard.SetCurrentFamily(family);
                        }
                    }
                        break;
                }
            }
        }
    }

    private void OnInvocationCardsRemoved(InvocationCard removedInvocationCard)
    {
        for (var j = oldInvocationCards.Count - 1; j >= 0; j--)
        {
            var invocationCard = oldInvocationCards[j];
            var permEffect = invocationCard.InvocationPermEffect;
            var actionEffect = invocationCard.InvocationActionEffect;
            if (permEffect != null)
            {
                var keys = permEffect.Keys;
                var values = permEffect.Values;

                List<InvocationCard> invocationCardsToChange = new List<InvocationCard>();
                List<InvocationCard> sameFamilyInvocationCards = new List<InvocationCard>();

                bool mustHaveMiminumATKDEF = false;

                for (var i = 0; i < keys.Count; i++)
                {
                    switch (keys[i])
                    {
                        case PermEffect.GiveStat:
                        {
                            if (Enum.TryParse(values[i], out CardFamily cardFamily))
                            {
                                if (removedInvocationCard.Nom == invocationCard.Nom)
                                {
                                    // Delete itself everybody lose advantage
                                    foreach (var invocationCardToCheck in invocationCards)
                                    {
                                        if (invocationCardToCheck.Nom != removedInvocationCard.Nom)
                                        {
                                            if (invocationCardToCheck.GetFamily().Contains(cardFamily))
                                            {
                                                invocationCardsToChange.Add(invocationCardToCheck);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                            break;
                        case PermEffect.Family:
                        {
                            if (Enum.TryParse(values[i], out CardFamily cardFamily))
                            {
                                if (removedInvocationCard.Nom == invocationCard.Nom)
                                {
                                    // Delete itself so loose all advantage to itself
                                    foreach (var invocationCardToCheck in invocationCards)
                                    {
                                        if (invocationCardToCheck.Nom != removedInvocationCard.Nom)
                                        {
                                            if (invocationCardToCheck.GetFamily().Contains(cardFamily))
                                            {
                                                sameFamilyInvocationCards.Add(invocationCardToCheck);
                                            }
                                        }
                                    }
                                }
                                else if (removedInvocationCard.GetFamily().Contains(cardFamily))
                                {
                                    // Just loose 1 element
                                    sameFamilyInvocationCards.Add(removedInvocationCard);
                                }
                            }
                        }
                            break;
                        case PermEffect.Condition:
                        {
                            switch (values[i])
                            {
                                case "2 ATK 2 DEF":
                                {
                                    for (var k = sameFamilyInvocationCards.Count - 1; k >= 0; k--)
                                    {
                                        var invocationCardToCheck = sameFamilyInvocationCards[k];
                                        if (invocationCardToCheck.GetAttack() != 2f ||
                                            invocationCardToCheck.GetDefense() != 2f)
                                        {
                                            sameFamilyInvocationCards.Remove(invocationCardToCheck);
                                        }
                                    }
                                }
                                    break;
                                case "Benzaie jeune":
                                {
                                    if (removedInvocationCard.Nom == invocationCard.Nom)
                                    {
                                        mustHaveMiminumATKDEF = true;
                                    }
                                }
                                    break;
                                default:
                                    break;
                            }
                        }
                            break;
                        case PermEffect.IncreaseAtk:
                        {
                            if (invocationCardsToChange.Count > 0)
                            {
                                foreach (var invocationCardToChange in invocationCardsToChange)
                                {
                                    var value = invocationCardToChange.GETBonusAttack() - float.Parse(values[i]);
                                    invocationCardToChange.SetBonusAttack(value);
                                }
                            }
                            else if (sameFamilyInvocationCards.Count > 0)
                            {
                                var newValue = invocationCard.GETBonusAttack() -
                                               float.Parse(values[i]) * sameFamilyInvocationCards.Count;
                                invocationCard.SetBonusAttack(newValue);
                            }
                            else if (mustHaveMiminumATKDEF)
                            {
                                invocationCard.SetBonusAttack(0);
                            }
                        }
                            break;
                        case PermEffect.IncreaseDef:
                        {
                            if (invocationCardsToChange.Count > 0)
                            {
                                foreach (var invocationCardToChange in invocationCardsToChange)
                                {
                                    var value = invocationCardToChange.GetBonusDefense() - float.Parse(values[i]);
                                    invocationCardToChange.SetBonusDefense(value);
                                }
                            }
                            else if (sameFamilyInvocationCards.Count > 0)
                            {
                                var newValue = invocationCard.GetBonusDefense() -
                                               float.Parse(values[i]) * sameFamilyInvocationCards.Count;
                                invocationCard.SetBonusDefense(newValue);
                            }
                            else if (mustHaveMiminumATKDEF)
                            {
                                invocationCard.SetBonusDefense(0);
                            }
                        }
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (actionEffect != null)
            {
                var keys = actionEffect.Keys;
                var values = actionEffect.Values;

                float atk = 0;
                float def = 0;
                List<int> indexToDelete = new List<int>();

                for (var i = 0; i < keys.Count; i++)
                {
                    switch (keys[i])
                    {
                        case ActionEffect.GiveAtk:
                        {
                            atk = float.Parse(values[i]);
                        }
                            break;
                        case ActionEffect.GiveDef:
                        {
                            def = float.Parse(values[i]);
                        }
                            break;
                        case ActionEffect.Beneficiary:
                        {
                            if (removedInvocationCard.Nom == invocationCard.Nom)
                            {
                                var beneficiary = values[i];
                                foreach (var invocationCardToCheck in invocationCards)
                                {
                                    if (invocationCardToCheck.Nom != beneficiary) continue;
                                    var newBonusAttack = invocationCardToCheck.GetBonusAttack() - atk;
                                    var newBonusDef = invocationCardToCheck.GETBonusDefense() - def;
                                    invocationCardToCheck.SetBonusAttack(newBonusAttack);
                                    invocationCardToCheck.SetBonusDefense(newBonusDef);
                                    indexToDelete.Add(i);
                                    break;
                                }
                            }
                        }
                            break;
                        default:
                            break;
                    }
                }

                if (indexToDelete.Count > 0)
                {
                    indexToDelete.Reverse();
                    foreach (var index in indexToDelete)
                    {
                        actionEffect.Keys.RemoveAt(index);
                        actionEffect.Values.RemoveAt(index);
                    }
                }
            }
        }

        for (var i = effectCards.Count - 1; i >= 0; i--)
        {
            var effectCard = effectCards[i];
            var effectCardEffect = effectCard.GetEffectCardEffect();
            if (effectCardEffect != null)
            {
                if (effectCardEffect.Keys.Contains(Effect.SameFamily))
                {
                    if (field != null)
                    {
                        removedInvocationCard.SetCurrentFamily(null);
                    }
                }
            }
        }

        if (field != null)
        {
            var fieldCardEffect = field.FieldCardEffect;

            var fieldKeys = fieldCardEffect.Keys;
            var fieldValues = fieldCardEffect.Values;

            var family = field.GETFamily();
            for (var i = 0; i < fieldKeys.Count; i++)
            {
                switch (fieldKeys[i])
                {
                    /*case FieldEffect.ATK:
                    {
                        var atk = float.Parse(fieldValues[i]);
                        if (removedInvocationCard.GetFamily().Contains(family))
                        {
                            var newBonusAttack = removedInvocationCard.GetBonusAttack() - atk;
                            removedInvocationCard.SetBonusAttack(newBonusAttack);
                        }
                    }
                   
                        break;
                    case FieldEffect.DEF:
                    {
                        var def = float.Parse(fieldValues[i]);
                        if (removedInvocationCard.GetFamily().Contains(family))
                        {
                            var newBonusDefense = removedInvocationCard.GetBonusDefense() - def;
                            removedInvocationCard.SetBonusDefense(newBonusDefense);
                        }
                    }
                        break;*/
                    case FieldEffect.Change:
                    {
                        var names = fieldValues[i].Split(';');
                        if (names.Contains(removedInvocationCard.Nom))
                        {
                            removedInvocationCard.SetCurrentFamily(null);
                        }
                    }
                        break;
                }
            }
        }
    }

    private void OnInvocationCardsChanged()
    {
        for (var j = invocationCards.Count - 1; j >= 0; j--)
        {
            var invocationCard = invocationCards[j];
            var permEffect = invocationCard.InvocationPermEffect;
            if (permEffect != null)
            {
                var keys = permEffect.Keys;
                var values = permEffect.Values;

                List<InvocationCard> invocationCardsToChange = new List<InvocationCard>();

                for (var i = 0; i < keys.Count; i++)
                {
                    switch (keys[i])
                    {
                        case PermEffect.checkCardsOnField:
                        {
                            var isFound = false;
                            if (Enum.TryParse(values[i], out CardFamily cardFamily))
                            {
                                if (invocationCards
                                    .Where(otherInvocationCard => otherInvocationCard.Nom != invocationCard.Nom)
                                    .Any(otherInvocationCard => otherInvocationCard.GetFamily().Contains(cardFamily)))
                                {
                                    isFound = true;
                                }
                            }
                            else
                            {
                                var cards = values[i].Split(';');

                                if (invocationCards
                                    .Where(otherInvocationCard => otherInvocationCard.Nom != invocationCard.Nom)
                                    .Any(otherInvocationCard => cards.Contains(otherInvocationCard.Nom)))
                                {
                                    isFound = true;
                                }
                            }

                            if (!isFound)
                            {
                                sendInvocationCardToYellowTrash(invocationCard);
                            }
                        }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    private void OnEffectCardsAdded(EffectCard newEffectCard)
    {
    }

    private void OnFieldCardChanged(FieldCard oldFieldCard)
    {
        if (oldFieldCard != null)
        {
            var fieldCardEffect = oldFieldCard.FieldCardEffect;

            var fieldKeys = fieldCardEffect.Keys;
            var fieldValues = fieldCardEffect.Values;

            var family = field.GETFamily();
            for (var i = 0; i < fieldKeys.Count; i++)
            {
                switch (fieldKeys[i])
                {
                    case FieldEffect.ATK:
                    {
                        var atk = float.Parse(fieldValues[i]);
                        foreach (var invocationCard in invocationCards)
                        {
                            if (!invocationCard.GetFamily().Contains(family)) continue;
                            var newBonusAttack = invocationCard.GetBonusAttack() - atk;
                            invocationCard.SetBonusAttack(newBonusAttack);
                        }
                    }
                        break;
                    case FieldEffect.DEF:
                    {
                        var def = float.Parse(fieldValues[i]);
                        foreach (var invocationCard in invocationCards)
                        {
                            if (!invocationCard.GetFamily().Contains(family)) continue;
                            var newBonusDefense = invocationCard.GetBonusDefense() - def;
                            invocationCard.SetBonusDefense(newBonusDefense);
                        }
                    }
                        break;
                    case FieldEffect.Change:
                    {
                        var names = fieldValues[i].Split(';');
                        foreach (var invocationCard in invocationCards)
                        {
                            if (names.Contains(invocationCard.Nom))
                            {
                                invocationCard.SetCurrentFamily(null);
                            }
                        }
                    }
                        break;
                }
            }
        }
    }
}
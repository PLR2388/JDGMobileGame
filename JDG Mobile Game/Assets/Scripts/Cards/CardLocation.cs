using System;
using System.Collections.Generic;
using System.Linq;
using Cards.EffectCards;
using UnityEngine;

namespace Cards
{
    public class CardLocation : MonoBehaviour
    {
    
        [SerializeField] private GameObject prefabCard;
        [SerializeField] private GameObject player1;

        [SerializeField] private GameObject player2;

        private List<GameObject> allPhysicalCards = new List<GameObject>();

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
        private static readonly Vector3 deckLocationP1 = new Vector3(52, 0.5f, -33);
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
            new Vector3(53.5f, 0.5f, 30),
            new Vector3(32, 0.5f, 30),
            new Vector3(11.5f, 0.5f, 30),
            new Vector3(-10, 0.5f, 30),
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
        private static readonly Vector3 deckLocationP2 = new Vector3(-53, 0.5f, 34);
        private readonly Vector3 yellowTrashLocationP2 = new Vector3(-32, 0.5f, 46);

        private PlayerCards player1Cards;
        private PlayerCards player2Cards;

        // Start is called before the first frame update
        void Awake()
        {
            player1Cards = player1.GetComponent<PlayerCards>();
            player2Cards = player2.GetComponent<PlayerCards>();
        }

        public void InitPhysicalCards(List<Card> deck, bool isPlayerOne)
        {
            for (var i = 0; i < deck.Count; i++)
            {
                var deckLocation = isPlayerOne ? deckLocationP1 : deckLocationP2;
                var newPhysicalCard = Instantiate(prefabCard, deckLocation, Quaternion.identity);
                if (isPlayerOne)
                {
                    newPhysicalCard.transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                newPhysicalCard.transform.position =
                    new Vector3(deckLocation.x, deckLocation.y + 0.1f * i, deckLocation.z);
                newPhysicalCard.name = deck[i].Nom + (isPlayerOne ? "P1" : "P2");
                newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = deck[i];
                AddPhysicalCard(newPhysicalCard);
            }
        }

        // Update is called once per frame
        void Update()
        {
            DisplayCardsInPosition(player1Cards, true);
            DisplayCardsInPosition(player2Cards, false);
        }
    
        public void AddPhysicalCard(Card card, string playerName)
        {
            var newPhysicalCard = Instantiate(prefabCard, playerName == "P1" ? deckLocationP1 : deckLocationP2,
                Quaternion.identity);
            newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = card;
            newPhysicalCard.name = card.Nom + playerName;
            allPhysicalCards.Add(newPhysicalCard);
        }

        private void AddPhysicalCard(GameObject physicalCard)
        {
            allPhysicalCards.Add(physicalCard);
        }

        public void HideCards(List<Card> cards)
        {
            foreach (var card in cards)
            {
                var index1 = GetPhysicalCardIndex(card, card.CardOwner == CardOwner.Player1);
                allPhysicalCards[index1].transform.position = new Vector3(999, 999);
            }
        }

        private int GetPhysicalCardIndex(Card card, bool isPlayerOne)
        {
            var cardName = card.Nom + (isPlayerOne ? "P1" : "P2");

            for (var i = 0; i < allPhysicalCards.Count; i++)
            {
                if (cardName == allPhysicalCards[i].name)
                {
                    return i;
                }
            }

            return -1;
        }

        public void RemovePhysicalCard(Card card)
        {
            var currentIndex = GetPhysicalCardIndex(card, card.CardOwner == CardOwner.Player1);
            if (currentIndex >= 0)
            {
                var physicalCard = allPhysicalCards[currentIndex];
                allPhysicalCards.Remove(physicalCard);
                Destroy(physicalCard);
            }
        }

        // isPlayerOne is used to know if playerCard is from PlayerOne or PlayerTwo
        // CardOwner is used to know if physicalCard ends with P1 or P2
        private void DisplayCardsInPosition(PlayerCards playerCards, bool isPlayerOne)
        {
            var cardTag = isPlayerOne ? "card1" : "card2";
            var invocationCards = playerCards.invocationCards;
            var effectCards = playerCards.effectCards;
            var deck = playerCards.deck;
            var yellowTrash = playerCards.yellowTrash;
            var field = playerCards.field;
            var secretCards = playerCards.secretCards;
            for (var i = 0; i < invocationCards.Count; i++)
            {
                if (!invocationCards[i]) continue;
                var invocationCard = invocationCards[i];
                var index = GetPhysicalCardIndex(invocationCard, invocationCard.CardOwner == CardOwner.Player1);
                var invocationCardsLocation = isPlayerOne ? invocationCardsLocationP1[i] : invocationCardsLocationP2[i];
                allPhysicalCards[index].transform.position = invocationCardsLocation;
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = cardTag;

                if (invocationCard.GetEquipmentCard() == null) continue;
                var equipmentCard = invocationCard.GetEquipmentCard();
                index = GetPhysicalCardIndex(equipmentCard, equipmentCard.CardOwner == CardOwner.Player1);
                var equipmentCardsLocation = isPlayerOne ? equipmentCardsLocationP1[i] : equipmentCardsLocationP2[i];
                allPhysicalCards[index].transform.position = equipmentCardsLocation;
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = cardTag;
            }

            for (var i = 0; i < playerCards.effectCards.Count; i++)
            {
                var effectCard = effectCards[i];
                if (!effectCard) continue;
                var index = GetPhysicalCardIndex(effectCards[i], effectCard.CardOwner == CardOwner.Player1);
                var effectCardsLocation = isPlayerOne ? effectCardsLocationP1[i] : effectCardsLocationP2[i];
                allPhysicalCards[index].transform.position = effectCardsLocation;

                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                if (effectCard.GetEffectCardEffect().Keys.Contains(Effect.SameFamily))
                {
                    foreach (var invocationCard in invocationCards.Where(invocationCard => invocationCard)
                                 .Where(invocationCard => field != null && !playerCards.IsFieldDesactivate))
                    {
                        invocationCard.SetCurrentFamily(field.GetFamily());
                    }
                }

                allPhysicalCards[index].tag = cardTag;
            }

            for (var i = 0; i < deck.Count; i++)
            {
                if (!deck[i]) continue;
                var index = GetPhysicalCardIndex(deck[i],deck[i].CardOwner == CardOwner.Player1);
                if (index == -1)
                {
                    print("Cannot find card " + deck[i].Nom);
                }
                else
                {
                    var deckLocation = isPlayerOne ? deckLocationP1 : deckLocationP2;
                    allPhysicalCards[index].transform.position =
                        new Vector3(deckLocation.x, deckLocation.y + 0.1f * i, deckLocation.z);
                }
            }

            for (var i = 0; i < yellowTrash.Count; i++)
            {
                if (!yellowTrash[i]) continue;
                var index = GetPhysicalCardIndex(yellowTrash[i], yellowTrash[i].CardOwner == CardOwner.Player1);
                var yellowTrashLocation = isPlayerOne ? yellowTrashLocationP1 : yellowTrashLocationP2;
                allPhysicalCards[index].transform.position =
                    new Vector3(yellowTrashLocation.x, yellowTrashLocation.y + 0.1f * i, yellowTrashLocation.z);
                if (!allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = true;
                }
            }

            if (field)
            {
                var index = GetPhysicalCardIndex(field, field.CardOwner == CardOwner.Player1);
                var fieldCardLocation = isPlayerOne ? fieldCardLocationP1 : fieldCardLocationP2;
                allPhysicalCards[index].transform.position = fieldCardLocation;
                if (allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    allPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden = false;
                }

                allPhysicalCards[index].tag = cardTag;
            }

            foreach (var index in secretCards.Select(card => GetPhysicalCardIndex(card, card.CardOwner == CardOwner.Player1)))
            {
                allPhysicalCards[index].transform.position = secretHide;
                allPhysicalCards[index].tag = cardTag;
            }

            foreach (var handCard in playerCards.handCards)
            {
                var index = GetPhysicalCardIndex(handCard, handCard.CardOwner == CardOwner.Player1);
                if (index == -1)
                {
                    print("Cannot find Card " + handCard.Nom);
                }
                else
                {
                    allPhysicalCards[index].transform.position = secretHide;
                    allPhysicalCards[index].tag = cardTag;
                }
            }
        }
    }
}
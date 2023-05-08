using System.Collections.Generic;
using System.Linq;
using Cards.EffectCards;
using UnityEngine;
using UnityEngine.Events;

namespace Cards
{
    public class CardLocation : MonoBehaviour
    {
    
        [SerializeField] private GameObject prefabCard;
        [SerializeField] private GameObject player1;

        [SerializeField] private GameObject player2;
        
        public static readonly UnityEvent UpdateLocation = new UnityEvent();

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
        public static readonly Vector3 deckLocationP1 = new Vector3(52, 0.5f, -33);
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
        public static readonly Vector3 deckLocationP2 = new Vector3(-53, 0.5f, 34);
        private readonly Vector3 yellowTrashLocationP2 = new Vector3(-32, 0.5f, 46);

        private PlayerCards player1Cards;
        private PlayerCards player2Cards;

        // Start is called before the first frame update
        void Awake()
        {
            player1Cards = player1.GetComponent<PlayerCards>();
            player2Cards = player2.GetComponent<PlayerCards>();
            UpdateLocation.AddListener(UpdateCardLocation);
        }

        private void UpdateCardLocation()
        {
            DisplayCardsInPosition(player1Cards, true);
            DisplayCardsInPosition(player2Cards, false);
        }

        public void AddPhysicalCard(InGameCard card, string playerName)
        {
            var newPhysicalCard = Instantiate(prefabCard, playerName == "P1" ? deckLocationP1 : deckLocationP2,
                Quaternion.identity);
            newPhysicalCard.GetComponent<PhysicalCardDisplay>().card = card;
            newPhysicalCard.name = card.Title + playerName;
            UnitManager.Instance.AllPhysicalCards.Add(newPhysicalCard);
        }

        public void HideCards(List<InGameCard> cards)
        {
            foreach (var card in cards)
            {
                var index1 = GetPhysicalCardIndex(card, card.CardOwner == CardOwner.Player1);
                UnitManager.Instance.AllPhysicalCards[index1].transform.position = new Vector3(999, 999);
            }
        }

        private int GetPhysicalCardIndex(InGameCard card, bool isPlayerOne)
        {
            var cardName = card.Title + (isPlayerOne ? "P1" : "P2");

            for (var i = 0; i < UnitManager.Instance.AllPhysicalCards.Count; i++)
            {
                if (cardName == UnitManager.Instance.AllPhysicalCards[i].name)
                {
                    return i;
                }
            }

            return -1;
        }

        public void RemovePhysicalCard(InGameCard card)
        {
            var currentIndex = GetPhysicalCardIndex(card, card.CardOwner == CardOwner.Player1);
            if (currentIndex >= 0)
            {
                var physicalCard = UnitManager.Instance.AllPhysicalCards[currentIndex];
                UnitManager.Instance.AllPhysicalCards.Remove(physicalCard);
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
                if (invocationCards[i] == null) continue;
                var invocationCard = invocationCards[i];
                var index = GetPhysicalCardIndex(invocationCard, invocationCard.CardOwner == CardOwner.Player1);
                var invocationCardsLocation = isPlayerOne ? invocationCardsLocationP1[i] : invocationCardsLocationP2[i];
                UnitManager.Instance.AllPhysicalCards[index].transform.position = invocationCardsLocation;
                if (UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().Display();
                }

                UnitManager.Instance.AllPhysicalCards[index].tag = cardTag;

                if (invocationCard.EquipmentCard == null) continue;
                var equipmentCard = invocationCard.EquipmentCard;
                index = GetPhysicalCardIndex(equipmentCard, equipmentCard.CardOwner == CardOwner.Player1);
                var equipmentCardsLocation = isPlayerOne ? equipmentCardsLocationP1[i] : equipmentCardsLocationP2[i];
                UnitManager.Instance.AllPhysicalCards[index].transform.position = equipmentCardsLocation;
                if (UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    
                    UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().Display();
                }

                UnitManager.Instance.AllPhysicalCards[index].tag = cardTag;
            }

            for (var i = 0; i < playerCards.effectCards.Count; i++)
            {
                var effectCard = effectCards[i];
                if (effectCard == null) continue;
                var index = GetPhysicalCardIndex(effectCards[i], effectCard.CardOwner == CardOwner.Player1);
                var effectCardsLocation = isPlayerOne ? effectCardsLocationP1[i] : effectCardsLocationP2[i];
                UnitManager.Instance.AllPhysicalCards[index].transform.position = effectCardsLocation;

                if (UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().Display();
                }

                if (effectCard.EffectCardEffect.Keys.Contains(Effect.SameFamily))
                {
                    foreach (var invocationCard in invocationCards.Where(invocationCard => invocationCard != null)
                                 .Where(invocationCard => field != null && !playerCards.IsFieldDesactivate))
                    {
                        invocationCard.Families = new[]
                        {
                            field.GetFamily()
                        };
                    }
                }

                UnitManager.Instance.AllPhysicalCards[index].tag = cardTag;
            }

            for (var i = 0; i < deck.Count; i++)
            {
                if (deck[i] == null) continue;
                var index = GetPhysicalCardIndex(deck[i],deck[i].CardOwner == CardOwner.Player1);
                if (index == -1)
                {
                    print("Cannot find card " + deck[i].Title);
                }
                else
                {
                    var deckLocation = isPlayerOne ? deckLocationP1 : deckLocationP2;
                    UnitManager.Instance.AllPhysicalCards[index].transform.position =
                        new Vector3(deckLocation.x, deckLocation.y + 0.1f * i, deckLocation.z);
                    if (!UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                    {
                        UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().Hide();
                    }
                }
            }

            for (var i = 0; i < yellowTrash.Count; i++)
            {
                if (yellowTrash[i] == null) continue;
                var index = GetPhysicalCardIndex(yellowTrash[i], yellowTrash[i].CardOwner == CardOwner.Player1);
                var yellowTrashLocation = isPlayerOne ? yellowTrashLocationP1 : yellowTrashLocationP2;
                UnitManager.Instance.AllPhysicalCards[index].transform.position =
                    new Vector3(yellowTrashLocation.x, yellowTrashLocation.y + 0.1f * i, yellowTrashLocation.z);
                if (!UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().Hide();
                }
            }

            if (field != null)
            {
                var index = GetPhysicalCardIndex(field, field.CardOwner == CardOwner.Player1);
                var fieldCardLocation = isPlayerOne ? fieldCardLocationP1 : fieldCardLocationP2;
                UnitManager.Instance.AllPhysicalCards[index].transform.position = fieldCardLocation;
                if (UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().bIsFaceHidden)
                {
                    UnitManager.Instance.AllPhysicalCards[index].GetComponent<PhysicalCardDisplay>().Display();
                }

                UnitManager.Instance.AllPhysicalCards[index].tag = cardTag;
            }

            foreach (var index in secretCards.Select(card => GetPhysicalCardIndex(card, card.CardOwner == CardOwner.Player1)))
            {
                UnitManager.Instance.AllPhysicalCards[index].transform.position = secretHide;
                UnitManager.Instance.AllPhysicalCards[index].tag = cardTag;
            }

            foreach (var handCard in playerCards.handCards)
            {
                var index = GetPhysicalCardIndex(handCard, handCard.CardOwner == CardOwner.Player1);
                if (index == -1)
                {
                    print("Cannot find Card " + handCard.Title);
                }
                else
                {
                    UnitManager.Instance.AllPhysicalCards[index].transform.position = secretHide;
                    UnitManager.Instance.AllPhysicalCards[index].tag = cardTag;
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using _Scripts.Units.Invocation;
using Cards.EffectCards;
using UnityEngine;
using UnityEngine.Events;

namespace Cards
{
    public class CardLocation : MonoBehaviour
    {
        [SerializeField] private GameObject player1;
        [SerializeField] private GameObject player2;

        public static readonly UnityEvent UpdateLocation = new UnityEvent();

        private const float CardYOffset = 0.5f;
        private const float HiddenCardPosition = 1000f;
        private readonly Vector3 secretHide = new Vector3(HiddenCardPosition, HiddenCardPosition, HiddenCardPosition);

        private PlayerCards player1Cards;
        private PlayerCards player2Cards;

        private static readonly PlayerCardLocations Player1Locations = new PlayerCardLocations
        {
            InvocationCards = new[]
            {
                new Vector3(-53.5f, CardYOffset, -21),
                new Vector3(-32, CardYOffset, -21),
                new Vector3(-11.5f, CardYOffset, -21),
                new Vector3(10, CardYOffset, -21),
            },
            EquipmentCards = new[]
            {
                new Vector3(-53.5f, CardYOffset, -30),
                new Vector3(-32, CardYOffset, -30),
                new Vector3(-11.5f, CardYOffset, -30),
                new Vector3(10, CardYOffset, -30),
            },
            EffectCards = new[]
            {
                new Vector3(-53.5f, CardYOffset, -46.7f),
                new Vector3(-32, CardYOffset, -46.7f),
                new Vector3(-11.5f, CardYOffset, -46.7f),
                new Vector3(10, CardYOffset, -46.7f),
            },
            FieldCard = new Vector3(31.5f, CardYOffset, -21),
            Deck = new Vector3(52, CardYOffset, -33),
            YellowTrash = new Vector3(31.5f, CardYOffset, -46.7f),
        };

        private static readonly PlayerCardLocations Player2Locations = new PlayerCardLocations
        {
            InvocationCards = new[]
            {
                new Vector3(53.5f, CardYOffset, 21),
                new Vector3(32, CardYOffset, 21),
                new Vector3(10.5f, CardYOffset, 21),
                new Vector3(-11, CardYOffset, 21),
            },
            EquipmentCards = new[]
            {
                new Vector3(53.5f, CardYOffset, 30),
                new Vector3(32, CardYOffset, 30),
                new Vector3(11.5f, CardYOffset, 30),
                new Vector3(-10, CardYOffset, 30),
            },
            EffectCards = new[]
            {
                new Vector3(53.5f, CardYOffset, 46),
                new Vector3(32, CardYOffset, 46),
                new Vector3(10.5f, CardYOffset, 46),
                new Vector3(-11, CardYOffset, 46),
            },
            FieldCard = new Vector3(-32, CardYOffset, 21),
            Deck = new Vector3(-53, CardYOffset, 34),
            YellowTrash = new Vector3(-32, CardYOffset, 46),
        };

        /// <summary>
        /// Returns the location for the deck for either player one or two based on the passed boolean.
        /// </summary>
        /// <param name="isPlayerOne"></param>
        /// <returns></returns>
        public static Vector3 GetDeckLocation(bool isPlayerOne)
        {
            return isPlayerOne ? Player1Locations.Deck : Player2Locations.Deck;
        }

        /// <summary>
        /// Initializes the player1Cards and player2Cards by getting the PlayerCards component from the serialized GameObjects. It also adds a listener to the UpdateLocation UnityEvent.
        /// </summary>
        void Awake()
        {
            player1Cards = player1.GetComponent<PlayerCards>();
            player2Cards = player2.GetComponent<PlayerCards>();
            UpdateLocation.AddListener(UpdateCardLocation);
        }

        /// <summary>
        /// Removes the listener from the UpdateLocation UnityEvent when the object is destroyed.
        /// </summary>
        void OnDestroy()
        {
            UpdateLocation.RemoveListener(UpdateCardLocation);
        }

        /// <summary>
        /// Updates the position of the cards for both players.
        /// </summary>
        private void UpdateCardLocation()
        {
            DisplayCardsInPosition(player1Cards, true);
            DisplayCardsInPosition(player2Cards, false);
        }

        /// <summary>
        /// Sets the position of each card in the list to a hidden position.
        /// </summary>
        /// <param name="cards"></param>
        public void HideCards(List<InGameCard> cards)
        {
            foreach (var card in cards)
            {
                var cardGameObject = GetPhysicalCard(card, card.CardOwner == CardOwner.Player1);
                cardGameObject.transform.position = secretHide;
            }
        }

        /// <summary>
        /// Fetches the physical game object of the card from the UnitManager.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isPlayerOne"></param>
        /// <returns></returns>
        private GameObject GetPhysicalCard(InGameCard card, bool isPlayerOne)
        {
            var cardName = card.Title + (isPlayerOne ? "P1" : "P2");
            UnitManager.Instance.CardNameToGameObject.TryGetValue(cardName, out var cardGameObject);
            return cardGameObject;
        }

        /// <summary>
        /// Updates the display properties and position of a card game object.
        /// </summary>
        /// <param name="cardGameObject"></param>
        /// <param name="position"></param>
        /// <param name="shouldDisplay"></param>
        /// <param name="cardTag"></param>
        private void UpdateCardDisplay(GameObject cardGameObject, Vector3 position, bool shouldDisplay, string cardTag)
        {
            cardGameObject.transform.position = position;
            var displayComponent = cardGameObject.GetComponent<PhysicalCardDisplay>();
            if (displayComponent.bIsFaceHidden == shouldDisplay)
            {
                if (shouldDisplay)
                    displayComponent.Display();
                else
                    displayComponent.Hide();
            }
            cardGameObject.tag = cardTag;
        }


        // isPlayerOne is used to know if playerCard is from PlayerOne or PlayerTwo
        // CardOwner is used to know if physicalCard ends with P1 or P2
        /// <summary>
        /// Sets the position of different types of cards based on the player's ID.
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="isPlayerOne"></param>
        private void DisplayCardsInPosition(PlayerCards playerCards, bool isPlayerOne)
        {
            var cardTag = isPlayerOne ? "card1" : "card2";
            DisplayInvocationCards(isPlayerOne, playerCards.InvocationCards, cardTag);
            DisplayEffectCards(isPlayerOne, playerCards.EffectCards, cardTag);
            DisplayDeckCards(isPlayerOne, playerCards.Deck, cardTag);
            DisplayYellowTrashCards(isPlayerOne, playerCards.YellowCards, cardTag);
            DisplayFieldCard(isPlayerOne, playerCards.FieldCard, cardTag);
            DisplayHandCard(playerCards, cardTag);
        }

        /// <summary>
        /// Updates the display properties for hand cards.
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="cardTag"></param>
        private void DisplayHandCard(PlayerCards playerCards, string cardTag)
        {

            foreach (var handCard in playerCards.HandCards)
            {
                var cardGameObject = GetPhysicalCard(handCard, handCard.CardOwner == CardOwner.Player1);
                if (cardGameObject == null)
                {
                    print("Cannot find Card " + handCard.Title + " in handCard");
                }
                else
                {
                    UpdateCardDisplay(cardGameObject, secretHide, false, cardTag);
                }
            }
        }

        /// <summary>
        /// Updates the display properties for the field card.
        /// </summary>
        /// <param name="isPlayerOne"></param>
        /// <param name="field"></param>
        /// <param name="cardTag"></param>
        private void DisplayFieldCard(bool isPlayerOne, InGameFieldCard field, string cardTag)
        {

            if (field != null)
            {
                var cardGameObject = GetPhysicalCard(field, field.CardOwner == CardOwner.Player1);
                var fieldCardLocation = isPlayerOne ? Player1Locations.FieldCard : Player2Locations.FieldCard;
                UpdateCardDisplay(cardGameObject, fieldCardLocation, true, cardTag);
            }
        }

        /// <summary>
        /// Calculates a new card position based on a base position and an index.
        /// </summary>
        /// <param name="baseLocation"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Vector3 ComputeNewCardPosition(Vector3 baseLocation, int index)
        {
            return new Vector3(baseLocation.x, baseLocation.y + 0.1f * index, baseLocation.z);
        }

        /// <summary>
        /// Updates the display properties for the yellow trash cards.
        /// </summary>
        /// <param name="isPlayerOne"></param>
        /// <param name="yellowTrash"></param>
        /// <param name="cardTag"></param>
        private void DisplayYellowTrashCards(bool isPlayerOne, ObservableCollection<InGameCard> yellowTrash, string cardTag)
        {

            for (var i = 0; i < yellowTrash.Count; i++)
            {
                if (yellowTrash[i] == null) continue;
                var cardGameObject = GetPhysicalCard(yellowTrash[i], yellowTrash[i].CardOwner == CardOwner.Player1);
                var yellowTrashLocation = isPlayerOne ? Player1Locations.YellowTrash : Player2Locations.YellowTrash;
                var newCardYellowPosition = ComputeNewCardPosition(yellowTrashLocation, i);
                UpdateCardDisplay(cardGameObject, newCardYellowPosition, false, cardTag);
            }
        }
        
        /// <summary>
        /// Updates the display properties for deck cards.
        /// </summary>
        /// <param name="isPlayerOne"></param>
        /// <param name="deck"></param>
        /// <param name="cardTag"></param>
        private void DisplayDeckCards(bool isPlayerOne, List<InGameCard> deck, string cardTag)
        {

            for (var i = 0; i < deck.Count; i++)
            {
                if (deck[i] == null) continue;
                var cardGameObject = GetPhysicalCard(deck[i], deck[i].CardOwner == CardOwner.Player1);
                if (cardGameObject == null)
                {
                    print("Cannot find card " + deck[i].Title);
                }
                else
                {
                    var deckLocation = isPlayerOne ? Player1Locations.Deck : Player2Locations.Deck;
                    var newCardDeckPosition = ComputeNewCardPosition(deckLocation, i);
                    UpdateCardDisplay(cardGameObject, newCardDeckPosition, false, cardTag);
                }
            }
        }
        
        /// <summary>
        /// Updates the display properties for invocation cards and their associated equipment cards.
        /// </summary>
        /// <param name="isPlayerOne"></param>
        /// <param name="invocationCards"></param>
        /// <param name="cardTag"></param>
        private void DisplayInvocationCards(bool isPlayerOne, ObservableCollection<InGameInvocationCard> invocationCards, string cardTag)
        {
            var invocationCardsLocation = isPlayerOne ? Player1Locations.InvocationCards : Player2Locations.InvocationCards;
            for (var i = 0; i < invocationCards.Count; i++)
            {
                if (invocationCards[i] == null) continue;
                var invocationCard = invocationCards[i];
                var cardGameObject = GetPhysicalCard(invocationCard, invocationCard.CardOwner == CardOwner.Player1);

                UpdateCardDisplay(cardGameObject, invocationCardsLocation[i], true, cardTag);

                if (invocationCard.EquipmentCard == null) continue;
                var equipmentCard = invocationCard.EquipmentCard;
                cardGameObject = GetPhysicalCard(equipmentCard, equipmentCard.CardOwner == CardOwner.Player1);
                var equipmentCardsLocation = isPlayerOne ? Player1Locations.EquipmentCards[i] : Player2Locations.EquipmentCards[i];
                UpdateCardDisplay(cardGameObject, equipmentCardsLocation, true, cardTag);
            }
        }

        /// <summary>
        /// Updates the display properties for effect cards.
        /// </summary>
        /// <param name="isPlayerOne"></param>
        /// <param name="cards"></param>
        /// <param name="cardTag"></param>
        private void DisplayEffectCards(bool isPlayerOne, ObservableCollection<InGameEffectCard> cards, string cardTag)
        {
            var positions = isPlayerOne ? Player1Locations.EffectCards : Player2Locations.EffectCards;
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (card == null) continue;

                var cardGameObject = GetPhysicalCard(card, card.CardOwner == CardOwner.Player1);
                UpdateCardDisplay(cardGameObject, positions[i], true, cardTag);
            }
        }
    }
}
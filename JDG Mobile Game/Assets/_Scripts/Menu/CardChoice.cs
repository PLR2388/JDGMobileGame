using System.Collections.Generic;
using System.Linq;
using Cards;
using Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Menu
{
    /// <summary>
    /// Manages the card choices and selections in the game menu.
    /// </summary>
    public class CardChoice : MonoBehaviour
    {
        [SerializeField] private GameObject container;

        /// <summary>
        /// Event raised when the card choice of a player changes.
        /// </summary>
        public static readonly UnityEvent<int> ChangeChoicePlayer = new UnityEvent<int>();

        /// <summary>
        /// Indicates if the player one has chosen their cards.
        /// </summary>
        public bool isPlayerOneCardChosen;

        /// <summary>
        /// Checks and counts the selected cards in the deck.
        /// </summary>
        /// <param name="deck">The collection of cards to check.</param>
        /// <returns>The number of selected cards.</returns>
        private int CheckCard(ICollection<Card> deck)
        {
            var numberSelected = 0;
            var hoverComponents = container.GetComponentsInChildren<OnHover>();
            foreach (var hoverComponent in hoverComponents)
            {
                var isSelected = hoverComponent.IsSelected;
                if (!isSelected) continue;
                numberSelected++;
                deck.Add(hoverComponent.gameObject.GetComponent<CardDisplay>().Card);
            }

            return numberSelected;
        }

        /// <summary>
        /// Deselects all the cards.
        /// </summary>
        private void DeselectAllCards()
        {
            CardSelectionManager.Instance.ClearSelection();
        }

        /// <summary>
        /// Verifies and manages the cards chosen by the player.
        /// </summary>
        public void CheckPlayerCards()
        {
            var deck = new List<Card>();
            var numberSelected = CheckCard(deck);

            if (numberSelected == GameState.MaxDeckCards)
            {
                CardChoiceUIManager.Instance.UpdateTitleAndButtonTextForPlayer(isPlayerOneCardChosen);
                if (isPlayerOneCardChosen)
                {
                    AudioSystem.Instance.StopMusic();
                    SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                    isPlayerOneCardChosen = false;
                    ChangeChoicePlayer.Invoke(1);

                    GameState.Instance.Player2DeckCards =
                        deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player2)).ToList();
                }
                else
                {
                    isPlayerOneCardChosen = true;
                    ChangeChoicePlayer.Invoke(2);

                    GameState.Instance.Player1DeckCards =
                        deck.Select(card => CardFactory.CreateInGameCard(card, CardOwner.Player1)).ToList();
                    DeselectAllCards();
                }
            }
            else
            {
                var remainedCards = GameState.MaxDeckCards - numberSelected;
                CardChoiceUIManager.Instance.DisplayMessageBox(remainedCards);
            }
        }

        /// <summary>
        /// Filters and retrieves the cards that meet specific criteria.
        /// </summary>
        /// <param name="sourceCards">The source cards to filter from.</param>
        /// <returns>A list of filtered cards.</returns>
        private static List<Card> FilterCards(List<Card> sourceCards)
        {
            return sourceCards.Where(card =>
                    card.Type != CardType.Contre &&
                    card.Title != CardNameMappings.CardNameMap[CardNames.AttaqueDeLaTourEiffel] &&
                    card.Title != CardNameMappings.CardNameMap[CardNames.BlagueInterdite] &&
                    card.Title != CardNameMappings.CardNameMap[CardNames.UnBonTuyau])
                .ToList();
        }

        /// <summary>
        /// Generates a random deck of cards.
        /// </summary>
        /// <param name="numberOfCards">The number of cards required in the deck.</param>
        /// <param name="initialDeck">The initial collection of cards.</param>
        /// <param name="cards">The list of cards to choose from.</param>
        public static void GetRandomDeck(int numberOfCards, ref List<Card> initialDeck, List<Card> cards)
        {
            var deckAllCard = FilterCards(cards);

            while (initialDeck.Count != numberOfCards)
            {
                GetRandomCards(deckAllCard, initialDeck);
            }
        }

        /// <summary>
        /// Sets random decks for players and starts the game.
        /// </summary>
        public void RandomDeck()
        {
            var deck1 = new List<Card>();
            var deck2 = new List<Card>();

            var deck1AllCard = FilterCards(GameState.Instance.deck1AllCards);
            var deck2AllCard = FilterCards(GameState.Instance.deck2AllCards);

            while (deck1.Count != GameState.MaxDeckCards)
            {
                GetRandomCards(deck1AllCard, deck1);
            }

            while (deck2.Count != GameState.MaxDeckCards)
            {
                GetRandomCards(deck2AllCard, deck2);
            }

            GameState.Instance.Player1DeckCards =
                deck1.Select(card1 => CardFactory.CreateInGameCard(card1, CardOwner.Player1)).ToList();
            GameState.Instance.Player2DeckCards =
                deck2.Select(card2 => CardFactory.CreateInGameCard(card2, CardOwner.Player2)).ToList();
            AudioSystem.Instance.StopMusic();
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        }

        /// <summary>
        /// Sets a deck for testing purposes and starts the game.
        /// </summary>
        public void DeckToTest()
        {
            var deck1 = new List<Card>();
            var deck2 = new List<Card>();

            var deck1AllCard = GameState.Instance.deck1AllCards;
            var deck2AllCard = GameState.Instance.deck2AllCards;

            deck2.Add(GetSpecificCard(CardNames.LycéeMagiqueGeorgesPompidou, deck2AllCard));
            deck1.Add(GetSpecificCard(CardNames.SandrineLePorteManteauExtraterrestre, deck1AllCard));
            deck1.Add(GetSpecificCard(CardNames.BenzaieJeune, deck1AllCard));
            deck1.Add(GetSpecificCard(CardNames.AlphaMan, deck1AllCard));
            deck1.Add(GetSpecificCard(CardNames.FiltreDégueulasseFMV, deck1AllCard));

            while (deck1.Count != GameState.MaxDeckCards)
            {
                GetRandomCards(deck1AllCard, deck1);
            }

            deck1.Reverse();


            while (deck2.Count != GameState.MaxDeckCards)
            {
                GetRandomCards(deck2AllCard, deck2);
            }

            deck2.Reverse();

            GameState.Instance.Player1DeckCards =
                deck1.Select(card1 => CardFactory.CreateInGameCard(card1, CardOwner.Player1)).ToList();
            GameState.Instance.Player2DeckCards =
                deck2.Select(card2 => CardFactory.CreateInGameCard(card2, CardOwner.Player2)).ToList();
            AudioSystem.Instance.StopMusic();
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        }

        /// <summary>
        /// Retrieves a specific card based on its name.
        /// </summary>
        /// <param name="cardNames">The name identifier of the card.</param>
        /// <param name="cards">The list of cards to search from.</param>
        /// <returns>The specific card found; null otherwise.</returns>
        public static Card GetSpecificCard(CardNames cardNames, List<Card> cards)
        {
            var nameCard = CardNameMappings.CardNameMap[cardNames];
            var card = cards.Find(x => x.Title == nameCard);
            if (card != null)
            {
                cards.Remove(card);
            }

            return card;
        }

        /// <summary>
        /// Gets a random card from the list and adds it to the deck.
        /// </summary>
        /// <param name="allCards">The list of cards to choose from.</param>
        /// <param name="deck">The deck to which the card is added.</param>
        private static void GetRandomCards(IList<Card> allCards, ICollection<Card> deck)
        {
            var randomIndex = Random.Range(0, allCards.Count - 1);
            var card = allCards[randomIndex];
            if (card.Type == CardType.Contre) return;
            if (card == null) return;
            deck.Add(card);
            allCards.Remove(card);
        }

        /// <summary>
        /// Handles the back action in the game menu.
        /// </summary>
        public void Back()
        {
            if (isPlayerOneCardChosen)
            {
                CardChoiceUIManager.Instance.UpdateTitleAndButtonTextForPlayer(true);
                isPlayerOneCardChosen = false;
                GameState.Instance.Player1DeckCards = new List<InGameCard>();
                DeselectAllCards();
                ChangeChoicePlayer.Invoke(1);
            }
            else
            {
                DeselectAllCards();
                CardChoiceUIManager.Instance.ShowChoiceCardMenu(false);
                CardChoiceUIManager.Instance.ShowTwoPlayerModeMenu(true);
            }
        }
    }
}
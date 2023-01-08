using System.Collections.Generic;
using System.Linq;
using Cards;
using Lean.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Menu
{
    public class CardChoice : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private LeanToken playerToken;
        [SerializeField] private LeanLocalizedText buttonLabel;
        [SerializeField] private LeanPhrase choiceText;
        [SerializeField] private LeanPhrase playText;
        [SerializeField] private Transform canvas;

        [SerializeField] private GameObject choiceCardMenu;
        [SerializeField] private GameObject gameModeMenu;

        public bool isPlayerOneCardChosen;

        private int CheckCard(ICollection<Card> deck)
        {
            var numberSelected = 0;
            var children = container.GetComponentsInChildren<Transform>();
            foreach (var transformInChildren in children)
            {
                var cardGameObject = transformInChildren.gameObject;
                if (cardGameObject.GetComponent<OnHover>() == null) continue;
                var isSelected = cardGameObject.GetComponent<OnHover>().bIsSelected;
                if (!isSelected) continue;
                numberSelected++;
                deck.Add(cardGameObject.GetComponent<CardDisplay>().card);
            }

            return numberSelected;
        }

        private void DisplayMessageBox(int remainedCards)
        {
            MessageBox.CreateOkMessageBox(canvas, "Modifie ton deck",
                "Tu dois avoir 30 cartes !\n " + remainedCards + " cartes restantes à choisir !");
        }

        private void DeselectAllCards()
        {
            var children = container.GetComponentsInChildren<Transform>();
            foreach (var transformChildren in children)
            {
                var gameObjectChildren = transformChildren.gameObject;
                if (gameObjectChildren.GetComponent<OnHover>() != null)
                {
                    gameObjectChildren.GetComponent<OnHover>().bIsSelected = false;
                }
            }
        }

        public void CheckPlayerCards()
        {
            var deck = new List<Card>();
            var numberSelected = CheckCard(deck);

            if (numberSelected == GameState.MaxDeckCards)
            {
                if (isPlayerOneCardChosen)
                {
                    SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                    playerToken.SetValue(1);
                    buttonLabel.TranslationName = choiceText.name;
                    isPlayerOneCardChosen = false;

                    FindObjectOfType<GameState>().deckP2 = deck.Select(card => InGameCard.CreateInGameCard(card, CardOwner.Player2)).ToList();
                }
                else
                {
                    playerToken.SetValue(2);
                    buttonLabel.TranslationName = playText.name;
                    isPlayerOneCardChosen = true;

                    FindObjectOfType<GameState>().deckP1 = deck.Select(card => InGameCard.CreateInGameCard(card, CardOwner.Player1)).ToList();
                    DeselectAllCards();
                }
            }
            else
            {
                var remainedCards = GameState.MaxDeckCards - numberSelected;
                DisplayMessageBox(remainedCards);
            }
        }

        public static void GetRandomDeck(int numberOfCards, ref List<Card> initialDeck, List<Card> cards)
        {
            var deckAllCard = cards.Where(card =>
                card.Type != CardType.Contre && card.Nom != "Attaque de la tour Eiffel" &&
                card.Nom != "Blague interdite" &&
                card.Nom != "Un bon tuyau").ToList();

            while (initialDeck.Count != numberOfCards)
            {
                GetRandomCards(deckAllCard, initialDeck);
            }
        }

        public void RandomDeck()
        {
            var deck1 = new List<Card>();
            var deck2 = new List<Card>();

            var deck1AllCard = FindObjectOfType<GameState>().deck1AllCards.Where(card =>
                card.Type != CardType.Contre && card.Nom != "Attaque de la tour Eiffel" &&
                card.Nom != "Blague interdite" &&
                card.Nom != "Un bon tuyau").ToList();

            var deck2AllCard = FindObjectOfType<GameState>().deck2AllCards.Where(card =>
                card.Type != CardType.Contre && card.Nom != "Attaque de la tour Eiffel" &&
                card.Nom != "Blague interdite" &&
                card.Nom != "Un bon tuyau").ToList();

            while (deck1.Count != 30)
            {
                GetRandomCards(deck1AllCard, deck1);
            }

            while (deck2.Count != 30)
            {
                GetRandomCards(deck2AllCard, deck2);
            }

            FindObjectOfType<GameState>().deckP1 = deck1.Select(card1 => InGameCard.CreateInGameCard(card1, CardOwner.Player1)).ToList();
            FindObjectOfType<GameState>().deckP2 = deck2.Select(card2 => InGameCard.CreateInGameCard(card2, CardOwner.Player2)).ToList();
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        }

        public void DeckToTest()
        {
            var deck1 = new List<Card>();
            var deck2 = new List<Card>();

            var deck1AllCard = FindObjectOfType<GameState>().deck1AllCards;
            var deck2AllCard = FindObjectOfType<GameState>().deck2AllCards;

            deck2.Add(GetSpecificCard("Studio de scénaristes Canadien", deck2AllCard));
            deck2.Add(GetSpecificCard("Petite culotte", deck2AllCard));
            deck1.Add(GetSpecificCard("Main bionique", deck1AllCard));
            deck1.Add(GetSpecificCard("Moïse, le plus grand de tous les hébreux", deck1AllCard));
            deck1.Add(GetSpecificCard("Un délicieux risotto", deck1AllCard));

            while (deck1.Count != 30)
            {
                GetRandomCards(deck1AllCard, deck1);
            }

            deck1.Reverse();

            while (deck2.Count != 30)
            {
                GetRandomCards(deck2AllCard, deck2);
            }

            deck2.Reverse();

            FindObjectOfType<GameState>().deckP1 = deck1.Select(card1 => InGameCard.CreateInGameCard(card1, CardOwner.Player1)).ToList();
            FindObjectOfType<GameState>().deckP2 = deck2.Select(card2 => InGameCard.CreateInGameCard(card2, CardOwner.Player2)).ToList();
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        }

        public static Card GetSpecificCard(string nameCard, List<Card> cards)
        {
            var card = cards.Find(x => x.Nom == nameCard);
            if (card != null)
            {
                cards.Remove(card);
            }

            return card;
        }

        private static void GetRandomCards(IList<Card> allCards, ICollection<Card> deck)
        {
            var randomIndex = Random.Range(0, allCards.Count - 1);
            var card = allCards[randomIndex];
            if (card.Type == CardType.Contre) return;
            if (card == null) return;
            deck.Add(card);
            allCards.Remove(card);
        }

        public void Back()
        {
            if (isPlayerOneCardChosen)
            {
                playerToken.SetValue(1);
                buttonLabel.TranslationName = choiceText.name;
                isPlayerOneCardChosen = false;
                FindObjectOfType<GameState>().deckP1 = new List<InGameCard>();
                DeselectAllCards();
            }
            else
            {
                DeselectAllCards();
                choiceCardMenu.SetActive(false);
                gameModeMenu.SetActive(true);
            }
        }
    }
}
using System.Collections.Generic;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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

    private bool isPlayerOneCardChosen = false;

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
        if (!isPlayerOneCardChosen)
        {
            var deck = new List<Card>();
            var numberSelected = CheckCard(deck);

            if (numberSelected == GameState.maxDeckCards)
            {
                playerToken.SetValue(2);
                buttonLabel.TranslationName = playText.name;
                isPlayerOneCardChosen = true;
                FindObjectOfType<GameState>().deckP1 = deck;
                DeselectAllCards();
            }
            else
            {
                var remainedCards = GameState.maxDeckCards - numberSelected;
                DisplayMessageBox(remainedCards);
            }
        }
        else
        {
            var deck = new List<Card>();
            var numberSelected = CheckCard(deck);

            if (numberSelected == GameState.maxDeckCards)
            {
                SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                playerToken.SetValue(1);
                buttonLabel.TranslationName = choiceText.name;
                isPlayerOneCardChosen = false;
                FindObjectOfType<GameState>().deckP2 = deck;
            }
            else
            {
                var remainedCards = GameState.maxDeckCards - numberSelected;
                DisplayMessageBox(remainedCards);
            }
        }
    }

    public void RandomDeck()
    {
        var deck1 = new List<Card>();
        var deck2 = new List<Card>();

        var allCards = FindObjectOfType<GameState>().allCards;

        while (deck1.Count != 30)
        {
            GetRandomCards(allCards, deck1);
        }

        while (deck2.Count != 30)
        {
            GetRandomCards(allCards, deck2);
        }

        FindObjectOfType<GameState>().deckP1 = deck1;
        FindObjectOfType<GameState>().deckP2 = deck2;
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    public void DeckToTest()
    {
        var deck1 = new List<Card>();
        var deck2 = new List<Card>();

        var allCards = FindObjectOfType<GameState>().allCards;

     
        deck1.Add(GetSpecificCard("Alpha V De Gelganech", allCards));
        deck1.Add(GetSpecificCard("Benzaie jeune", allCards));
        deck1.Add(GetSpecificCard("Bébé Terreur-Nocturne", allCards));
        deck1.Add(GetSpecificCard("Carole du service marketing", allCards));
        deck1.Add(GetSpecificCard("Cliché Raciste", allCards));
        deck1.Add(GetSpecificCard("Clodo du coin", allCards));
        deck1.Add(GetSpecificCard("Dictateur Sympa", allCards));
        deck1.Add(GetSpecificCard("Fourchette", allCards));
        deck1.Add(GetSpecificCard("Frangipanus", allCards));
        deck1.Add(GetSpecificCard("Armure trop lourde", allCards));
        deck1.Add(GetSpecificCard("Gérard Choixpeau", allCards));
        deck1.Add(GetSpecificCard("Inspecteur Magret", allCards));
        deck1.Add(GetSpecificCard("Koaloutre-Ornithambas Lapinzord nain de Californie", allCards));
        deck1.Add(GetSpecificCard("La Mort", allCards));
        deck1.Add(GetSpecificCard("Le Hobbit", allCards));
        deck1.Add(GetSpecificCard("Le voisin", allCards));
        deck1.Add(GetSpecificCard("Lolhitler", allCards));
        deck1.Add(GetSpecificCard("Maman", allCards));
        deck1.Add(GetSpecificCard("Manuel Ferrara", allCards));
        deck1.Add(GetSpecificCard("Poignée de porte", allCards));
        deck1.Add(GetSpecificCard("Spaghetti", allCards));
        deck1.Add(GetSpecificCard("Théodule", allCards));
        deck1.Add(GetSpecificCard("Sangoku", allCards));
        deck1.Add(GetSpecificCard("Sheik Point", allCards));
        deck1.Add(GetSpecificCard("Starlight Unicorn", allCards));
        deck1.Add(GetSpecificCard("Attaque de la tour Eiffel", allCards));


        while (deck2.Count != 30)
        {
            GetRandomCards(allCards, deck2);
        }

        FindObjectOfType<GameState>().deckP1 = deck1;
        FindObjectOfType<GameState>().deckP2 = deck2;
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    private Card GetSpecificCard(string nameCard, List<Card> cards)
    {
        return cards.Find(x => x.Nom == nameCard);
    }

    private static void GetRandomCards(IList<Card> allCards, ICollection<Card> deck)
    {
        var randomIndex = Random.Range(0, allCards.Count - 1);
        var card = allCards[randomIndex];
        if (card.Type == CardType.Contre) return;
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
            FindObjectOfType<GameState>().deckP1 = new List<Card>();
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
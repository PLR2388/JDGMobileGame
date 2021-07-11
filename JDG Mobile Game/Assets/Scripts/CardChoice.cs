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
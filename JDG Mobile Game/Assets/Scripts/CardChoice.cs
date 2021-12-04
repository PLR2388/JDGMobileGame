﻿using System.Collections.Generic;
using Lean.Localization;
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
        var deck = new List<Card>();
        var numberSelected = CheckCard(deck);
        
        if (numberSelected == GameState.maxDeckCards)
        {
            if (isPlayerOneCardChosen)
            {
                SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                playerToken.SetValue(1);
                buttonLabel.TranslationName = choiceText.name;
                isPlayerOneCardChosen = false;
                FindObjectOfType<GameState>().deckP2 = deck;
            }
            else
            {
                playerToken.SetValue(2);
                buttonLabel.TranslationName = playText.name;
                isPlayerOneCardChosen = true;
                FindObjectOfType<GameState>().deckP1 = deck;
                DeselectAllCards();
            }
        }
        else
        {
            var remainedCards = GameState.maxDeckCards - numberSelected;
            DisplayMessageBox(remainedCards);
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

        deck1.Add(GetSpecificCard("Daffy",allCards));
        deck1.Add(GetSpecificCard("Benzaie jeune", allCards));
        deck1.Add(GetSpecificCard("Cassette VHS", allCards));
        deck1.Add(GetSpecificCard("Benzaie", allCards));
        deck1.Add(GetSpecificCard("Jean-Claude", allCards));
        
        while (deck1.Count != 30)
        {
            GetRandomCards(allCards, deck1);
        }
        
        deck1.Reverse();

        while (deck2.Count != 30)
        {
            GetRandomCards(allCards, deck2);
        }

        FindObjectOfType<GameState>().deckP1 = deck1;
        FindObjectOfType<GameState>().deckP2 = deck2;
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    private static Card GetSpecificCard(string nameCard, List<Card> cards)
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
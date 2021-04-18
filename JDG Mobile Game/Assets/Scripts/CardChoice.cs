using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CardChoice : MonoBehaviour
{
    [SerializeField] private GameObject gameState;
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI buttonLabel;
    [SerializeField] private GameObject messageBox;

    [SerializeField] private GameObject choiceCardMenu;
    [SerializeField] private GameObject gameModeMenu;

    private bool isPlayerOneCardChosen = false;

    private int CheckCard(List<Card> deck)
    {
        int numberSelected = 0;
        Transform[] children = container.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            GameObject cardGameObject = children[i].gameObject;
            if (cardGameObject.GetComponent<OnHover>() != null)
            {
                bool isSelected = cardGameObject.GetComponent<OnHover>().bIsSelected;
                if (isSelected)
                {
                    numberSelected++;
                    deck.Add(cardGameObject.GetComponent<CardDisplay>().card);
                }
            }
        }

        return numberSelected;
    }

    private void DisplayMessageBox(int remainedCards)
    {
        GameObject message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Modifie ton deck";
        message.GetComponent<MessageBox>().isInformation = true;
        message.GetComponent<MessageBox>().description =
            "Tu dois avoir 30 cartes !\n " + remainedCards + " cartes restantes à choisir !";
    }

    private void DeselectAllCards()
    {
        Transform[] children = container.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            GameObject gameObject = children[i].gameObject;
            if (gameObject.GetComponent<OnHover>() != null)
            { 
                gameObject.GetComponent<OnHover>().bIsSelected = false;
            }
        }
    }

    public void CheckPlayerCards()
    {
        if (!isPlayerOneCardChosen)
        {
            List<Card> deck = new List<Card>();
            int numberSelected = CheckCard(deck);

            if (numberSelected == GameState.maxDeckCards)
            {
                label.text = "Choix de cartes pour le joueur 2";
                buttonLabel.text = "Jouer";
                isPlayerOneCardChosen = true;
                gameState.GetComponent<GameState>().DeckP1 = deck;
                DeselectAllCards();
            }
            else
            {
                int remainedCards = GameState.maxDeckCards - numberSelected;
                DisplayMessageBox(remainedCards);
            }
        }
        else
        {
            List<Card> deck = new List<Card>();
            int numberSelected = CheckCard(deck);

            if (numberSelected == GameState.maxDeckCards)
            {
                SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                label.text = "Choix de cartes pour le joueur 1";
                buttonLabel.text = "Choix joueur 2";
                isPlayerOneCardChosen = false;
                gameState.GetComponent<GameState>().DeckP2 = deck;
            }
            else
            {
                int remainedCards = GameState.maxDeckCards - numberSelected;
                DisplayMessageBox(remainedCards);
            }
        }
    }

    public void RandomDeck()
    {
        List<Card> deck1 = new List<Card>();
        List<Card> deck2 = new List<Card>();

        List<Card> allCards = gameState.GetComponent<GameState>().allCards;

        while (deck1.Count != 30)
        {
            GetRandomCards(allCards, deck1);
        }
        
        while (deck2.Count != 30)
        {
            GetRandomCards(allCards,deck2);
        }
        
        gameState.GetComponent<GameState>().DeckP1 = deck1;
        gameState.GetComponent<GameState>().DeckP2 = deck2;
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    private void GetRandomCards(List<Card> allCards, List<Card> deck)
    {
        int randomIndex = Random.Range(0, allCards.Count - 1);
        Card card = allCards[randomIndex];
        if (card.Type != "contre")
        {
            deck.Add(card);
            allCards.Remove(card);
        }
    }

    public void Back()
    {
        if (isPlayerOneCardChosen)
        {
            label.text = "Choix de cartes pour le joueur 1";
            buttonLabel.text = "Choix joueur 2";
            isPlayerOneCardChosen = false;
            gameState.GetComponent<GameState>().DeckP1 = new List<Card>();
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
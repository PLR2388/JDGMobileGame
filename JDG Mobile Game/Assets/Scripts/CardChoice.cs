using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private int checkCard(List<Card> deck)
    {
        int numberSelected = 0;
        Transform[] children = container.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            GameObject gameObject = children[i].gameObject;
            if (gameObject.GetComponent<OnHover>() != null)
            {
                bool isSelected = gameObject.GetComponent<OnHover>().bIsSelected;
                if (isSelected)
                {
                    numberSelected++;
                    deck.Add(gameObject.GetComponent<CardDisplay>().card);
                }
            }
        }

        return numberSelected;
    }

    private void displayMessageBox(int remainedCards)
    {
        GameObject message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Modifie ton deck";
        message.GetComponent<MessageBox>().isInformation = true;
        message.GetComponent<MessageBox>().description =
            "Tu dois avoir 30 cartes !\n " + remainedCards + " cartes restantes à choisir !";
    }

    private void deselectAllCards()
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

    public void checkPlayerCards()
    {
        if (!isPlayerOneCardChosen)
        {
            List<Card> deck = new List<Card>();
            int numberSelected = checkCard(deck);

            if (numberSelected == GameState.maxDeckCards)
            {
                label.text = "Choix de cartes pour le joueur 2";
                buttonLabel.text = "Jouer";
                isPlayerOneCardChosen = true;
                gameState.GetComponent<GameState>().DeckP1 = deck;
                deselectAllCards();
            }
            else
            {
                int remainedCards = GameState.maxDeckCards - numberSelected;
                displayMessageBox(remainedCards);
            }
        }
        else
        {
            List<Card> deck = new List<Card>();
            int numberSelected = checkCard(deck);

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
                displayMessageBox(remainedCards);
            }
        }
    }

    public void getRandomDeck()
    {
        List<Card> deck1 = new List<Card>();
        List<Card> deck2 = new List<Card>();

        int size = gameState.GetComponent<GameState>().allCards.Count;
        Card[] cards = new Card[size];

        gameState.GetComponent<GameState>().allCards.CopyTo(cards);

        List<Card> allDeck = cards.ToList();
        Random alea = new Random();

        for (int i = 0; i < 30; i++)
        {
            int random = alea.Next(size);
            deck1.Add(allDeck[random]);
            allDeck.Remove(allDeck[random]);
            size--;
            random = alea.Next(size);
            deck2.Add(allDeck[random]);
            allDeck.Remove(allDeck[random]);
            size--;
        }

        gameState.GetComponent<GameState>().DeckP1 = deck1;
        gameState.GetComponent<GameState>().DeckP2 = deck2;
        
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        label.text = "Choix de cartes pour le joueur 1";
        buttonLabel.text = "Choix joueur 2";
        isPlayerOneCardChosen = true;
    }

    public void back()
    {
        if (isPlayerOneCardChosen)
        {
            label.text = "Choix de cartes pour le joueur 1";
            buttonLabel.text = "Choix joueur 2";
            isPlayerOneCardChosen = false;
            gameState.GetComponent<GameState>().DeckP1 = new List<Card>();
            deselectAllCards();
        }
        else
        {
            deselectAllCards();
            choiceCardMenu.SetActive(false);
            gameModeMenu.SetActive(true);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Cards;
using Menu;
using UnityEngine;

public class GameState : StaticInstance<GameState>
{
    [SerializeField]
    private List<Card> allCards;

    public List<Card> deck1AllCards = new List<Card>();
    public List<Card> deck2AllCards = new List<Card>();

    public List<InGameCard> Player1DeckCards = new List<InGameCard>();
    public List<InGameCard> Player2DeckCards = new List<InGameCard>();

    public const int MaxDeckCards = 30;
    public const int MaxRare = 5;
    public const int InitialNumberOfHandCards = 5;

    /// <summary>
    /// Initializes the game state.
    /// </summary>
    private void Start()
    {
        ResetDeckPlayer();
    }

    /// <summary>
    /// Resets the decks for both players.
    /// </summary>
    private void ResetDeckPlayer()
    {
        deck1AllCards.Clear();
        deck2AllCards.Clear();
        foreach (var card in allCards)
        {
            deck1AllCards.Add(Instantiate(card));
            deck2AllCards.Add(Instantiate(card));
        }
    }

    /// <summary>
    /// Builds the tutorial decks for both players.
    /// </summary>
    public void BuildDeckForTuto()
    {
        ResetDeckPlayer();
        BuildPlayer1DeckForTuto();
        BuildPlayer2DeckForTuto();
    }
    
    /// <summary>
    /// Instantiates specific cards based on the card names provided.
    /// </summary>
    /// <param name="cardNames">The names of the cards to be instantiated.</param>
    /// <param name="deck">The deck to get the cards from.</param>
    /// <returns>A list of instantiated cards.</returns>
    private List<Card> InstantiateSpecificCards(List<CardNames> cardNames, List<Card> deck)
    {
        return cardNames.Select(cardName => CardChoice.GetSpecificCard(cardName, deck)).ToList();
    }
    
    /// <summary>
    /// Builds the tutorial deck for player 1.
    /// </summary>
    private void BuildPlayer1DeckForTuto()
    {
        var player1Deck = InstantiateSpecificCards(
            new List<CardNames>
            {
                CardNames.Fisti,
                CardNames.JeanMichelBruitages,
                CardNames.LePyroBarbare,
                CardNames.Fistiland,
                CardNames.MerdeMagiqueEnPlastiqueRose
            }, 
            deck1AllCards
        );
        CardChoice.GetRandomDeck(25, ref player1Deck, deck1AllCards);
        Player1DeckCards = player1Deck.Select(card1 => InGameCard.CreateInGameCard(card1, CardOwner.Player1)).ToList();
    }
    
    /// <summary>
    /// Builds the tutorial deck for player 2.
    /// </summary>
    private void BuildPlayer2DeckForTuto()
    {
        var player2Deck = InstantiateSpecificCards(
            new List<CardNames>
            {
                CardNames.ClichéRaciste,
                CardNames.Tentacules,
                CardNames.MusiqueDeMegaDrive,
                CardNames.LElfette
            },
            deck2AllCards
        );

        Player2DeckCards = player2Deck.Select(card2 => InGameCard.CreateInGameCard(card2, CardOwner.Player2)).ToList();
    }
}
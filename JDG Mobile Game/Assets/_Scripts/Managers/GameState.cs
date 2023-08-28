using System.Collections.Generic;
using System.Linq;
using Cards;
using Menu;
using UnityEngine;

public class GameState : StaticInstance<GameState>
{
    public List<Card> allCards;

    public List<Card> deck1AllCards = new List<Card>();
    public List<Card> deck2AllCards = new List<Card>();

    public List<InGameCard> deckP1 = new List<InGameCard>();
    public List<InGameCard> deckP2 = new List<InGameCard>();

    public const int MaxDeckCards = 30;
    public const int MaxRare = 5;

    private void Start()
    {
        ResetDeckPlayer();
    }

    private void ResetDeckPlayer()
    {
        deck1AllCards.Clear();
        deck2AllCards.Clear();
        foreach (var card in allCards)
        {
            deck1AllCards.Add(Instantiate(card));
            deck2AllCards.Add(Instantiate(card));
            Debug.Log(card.Title);
        }
    }

    public void BuildDeckForTuto()
    {
        ResetDeckPlayer();
        List<Card> deck1 = new List<Card>();
        List<Card> deck2 = new List<Card>();

        var card = CardChoice.GetSpecificCard(CardNames.ClichéRaciste, deck2AllCards);
        var tentacule = CardChoice.GetSpecificCard(CardNames.Tentacules, deck2AllCards);
        var musicMegaDrive = CardChoice.GetSpecificCard(CardNames.MusiqueDeMegaDrive, deck2AllCards);
        var elfette = CardChoice.GetSpecificCard(CardNames.LElfette, deck2AllCards);


        var fisti = CardChoice.GetSpecificCard(CardNames.Fisti, deck1AllCards);
        var jeanMichelBruitage = CardChoice.GetSpecificCard(CardNames.JeanMichelBruitages, deck1AllCards);
        var pyroBarbare = CardChoice.GetSpecificCard(CardNames.LePyroBarbare, deck1AllCards);
        var fistiland = CardChoice.GetSpecificCard(CardNames.Fistiland, deck1AllCards);
        var merdeRose = CardChoice.GetSpecificCard(CardNames.MerdeMagiqueEnPlastiqueRose, deck1AllCards);


        CardChoice.GetRandomDeck(25, ref deck1, deck1AllCards);
        deck1.Add(fisti);
        deck1.Add(pyroBarbare);
        deck1.Add(jeanMichelBruitage);
        deck1.Add(fistiland);
        deck1.Add(merdeRose);
        CardChoice.GetRandomDeck(26, ref deck2, deck2AllCards);
        deck2.Add(card);
        deck2.Add(musicMegaDrive);
        deck2.Add(elfette);
        deck2.Add(tentacule);

        deckP1 = deck1.Select(card1 => InGameCard.CreateInGameCard(card1, CardOwner.Player1)).ToList();
        deckP2 = deck2.Select(card2 => InGameCard.CreateInGameCard(card2, CardOwner.Player2)).ToList();
    }
}
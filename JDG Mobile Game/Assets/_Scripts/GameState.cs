using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.InvocationCards;
using Menu;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public List<Card> allCards;

    public List<Card> deck1AllCards;
    public List<Card> deck2AllCards;

    public List<InGameCard> deckP1 = new List<InGameCard>();
    public List<InGameCard> deckP2 = new List<InGameCard>();

    private static GameState instance;

    public const int MaxDeckCards = 30;
    public const int MaxRare = 5;

    private void Awake()
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
        }
    }

    private void Start()
    {
        foreach (var t in allCards)
        {
            InGameCard inGameCard = InGameCard.CreateInGameCard(t, CardOwner.Player1);
            deckP1.Add(inGameCard);
        }

        for (var i = 30; i < 60; i++)
        {
            deckP2.Add(InGameCard.CreateInGameCard(allCards[i], CardOwner.Player2));
        }
    }

    public void BuildDeckForTuto()
    {
        ResetDeckPlayer();
        List<Card> deck1 = new List<Card>();
        List<Card> deck2 = new List<Card>();

        var card = CardChoice.GetSpecificCard("Cliché Raciste", deck2AllCards);
        var tentacule = CardChoice.GetSpecificCard("Tentacules", deck2AllCards);
        var musicMegaDrive = CardChoice.GetSpecificCard("Musique de Mega Drive", deck2AllCards);
        var elfette = CardChoice.GetSpecificCard("L'Elfette", deck2AllCards);


        var fisti = CardChoice.GetSpecificCard("Fisti", deck1AllCards);
        var jeanMichelBruitage = CardChoice.GetSpecificCard("Jean-Michel Bruitages", deck1AllCards);
        var pyroBarbare = CardChoice.GetSpecificCard("Le Pyro-Barbare", deck1AllCards);
        var fistiland = CardChoice.GetSpecificCard("Fistiland", deck1AllCards);
        var merdeRose = CardChoice.GetSpecificCard("Merde magique en plastique rose", deck1AllCards);


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
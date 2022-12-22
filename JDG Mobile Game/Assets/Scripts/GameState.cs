using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EffectCards;
using Cards.FieldCards;
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


    public static GameState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameState>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

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

    private void InitCards()
    {
        foreach (var invocationCard in allCards.Where(card => card.Type == CardType.Invocation).Cast<InvocationCard>())
        {
            invocationCard.Init();
        }

        foreach (var effectCard in allCards.Where(card => card.Type == CardType.Effect).Cast<EffectCard>())
        {
            effectCard.Init();
        }
    }

    private void Start()
    {
        InitCards();
        foreach (var t in allCards)
        {
            InGameCard inGameCard = InGameCard.CreateInGameCard(t);
            deckP1.Add(inGameCard);
        }

        for (var i = 30; i < 60; i++)
        {
            deckP2.Add(InGameCard.CreateInGameCard(allCards[i]));
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
        card.CardOwner = CardOwner.Player2;
        tentacule.CardOwner = CardOwner.Player2;
        musicMegaDrive.CardOwner = CardOwner.Player2;
        elfette.CardOwner = CardOwner.Player2;


        var fisti = CardChoice.GetSpecificCard("Fisti", deck1AllCards);
        fisti.CardOwner = CardOwner.Player1;
        var jeanMichelBruitage = CardChoice.GetSpecificCard("Jean-Michel Bruitages", deck1AllCards);
        jeanMichelBruitage.CardOwner = CardOwner.Player1;
        var pyroBarbare = CardChoice.GetSpecificCard("Le Pyro-Barbare", deck1AllCards);
        pyroBarbare.CardOwner = CardOwner.Player1;
        var fistiland = CardChoice.GetSpecificCard("Fistiland", deck1AllCards);
        fistiland.CardOwner = CardOwner.Player1;
        var merdeRose = CardChoice.GetSpecificCard("Merde magique en plastique rose", deck1AllCards);
        merdeRose.CardOwner = CardOwner.Player1;


        CardChoice.GetRandomDeck(25, ref deck1, deck1AllCards, CardOwner.Player1);
        deck1.Add(fisti);
        deck1.Add(pyroBarbare);
        deck1.Add(jeanMichelBruitage);
        deck1.Add(fistiland);
        deck1.Add(merdeRose);
        deck1.Add(tentacule);
        CardChoice.GetRandomDeck(26, ref deck2, deck2AllCards, CardOwner.Player2);
        deck2.Add(card);
        deck2.Add(musicMegaDrive);
        deck2.Add(elfette);

        deckP1 = deck1.Select(InGameCard.CreateInGameCard).ToList();
        deckP2 = deck2.Select(InGameCard.CreateInGameCard).ToList();
    }
}
using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EffectCards;
using Cards.InvocationCards;
using Menu;
using UnityEngine;
using UnityEngine.Serialization;

public class GameState : MonoBehaviour
{
    public List<Card> allCards;

    public List<Card> deck1AllCards;
    public List<Card> deck2AllCards;

    [FormerlySerializedAs("DeckP1")] public List<Card> deckP1;
    [FormerlySerializedAs("DeckP2")] public List<Card> deckP2;

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
            deckP1.Add(t);
        }

        for (var i = 30; i < 60; i++)
        {
            deckP2.Add(allCards[i]);
        }
    }

    public void BuildDeckForTuto()
    {
        deckP1.Clear();
        deckP2.Clear();

        var card = CardChoice.GetSpecificCard("Cliché Raciste", deck1AllCards);
        var tentacule = CardChoice.GetSpecificCard("Tentacules", deck1AllCards);
        var musicMegaDrive = CardChoice.GetSpecificCard("Musique de Mega Drive", deck1AllCards);
        var elfette = CardChoice.GetSpecificCard("L'Elfette", deck1AllCards);
        card.CardOwner = CardOwner.Player1;
        tentacule.CardOwner = CardOwner.Player1;
        musicMegaDrive.CardOwner = CardOwner.Player1;
        elfette.CardOwner = CardOwner.Player1;
        deckP1.Add(card);
        deckP1.Add(tentacule);
        deckP1.Add(musicMegaDrive);
        deckP1.Add(elfette);

        var fisti = CardChoice.GetSpecificCard("Fisti", deck2AllCards);
        fisti.CardOwner = CardOwner.Player2;
        var jeanMichelBruitage = CardChoice.GetSpecificCard("Jean-Michel Bruitages", deck2AllCards);
        jeanMichelBruitage.CardOwner = CardOwner.Player2;
        var pyroBarbare = CardChoice.GetSpecificCard("Le Pyro-Barbare", deck2AllCards);
        pyroBarbare.CardOwner = CardOwner.Player2;
        var fistiland = CardChoice.GetSpecificCard("Fistiland", deck2AllCards);
        fistiland.CardOwner = CardOwner.Player2;
        var merdeRose = CardChoice.GetSpecificCard("Merde magique en plastique rose", deck2AllCards);
        merdeRose.CardOwner = CardOwner.Player2;
        
        deckP2.Add(fisti);
        deckP2.Add(pyroBarbare);
        deckP2.Add(jeanMichelBruitage);
        deckP2.Add(fistiland);
        deckP2.Add(merdeRose);
        
        CardChoice.GetRandomDeck(30, ref deckP1, deck1AllCards, CardOwner.Player1);
        CardChoice.GetRandomDeck(30, ref deckP2, deck2AllCards, CardOwner.Player2);
    }
}
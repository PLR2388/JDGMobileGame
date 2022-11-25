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
        ResetDeckPlayer();
        deckP1.Clear();
        deckP2.Clear();

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
        

        
        CardChoice.GetRandomDeck(25, ref deckP1, deck1AllCards, CardOwner.Player1);
        deckP1.Add(fisti);
        deckP1.Add(pyroBarbare);
        deckP1.Add(jeanMichelBruitage);
        deckP1.Add(fistiland);
        deckP1.Add(merdeRose);
        deckP2.Add(tentacule);
        CardChoice.GetRandomDeck(26, ref deckP2, deck2AllCards, CardOwner.Player2);
        deckP2.Add(card);
        deckP2.Add(musicMegaDrive);
        deckP2.Add(elfette);
    }
}
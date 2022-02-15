using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EffectCards;
using Cards.InvocationCards;
using UnityEngine;
using UnityEngine.Serialization;

public class GameState : MonoBehaviour
{
    public List<Card> allCards;

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
}
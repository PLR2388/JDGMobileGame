using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameState : MonoBehaviour
{
    public List<Card> allCards;

    [FormerlySerializedAs("DeckP1")] public List<Card> deckP1;
    [FormerlySerializedAs("DeckP2")] public List<Card> deckP2;

    private static GameState _instance;

    public static readonly int maxDeckCards = 30;
    public static readonly int maxRare = 5;


    public static GameState Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameState>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void InitCards()
    {
        foreach (var card in allCards)
        {
            if (card.Type == CardType.Invocation)
            {
                var invocationCard = (InvocationCard)card;
                invocationCard.Init();
            }
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
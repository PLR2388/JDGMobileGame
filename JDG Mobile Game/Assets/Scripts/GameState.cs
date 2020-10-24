using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public List<Card> allCards;

    public List<Card> DeckP1;
    public List<Card> DeckP2;
    
    private static GameState _instance;

    public static GameState Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameState>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            DeckP1.Add(allCards[i]);
        }

        for (int i = 0; i < 30; i++)
        {
            DeckP2.Add(allCards[i]);
        }
    }
}

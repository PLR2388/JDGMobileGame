using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decide if a card can be used from the hand card menu
public class UseCard : MonoBehaviour
{
    [SerializeField] private GameObject putCardButton;

    // Start is called before the first frame update
    private void Start()
    {
        InGameMenuScript.EventClick.AddListener(LaunchCalculationCard);
    }

    private static void LaunchCalculationCard(Card card)
    {
        var type = card.Type;
        switch (type)
        {
            case CardType.Invocation: break;
            case CardType.Contre:
                break;
            case CardType.Effect:
                break;
            case CardType.Equipment:
                break;
            case CardType.Field:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decide if a card can be used from the hand card menu
public class UseCard : MonoBehaviour
{
    [SerializeField] private GameObject putCardButton;
    // Start is called before the first frame update
    void Start()
    {
        InGameMenuScript.EventClick.AddListener(LaunchCalculationCard);
    }

    public void LaunchCalculationCard(Card card)
    {
        string type = card.Type;
        switch (type)
        {
            case "invocation" : break;
        }
    }
    
    
}
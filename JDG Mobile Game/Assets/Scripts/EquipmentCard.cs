using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card",menuName="EquipmentCard")]
public class EquipmentCard : Card
{
    private void Awake()
    {
        this.type= "equipment";
    }

    public bool isEquipmentPossible()
    {
        if (GameLoop.isP1Turn)
        {
            GameObject player = GameObject.Find("Player1");
            PlayerCards currentPlayerCard = player.GetComponent<PlayerCards>();

            InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;

            return hasEnoughInvocationCard(invocationCards);

        }
        else
        {
            GameObject player = GameObject.Find("Player2");
            PlayerCards currentPlayerCard = player.GetComponent<PlayerCards>();

            InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
            
            return hasEnoughInvocationCard(invocationCards);
        }
    }

    private bool hasEnoughInvocationCard(InvocationCard[] invocationCards)
    {
        int count = 0;
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i] != null && invocationCards[i].GetNom() != null)
            {
                count++;
            } 
        }

        if (count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

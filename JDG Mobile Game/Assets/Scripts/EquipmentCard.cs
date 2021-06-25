using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card",menuName="EquipmentCard")]
public class EquipmentCard : Card
{
    
    [SerializeField] private EquipmentInstantEffect equipmentInstantEffect;
    [SerializeField] private EquipmentPermEffect equipmentPermEffect;
    private void Awake()
    {
        this.type= "equipment";
    }

    public bool isEquipmentPossible()
    {
        if (GameLoop.isP1Turn)
        {
            GameObject player = GameObject.Find("Player1");
            return HasEnoughInvocationCard(player);
        }
        else
        {
            GameObject player = GameObject.Find("Player2");
            return HasEnoughInvocationCard(player);
        }
    }

    private bool HasEnoughInvocationCard(GameObject player)
    {
        PlayerCards currentPlayerCard = player.GetComponent<PlayerCards>();

        List<InvocationCard> invocationCards = currentPlayerCard.InvocationCards;

        return hasEnoughInvocationCard(invocationCards);
    }

    private bool hasEnoughInvocationCard(List<InvocationCard> invocationCards)
    {
        int count = 0;
        for (int i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i] != null && invocationCards[i].Nom != null)
            {
                count++;
            } 
        }

        return count > 0;
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;
using Cards.InvocationCards;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "EquipmentCard")]
public class EquipmentCard : Card
{
    [SerializeField] private EquipmentInstantEffect equipmentInstantEffect;
    [SerializeField] private EquipmentPermEffect equipmentPermEffect;

    public EquipmentInstantEffect EquipmentInstantEffect => equipmentInstantEffect;
    public EquipmentPermEffect EquipmentPermEffect => equipmentPermEffect;

    private void Awake()
    {
        type = CardType.Equipment;
    }

    public bool IsEquipmentPossible()
    {
        if (GameLoop.IsP1Turn)
        {
            var player = GameObject.Find("Player1");
            return HasEnoughInvocationCard(player);
        }
        else
        {
            var player = GameObject.Find("Player2");
            return HasEnoughInvocationCard(player);
        }
    }

    private bool HasEnoughInvocationCard(GameObject player)
    {
        var currentPlayerCard = player.GetComponent<PlayerCards>();

        var invocationCards = currentPlayerCard.invocationCards;

        return HasEnoughInvocationCard(invocationCards);
    }

    private bool HasEnoughInvocationCard(IReadOnlyList<InvocationCard> invocationCards)
    {
        var count = invocationCards.Count(t => t != null && t.Nom != null && (t.GETEquipmentCard() == null || (equipmentInstantEffect!= null && equipmentInstantEffect.Keys.Contains(InstantEffect.SwitchEquipment)) ));
        return count > 0;
    }
}
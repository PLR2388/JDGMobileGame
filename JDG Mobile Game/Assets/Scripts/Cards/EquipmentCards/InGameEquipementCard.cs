using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;
using Cards.InvocationCards;
using UnityEngine;

public class InGameEquipementCard : InGameCard
{
    public EquipmentCard baseEquipmentCard;
    [SerializeField] private EquipmentInstantEffect equipmentInstantEffect;
    [SerializeField] private EquipmentPermEffect equipmentPermEffect;

    public EquipmentInstantEffect EquipmentInstantEffect => equipmentInstantEffect;
    public EquipmentPermEffect EquipmentPermEffect => equipmentPermEffect;


    public static InGameEquipementCard Init(EquipmentCard equipmentCard)
    {
        InGameEquipementCard inGameEquipmentCard = new InGameEquipementCard
        {
            baseEquipmentCard = equipmentCard
        };
        inGameEquipmentCard.Reset();
        return inGameEquipmentCard;
    }

    public void Reset()
    {
        title = baseEquipmentCard.Nom;
        description = baseEquipmentCard.Description;
        detailedDescription = baseEquipmentCard.DetailedDescription;
        baseCard = baseEquipmentCard;
        type = baseEquipmentCard.Type;
        materialCard = baseEquipmentCard.MaterialCard;
        collector = baseEquipmentCard.Collector;
        equipmentInstantEffect = baseEquipmentCard.EquipmentInstantEffect;
        equipmentPermEffect = baseEquipmentCard.EquipmentPermEffect;
    }

    // TODO: Modifiy this function as we can put an equipment card on any invocationCard on field
    /// <summary>
    /// IsEquipmentPossible.
    /// Test if user can put an equipment on at least one invocation card on field.
    /// </summary>
    public static bool IsEquipmentPossible(EquipmentInstantEffect equipmentInstantEffect)
    {
        var canSwitchEquipment = equipmentInstantEffect != null &&
                                 equipmentInstantEffect.Keys.Contains(
                                     InstantEffect.SwitchEquipment);
        var player1 = GameObject.Find("Player1");
        var player2 = GameObject.Find("Player2");
        return HasEnoughInvocationCard(player1, canSwitchEquipment) || HasEnoughInvocationCard(player2, canSwitchEquipment);
    }

    /// <summary>
    /// IsEquipmentPossible.
    /// Test if the player in parameter have enough invocation cards without equipment card or
    /// the current equipment card can change its place with another
    /// <param name="player">Player gameObject</param>
    /// </summary>
    private static bool HasEnoughInvocationCard(GameObject player, bool canSwitchEquipment)
    {
        var currentPlayerCard = player.GetComponent<PlayerCards>();

        var invocationCards = currentPlayerCard.invocationCards;

        return HasEnoughInvocationCard(invocationCards, canSwitchEquipment);
    }

    private static bool HasEnoughInvocationCard(IReadOnlyList<InGameInvocationCard> invocationCards,
        bool canSwitchEquipment)
    {
        var count = invocationCards.Count(t => t != null && t.Title != null &&
                                               (t.EquipmentCard == null || canSwitchEquipment
                                               ));
        return count > 0;
    }
}
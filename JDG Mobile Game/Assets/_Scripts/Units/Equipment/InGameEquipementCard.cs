using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EquipmentCards;
using Cards.InvocationCards;
using UnityEngine;

public class InGameEquipementCard : InGameCard
{
    private EquipmentCard baseEquipmentCard;
    private EquipmentInstantEffect equipmentInstantEffect;
    private EquipmentPermEffect equipmentPermEffect;

    public EquipmentInstantEffect EquipmentInstantEffect => equipmentInstantEffect;
    public EquipmentPermEffect EquipmentPermEffect => equipmentPermEffect;


    public static InGameEquipementCard Init(EquipmentCard equipmentCard, CardOwner cardOwner)
    {
        InGameEquipementCard inGameEquipmentCard = new InGameEquipementCard
        {
            baseEquipmentCard = equipmentCard,
            CardOwner = cardOwner
        };
        inGameEquipmentCard.Reset();
        return inGameEquipmentCard;
    }

    private void Reset()
    {
        title = baseEquipmentCard.Title;
        description = baseEquipmentCard.Description;
        detailedDescription = baseEquipmentCard.DetailedDescription;
        baseCard = baseEquipmentCard;
        type = baseEquipmentCard.Type;
        materialCard = baseEquipmentCard.MaterialCard;
        collector = baseEquipmentCard.Collector;
        equipmentInstantEffect = baseEquipmentCard.EquipmentInstantEffect;
        equipmentPermEffect = baseEquipmentCard.EquipmentPermEffect;
    }
    
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
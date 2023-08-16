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

    public List<EquipmentAbility> EquipmentAbilities = new List<EquipmentAbility>();


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
        EquipmentAbilities = baseEquipmentCard.EquipmentAbilities.Select(
            equipmentAbilityName => EquipmentAbilityLibrary.Instance.equipmentAbilityDictionary[equipmentAbilityName]
        ).ToList();
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
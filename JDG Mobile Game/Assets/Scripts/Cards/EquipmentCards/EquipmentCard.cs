using System.Collections.Generic;
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

    // TODO: Modifiy this function as we can put an equipment card on any invocationCard on field
    /// <summary>
    /// IsEquipmentPossible.
    /// Test if user can put an equipment on at least one invocation card on field.
    /// </summary>
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

    /// <summary>
    /// IsEquipmentPossible.
    /// Test if the player in parameter have enough invocation cards without equipment card or
    /// the current equipment card can change its place with another
    /// <param name="player">Player gameObject</param>
    /// </summary>
    private bool HasEnoughInvocationCard(GameObject player)
    {
        var currentPlayerCard = player.GetComponent<PlayerCards>();

        var invocationCards = currentPlayerCard.invocationCards;

        return HasEnoughInvocationCard(invocationCards);
    }

    private bool HasEnoughInvocationCard(IReadOnlyList<InvocationCard> invocationCards)
    {
        var count = invocationCards.Count(t => t != null && t.Nom != null && (t.GetEquipmentCard() == null ||
                                                                              (equipmentInstantEffect != null &&
                                                                               equipmentInstantEffect.Keys.Contains(
                                                                                   InstantEffect.SwitchEquipment))));
        return count > 0;
    }
}
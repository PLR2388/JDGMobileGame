using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "EquipmentCard")]
public class EquipmentCard : Card
{
    [SerializeField] private EquipmentInstantEffect equipmentInstantEffect;
    [SerializeField] private EquipmentPermEffect equipmentPermEffect;

    private void Awake()
    {
        this.type = "equipment";
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

    private static bool HasEnoughInvocationCard(IReadOnlyList<InvocationCard> invocationCards)
    {
        var count = invocationCards.Count(t => t != null && t.Nom != null);
        return count > 0;
    }
}
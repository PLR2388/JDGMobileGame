using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class FamilyFieldToInvocationsEffectAbility : EffectAbility
{
    private string cardName;
    private float costPerTurn;

    public FamilyFieldToInvocationsEffectAbility(EffectAbilityName name, string description, float costPerTurn, string cardName)
    {
        Name = name;
        Description = description;
        this.costPerTurn = costPerTurn;
        this.cardName = cardName;
    }

    private void ApplyPower(PlayerCards playerCards)
    {
        var fieldFamily = playerCards.FieldCard.Family;
        foreach (var invocationCard in playerCards.InvocationCards)
        {
            invocationCard.Families = new[]
            {
                fieldFamily
            };
        }
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.FieldCard != null && playerCards.InvocationCards.Count > 0;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        ApplyPower(playerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_TITLE),
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_CONTINUE_APPLY_FAMILY_MESSAGE),
                costPerTurn
            ),
            showPositiveButton: true,
            showNegativeButton: true,
            positiveAction: () =>
            {
                playerStatus.ChangePv(-costPerTurn);
            },
            negativeAction: () =>
            {
                foreach (var invocationCard in playerCards.InvocationCards)
                {
                    invocationCard.Families = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Families;
                }

                var effectCard = playerCards.EffectCards.First(effectCard => effectCard.Title == cardName);
                playerCards.EffectCards.Remove(effectCard);
                playerCards.YellowCards.Add(effectCard);
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    public override void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardAdded(playerCards, invocationCard);
        invocationCard.Families = new[]
        {
            playerCards.FieldCard.Family
        };
    }

    public override void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardRemoved(playerCards, invocationCard);
        invocationCard.Families = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Families;
    }
}
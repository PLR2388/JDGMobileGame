using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        var fieldFamily = playerCards.FieldCard.GetFamily();
        foreach (var invocationCard in playerCards.invocationCards)
        {
            invocationCard.Families = new[]
            {
                fieldFamily
            };
        }
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.FieldCard != null && playerCards.invocationCards.Count > 0;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        ApplyPower(playerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCards)
    {
        var messageBox = MessageBox.CreateSimpleMessageBox(
            canvas,
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_TITLE),
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_CONTINUE_APPLY_FAMILY_MESSAGE),
                costPerTurn
            )
        );
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            playerStatus.ChangePv(-costPerTurn);
            Object.Destroy(messageBox);
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            Object.Destroy(messageBox);
            foreach (var invocationCard in playerCards.invocationCards)
            {
                invocationCard.Families = invocationCard.baseInvocationCard.BaseInvocationCardStats.Families;
            }

            var effectCard = playerCards.effectCards.Find(effectCard => effectCard.Title == cardName);
            playerCards.effectCards.Remove(effectCard);
            playerCards.yellowCards.Add(effectCard);
        };
    }

    public override void OnInvocationCardAdded(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardAdded(playerCards, invocationCard);
        invocationCard.Families = new[]
        {
            playerCards.FieldCard.GetFamily()
        };
    }

    public override void OnInvocationCardRemoved(PlayerCards playerCards, InGameInvocationCard invocationCard)
    {
        base.OnInvocationCardRemoved(playerCards, invocationCard);
        invocationCard.Families = invocationCard.baseInvocationCard.BaseInvocationCardStats.Families;
    }
}
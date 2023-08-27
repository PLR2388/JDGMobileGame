using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetHPBackEffectAbility : EffectAbility
{
    private int numberInvocationToSacrifice;
    private float atkDefCondition;

    // 0 = MAX
    private float HPToRecover;

    public GetHPBackEffectAbility(EffectAbilityName name, string description, int numberInvocations,
        float atkDefCondition, float hpToRecover)
    {
        Name = name;
        Description = description;
        numberInvocationToSacrifice = numberInvocations;
        this.atkDefCondition = atkDefCondition;
        HPToRecover = hpToRecover;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_SACRIFICE),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.invocationCards.Count(card =>
            card.Attack >= atkDefCondition || card.Defense >= atkDefCondition) >= numberInvocationToSacrifice;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        if (numberInvocationToSacrifice == 0)
        {
            playerStatus.ChangePv(HPToRecover);
        }
        else if (numberInvocationToSacrifice == 1)
        {
            var invocationCards = new List<InGameCard>(playerCards.invocationCards
                .Where(card => card.Attack >= atkDefCondition || card.Defense >= atkDefCondition).ToList());
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SACRIFICE),
                invocationCards,
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: (card) =>
                {
                    if (card is InGameInvocationCard invocationCard)
                    {
                        playerCards.yellowCards.Add(invocationCard);
                        playerCards.invocationCards.Remove(invocationCard);
                        playerStatus.ChangePv(HPToRecover == 0 ? PlayerStatus.MaxPv : HPToRecover);
                    }
                    else
                    {
                        DisplayOkMessage(canvas);
                    }
                },
                negativeAction: () =>
                {
                    DisplayOkMessage(canvas);
                }
            );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}
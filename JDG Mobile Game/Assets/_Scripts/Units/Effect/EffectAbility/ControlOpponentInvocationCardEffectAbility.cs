using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class ControlOpponentInvocationCardEffectAbility : EffectAbility
{
    private string invocationControlled;

    public ControlOpponentInvocationCardEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.invocationCards.Count > 0;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_CONTROLLED_INVOCATION),
            new List<InGameCard>(opponentPlayerCard.invocationCards.ToList()),
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: (card) =>
            {
                if (card is InGameInvocationCard invocationCard)
                {
                    invocationControlled = invocationCard.Title;
                    invocationCard.ControlCard();
                    invocationCard.UnblockAttack();
                    opponentPlayerCard.invocationCards.Remove(invocationCard);
                    playerCards.invocationCards.Add(invocationCard);
                    //opponentPlayerCard.SendToSecretHide(card);
                    //cardLocation.AddPhysicalCard(card, GameLoop.IsP1Turn ? "P1" : "P2");
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

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        base.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCard);

        var invocationCard =
            playerCards.invocationCards.FirstOrDefault(elt => elt.Title == invocationControlled);
        if (invocationCard != null)
        {
            invocationCard.UnblockAttack();
            invocationCard.FreeCard();
            playerCards.invocationCards.Remove(invocationCard);
            opponentPlayerCard.invocationCards.Add(invocationCard);
        }
    }
}
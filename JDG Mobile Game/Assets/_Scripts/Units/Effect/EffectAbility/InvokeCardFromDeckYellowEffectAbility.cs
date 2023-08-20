using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class InvokeCardFromDeckYellowEffectAbility : EffectAbility
{
    private bool fromYellowTrash;

    public InvokeCardFromDeckYellowEffectAbility(EffectAbilityName name, string description,
        bool fromYellowTrash = false)
    {
        Name = name;
        Description = description;
        this.fromYellowTrash = fromYellowTrash;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        var hasSpaces = playerCards.invocationCards.Count < 4;
        if (fromYellowTrash)
        {
            return hasSpaces && playerCards.yellowCards.Count(elt => elt.Type == CardType.Invocation) > 0;
        }
        else
        {
            return hasSpaces;
        }
    }

    private void DisplayOkMessageBox(Transform canvas)
    {
        MessageBox.CreateOkMessageBox(
            canvas,
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD)
        );
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        if (fromYellowTrash)
        {
            var messageBox =
                MessageBox.CreateMessageBoxWithCardSelector(
                    canvas,
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOKE),
                    playerCards.yellowCards.ToList()
                );
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var invocationCard = (InGameInvocationCard)messageBox.GetComponent<MessageBox>().GetSelectedCard();
                if (invocationCard == null)
                {
                    DisplayOkMessageBox(canvas);
                }
                else
                {
                    playerCards.yellowCards.Remove(invocationCard);
                    playerCards.invocationCards.Add(invocationCard);
                    Object.Destroy(messageBox);
                }
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                DisplayOkMessageBox(canvas);
            };
        }
    }
}
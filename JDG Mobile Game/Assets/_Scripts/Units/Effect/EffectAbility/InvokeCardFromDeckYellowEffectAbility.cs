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

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        if (fromYellowTrash)
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOKE),
                playerCards.yellowCards.ToList(),
                showOkButton: true,
                okAction: (card) =>
                {
                    if (card is InGameInvocationCard invocationCard)
                    {
                        playerCards.yellowCards.Remove(invocationCard);
                        playerCards.invocationCards.Add(invocationCard);
                    }
                    else
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
                }
            );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

public class ChangeFieldCardEffectAbility : EffectAbility
{
    public ChangeFieldCardEffectAbility(EffectAbilityName name, string description, int numberTurn)
    {
        Name = name;
        Description = description;
        NumberOfTurn = numberTurn;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus)
    {
        return playerCards.deck.Any(card => card.Type == CardType.Field);
    }

    private void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_FIELD_CARD),
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        var fieldCards = playerCards.deck.Where(card => card.Type == CardType.Field).ToList();
        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_FIELD),
            fieldCards,
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: (card) =>
            {
                if (card is InGameFieldCard fieldCard)
                {
                    if (playerCards.FieldCard == null)
                    {
                        playerCards.FieldCard = fieldCard;
                    }
                    else
                    {
                        playerCards.yellowCards.Add(playerCards.FieldCard);
                        playerCards.FieldCard = fieldCard;
                    }
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
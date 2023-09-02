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
        return playerCards.Deck.Any(card => card.Type == CardType.Field);
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        var fieldCards = playerCards.Deck.Where(card => card.Type == CardType.Field).ToList();
        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_FIELD),
            fieldCards,
            showOkButton: true,
            okAction: (card) =>
            {
                if (card is InGameFieldCard fieldCard)
                {
                    if (playerCards.FieldCard == null)
                    {
                        playerCards.FieldCard = fieldCard;
                    }
                    else
                    {
                        playerCards.YellowCards.Add(playerCards.FieldCard);
                        playerCards.FieldCard = fieldCard;
                    }
                }
                else
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
            }
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }
}
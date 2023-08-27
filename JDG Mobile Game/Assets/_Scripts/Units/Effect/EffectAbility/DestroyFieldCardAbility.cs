using System.Collections.Generic;
using Cards;
using UnityEngine;

public class DestroyFieldCardAbility : EffectAbility
{
    private float costHealth;

    public DestroyFieldCardAbility(EffectAbilityName name, string description, float cost)
    {
        Name = name;
        Description = description;
        costHealth = cost;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var cards = new List<InGameCard>();
        if (playerCards.FieldCard != null)
        {
            cards.Add(playerCards.FieldCard);
        }

        if (opponentPlayerCard.FieldCard != null)
        {
            cards.Add(opponentPlayerCard.FieldCard);
        }

        var config = new CardSelectorConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_DESTROY_FIELD_CARD),
            cards,
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: (card) =>
            {
                if (card == null)
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
                else
                {
                    if (card.CardOwner == CardOwner.Player1)
                    {
                        if (playerCards.isPlayerOne)
                        {
                            playerCards.yellowCards.Add(card);
                            playerCards.FieldCard = null;
                        }
                        else
                        {
                            opponentPlayerCard.yellowCards.Add(card);
                            opponentPlayerCard.FieldCard = null;
                        }
                    }
                    else
                    {
                        if (playerCards.isPlayerOne)
                        {
                            opponentPlayerCard.yellowCards.Add(card);
                            opponentPlayerCard.FieldCard = null;
                        }
                        else
                        {
                            playerCards.yellowCards.Add(card);
                            playerCards.FieldCard = null;
                        }
                    }
                    playerStatus.ChangePv(-costHealth);
                }
            },
            negativeAction: () =>
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
        );
        CardSelector.Instance.CreateCardSelection(canvas, config);
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus opponentPlayerStatus)
    {
        return playerCards.FieldCard != null || opponentPlayerCards.FieldCard != null;
    }
}
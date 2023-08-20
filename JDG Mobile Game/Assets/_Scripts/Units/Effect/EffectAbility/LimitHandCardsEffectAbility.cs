using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class LimitHandCardsEffectAbility : EffectAbility
{
    private int numberCards;

    public LimitHandCardsEffectAbility(EffectAbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus, PlayerStatus opponentPlayerStatus)
    {
        ApplyPowerLimitHandCard(canvas, playerCards);
        ApplyPowerLimitHandCard(canvas, opponentPlayerCard);
    }

    private void ApplyPowerLimitHandCard(Transform canvas, PlayerCards playerCards)
    {
        var numberCardPlayerCard = playerCards.handCards.Count;
        if (numberCardPlayerCard < numberCards)
        {
            // Draw numberCards - numberCardPlayerCard
            var size = playerCards.deck.Count;
            var number = Math.Min(numberCards - numberCardPlayerCard, playerCards.deck.Count);
            for (var i = 0; i < number; i++)
            {
                var c = playerCards.deck[size - 1 - i];
                playerCards.handCards.Add(c);
                playerCards.deck.RemoveAt(size - 1 - i);
            }
        }
        else if (numberCardPlayerCard > numberCards)
        {
            // Prompt messageBox to remove numberCardPlayerCard - numberCard cards
            var number = numberCardPlayerCard - numberCards;
            var messageBox = MessageBox.CreateMessageBoxWithCardSelector(
                canvas,
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                    playerCards.isPlayerOne ? "1" : "2",
                    number),
                playerCards.handCards.ToList(),
                multipleCardSelection: true,
                numberCardInSelection: number
            );

            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var cards = messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();
                if (cards.Count == number)
                {
                    foreach (var inGameCard in cards)
                    {
                        playerCards.yellowCards.Add(inGameCard);
                        playerCards.handCards.Remove(inGameCard);
                    }
                    Object.Destroy(messageBox);
                }
                else
                {
                    var messageBox1 = MessageBox.CreateOkMessageBox(
                        canvas,
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                        string.Format(
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARDS),
                            number
                        )
                    );
                    messageBox1.GetComponent<MessageBox>().OkAction = () =>
                    {
                        Object.Destroy(messageBox1);
                    };
                }
            };

            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                var messageBox1 = MessageBox.CreateOkMessageBox(
                    canvas,
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_REMOVE_CARDS)
                );
                messageBox1.GetComponent<MessageBox>().OkAction = () => { Object.Destroy(messageBox1); };
            };
        }
    }
}
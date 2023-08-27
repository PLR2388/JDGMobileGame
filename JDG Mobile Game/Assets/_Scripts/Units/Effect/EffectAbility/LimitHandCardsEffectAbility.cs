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

            var config = new CardSelectorConfig(
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                    playerCards.isPlayerOne ? "1" : "2",
                    number),
                playerCards.handCards.ToList(),
                numberCardSelection: number,
                showOkButton: true,
                okMultipleAction: (cards) =>
                {
                    if (cards.Count == number)
                    {
                        foreach (var inGameCard in cards)
                        {
                            playerCards.yellowCards.Add(inGameCard);
                            playerCards.handCards.Remove(inGameCard);
                        }
                    }
                    else
                    {
                        var config = new MessageBoxConfig(
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                            string.Format(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARDS),
                                number
                            ),
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
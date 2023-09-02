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
        var numberCardPlayerCard = playerCards.HandCards.Count;
        if (numberCardPlayerCard < numberCards)
        {
            // Draw numberCards - numberCardPlayerCard
            var size = playerCards.Deck.Count;
            var number = Math.Min(numberCards - numberCardPlayerCard, playerCards.Deck.Count);
            for (var i = 0; i < number; i++)
            {
                var c = playerCards.Deck[size - 1 - i];
                playerCards.HandCards.Add(c);
                playerCards.Deck.RemoveAt(size - 1 - i);
            }
        }
        else if (numberCardPlayerCard > numberCards)
        {
            // Prompt messageBox to remove numberCardPlayerCard - numberCard cards
            var number = numberCardPlayerCard - numberCards;

            var config = new CardSelectorConfig(
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_REMOVE_CARD_FROM_HAND),
                    playerCards.IsPlayerOne ? "1" : "2",
                    number),
                playerCards.HandCards.ToList(),
                numberCardSelection: number,
                showOkButton: true,
                okMultipleAction: (cards) =>
                {
                    if (cards.Count == number)
                    {
                        foreach (var inGameCard in cards)
                        {
                            playerCards.YellowCards.Add(inGameCard);
                            playerCards.HandCards.Remove(inGameCard);
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
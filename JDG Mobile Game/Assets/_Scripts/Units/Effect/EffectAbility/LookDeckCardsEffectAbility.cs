using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class LookDeckCardsEffectAbility : EffectAbility
{
    private int numberCards;

    public LookDeckCardsEffectAbility(EffectAbilityName name, string description, int numberCards)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        return playerCards.deck.Count > 0 || opponentPlayerCard.deck.Count > 0;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_CHOICE_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_CHOICE_SEE_DECK_MESSAGE),
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: () =>
            {
                DisplayAndOrderCardMessageBox(canvas, playerCards);
            },
            negativeAction: () =>
            {
                DisplayAndOrderCardMessageBox(canvas, opponentPlayerCard);
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    private void DisplayAndOrderCardMessageBox(Transform canvas, PlayerCards playerCards)
    {
        var deck = playerCards.deck;
        if (deck.Count >= numberCards)
        {
            var shortList = new List<InGameCard>();
            for (var i = 0; i < numberCards; i++)
            {
                shortList.Add(deck[deck.Count - 1 - i]);
            }

            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHANGE_ORDER_CARTES),
                shortList,
                numberCardSelection: numberCards,
                displayOrder: true,
                showPositiveButton: true,
                showNegativeButton: true,
                positiveMultipleAction: (selectedCards) =>
                {
                    if (selectedCards.Count == numberCards)
                    {
                        for (var i = 0; i < numberCards; i++)
                        {
                            playerCards.deck.Remove(selectedCards[i]);
                        }

                        for (var i = numberCards - 1; i >= 0; i--)
                        {
                            playerCards.deck.Add(selectedCards[i]);
                        }
                    }
                    else
                    {
                        var config = new MessageBoxConfig(
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_ORDER_CARDS),
                            showOkButton: true
                        );
                        MessageBox.Instance.CreateMessageBox(
                            canvas,
                            config
                        );
                    }
                },
                negativeAction: () => {}
            );

            CardSelector.Instance.CreateCardSelection(
                canvas, config);
        }
        else
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHANGE_ORDER_CARTES),
                deck,
                numberCardSelection: deck.Count,
                displayOrder: true,
                showNegativeButton: true,
                showPositiveButton: true,
                positiveMultipleAction: (selectedCards) =>
                {
                    if (selectedCards.Count == deck.Count)
                    {
                        for (var i = 0; i < deck.Count; i++)
                        {
                            playerCards.deck.Remove(selectedCards[i]);
                        }

                        for (var i = deck.Count - 1; i >= 0; i--)
                        {
                            playerCards.deck.Add(selectedCards[i]);
                        }
                    }
                    else
                    {
                        var config = new MessageBoxConfig(
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_ORDER_CARDS),
                            showOkButton: true
                        );
                        MessageBox.Instance.CreateMessageBox(
                            canvas,
                            config
                        );
                    }
                },
                negativeAction: () => {}
                );
            
            CardSelector.Instance.CreateCardSelection(
                canvas,
                config
                );
        }
    }
}
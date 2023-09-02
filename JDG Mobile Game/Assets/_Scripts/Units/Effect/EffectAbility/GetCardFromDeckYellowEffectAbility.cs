using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class GetCardFromDeckYellowEffectAbility : EffectAbility
{
    private bool fromDeck;
    private bool fromYellow;
    private int numberCards;

    public GetCardFromDeckYellowEffectAbility(EffectAbilityName name, string description, int numberCards,
        bool fromDeck = true, bool fromYellow = false)
    {
        Name = name;
        Description = description;
        this.numberCards = numberCards;
        this.fromDeck = fromDeck;
        this.fromYellow = fromYellow;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus opponentPlayerStatus)
    {
        var number = fromDeck ? playerCards.Deck.Count : 0 + (fromYellow ? playerCards.YellowCards.Count : 0);
        return number > numberCards;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard,
        PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);
        var cards = new List<InGameCard>();
        if (fromDeck)
        {
            cards.AddRange(playerCards.Deck);
        }

        if (fromYellow)
        {
            cards.AddRange(playerCards.YellowCards);
        }

        if (numberCards == 1)
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_CARD_FROM_DECK_YELLOW),
                cards,
                showOkButton: true,
                okAction: (card) =>
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
                        if (playerCards.Deck.Contains(card))
                        {
                            playerCards.Deck.Remove(card);
                        }
                        else if (playerCards.YellowCards.Contains(card))
                        {
                            playerCards.YellowCards.Remove(card);
                        }
                        playerCards.HandCards.Add(card);
                    }
                }
            );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}
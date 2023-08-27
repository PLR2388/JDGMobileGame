using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetCardFromFamilyIfSkipDrawAbility : FieldAbility
{
    private CardFamily family;

    public GetCardFromFamilyIfSkipDrawAbility(FieldAbilityName name, string description, CardFamily family)
    {
        Name = name;
        Description = description;
        this.family = family;
    }

    private void DisplayOkMessage(Transform canvas)
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

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        base.OnTurnStart(canvas, playerCards, playerStatus);

        var validCards = playerCards.deck.Where(card =>
            card.Type == CardType.Invocation && (card as InGameInvocationCard)?.Families?.Contains(family) == true).ToList();
        validCards.AddRange(playerCards.yellowCards.Where(card =>
            card.Type == CardType.Invocation && (card as InGameInvocationCard)?.Families.Contains(family) == true));

        if (validCards.Count > 0)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_SKIP_DRAW_FOR_FISTILAND_CARD_MESSAGE),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    var config = new CardSelectorConfig(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_CARD_FROM_DECK_YELLOW),
                        validCards,
                        showNegativeButton: true,
                        showPositiveButton: true,
                        positiveAction: (selectedCard) =>
                        {
                            if (selectedCard is InGameInvocationCard invocationCard)
                            {
                                if (playerCards.yellowCards.Contains(invocationCard))
                                {
                                    playerCards.yellowCards.Remove(invocationCard);
                                    playerCards.handCards.Add(invocationCard);
                                }
                                else if (playerCards.deck.Contains(invocationCard))
                                {
                                    playerCards.deck.Remove(invocationCard);
                                    playerCards.handCards.Add(invocationCard);
                                }

                                playerCards.SkipCurrentDraw = true;
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
            );
            MessageBox.Instance.CreateMessageBox(
                canvas,
                config
            );
        }
    }
}
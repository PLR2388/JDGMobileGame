using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetFamilyInDeckAbility : Ability
{
    private CardFamily family;

    public GetFamilyInDeckAbility(AbilityName name, string description, CardFamily cardFamily)
    {
        Name = name;
        Description = description;
        family = cardFamily;
    }


    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_GET_CARD_FROM_FAMILY_MESSAGE),
                family.ToName()
            ),
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: () =>
            {
                List<InGameCard> familyCards = playerCards.deck.FindAll(card =>
                    card.Type == CardType.Invocation && (card as InGameInvocationCard)?.Families.Contains(family) == true);

                var config = new CardSelectorConfig(
                    string.Format(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_FAMILY_CARD),
                        family.ToName()
                    ),
                    familyCards,
                    showPositiveButton: true,
                    showNegativeButton: true,
                    positiveAction: (card) =>
                    {
                        if (card == null)
                        {
                            var config = new MessageBoxConfig(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD),
                                showOkButton: true,
                                okAction: () =>
                                {
                                }
                            );
                            MessageBox.Instance.CreateMessageBox(
                                canvas,
                                config
                            );
                        }
                        else
                        {
                            playerCards.deck.Remove(card);
                            playerCards.handCards.Add(card);
                        }
                    },
                    negativeAction: () =>
                    {
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
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GiveAtkDefToFamilyMemberAbility : Ability
{
    private CardFamily family;

    public GiveAtkDefToFamilyMemberAbility(AbilityName name, string description, CardFamily family)
    {
        Name = name;
        Description = description;
        IsAction = true;
        this.family = family;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_WIN_STARS_MESSAGE),
            showOkButton: true,
            okAction: () =>
            {
            }
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    public override bool IsActionPossible(PlayerCards playerCards)
    {
        var invocationCards = playerCards.invocationCards;
        return invocationCard.Receiver == null &&
               invocationCards.Any(elt => elt.Families.Contains(family) && elt.Title != invocationCard.Title);
    }

    public override void OnCardActionTouched(Transform canvas, PlayerCards playerCards, PlayerCards opponentCards)
    {
        base.OnCardActionTouched(canvas, playerCards, opponentCards);
        if (invocationCard.CancelEffect)
        {
            return;
        }
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_CONFIRM_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.ACTION_CONFIRM_TRANSFER_ATK_DEF_MESSAGE),
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: () =>
            {
                var invocationsCardsValid = new List<InGameCard>(playerCards.invocationCards.Where(inGameInvocationCard =>
                    inGameInvocationCard.Families.Contains(family) && inGameInvocationCard.Title != invocationCard.Title).ToList());
                var config = new CardSelectorConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_RECEIVER_CARD),
                    invocationsCardsValid,
                    showNegativeButton: true,
                    showPositiveButton: true,
                    positiveAction: (card) =>
                    {
                        if (card is InGameInvocationCard chosenInvocationCard)
                        {
                            // TODO: Think of a better way if dead card has more atk and def due to power-up
                            chosenInvocationCard.Attack += invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                            chosenInvocationCard.Defense += invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                            invocationCard.Attack -= invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                            invocationCard.Defense -= invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                            invocationCard.Receiver = chosenInvocationCard.Title;
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
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
        }
        if (deadCard.Receiver != null)
        {
            var receiverInvocationCard =
                playerCards.invocationCards.FirstOrDefault(card => card.Title == deadCard.Receiver);
            deadCard.Receiver = null;
            if (receiverInvocationCard == null) return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
            receiverInvocationCard.Attack -= deadCard.baseInvocationCard.BaseInvocationCardStats.Attack;
            receiverInvocationCard.Defense -= deadCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        }
        return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
    }
}
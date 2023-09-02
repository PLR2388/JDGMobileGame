using _Scripts.Units.Invocation;
using UnityEngine;

public class InvokeSpecificCardAbility : Ability
{
    private string cardName;

    public InvokeSpecificCardAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool hasCardInDeck = playerCards.Deck.Exists(card => card.Title == cardName);
        if (hasCardInDeck)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_INVOKE_SPECIFIC_CARD_MESSAGE),
                    cardName
                ),
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: () =>
                {
                    InGameInvocationCard card = playerCards.Deck.Find(card => card.Title == cardName) as InGameInvocationCard;
                    playerCards.Deck.Remove(card);
                    playerCards.InvocationCards.Add(card);
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}
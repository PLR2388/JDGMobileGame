using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class OptionalSacrificeForAtkDefAbility : Ability
{
    private string cardNameToSacrifice;

    private string requiredField;

    private float newAtk;
    private float newDef;

    public OptionalSacrificeForAtkDefAbility(AbilityName name, string description,
        string cardNameToSacrifice, string requiredField, float newAtk, float newDef)
    {
        Name = name;
        Description = description;
        this.cardNameToSacrifice = cardNameToSacrifice;
        this.requiredField = requiredField;
        this.newAtk = newAtk;
        this.newDef = newDef;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool isFieldPresent = playerCards.FieldCard?.Title == requiredField;
        bool isInvocationPresent = playerCards.InvocationCards.Any(card => card.Title == cardNameToSacrifice);

        if (isFieldPresent && isInvocationPresent)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_SACRIFICE_TO_BOOST_MESSAGE),
                    cardNameToSacrifice,
                    newAtk,
                    newDef,
                    invocationCard.Title
                ),
                showPositiveButton: true,
                showNegativeButton: true,
                positiveAction: () =>
                {
                    InGameInvocationCard invocationCardToSacrifice =
                        playerCards.InvocationCards.First(card => card.Title == cardNameToSacrifice);
                    playerCards.InvocationCards.Remove(invocationCardToSacrifice);
                    playerCards.YellowCards.Add(invocationCardToSacrifice);

                    invocationCard.Attack = newAtk;
                    invocationCard.Defense = newDef;
                }
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        if (deadCard.CancelEffect)
        {
            return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
        }
        deadCard.Attack = deadCard.baseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
    }
}
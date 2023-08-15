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
        bool isInvocationPresent = playerCards.invocationCards.Any(card => card.Title == cardNameToSacrifice);

        if (isFieldPresent && isInvocationPresent)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Augmentation de stats",
                "Veux-tu sacrifier " +
                cardNameToSacrifice + " pour augmenter l'attaque à " + newAtk + " et la défense à " + newDef + " de " +
                invocationCard.Title + " ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                InGameInvocationCard invocationCardToSacrifice =
                    playerCards.invocationCards.First(card => card.Title == cardNameToSacrifice);
                playerCards.invocationCards.Remove(invocationCardToSacrifice);
                playerCards.yellowCards.Add(invocationCardToSacrifice);

                invocationCard.Attack = newAtk;
                invocationCard.Defense = newDef;

                Object.Destroy(messageBox);
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { Object.Destroy(messageBox); };
        }
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        deadCard.Attack = deadCard.baseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        return base.OnCardDeath(canvas, deadCard, playerCards, opponentPlayerCards);
    }
}
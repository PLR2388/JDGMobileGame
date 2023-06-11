using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

public class OptionalSacrificeForAtkDefAbility : Ability
{
    private string cardName;

    private string cardNameToSacrifice;

    private string requiredField;

    private float newAtk;
    private float newDef;

    public OptionalSacrificeForAtkDefAbility(AbilityName name, string description, string cardName,
        string cardNameToSacrifice, string requiredField, float newAtk, float newDef)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
        this.cardNameToSacrifice = cardNameToSacrifice;
        this.requiredField = requiredField;
        this.newAtk = newAtk;
        this.newDef = newDef;
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        bool isFieldPresent = playerCards.field?.Title == requiredField;
        bool isInvocationPresent = playerCards.invocationCards.Any(card => card.Title == cardNameToSacrifice);

        if (isFieldPresent && isInvocationPresent)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Augmentation de stats",
                "Veux-tu sacrifier " +
                cardNameToSacrifice + " pour augmenter l'attaque à " + newAtk + " et la défense à " + newDef + " de "+ cardName +" ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                InGameInvocationCard invocationCardToSacrifice =
                    playerCards.invocationCards.First(card => card.Title == cardNameToSacrifice);
                InGameInvocationCard invocationCardToIncrement =
                    playerCards.invocationCards.First(card => card.Title == cardName);
                playerCards.invocationCards.Remove(invocationCardToSacrifice);
                playerCards.yellowTrash.Add(invocationCardToSacrifice);

                invocationCardToIncrement.Attack = newAtk;
                invocationCardToIncrement.Defense = newDef;

                Object.Destroy(messageBox);
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
            };
        }
    }

    public override void OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards)
    {
        deadCard.Attack = deadCard.baseInvocationCard.BaseInvocationCardStats.Attack;
        deadCard.Defense = deadCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        base.OnCardDeath(canvas, deadCard, playerCards);
    }
}
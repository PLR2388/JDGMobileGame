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
        bool isInvocationPresent = playerCards.invocationCards.Exists(card => card.Title == cardNameToSacrifice);

        if (isFieldPresent && isInvocationPresent)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Augmentation de stats",
                "Veux-tu sacrifier " +
                cardNameToSacrifice + " pour augmenter l'attaque à " + newAtk + " et la défense à " + newDef + " de "+ cardName +" ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                InGameInvocationCard invocationCardToSacrifice =
                    playerCards.invocationCards.Find(card => card.Title == cardNameToSacrifice);
                InGameInvocationCard invocationCardToIncrement =
                    playerCards.invocationCards.Find(card => card.Title == cardName);
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

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }
}
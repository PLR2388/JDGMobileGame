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

    private void DisplayOkMessage(Transform canvas, GameObject messageBox1, GameObject messageBox2)
    {
        var messageBox =
            MessageBox.CreateOkMessageBox(canvas, "Information", "Aucune carte n'a bénéficié des étoiles !");
        messageBox.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
            Object.Destroy(messageBox2);
        };
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
        var messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Confirmation",
            "Veux-tu transfèrer tes points d'ATK et de DEF à une carte de la même famille ?");
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var invocationsCardsValid = new List<InGameCard>(playerCards.invocationCards.Where(inGameInvocationCard =>
                inGameInvocationCard.Families.Contains(family) && inGameInvocationCard.Title != invocationCard.Title).ToList());
            var messageBox1 =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix de la carte récepteur",
                    invocationsCardsValid);
            messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var chosenInvocationCard =
                    messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                if (chosenInvocationCard == null)
                {
                    DisplayOkMessage(canvas, messageBox, messageBox1);
                }
                else
                {
                    // TODO: Think of a better way if dead card has more atk and def due to power-up
                    chosenInvocationCard.Attack += invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                    chosenInvocationCard.Defense += invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                    invocationCard.Attack -= invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                    invocationCard.Defense -= invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                    invocationCard.Receiver = chosenInvocationCard.Title;
                    Object.Destroy(messageBox);
                    Object.Destroy(messageBox1);
                }
            };
            messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
            {
                DisplayOkMessage(canvas, messageBox, messageBox1);
            };
        };
    }

    public override bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards,PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
        }
        if (deadCard.Receiver != null)
        {
            var receiverInvocationCard =
                playerCards.invocationCards.FirstOrDefault(card => card.Title == deadCard.Receiver);
            deadCard.Receiver = null;
            if (receiverInvocationCard == null) return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
            receiverInvocationCard.Attack -= deadCard.baseInvocationCard.BaseInvocationCardStats.Attack;
            receiverInvocationCard.Defense -= deadCard.baseInvocationCard.BaseInvocationCardStats.Defense;
        }
        return base.OnCardDeath(canvas, deadCard, playerCards,opponentPlayerCards);
    }
}
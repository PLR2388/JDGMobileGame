using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeCardFamilyAbility : SacrificeCardAbility
{
    private CardFamily family;

    public SacrificeCardFamilyAbility(AbilityName name, string description, CardFamily family, string cardName) : base(name, description, cardName)
    {
        this.family = family;
    }
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> validInvocationCards =
            new List<InGameCard>(
                playerCards.invocationCards.FindAll(card => card.Title != cardName && card.Families.Contains(family)));
        if (validInvocationCards.Count == 1)
        {
            InGameInvocationCard invocationCard = validInvocationCards[0] as InGameInvocationCard;
            playerCards.invocationCards.Remove(invocationCard);
            playerCards.yellowTrash.Add(invocationCard);
        }
        else
        {
            GameObject messageBox =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis une carte à sacrifier", validInvocationCards);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                InGameInvocationCard card = messageBox.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                if (card == null)
                {
                    DisplayOkMessage(canvas, messageBox);
                }
                else
                {
                    playerCards.invocationCards.Remove(card);
                    playerCards.yellowTrash.Add(card);
                }
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas, messageBox); };
        }
    }
}

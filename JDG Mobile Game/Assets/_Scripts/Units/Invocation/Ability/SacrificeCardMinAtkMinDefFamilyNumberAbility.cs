using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeCardMinAtkMinDefFamilyNumberAbility : SacrificeCardMinAtkMinDefAbility
{
    private CardFamily family;
    private int numberCard;
    public SacrificeCardMinAtkMinDefFamilyNumberAbility(AbilityName name, string description, string cardName, float atk, float def, CardFamily cardFamily, int number) : base(name, description, cardName, atk, def)
    {
        family = cardFamily;
        numberCard = number;
    }
    
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameInvocationCard> invocationCards = GetValidInvocationCards(playerCards)
            .FindAll(invocationCard => invocationCard.Families.Contains(family));
        
        if (invocationCards.Count == numberCard)
        {
            foreach (var invocationCard in invocationCards)
            {
                playerCards.invocationCards.Remove(invocationCard);
                playerCards.yellowTrash.Add(invocationCard);
            }
        }
        else
        {
            List<InGameCard> cards = new List<InGameCard>(invocationCards);
            
            GameObject messageBox =
                MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choisis les cartes à sacrifier", cards, null, null, false, true, numberCard);
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                List<InGameCard> cards = messageBox.GetComponent<MessageBox>().GetMultipleSelectedCards();
                if (cards == null)
                {
                    DisplayOkMessage(canvas, messageBox);
                }
                else if (cards.Count == numberCard)
                {
                    foreach (InGameInvocationCard invocationCard in cards)
                    {
                        playerCards.invocationCards.Remove(invocationCard);
                        playerCards.yellowTrash.Add(invocationCard);
                    }
                }
                else
                {
                    DisplayOkMessage(canvas, messageBox);
                }
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas, messageBox); };
        }
    }
}

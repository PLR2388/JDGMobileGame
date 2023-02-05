using System.Collections;
using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetSpecificCardFromDeckAbility : Ability
{
    private string cardName;

    public GetSpecificCardFromDeckAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }

    protected void GetSpecificCard(Transform canvas, PlayerCards playerCards)
    {
        bool hasCardInDeck = playerCards.deck.Exists(card => card.Title == cardName);
        if (hasCardInDeck)
        {
            GameObject messageBox =
                MessageBox.CreateSimpleMessageBox(
                    canvas, 
                    "Aller chercher carte",
                    "Veux-tu aller chercher directement " + cardName + " dans ton deck ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                InGameCard card = playerCards.deck.Find(card => card.Title == cardName);
                playerCards.deck.Remove(card);
                playerCards.handCards.Add(card);
                Object.Destroy(messageBox);
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () =>
            {
                Object.Destroy(messageBox);
            };
        }
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        GetSpecificCard(canvas, playerCards);
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
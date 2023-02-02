using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SacrificeCardMinAtkMinDefAbility : Ability
{
    private string originCardName;
    private float minAtk;
    private float minDef;


    public SacrificeCardMinAtkMinDefAbility(AbilityName name, string description, string cardName, float atk, float def)
    {
        Name = name;
        Description = description;
        originCardName = cardName;
        minAtk = atk;
        minDef = def;
    }

    protected static void DisplayOkMessage(Transform canvas, GameObject messageBox)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Attention",
            "Tu dois sélectionner une carte à sacrifier");
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            messageBox.SetActive(true);
            Object.Destroy(messageBox1);
        };
    }

    protected List<InGameInvocationCard> GetValidInvocationCards(PlayerCards playerCards)
    {
        return playerCards.invocationCards.FindAll(card =>
            card.Title != originCardName && (card.Attack >= minAtk || card.Defense >= minDef));
    }
    
    protected static void DisplaySacrificeMessageBox(Transform canvas, PlayerCards playerCards, List<InGameCard> validInvocationCards)
    {
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

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        List<InGameCard> validInvocationCards =
            new List<InGameCard>(GetValidInvocationCards(playerCards));
        DisplaySacrificeMessageBox(canvas, playerCards, validInvocationCards);
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
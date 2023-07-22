using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class ControlOpponentInvocationCardEffectAbility : EffectAbility
{
    private string invocationControlled;
    
    public ControlOpponentInvocationCardEffectAbility(EffectAbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        return opponentPlayerCard.invocationCards.Count > 0;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte");
    }

    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        var messageBox = MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix de la carte à controller",
            new List<InGameCard>(opponentPlayerCard.invocationCards.ToList()));
        messageBox.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var invocationCard = (InGameInvocationCard)messageBox.GetComponent<MessageBox>().GetSelectedCard();
            if (invocationCard == null)
            {
                DisplayOkMessage(canvas);
            }
            else
            {
                invocationControlled = invocationCard.Title;
                invocationCard.ControlCard();
                invocationCard.UnblockAttack();
                opponentPlayerCard.invocationCards.Remove(invocationCard);
                playerCards.invocationCards.Add(invocationCard);
                //opponentPlayerCard.SendToSecretHide(card);
                //cardLocation.AddPhysicalCard(card, GameLoop.IsP1Turn ? "P1" : "P2");

                Object.Destroy(messageBox);
            }
        };
        messageBox.GetComponent<MessageBox>().NegativeAction = () =>
        {
            DisplayOkMessage(canvas);
        };
    }

    public override void OnTurnStart(Transform canvas, PlayerStatus playerStatus, PlayerCards playerCards,
        PlayerStatus opponentPlayerStatus, PlayerCards opponentPlayerCard)
    {
        base.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCard);

        var invocationCard =
            playerCards.invocationCards.FirstOrDefault(elt => elt.Title == invocationControlled);
        if (invocationCard != null)
        {
            invocationCard.UnblockAttack();
            invocationCard.FreeCard();
            playerCards.invocationCards.Remove(invocationCard);
            opponentPlayerCard.invocationCards.Add(invocationCard);
        }
    }
}

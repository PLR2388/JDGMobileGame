using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class GetCardFromFamilyIfSkipDrawAbility : FieldAbility
{
    private CardFamily family;

    public GetCardFromFamilyIfSkipDrawAbility(FieldAbilityName name, string description, CardFamily family)
    {
        Name = name;
        Description = description;
        this.family = family;
    }

    private void DisplayOkMessage(Transform canvas)
    {
        MessageBox.CreateOkMessageBox(canvas, "Attention", "Tu dois choisir une carte");
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerStatus playerStatus)
    {
        base.OnTurnStart(canvas, playerCards, playerStatus);

        var validCards = playerCards.deck.Where(card =>
            card.Type == CardType.Invocation && ((InGameInvocationCard)card).Families.Contains(family)).ToList();
        validCards.AddRange(playerCards.yellowCards.Where(card =>
            card.Type == CardType.Invocation && ((InGameInvocationCard)card).Families.Contains(family)));

        if (validCards.Count > 0)
        {
            var messageBox = MessageBox.CreateSimpleMessageBox(canvas, "Action nécessaire",
                "Veux-tu sauter ta phase de pioche pour aller chercher une INVOCATION de type Fistiland dans ta pioche ou ta poubelle jaune ?");
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var messageBox1 = MessageBox.CreateMessageBoxWithCardSelector(canvas, "Choix de la carte", validCards);
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    var invocationCard = (InGameInvocationCard)messageBox1.GetComponent<MessageBox>().GetSelectedCard();
                    if (invocationCard == null)
                    {
                        DisplayOkMessage(canvas);
                    }
                    else
                    {
                        if (playerCards.yellowCards.Contains(invocationCard))
                        {
                            playerCards.yellowCards.Remove(invocationCard);
                            playerCards.handCards.Add(invocationCard);
                        }
                        else if (playerCards.deck.Contains(invocationCard))
                        {
                            playerCards.deck.Remove(invocationCard);
                            playerCards.handCards.Add(invocationCard);
                        }

                        playerCards.SkipCurrentDraw = true;
                        Object.Destroy(messageBox1);
                        Object.Destroy(messageBox);
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () => { DisplayOkMessage(canvas); };
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { Object.Destroy(messageBox); };
        }
    }
}
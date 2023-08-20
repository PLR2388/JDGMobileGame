using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

public class SkipOpponentAttackAbility : Ability
{
    public SkipOpponentAttackAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }

    protected static void DisplayOkMessage(Transform canvas, string message, GameObject messageBox,
        GameObject messageBox2)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(
            canvas,
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_TITLE),
            message
            );
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            Object.Destroy(messageBox);
            Object.Destroy(messageBox1);
            Object.Destroy(messageBox2);
        };
    }

    private void ApplyEffect(Transform canvas, PlayerCards opponentPlayerCard)
    {
        if (opponentPlayerCard.invocationCards.Count > 0)
        {
            GameObject messageBox = MessageBox.CreateSimpleMessageBox(
                canvas,
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_CHOICE_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_SKIP_OPPONENT_ATTACK_MESSAGE)
            );
            messageBox.GetComponent<MessageBox>().PositiveAction = () =>
            {
                messageBox.SetActive(false);
                List<InGameCard> list = new List<InGameCard>(opponentPlayerCard.invocationCards);
                GameObject messageBox1 =
                    MessageBox.CreateMessageBoxWithCardSelector(
                        canvas,
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_SKIP_ATTACK),
                        list
                    );
                messageBox1.GetComponent<MessageBox>().PositiveAction = () =>
                {
                    InGameInvocationCard invocationCard =
                        messageBox1.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
                    if (invocationCard == null)
                    {
                        DisplayOkMessage(
                            canvas,
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_NO_SKIP_ATTACK_MESSAGE),
                            messageBox,
                            messageBox1);
                    }
                    else
                    {
                        invocationCard.BlockAttack();
                        DisplayOkMessage(
                            canvas,
                            string.Format(
                                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.INFORMATION_OPPONENT_CANT_ATTACK_MESSAGE),
                                invocationCard.Title
                            ),
                            messageBox,
                            messageBox1
                        );
                    }
                };
                messageBox1.GetComponent<MessageBox>().NegativeAction = () =>
                {
                    Object.Destroy(messageBox);
                    Object.Destroy(messageBox1);
                };
            };
            messageBox.GetComponent<MessageBox>().NegativeAction = () => { Object.Destroy(messageBox); };
        }
    }


    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        ApplyEffect(canvas, opponentPlayerCards);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (invocationCard.CancelEffect)
        {
            return;
        }
        var tag = GameStateManager.Instance.IsP1Turn ? "card1" : "card2";
        if (tag == playerCards.Tag)
        {
            ApplyEffect(canvas, opponentPlayerCards);
        }
    }
}
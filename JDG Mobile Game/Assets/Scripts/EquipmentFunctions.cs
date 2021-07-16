using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentFunctions : MonoBehaviour
{
    [SerializeField] private GameObject miniCardMenu;
    [SerializeField] private Transform canvas;

    private void Start()
    {
        InGameMenuScript.EquipmentCardEvent.AddListener(DisplayEquipmentPopUp);
    }

    private static PlayerCards CurrentPlayerCard
    {
        get
        {
            PlayerCards currentPlayerCard = null;
            if (GameLoop.IsP1Turn)
            {
                var player = GameObject.Find("Player1");
                currentPlayerCard = player.GetComponent<PlayerCards>();
            }
            else
            {
                GameObject player = GameObject.Find("Player2");
                currentPlayerCard = player.GetComponent<PlayerCards>();
            }

            return currentPlayerCard;
        }
    }

    private void DisplayEquipmentPopUp(EquipmentCard equipmentCard)
    {
        var playerCards = CurrentPlayerCard;
        var invocationCards = playerCards.invocationCards;
        var message = DisplayEquipmentMessageBox(invocationCards);

        message.GetComponent<MessageBox>().PositiveAction = () =>
        {
            var currentSelectedInvocationCard = (InvocationCard) message.GetComponent<MessageBox>().GETSelectedCard();
            if (currentSelectedInvocationCard != null)
            {
                miniCardMenu.SetActive(false);
                currentSelectedInvocationCard.SetEquipmentCard(equipmentCard);
                playerCards.handCards.Remove(equipmentCard);
            }

            Destroy(message);
        };
        message.GetComponent<MessageBox>().NegativeAction = () =>
        {
            miniCardMenu.SetActive(false);
            Destroy(message);
        };
    }

    private static int IndexInvocationCard(IReadOnlyList<InvocationCard> invocationCards, string nameInvocationCard)
    {
        for (var i = 0; i < invocationCards.Count; i++)
        {
            if (invocationCards[i] == null) continue;
            var invocationCardName = invocationCards[i].Nom;
            if (invocationCardName == null) continue;
            if (invocationCardName == nameInvocationCard)
            {
                return i;
            }
        }

        return -1;
    }

    private GameObject DisplayEquipmentMessageBox(IReadOnlyList<InvocationCard> invocationCards)
    {
        var cards = invocationCards.Cast<Card>().ToList();
        return MessageBox.CreateMessageBoxWithCardSelector(canvas,
            "Choisis l'invocation auquelle associée l'équipement :", cards);
    }

    public void DealWithInstantEffect(InvocationCard invocationCard, EquipmentInstantEffect equipmentInstantEffect)
    {
        var keys = equipmentInstantEffect.Keys;
        var values = equipmentInstantEffect.Values;

        foreach (var key in keys)
        {
            switch (key)
            {
                case InstantEffect.AddAtk:
                    break;
                case InstantEffect.AddDef:
                {
                }
                    break;
                case InstantEffect.MultiplyAtk:
                {
                }
                    break;
                case InstantEffect.MultiplyDef:
                {
                }
                    break;
                case InstantEffect.SetAtk:
                {
                }
                    break;
                case InstantEffect.SetDef:
                {
                }
                    break;
                case InstantEffect.BlockAtk:
                {
                }
                    break;
                case InstantEffect.DirectAtk:
                {
                }
                    break;
                case InstantEffect.SwitchEquipment:
                {
                }
                    break;
                case InstantEffect.DisableBonus:
                {
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Cards.InvocationCards;
using UnityEngine;

namespace Cards.EquipmentCards
{
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
                PlayerCards currentPlayerCard;
                if (GameLoop.IsP1Turn)
                {
                    var player = GameObject.Find("Player1");
                    currentPlayerCard = player.GetComponent<PlayerCards>();
                }
                else
                {
                    var player = GameObject.Find("Player2");
                    currentPlayerCard = player.GetComponent<PlayerCards>();
                }

                return currentPlayerCard;
            }
        }

        private void DisplayEquipmentPopUp(EquipmentCard equipmentCard)
        {
            var playerCards = CurrentPlayerCard;
            var invocationCards = playerCards.invocationCards;
            var message = DisplayEquipmentMessageBox(invocationCards, equipmentCard.EquipmentInstantEffect);

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var currentSelectedInvocationCard = (InvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                if (currentSelectedInvocationCard != null)
                {
                    miniCardMenu.SetActive(false);

                    var instantEffect = equipmentCard.EquipmentInstantEffect;
                    var permEffect = equipmentCard.EquipmentPermEffect;
                
                    if (instantEffect != null)
                    {
                        DealWithInstantEffect(currentSelectedInvocationCard, instantEffect);
                    }
                
                    if (permEffect != null)
                    {
                        DealWithPermEffect(currentSelectedInvocationCard, permEffect);
                    }
                
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

        private GameObject DisplayEquipmentMessageBox(IEnumerable<InvocationCard> invocationCards, EquipmentInstantEffect equipmentInstantEffect)
        {
            var cards = new List<Card>();
            if (equipmentInstantEffect != null)
            {
                foreach (var invocationCard in invocationCards)
                {
                    if (invocationCard.GETEquipmentCard() == null)
                    {
                        cards.Add(invocationCard);
                    } else if (equipmentInstantEffect.Keys.Contains(InstantEffect.SwitchEquipment))
                    {
                        cards.Add(invocationCard);
                    }
                }
            }
            else
            {
                cards = invocationCards.Cast<Card>().ToList();
            }
        
        
            return MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Choisis l'invocation auquelle associée l'équipement :", cards);
        }

        private static void DealWithInstantEffect(InvocationCard invocationCard, EquipmentInstantEffect equipmentInstantEffect)
        {
            var keys = equipmentInstantEffect.Keys;
            var values = equipmentInstantEffect.Values;

            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case InstantEffect.AddAtk:
                    {
                        var newBonusAttack = float.Parse(values[i]) + invocationCard.GetBonusAttack();
                        invocationCard.SetBonusAttack(newBonusAttack);
                    }
                        break;
                    case InstantEffect.AddDef:
                    {
                        var newBonusDefense = float.Parse(values[i]) + invocationCard.GetBonusDefense();
                        invocationCard.SetBonusDefense(newBonusDefense);
                    }
                        break;
                    case InstantEffect.MultiplyAtk:
                    {
                        var multiplicator = int.Parse(values[i]);
                        if (multiplicator > 1)
                        {
                            var newBonusAttack = (multiplicator - 1) * invocationCard.GetAttack() + invocationCard.GetBonusAttack();
                            invocationCard.SetBonusAttack(newBonusAttack);
                        }
                    }
                        break;
                    case InstantEffect.MultiplyDef:
                    {
                        var multiplicator = int.Parse(values[i]);
                        if (multiplicator > 1)
                        {
                            var newBonusDefense = (multiplicator - 1) * invocationCard.GetDefense() + invocationCard.GetBonusDefense();
                            invocationCard.SetBonusDefense(newBonusDefense);
                        } else if (multiplicator < 0)
                        {
                            var newBonusDefense = (invocationCard.GetDefense() / multiplicator) + invocationCard.GetBonusDefense();
                            invocationCard.SetBonusDefense(newBonusDefense);
                        }
               
                    }
                        break;
                    case InstantEffect.SetAtk:
                    {
                        var specificAtk = float.Parse(values[i]);
                        var newBonusAttack = specificAtk - invocationCard.GetAttack();
                        invocationCard.SetBonusAttack(newBonusAttack);
                    }
                        break;
                    case InstantEffect.SetDef:
                    {
                        var specificDef = float.Parse(values[i]);
                        var newBonusDefense = specificDef - invocationCard.GetDefense();
                        invocationCard.SetBonusDefense(newBonusDefense);
                    }
                        break;
                    case InstantEffect.BlockAtk:
                    {
                        invocationCard.BlockAttack();
                    }
                        break;
                    case InstantEffect.SwitchEquipment:
                    {
                        if (invocationCard.GETEquipmentCard() != null)
                        {
                            CurrentPlayerCard.yellowTrash.Add(invocationCard.GETEquipmentCard());
                            invocationCard.SetEquipmentCard(null);
                        }
                    }
                        break;
                    case InstantEffect.DisableBonus:
                    {
                        invocationCard.DeactivateEffect(); // TODO: Think about it when over
                    }
                        break;
                    case InstantEffect.DirectAtk:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void DealWithPermEffect(InvocationCard invocationCard, EquipmentPermEffect equipmentPermEffect)
        {
            var keys = equipmentPermEffect.Keys;
            var values = equipmentPermEffect.Values;

            for (var i = 0; i < keys.Count; i++)
            {
                switch (keys[i])
                {
                    case PermanentEffect.AddAtkBaseOnHandCards:
                    {
                        var value = float.Parse(values[i]);
                        var newBonusAttack = value * CurrentPlayerCard.handCards.Count + invocationCard.GetBonusAttack();
                        invocationCard.SetBonusAttack(newBonusAttack);
                    }
                        break;
                    case PermanentEffect.AddDefBaseOnHandCards:
                    {
                        var value = float.Parse(values[i]);
                        var newBonusDefense = value * CurrentPlayerCard.handCards.Count + invocationCard.GetBonusDefense();
                        invocationCard.SetBonusDefense(newBonusDefense);
                    }
                        break;
                }
            }
        }
    }
}
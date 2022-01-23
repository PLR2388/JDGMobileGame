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
                        DealWithInstantEffectAddAtk(invocationCard, values[i]);
                    }
                        break;
                    case InstantEffect.AddDef:
                    {
                        DealWithInstantEffectAddDef(invocationCard, values[i]);
                    }
                        break;
                    case InstantEffect.MultiplyAtk:
                    {
                        DealWithInstantEffectMultiplyAtk(invocationCard, values[i]);
                    }
                        break;
                    case InstantEffect.MultiplyDef:
                    {
                        DealWithInstantEffectMultiplyDef(invocationCard, values[i]);
                    }
                        break;
                    case InstantEffect.SetAtk:
                    {
                        DealWithInstantEffectSetAtk(invocationCard, values[i]);
                    }
                        break;
                    case InstantEffect.SetDef:
                    {
                        DealWithInstantEffectSetDef(invocationCard, values[i]);
                    }
                        break;
                    case InstantEffect.BlockAtk:
                    {
                        invocationCard.BlockAttack();
                    }
                        break;
                    case InstantEffect.SwitchEquipment:
                    {
                        DealWithInstantEffectSwitchEquipment(invocationCard);
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

        private static void DealWithInstantEffectSwitchEquipment(InvocationCard invocationCard)
        {
            if (invocationCard.GETEquipmentCard() == null) return;
            CurrentPlayerCard.yellowTrash.Add(invocationCard.GETEquipmentCard());
            invocationCard.SetEquipmentCard(null);
        }

        private static void DealWithInstantEffectSetDef(InvocationCard invocationCard, string value)
        {
            var specificDef = float.Parse(value);
            var newBonusDefense = specificDef - invocationCard.GetDefense();
            invocationCard.SetBonusDefense(newBonusDefense);
        }

        private static void DealWithInstantEffectSetAtk(InvocationCard invocationCard, string value)
        {
            var specificAtk = float.Parse(value);
            var newBonusAttack = specificAtk - invocationCard.GetAttack();
            invocationCard.SetBonusAttack(newBonusAttack);
        }

        private static void DealWithInstantEffectMultiplyDef(InvocationCard invocationCard, string value)
        {
            var multiplicator = int.Parse(value);
            if (multiplicator > 1)
            {
                var newBonusDefense = (multiplicator - 1) * invocationCard.GetDefense() + invocationCard.GetBonusDefense();
                invocationCard.SetBonusDefense(newBonusDefense);
            }
            else if (multiplicator < 0)
            {
                var newBonusDefense = (invocationCard.GetDefense() / multiplicator) + invocationCard.GetBonusDefense();
                invocationCard.SetBonusDefense(newBonusDefense);
            }
        }

        private static void DealWithInstantEffectMultiplyAtk(InvocationCard invocationCard, string value)
        {
            var multiplicator = int.Parse(value);
            if (multiplicator > 1)
            {
                var newBonusAttack = (multiplicator - 1) * invocationCard.GetAttack() + invocationCard.GetBonusAttack();
                invocationCard.SetBonusAttack(newBonusAttack);
            }
        }

        private static void DealWithInstantEffectAddDef(InvocationCard invocationCard, string value)
        {
            var newBonusDefense = float.Parse(value) + invocationCard.GetBonusDefense();
            invocationCard.SetBonusDefense(newBonusDefense);
        }

        private static void DealWithInstantEffectAddAtk(InvocationCard invocationCard, string value)
        {
            var newBonusAttack = float.Parse(value) + invocationCard.GetBonusAttack();
            invocationCard.SetBonusAttack(newBonusAttack);
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
                        DealWithPermEffectAddAtkBaseOnHandCards(invocationCard, values[i]);
                    }
                        break;
                    case PermanentEffect.AddDefBaseOnHandCards:
                    {
                        DealWithPermEffectAddDefBaseOnHandCards(invocationCard, values[i]);
                    }
                        break;
                    case PermanentEffect.BlockOpponentDuringInvocation:
                        break;
                    case PermanentEffect.PreventAttackOnInvocation:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void DealWithPermEffectAddDefBaseOnHandCards(InvocationCard invocationCard, string value)
        {
            var floatValue = float.Parse(value);
            var newBonusDefense = floatValue * CurrentPlayerCard.handCards.Count + invocationCard.GetBonusDefense();
            invocationCard.SetBonusDefense(newBonusDefense);
        }

        private static void DealWithPermEffectAddAtkBaseOnHandCards(InvocationCard invocationCard, string value)
        {
            var floatValue = float.Parse(value);
            var newBonusAttack = floatValue * CurrentPlayerCard.handCards.Count + invocationCard.GetBonusAttack();
            invocationCard.SetBonusAttack(newBonusAttack);
        }
    }
}
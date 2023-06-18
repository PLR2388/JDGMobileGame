using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
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
            TutoInGameMenuScript.EquipmentCardEvent.AddListener(DisplayEquipmentPopUp);
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

        private static PlayerCards OpponentPlayerCard
        {
            get
            {
                PlayerCards opponentPlayerCard;
                if (GameLoop.IsP1Turn)
                {
                    var player = GameObject.Find("Player2");
                    opponentPlayerCard = player.GetComponent<PlayerCards>();
                }
                else
                {
                    var player = GameObject.Find("Player1");
                    opponentPlayerCard = player.GetComponent<PlayerCards>();
                }

                return opponentPlayerCard;
            }
        }

        /// <summary>
        /// DisplayEquipmentPopUp.
        /// Show the player invocations cards he can put equipment on.
        /// <param name="equipmentCard">equipmentCard player want to put</param>
        /// </summary>
        private void DisplayEquipmentPopUp(InGameEquipementCard equipmentCard)
        {
            var playerCards = CurrentPlayerCard;
            var opponentInvocationCards = OpponentPlayerCard.invocationCards;
            var currentInvocationCards = playerCards.invocationCards;
            var invocationCards = currentInvocationCards.Concat(opponentInvocationCards);

            var message = DisplayEquipmentMessageBox(invocationCards, equipmentCard.EquipmentInstantEffect);

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var currentSelectedInvocationCard = message.GetComponent<MessageBox>().GetSelectedCard() as InGameInvocationCard;
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

        /// <summary>
        /// DisplayEquipmentMessageBox.
        /// Show messageBox with invocationsCards that can receive the equipment card.
        /// <param name="invocationCards">invocation card allow to receive equipment card</param>
        /// <param name="equipmentInstantEffect">equipmentCard instant effect to test if it authorizes invocations with equipment</param>
        /// </summary>
        private GameObject DisplayEquipmentMessageBox(IEnumerable<InGameInvocationCard> invocationCards,
            EquipmentInstantEffect equipmentInstantEffect)
        {
            var cards = new List<InGameCard>();
            if (equipmentInstantEffect != null)
            {
                foreach (var invocationCard in invocationCards)
                {
                    if (invocationCard.EquipmentCard == null)
                    {
                        cards.Add(invocationCard);
                    }
                    else if (equipmentInstantEffect.Keys.Contains(InstantEffect.SwitchEquipment))
                    {
                        cards.Add(invocationCard);
                    }
                }
            }
            else
            {
                cards = invocationCards.Cast<InGameCard>().ToList();
            }


            return MessageBox.CreateMessageBoxWithCardSelector(canvas,
                "Choisis l'invocation auquelle associée l'équipement :", cards);
        }

        /// <summary>
        /// Apply Instant effect.
        /// <param name="invocationCards">invocation card allow to receive equipment card</param>
        /// <param name="equipmentInstantEffect">equipmentCard instant effect to test if it authorizes invocations with equipment</param>
        /// </summary>
        public static void DealWithInstantEffect(InGameInvocationCard invocationCard,
            EquipmentInstantEffect equipmentInstantEffect)
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
                        break;
                    case InstantEffect.ProtectInvocation:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Apply AddAtk Instant effect.
        /// Increment invocation's ATK
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is a number representing the number of ATK won</param>
        /// </summary>
        private static void DealWithInstantEffectAddAtk(InGameInvocationCard invocationCard, string value)
        {
            invocationCard.Attack += float.Parse(value);
        }

        /// <summary>
        /// Apply AddDef Instant effect.
        /// Increment invocation's DEF
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is a number representing the number of DEF won</param>
        /// </summary>
        private static void DealWithInstantEffectAddDef(InGameInvocationCard invocationCard, string value)
        {
            invocationCard.Defense = float.Parse(value);
        }

        /// <summary>
        /// Apply MultiplyAtk Instant effect.
        /// Multiply invocation's ATK
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is a number representing the multiplicator of ATK won</param>
        /// </summary>
        private static void DealWithInstantEffectMultiplyAtk(InGameInvocationCard invocationCard, string value)
        {
            var multiplicator = int.Parse(value);
            if (multiplicator > 1)
            {
                var newBonusAttack = (multiplicator - 1) * invocationCard.baseInvocationCard.BaseInvocationCardStats.Attack;
                invocationCard.Attack += newBonusAttack;
            }
        }

        /// <summary>
        /// Apply MultiplyAtk Instant effect.
        /// Multiply/Divide invocation's DEF
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is a number representing the multiplicator/dividor of DEF won</param>
        /// </summary>
        private static void DealWithInstantEffectMultiplyDef(InGameInvocationCard invocationCard, string value)
        {
            var multiplicator = int.Parse(value);
            if (multiplicator > 1)
            {
                var newBonusDefense =
                    (multiplicator - 1) * invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense;
                invocationCard.Defense += newBonusDefense;
            }
            else if (multiplicator < 0)
            {
                var newBonusDefense = (invocationCard.baseInvocationCard.BaseInvocationCardStats.Defense / multiplicator);
                invocationCard.Defense += newBonusDefense;
            }
        }

        /// <summary>
        /// Apply setAtk Instant effect.
        /// Fix invocation card attack
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is a number representing the fixed ATK value</param>
        /// </summary>
        private static void DealWithInstantEffectSetAtk(InGameInvocationCard invocationCard, string value)
        {
            var specificAtk = float.Parse(value);
            invocationCard.Attack = specificAtk;
        }

        /// <summary>
        /// Apply setDef Instant effect.
        /// Fix invocation card defense
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is a number representing the fixed DEF value</param>
        /// </summary>
        private static void DealWithInstantEffectSetDef(InGameInvocationCard invocationCard, string value)
        {
            var specificDef = float.Parse(value);
            invocationCard.Defense = specificDef;
        }


        /// <summary>
        /// Apply SwitchEquipment Instant effect.
        /// Remove the previous equipment card link to invocation card
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// </summary>
        private static void DealWithInstantEffectSwitchEquipment(InGameInvocationCard invocationCard)
        {
            if (invocationCard.EquipmentCard == null) return;
            CurrentPlayerCard.yellowCards.Add(invocationCard.EquipmentCard);
            invocationCard.EquipmentCard = null;
            invocationCard.SetEquipmentCard(null);
        }

        /// <summary>
        /// Apply Perm effect.
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="equipmentPermEffect">equipment perm effect of equipment card</param>
        /// </summary>
        public static void DealWithPermEffect(InGameInvocationCard invocationCard, EquipmentPermEffect equipmentPermEffect)
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

        /// <summary>
        /// Apply AddAtkBaseOnHandCards Perm effect.
        /// Add atk to the invocation card based on hand cards number
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is the atk bonus per card on hand</param>
        /// </summary>
        private static void DealWithPermEffectAddAtkBaseOnHandCards(InGameInvocationCard invocationCard, string value)
        {
            var floatValue = float.Parse(value);
            var newBonusAttack = floatValue * CurrentPlayerCard.handCards.Count;
            invocationCard.Attack += newBonusAttack;
        }

        /// <summary>
        /// Apply AddDefBaseOnHandCards Perm effect.
        /// Add def to the invocation card based on hand cards number
        /// <param name="invocationCard">invocation card that receive the equipment card</param>
        /// <param name="value">value is a string that is the def bonus per card on hand</param>
        /// </summary>
        private static void DealWithPermEffectAddDefBaseOnHandCards(InGameInvocationCard invocationCard, string value)
        {
            var floatValue = float.Parse(value);
            var newBonusDefense = floatValue * CurrentPlayerCard.handCards.Count;
            invocationCard.Defense += newBonusDefense;
        }
    }
}
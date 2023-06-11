using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.EquipmentCards;
using Cards.InvocationCards;
using UnityEngine;

namespace _Scripts.Units.Invocation
{
    public class InGameInvocationCard : InGameCard
    {
        public InvocationCard baseInvocationCard;
        protected float attack;
        protected float defense;
        private CardFamily[] families;
        private InGameEquipementCard equipmentCard;
        private InvocationConditions invocationConditions;
        private int numberTurnOnField;
        private int numberDeaths;
        private bool blockAttackNextTurn;
        private bool affectedByEffect = true;
        private int remainedAttackThisTurn;
        private bool isControlled;
        private bool cantBeAttack = false;
        private bool attrackAttack = false;

        public bool CantBeAttack
        {
            get => cantBeAttack;
            set => cantBeAttack = value;
        }

        public bool Aggro
        {
            get => attrackAttack;
            set => attrackAttack = value;
        }

        public List<global::Condition> Conditions = new List<global::Condition>();
        public List<Ability> Abilities = new List<Ability>();


        public int NumberOfTurnOnField => numberTurnOnField;

        public int NumberOfDeaths => numberDeaths;

        public InGameEquipementCard EquipmentCard
        {
            get => equipmentCard;
            set => equipmentCard = value;
        }
        
        public static InGameInvocationCard Init(InvocationCard invocationCard, CardOwner cardOwner)
        {
            InGameInvocationCard inGameInvocationCard = new InGameInvocationCard
            {
                baseInvocationCard = invocationCard,
                CardOwner = cardOwner
            };
            inGameInvocationCard.Reset();
            return inGameInvocationCard;
        }

        public CardFamily[] Families
        {
            get => families;
            set => families = value;
        }

        public float Attack
        {
            get => attack;
            set => attack = value;
        }

        public float Defense
        {
            get => defense;
            set => defense = value;
        }

        public void Reset()
        {
            title = baseInvocationCard.Title;
            description = baseInvocationCard.Description;
            detailedDescription = baseInvocationCard.DetailedDescription;
            type = baseInvocationCard.Type;
            baseCard = baseInvocationCard;
            materialCard = baseInvocationCard.MaterialCard;
            collector = baseInvocationCard.Collector;
            numberTurnOnField = 0;
            numberDeaths = 0;
            remainedAttackThisTurn = 1;
            numberTurnOnField = 0;

            isControlled = false;

            attack = baseInvocationCard.BaseInvocationCardStats.Attack;
            defense = baseInvocationCard.BaseInvocationCardStats.Defense;
            families = baseInvocationCard.BaseInvocationCardStats.Families;
            equipmentCard = null;
            IsAffectedByEffectCard = baseInvocationCard.BaseInvocationCardStats.AffectedByEffect;
            Conditions = baseInvocationCard.Conditions.Select(conditionName => ConditionLibrary.Instance.conditionDictionary[conditionName]).ToList();
            Abilities = baseInvocationCard.Abilities.Select(abilityName => AbilityLibrary.Instance.abilityDictionary[abilityName]).ToList();
        }

        public bool CanBeSummoned(PlayerCards playerCards)
        {
            return Conditions.Count == 0 || Conditions.TrueForAll(condition => condition.CanBeSummoned(playerCards));
        }

        public void DeactivateEffect()
        {
        }

        public void BlockAttack()
        {
            blockAttackNextTurn = true;
        }

        public void UnblockAttack()
        {
            blockAttackNextTurn = false;
        }

        public bool IsAffectedByEffectCard
        {
            get => affectedByEffect;
            set => affectedByEffect = value;
        }

        public void SetRemainedAttackThisTurn(int number)
        {
            remainedAttackThisTurn = number;
        }

        /// <summary>
        /// IncrementNumberDeaths.
        /// Increment the value numberDeaths by one.
        /// </summary>
        public void IncrementNumberDeaths()
        {
            numberDeaths++;
        }

        /// <summary>
        /// CanAttack.
        /// Check if an invocation card can attack.
        /// if there is no equipment, we look at remainedAttackThisTurn and blockAttackNextTurn.
        /// Otherwise, if there is an equipment card with BlockAtk InstantEffect, we had this condition to the previous one
        /// </summary>
        public bool CanAttack()
        {
            if (equipmentCard == null) return remainedAttackThisTurn > 0 && !blockAttackNextTurn;
            var instantEffect = equipmentCard.EquipmentInstantEffect;
            var equipmentBlockedAttack = false;
            if (instantEffect != null)
            {
                equipmentBlockedAttack = instantEffect.Keys.Contains(InstantEffect.BlockAtk);
            }

            return remainedAttackThisTurn > 0 && !blockAttackNextTurn & !equipmentBlockedAttack;
        }

        /// <summary>
        /// AttackTurnDone.
        /// Decrement remainedAttackThisTurn.
        /// </summary>
        public void AttackTurnDone()
        {
            remainedAttackThisTurn--;
        }

        /// <summary>
        /// ResetNewTurn.
        /// Reset remainedAttackThisTurn variable to one.
        /// </summary>
        public void ResetNewTurn()
        {
            remainedAttackThisTurn = 1;
        }

        /// <summary>
        /// IsInvocationPossible.
        /// Check if an invocation card can be put on field.
        /// </summary>
        public bool IsInvocationPossible()
        {
            var playerCards = GameLoop.IsP1Turn
                ? GameObject.Find("Player1").GetComponent<PlayerCards>()
                : GameObject.Find("Player2").GetComponent<PlayerCards>();
            return CanBeSummoned(playerCards);
        }

        /// <summary>
        /// SetEquipmentCard.
        /// Change equipment card.
        /// If user decided to remove an equipment (card = null), one should remove all equipment effect
        /// <param name="card">new equipment card</param>
        /// </summary>
        public void SetEquipmentCard(InGameEquipementCard card)
        {
            if (equipmentCard != null && card == null)
            {
                RemoveEquipmentCardEffect(equipmentCard.EquipmentInstantEffect);
            }

            equipmentCard = card;
        }

        /// <summary>
        /// RemoveEquipmentCardEffect.
        /// Disable equipment card effect on the invocation card.
        /// <param name="equipmentCardEquipmentInstantEffect">instant effect of the previous equipment card</param>
        /// </summary>
        private void RemoveEquipmentCardEffect(EquipmentInstantEffect equipmentCardEquipmentInstantEffect)
        {
            if (equipmentCardEquipmentInstantEffect == null) return;
            var keys = equipmentCardEquipmentInstantEffect.Keys;
            var values = equipmentCardEquipmentInstantEffect.Values;
            for (var i = 0; i < keys.Count; i++)
            {
                var value = values[i];
                switch (keys[i])
                {
                    case InstantEffect.AddAtk:
                    {
                        RemoveAddAtkEffect(value);
                    }
                        break;
                    case InstantEffect.AddDef:
                    {
                        RemoveAddDefEffect(value);
                    }
                        break;
                    case InstantEffect.MultiplyAtk:
                    {
                        RemoveMultiplyAtkEffect(value);
                    }
                        break;
                    case InstantEffect.MultiplyDef:
                    {
                        RemoveMultiplyDefEffect(value);
                    }
                        break;
                    case InstantEffect.SetAtk:
                    {
                        attack = baseInvocationCard.BaseInvocationCardStats.Attack;
                    }
                        break;
                    case InstantEffect.SetDef:
                    {
                        defense = baseInvocationCard.BaseInvocationCardStats.Defense;
                    }
                        break;
                    case InstantEffect.BlockAtk:
                    {
                        UnblockAttack();
                    }
                        break;
                    case InstantEffect.DirectAtk:
                        break;
                    case InstantEffect.SwitchEquipment:
                        break;
                    case InstantEffect.DisableBonus:
                        break;
                    case InstantEffect.ProtectInvocation:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// RemoveAddAtkEffect.
        /// Disable equipment card effect on the invocation card.
        /// <param name="value">value is a string that represent the bonus atk</param>
        /// </summary>
        private void RemoveAddAtkEffect(string value)
        {
            attack -= float.Parse(value);
        }

        /// <summary>
        /// RemoveAddDefEffect.
        /// Disable equipment card effect on the invocation card.
        /// <param name="value">value is a string that represent the bonus def</param>
        /// </summary>
        private void RemoveAddDefEffect(string value)
        {
            defense -= float.Parse(value);
        }

        /// <summary>
        /// RemoveMultiplyAtkEffect.
        /// Disable equipment card effect on the invocation card.
        /// <param name="value">value is a string that represent the multiplicator atk</param>
        /// </summary>
        private void RemoveMultiplyAtkEffect(string value)
        {
            var multiplicator = int.Parse(value);
            if (multiplicator > 1)
            {
                var newBonusAttack = -(multiplicator - 1) * baseInvocationCard.BaseInvocationCardStats.Attack + attack;
            }
        }

        /// <summary>
        /// RemoveMultiplyDefEffect.
        /// Disable equipment card effect on the invocation card.
        /// <param name="value">value is a string that represent the multiplicator def</param>
        /// </summary>
        private void RemoveMultiplyDefEffect(string value)
        {
            var multiplicator = int.Parse(value);
            if (multiplicator > 1)
            {
                var newBonusDefense = -(multiplicator - 1) * baseInvocationCard.BaseInvocationCardStats.Defense + defense;
            }
            else if (multiplicator < 0)
            {
                var newBonusDefense = -(baseInvocationCard.BaseInvocationCardStats.Defense / multiplicator) + defense;
            }
        }

        public bool IsControlled => isControlled;

        public void ControlCard()
        {
            isControlled = true;
        }

        public void FreeCard()
        {
            isControlled = false;
        }

        public void incrementNumberTurnOnField()
        {
            numberTurnOnField++;
        }

        public float GetCurrentDefense()
        {
            return defense;
        }

        public float GetCurrentAttack()
        {
            return attack;
        }
    }
}
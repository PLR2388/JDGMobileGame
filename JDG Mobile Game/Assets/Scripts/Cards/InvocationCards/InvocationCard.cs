using System;
using System.Collections.Generic;
using Cards.EquipmentCards;
using UnityEngine;

namespace Cards.InvocationCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
    public class InvocationCard : Card
    {
        [SerializeField] protected float attack;
        [SerializeField] protected float defense;
        [SerializeField] private CardFamily[] family;
        [SerializeField] private EquipmentCard equipmentCard;
        [SerializeField] private InvocationConditions invocationConditions;
        [SerializeField] private InvocationStartEffect invocationStartEffect;
        [SerializeField] private InvocationPermEffect invocationPermEffect;
        [SerializeField] private InvocationActionEffect invocationActionEffect;
        [SerializeField] private InvocationDeathEffect invocationDeathEffect;
        [SerializeField] private float bonusAttack;
        [SerializeField] private float bonusDefense;
        [SerializeField] private int numberTurnOnField;
        [SerializeField] private int numberDeaths;
        [SerializeField] private bool blockAttackNextTurn;
        [SerializeField] private bool affectedByEffect = true;
        [SerializeField] private int remainedAttackThisTurn;
        private CardFamily? currentFamily;
        [SerializeField] private bool isControlled;

        private void Awake()
        {
            type = CardType.Invocation;
            bonusAttack = 0;
            bonusDefense = 0;
            numberTurnOnField = 0;
            numberDeaths = 0;
            remainedAttackThisTurn = 1;
        }

        public void Init()
        {
            numberTurnOnField = 0;
            numberDeaths = 0;
            SetEquipmentCard(null);
            SetBonusAttack(0);
            SetBonusDefense(0);
            SetCurrentFamily(null);
            SetRemainedAttackThisTurn(1);
            isControlled = false;
            if (invocationActionEffect != null)
            {
                var keys = invocationActionEffect.Keys;
                var values = invocationActionEffect.Values;
                for (var i = keys.Count - 1; i >= 0; i--)
                {
                    if (keys[i] != ActionEffect.Beneficiary) continue;
                    keys.Remove(keys[i]);
                    values.Remove(values[i]);
                }
            }
        }

        public float GetAttack() => attack;
        public float GetDefense() => defense;

        public CardFamily[] GetFamily()
        {
            if (currentFamily.HasValue)
            {
                return new[]
                {
                    currentFamily.Value
                };
            }

            return family;
        }

        public EquipmentCard GetEquipmentCard() => equipmentCard;
        public InvocationConditions InvocationConditions => invocationConditions;
        public InvocationStartEffect GetInvocationStartEffect() => invocationStartEffect;
        public InvocationPermEffect InvocationPermEffect => invocationPermEffect;

        public InvocationActionEffect InvocationActionEffect
        {
            get => invocationActionEffect;
            set => invocationActionEffect = value;
        }

        public InvocationDeathEffect GetInvocationDeathEffect() => invocationDeathEffect;
        public float GetBonusAttack() => bonusAttack;
        public float GetBonusDefense() => bonusDefense;
        public int NumberTurnOnField => numberTurnOnField;
        public int GetNumberDeaths() => numberDeaths;

        public void BlockAttack()
        {
            blockAttackNextTurn = true;
        }

        public void UnblockAttack()
        {
            blockAttackNextTurn = false;
        }

        public bool IsAffectedByEffectCard => affectedByEffect;

        public void SetRemainedAttackThisTurn(int number)
        {
            remainedAttackThisTurn = number;
        }

        public void SetCurrentFamily(CardFamily? family)
        {
            currentFamily = family;
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

        public float GetCurrentDefense()
        {
            return defense + bonusDefense;
        }

        public float GetCurrentAttack()
        {
            return attack + bonusAttack;
        }

        public void incrementNumberTurnOnField()
        {
            numberTurnOnField++;
        }

        public void DeactivateEffect()
        {
        }

        /// <summary>
        /// SetBonusDefense.
        /// Change bonus defense.
        /// Prevent user from modifying its bonus if he equips invocation card with equipment card "Set Def"
        /// <param name="bonus">bonus defense</param>
        /// </summary>
        public void SetBonusDefense(float bonus)
        {
            bonusDefense = bonus;
            if (equipmentCard == null) return;
            var instantEffect = equipmentCard.EquipmentInstantEffect;
            if (instantEffect == null) return;
            if (!instantEffect.Keys.Contains(InstantEffect.SetDef)) return;
            var index = instantEffect.Keys.IndexOf(InstantEffect.SetDef);
            var def = float.Parse(instantEffect.Values[index]);
            bonusDefense = def - GetDefense();
        }

        /// <summary>
        /// SetBonusAttack.
        /// Change bonus attack.
        /// Prevent user from modifying its bonus if he equips invocation card with equipment card "Set Atk"
        /// <param name="bonus">bonus attack</param>
        /// </summary>
        public void SetBonusAttack(float bonus)
        {
            bonusAttack = bonus;
            if (equipmentCard == null) return;
            var instantEffect = equipmentCard.EquipmentInstantEffect;
            if (instantEffect == null) return;
            if (!instantEffect.Keys.Contains(InstantEffect.SetAtk)) return;
            var index = instantEffect.Keys.IndexOf(InstantEffect.SetAtk);
            var atk = float.Parse(instantEffect.Values[index]);
            bonusAttack = atk - GetAttack();
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
        /// SetEquipmentCard.
        /// Change equipment card.
        /// If user decided to remove an equipment (card = null), one should remove all equipment effect
        /// <param name="card">new equipment card</param>
        /// </summary>
        public void SetEquipmentCard(EquipmentCard card)
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
                        SetBonusAttack(0);
                    }
                        break;
                    case InstantEffect.SetDef:
                    {
                        SetBonusDefense(0);
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
            var newBonusAttack = -float.Parse(value) + GetBonusAttack();
            SetBonusAttack(newBonusAttack);
        }

        /// <summary>
        /// RemoveAddDefEffect.
        /// Disable equipment card effect on the invocation card.
        /// <param name="value">value is a string that represent the bonus def</param>
        /// </summary>
        private void RemoveAddDefEffect(string value)
        {
            var newBonusDefense = -float.Parse(value) + GetBonusDefense();
            SetBonusDefense(newBonusDefense);
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
                var newBonusAttack = -(multiplicator - 1) * GetAttack() + GetBonusAttack();
                SetBonusAttack(newBonusAttack);
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
                var newBonusDefense = -(multiplicator - 1) * GetDefense() + GetBonusDefense();
                SetBonusDefense(newBonusDefense);
            }
            else if (multiplicator < 0)
            {
                var newBonusDefense = -(GetDefense() / multiplicator) + GetBonusDefense();
                SetBonusDefense(newBonusDefense);
            }
        }

        /// <summary>
        /// IsInvocationPossible.
        /// Check if an invocation card can be put on field.
        /// </summary>
        public bool IsInvocationPossible()
        {
            return InvocationFunctions.IsInvocationPossible(invocationConditions,
                GameLoop.IsP1Turn ? "Player1" : "Player2");
        }
    }
}
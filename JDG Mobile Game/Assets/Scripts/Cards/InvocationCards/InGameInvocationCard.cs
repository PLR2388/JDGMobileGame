using System;
using Cards.EquipmentCards;
using UnityEngine;

namespace Cards.InvocationCards
{
    public class InGameInvocationCard : InGameCard
    {
        public InvocationCard baseInvocationCard;
        [SerializeField] protected float attack;
        [SerializeField] protected float defense;
        [SerializeField] private CardFamily[] families;
        [SerializeField] private InGameEquipementCard equipmentCard;
        [SerializeField] private InvocationConditions invocationConditions;
        [SerializeField] private InvocationStartEffect invocationStartEffect;
        [SerializeField] private InvocationPermEffect invocationPermEffect;
        [SerializeField] private InvocationActionEffect invocationActionEffect;
        [SerializeField] private InvocationDeathEffect invocationDeathEffect;
        [SerializeField] private int numberTurnOnField;
        [SerializeField] private int numberDeaths;
        [SerializeField] private bool blockAttackNextTurn;
        [SerializeField] private bool affectedByEffect = true;
        [SerializeField] private int remainedAttackThisTurn;
        [SerializeField] private bool isControlled;

        public int NumberOfTurnOnField => numberTurnOnField;

        public int NumberOfDeaths => numberDeaths;

        public InvocationConditions InvocationConditions => invocationConditions;
        public InvocationStartEffect InvocationStartEffect => invocationStartEffect;
        public InvocationPermEffect InvocationPermEffect => invocationPermEffect;

        public InvocationActionEffect InvocationActionEffect
        {
            get => invocationActionEffect;
            set => invocationActionEffect = value;
        }
        public InvocationDeathEffect InvocationDeathEffect => invocationDeathEffect;

        public InGameEquipementCard EquipmentCard
        {
            get => equipmentCard;
            set => equipmentCard = value;
        }
        
        public static InGameInvocationCard Init(InvocationCard invocationCard)
        {
            InGameInvocationCard inGameInvocationCard = new InGameInvocationCard
            {
                baseInvocationCard = invocationCard
            };
            inGameInvocationCard.Reset();
            return inGameInvocationCard;
        }

        private void Awake()
        {
            Reset();
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
            title = baseInvocationCard.Nom;
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

            attack = baseInvocationCard.GetAttack();
            defense = baseInvocationCard.GetDefense();
            families = baseInvocationCard.GetFamily();
            equipmentCard = null;
            invocationConditions = baseInvocationCard.InvocationConditions;
            invocationStartEffect = baseInvocationCard.GetInvocationStartEffect();
            invocationPermEffect = baseInvocationCard.InvocationPermEffect;
            invocationActionEffect = baseInvocationCard.InvocationActionEffect;
            invocationDeathEffect = baseInvocationCard.GetInvocationDeathEffect();
        }

        public void DeactivateEffect()
        {
            invocationConditions = null;
            invocationStartEffect = null;
            invocationPermEffect = null;
            invocationActionEffect = null;
            invocationDeathEffect = null;
        }

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
            return InvocationFunctions.IsInvocationPossible(invocationConditions,
                GameLoop.IsP1Turn ? "Player1" : "Player2");
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
                        attack = baseInvocationCard.GetAttack();
                    }
                        break;
                    case InstantEffect.SetDef:
                    {
                        defense = baseInvocationCard.GetDefense();
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
                var newBonusAttack = -(multiplicator - 1) * baseInvocationCard.GetAttack() + attack;
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
                var newBonusDefense = -(multiplicator - 1) * baseInvocationCard.GetDefense() + defense;
            }
            else if (multiplicator < 0)
            {
                var newBonusDefense = -(baseInvocationCard.GetDefense() / multiplicator) + defense;
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

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
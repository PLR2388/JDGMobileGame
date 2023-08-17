using System.Collections.Generic;
using System.Linq;
using Cards;
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
        private int numberTurnOnField;
        private int numberDeaths;
        private bool blockAttackNextTurn;
        private bool affectedByEffect = true;
        private int remainedAttackThisTurn;
        private bool isControlled;
        private bool cantBeAttack = false;
        private bool attrackAttack = false;
        private string receiver = null;
        private bool cancelEffect = false;

        private bool directAttackPossible = false;

        public bool CancelEffect
        {
            get => cancelEffect;
            set
            {
                cancelEffect = value;
                InvocationFunctions.cancelInvocationEvent.Invoke(this);
            }
        }

        public bool CanDirectAttack
        {
            get => directAttackPossible;
            set => directAttackPossible = value;
        }

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

        /// <summary>
        /// This represent the card that receive the power from this card
        /// only use for L'elfette and Sangoku right now that give their
        /// atk and def
        /// </summary>
        public string Receiver
        {
            get => receiver;
            set => receiver = value;
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

            isControlled = false;

            attack = baseInvocationCard.BaseInvocationCardStats.Attack;
            defense = baseInvocationCard.BaseInvocationCardStats.Defense;
            families = baseInvocationCard.BaseInvocationCardStats.Families;
            equipmentCard = null;
            IsAffectedByEffectCard = baseInvocationCard.BaseInvocationCardStats.AffectedByEffect;
            Conditions = baseInvocationCard.Conditions
                .Select(conditionName => ConditionLibrary.Instance.conditionDictionary[conditionName]).ToList();
            Abilities = baseInvocationCard.Abilities
                .Select(abilityName => AbilityLibrary.Instance.abilityDictionary[abilityName]).ToList();
            foreach (var ability in Abilities)
            {
                ability.InvocationCard = this;
            }
        }

        public bool CanBeSummoned(PlayerCards playerCards)
        {
            return Conditions.Count == 0 || Conditions.TrueForAll(condition => condition.CanBeSummoned(playerCards));
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
            /*var instantEffect = equipmentCard.EquipmentInstantEffect;
            var equipmentBlockedAttack = false;
            if (instantEffect != null)
            {
                equipmentBlockedAttack = instantEffect.Keys.Contains(InstantEffect.BlockAtk);
            }*/

            return remainedAttackThisTurn > 0 && !blockAttackNextTurn; //& !equipmentBlockedAttack;
        }

        /// <summary>
        /// HasAction.
        /// Return true if an in-game action is available for this card
        /// </summary>
        public bool HasAction()
        {
            return Abilities.Exists(elt => elt.IsAction);
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
                // RemoveEquipmentCardEffect(equipmentCard.EquipmentInstantEffect);
            }

            equipmentCard = card;
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
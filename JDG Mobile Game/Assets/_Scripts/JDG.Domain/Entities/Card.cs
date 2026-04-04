using System;
using System.Collections.Generic;
using System.Linq;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Entities
{
    /// <summary>
    /// Domain entity representing a card in the game.
    /// Pure C# - no Unity dependencies.
    /// ECS-ready: Structured to easily convert to ECS components.
    /// Supports all card types: Invocation, Equipment, Field, Effect, Contre.
    /// </summary>
    public class Card
    {
        // Identity (ECS: Entity ID)
        public CardId Id { get; }

        // Base Card Properties
        public string Title { get; }
        public string Description { get; }
        public string DetailedDescription { get; }
        public CardType Type { get; }
        public bool IsCollector { get; }
        public CardOwner Owner { get; private set; }

        // Invocation Card Properties (null for non-invocation cards)
        public CardStats? Stats { get; private set; }
        private List<CardFamily> _families;
        public IReadOnlyList<CardFamily> Families => _families.AsReadOnly();
        public bool AffectedByEffect { get; }
        public IReadOnlyList<ConditionName> Conditions { get; }
        public IReadOnlyList<AbilityName> Abilities { get; }

        // Runtime State (mutable during gameplay)
        public int TimesRevived { get; private set; }
        public bool CancelEffect { get; private set; }
        public bool CanDirectAttack { get; private set; }
        public bool AttackBlocked { get; private set; }
        public bool CantBeAttacked { get; private set; }
        public int BonusAttacks { get; private set; }

        // Equipment Card Properties
        public IReadOnlyList<EquipmentAbilityName> EquipmentAbilities { get; }

        // Field Card Properties
        public CardFamily? FieldFamily { get; }
        public IReadOnlyList<FieldAbilityName> FieldAbilities { get; }

        // Effect Card Properties
        public IReadOnlyList<EffectAbilityName> EffectAbilities { get; }

        // Private constructor - use factory methods instead
        private Card(
            CardId id,
            string title,
            string description,
            string detailedDescription,
            CardType type,
            bool isCollector,
            CardOwner owner = CardOwner.NotDefined,
            CardStats? stats = null,
            IEnumerable<CardFamily> families = null,
            bool affectedByEffect = false,
            IEnumerable<ConditionName> conditions = null,
            IEnumerable<AbilityName> abilities = null,
            IEnumerable<EquipmentAbilityName> equipmentAbilities = null,
            CardFamily? fieldFamily = null,
            IEnumerable<FieldAbilityName> fieldAbilities = null,
            IEnumerable<EffectAbilityName> effectAbilities = null)
        {
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? "";
            DetailedDescription = detailedDescription ?? "";
            Type = type;
            IsCollector = isCollector;
            Owner = owner;
            Stats = stats;
            _families = families?.ToList() ?? new List<CardFamily>();
            AffectedByEffect = affectedByEffect;
            Conditions = conditions?.ToList().AsReadOnly() ?? new List<ConditionName>().AsReadOnly();
            Abilities = abilities?.ToList().AsReadOnly() ?? new List<AbilityName>().AsReadOnly();
            EquipmentAbilities = equipmentAbilities?.ToList().AsReadOnly() ?? new List<EquipmentAbilityName>().AsReadOnly();
            FieldFamily = fieldFamily;
            FieldAbilities = fieldAbilities?.ToList().AsReadOnly() ?? new List<FieldAbilityName>().AsReadOnly();
            EffectAbilities = effectAbilities?.ToList().AsReadOnly() ?? new List<EffectAbilityName>().AsReadOnly();

            // Initialize runtime state
            TimesRevived = 0;
            CancelEffect = false;
        }

        #region Factory Methods

        /// <summary>
        /// Creates an Invocation card.
        /// Phase 159: Changed attack/defense from int to float for half-star support.
        /// </summary>
        public static Card CreateInvocation(
            CardId id,
            string title,
            string description,
            string detailedDescription,
            float attack,
            float defense,
            IEnumerable<CardFamily> families,
            bool affectedByEffect,
            IEnumerable<ConditionName> conditions = null,
            IEnumerable<AbilityName> abilities = null,
            bool isCollector = false)
        {
            return new Card(
                id: id,
                title: title,
                description: description,
                detailedDescription: detailedDescription,
                type: CardType.Invocation,
                isCollector: isCollector,
                stats: new CardStats(attack, defense),
                families: families,
                affectedByEffect: affectedByEffect,
                conditions: conditions,
                abilities: abilities);
        }

        /// <summary>
        /// Creates an Equipment card.
        /// </summary>
        public static Card CreateEquipment(
            CardId id,
            string title,
            string description,
            string detailedDescription,
            IEnumerable<EquipmentAbilityName> equipmentAbilities,
            bool isCollector = false)
        {
            return new Card(
                id: id,
                title: title,
                description: description,
                detailedDescription: detailedDescription,
                type: CardType.Equipment,
                isCollector: isCollector,
                equipmentAbilities: equipmentAbilities);
        }

        /// <summary>
        /// Creates a Field card.
        /// </summary>
        public static Card CreateField(
            CardId id,
            string title,
            string description,
            string detailedDescription,
            CardFamily fieldFamily,
            IEnumerable<FieldAbilityName> fieldAbilities,
            bool isCollector = false)
        {
            return new Card(
                id: id,
                title: title,
                description: description,
                detailedDescription: detailedDescription,
                type: CardType.Field,
                isCollector: isCollector,
                fieldFamily: fieldFamily,
                fieldAbilities: fieldAbilities);
        }

        /// <summary>
        /// Creates an Effect card.
        /// </summary>
        public static Card CreateEffect(
            CardId id,
            string title,
            string description,
            string detailedDescription,
            IEnumerable<EffectAbilityName> effectAbilities,
            bool isCollector = false)
        {
            return new Card(
                id: id,
                title: title,
                description: description,
                detailedDescription: detailedDescription,
                type: CardType.Effect,
                isCollector: isCollector,
                effectAbilities: effectAbilities);
        }

        /// <summary>
        /// Creates a Contre (Counter) card.
        /// </summary>
        public static Card CreateContre(
            CardId id,
            string title,
            string description,
            string detailedDescription,
            bool isCollector = false)
        {
            return new Card(
                id: id,
                title: title,
                description: description,
                detailedDescription: detailedDescription,
                type: CardType.Contre,
                isCollector: isCollector);
        }

        #endregion

        #region Card State Modifications

        /// <summary>
        /// Sets the owner of this card.
        /// </summary>
        public void SetOwner(CardOwner owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Modifies the stats of an invocation card.
        /// Returns true if successful, false if not an invocation card.
        /// Phase 159: Changed from int to float for half-star support.
        /// </summary>
        public bool ModifyStats(float attackDelta, float defenseDelta)
        {
            if (!Stats.HasValue || Type != CardType.Invocation)
                return false;

            Stats = Stats.Value.Modify(attackDelta, defenseDelta);
            return true;
        }

        /// <summary>
        /// Sets the stats of an invocation card.
        /// Returns true if successful, false if not an invocation card.
        /// Phase 159: Changed from int to float for half-star support.
        /// </summary>
        public bool SetStats(float attack, float defense)
        {
            if (!Stats.HasValue || Type != CardType.Invocation)
                return false;

            Stats = new CardStats(attack, defense);
            return true;
        }

        /// <summary>
        /// Resets stats to base values (if you track base stats separately).
        /// For now, this is a placeholder for future functionality.
        /// </summary>
        public void ResetStats(CardStats baseStats)
        {
            if (Stats.HasValue && Type == CardType.Invocation)
            {
                Stats = baseStats;
            }
        }

        /// <summary>
        /// Increments the times this card has been revived.
        /// Used for resurrection abilities with limited revive counts.
        /// </summary>
        public void IncrementTimesRevived()
        {
            TimesRevived++;
        }

        /// <summary>
        /// Resets the revive counter to zero.
        /// </summary>
        public void ResetTimesRevived()
        {
            TimesRevived = 0;
        }

        /// <summary>
        /// Sets whether card abilities are canceled.
        /// Used by equipment that cancels invocation abilities.
        /// </summary>
        public void SetCancelEffect(bool canceled)
        {
            CancelEffect = canceled;
        }

        /// <summary>
        /// Enables direct attack for this card (can attack player directly).
        /// </summary>
        public void EnableDirectAttack()
        {
            if (Type == CardType.Invocation)
                CanDirectAttack = true;
        }

        /// <summary>
        /// Disables direct attack for this card.
        /// </summary>
        public void DisableDirectAttack()
        {
            CanDirectAttack = false;
        }

        /// <summary>
        /// Blocks this card from attacking (for current turn).
        /// </summary>
        public void BlockAttack()
        {
            if (Type == CardType.Invocation)
                AttackBlocked = true;
        }

        /// <summary>
        /// Unblocks this card's attack ability.
        /// </summary>
        public void UnblockAttack()
        {
            AttackBlocked = false;
        }

        /// <summary>
        /// Sets whether this card can be attacked by other cards.
        /// Used by equipment that protects from invocation attacks.
        /// </summary>
        public void SetCantBeAttacked(bool cantBeAttacked)
        {
            if (Type == CardType.Invocation)
                CantBeAttacked = cantBeAttacked;
        }

        /// <summary>
        /// Sets the number of bonus attacks for this card.
        /// </summary>
        public void SetBonusAttacks(int bonus)
        {
            if (Type == CardType.Invocation)
                BonusAttacks = bonus;
        }

        /// <summary>
        /// Resets turn-based runtime state (attack blocked, bonus attacks, cant be attacked).
        /// Called at start of each turn.
        /// </summary>
        public void ResetTurnState()
        {
            AttackBlocked = false;
            CantBeAttacked = false;
            BonusAttacks = 0;
        }

        /// <summary>
        /// Changes the families of this card.
        /// Used by field abilities that change card families.
        /// </summary>
        public void SetFamilies(IEnumerable<CardFamily> families)
        {
            _families.Clear();
            _families.AddRange(families);
        }

        #endregion

        #region Queries

        /// <summary>
        /// Checks if this card is an invocation card.
        /// </summary>
        public bool IsInvocation => Type == CardType.Invocation;

        /// <summary>
        /// Checks if this card is an equipment card.
        /// </summary>
        public bool IsEquipment => Type == CardType.Equipment;

        /// <summary>
        /// Checks if this card is a field card.
        /// </summary>
        public bool IsField => Type == CardType.Field;

        /// <summary>
        /// Checks if this card is an effect card.
        /// </summary>
        public bool IsEffect => Type == CardType.Effect;

        /// <summary>
        /// Checks if this card is a contre (counter) card.
        /// </summary>
        public bool IsContre => Type == CardType.Contre;

        /// <summary>
        /// Checks if this card belongs to a specific family.
        /// </summary>
        public bool HasFamily(CardFamily family)
        {
            return Families.Contains(family) || Families.Contains(CardFamily.Any);
        }

        /// <summary>
        /// Checks if this card has a specific ability.
        /// </summary>
        public bool HasAbility(AbilityName ability)
        {
            return Abilities.Contains(ability);
        }

        /// <summary>
        /// Checks if this card has a specific condition.
        /// </summary>
        public bool HasCondition(ConditionName condition)
        {
            return Conditions.Contains(condition);
        }

        /// <summary>
        /// Returns true if the card is destroyed (defense <= 0 for invocation cards).
        /// </summary>
        public bool IsDestroyed => IsInvocation && Stats.HasValue && Stats.Value.Defense <= 0;

        #endregion

        #region ECS Snapshot (for future ECS migration)

        /// <summary>
        /// Creates a snapshot of card state (for ECS conversion or serialization).
        /// </summary>
        public CardSnapshot CreateSnapshot()
        {
            return new CardSnapshot
            {
                Id = Id,
                Title = Title,
                Type = Type,
                Owner = Owner,
                Attack = Stats?.Attack ?? 0,
                Defense = Stats?.Defense ?? 0,
                FamilyCount = Families.Count,
                AbilityCount = Abilities.Count + EquipmentAbilities.Count + FieldAbilities.Count + EffectAbilities.Count,
                IsDestroyed = IsDestroyed
            };
        }

        #endregion

        public override string ToString()
        {
            if (IsInvocation && Stats.HasValue)
            {
                return $"{Title} ({Type}) - ATK: {Stats.Value.Attack}, DEF: {Stats.Value.Defense}";
            }
            return $"{Title} ({Type})";
        }
    }

    /// <summary>
    /// Read-only snapshot of card state.
    /// Can be used for UI, serialization, or ECS conversion.
    /// Phase 159: Changed Attack/Defense from int to float for half-star support.
    /// </summary>
    public struct CardSnapshot
    {
        public CardId Id;
        public string Title;
        public CardType Type;
        public CardOwner Owner;
        public float Attack;
        public float Defense;
        public int FamilyCount;
        public int AbilityCount;
        public bool IsDestroyed;
    }
}

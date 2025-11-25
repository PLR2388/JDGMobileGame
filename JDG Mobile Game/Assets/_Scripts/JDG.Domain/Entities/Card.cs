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
        public IReadOnlyList<CardFamily> Families { get; }
        public bool AffectedByEffect { get; }
        public IReadOnlyList<ConditionName> Conditions { get; }
        public IReadOnlyList<AbilityName> Abilities { get; }

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
            Families = families?.ToList().AsReadOnly() ?? new List<CardFamily>().AsReadOnly();
            AffectedByEffect = affectedByEffect;
            Conditions = conditions?.ToList().AsReadOnly() ?? new List<ConditionName>().AsReadOnly();
            Abilities = abilities?.ToList().AsReadOnly() ?? new List<AbilityName>().AsReadOnly();
            EquipmentAbilities = equipmentAbilities?.ToList().AsReadOnly() ?? new List<EquipmentAbilityName>().AsReadOnly();
            FieldFamily = fieldFamily;
            FieldAbilities = fieldAbilities?.ToList().AsReadOnly() ?? new List<FieldAbilityName>().AsReadOnly();
            EffectAbilities = effectAbilities?.ToList().AsReadOnly() ?? new List<EffectAbilityName>().AsReadOnly();
        }

        #region Factory Methods

        /// <summary>
        /// Creates an Invocation card.
        /// </summary>
        public static Card CreateInvocation(
            CardId id,
            string title,
            string description,
            string detailedDescription,
            int attack,
            int defense,
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
        /// </summary>
        public bool ModifyStats(int attackDelta, int defenseDelta)
        {
            if (!Stats.HasValue || Type != CardType.Invocation)
                return false;

            Stats = Stats.Value.Modify(attackDelta, defenseDelta);
            return true;
        }

        /// <summary>
        /// Sets the stats of an invocation card.
        /// Returns true if successful, false if not an invocation card.
        /// </summary>
        public bool SetStats(int attack, int defense)
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
    /// </summary>
    public struct CardSnapshot
    {
        public CardId Id;
        public string Title;
        public CardType Type;
        public CardOwner Owner;
        public int Attack;
        public int Defense;
        public int FamilyCount;
        public int AbilityCount;
        public bool IsDestroyed;
    }
}

using System;

namespace JDG.Domain.ValueObjects
{
    /// <summary>
    /// Immutable value object representing card attack and defense statistics.
    /// ECS-ready: Structured to convert easily to IComponentData.
    /// </summary>
    public readonly struct CardStats : IEquatable<CardStats>
    {
        public int Attack { get; }
        public int Defense { get; }

        public CardStats(int attack, int defense)
        {
            Attack = attack;
            Defense = defense;
        }

        /// <summary>
        /// Creates new stats with modified values.
        /// Immutable - returns a new instance.
        /// </summary>
        public CardStats Modify(int attackDelta, int defenseDelta)
        {
            return new CardStats(Attack + attackDelta, Defense + defenseDelta);
        }

        /// <summary>
        /// Creates new stats with set values (replaces current).
        /// </summary>
        public CardStats WithAttack(int newAttack)
        {
            return new CardStats(newAttack, Defense);
        }

        /// <summary>
        /// Creates new stats with set values (replaces current).
        /// </summary>
        public CardStats WithDefense(int newDefense)
        {
            return new CardStats(Attack, newDefense);
        }

        /// <summary>
        /// Adds attack and defense from another CardStats.
        /// </summary>
        public CardStats Add(CardStats other)
        {
            return new CardStats(Attack + other.Attack, Defense + other.Defense);
        }

        /// <summary>
        /// Subtracts attack and defense from another CardStats.
        /// </summary>
        public CardStats Subtract(CardStats other)
        {
            return new CardStats(Attack - other.Attack, Defense - other.Defense);
        }

        public bool Equals(CardStats other)
        {
            return Attack == other.Attack && Defense == other.Defense;
        }

        public override bool Equals(object obj)
        {
            return obj is CardStats other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Attack * 397) ^ Defense;
            }
        }

        public static bool operator ==(CardStats left, CardStats right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CardStats left, CardStats right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"ATK: {Attack}, DEF: {Defense}";
        }

        public static CardStats Zero => new CardStats(0, 0);
    }
}

using System;

namespace JDG.Domain.ValueObjects
{
    /// <summary>
    /// Immutable value object representing card attack and defense statistics.
    /// ECS-ready: Structured to convert easily to IComponentData.
    /// Phase 159: Changed from int to float to support half-star values (e.g., 2.5 ATK).
    /// </summary>
    public readonly struct CardStats : IEquatable<CardStats>
    {
        public float Attack { get; }
        public float Defense { get; }

        public CardStats(float attack, float defense)
        {
            Attack = attack;
            Defense = defense;
        }

        /// <summary>
        /// Creates new stats with modified values.
        /// Immutable - returns a new instance.
        /// </summary>
        public CardStats Modify(float attackDelta, float defenseDelta)
        {
            return new CardStats(Attack + attackDelta, Defense + defenseDelta);
        }

        /// <summary>
        /// Creates new stats with set values (replaces current).
        /// </summary>
        public CardStats WithAttack(float newAttack)
        {
            return new CardStats(newAttack, Defense);
        }

        /// <summary>
        /// Creates new stats with set values (replaces current).
        /// </summary>
        public CardStats WithDefense(float newDefense)
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
            // Use approximate equality for floats
            const float epsilon = 0.0001f;
            return Math.Abs(Attack - other.Attack) < epsilon &&
                   Math.Abs(Defense - other.Defense) < epsilon;
        }

        public override bool Equals(object obj)
        {
            return obj is CardStats other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Attack.GetHashCode() * 397) ^ Defense.GetHashCode();
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

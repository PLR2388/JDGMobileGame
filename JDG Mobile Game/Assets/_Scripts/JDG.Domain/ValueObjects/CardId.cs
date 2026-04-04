using System;

namespace JDG.Domain.ValueObjects
{
    /// <summary>
    /// Unique identifier for a card instance.
    /// Value object - immutable and comparable by value.
    /// ECS-ready: Can be converted to IComponentData in future.
    /// </summary>
    public readonly struct CardId : IEquatable<CardId>
    {
        private readonly Guid _value;

        private CardId(Guid value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new unique CardId.
        /// </summary>
        public static CardId New() => new CardId(Guid.NewGuid());

        /// <summary>
        /// Creates a CardId from an existing Guid (for deserialization).
        /// </summary>
        public static CardId FromGuid(Guid guid) => new CardId(guid);

        /// <summary>
        /// Empty/null CardId for comparisons.
        /// </summary>
        public static CardId Empty => new CardId(Guid.Empty);

        public Guid ToGuid() => _value;

        public bool Equals(CardId other)
        {
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is CardId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(CardId left, CardId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CardId left, CardId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}

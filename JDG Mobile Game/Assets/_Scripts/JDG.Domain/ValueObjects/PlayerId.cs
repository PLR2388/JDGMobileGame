using System;

namespace JDG.Domain.ValueObjects
{
    /// <summary>
    /// Type-safe player identifier.
    /// Value object - immutable and comparable by value.
    /// ECS-ready: Can be converted to IComponentData in future.
    /// </summary>
    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        private readonly int _value;

        private PlayerId(int value)
        {
            if (value != 1 && value != 2)
                throw new ArgumentException("PlayerId must be 1 or 2", nameof(value));
            _value = value;
        }

        public static PlayerId Player1 => new PlayerId(1);
        public static PlayerId Player2 => new PlayerId(2);

        public int ToInt() => _value;

        /// <summary>
        /// Converts CardOwner enum to PlayerId.
        /// </summary>
        public static PlayerId FromCardOwner(CardOwner owner)
        {
            return owner switch
            {
                CardOwner.Player1 => Player1,
                CardOwner.Player2 => Player2,
                _ => throw new ArgumentException($"Cannot convert CardOwner.{owner} to PlayerId", nameof(owner))
            };
        }

        /// <summary>
        /// Converts PlayerId to CardOwner enum.
        /// </summary>
        public CardOwner ToCardOwner()
        {
            return _value == 1 ? CardOwner.Player1 : CardOwner.Player2;
        }

        public bool Equals(PlayerId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(PlayerId left, PlayerId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerId left, PlayerId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"Player{_value}";
        }
    }
}

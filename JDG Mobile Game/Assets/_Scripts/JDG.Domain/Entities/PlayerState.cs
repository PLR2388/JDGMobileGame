namespace JDG.Domain.Entities
{
    /// <summary>
    /// Immutable domain entity representing a player's state in the game.
    /// Pure C# with no Unity dependencies.
    /// </summary>
    public class PlayerState
    {
        public const int MaxHealth = 30;
        public const int MinHealth = 0;

        public CardOwner PlayerId { get; }
        public int CurrentHealth { get; }
        public int ShieldCount { get; }
        public bool CanBlockAttack { get; }

        /// <summary>
        /// Creates a new PlayerState with the specified values.
        /// </summary>
        public PlayerState(
            CardOwner playerId,
            int currentHealth,
            int shieldCount,
            bool canBlockAttack)
        {
            PlayerId = playerId;
            CurrentHealth = currentHealth;
            ShieldCount = shieldCount;
            CanBlockAttack = canBlockAttack;
        }

        /// <summary>
        /// Creates a new player state with default starting values.
        /// </summary>
        public static PlayerState CreateDefault(CardOwner playerId)
        {
            return new PlayerState(
                playerId: playerId,
                currentHealth: MaxHealth,
                shieldCount: 0,
                canBlockAttack: false);
        }

        /// <summary>
        /// Returns a new PlayerState with updated health.
        /// Health is clamped between MinHealth and MaxHealth.
        /// </summary>
        public PlayerState WithHealth(int newHealth)
        {
            var clampedHealth = System.Math.Max(MinHealth, System.Math.Min(MaxHealth, newHealth));
            return new PlayerState(PlayerId, clampedHealth, ShieldCount, CanBlockAttack);
        }

        /// <summary>
        /// Returns a new PlayerState with health changed by the specified amount.
        /// </summary>
        public PlayerState ChangeHealth(int delta)
        {
            return WithHealth(CurrentHealth + delta);
        }

        /// <summary>
        /// Returns a new PlayerState with updated shield count.
        /// </summary>
        public PlayerState WithShields(int newShieldCount)
        {
            var clampedShields = System.Math.Max(0, newShieldCount);
            return new PlayerState(PlayerId, CurrentHealth, clampedShields, CanBlockAttack);
        }

        /// <summary>
        /// Returns a new PlayerState with shields decreased by one.
        /// </summary>
        public PlayerState DecrementShield()
        {
            return WithShields(ShieldCount - 1);
        }

        /// <summary>
        /// Returns a new PlayerState with block attack enabled.
        /// </summary>
        public PlayerState EnableBlockAttack()
        {
            return new PlayerState(PlayerId, CurrentHealth, ShieldCount, canBlockAttack: true);
        }

        /// <summary>
        /// Returns a new PlayerState with block attack disabled.
        /// </summary>
        public PlayerState DisableBlockAttack()
        {
            return new PlayerState(PlayerId, CurrentHealth, ShieldCount, canBlockAttack: false);
        }

        /// <summary>
        /// Returns true if the player is defeated (health <= 0).
        /// </summary>
        public bool IsDefeated => CurrentHealth <= MinHealth;

        /// <summary>
        /// Returns true if the player has any shields.
        /// </summary>
        public bool HasShields => ShieldCount > 0;
    }
}

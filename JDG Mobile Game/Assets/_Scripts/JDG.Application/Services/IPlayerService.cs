using JDG.Domain;
using JDG.Domain.Entities;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for managing player state (health, shields, block attack).
    /// Replaces the PlayerManager singleton.
    /// </summary>
    public interface IPlayerService
    {
        /// <summary>
        /// Gets the current state of the specified player.
        /// </summary>
        PlayerState GetPlayerState(CardOwner playerId);

        /// <summary>
        /// Changes the player's health by the specified amount.
        /// Publishes PlayerHealthChangedEvent via EventBus.
        /// Phase 151: Changed delta to float for half-star damage support.
        /// </summary>
        /// <param name="playerId">The player to affect.</param>
        /// <param name="delta">Amount to change health by (can be positive or negative).</param>
        void ChangeHealth(CardOwner playerId, float delta);

        /// <summary>
        /// Sets the player's health to a specific value.
        /// Publishes PlayerHealthChangedEvent via EventBus.
        /// Phase 151: Changed health to float for half-star damage support.
        /// </summary>
        void SetHealth(CardOwner playerId, float health);

        /// <summary>
        /// Sets the player's shield count.
        /// Publishes PlayerShieldChangedEvent via EventBus.
        /// </summary>
        void SetShieldCount(CardOwner playerId, int shieldCount);

        /// <summary>
        /// Decrements the player's shield count by one.
        /// Publishes PlayerShieldChangedEvent via EventBus.
        /// </summary>
        void DecrementShield(CardOwner playerId);

        /// <summary>
        /// Enables the player's ability to block attacks.
        /// </summary>
        void EnableBlockAttack(CardOwner playerId);

        /// <summary>
        /// Disables the player's ability to block attacks.
        /// </summary>
        void DisableBlockAttack(CardOwner playerId);

        /// <summary>
        /// Returns true if the player is defeated (health <= 0).
        /// </summary>
        bool IsPlayerDefeated(CardOwner playerId);

        /// <summary>
        /// Returns true if the player has any shields.
        /// </summary>
        bool HasShields(CardOwner playerId);

        /// <summary>
        /// Resets both players to their starting state.
        /// </summary>
        void ResetPlayers();
    }
}

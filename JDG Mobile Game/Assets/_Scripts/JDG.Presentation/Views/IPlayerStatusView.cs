using JDG.Application.DTOs;

namespace JDG.Presentation.Views
{
    /// <summary>
    /// View interface for displaying a player's status (health, shields, etc.).
    /// </summary>
    public interface IPlayerStatusView
    {
        /// <summary>
        /// Updates the player status display.
        /// </summary>
        void UpdateStatus(PlayerDTO player);

        /// <summary>
        /// Animates health change.
        /// </summary>
        void AnimateHealthChange(int oldHealth, int newHealth);

        /// <summary>
        /// Animates shield change.
        /// </summary>
        void AnimateShieldChange(int oldShields, int newShields);

        /// <summary>
        /// Shows damage taken effect.
        /// </summary>
        void ShowDamageEffect(int damage);

        /// <summary>
        /// Highlights when it's this player's turn.
        /// </summary>
        void SetActiveTurn(bool isActive);
    }
}

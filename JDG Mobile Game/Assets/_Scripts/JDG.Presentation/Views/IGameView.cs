using JDG.Application.DTOs;
using JDG.Domain;

namespace JDG.Presentation.Views
{
    /// <summary>
    /// View interface for the main game screen.
    /// Defines what the UI must be able to display and respond to.
    /// </summary>
    public interface IGameView
    {
        /// <summary>
        /// Updates the current game phase display.
        /// </summary>
        void ShowPhase(Phase phase);

        /// <summary>
        /// Updates the turn number display.
        /// </summary>
        void ShowTurnNumber(int turnNumber);

        /// <summary>
        /// Highlights the current active player.
        /// </summary>
        void ShowCurrentPlayer(CardOwner player);

        /// <summary>
        /// Updates Player 1's UI (health, shields, deck count, etc.).
        /// </summary>
        void UpdatePlayer1(PlayerDTO player);

        /// <summary>
        /// Updates Player 2's UI (health, shields, deck count, etc.).
        /// </summary>
        void UpdatePlayer2(PlayerDTO player);

        /// <summary>
        /// Shows the game over screen with winner.
        /// </summary>
        void ShowGameOver(CardOwner winner, string reason);

        /// <summary>
        /// Enables or disables the end turn button.
        /// </summary>
        void SetEndTurnButtonEnabled(bool enabled);

        /// <summary>
        /// Shows a message to the player.
        /// </summary>
        void ShowMessage(string message);
    }
}

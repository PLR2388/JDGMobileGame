using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Repositories
{
    /// <summary>
    /// Repository interface for managing game state.
    /// Handles current phase, turn, and active player.
    /// </summary>
    public interface IGameStateRepository
    {
        /// <summary>
        /// Gets the current game phase.
        /// </summary>
        Phase CurrentPhase { get; }

        /// <summary>
        /// Sets the current game phase.
        /// </summary>
        void SetPhase(Phase phase);

        /// <summary>
        /// Gets the current turn number.
        /// </summary>
        int TurnNumber { get; }

        /// <summary>
        /// Increments the turn number.
        /// </summary>
        void IncrementTurn();

        /// <summary>
        /// Gets the currently active player.
        /// </summary>
        PlayerId CurrentPlayer { get; }

        /// <summary>
        /// Sets the currently active player.
        /// </summary>
        void SetCurrentPlayer(PlayerId playerId);

        /// <summary>
        /// Switches to the other player.
        /// </summary>
        void SwitchPlayer();

        /// <summary>
        /// Resets game state to initial values.
        /// </summary>
        void ResetGameState();

        /// <summary>
        /// Checks if the game is over.
        /// </summary>
        bool IsGameOver { get; }

        /// <summary>
        /// Sets game over state.
        /// </summary>
        void SetGameOver(bool isGameOver);
    }
}

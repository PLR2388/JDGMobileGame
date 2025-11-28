using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.ValueObjects;

// Alias to avoid conflict with global Phase enum in GameStateManager.cs (in JDG.Legacy)
using Phase = JDG.Domain.Phase;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IGameStateRepository.
    /// Manages game state in memory (could be extended to persist state).
    /// </summary>
    public class GameStateRepository : IGameStateRepository
    {
        private Phase _currentPhase;
        private int _turnNumber;
        private PlayerId _currentPlayer;
        private bool _isGameOver;

        public Phase CurrentPhase => _currentPhase;

        public void SetPhase(Phase phase)
        {
            _currentPhase = phase;
        }

        public int TurnNumber => _turnNumber;

        public void IncrementTurn()
        {
            _turnNumber++;
        }

        public PlayerId CurrentPlayer => _currentPlayer;

        public void SetCurrentPlayer(PlayerId playerId)
        {
            _currentPlayer = playerId;
        }

        public void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == PlayerId.Player1
                ? PlayerId.Player2
                : PlayerId.Player1;
        }

        public void ResetGameState()
        {
            _currentPhase = Phase.Draw;
            _turnNumber = 1;
            _currentPlayer = PlayerId.Player1;
            _isGameOver = false;
        }

        public bool IsGameOver => _isGameOver;

        public void SetGameOver(bool isGameOver)
        {
            _isGameOver = isGameOver;
        }
    }
}

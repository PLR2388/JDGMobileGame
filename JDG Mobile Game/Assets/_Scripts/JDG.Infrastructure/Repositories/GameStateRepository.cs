using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.ValueObjects;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IGameStateRepository.
    /// Manages game state in memory (could be extended to persist state).
    /// </summary>
    public class GameStateRepository : IGameStateRepository
    {
        private JDG.Domain.Phase _currentPhase = JDG.Domain.Phase.Draw;
        private int _turnNumber = 1;
        private PlayerId _currentPlayer = PlayerId.Player1;
        private bool _isGameOver = false;

        public JDG.Domain.Phase CurrentPhase => _currentPhase;

        public void SetPhase(JDG.Domain.Phase phase)
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
            _currentPhase = JDG.Domain.Phase.Draw;
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

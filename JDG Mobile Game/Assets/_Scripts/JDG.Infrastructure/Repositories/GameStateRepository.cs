using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

// Alias to avoid conflict with global Phase enum in GameStateManager.cs
using DomainPhase = JDG.Domain.Enums.Phase;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IGameStateRepository.
    /// Manages game state in memory (could be extended to persist state).
    /// </summary>
    public class GameStateRepository : IGameStateRepository
    {
        private DomainPhase _currentPhase;
        private int _turnNumber;
        private PlayerId _currentPlayer;
        private bool _isGameOver;

        public DomainPhase CurrentPhase => _currentPhase;

        public void SetPhase(DomainPhase phase)
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
            _currentPhase = DomainPhase.Draw;
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

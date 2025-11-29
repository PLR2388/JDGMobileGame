using JDG.Application;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Service that manages game state and publishes events when state changes.
    /// Replaces the old GameStateManager singleton with proper DI.
    /// </summary>
    public class GameStateService
    {
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IEventBus _eventBus;

        public GameStateService(
            IGameStateRepository gameStateRepository,
            IEventBus eventBus)
        {
            _gameStateRepository = gameStateRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Gets the current game phase.
        /// </summary>
        public Phase CurrentPhase => _gameStateRepository.CurrentPhase;

        /// <summary>
        /// Gets the current turn number.
        /// </summary>
        public int TurnNumber => _gameStateRepository.TurnNumber;

        /// <summary>
        /// Gets the current active player.
        /// </summary>
        public PlayerId CurrentPlayer => _gameStateRepository.CurrentPlayer;

        /// <summary>
        /// Checks if the game is over.
        /// </summary>
        public bool IsGameOver => _gameStateRepository.IsGameOver;

        /// <summary>
        /// Sets the current phase and publishes PhaseChangedEvent.
        /// </summary>
        public void SetPhase(Phase newPhase)
        {
            var oldPhase = _gameStateRepository.CurrentPhase;
            _gameStateRepository.SetPhase(newPhase);

            _eventBus.Publish(new PhaseChangedEvent
            {
                OldPhase = oldPhase,
                NewPhase = newPhase,
                TurnNumber = _gameStateRepository.TurnNumber
            });
        }

        /// <summary>
        /// Advances to the next phase in sequence (Draw → Choose → Attack → End).
        /// </summary>
        public void NextPhase()
        {
            if (_gameStateRepository.CurrentPhase == Phase.GameOver)
                return;

            var oldPhase = _gameStateRepository.CurrentPhase;
            var newPhase = (Phase)(((int)oldPhase + 1) % 4);
            SetPhase(newPhase);
        }

        /// <summary>
        /// Switches to the other player and publishes PlayerTurnChangedEvent.
        /// </summary>
        public void SwitchPlayer()
        {
            _gameStateRepository.SwitchPlayer();

            _eventBus.Publish(new PlayerTurnChangedEvent
            {
                NewPlayer = _gameStateRepository.CurrentPlayer.ToCardOwner(),
                TurnNumber = _gameStateRepository.TurnNumber
            });
        }

        /// <summary>
        /// Increments the turn counter and publishes TurnStartEvent.
        /// </summary>
        public void StartNewTurn()
        {
            _gameStateRepository.IncrementTurn();

            _eventBus.Publish(new TurnStartEvent
            {
                CurrentPlayer = _gameStateRepository.CurrentPlayer.ToCardOwner(),
                TurnNumber = _gameStateRepository.TurnNumber
            });
        }

        /// <summary>
        /// Publishes TurnEndEvent.
        /// </summary>
        public void EndTurn()
        {
            _eventBus.Publish(new TurnEndEvent
            {
                CurrentPlayer = _gameStateRepository.CurrentPlayer.ToCardOwner(),
                TurnNumber = _gameStateRepository.TurnNumber
            });
        }

        /// <summary>
        /// Handles the complete end-of-turn flow: publish end event, switch player, reset phase, start new turn.
        /// </summary>
        public void HandleEndTurn()
        {
            EndTurn();
            SwitchPlayer();
            SetPhase(Phase.Draw);
            StartNewTurn();
        }

        /// <summary>
        /// Ends the game and publishes GameOverEvent.
        /// </summary>
        public void EndGame(CardOwner winner, string reason)
        {
            _gameStateRepository.SetGameOver(true);
            SetPhase(Phase.GameOver);

            _eventBus.Publish(new GameOverEvent
            {
                Winner = winner,
                Reason = reason
            });
        }

        /// <summary>
        /// Resets the game state to initial values.
        /// </summary>
        public void ResetGame()
        {
            _gameStateRepository.ResetGameState();
        }
    }
}

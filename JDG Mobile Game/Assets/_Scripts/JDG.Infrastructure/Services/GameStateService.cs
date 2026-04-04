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
        public JDG.Domain.Phase CurrentPhase => _gameStateRepository.CurrentPhase;

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
        /// Checks if the attack phase should be skipped for the current player.
        /// Rule: Player 1 cannot attack on Turn 1 (first turn advantage rule).
        /// </summary>
        public bool ShouldSkipAttackPhase =>
            TurnNumber == 1 && CurrentPlayer == PlayerId.Player1;

        /// <summary>
        /// Sets the current phase and publishes PhaseChangedEvent.
        /// </summary>
        public void SetPhase(JDG.Domain.Phase newPhase)
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
        /// Automatically skips Attack phase for Player 1 on Turn 1.
        /// </summary>
        public void NextPhase()
        {
            if (_gameStateRepository.CurrentPhase == JDG.Domain.Phase.GameOver)
                return;

            var oldPhase = _gameStateRepository.CurrentPhase;
            var newPhase = (JDG.Domain.Phase)(((int)oldPhase + 1) % 4);

            // Skip Attack phase for Player 1 on Turn 1 (first turn advantage rule)
            if (newPhase == JDG.Domain.Phase.Attack && ShouldSkipAttackPhase)
            {
                newPhase = JDG.Domain.Phase.End;
            }

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
            SetPhase(JDG.Domain.Phase.Draw);
            StartNewTurn();
        }

        /// <summary>
        /// Ends the game and publishes GameOverEvent.
        /// </summary>
        public void EndGame(JDG.Domain.CardOwner winner, string reason)
        {
            _gameStateRepository.SetGameOver(true);
            SetPhase(JDG.Domain.Phase.GameOver);

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

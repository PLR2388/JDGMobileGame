using System;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Application.UseCases;
using JDG.Application.Mappers;
using JDG.Domain.Events;
using JDG.Presentation.Views;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for the main game screen.
    /// Handles game flow, subscribes to events, and updates the view.
    /// </summary>
    public class GamePresenter : IDisposable
    {
        private readonly IGameView _view;
        private readonly IEventBus _eventBus;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly EndTurnUseCase _endTurnUseCase;
        private readonly StartGameUseCase _startGameUseCase;

        private IDisposable _phaseChangedSubscription;
        private IDisposable _turnChangedSubscription;
        private IDisposable _gameOverSubscription;
        private IDisposable _gameStartedSubscription;

        public GamePresenter(
            IGameView view,
            IEventBus eventBus,
            IGameStateRepository gameStateRepository,
            IPlayerRepository playerRepository,
            EndTurnUseCase endTurnUseCase,
            StartGameUseCase startGameUseCase)
        {
            _view = view;
            _eventBus = eventBus;
            _gameStateRepository = gameStateRepository;
            _playerRepository = playerRepository;
            _endTurnUseCase = endTurnUseCase;
            _startGameUseCase = startGameUseCase;

            SubscribeToEvents();
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void StartGame(string player1Deck = null, string player2Deck = null)
        {
            var result = _startGameUseCase.Execute(player1Deck, player2Deck);

            if (!result.IsSuccess)
            {
                _view.ShowMessage($"Failed to start game: {result.Message}");
                return;
            }

            // Update view with initial state
            UpdateGameState();
            UpdatePlayers();
        }

        /// <summary>
        /// Ends the current turn.
        /// </summary>
        public void EndTurn()
        {
            var result = _endTurnUseCase.Execute();

            if (!result.IsSuccess)
            {
                _view.ShowMessage($"Failed to end turn: {result.Message}");
            }
        }

        /// <summary>
        /// Updates the game state display.
        /// </summary>
        private void UpdateGameState()
        {
            _view.ShowPhase((Phase)(int)_gameStateRepository.CurrentPhase);
            _view.ShowTurnNumber(_gameStateRepository.TurnNumber);
            _view.ShowCurrentPlayer((CardOwner)(int)_gameStateRepository.CurrentPlayer.ToCardOwner());
            _view.SetEndTurnButtonEnabled(!_gameStateRepository.IsGameOver);
        }

        /// <summary>
        /// Updates both players' UI.
        /// </summary>
        private void UpdatePlayers()
        {
            var player1 = _playerRepository.GetPlayer(Domain.ValueObjects.PlayerId.Player1);
            var player2 = _playerRepository.GetPlayer(Domain.ValueObjects.PlayerId.Player2);

            if (player1 != null)
            {
                _view.UpdatePlayer1(player1.ToDTO());
            }

            if (player2 != null)
            {
                _view.UpdatePlayer2(player2.ToDTO());
            }
        }

        /// <summary>
        /// Subscribes to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            _phaseChangedSubscription = _eventBus.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            _turnChangedSubscription = _eventBus.Subscribe<PlayerTurnChangedEvent>(OnTurnChanged);
            _gameOverSubscription = _eventBus.Subscribe<GameOverEvent>(OnGameOver);
            _gameStartedSubscription = _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            _view.ShowPhase((Phase)(int)evt.NewPhase);
        }

        private void OnTurnChanged(PlayerTurnChangedEvent evt)
        {
            _view.ShowCurrentPlayer((CardOwner)(int)evt.NewPlayer);
            _view.ShowTurnNumber(evt.TurnNumber);
            UpdatePlayers();
        }

        private void OnGameOver(GameOverEvent evt)
        {
            _view.ShowGameOver((CardOwner)(int)evt.Winner, evt.Reason);
            _view.SetEndTurnButtonEnabled(false);
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            _view.ShowMessage("Game Started!");
            UpdateGameState();
        }

        public void Dispose()
        {
            _phaseChangedSubscription?.Dispose();
            _turnChangedSubscription?.Dispose();
            _gameOverSubscription?.Dispose();
            _gameStartedSubscription?.Dispose();
        }
    }
}

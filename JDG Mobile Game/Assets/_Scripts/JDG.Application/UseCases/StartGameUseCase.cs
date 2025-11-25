using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for starting a new game.
    /// Initializes both players, game state, and publishes GameStartedEvent.
    /// </summary>
    public class StartGameUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IEventBus _eventBus;

        public StartGameUseCase(
            IPlayerRepository playerRepository,
            IDeckRepository deckRepository,
            IGameStateRepository gameStateRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _deckRepository = deckRepository;
            _gameStateRepository = gameStateRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Executes the start game use case.
        /// </summary>
        public StartGameResult Execute(string player1DeckName, string player2DeckName)
        {
            // Reset game state
            _gameStateRepository.ResetGameState();
            _gameStateRepository.SetPhase(Phase.Draw);
            _gameStateRepository.SetCurrentPlayer(PlayerId.Player1);

            // Load decks
            var player1Deck = string.IsNullOrEmpty(player1DeckName)
                ? _deckRepository.GetDefaultDeck(PlayerId.Player1)
                : _deckRepository.GetDeck(player1DeckName);

            var player2Deck = string.IsNullOrEmpty(player2DeckName)
                ? _deckRepository.GetDefaultDeck(PlayerId.Player2)
                : _deckRepository.GetDeck(player2DeckName);

            if (player1Deck == null || player1Deck.Length == 0)
                return StartGameResult.Failure("Player 1 deck not found or empty");

            if (player2Deck == null || player2Deck.Length == 0)
                return StartGameResult.Failure("Player 2 deck not found or empty");

            // Create players
            _playerRepository.ResetPlayer(PlayerId.Player1);
            _playerRepository.ResetPlayer(PlayerId.Player2);

            var player1 = _playerRepository.CreatePlayer(PlayerId.Player1, player1Deck);
            var player2 = _playerRepository.CreatePlayer(PlayerId.Player2, player2Deck);

            if (player1 == null || player2 == null)
                return StartGameResult.Failure("Failed to create players");

            // Publish game started event
            _eventBus.Publish(new GameStartedEvent
            {
                Player1Id = PlayerId.Player1.ToCardOwner(),
                Player2Id = PlayerId.Player2.ToCardOwner(),
                StartingPlayer = PlayerId.Player1.ToCardOwner(),
                TurnNumber = 1
            });

            // Publish initial phase
            _eventBus.Publish(new PhaseChangedEvent
            {
                OldPhase = Phase.GameOver,
                NewPhase = Phase.Draw,
                TurnNumber = 1
            });

            return StartGameResult.Success(player1, player2);
        }
    }

    /// <summary>
    /// Result of a start game operation.
    /// </summary>
    public class StartGameResult
    {
        public bool IsSuccess { get; private set; }
        public Domain.Entities.Player Player1 { get; private set; }
        public Domain.Entities.Player Player2 { get; private set; }
        public string Message { get; private set; }

        public static StartGameResult Success(Domain.Entities.Player player1, Domain.Entities.Player player2) => new StartGameResult
        {
            IsSuccess = true,
            Player1 = player1,
            Player2 = player2,
            Message = "Game started successfully"
        };

        public static StartGameResult Failure(string message) => new StartGameResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

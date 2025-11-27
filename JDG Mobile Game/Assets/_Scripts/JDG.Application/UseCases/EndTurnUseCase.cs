using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for ending the current turn and transitioning to the next player.
    /// Publishes PhaseChangedEvent and PlayerTurnChangedEvent.
    /// </summary>
    public class EndTurnUseCase
    {
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IEventBus _eventBus;

        public EndTurnUseCase(IGameStateRepository gameStateRepository, IEventBus eventBus)
        {
            _gameStateRepository = gameStateRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Executes the end turn use case.
        /// </summary>
        public EndTurnResult Execute()
        {
            if (_gameStateRepository.IsGameOver)
                return EndTurnResult.Failure("Game is over");

            var oldPhase = _gameStateRepository.CurrentPhase;
            var currentPlayer = _gameStateRepository.CurrentPlayer;

            // Transition to End phase
            _gameStateRepository.SetPhase(Phase.End);

            _eventBus.Publish(new PhaseChangedEvent
            {
                OldPhase = oldPhase,
                NewPhase = Phase.End,
                TurnNumber = _gameStateRepository.TurnNumber
            });

            // Switch to next player and increment turn
            _gameStateRepository.SwitchPlayer();
            _gameStateRepository.IncrementTurn();
            var newPlayer = _gameStateRepository.CurrentPlayer;

            _eventBus.Publish(new PlayerTurnChangedEvent
            {
                NewPlayer = newPlayer.ToCardOwner(),
                TurnNumber = _gameStateRepository.TurnNumber
            });

            // Transition to Draw phase for new player
            _gameStateRepository.SetPhase(Phase.Draw);

            _eventBus.Publish(new PhaseChangedEvent
            {
                OldPhase = Phase.End,
                NewPhase = Phase.Draw,
                TurnNumber = _gameStateRepository.TurnNumber
            });

            return EndTurnResult.Success(newPlayer, _gameStateRepository.TurnNumber);
        }
    }

    /// <summary>
    /// Result of an end turn operation.
    /// </summary>
    public class EndTurnResult
    {
        public bool IsSuccess { get; private set; }
        public PlayerId NextPlayer { get; private set; }
        public int TurnNumber { get; private set; }
        public string Message { get; private set; }

        public static EndTurnResult Success(PlayerId nextPlayer, int turnNumber) => new EndTurnResult
        {
            IsSuccess = true,
            NextPlayer = nextPlayer,
            TurnNumber = turnNumber,
            Message = "Turn ended successfully"
        };

        public static EndTurnResult Failure(string message) => new EndTurnResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

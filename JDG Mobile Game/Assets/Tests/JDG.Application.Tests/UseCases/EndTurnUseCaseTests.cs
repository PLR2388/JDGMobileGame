using NUnit.Framework;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Domain.Events;
using System.Collections.Generic;

namespace JDG.Application.Tests.UseCases
{
    [TestFixture]
    public class EndTurnUseCaseTests
    {
        private EndTurnUseCase _useCase;
        private TestGameStateRepository _gameStateRepository;
        private TestEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _gameStateRepository = new TestGameStateRepository();
            _eventBus = new TestEventBus();
            _useCase = new EndTurnUseCase(_gameStateRepository, _eventBus);
        }

        [Test]
        public void Execute_WithValidState_EndsTurnSuccessfully()
        {
            // Arrange
            _gameStateRepository.SetCurrentPlayer(PlayerId.Player1);
            _gameStateRepository.SetPhase(Phase.Attack);

            // Act
            var result = _useCase.Execute();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(PlayerId.Player2, result.NextPlayer);
            Assert.AreEqual(2, result.TurnNumber);
        }

        [Test]
        public void Execute_SwitchesPlayer()
        {
            // Arrange
            _gameStateRepository.SetCurrentPlayer(PlayerId.Player1);

            // Act
            _useCase.Execute();

            // Assert
            Assert.AreEqual(PlayerId.Player2, _gameStateRepository.CurrentPlayer);
        }

        [Test]
        public void Execute_IncrementsTurnNumber()
        {
            // Arrange
            Assert.AreEqual(1, _gameStateRepository.TurnNumber);

            // Act
            _useCase.Execute();

            // Assert
            Assert.AreEqual(2, _gameStateRepository.TurnNumber);
        }

        [Test]
        public void Execute_TransitionsToDrawPhase()
        {
            // Arrange
            _gameStateRepository.SetPhase(Phase.Attack);

            // Act
            _useCase.Execute();

            // Assert
            Assert.AreEqual(Phase.Draw, _gameStateRepository.CurrentPhase);
        }

        [Test]
        public void Execute_WhenGameOver_ReturnsFailure()
        {
            // Arrange
            _gameStateRepository.SetGameOver(true);

            // Act
            var result = _useCase.Execute();

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Game is over", result.Message);
        }

        [Test]
        public void Execute_PublishesPhaseChangedEvents()
        {
            // Arrange
            _gameStateRepository.SetPhase(Phase.Attack);

            // Act
            _useCase.Execute();

            // Assert - Should publish 2 PhaseChangedEvents (to End, then to Draw)
            var phaseEvents = _eventBus.PublishedEvents.FindAll(e => e is PhaseChangedEvent);
            Assert.AreEqual(2, phaseEvents.Count);
        }

        [Test]
        public void Execute_PublishesPlayerTurnChangedEvent()
        {
            // Arrange
            _gameStateRepository.SetCurrentPlayer(PlayerId.Player1);

            // Act
            _useCase.Execute();

            // Assert
            var turnEvents = _eventBus.PublishedEvents.FindAll(e => e is PlayerTurnChangedEvent);
            Assert.AreEqual(1, turnEvents.Count);
        }

        [Test]
        public void Execute_FromPlayer2ToPlayer1()
        {
            // Arrange
            _gameStateRepository.SetCurrentPlayer(PlayerId.Player2);

            // Act
            var result = _useCase.Execute();

            // Assert
            Assert.AreEqual(PlayerId.Player1, result.NextPlayer);
        }
    }

    // Test double for IGameStateRepository
    public class TestGameStateRepository : IGameStateRepository
    {
        private Phase _currentPhase = Phase.Draw;
        private int _turnNumber = 1;
        private PlayerId _currentPlayer = PlayerId.Player1;
        private bool _isGameOver = false;

        public Phase CurrentPhase => _currentPhase;
        public int TurnNumber => _turnNumber;
        public PlayerId CurrentPlayer => _currentPlayer;
        public bool IsGameOver => _isGameOver;

        public void SetPhase(Phase phase) => _currentPhase = phase;
        public void IncrementTurn() => _turnNumber++;
        public void SetCurrentPlayer(PlayerId playerId) => _currentPlayer = playerId;
        public void SetGameOver(bool isGameOver) => _isGameOver = isGameOver;

        public void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        }

        public void ResetGameState()
        {
            _currentPhase = Phase.Draw;
            _turnNumber = 1;
            _currentPlayer = PlayerId.Player1;
            _isGameOver = false;
        }
    }
}

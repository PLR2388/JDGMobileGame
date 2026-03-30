using NUnit.Framework;
using JDG.Infrastructure.Services;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Domain.Events;
using System.Collections.Generic;
using System.Linq;


namespace JDG.Infrastructure.Tests.Services
{
    [TestFixture]
    public class GameStateServiceTests
    {
        private GameStateService _service;
        private TestGameStateRepository _repository;
        private TestEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _repository = new TestGameStateRepository();
            _eventBus = new TestEventBus();
            _service = new GameStateService(_repository, _eventBus);
        }

        [Test]
        public void CurrentPhase_ReturnsRepositoryPhase()
        {
            // Arrange
            _repository.SetPhase(Phase.Attack);

            // Assert
            Assert.AreEqual(Phase.Attack, _service.CurrentPhase);
        }

        [Test]
        public void TurnNumber_ReturnsRepositoryTurnNumber()
        {
            // Arrange
            _repository.IncrementTurn();
            _repository.IncrementTurn();

            // Assert
            Assert.AreEqual(3, _service.TurnNumber);
        }

        [Test]
        public void CurrentPlayer_ReturnsRepositoryCurrentPlayer()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player2);

            // Assert
            Assert.AreEqual(PlayerId.Player2, _service.CurrentPlayer);
        }

        [Test]
        public void IsGameOver_ReturnsRepositoryGameOverState()
        {
            // Arrange
            _repository.SetGameOver(true);

            // Assert
            Assert.IsTrue(_service.IsGameOver);
        }

        [Test]
        public void SetPhase_UpdatesRepositoryPhase()
        {
            // Act
            _service.SetPhase(Phase.Attack);

            // Assert
            Assert.AreEqual(Phase.Attack, _repository.CurrentPhase);
        }

        [Test]
        public void SetPhase_PublishesPhaseChangedEvent()
        {
            // Arrange
            _repository.SetPhase(Phase.Draw);

            // Act
            _service.SetPhase(Phase.Choose);

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            var phaseEvent = (PhaseChangedEvent)_eventBus.PublishedEvents[0];
            Assert.AreEqual(Phase.Draw, phaseEvent.OldPhase);
            Assert.AreEqual(Phase.Choose, phaseEvent.NewPhase);
        }

        [Test]
        public void NextPhase_AdvancesToNextPhaseInSequence()
        {
            // Arrange
            _repository.SetPhase(Phase.Draw);

            // Act
            _service.NextPhase();

            // Assert
            Assert.AreEqual(Phase.Choose, _repository.CurrentPhase);
        }

        [Test]
        public void NextPhase_WrapsAroundAfterEnd()
        {
            // Arrange
            _repository.SetPhase(Phase.End);

            // Act
            _service.NextPhase();

            // Assert
            Assert.AreEqual(Phase.Draw, _repository.CurrentPhase);
        }

        [Test]
        public void NextPhase_DoesNothingWhenGameOver()
        {
            // Arrange
            _repository.SetPhase(Phase.GameOver);

            // Act
            _service.NextPhase();

            // Assert
            Assert.AreEqual(Phase.GameOver, _repository.CurrentPhase);
            Assert.AreEqual(0, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void SwitchPlayer_SwitchesActivePlayer()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player1);

            // Act
            _service.SwitchPlayer();

            // Assert
            Assert.AreEqual(PlayerId.Player2, _repository.CurrentPlayer);
        }

        [Test]
        public void SwitchPlayer_PublishesPlayerTurnChangedEvent()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player1);

            // Act
            _service.SwitchPlayer();

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            Assert.IsInstanceOf<PlayerTurnChangedEvent>(_eventBus.PublishedEvents[0]);
        }

        [Test]
        public void StartNewTurn_IncrementsTurnCounter()
        {
            // Arrange
            var initialTurn = _repository.TurnNumber;

            // Act
            _service.StartNewTurn();

            // Assert
            Assert.AreEqual(initialTurn + 1, _repository.TurnNumber);
        }

        [Test]
        public void StartNewTurn_PublishesTurnStartEvent()
        {
            // Act
            _service.StartNewTurn();

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            Assert.IsInstanceOf<TurnStartEvent>(_eventBus.PublishedEvents[0]);
        }

        [Test]
        public void EndTurn_PublishesTurnEndEvent()
        {
            // Act
            _service.EndTurn();

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            Assert.IsInstanceOf<TurnEndEvent>(_eventBus.PublishedEvents[0]);
        }

        [Test]
        public void HandleEndTurn_ExecutesCompleteTurnEndSequence()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player1);
            _repository.SetPhase(Phase.Attack);
            var initialTurn = _repository.TurnNumber;

            // Act
            _service.HandleEndTurn();

            // Assert
            Assert.AreEqual(PlayerId.Player2, _repository.CurrentPlayer);
            Assert.AreEqual(Phase.Draw, _repository.CurrentPhase);
            Assert.AreEqual(initialTurn + 1, _repository.TurnNumber);
        }

        [Test]
        public void HandleEndTurn_PublishesAllRequiredEvents()
        {
            // Act
            _service.HandleEndTurn();

            // Assert - should publish: TurnEndEvent, PlayerTurnChangedEvent, PhaseChangedEvent, TurnStartEvent
            Assert.AreEqual(4, _eventBus.PublishedEvents.Count);
            Assert.IsInstanceOf<TurnEndEvent>(_eventBus.PublishedEvents[0]);
            Assert.IsInstanceOf<PlayerTurnChangedEvent>(_eventBus.PublishedEvents[1]);
            Assert.IsInstanceOf<PhaseChangedEvent>(_eventBus.PublishedEvents[2]);
            Assert.IsInstanceOf<TurnStartEvent>(_eventBus.PublishedEvents[3]);
        }

        [Test]
        public void EndGame_SetsGameOverState()
        {
            // Act
            _service.EndGame(CardOwner.Player1, "Test reason");

            // Assert
            Assert.IsTrue(_repository.IsGameOver);
        }

        [Test]
        public void EndGame_SetsPhaseToGameOver()
        {
            // Act
            _service.EndGame(CardOwner.Player1, "Test reason");

            // Assert
            Assert.AreEqual(Phase.GameOver, _repository.CurrentPhase);
        }

        [Test]
        public void EndGame_PublishesGameOverEvent()
        {
            // Act
            _service.EndGame(CardOwner.Player1, "Victory");

            // Assert
            var gameOverEvents = _eventBus.PublishedEvents.Where(e => e is GameOverEvent).ToList();
            Assert.AreEqual(1, gameOverEvents.Count);
            var gameOverEvent = (GameOverEvent)gameOverEvents[0];
            Assert.AreEqual(CardOwner.Player1, gameOverEvent.Winner);
            Assert.AreEqual("Victory", gameOverEvent.Reason);
        }

        [Test]
        public void ResetGame_ResetsRepositoryState()
        {
            // Arrange
            _repository.SetPhase(Phase.Attack);
            _repository.SetCurrentPlayer(PlayerId.Player2);
            _repository.SetGameOver(true);
            _repository.IncrementTurn();

            // Act
            _service.ResetGame();

            // Assert
            Assert.AreEqual(Phase.Draw, _repository.CurrentPhase);
            Assert.AreEqual(PlayerId.Player1, _repository.CurrentPlayer);
            Assert.IsFalse(_repository.IsGameOver);
            Assert.AreEqual(1, _repository.TurnNumber);
        }

        #region Test Doubles

        private class TestGameStateRepository : IGameStateRepository
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

        private class TestEventBus : IEventBus
        {
            public List<object> PublishedEvents { get; } = new List<object>();

            public void Publish<T>(T eventData) where T : struct
            {
                PublishedEvents.Add(eventData);
            }

            public System.IDisposable Subscribe<T>(System.Action<T> handler) where T : struct
            {
                throw new System.NotImplementedException();
            }

            public void ClearSubscriptions<T>() where T : struct
            {
                throw new System.NotImplementedException();
            }

            public void ClearAllSubscriptions()
            {
                PublishedEvents.Clear();
            }
        }

        #endregion
    }
}

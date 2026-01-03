using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

// Alias to avoid conflict with global namespace CardOwner
using DomainCardOwner = JDG.Domain.CardOwner;

namespace JDG.Infrastructure.Tests.Scenarios
{
    /// <summary>
    /// Scenario tests for game loop and turn cycle mechanics.
    /// Tests complete turn flows, player switching, and game state transitions.
    /// Phase 131: Game loop scenario tests.
    /// </summary>
    [TestFixture]
    public class GameLoopScenarioTests
    {
        private GameStateService _gameStateService;
        private TestGameStateRepository _repository;
        private TestEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _repository = new TestGameStateRepository();
            _eventBus = new TestEventBus();
            _gameStateService = new GameStateService(_repository, _eventBus);
        }

        #region Turn Cycle Scenarios

        [Test]
        public void TurnCycle_StartsWithDrawPhase()
        {
            // Assert - Game starts in Draw phase
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        [Test]
        public void TurnCycle_ProgressesThroughAllPhases()
        {
            // Arrange - Use Turn 2 to avoid "Player 1 can't attack on Turn 1" rule
            _repository.SetPhase(Phase.Draw);
            _repository.IncrementTurn(); // Turn 2

            // Act & Assert - Progress through all phases
            _gameStateService.NextPhase();
            Assert.AreEqual(Phase.Choose, _gameStateService.CurrentPhase, "Draw -> Choose");

            _gameStateService.NextPhase();
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase, "Choose -> Attack");

            _gameStateService.NextPhase();
            Assert.AreEqual(Phase.End, _gameStateService.CurrentPhase, "Attack -> End");

            _gameStateService.NextPhase();
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase, "End -> Draw (wrap around)");
        }

        [Test]
        public void TurnCycle_SwitchesPlayers()
        {
            // Arrange
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);

            // Act
            _gameStateService.SwitchPlayer();

            // Assert
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer);
        }

        [Test]
        public void TurnCycle_SwitchBackToPlayer1()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player2);

            // Act
            _gameStateService.SwitchPlayer();

            // Assert
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
        }

        #endregion

        #region Complete Turn Scenarios

        [Test]
        public void CompleteTurn_Player1ToPlayer2()
        {
            // Arrange
            _repository.SetPhase(Phase.End);
            _repository.SetCurrentPlayer(PlayerId.Player1);
            var initialTurn = _gameStateService.TurnNumber;

            // Act - End turn triggers complete flow
            _gameStateService.HandleEndTurn();

            // Assert
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer, "Should switch to Player2");
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase, "Should start at Draw");
            Assert.AreEqual(initialTurn + 1, _gameStateService.TurnNumber, "Turn should increment");
        }

        [Test]
        public void CompleteTurn_PublishesCorrectEvents()
        {
            // Act
            _gameStateService.HandleEndTurn();

            // Assert - Verify event order
            Assert.AreEqual(4, _eventBus.PublishedEvents.Count, "Should publish 4 events");
            Assert.IsInstanceOf<TurnEndEvent>(_eventBus.PublishedEvents[0], "1st: TurnEnd");
            Assert.IsInstanceOf<PlayerTurnChangedEvent>(_eventBus.PublishedEvents[1], "2nd: PlayerTurnChanged");
            Assert.IsInstanceOf<PhaseChangedEvent>(_eventBus.PublishedEvents[2], "3rd: PhaseChanged");
            Assert.IsInstanceOf<TurnStartEvent>(_eventBus.PublishedEvents[3], "4th: TurnStart");
        }

        [Test]
        public void CompleteTurn_TurnEndEventHasCorrectData()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player1);
            _repository.IncrementTurn(); // Turn 2

            // Act
            _gameStateService.EndTurn();

            // Assert
            var turnEndEvent = (TurnEndEvent)_eventBus.PublishedEvents[0];
            Assert.AreEqual(DomainCardOwner.Player1, turnEndEvent.CurrentPlayer);
            Assert.AreEqual(2, turnEndEvent.TurnNumber);
        }

        [Test]
        public void CompleteTurn_TurnStartEventHasCorrectData()
        {
            // Arrange
            _repository.SetCurrentPlayer(PlayerId.Player1);

            // Act
            _gameStateService.StartNewTurn();

            // Assert
            var turnStartEvent = (TurnStartEvent)_eventBus.PublishedEvents[0];
            Assert.AreEqual(DomainCardOwner.Player1, turnStartEvent.CurrentPlayer);
            Assert.AreEqual(2, turnStartEvent.TurnNumber); // Incremented
        }

        #endregion

        #region Win Condition Scenarios

        [Test]
        public void WinCondition_Player1Wins_GameOver()
        {
            // Act
            _gameStateService.EndGame(DomainCardOwner.Player1, "Player 2 HP reached 0");

            // Assert
            Assert.IsTrue(_gameStateService.IsGameOver);
            Assert.AreEqual(Phase.GameOver, _gameStateService.CurrentPhase);
        }

        [Test]
        public void WinCondition_Player2Wins_GameOver()
        {
            // Act
            _gameStateService.EndGame(DomainCardOwner.Player2, "Player 1 HP reached 0");

            // Assert
            Assert.IsTrue(_gameStateService.IsGameOver);
            Assert.AreEqual(Phase.GameOver, _gameStateService.CurrentPhase);
        }

        [Test]
        public void WinCondition_GameOverPublishesEvent()
        {
            // Act
            _gameStateService.EndGame(DomainCardOwner.Player1, "Victory");

            // Assert
            var gameOverEvents = _eventBus.PublishedEvents.Where(e => e is GameOverEvent).ToList();
            Assert.AreEqual(1, gameOverEvents.Count);
            var evt = (GameOverEvent)gameOverEvents[0];
            Assert.AreEqual(DomainCardOwner.Player1, evt.Winner);
            Assert.AreEqual("Victory", evt.Reason);
        }

        [Test]
        public void WinCondition_GameOver_StopsPhaseProgression()
        {
            // Arrange
            _gameStateService.EndGame(DomainCardOwner.Player1, "Test");
            _eventBus.PublishedEvents.Clear();

            // Act - Try to progress phases
            _gameStateService.NextPhase();

            // Assert - Phase should stay at GameOver
            Assert.AreEqual(Phase.GameOver, _gameStateService.CurrentPhase);
            Assert.AreEqual(0, _eventBus.PublishedEvents.Count, "No events when game over");
        }

        #endregion

        #region Multi-Turn Simulation

        [Test]
        public void MultiTurn_ThreeTurns_MaintainsState()
        {
            // Turn 1 - Player 1
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
            Assert.AreEqual(1, _gameStateService.TurnNumber);

            // End Turn 1
            _gameStateService.HandleEndTurn();
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer);
            Assert.AreEqual(2, _gameStateService.TurnNumber);

            // End Turn 2
            _gameStateService.HandleEndTurn();
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
            Assert.AreEqual(3, _gameStateService.TurnNumber);

            // End Turn 3
            _gameStateService.HandleEndTurn();
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer);
            Assert.AreEqual(4, _gameStateService.TurnNumber);
        }

        [Test]
        public void MultiTurn_FullTurnCycle_PublishesAllEvents()
        {
            // Act - Complete one full turn cycle
            _gameStateService.HandleEndTurn();
            _gameStateService.HandleEndTurn();

            // Assert - 8 events total (4 per turn)
            Assert.AreEqual(8, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void MultiTurn_PhaseHistory_RecordedCorrectly()
        {
            // Collect phase change history
            var phaseHistory = new List<Phase>();

            // Simulate full turn with phase transitions
            // Use Turn 2 to avoid "Player 1 can't attack on Turn 1" rule
            _repository.SetPhase(Phase.Draw);
            _repository.IncrementTurn(); // Turn 2
            phaseHistory.Add(_gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // Draw -> Choose
            phaseHistory.Add(_gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // Choose -> Attack
            phaseHistory.Add(_gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // Attack -> End
            phaseHistory.Add(_gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // End -> Draw (new turn)
            phaseHistory.Add(_gameStateService.CurrentPhase);

            // Assert
            Assert.AreEqual(new[] { Phase.Draw, Phase.Choose, Phase.Attack, Phase.End, Phase.Draw },
                phaseHistory.ToArray());
        }

        #endregion

        #region Reset Scenarios

        [Test]
        public void Reset_RestoresInitialState()
        {
            // Arrange - Modify state
            _repository.SetPhase(Phase.Attack);
            _repository.SetCurrentPlayer(PlayerId.Player2);
            _repository.IncrementTurn();
            _repository.IncrementTurn();
            _repository.SetGameOver(true);

            // Act
            _gameStateService.ResetGame();

            // Assert
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
            Assert.AreEqual(1, _gameStateService.TurnNumber);
            Assert.IsFalse(_gameStateService.IsGameOver);
        }

        [Test]
        public void Reset_AllowsNewGameAfterGameOver()
        {
            // Arrange - Game was over
            _gameStateService.EndGame(DomainCardOwner.Player1, "Previous game");

            // Act
            _gameStateService.ResetGame();
            _eventBus.PublishedEvents.Clear();

            // Assert - Can progress phases again
            _gameStateService.NextPhase();
            Assert.AreEqual(Phase.Choose, _gameStateService.CurrentPhase);
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
        }

        #endregion

        #region Phase Transition Events

        [Test]
        public void PhaseTransition_EventContainsCorrectData()
        {
            // Arrange
            _repository.SetPhase(Phase.Choose);
            _repository.IncrementTurn(); // Turn 2

            // Act
            _gameStateService.SetPhase(Phase.Attack);

            // Assert
            var evt = (PhaseChangedEvent)_eventBus.PublishedEvents[0];
            Assert.AreEqual(Phase.Choose, evt.OldPhase);
            Assert.AreEqual(Phase.Attack, evt.NewPhase);
            Assert.AreEqual(2, evt.TurnNumber);
        }

        [Test]
        public void PhaseTransition_DirectSetToGameOver()
        {
            // Arrange
            _repository.SetPhase(Phase.Attack);

            // Act
            _gameStateService.SetPhase(Phase.GameOver);

            // Assert
            Assert.AreEqual(Phase.GameOver, _gameStateService.CurrentPhase);
        }

        [Test]
        public void PhaseTransition_CanSkipPhases()
        {
            // Arrange
            _repository.SetPhase(Phase.Draw);

            // Act - Skip directly to Attack
            _gameStateService.SetPhase(Phase.Attack);

            // Assert
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase);
            var evt = (PhaseChangedEvent)_eventBus.PublishedEvents[0];
            Assert.AreEqual(Phase.Draw, evt.OldPhase);
            Assert.AreEqual(Phase.Attack, evt.NewPhase);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void EdgeCase_FirstTurnPlayer1_CanEndTurn()
        {
            // First turn special case - Player 1 should be able to end turn
            Assert.AreEqual(1, _gameStateService.TurnNumber);
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);

            // Act
            _gameStateService.HandleEndTurn();

            // Assert
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer);
        }

        [Test]
        public void EdgeCase_ManyTurns_NoOverflow()
        {
            // Simulate many turns
            for (int i = 0; i < 100; i++)
            {
                _gameStateService.HandleEndTurn();
            }

            // Assert - No overflow, alternates correctly
            Assert.AreEqual(101, _gameStateService.TurnNumber);
            // 100 switches from Player1: even number of switches returns to Player1
            // (P1 -> P2 -> P1 -> P2 -> ... -> P1 after 100 switches)
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
        }

        [Test]
        public void EdgeCase_PhaseWrapAround_MultipleTimess()
        {
            // Wrap around multiple times
            for (int i = 0; i < 12; i++) // 3 full cycles
            {
                _gameStateService.NextPhase();
            }

            // Should be back at Draw (12 % 4 = 0 = Draw)
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        #endregion

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
                return new DummyDisposable();
            }

            public void ClearSubscriptions<T>() where T : struct { }

            public void ClearAllSubscriptions()
            {
                PublishedEvents.Clear();
            }

            private class DummyDisposable : System.IDisposable
            {
                public void Dispose() { }
            }
        }

        #endregion
    }
}

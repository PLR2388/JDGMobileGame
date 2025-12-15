using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Application.Repositories;
using JDG.Infrastructure.Services;
using JDG.PlayMode.Tests.TestHelpers;
using Phase = JDG.Domain.Phase;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// PlayMode tests for GameLoop.
    /// Tests game phase transitions, player death handling, and turn flow.
    /// Phase: Test coverage for critical paths.
    /// </summary>
    [TestFixture]
    public class GameLoopPlayTests
    {
        private GameStateService _gameStateService;
        private TestEventBus _eventBus;
        private TestGameStateRepository _gameStateRepository;
        private GameObject _testContainer;

        [SetUp]
        public void SetUp()
        {
            _testContainer = new GameObject("TestContainer");
            _eventBus = new TestEventBus();
            _gameStateRepository = new TestGameStateRepository();
            _gameStateService = new GameStateService(_gameStateRepository, _eventBus);
        }

        [TearDown]
        public void TearDown()
        {
            if (_testContainer != null)
                Object.DestroyImmediate(_testContainer);
        }

        #region GameStateService Phase Tests

        [UnityTest]
        public IEnumerator GameStateService_InitialPhase_IsDraw()
        {
            yield return null;

            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_InitialPlayer_IsPlayer1()
        {
            yield return null;

            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
        }

        [UnityTest]
        public IEnumerator GameStateService_InitialTurnNumber_IsZero()
        {
            yield return null;

            Assert.AreEqual(0, _gameStateService.TurnNumber);
        }

        [UnityTest]
        public IEnumerator GameStateService_StartNewTurn_IncrementsTurnNumber()
        {
            // Arrange
            var initialTurn = _gameStateService.TurnNumber;

            // Act
            _gameStateService.StartNewTurn();

            yield return null;

            // Assert
            Assert.AreEqual(initialTurn + 1, _gameStateService.TurnNumber);
        }

        [UnityTest]
        public IEnumerator GameStateService_NextPhase_TransitionsFromDrawToChoose()
        {
            // Arrange
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);

            // Act
            _gameStateService.NextPhase();

            yield return null;

            // Assert
            Assert.AreEqual(Phase.Choose, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_NextPhase_TransitionsFromChooseToAttack()
        {
            // Arrange
            _gameStateService.NextPhase(); // Draw -> Choose

            // Act
            _gameStateService.NextPhase(); // Choose -> Attack

            yield return null;

            // Assert
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_NextPhase_TransitionsFromAttackToEnd()
        {
            // Arrange
            _gameStateService.NextPhase(); // Draw -> Choose
            _gameStateService.NextPhase(); // Choose -> Attack

            // Act
            _gameStateService.NextPhase(); // Attack -> End

            yield return null;

            // Assert
            Assert.AreEqual(Phase.End, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_NextPhase_TransitionsFromEndToDraw()
        {
            // Arrange
            _gameStateService.NextPhase(); // Draw -> Choose
            _gameStateService.NextPhase(); // Choose -> Attack
            _gameStateService.NextPhase(); // Attack -> End

            // Act
            _gameStateService.NextPhase(); // End -> Draw

            yield return null;

            // Assert
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_SetPhase_ChangesPhaseDirectly()
        {
            // Act
            _gameStateService.SetPhase(Phase.Attack);

            yield return null;

            // Assert
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_SetPhase_ToGameOver_SetsGameOverPhase()
        {
            // Act
            _gameStateService.SetPhase(Phase.GameOver);

            yield return null;

            // Assert
            Assert.AreEqual(Phase.GameOver, _gameStateService.CurrentPhase);
        }

        #endregion

        #region Player Switching Tests

        [UnityTest]
        public IEnumerator GameStateService_HandleEndTurn_SwitchesCurrentPlayer()
        {
            // Arrange
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);

            // Act
            _gameStateService.HandleEndTurn();

            yield return null;

            // Assert
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer);
        }

        [UnityTest]
        public IEnumerator GameStateService_HandleEndTurn_TwiceSwitchesBackToPlayer1()
        {
            // Arrange
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);

            // Act
            _gameStateService.HandleEndTurn(); // P1 -> P2
            _gameStateService.HandleEndTurn(); // P2 -> P1

            yield return null;

            // Assert
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
        }

        #endregion

        #region Event Publishing Tests

        [UnityTest]
        public IEnumerator GameStateService_NextPhase_PublishesPhaseChangedEvent()
        {
            // Arrange
            _eventBus.PublishedEvents.Clear();

            // Act
            _gameStateService.NextPhase();

            yield return null;

            // Assert
            Assert.IsTrue(_eventBus.HasEvent<PhaseChangedEvent>(),
                "Should publish PhaseChangedEvent on phase transition");
        }

        [UnityTest]
        public IEnumerator GameStateService_SetPhase_PublishesPhaseChangedEvent()
        {
            // Arrange
            _eventBus.PublishedEvents.Clear();

            // Act
            _gameStateService.SetPhase(Phase.Attack);

            yield return null;

            // Assert
            Assert.IsTrue(_eventBus.HasEvent<PhaseChangedEvent>(),
                "Should publish PhaseChangedEvent when setting phase directly");
        }

        [UnityTest]
        public IEnumerator GameStateService_HandleEndTurn_PublishesPlayerTurnChangedEvent()
        {
            // Arrange
            _eventBus.PublishedEvents.Clear();

            // Act
            _gameStateService.HandleEndTurn();

            yield return null;

            // Assert
            Assert.IsTrue(_eventBus.HasEvent<PlayerTurnChangedEvent>(),
                "Should publish PlayerTurnChangedEvent on turn end");
        }

        [UnityTest]
        public IEnumerator GameStateService_StartNewTurn_PublishesTurnStartEvent()
        {
            // Arrange
            _eventBus.PublishedEvents.Clear();

            // Act
            _gameStateService.StartNewTurn();

            yield return null;

            // Assert
            Assert.IsTrue(_eventBus.HasEvent<TurnStartEvent>(),
                "Should publish TurnStartEvent on new turn");
        }

        #endregion

        #region Full Turn Cycle Tests

        [UnityTest]
        public IEnumerator GameStateService_FullTurnCycle_CompletesAllPhases()
        {
            // Arrange
            _gameStateService.StartNewTurn();
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);

            // Act - Complete full turn
            _gameStateService.NextPhase(); // Draw -> Choose
            Assert.AreEqual(Phase.Choose, _gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // Choose -> Attack
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // Attack -> End
            Assert.AreEqual(Phase.End, _gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // End -> Draw
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);

            yield return null;

            // Assert - Back to Draw phase
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_MultipleCompleteTurns_MaintainsIntegrity()
        {
            // Act - Complete 3 full turn cycles
            for (int i = 0; i < 3; i++)
            {
                _gameStateService.StartNewTurn();
                _gameStateService.NextPhase(); // Draw -> Choose
                _gameStateService.NextPhase(); // Choose -> Attack
                _gameStateService.NextPhase(); // Attack -> End
                _gameStateService.HandleEndTurn(); // Switch player
                _gameStateService.NextPhase(); // End -> Draw
            }

            yield return null;

            // Assert
            Assert.AreEqual(3, _gameStateService.TurnNumber);
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        #endregion

        #region Edge Cases

        [UnityTest]
        public IEnumerator GameStateService_SetPhaseMultipleTimes_LastOneWins()
        {
            // Act
            _gameStateService.SetPhase(Phase.Choose);
            _gameStateService.SetPhase(Phase.Attack);
            _gameStateService.SetPhase(Phase.End);

            yield return null;

            // Assert
            Assert.AreEqual(Phase.End, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_CurrentPlayerCardOwner_ReturnsCorrectOwner()
        {
            // Arrange & Assert Player1
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
            var ownerP1 = _gameStateService.CurrentPlayer.ToCardOwner();
            Assert.AreEqual(JDG.Domain.CardOwner.Player1, ownerP1);

            // Act
            _gameStateService.HandleEndTurn();

            // Assert Player2
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer);
            var ownerP2 = _gameStateService.CurrentPlayer.ToCardOwner();
            Assert.AreEqual(JDG.Domain.CardOwner.Player2, ownerP2);

            yield return null;
        }

        #endregion
    }

    /// <summary>
    /// In-memory test implementation of IGameStateRepository.
    /// </summary>
    public class TestGameStateRepository : IGameStateRepository
    {
        public Phase CurrentPhase { get; private set; } = Phase.Draw;
        public int TurnNumber { get; private set; } = 0;
        public PlayerId CurrentPlayer { get; private set; } = PlayerId.Player1;
        public bool IsGameOver { get; private set; } = false;

        public void SetPhase(Phase phase) => CurrentPhase = phase;
        public void IncrementTurn() => TurnNumber++;
        public void SetCurrentPlayer(PlayerId playerId) => CurrentPlayer = playerId;
        public void SwitchPlayer() => CurrentPlayer = CurrentPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        public void SetGameOver(bool isGameOver) => IsGameOver = isGameOver;
        public void ResetGameState()
        {
            CurrentPhase = Phase.Draw;
            TurnNumber = 0;
            CurrentPlayer = PlayerId.Player1;
            IsGameOver = false;
        }
    }
}

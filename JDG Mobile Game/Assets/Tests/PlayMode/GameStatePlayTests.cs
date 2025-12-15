using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Events;
using JDG.Infrastructure.Repositories;
using JDG.Infrastructure.Services;
using JDG.TestUtilities;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// Play Mode tests for game state management.
    /// Tests phase transitions, turn management, and game flow in Unity runtime.
    /// Phase 11: Play Mode test implementation.
    /// </summary>
    [TestFixture]
    public class GameStatePlayTests
    {
        private IEventBus _eventBus;
        private IGameStateRepository _gameStateRepository;
        private GameStateService _gameStateService;
        private List<PhaseChangedEvent> _phaseEvents;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _gameStateRepository = new GameStateRepository();
            _gameStateService = new GameStateService(_gameStateRepository, _eventBus);
            _phaseEvents = new List<PhaseChangedEvent>();

            _eventBus.Subscribe<PhaseChangedEvent>(e => _phaseEvents.Add(e));
        }

        [TearDown]
        public void TearDown()
        {
            _eventBus.ClearAllSubscriptions();
            _phaseEvents.Clear();
        }

        [UnityTest]
        public IEnumerator GameStateService_InitialPhase_IsDraw()
        {
            yield return null;

            // Assert - Default enum value is Draw (0)
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_SetPhase_UpdatesCurrentPhase()
        {
            yield return null;

            // Act
            _gameStateService.SetPhase(Phase.Draw);

            // Assert
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_SetPhase_PublishesPhaseChangedEvent()
        {
            yield return null;

            // Act
            _gameStateService.SetPhase(Phase.Draw);

            // Assert
            Assert.AreEqual(1, _phaseEvents.Count);
            Assert.AreEqual(Phase.Draw, _phaseEvents[0].NewPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_PhaseTransitions_WorkCorrectly()
        {
            yield return null;

            // Act - Simulate game flow
            _gameStateService.SetPhase(Phase.Draw);
            yield return null;

            _gameStateService.SetPhase(Phase.Choose);
            yield return null;

            _gameStateService.SetPhase(Phase.Attack);
            yield return null;

            _gameStateService.SetPhase(Phase.End);
            yield return null;

            // Assert
            Assert.AreEqual(4, _phaseEvents.Count);
            Assert.AreEqual(Phase.Draw, _phaseEvents[0].NewPhase);
            Assert.AreEqual(Phase.Choose, _phaseEvents[1].NewPhase);
            Assert.AreEqual(Phase.Attack, _phaseEvents[2].NewPhase);
            Assert.AreEqual(Phase.End, _phaseEvents[3].NewPhase);
        }

        [UnityTest]
        public IEnumerator GameStateService_MultiplePhaseChanges_TrackedCorrectly()
        {
            yield return null;

            // Simulate multiple rounds
            for (int round = 0; round < 3; round++)
            {
                _gameStateService.SetPhase(Phase.Draw);
                _gameStateService.SetPhase(Phase.Choose);
                _gameStateService.SetPhase(Phase.Attack);
                _gameStateService.SetPhase(Phase.End);
                yield return null;
            }

            // Assert - 4 phases per round * 3 rounds = 12 events
            Assert.AreEqual(12, _phaseEvents.Count);
        }

        [UnityTest]
        public IEnumerator EventBus_PhaseAndCardEvents_CoexistCorrectly()
        {
            // Arrange
            var cardEvents = new List<CardDrawnEvent>();
            _eventBus.Subscribe<CardDrawnEvent>(e => cardEvents.Add(e));

            yield return null;

            // Act - Mix of phase and card events
            _gameStateService.SetPhase(Phase.Draw);
            _eventBus.Publish(new CardDrawnEvent { Owner = CardOwner.Player1 });
            _eventBus.Publish(new CardDrawnEvent { Owner = CardOwner.Player1 });
            _gameStateService.SetPhase(Phase.Choose);
            _eventBus.Publish(new CardDrawnEvent { Owner = CardOwner.Player2 });

            yield return null;

            // Assert
            Assert.AreEqual(2, _phaseEvents.Count);
            Assert.AreEqual(3, cardEvents.Count);
        }

        [UnityTest]
        public IEnumerator GameStateService_GameOverPhase_SetCorrectly()
        {
            yield return null;

            // Act
            _gameStateService.SetPhase(Phase.Draw);
            _gameStateService.SetPhase(Phase.GameOver);

            // Assert
            Assert.AreEqual(Phase.GameOver, _gameStateService.CurrentPhase);
            Assert.AreEqual(2, _phaseEvents.Count);
            Assert.AreEqual(Phase.GameOver, _phaseEvents[1].NewPhase);
        }
    }

    /// <summary>
    /// Play Mode tests for player state management.
    /// Tests player creation, HP changes, and win/loss conditions.
    /// </summary>
    [TestFixture]
    public class PlayerStatePlayTests
    {
        [UnityTest]
        public IEnumerator Player_Creation_HasCorrectInitialValues()
        {
            yield return null;

            // Act
            var player = PlayerFactory.CreatePlayer1();

            // Assert
            Assert.AreEqual(PlayerId.Player1, player.Id);
            Assert.AreEqual(30, player.Health); // Default maxHealth is 30
        }

        [UnityTest]
        public IEnumerator Player_DrawCard_MovesCardFromDeckToHand()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1(deckSize: 30);
            var initialDeckCount = player.Deck.Count;
            var initialHandCount = player.HandCount;

            yield return null;

            // Act
            var drawnCard = player.DrawCard();

            // Assert
            Assert.IsNotNull(drawnCard);
            Assert.AreEqual(initialDeckCount - 1, player.Deck.Count);
            Assert.AreEqual(initialHandCount + 1, player.HandCount);
        }

        [UnityTest]
        public IEnumerator Player_MultipleDraws_WorkAcrossFrames()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1(deckSize: 30);

            // Act - Draw across multiple frames
            for (int i = 0; i < 5; i++)
            {
                player.DrawCard();
                yield return null;
            }

            // Assert
            Assert.AreEqual(25, player.Deck.Count);
            Assert.AreEqual(5, player.HandCount);
        }

        [UnityTest]
        public IEnumerator Player_EmptyDeck_DrawReturnsNull()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1(deckSize: 0);

            yield return null;

            // Act
            var drawnCard = player.DrawCard();

            // Assert
            Assert.IsNull(drawnCard);
        }

        [UnityTest]
        public IEnumerator TwoPlayers_IndependentState()
        {
            // Arrange
            var player1 = PlayerFactory.CreatePlayer1(deckSize: 30);
            var player2 = PlayerFactory.CreatePlayer2(deckSize: 30);

            yield return null;

            // Act - Player 1 draws cards
            player1.DrawCard();
            player1.DrawCard();

            // Assert - Player 2's deck is unaffected
            Assert.AreEqual(28, player1.Deck.Count);
            Assert.AreEqual(30, player2.Deck.Count);
            Assert.AreEqual(2, player1.HandCount);
            Assert.AreEqual(0, player2.HandCount);
        }
    }
}

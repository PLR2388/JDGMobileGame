using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using DomainCardOwner = JDG.Domain.CardOwner;
using JDG.Infrastructure.Events;
using JDG.Infrastructure.Repositories;
using JDG.Infrastructure.Services;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// PlayMode integration tests for service layer interactions.
    /// Tests that services work correctly together in Unity runtime.
    /// Phase 97: Service integration verification.
    ///
    /// Note: Uses ContainerBuilder directly instead of LifetimeScope MonoBehaviour
    /// because AddComponent doesn't reliably trigger VContainer's Build() in test isolation.
    /// </summary>
    [TestFixture]
    public class ServiceIntegrationPlayTests
    {
        private IObjectResolver _container;

        [TearDown]
        public void TearDown()
        {
            _container?.Dispose();
            _container = null;
        }

        /// <summary>
        /// Creates a test container with all core services registered.
        /// </summary>
        private IObjectResolver BuildFullServiceContainer()
        {
            var builder = new ContainerBuilder();

            // Core Infrastructure
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // Repositories
            builder.Register<ICardRepository, CardRepository>(Lifetime.Singleton);
            builder.Register<IPlayerRepository, PlayerRepository>(Lifetime.Singleton);
            builder.Register<IDeckRepository, DeckRepository>(Lifetime.Singleton);
            builder.Register<IGameStateRepository, GameStateRepository>(Lifetime.Singleton);

            // Services
            builder.Register<GameStateService>(Lifetime.Singleton);
            builder.Register<ICardStateService, CardStateService>(Lifetime.Singleton);

            // Ability System
            builder.Register<AbilityRegistry>(Lifetime.Singleton);

            return builder.Build();
        }

        #region Service Resolution Tests

        [UnityTest]
        public IEnumerator AllCoreServices_ResolveSuccessfully()
        {
            // Arrange
            _container = BuildFullServiceContainer();
            yield return null;

            // Act & Assert - All core services should resolve
            // Infrastructure
            Assert.IsNotNull(_container.Resolve<IEventBus>(), "IEventBus should resolve");
            Assert.IsNotNull(_container.Resolve<IGameStateRepository>(), "IGameStateRepository should resolve");
            Assert.IsNotNull(_container.Resolve<IPlayerRepository>(), "IPlayerRepository should resolve");
            Assert.IsNotNull(_container.Resolve<ICardRepository>(), "ICardRepository should resolve");

            // Services
            Assert.IsNotNull(_container.Resolve<GameStateService>(), "GameStateService should resolve");
            Assert.IsNotNull(_container.Resolve<ICardStateService>(), "ICardStateService should resolve");

            // Ability System
            Assert.IsNotNull(_container.Resolve<AbilityRegistry>(), "AbilityRegistry should resolve");
        }

        [UnityTest]
        public IEnumerator GameStateService_WorksWithEventBus_Integration()
        {
            // Arrange
            _container = BuildFullServiceContainer();
            yield return null;

            var eventBus = _container.Resolve<IEventBus>();
            var gameStateService = _container.Resolve<GameStateService>();

            // Subscribe to phase change events
            PhaseChangedEvent? receivedEvent = null;
            eventBus.Subscribe<PhaseChangedEvent>(e => receivedEvent = e);

            // Act - Change phase
            gameStateService.NextPhase();

            yield return null;

            // Assert - Event should have been published
            Assert.IsTrue(receivedEvent.HasValue, "PhaseChangedEvent should be received");
            Assert.AreEqual(Phase.Draw, receivedEvent.Value.OldPhase);
            Assert.AreEqual(Phase.Choose, receivedEvent.Value.NewPhase);
        }

        [UnityTest]
        public IEnumerator CardStateService_WorksWithDomainEntities_Integration()
        {
            // Arrange
            _container = BuildFullServiceContainer();
            yield return null;

            var cardStateService = _container.Resolve<ICardStateService>();

            // Create test state
            var state = new JDG.Domain.Entities.InvocationCardState(
                cardDefinitionId: JDG.Domain.ValueObjects.CardId.New(),
                owner: DomainCardOwner.Player1,
                baseAttack: 50,
                baseDefense: 100,
                families: new[] { JDG.Domain.Enums.CardFamily.Comics },
                abilities: System.Array.Empty<AbilityName>(),
                conditions: System.Array.Empty<JDG.Domain.Enums.ConditionName>(),
                isAffectedByEffect: true);

            // Act - Apply damage
            var isDestroyed = cardStateService.ApplyDamage(state, 30);

            yield return null;

            // Assert
            Assert.IsFalse(isDestroyed);
            Assert.AreEqual(70, state.CurrentDefense);
        }

        [UnityTest]
        public IEnumerator GameStateService_EndGame_PublishesGameOverEvent()
        {
            // Arrange
            _container = BuildFullServiceContainer();
            yield return null;

            var eventBus = _container.Resolve<IEventBus>();
            var gameStateService = _container.Resolve<GameStateService>();

            // Subscribe to game over events
            GameOverEvent? receivedEvent = null;
            eventBus.Subscribe<GameOverEvent>(e => receivedEvent = e);

            // Act - End game
            gameStateService.EndGame(DomainCardOwner.Player1, "Test Victory");

            yield return null;

            // Assert
            Assert.IsTrue(receivedEvent.HasValue, "GameOverEvent should be received");
            Assert.AreEqual(DomainCardOwner.Player1, receivedEvent.Value.Winner);
            Assert.AreEqual("Test Victory", receivedEvent.Value.Reason);
            Assert.AreEqual(Phase.GameOver, gameStateService.CurrentPhase);
        }

        #endregion

        #region Full Turn Cycle Integration

        [UnityTest]
        public IEnumerator FullTurnCycle_PublishesAllExpectedEvents()
        {
            // Arrange
            _container = BuildFullServiceContainer();
            yield return null;

            var eventBus = _container.Resolve<IEventBus>() as EventBus;
            var gameStateService = _container.Resolve<GameStateService>();

            // Track all events
            var phaseChanges = 0;
            var turnStarts = 0;
            var turnEnds = 0;
            var playerChanges = 0;

            eventBus.Subscribe<PhaseChangedEvent>(_ => phaseChanges++);
            eventBus.Subscribe<TurnStartEvent>(_ => turnStarts++);
            eventBus.Subscribe<TurnEndEvent>(_ => turnEnds++);
            eventBus.Subscribe<PlayerTurnChangedEvent>(_ => playerChanges++);

            // Act - Complete one full turn
            // Note: HandleEndTurn() internally calls StartNewTurn(), so calling both causes 2 TurnStartEvents
            // For a single turn cycle test, only call StartNewTurn() at the beginning
            gameStateService.StartNewTurn();
            gameStateService.NextPhase(); // Draw -> Choose
            gameStateService.NextPhase(); // Choose -> Attack
            gameStateService.NextPhase(); // Attack -> End
            gameStateService.EndTurn(); // Just publish TurnEndEvent without starting next turn

            yield return null;

            // Assert
            Assert.AreEqual(1, turnStarts, "Should have 1 turn start");
            Assert.AreEqual(1, turnEnds, "Should have 1 turn end");
            // Note: We're not calling HandleEndTurn(), so no player change
            Assert.AreEqual(0, playerChanges, "Should have 0 player changes (EndTurn doesn't switch)");
            Assert.GreaterOrEqual(phaseChanges, 3, "Should have at least 3 phase changes");
        }

        #endregion

        #region Singleton Consistency Tests

        [UnityTest]
        public IEnumerator SingletonServices_ReturnSameInstance_AcrossResolutions()
        {
            // Arrange
            _container = BuildFullServiceContainer();
            yield return null;

            // Resolve multiple times
            var eventBus1 = _container.Resolve<IEventBus>();
            var eventBus2 = _container.Resolve<IEventBus>();
            var registry1 = _container.Resolve<AbilityRegistry>();
            var registry2 = _container.Resolve<AbilityRegistry>();

            yield return null;

            // Assert - Singletons should be same instance
            Assert.AreSame(eventBus1, eventBus2, "EventBus should be singleton");
            Assert.AreSame(registry1, registry2, "AbilityRegistry should be singleton");
        }

        #endregion
    }
}

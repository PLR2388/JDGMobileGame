using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain.Events;
using JDG.Infrastructure.Events;
using JDG.Infrastructure.Repositories;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// Play Mode tests for VContainer dependency injection.
    /// Tests that DI container resolves dependencies correctly in Unity runtime.
    /// Phase 11: Play Mode test implementation.
    ///
    /// Note: Uses ContainerBuilder directly instead of LifetimeScope MonoBehaviour
    /// because AddComponent doesn't reliably trigger VContainer's Build() in test isolation.
    /// </summary>
    [TestFixture]
    public class DIContainerPlayTests
    {
        private IObjectResolver _container;

        [TearDown]
        public void TearDown()
        {
            _container?.Dispose();
            _container = null;
        }

        /// <summary>
        /// Creates a test container with core services registered.
        /// </summary>
        private IObjectResolver BuildTestContainer()
        {
            var builder = new ContainerBuilder();

            // Core Infrastructure
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // Repositories
            builder.Register<ICardRepository, CardRepository>(Lifetime.Singleton);
            builder.Register<IPlayerRepository, PlayerRepository>(Lifetime.Singleton);
            builder.Register<IDeckRepository, DeckRepository>(Lifetime.Singleton);
            builder.Register<IGameStateRepository, GameStateRepository>(Lifetime.Singleton);

            // Ability System
            builder.Register<AbilityRegistry>(Lifetime.Singleton);

            return builder.Build();
        }

        [UnityTest]
        public IEnumerator DIContainer_ResolvesEventBus_AsSingleton()
        {
            // Arrange
            _container = BuildTestContainer();
            yield return null;

            // Act
            var eventBus1 = _container.Resolve<IEventBus>();
            var eventBus2 = _container.Resolve<IEventBus>();

            // Assert - Should be same instance (singleton)
            Assert.IsNotNull(eventBus1);
            Assert.AreSame(eventBus1, eventBus2);
        }

        [UnityTest]
        public IEnumerator DIContainer_ResolvesRepositories_Correctly()
        {
            // Arrange
            _container = BuildTestContainer();
            yield return null;

            // Act
            var playerRepo = _container.Resolve<IPlayerRepository>();
            var cardRepo = _container.Resolve<ICardRepository>();

            // Assert
            Assert.IsNotNull(playerRepo);
            Assert.IsNotNull(cardRepo);
        }

        [UnityTest]
        public IEnumerator DIContainer_ResolvesAbilityRegistry_AsSingleton()
        {
            // Arrange
            _container = BuildTestContainer();
            yield return null;

            // Act
            var registry1 = _container.Resolve<AbilityRegistry>();
            var registry2 = _container.Resolve<AbilityRegistry>();

            // Assert
            Assert.IsNotNull(registry1);
            Assert.AreSame(registry1, registry2);
        }

        [UnityTest]
        public IEnumerator DIContainer_EventBusWorks_AfterResolution()
        {
            // Arrange
            _container = BuildTestContainer();
            yield return null;

            // Act
            var eventBus = _container.Resolve<IEventBus>();

            var eventReceived = false;
            eventBus.Subscribe<CardDrawnEvent>(e => eventReceived = true);
            eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });

            yield return null;

            // Assert
            Assert.IsTrue(eventReceived);
        }

        [UnityTest]
        public IEnumerator DIContainer_MultipleResolutions_ReturnConsistentInstances()
        {
            // Arrange
            _container = BuildTestContainer();
            yield return null;

            // Act - Resolve multiple times across frames
            var eventBus1 = _container.Resolve<IEventBus>();
            yield return null;

            var eventBus2 = _container.Resolve<IEventBus>();
            yield return null;

            var eventBus3 = _container.Resolve<IEventBus>();

            // Assert - All should be same instance
            Assert.AreSame(eventBus1, eventBus2);
            Assert.AreSame(eventBus2, eventBus3);
        }
    }
}

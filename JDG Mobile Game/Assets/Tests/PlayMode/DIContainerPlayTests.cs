using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;
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
    /// </summary>
    [TestFixture]
    public class DIContainerPlayTests
    {
        private LifetimeScope _testScope;
        private GameObject _scopeObject;

        [SetUp]
        public void SetUp()
        {
            _scopeObject = new GameObject("TestScope");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testScope != null)
            {
                Object.DestroyImmediate(_testScope);
            }
            if (_scopeObject != null)
            {
                Object.DestroyImmediate(_scopeObject);
            }
        }

        [UnityTest]
        public IEnumerator DIContainer_ResolvesEventBus_AsSingleton()
        {
            // Arrange
            _testScope = _scopeObject.AddComponent<TestLifetimeScope>();

            yield return null; // Wait for scope to initialize

            // Act
            using (LifetimeScope.EnqueueParent(_testScope))
            {
                var container = _testScope.Container;
                var eventBus1 = container.Resolve<IEventBus>();
                var eventBus2 = container.Resolve<IEventBus>();

                // Assert - Should be same instance (singleton)
                Assert.IsNotNull(eventBus1);
                Assert.AreSame(eventBus1, eventBus2);
            }
        }

        [UnityTest]
        public IEnumerator DIContainer_ResolvesRepositories_Correctly()
        {
            // Arrange
            _testScope = _scopeObject.AddComponent<TestLifetimeScope>();

            yield return null;

            // Act
            using (LifetimeScope.EnqueueParent(_testScope))
            {
                var container = _testScope.Container;
                var playerRepo = container.Resolve<IPlayerRepository>();
                var cardRepo = container.Resolve<ICardRepository>();

                // Assert
                Assert.IsNotNull(playerRepo);
                Assert.IsNotNull(cardRepo);
            }
        }

        [UnityTest]
        public IEnumerator DIContainer_ResolvesAbilityRegistry_AsSingleton()
        {
            // Arrange
            _testScope = _scopeObject.AddComponent<TestLifetimeScope>();

            yield return null;

            // Act
            using (LifetimeScope.EnqueueParent(_testScope))
            {
                var container = _testScope.Container;
                var registry1 = container.Resolve<AbilityRegistry>();
                var registry2 = container.Resolve<AbilityRegistry>();

                // Assert
                Assert.IsNotNull(registry1);
                Assert.AreSame(registry1, registry2);
            }
        }

        [UnityTest]
        public IEnumerator DIContainer_EventBusWorks_AfterResolution()
        {
            // Arrange
            _testScope = _scopeObject.AddComponent<TestLifetimeScope>();

            yield return null;

            // Act
            using (LifetimeScope.EnqueueParent(_testScope))
            {
                var container = _testScope.Container;
                var eventBus = container.Resolve<IEventBus>();

                var eventReceived = false;
                eventBus.Subscribe<CardDrawnEvent>(e => eventReceived = true);
                eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });

                yield return null;

                // Assert
                Assert.IsTrue(eventReceived);
            }
        }

        [UnityTest]
        public IEnumerator DIContainer_MultipleResolutions_ReturnConsistentInstances()
        {
            // Arrange
            _testScope = _scopeObject.AddComponent<TestLifetimeScope>();

            yield return null;

            // Act - Resolve multiple times across frames
            IEventBus eventBus1 = null;
            IEventBus eventBus2 = null;
            IEventBus eventBus3 = null;

            using (LifetimeScope.EnqueueParent(_testScope))
            {
                var container = _testScope.Container;

                eventBus1 = container.Resolve<IEventBus>();
                yield return null;

                eventBus2 = container.Resolve<IEventBus>();
                yield return null;

                eventBus3 = container.Resolve<IEventBus>();
            }

            // Assert - All should be same instance
            Assert.AreSame(eventBus1, eventBus2);
            Assert.AreSame(eventBus2, eventBus3);
        }
    }

    /// <summary>
    /// Test lifetime scope for DI container tests.
    /// Registers core services needed for testing.
    /// </summary>
    public class TestLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Core Infrastructure
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // Repositories
            builder.Register<ICardRepository, CardRepository>(Lifetime.Singleton);
            builder.Register<IPlayerRepository, PlayerRepository>(Lifetime.Singleton);
            builder.Register<IDeckRepository, DeckRepository>(Lifetime.Singleton);
            builder.Register<IGameStateRepository, GameStateRepository>(Lifetime.Singleton);

            // Ability System
            builder.Register<AbilityRegistry>(Lifetime.Singleton);
        }
    }
}

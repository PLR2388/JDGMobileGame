using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Infrastructure.Events;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// Play Mode tests for the EventBus system.
    /// Tests event publishing and subscription in Unity runtime context.
    /// Phase 11: Play Mode test implementation.
    /// </summary>
    [TestFixture]
    public class EventBusPlayTests
    {
        private IEventBus _eventBus;
        private List<object> _receivedEvents;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _receivedEvents = new List<object>();
        }

        [TearDown]
        public void TearDown()
        {
            _eventBus.ClearAllSubscriptions();
            _receivedEvents.Clear();
        }

        [UnityTest]
        public IEnumerator EventBus_PublishAndSubscribe_WorksInPlayMode()
        {
            // Arrange
            _eventBus.Subscribe<CardDrawnEvent>(e => _receivedEvents.Add(e));

            // Act
            _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });

            // Wait one frame for any async processing
            yield return null;

            // Assert
            Assert.AreEqual(1, _receivedEvents.Count);
            Assert.IsInstanceOf<CardDrawnEvent>(_receivedEvents[0]);
        }

        [UnityTest]
        public IEnumerator EventBus_MultipleSubscribers_AllReceiveEvent()
        {
            // Arrange
            var subscriber1Received = false;
            var subscriber2Received = false;
            var subscriber3Received = false;

            _eventBus.Subscribe<PhaseChangedEvent>(e => subscriber1Received = true);
            _eventBus.Subscribe<PhaseChangedEvent>(e => subscriber2Received = true);
            _eventBus.Subscribe<PhaseChangedEvent>(e => subscriber3Received = true);

            // Act
            _eventBus.Publish(new PhaseChangedEvent { NewPhase = JDG.Domain.Phase.Draw });

            yield return null;

            // Assert
            Assert.IsTrue(subscriber1Received);
            Assert.IsTrue(subscriber2Received);
            Assert.IsTrue(subscriber3Received);
        }

        [UnityTest]
        public IEnumerator EventBus_Unsubscribe_StopsReceivingEvents()
        {
            // Arrange
            var eventCount = 0;
            void Handler(CardDrawnEvent e) => eventCount++;

            var subscription = _eventBus.Subscribe<CardDrawnEvent>(Handler);
            _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });

            yield return null;
            Assert.AreEqual(1, eventCount);

            // Act - Unsubscribe by disposing the subscription
            subscription.Dispose();
            _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player2 });

            yield return null;

            // Assert - Count should not increase
            Assert.AreEqual(1, eventCount);
        }

        [UnityTest]
        public IEnumerator EventBus_DifferentEventTypes_DoNotInterfere()
        {
            // Arrange
            var cardDrawnCount = 0;
            var phaseChangedCount = 0;

            _eventBus.Subscribe<CardDrawnEvent>(e => cardDrawnCount++);
            _eventBus.Subscribe<PhaseChangedEvent>(e => phaseChangedCount++);

            // Act
            _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });
            _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });
            _eventBus.Publish(new PhaseChangedEvent { NewPhase = JDG.Domain.Phase.Attack });

            yield return null;

            // Assert
            Assert.AreEqual(2, cardDrawnCount);
            Assert.AreEqual(1, phaseChangedCount);
        }

        [UnityTest]
        public IEnumerator EventBus_PublishAcrossFrames_WorksCorrectly()
        {
            // Arrange
            var eventCount = 0;
            _eventBus.Subscribe<TurnEndEvent>(e => eventCount++);

            // Act - Publish across multiple frames
            for (int i = 0; i < 5; i++)
            {
                _eventBus.Publish(new TurnEndEvent { CurrentPlayer = JDG.Domain.CardOwner.Player1, TurnNumber = i });
                yield return null; // Wait one frame between each publish
            }

            // Assert
            Assert.AreEqual(5, eventCount);
        }

        [UnityTest]
        public IEnumerator EventBus_ClearAllSubscriptions_RemovesAllHandlers()
        {
            // Arrange
            var eventCount = 0;
            _eventBus.Subscribe<CardDrawnEvent>(e => eventCount++);
            _eventBus.Subscribe<PhaseChangedEvent>(e => eventCount++);
            _eventBus.Subscribe<TurnEndEvent>(e => eventCount++);

            // Publish once to verify subscriptions work
            _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });
            yield return null;
            Assert.AreEqual(1, eventCount);

            // Act - Clear all subscriptions
            _eventBus.ClearAllSubscriptions();

            // Publish again
            _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });
            _eventBus.Publish(new PhaseChangedEvent { NewPhase = JDG.Domain.Phase.Draw });
            _eventBus.Publish(new TurnEndEvent { CurrentPlayer = JDG.Domain.CardOwner.Player1, TurnNumber = 1 });

            yield return null;

            // Assert - Count should not increase
            Assert.AreEqual(1, eventCount);
        }
    }
}

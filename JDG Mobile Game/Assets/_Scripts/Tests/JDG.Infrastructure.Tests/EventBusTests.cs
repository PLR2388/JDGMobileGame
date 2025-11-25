using System;
using NUnit.Framework;
using JDG.Application;
using JDG.Infrastructure.Events;
using JDG.Domain;
using JDG.Domain.Events;

namespace JDG.Infrastructure.Tests
{
    [TestFixture]
    public class EventBusTests
    {
        private EventBus _eventBus;

        // Test event
        private struct TestEvent
        {
            public int Value;
            public string Message;
        }

        [SetUp]
        public void Setup()
        {
            _eventBus = new EventBus();
        }

        [TearDown]
        public void TearDown()
        {
            _eventBus.ClearAllSubscriptions();
        }

        [Test]
        public void Subscribe_WhenCalledWithHandler_RegistersSubscription()
        {
            // Arrange
            var callCount = 0;
            Action<TestEvent> handler = evt => callCount++;

            // Act
            var subscription = _eventBus.Subscribe(handler);

            // Assert
            Assert.IsNotNull(subscription);
            Assert.AreEqual(1, _eventBus.GetSubscriberCount<TestEvent>());
        }

        [Test]
        public void Publish_WhenNoSubscribers_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _eventBus.Publish(new TestEvent { Value = 42 }));
        }

        [Test]
        public void Publish_WhenHasSubscriber_InvokesHandler()
        {
            // Arrange
            TestEvent receivedEvent = default;
            var callCount = 0;

            _eventBus.Subscribe<TestEvent>(evt =>
            {
                receivedEvent = evt;
                callCount++;
            });

            var testEvent = new TestEvent { Value = 42, Message = "Hello" };

            // Act
            _eventBus.Publish(testEvent);

            // Assert
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(42, receivedEvent.Value);
            Assert.AreEqual("Hello", receivedEvent.Message);
        }

        [Test]
        public void Publish_WhenMultipleSubscribers_InvokesAllHandlers()
        {
            // Arrange
            var callCount1 = 0;
            var callCount2 = 0;
            var callCount3 = 0;

            _eventBus.Subscribe<TestEvent>(evt => callCount1++);
            _eventBus.Subscribe<TestEvent>(evt => callCount2++);
            _eventBus.Subscribe<TestEvent>(evt => callCount3++);

            // Act
            _eventBus.Publish(new TestEvent { Value = 1 });

            // Assert
            Assert.AreEqual(1, callCount1);
            Assert.AreEqual(1, callCount2);
            Assert.AreEqual(1, callCount3);
        }

        [Test]
        public void Subscribe_WhenNullHandler_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _eventBus.Subscribe<TestEvent>(null));
        }

        [Test]
        public void Dispose_WhenCalled_UnsubscribesHandler()
        {
            // Arrange
            var callCount = 0;
            var subscription = _eventBus.Subscribe<TestEvent>(evt => callCount++);

            // Act
            subscription.Dispose();
            _eventBus.Publish(new TestEvent());

            // Assert
            Assert.AreEqual(0, callCount);
            Assert.AreEqual(0, _eventBus.GetSubscriberCount<TestEvent>());
        }

        [Test]
        public void Dispose_WhenCalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var subscription = _eventBus.Subscribe<TestEvent>(evt => { });

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                subscription.Dispose();
                subscription.Dispose();
                subscription.Dispose();
            });
        }

        [Test]
        public void ClearSubscriptions_WhenCalled_RemovesAllSubscribersForEventType()
        {
            // Arrange
            var callCount = 0;
            _eventBus.Subscribe<TestEvent>(evt => callCount++);
            _eventBus.Subscribe<TestEvent>(evt => callCount++);

            // Act
            _eventBus.ClearSubscriptions<TestEvent>();
            _eventBus.Publish(new TestEvent());

            // Assert
            Assert.AreEqual(0, callCount);
            Assert.AreEqual(0, _eventBus.GetSubscriberCount<TestEvent>());
        }

        [Test]
        public void ClearAllSubscriptions_WhenCalled_RemovesAllSubscribersForAllEventTypes()
        {
            // Arrange
            var callCount1 = 0;
            var callCount2 = 0;

            _eventBus.Subscribe<TestEvent>(evt => callCount1++);

            // Different event type
            _eventBus.Subscribe<CardDrawnEvent>(evt => callCount2++);

            // Act
            _eventBus.ClearAllSubscriptions();
            _eventBus.Publish(new TestEvent());
            _eventBus.Publish(new CardDrawnEvent());

            // Assert
            Assert.AreEqual(0, callCount1);
            Assert.AreEqual(0, callCount2);
        }

        [Test]
        public void Publish_WhenHandlerThrows_ContinuesInvokingOtherHandlers()
        {
            // Arrange
            var callCount1 = 0;
            var callCount2 = 0;

            _eventBus.Subscribe<TestEvent>(evt => callCount1++);
            _eventBus.Subscribe<TestEvent>(evt => throw new Exception("Test exception"));
            _eventBus.Subscribe<TestEvent>(evt => callCount2++);

            // Act
            _eventBus.Publish(new TestEvent());

            // Assert - both handlers before and after exception should be called
            Assert.AreEqual(1, callCount1);
            Assert.AreEqual(1, callCount2);
        }

        [Test]
        public void Subscribe_WithMultipleEventTypes_MaintainsSeparateSubscriberLists()
        {
            // Arrange
            var testEventCallCount = 0;
            var cardDrawnEventCallCount = 0;

            _eventBus.Subscribe<TestEvent>(evt => testEventCallCount++);
            _eventBus.Subscribe<CardDrawnEvent>(evt => cardDrawnEventCallCount++);

            // Act
            _eventBus.Publish(new TestEvent { Value = 1 });

            // Assert - only TestEvent handler should be called
            Assert.AreEqual(1, testEventCallCount);
            Assert.AreEqual(0, cardDrawnEventCallCount);
        }

        // ============================================
        // Integration tests with domain events
        // ============================================

        [Test]
        public void Publish_WithCardDrawnEvent_InvokesSubscribers()
        {
            // Arrange
            CardDrawnEvent receivedEvent = default;
            var cardId = Guid.NewGuid();

            _eventBus.Subscribe<CardDrawnEvent>(evt => receivedEvent = evt);

            // Act
            _eventBus.Publish(new CardDrawnEvent
            {
                CardId = cardId,
                Owner = CardOwner.Player1,
                CardTitle = "Test Card"
            });

            // Assert
            Assert.AreEqual(cardId, receivedEvent.CardId);
            Assert.AreEqual(CardOwner.Player1, receivedEvent.Owner);
            Assert.AreEqual("Test Card", receivedEvent.CardTitle);
        }

        [Test]
        public void Publish_WithPhaseChangedEvent_InvokesSubscribers()
        {
            // Arrange
            PhaseChangedEvent receivedEvent = default;

            _eventBus.Subscribe<PhaseChangedEvent>(evt => receivedEvent = evt);

            // Act
            _eventBus.Publish(new PhaseChangedEvent
            {
                OldPhase = Phase.Draw,
                NewPhase = Phase.Choose,
                TurnNumber = 1
            });

            // Assert
            Assert.AreEqual(Phase.Draw, receivedEvent.OldPhase);
            Assert.AreEqual(Phase.Choose, receivedEvent.NewPhase);
            Assert.AreEqual(1, receivedEvent.TurnNumber);
        }
    }
}

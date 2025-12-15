using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Infrastructure.Events;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// Play Mode tests for MonoBehaviour integration.
    /// Tests that services work correctly when used from MonoBehaviours.
    /// Phase 11: Play Mode test implementation.
    /// </summary>
    [TestFixture]
    public class MonoBehaviourPlayTests
    {
        private GameObject _testObject;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestObject");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
            {
                Object.DestroyImmediate(_testObject);
            }
        }

        [UnityTest]
        public IEnumerator MonoBehaviour_CanSubscribeToEventBus()
        {
            // Arrange
            var testComponent = _testObject.AddComponent<TestEventSubscriber>();
            var eventBus = new EventBus();
            testComponent.Initialize(eventBus);

            yield return null; // Wait for Start()

            // Act
            eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });

            yield return null;

            // Assert
            Assert.AreEqual(1, testComponent.EventsReceived);
        }

        [UnityTest]
        public IEnumerator MonoBehaviour_CanPublishToEventBus()
        {
            // Arrange
            var eventBus = new EventBus();
            var eventsReceived = 0;
            eventBus.Subscribe<PhaseChangedEvent>(e => eventsReceived++);

            var testComponent = _testObject.AddComponent<TestEventPublisher>();
            testComponent.Initialize(eventBus);

            yield return null;

            // Act
            testComponent.PublishPhaseChange(JDG.Domain.Phase.Draw);

            yield return null;

            // Assert
            Assert.AreEqual(1, eventsReceived);
        }

        [UnityTest]
        public IEnumerator MonoBehaviour_CoroutineWithEventBus_Works()
        {
            // Arrange
            var eventBus = new EventBus();
            var eventsReceived = 0;
            eventBus.Subscribe<CardDrawnEvent>(e => eventsReceived++);

            var testComponent = _testObject.AddComponent<TestCoroutinePublisher>();
            testComponent.Initialize(eventBus);

            yield return null;

            // Act - Start coroutine that publishes 3 events over time
            testComponent.StartPublishing(3);

            // Wait for coroutine to complete (3 events + buffer)
            yield return new WaitForSeconds(0.5f);

            // Assert
            Assert.AreEqual(3, eventsReceived);
        }

        [UnityTest]
        public IEnumerator MonoBehaviour_LifecycleMethods_ExecuteInOrder()
        {
            // Arrange
            var testComponent = _testObject.AddComponent<TestLifecycleTracker>();

            yield return null; // Wait for Awake and Start

            // Assert
            Assert.IsTrue(testComponent.AwakeCalled);
            Assert.IsTrue(testComponent.StartCalled);
            Assert.IsTrue(testComponent.AwakeCalledBeforeStart);
        }

        [UnityTest]
        public IEnumerator MonoBehaviour_UpdateCalledEveryFrame()
        {
            // Arrange
            var testComponent = _testObject.AddComponent<TestUpdateCounter>();

            // Wait for several frames
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            // Assert - Update should have been called multiple times
            Assert.GreaterOrEqual(testComponent.UpdateCount, 4);
        }
    }

    #region Test MonoBehaviour Components

    /// <summary>
    /// Test component that subscribes to EventBus events.
    /// </summary>
    public class TestEventSubscriber : MonoBehaviour
    {
        public int EventsReceived { get; private set; }
        private IEventBus _eventBus;
        private System.IDisposable _subscription;

        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _subscription = _eventBus.Subscribe<CardDrawnEvent>(OnCardDrawn);
        }

        private void OnCardDrawn(CardDrawnEvent e)
        {
            EventsReceived++;
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }

    /// <summary>
    /// Test component that publishes to EventBus.
    /// </summary>
    public class TestEventPublisher : MonoBehaviour
    {
        private IEventBus _eventBus;

        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void PublishPhaseChange(JDG.Domain.Phase phase)
        {
            _eventBus.Publish(new PhaseChangedEvent { NewPhase = phase });
        }
    }

    /// <summary>
    /// Test component that publishes events via coroutine.
    /// </summary>
    public class TestCoroutinePublisher : MonoBehaviour
    {
        private IEventBus _eventBus;

        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void StartPublishing(int count)
        {
            StartCoroutine(PublishEventsCoroutine(count));
        }

        private IEnumerator PublishEventsCoroutine(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _eventBus.Publish(new CardDrawnEvent { Owner = JDG.Domain.CardOwner.Player1 });
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    /// <summary>
    /// Test component that tracks lifecycle method execution.
    /// </summary>
    public class TestLifecycleTracker : MonoBehaviour
    {
        public bool AwakeCalled { get; private set; }
        public bool StartCalled { get; private set; }
        public bool AwakeCalledBeforeStart { get; private set; }

        private void Awake()
        {
            AwakeCalled = true;
        }

        private void Start()
        {
            StartCalled = true;
            AwakeCalledBeforeStart = AwakeCalled;
        }
    }

    /// <summary>
    /// Test component that counts Update calls.
    /// </summary>
    public class TestUpdateCounter : MonoBehaviour
    {
        public int UpdateCount { get; private set; }

        private void Update()
        {
            UpdateCount++;
        }
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using JDG.Application;
using JDG.Domain;
using JDG.Application.Services;
using JDG.Infrastructure.Cards;
using UnityEngine;

namespace JDG.PlayMode.Tests.TestHelpers
{
    #region Timestamped Events

    /// <summary>
    /// Represents an event with timestamp and stack trace for debugging.
    /// </summary>
    public class TimestampedEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public object EventData { get; set; }
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss.fff}] {EventType}";
        }
    }

    #endregion

    #region Test Awaiters

    /// <summary>
    /// Coroutine utilities for waiting in PlayMode tests.
    /// </summary>
    public static class TestAwaiters
    {
        /// <summary>
        /// Waits until a condition is true or timeout is reached.
        /// </summary>
        /// <param name="condition">The condition to wait for.</param>
        /// <param name="timeout">Maximum time to wait in seconds.</param>
        /// <param name="timeoutMessage">Message to include in timeout exception.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public static IEnumerator WaitForCondition(
            Func<bool> condition,
            float timeout = 5f,
            string timeoutMessage = null)
        {
            float elapsed = 0f;
            while (!condition() && elapsed < timeout)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            if (elapsed >= timeout)
            {
                throw new TimeoutException(
                    timeoutMessage ?? $"Condition not met within {timeout}s");
            }
        }

        /// <summary>
        /// Waits for an event of type T to be published to the TestEventBus.
        /// </summary>
        /// <typeparam name="T">The event type to wait for.</typeparam>
        /// <param name="eventBus">The test event bus to monitor.</param>
        /// <param name="timeout">Maximum time to wait in seconds.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public static IEnumerator WaitForEvent<T>(TestEventBus eventBus, float timeout = 5f) where T : struct
        {
            int initialCount = eventBus.CountEvents<T>();
            yield return WaitForCondition(
                () => eventBus.CountEvents<T>() > initialCount,
                timeout,
                $"Event {typeof(T).Name} not published within {timeout}s"
            );
        }

        /// <summary>
        /// Waits for a specific number of frames.
        /// </summary>
        /// <param name="frames">Number of frames to wait.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public static IEnumerator WaitFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Waits for a specific amount of real time (unscaled).
        /// </summary>
        /// <param name="seconds">Seconds to wait.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public static IEnumerator WaitRealTime(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
        }
    }

    #endregion

    /// <summary>
    /// Test double for IEventBus that captures published events.
    /// Use this in PlayMode tests to verify event publishing behavior.
    /// Enhanced with event history tracking for detailed debugging.
    /// </summary>
    public class TestEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        /// <summary>
        /// Detailed event history with timestamps and stack traces.
        /// Use GetEventHistoryReport() for formatted output.
        /// </summary>
        public List<TimestampedEvent> EventHistory { get; } = new List<TimestampedEvent>();

        /// <summary>
        /// Whether to capture stack traces for debugging. Default is false for performance.
        /// </summary>
        public bool CaptureStackTraces { get; set; } = false;

        public void Publish<T>(T eventData) where T : struct
        {
            PublishedEvents.Add(eventData);

            // Also capture to timestamped history
            EventHistory.Add(new TimestampedEvent
            {
                Timestamp = DateTime.Now,
                EventType = typeof(T).Name,
                EventData = eventData,
                StackTrace = CaptureStackTraces ? Environment.StackTrace : null
            });
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
        {
            return new TestDisposable();
        }

        public void ClearSubscriptions<T>() where T : struct { }

        public void ClearAllSubscriptions()
        {
            PublishedEvents.Clear();
            EventHistory.Clear();
        }

        /// <summary>
        /// Gets a formatted report of all published events with timestamps.
        /// Useful for debugging test failures.
        /// </summary>
        /// <returns>Formatted string with event timeline.</returns>
        public string GetEventHistoryReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Event History ===");
            sb.AppendLine($"Total events: {EventHistory.Count}");
            sb.AppendLine();

            foreach (var evt in EventHistory)
            {
                sb.AppendLine($"[{evt.Timestamp:HH:mm:ss.fff}] {evt.EventType}");
                if (CaptureStackTraces && !string.IsNullOrEmpty(evt.StackTrace))
                {
                    sb.AppendLine($"  Stack: {evt.StackTrace.Substring(0, Math.Min(200, evt.StackTrace.Length))}...");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets all events of a specific type from history.
        /// </summary>
        /// <typeparam name="T">The event type to retrieve.</typeparam>
        /// <returns>List of events of the specified type.</returns>
        public List<T> GetAllEvents<T>() where T : struct
        {
            var result = new List<T>();
            foreach (var evt in PublishedEvents)
            {
                if (evt is T typedEvent)
                {
                    result.Add(typedEvent);
                }
            }
            return result;
        }

        /// <summary>
        /// Clears all captured events but keeps subscriptions.
        /// </summary>
        public void ClearEvents()
        {
            PublishedEvents.Clear();
            EventHistory.Clear();
        }

        public T GetLastEvent<T>() where T : struct
        {
            for (int i = PublishedEvents.Count - 1; i >= 0; i--)
            {
                if (PublishedEvents[i] is T evt)
                    return evt;
            }
            throw new InvalidOperationException($"No event of type {typeof(T).Name} was published");
        }

        public bool HasEvent<T>() where T : struct
        {
            foreach (var evt in PublishedEvents)
            {
                if (evt is T)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Alias for HasEvent for more intuitive test code.
        /// </summary>
        public bool HasPublishedEvent<T>() where T : struct => HasEvent<T>();

        public int CountEvents<T>() where T : struct
        {
            int count = 0;
            foreach (var evt in PublishedEvents)
            {
                if (evt is T)
                    count++;
            }
            return count;
        }

        private class TestDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }

    /// <summary>
    /// Test double for ICardCollectionService.
    /// Allows setting up player cards for testing combat and card placement.
    /// </summary>
    public class TestCardCollectionService : ICardCollectionService
    {
        public TestPlayerCards CurrentPlayerCards { get; set; }
        public TestPlayerCards OpponentPlayerCards { get; set; }

        public TestCardCollectionService()
        {
            CurrentPlayerCards = new TestPlayerCards();
            OpponentPlayerCards = new TestPlayerCards();
        }

        public PlayerCards GetCurrentPlayerCards()
        {
            // Return null since PlayerCards is a MonoBehaviour
            // Tests should use the TestPlayerCards properties directly
            return null;
        }

        public PlayerCards GetOpponentPlayerCards()
        {
            // Return null since PlayerCards is a MonoBehaviour
            // Tests should use the TestPlayerCards properties directly
            return null;
        }
    }

    /// <summary>
    /// Simplified test player cards that doesn't require MonoBehaviour.
    /// Used for testing services that need player card collections.
    /// </summary>
    public class TestPlayerCards
    {
        public ObservableCollection<InGameInvocationCard> InvocationCards { get; } = new ObservableCollection<InGameInvocationCard>();
        public ObservableCollection<InGameEffectCard> EffectCards { get; } = new ObservableCollection<InGameEffectCard>();
        public InGameFieldCard FieldCard { get; set; }
        public InGameCard Player { get; set; }
        public List<InGameCard> Deck { get; } = new List<InGameCard>();
        public ObservableCollection<InGameCard> HandCards { get; } = new ObservableCollection<InGameCard>();
        public ObservableCollection<InGameCard> YellowCards { get; } = new ObservableCollection<InGameCard>();

        public bool ContainsCardInInvocation(InGameInvocationCard card)
        {
            foreach (var invocation in InvocationCards)
            {
                if (invocation?.Title == card?.Title)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Test double for IPlayerStatusProvider.
    /// Allows setting up player status for combat testing.
    /// </summary>
    public class TestPlayerStatusProvider : IPlayerStatusProvider
    {
        public TestPlayerStatus CurrentPlayerStatus { get; set; }
        public TestPlayerStatus OpponentPlayerStatus { get; set; }
        public bool HandleAttackCalled { get; private set; }

        public TestPlayerStatusProvider()
        {
            CurrentPlayerStatus = new TestPlayerStatus();
            OpponentPlayerStatus = new TestPlayerStatus();
        }

        public PlayerStatus GetCurrentPlayerStatus()
        {
            // Return null since PlayerStatus is a MonoBehaviour
            // Tests should use the TestPlayerStatus properties directly
            return null;
        }

        public PlayerStatus GetOpponentPlayerStatus()
        {
            // Return null since PlayerStatus is a MonoBehaviour
            return null;
        }

        public void HandleAttackIfOpponentIsPlayer()
        {
            HandleAttackCalled = true;
        }
    }

    /// <summary>
    /// Simplified test player status that doesn't require MonoBehaviour.
    /// </summary>
    public class TestPlayerStatus
    {
        public float CurrentHealth { get; set; } = 30f;
        public int NumberShield { get; set; } = 0;
        public bool BlockAttack { get; set; } = false;

        public void ChangePv(float pv)
        {
            CurrentHealth += pv;
        }

        public float GetCurrentHealth()
        {
            return CurrentHealth;
        }
    }

    /// <summary>
    /// Factory for creating test InGameInvocationCards without Unity dependencies.
    /// </summary>
    public static class TestInGameCardFactory
    {
        /// <summary>
        /// Creates a minimal test InGameInvocationCard.
        /// Note: This requires actual InvocationCard ScriptableObject which is hard to create in tests.
        /// For pure unit tests, prefer using NSubstitute mocks.
        /// </summary>
        public static InGameInvocationCard CreateTestInvocationCard(
            string title,
            float attack,
            float defense,
            CardOwner owner,
            IEventBus eventBus,
            ICardCollectionService cardCollectionService,
            IAbilityProvider abilityProvider = null)
        {
            // Note: This won't work without a real InvocationCard ScriptableObject
            // In practice, tests should either:
            // 1. Use NSubstitute to create mock InGameInvocationCard
            // 2. Load actual cards from Resources in integration tests
            // 3. Create a TestInGameInvocationCard subclass
            throw new NotImplementedException(
                "Creating InGameInvocationCard requires an InvocationCard ScriptableObject. " +
                "Use NSubstitute mocks or load cards from Resources for integration tests.");
        }
    }

    /// <summary>
    /// Extension methods for test assertions.
    /// </summary>
    public static class TestAssertionExtensions
    {
        public static void AssertEventPublished<T>(this TestEventBus eventBus) where T : struct
        {
            if (!eventBus.HasEvent<T>())
            {
                throw new NUnit.Framework.AssertionException(
                    $"Expected event {typeof(T).Name} to be published, but it was not.");
            }
        }

        public static void AssertNoEventPublished<T>(this TestEventBus eventBus) where T : struct
        {
            if (eventBus.HasEvent<T>())
            {
                throw new NUnit.Framework.AssertionException(
                    $"Expected no event {typeof(T).Name} to be published, but it was.");
            }
        }
    }
}

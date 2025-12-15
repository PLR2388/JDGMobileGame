using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using JDG.Application;

namespace JDG.PlayMode.Tests.TestHelpers
{
    /// <summary>
    /// Test double for IEventBus that captures published events.
    /// Use this in PlayMode tests to verify event publishing behavior.
    /// </summary>
    public class TestEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData) where T : struct
        {
            PublishedEvents.Add(eventData);
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
        {
            return new TestDisposable();
        }

        public void ClearSubscriptions<T>() where T : struct { }

        public void ClearAllSubscriptions()
        {
            PublishedEvents.Clear();
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

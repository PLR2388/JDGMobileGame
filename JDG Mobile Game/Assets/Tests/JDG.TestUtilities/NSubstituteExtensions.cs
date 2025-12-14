using System;
using System.Collections.Generic;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using NSubstitute;

namespace JDG.TestUtilities
{
    /// <summary>
    /// Extension methods and factory methods for creating NSubstitute mocks.
    /// Simplifies common mock configurations for JDG project interfaces.
    /// </summary>
    public static class NSubstituteExtensions
    {
        #region IPlayerRepository Mocks

        /// <summary>
        /// Creates a mock IPlayerRepository with optional pre-configured players.
        /// </summary>
        public static IPlayerRepository CreateMockPlayerRepository(
            Player player1 = null,
            Player player2 = null)
        {
            var mock = Substitute.For<IPlayerRepository>();
            var players = new Dictionary<PlayerId, Player>();

            if (player1 != null)
            {
                players[PlayerId.Player1] = player1;
            }
            if (player2 != null)
            {
                players[PlayerId.Player2] = player2;
            }

            mock.GetPlayer(Arg.Any<PlayerId>())
                .Returns(x =>
                {
                    var playerId = x.Arg<PlayerId>();
                    return players.TryGetValue(playerId, out var player) ? player : null;
                });

            mock.When(x => x.SavePlayer(Arg.Any<Player>()))
                .Do(x =>
                {
                    var player = x.Arg<Player>();
                    players[player.Id] = player;
                });

            return mock;
        }

        /// <summary>
        /// Creates a mock IPlayerRepository with both players pre-configured.
        /// </summary>
        public static IPlayerRepository CreateMockPlayerRepositoryWithBothPlayers()
        {
            return CreateMockPlayerRepository(
                PlayerFactory.CreatePlayer1(),
                PlayerFactory.CreatePlayer2()
            );
        }

        #endregion

        #region ICardRepository Mocks

        /// <summary>
        /// Creates a mock ICardRepository with optional pre-loaded cards.
        /// </summary>
        public static ICardRepository CreateMockCardRepository(params Card[] cards)
        {
            var mock = Substitute.For<ICardRepository>();
            var cardDict = new Dictionary<CardId, Card>();

            foreach (var card in cards)
            {
                cardDict[card.Id] = card;
            }

            mock.GetCard(Arg.Any<CardId>())
                .Returns(x =>
                {
                    var cardId = x.Arg<CardId>();
                    return cardDict.TryGetValue(cardId, out var card) ? card : null;
                });

            mock.GetAllCardDefinitions().Returns(cards);

            return mock;
        }

        /// <summary>
        /// Adds a card to an existing mock card repository.
        /// </summary>
        public static void AddCard(this ICardRepository mockRepo, Card card)
        {
            mockRepo.GetCard(card.Id).Returns(card);
        }

        #endregion

        #region IEventBus Mocks

        /// <summary>
        /// Creates a mock IEventBus that tracks all published events.
        /// Returns a TrackingEventBus that implements IEventBus and stores events.
        /// </summary>
        public static (IEventBus mock, PublishedEventsList publishedEvents) CreateMockEventBus()
        {
            var publishedEvents = new PublishedEventsList();
            var mock = new TrackingEventBus(publishedEvents);
            return (mock, publishedEvents);
        }

        /// <summary>
        /// Creates a simple mock IEventBus without event tracking.
        /// </summary>
        public static IEventBus CreateMockEventBusSimple()
        {
            return Substitute.For<IEventBus>();
        }

        #endregion

        #region Common Test Scenarios

        /// <summary>
        /// Creates all mocks needed for a typical use case test.
        /// </summary>
        public static (
            IPlayerRepository playerRepo,
            ICardRepository cardRepo,
            IEventBus eventBus,
            PublishedEventsList publishedEvents,
            Player player1,
            Player player2
        ) CreateUseCaseTestContext()
        {
            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            var playerRepo = CreateMockPlayerRepository(player1, player2);
            var cardRepo = CreateMockCardRepository();
            var (eventBus, publishedEvents) = CreateMockEventBus();

            return (playerRepo, cardRepo, eventBus, publishedEvents, player1, player2);
        }

        /// <summary>
        /// Creates mocks for a combat test scenario.
        /// </summary>
        public static (
            IPlayerRepository playerRepo,
            ICardRepository cardRepo,
            IEventBus eventBus,
            PublishedEventsList publishedEvents,
            Player player1,
            Player player2,
            Card attacker,
            Card defender
        ) CreateCombatTestContext()
        {
            var (player1, player2, attacker, defender) = GameStateFixtures.CreateCombatSetup();
            var playerRepo = CreateMockPlayerRepository(player1, player2);
            var cardRepo = CreateMockCardRepository(attacker, defender);
            var (eventBus, publishedEvents) = CreateMockEventBus();

            return (playerRepo, cardRepo, eventBus, publishedEvents, player1, player2, attacker, defender);
        }

        #endregion

        #region Assertion Helpers

        /// <summary>
        /// Checks if a specific event type was published.
        /// </summary>
        public static bool WasEventPublished<T>(this List<object> events) where T : struct
        {
            return events.Exists(e => e is T);
        }

        /// <summary>
        /// Gets all events of a specific type.
        /// </summary>
        public static List<T> GetEventsOfType<T>(this List<object> events) where T : struct
        {
            var result = new List<T>();
            foreach (var e in events)
            {
                if (e is T typedEvent)
                {
                    result.Add(typedEvent);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the count of events of a specific type.
        /// </summary>
        public static int CountEventsOfType<T>(this List<object> events) where T : struct
        {
            return events.FindAll(e => e is T).Count;
        }

        #endregion
    }

    /// <summary>
    /// A list wrapper for tracking published events with helper methods.
    /// </summary>
    public class PublishedEventsList : List<object>
    {
        /// <summary>
        /// Checks if an event of the specified type was published.
        /// </summary>
        public bool WasEventPublished<T>() where T : struct
        {
            return Exists(e => e is T);
        }

        /// <summary>
        /// Gets all events of the specified type.
        /// </summary>
        public List<T> GetEventsOfType<T>() where T : struct
        {
            var result = new List<T>();
            foreach (var e in this)
            {
                if (e is T typedEvent)
                {
                    result.Add(typedEvent);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// A simple IEventBus implementation that tracks all published events.
    /// </summary>
    public class TrackingEventBus : IEventBus
    {
        private readonly PublishedEventsList _publishedEvents;

        public TrackingEventBus(PublishedEventsList publishedEvents)
        {
            _publishedEvents = publishedEvents;
        }

        public void Publish<T>(T eventData) where T : struct
        {
            _publishedEvents.Add(eventData);
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
        {
            // For testing, we don't need actual subscription functionality
            return new DummyDisposable();
        }

        public void ClearSubscriptions<T>() where T : struct
        {
            // No-op for testing
        }

        public void ClearAllSubscriptions()
        {
            _publishedEvents.Clear();
        }

        private class DummyDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}

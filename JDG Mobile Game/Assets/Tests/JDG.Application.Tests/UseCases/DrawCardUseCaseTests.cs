using NUnit.Framework;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using System.Collections.Generic;

namespace JDG.Application.Tests.UseCases
{
    [TestFixture]
    public class DrawCardUseCaseTests
    {
        private DrawCardUseCase _useCase;
        private TestPlayerRepository _playerRepository;
        private TestEventBus _eventBus;
        private Player _testPlayer;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepository();
            _eventBus = new TestEventBus();
            _useCase = new DrawCardUseCase(_playerRepository, _eventBus);

            // Create test player with a deck
            var deck = new List<Card>();
            for (int i = 0; i < 5; i++)
            {
                deck.Add(Card.CreateInvocation(
                    CardId.New(),
                    $"Test Card {i}",
                    "Test",
                    "Test",
                    3, 3,
                    new[] { CardFamily.Human },
                    false
                ));
            }

            _testPlayer = new Player(PlayerId.Player1, deck);
            _playerRepository.AddPlayer(_testPlayer);
        }

        [Test]
        public void Execute_WithValidPlayer_DrawsCardSuccessfully()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.DrawnCard);
            Assert.AreEqual(4, _testPlayer.DeckCount);
            Assert.AreEqual(1, _testPlayer.HandCount);
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void Execute_WithEmptyDeck_ReturnsFailure()
        {
            // Arrange - draw all cards
            for (int i = 0; i < 5; i++)
            {
                _testPlayer.DrawCard();
            }

            // Act
            var result = _useCase.Execute(PlayerId.Player1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.DrawnCard);
            Assert.AreEqual("Deck is empty", result.Message);
        }

        [Test]
        public void Execute_WithSkipDraw_ReturnsSkipped()
        {
            // Arrange
            _testPlayer.SkipCurrentDraw = true;

            // Act
            var result = _useCase.Execute(PlayerId.Player1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.WasSkipped);
            Assert.IsFalse(_testPlayer.SkipCurrentDraw); // Should be reset
            Assert.AreEqual(5, _testPlayer.DeckCount); // No card drawn
        }

        [Test]
        public void Execute_WithInvalidPlayer_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player2);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Message);
        }
    }

    // Test double for IPlayerRepository
    public class TestPlayerRepository : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();

        public void AddPlayer(Player player) => _players[player.Id] = player;

        public Player GetPlayer(PlayerId playerId) => _players.ContainsKey(playerId) ? _players[playerId] : null;

        public void SavePlayer(Player player) => _players[player.Id] = player;

        public Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30)
        {
            throw new System.NotImplementedException();
        }

        public void ResetPlayer(PlayerId playerId)
        {
            throw new System.NotImplementedException();
        }
    }

    // Test double for IEventBus
    public class TestEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData) where T : struct
        {
            PublishedEvents.Add(eventData);
        }

        public System.IDisposable Subscribe<T>(System.Action<T> handler) where T : struct
        {
            throw new System.NotImplementedException();
        }

        public void ClearSubscriptions<T>() where T : struct
        {
            throw new System.NotImplementedException();
        }

        public void ClearAllSubscriptions()
        {
            PublishedEvents.Clear();
        }
    }
}

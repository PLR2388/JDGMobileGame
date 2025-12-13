using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using System.Collections.Generic;

namespace JDG.Application.Tests.Abilities
{
    [TestFixture]
    public class DrawCardsAbilityTests
    {
        private DrawCardsAbility _ability;
        private DrawCardUseCase _drawCardUseCase;
        private TestPlayerRepositoryForAbility _playerRepository;
        private TestEventBusForAbility _eventBus;
        private Player _testPlayer;
        private AbilityContext _context;
        private Card _sourceCard;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepositoryForAbility();
            _eventBus = new TestEventBusForAbility();
            _drawCardUseCase = new DrawCardUseCase(_playerRepository, _eventBus);

            // Create test player with a deck of 10 cards
            var deck = new List<Card>();
            for (int i = 0; i < 10; i++)
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

            // Create source card and context
            _sourceCard = Card.CreateInvocation(
                CardId.New(),
                "Source Card",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            _context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                _sourceCard,
                AbilityName.Draw2Cards
            );

            _ability = new DrawCardsAbility(2, _drawCardUseCase);
        }

        [Test]
        public void Name_ReturnsCorrectAbilityName()
        {
            // Assert
            Assert.AreEqual(AbilityName.Draw2Cards, _ability.Name);
        }

        [Test]
        public void Description_ContainsCardCount()
        {
            // Assert
            Assert.IsTrue(_ability.Description.Contains("2"));
        }

        [Test]
        public void CanActivate_AlwaysReturnsTrue()
        {
            // Act
            var result = _ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanActivate_ReturnsTrueEvenWithEmptyDeck()
        {
            // Arrange - draw all cards
            for (int i = 0; i < 10; i++)
            {
                _testPlayer.DrawCard();
            }

            // Act
            var result = _ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Execute_DrawsSpecifiedNumberOfCards()
        {
            // Arrange
            var initialDeckCount = _testPlayer.DeckCount;

            // Act
            var result = _ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(initialDeckCount - 2, _testPlayer.DeckCount);
            Assert.AreEqual(2, _testPlayer.HandCount);
        }

        [Test]
        public void Execute_ReturnsSuccessMessage()
        {
            // Act
            var result = _ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("2"));
        }

        [Test]
        public void Execute_WithEmptyDeck_ReturnsFailure()
        {
            // Arrange - draw all cards
            for (int i = 0; i < 10; i++)
            {
                _testPlayer.DrawCard();
            }

            // Act
            var result = _ability.Execute(_context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No cards to draw from deck", result.Message);
        }

        [Test]
        public void Execute_WithPartialDeck_DrawsWhatIsAvailable()
        {
            // Arrange - draw 9 cards, leaving only 1
            for (int i = 0; i < 9; i++)
            {
                _testPlayer.DrawCard();
            }
            var handCountBefore = _testPlayer.HandCount;

            // Act - try to draw 2 cards but only 1 available
            var result = _ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("1")); // Only drew 1 card
            Assert.AreEqual(handCountBefore + 1, _testPlayer.HandCount);
        }

        [Test]
        public void Execute_PublishesDrawEvents()
        {
            // Act
            _ability.Execute(_context);

            // Assert - should publish 2 draw events
            Assert.AreEqual(2, _eventBus.PublishedEvents.Count);
        }
    }

    [TestFixture]
    public class DrawCardsAbilityFactoryTests
    {
        private DrawCardsAbilityFactory _factory;
        private DrawCardUseCase _drawCardUseCase;

        [SetUp]
        public void SetUp()
        {
            var playerRepository = new TestPlayerRepositoryForAbility();
            var eventBus = new TestEventBusForAbility();
            _drawCardUseCase = new DrawCardUseCase(playerRepository, eventBus);
            _factory = new DrawCardsAbilityFactory(_drawCardUseCase);
        }

        [Test]
        public void CreateDraw2Cards_ReturnsAbilityThatDraws2()
        {
            // Act
            var ability = _factory.CreateDraw2Cards();

            // Assert
            Assert.IsNotNull(ability);
            Assert.IsTrue(ability.Description.Contains("2"));
        }

        [Test]
        public void CreateDrawNCards_ReturnsAbilityThatDrawsN()
        {
            // Act
            var ability = _factory.CreateDrawNCards(5);

            // Assert
            Assert.IsNotNull(ability);
            Assert.IsTrue(ability.Description.Contains("5"));
        }
    }

    // Test doubles specific to ability tests
    public class TestPlayerRepositoryForAbility : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();

        public void AddPlayer(Player player) => _players[player.Id] = player;

        public Player GetPlayer(PlayerId playerId) =>
            _players.ContainsKey(playerId) ? _players[playerId] : null;

        public void SavePlayer(Player player) => _players[player.Id] = player;

        public Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30)
        {
            throw new System.NotImplementedException();
        }

        public void ResetPlayer(PlayerId playerId)
        {
            _players.Remove(playerId);
        }
    }

    public class TestEventBusForAbility : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData) where T : struct
        {
            PublishedEvents.Add(eventData);
        }

        public System.IDisposable Subscribe<T>(System.Action<T> handler) where T : struct
        {
            return new TestDisposable();
        }

        public void ClearSubscriptions<T>() where T : struct { }

        public void ClearAllSubscriptions() => PublishedEvents.Clear();

        private class TestDisposable : System.IDisposable
        {
            public void Dispose() { }
        }
    }
}

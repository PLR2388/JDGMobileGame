using NUnit.Framework;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

// Alias to avoid conflict with Cards namespace CardType
using DomainCardType = JDG.Domain.CardType;

namespace JDG.Application.Tests.UseCases
{
    [TestFixture]
    public class PlayCardUseCaseTests
    {
        private PlayCardUseCase _useCase;
        private TestPlayerRepository _playerRepository;
        private TestCardRepository _cardRepository;
        private TestEventBus _eventBus;
        private Player _testPlayer;
        private Card _testCard;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepository();
            _cardRepository = new TestCardRepository();
            _eventBus = new TestEventBus();
            _useCase = new PlayCardUseCase(_playerRepository, _cardRepository, _eventBus);

            // Create test card
            _testCard = Card.CreateInvocation(
                CardId.New(),
                "Test Card",
                "Test Description",
                "Test Image",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            // Create test player with card in hand
            var deck = new List<Card> { _testCard };
            _testPlayer = new Player(PlayerId.Player1, deck);
            _testPlayer.DrawCard(); // Move card to hand

            _playerRepository.AddPlayer(_testPlayer);
            _cardRepository.AddCard(_testCard);
        }

        [Test]
        public void Execute_WithValidCardInHand_PlaysCardSuccessfully()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player1, _testCard.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.PlayedCard);
            Assert.AreEqual(_testCard.Id, result.PlayedCard.Id);
            Assert.AreEqual(0, _testPlayer.HandCount);
            Assert.AreEqual(1, _testPlayer.FieldCount);
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void Execute_WithInvalidPlayer_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player2, _testCard.Id);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Message);
        }

        [Test]
        public void Execute_WithInvalidCard_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player1, CardId.New());

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card not found", result.Message);
        }

        [Test]
        public void Execute_WithCardNotInHand_ReturnsFailure()
        {
            // Arrange - create card not in hand
            var cardNotInHand = Card.CreateInvocation(
                CardId.New(),
                "Other Card",
                "Test",
                "Test",
                2, 2,
                new[] { CardFamily.Human },
                false
            );
            _cardRepository.AddCard(cardNotInHand);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, cardNotInHand.Id);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card is not in hand", result.Message);
        }

        [Test]
        public void Execute_PublishesCardPlayedEvent()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player1, _testCard.Id);

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            var publishedEvent = _eventBus.PublishedEvents[0];
            Assert.IsInstanceOf<JDG.Domain.Events.CardPlayedEvent>(publishedEvent);
        }
    }

    // Test double for ICardRepository
    public class TestCardRepository : ICardRepository
    {
        private readonly Dictionary<CardId, Card> _cards = new Dictionary<CardId, Card>();

        public void AddCard(Card card) => _cards[card.Id] = card;

        public Card GetCard(CardId cardId) => _cards.ContainsKey(cardId) ? _cards[cardId] : null;

        public Card CreateCardInstance(string cardDefinitionName)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Card> GetAllCardDefinitions() => _cards.Values;

        public IEnumerable<Card> GetCardsByType(DomainCardType type) => _cards.Values.Where(c => c.Type == type);

        public IEnumerable<Card> GetCardsByFamily(CardFamily family) =>
            _cards.Values.Where(c => c.Families != null && c.Families.Contains(family));

        public Card GetCardByTitle(string title) => _cards.Values.FirstOrDefault(c => c.Title == title);
    }
}

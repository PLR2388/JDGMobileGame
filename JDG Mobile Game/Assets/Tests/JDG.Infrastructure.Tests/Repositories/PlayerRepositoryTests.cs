using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Repositories;

namespace JDG.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class PlayerRepositoryTests
    {
        private PlayerRepository _repository;
        private TestCardRepository _cardRepository;

        [SetUp]
        public void SetUp()
        {
            _cardRepository = new TestCardRepository();
            _repository = new PlayerRepository(_cardRepository);
        }

        [Test]
        public void GetPlayer_ReturnsNullForNonexistentPlayer()
        {
            // Act
            var player = _repository.GetPlayer(PlayerId.Player1);

            // Assert
            Assert.IsNull(player);
        }

        [Test]
        public void SavePlayer_StoresPlayer()
        {
            // Arrange
            var player = new Player(PlayerId.Player1, new List<Card>());

            // Act
            _repository.SavePlayer(player);
            var retrieved = _repository.GetPlayer(PlayerId.Player1);

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(PlayerId.Player1, retrieved.Id);
        }

        [Test]
        public void CreatePlayer_CreatesPlayerWithDeck()
        {
            // Arrange
            var card1 = Card.CreateInvocation(CardId.New(), "Card1", "", "", 3, 3, new[] { CardFamily.Human }, false);
            var card2 = Card.CreateInvocation(CardId.New(), "Card2", "", "", 3, 3, new[] { CardFamily.Human }, false);

            _cardRepository.RegisterCard(card1);
            _cardRepository.RegisterCard(card2);

            var deckCardIds = new[] { card1.Id, card2.Id };

            // Act
            var player = _repository.CreatePlayer(PlayerId.Player1, deckCardIds);

            // Assert
            Assert.IsNotNull(player);
            Assert.AreEqual(PlayerId.Player1, player.Id);
            Assert.AreEqual(2, player.DeckCount);
        }

        [Test]
        public void ResetPlayer_RemovesPlayer()
        {
            // Arrange
            var player = new Player(PlayerId.Player1, new List<Card>());
            _repository.SavePlayer(player);

            // Act
            _repository.ResetPlayer(PlayerId.Player1);
            var retrieved = _repository.GetPlayer(PlayerId.Player1);

            // Assert
            Assert.IsNull(retrieved);
        }
    }

    // Test double for ICardRepository
    public class TestCardRepository : JDG.Application.Repositories.ICardRepository
    {
        private readonly Dictionary<CardId, Card> _cards = new Dictionary<CardId, Card>();

        public void RegisterCard(Card card)
        {
            _cards[card.Id] = card;
        }

        public Card GetCard(CardId cardId) => _cards.ContainsKey(cardId) ? _cards[cardId] : null;

        public Card CreateCardInstance(string cardDefinitionName) => null;
        public IEnumerable<Card> GetAllCardDefinitions() => _cards.Values;
        public IEnumerable<Card> GetCardsByType(CardType type) => Enumerable.Empty<Card>();
        public IEnumerable<Card> GetCardsByFamily(CardFamily family) => Enumerable.Empty<Card>();
        public Card GetCardByTitle(string title) => null;
    }
}

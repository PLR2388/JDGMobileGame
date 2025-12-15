using NUnit.Framework;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Application;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JDG.Application.Tests.UseCases
{
    [TestFixture]
    public class AttackUseCaseTests
    {
        private AttackUseCase _useCase;
        private TestPlayerRepository _playerRepository;
        private TestCardRepository _cardRepository;
        private TestEventBus _eventBus;
        private Player _player1;
        private Player _player2;
        private Card _attackerCard;
        private Card _defenderCard;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepository();
            _cardRepository = new TestCardRepository();
            _eventBus = new TestEventBus();
            _useCase = new AttackUseCase(_playerRepository, _cardRepository, _eventBus);

            // Create attacker card (ATK: 5, DEF: 3)
            _attackerCard = Card.CreateInvocation(
                CardId.New(),
                "Attacker Card",
                "Test Attacker",
                "Test",
                5, 3,
                new[] { CardFamily.Human },
                false
            );

            // Create defender card (ATK: 2, DEF: 4)
            _defenderCard = Card.CreateInvocation(
                CardId.New(),
                "Defender Card",
                "Test Defender",
                "Test",
                2, 4,
                new[] { CardFamily.Human },
                false
            );

            // Create players with cards
            var player1Deck = new List<Card> { _attackerCard };
            var player2Deck = new List<Card> { _defenderCard };

            _player1 = new Player(PlayerId.Player1, player1Deck);
            _player2 = new Player(PlayerId.Player2, player2Deck);

            // Draw cards and play them to field
            _player1.DrawCard();
            _player2.DrawCard();
            _player1.PlayCard(_attackerCard);
            _player2.PlayCard(_defenderCard);

            _playerRepository.AddPlayer(_player1);
            _playerRepository.AddPlayer(_player2);
            _cardRepository.AddCard(_attackerCard);
            _cardRepository.AddCard(_defenderCard);
        }

        [Test]
        public void Execute_WithValidAttack_ExecutesSuccessfully()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player1, _attackerCard.Id, _defenderCard.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(5, result.Damage); // Attacker's ATK
            Assert.AreEqual("Attack executed successfully", result.Message);
        }

        [Test]
        public void Execute_WithValidAttack_PublishesEvent()
        {
            // Act
            _useCase.Execute(PlayerId.Player1, _attackerCard.Id, _defenderCard.Id);

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            var attackEvent = _eventBus.PublishedEvents[0];
            Assert.IsInstanceOf<AttackExecutedEvent>(attackEvent);
        }

        [Test]
        public void Execute_DefenderDestroyedWhenDefenseLessThanAttack()
        {
            // Arrange - Attacker has 5 ATK, Defender has 4 DEF
            // After attack, defender DEF = 4 - 5 = -1 (destroyed)

            // Act
            var result = _useCase.Execute(PlayerId.Player1, _attackerCard.Id, _defenderCard.Id);

            // Assert
            Assert.IsTrue(result.DefenderDestroyed);
        }

        [Test]
        public void Execute_AttackerDestroyedByCounterAttack()
        {
            // Arrange - Create strong defender that can counter-destroy attacker
            var strongDefender = Card.CreateInvocation(
                CardId.New(),
                "Strong Defender",
                "Test",
                "Test",
                10, 10, // ATK: 10, DEF: 10
                new[] { CardFamily.Human },
                false
            );

            _player2 = new Player(PlayerId.Player2, new List<Card> { strongDefender });
            _player2.DrawCard();
            _player2.PlayCard(strongDefender);
            _playerRepository.AddPlayer(_player2);
            _cardRepository.AddCard(strongDefender);

            // Act - Attacker has DEF: 3, Strong defender has ATK: 10
            // Counter attack: 3 - 10 = -7 (destroyed)
            var result = _useCase.Execute(PlayerId.Player1, _attackerCard.Id, strongDefender.Id);

            // Assert
            Assert.IsTrue(result.AttackerDestroyed);
        }

        [Test]
        public void Execute_WithInvalidAttacker_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player1, CardId.New(), _defenderCard.Id);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card not found", result.Message);
        }

        [Test]
        public void Execute_WithInvalidDefender_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(PlayerId.Player1, _attackerCard.Id, CardId.New());

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card not found", result.Message);
        }

        [Test]
        public void Execute_WithInvalidPlayer_ReturnsFailure()
        {
            // Arrange
            _playerRepository = new TestPlayerRepository(); // Empty repository
            _useCase = new AttackUseCase(_playerRepository, _cardRepository, _eventBus);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, _attackerCard.Id, _defenderCard.Id);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Message);
        }

        [Test]
        public void Execute_WithBlockedAttack_ReturnsFailure()
        {
            // Arrange
            _player1.SetBlockAttack(true);
            _playerRepository.SavePlayer(_player1);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, _attackerCard.Id, _defenderCard.Id);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Attack is blocked", result.Message);
        }

        [Test]
        public void Execute_WithNonInvocationCard_ReturnsFailure()
        {
            // Arrange
            var effectCard = Card.CreateEffect(
                CardId.New(),
                "Effect Card",
                "Test",
                "Test",
                new EffectAbilityName[0]
            );
            _cardRepository.AddCard(effectCard);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, effectCard.Id, _defenderCard.Id);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Only invocation cards can battle", result.Message);
        }

        [Test]
        public void ExecuteDirectAttack_WithValidAttack_DamagesPlayer()
        {
            // Arrange
            var initialHealth = _player2.Health;

            // Act
            var result = _useCase.ExecuteDirectAttack(PlayerId.Player1, _attackerCard.Id, PlayerId.Player2);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.IsDirectAttack);
            Assert.AreEqual(5, result.Damage); // Attacker's ATK
        }

        [Test]
        public void ExecuteDirectAttack_PublishesPlayerDamagedEvent()
        {
            // Act
            _useCase.ExecuteDirectAttack(PlayerId.Player1, _attackerCard.Id, PlayerId.Player2);

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            var damageEvent = _eventBus.PublishedEvents[0];
            Assert.IsInstanceOf<PlayerDamagedEvent>(damageEvent);
        }

        [Test]
        public void ExecuteDirectAttack_WithInvalidCard_ReturnsFailure()
        {
            // Act
            var result = _useCase.ExecuteDirectAttack(PlayerId.Player1, CardId.New(), PlayerId.Player2);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid attacker card", result.Message);
        }

        [Test]
        public void ExecuteDirectAttack_WithEffectCard_ReturnsFailure()
        {
            // Arrange
            var effectCard = Card.CreateEffect(
                CardId.New(),
                "Effect Card",
                "Test",
                "Test",
                new EffectAbilityName[0]
            );
            _cardRepository.AddCard(effectCard);

            // Act
            var result = _useCase.ExecuteDirectAttack(PlayerId.Player1, effectCard.Id, PlayerId.Player2);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid attacker card", result.Message);
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
            throw new NotImplementedException();
        }

        public void ResetPlayer(PlayerId playerId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetAllCardDefinitions() => _cards.Values;

        public IEnumerable<Card> GetCardsByType(CardType type) => _cards.Values.Where(c => c.Type == type);

        public IEnumerable<Card> GetCardsByFamily(CardFamily family) =>
            _cards.Values.Where(c => c.Families != null && c.Families.Contains(family));

        public Card GetCardByTitle(string title) => _cards.Values.FirstOrDefault(c => c.Title == title);
    }

    // Test double for IEventBus
    public class TestEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData) where T : struct
        {
            PublishedEvents.Add(eventData);
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
        {
            return new DummyDisposable();
        }

        public void ClearSubscriptions<T>() where T : struct { }

        public void ClearAllSubscriptions() { }

        private class DummyDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}

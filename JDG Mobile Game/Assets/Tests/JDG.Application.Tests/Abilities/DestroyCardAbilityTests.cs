using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using System.Collections.Generic;
using System.Linq;

namespace JDG.Application.Tests.Abilities
{
    [TestFixture]
    public class DestroyCardAbilityTests
    {
        private DestroyCardAbility _ability;
        private TestPlayerRepositoryForDestroyAbility _playerRepository;
        private TestEventBusForAbility _eventBus;
        private Player _player1;
        private Player _player2;
        private Card _sourceCard;
        private Card _targetInvocation;
        private AbilityContext _context;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepositoryForDestroyAbility();
            _eventBus = new TestEventBusForAbility();

            // Create target invocation on opponent's field
            _targetInvocation = Card.CreateInvocation(
                CardId.New(),
                "Target Invocation",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            // Create players
            _player1 = new Player(PlayerId.Player1, new List<Card>());
            _player2 = new Player(PlayerId.Player2, new List<Card> { _targetInvocation });
            _player2.DrawCard();
            _player2.PlayCard(_targetInvocation);

            _playerRepository.AddPlayer(_player1);
            _playerRepository.AddPlayer(_player2);

            // Create source card and context
            _sourceCard = Card.CreateInvocation(
                CardId.New(),
                "Source Card",
                "Test",
                "Test",
                5, 5,
                new[] { CardFamily.Human },
                false
            );

            _context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                _sourceCard,
                AbilityName.KillOpponentInvocation
            );

            _ability = new DestroyCardAbility(
                AbilityName.KillOpponentInvocation,
                CardType.Invocation,
                _playerRepository,
                _eventBus
            );
        }

        [Test]
        public void Name_ReturnsCorrectAbilityName()
        {
            // Assert
            Assert.AreEqual(AbilityName.KillOpponentInvocation, _ability.Name);
        }

        [Test]
        public void Description_ContainsTargetType()
        {
            // Assert
            Assert.IsTrue(_ability.Description.Contains("Invocation"));
        }

        [Test]
        public void CanActivate_WithValidTarget_ReturnsTrue()
        {
            // Act
            var result = _ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanActivate_WithNoTargets_ReturnsFalse()
        {
            // Arrange - clear opponent's field
            _player2.DestroyCardFromField(_targetInvocation);
            _playerRepository.SavePlayer(_player2);

            // Act
            var result = _ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CanActivate_WithWrongCardType_ReturnsFalse()
        {
            // Arrange - create ability that targets field cards
            var fieldAbility = new DestroyCardAbility(
                AbilityName.DestroyFieldATK,
                CardType.Field,
                _playerRepository,
                _eventBus
            );

            // Act - opponent has invocation, not field card
            var result = fieldAbility.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Execute_DestroysTargetCard()
        {
            // Arrange
            Assert.AreEqual(1, _player2.FieldCount);

            // Act
            var result = _ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, _player2.FieldCount);
        }

        [Test]
        public void Execute_ReturnsSuccessWithCardName()
        {
            // Act
            var result = _ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains(_targetInvocation.Title));
        }

        [Test]
        public void Execute_PublishesCardDestroyedEvent()
        {
            // Act
            _ability.Execute(_context);

            // Assert
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count);
            var destroyEvent = _eventBus.PublishedEvents[0];
            Assert.IsInstanceOf<CardDestroyedEvent>(destroyEvent);
        }

        [Test]
        public void Execute_PublishesEventWithCorrectDetails()
        {
            // Act
            _ability.Execute(_context);

            // Assert
            var destroyEvent = (CardDestroyedEvent)_eventBus.PublishedEvents[0];
            Assert.AreEqual(_targetInvocation.Id.ToGuid(), destroyEvent.CardId);
            Assert.AreEqual(PlayerId.Player2.ToCardOwner(), destroyEvent.Owner);
        }

        [Test]
        public void Execute_WithNoOpponent_ReturnsFailure()
        {
            // Arrange
            _playerRepository = new TestPlayerRepositoryForDestroyAbility();
            _playerRepository.AddPlayer(_player1);
            // Don't add player 2
            var ability = new DestroyCardAbility(
                AbilityName.KillOpponentInvocation,
                CardType.Invocation,
                _playerRepository,
                _eventBus
            );

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Opponent not found", result.Message);
        }

        [Test]
        public void Execute_WithNoValidTargets_ReturnsFailure()
        {
            // Arrange - clear opponent's field
            _player2.DestroyCardFromField(_targetInvocation);
            _playerRepository.SavePlayer(_player2);

            // Act
            var result = _ability.Execute(_context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("No"));
        }

        [Test]
        public void Execute_SavesOpponentAfterDestruction()
        {
            // Arrange
            _playerRepository.SaveCount = 0;

            // Act
            _ability.Execute(_context);

            // Assert
            Assert.AreEqual(1, _playerRepository.SaveCount);
        }
    }

    [TestFixture]
    public class DestroyCardAbilityFactoryTests
    {
        private DestroyCardAbilityFactory _factory;

        [SetUp]
        public void SetUp()
        {
            var playerRepository = new TestPlayerRepositoryForAbility();
            var eventBus = new TestEventBusForAbility();
            _factory = new DestroyCardAbilityFactory(playerRepository, eventBus);
        }

        [Test]
        public void CreateKillOpponentInvocation_ReturnsCorrectAbility()
        {
            // Act
            var ability = _factory.CreateKillOpponentInvocation();

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.KillOpponentInvocation, ability.Name);
        }

        [Test]
        public void CreateDestroyField_ReturnsCorrectAbility()
        {
            // Act
            var ability = _factory.CreateDestroyField();

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.DestroyFieldATK, ability.Name);
        }
    }

    // Test player repository with save count tracking for destroy tests
    public class TestPlayerRepositoryForDestroyAbility : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();
        public int SaveCount { get; set; }

        public void AddPlayer(Player player) => _players[player.Id] = player;

        public Player GetPlayer(PlayerId playerId) =>
            _players.ContainsKey(playerId) ? _players[playerId] : null;

        public void SavePlayer(Player player)
        {
            _players[player.Id] = player;
            SaveCount++;
        }

        public Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30)
        {
            throw new System.NotImplementedException();
        }

        public void ResetPlayer(PlayerId playerId)
        {
            _players.Remove(playerId);
        }
    }
}

using NUnit.Framework;
using NSubstitute;
using JDG.Application;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using JDG.TestUtilities;
using System.Collections.Generic;

namespace JDG.Application.Tests.UseCases
{
    /// <summary>
    /// AttackUseCase tests using NSubstitute.
    /// Demonstrates cleaner test patterns with mocking framework.
    /// </summary>
    [TestFixture]
    public class AttackUseCaseNSubstituteTests
    {
        private AttackUseCase _useCase;
        private IPlayerRepository _playerRepository;
        private ICardRepository _cardRepository;
        private IEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _cardRepository = Substitute.For<ICardRepository>();
            _eventBus = Substitute.For<IEventBus>();
            _useCase = new AttackUseCase(_playerRepository, _cardRepository, _eventBus);
        }

        [Test]
        public void Execute_WithValidAttack_PublishesAttackEvent()
        {
            // Arrange
            var attacker = CardFactory.CreateAttacker(attack: 5, defense: 3);
            var defender = CardFactory.CreateDefender(attack: 2, defense: 4);

            var player1 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player1, new List<Card> { attacker });
            var player2 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player2, new List<Card> { defender });

            // Move cards to field
            player1.DrawCard();
            player2.DrawCard();
            player1.PlayCard(attacker);
            player2.PlayCard(defender);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(player2);
            _cardRepository.GetCard(attacker.Id).Returns(attacker);
            _cardRepository.GetCard(defender.Id).Returns(defender);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, attacker.Id, defender.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _eventBus.Received(1).Publish(Arg.Any<AttackExecutedEvent>());
        }

        [Test]
        public void Execute_WhenDefenderDestroyed_ReturnsDestroyedFlag()
        {
            // Arrange - Attacker ATK(5) > Defender DEF(3) => destroyed
            var attacker = CardFactory.CreateInvocation("Attacker", attack: 5, defense: 3);
            var defender = CardFactory.CreateInvocation("Defender", attack: 2, defense: 3);

            var player1 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player1, new List<Card> { attacker });
            var player2 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player2, new List<Card> { defender });

            player1.DrawCard();
            player2.DrawCard();
            player1.PlayCard(attacker);
            player2.PlayCard(defender);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(player2);
            _cardRepository.GetCard(attacker.Id).Returns(attacker);
            _cardRepository.GetCard(defender.Id).Returns(defender);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, attacker.Id, defender.Id);

            // Assert
            Assert.IsTrue(result.DefenderDestroyed);
        }

        [Test]
        public void Execute_WithInvalidAttacker_DoesNotPublishEvent()
        {
            // Arrange
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(PlayerFactory.CreatePlayer1());
            _cardRepository.GetCard(Arg.Any<CardId>()).Returns((Card)null);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, CardId.New(), CardId.New());

            // Assert
            Assert.IsFalse(result.IsSuccess);
            _eventBus.DidNotReceive().Publish(Arg.Any<AttackExecutedEvent>());
        }

        [Test]
        public void Execute_WithBlockedPlayer_ReturnsFailure()
        {
            // Arrange
            var attacker = CardFactory.CreateAttacker();
            var defender = CardFactory.CreateDefender();

            var player1 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player1, new List<Card> { attacker });
            player1.DrawCard();
            player1.PlayCard(attacker);
            player1.SetBlockAttack(true); // Block attacks

            var player2 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player2, new List<Card> { defender });
            player2.DrawCard();
            player2.PlayCard(defender);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(player2);
            _cardRepository.GetCard(attacker.Id).Returns(attacker);
            _cardRepository.GetCard(defender.Id).Returns(defender);

            // Act
            var result = _useCase.Execute(PlayerId.Player1, attacker.Id, defender.Id);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Attack is blocked", result.Message);
        }

        [Test]
        public void ExecuteDirectAttack_WithEmptyOpponentField_DamagesPlayer()
        {
            // Arrange
            var attacker = CardFactory.CreateAttacker(attack: 5);

            var player1 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player1, new List<Card> { attacker });
            player1.DrawCard();
            player1.PlayCard(attacker);

            var player2 = PlayerFactory.CreatePlayer2(); // No cards on field

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(player2);
            _cardRepository.GetCard(attacker.Id).Returns(attacker);

            // Act
            var result = _useCase.ExecuteDirectAttack(PlayerId.Player1, attacker.Id, PlayerId.Player2);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.IsDirectAttack);
            Assert.AreEqual(5, result.Damage);
        }

        [Test]
        public void ExecuteDirectAttack_PublishesPlayerDamagedEvent()
        {
            // Arrange
            var attacker = CardFactory.CreateAttacker(attack: 3);

            var player1 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player1, new List<Card> { attacker });
            player1.DrawCard();
            player1.PlayCard(attacker);

            var player2 = PlayerFactory.CreatePlayer2();

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(player2);
            _cardRepository.GetCard(attacker.Id).Returns(attacker);

            // Act
            _useCase.ExecuteDirectAttack(PlayerId.Player1, attacker.Id, PlayerId.Player2);

            // Assert
            _eventBus.Received(1).Publish(Arg.Any<PlayerDamagedEvent>());
        }

        [Test]
        public void Execute_SavesPlayerStateAfterAttack()
        {
            // Arrange
            var attacker = CardFactory.CreateAttacker();
            var defender = CardFactory.CreateDefender();

            var player1 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player1, new List<Card> { attacker });
            var player2 = PlayerFactory.CreatePlayerWithCards(PlayerId.Player2, new List<Card> { defender });

            player1.DrawCard();
            player2.DrawCard();
            player1.PlayCard(attacker);
            player2.PlayCard(defender);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(player2);
            _cardRepository.GetCard(attacker.Id).Returns(attacker);
            _cardRepository.GetCard(defender.Id).Returns(defender);

            // Act
            _useCase.Execute(PlayerId.Player1, attacker.Id, defender.Id);

            // Assert - Both players should be saved
            _playerRepository.Received().SavePlayer(player1);
            _playerRepository.Received().SavePlayer(player2);
        }
    }
}

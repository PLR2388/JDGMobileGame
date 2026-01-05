using NUnit.Framework;
using NSubstitute;
using JDG.Application;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Events;
using JDG.TestUtilities;
using TestCardFactory = JDG.TestUtilities.CardFactory;
using System.Collections.Generic;
using System.Linq;

namespace JDG.Application.Tests.UseCases
{
    /// <summary>
    /// DrawCardUseCase tests using NSubstitute.
    /// Demonstrates cleaner test patterns with mocking framework.
    /// </summary>
    [TestFixture]
    public class DrawCardUseCaseNSubstituteTests
    {
        private DrawCardUseCase _useCase;
        private IPlayerRepository _playerRepository;
        private IEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _eventBus = Substitute.For<IEventBus>();
            _useCase = new DrawCardUseCase(_playerRepository, _eventBus);
        }

        [Test]
        public void Execute_WithCardsInDeck_DrawsCardSuccessfully()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1(deckSize: 30);
            var initialDeckCount = player.Deck.Count;
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player);

            // Act
            var result = _useCase.Execute(PlayerId.Player1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.DrawnCard);
            Assert.AreEqual(initialDeckCount - 1, player.Deck.Count);
            Assert.AreEqual(1, player.HandCount);
        }

        [Test]
        public void Execute_PublishesCardDrawnEvent()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1(deckSize: 10);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player);

            // Act
            _useCase.Execute(PlayerId.Player1);

            // Assert
            _eventBus.Received(1).Publish(Arg.Any<CardDrawnEvent>());
        }

        [Test]
        public void Execute_SavesPlayerAfterDraw()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1();
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player);

            // Act
            _useCase.Execute(PlayerId.Player1);

            // Assert
            _playerRepository.Received(1).SavePlayer(player);
        }

        [Test]
        public void Execute_WithEmptyDeck_ReturnsFailure()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1(deckSize: 0);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player);

            // Act
            var result = _useCase.Execute(PlayerId.Player1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Deck is empty", result.Message);
        }

        [Test]
        public void Execute_WithInvalidPlayer_ReturnsFailure()
        {
            // Arrange
            _playerRepository.GetPlayer(PlayerId.Player1).Returns((Player)null);

            // Act
            var result = _useCase.Execute(PlayerId.Player1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player not found", result.Message);
        }

        [Test]
        public void Execute_WithInvalidPlayer_DoesNotPublishEvent()
        {
            // Arrange
            _playerRepository.GetPlayer(Arg.Any<PlayerId>()).Returns((Player)null);

            // Act
            _useCase.Execute(PlayerId.Player1);

            // Assert
            _eventBus.DidNotReceive().Publish(Arg.Any<CardDrawnEvent>());
        }

        [Test]
        public void Execute_DrawnCardAddedToHand()
        {
            // Arrange
            var deck = TestCardFactory.CreateDeck(5);
            var player = PlayerFactory.CreatePlayerWithCards(PlayerId.Player1, deck);
            // Player.DrawCard() draws from END of deck (last element), not beginning
            var expectedCard = player.Deck[^1];
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player);

            // Act
            var result = _useCase.Execute(PlayerId.Player1);

            // Assert
            Assert.AreEqual(expectedCard, result.DrawnCard);
            Assert.IsTrue(player.Hand.Contains(expectedCard));
        }

        [Test]
        public void Execute_MultipleDraws_DrawsSequentially()
        {
            // Arrange
            var player = PlayerFactory.CreatePlayer1(deckSize: 30);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(player);

            // Act - Draw 5 cards
            for (int i = 0; i < 5; i++)
            {
                _useCase.Execute(PlayerId.Player1);
            }

            // Assert
            Assert.AreEqual(25, player.Deck.Count);
            Assert.AreEqual(5, player.HandCount);
            _eventBus.Received(5).Publish(Arg.Any<CardDrawnEvent>());
        }
    }
}

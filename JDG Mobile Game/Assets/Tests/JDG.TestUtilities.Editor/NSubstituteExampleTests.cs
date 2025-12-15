using NUnit.Framework;
using NSubstitute;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Events;
using System.Collections.Generic;

namespace JDG.TestUtilities
{
    /// <summary>
    /// Example tests demonstrating NSubstitute usage patterns.
    /// Use these as templates for writing new tests.
    /// </summary>
    [TestFixture]
    public class NSubstituteExampleTests
    {
        #region Basic Mock Creation

        [Test]
        public void Example_CreateSimpleMock()
        {
            // Arrange - Create a simple mock
            var playerRepo = Substitute.For<IPlayerRepository>();

            // Configure return value
            var player = PlayerFactory.CreatePlayer1();
            playerRepo.GetPlayer(PlayerId.Player1).Returns(player);

            // Act
            var result = playerRepo.GetPlayer(PlayerId.Player1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(PlayerId.Player1, result.Id);
        }

        [Test]
        public void Example_VerifyMethodWasCalled()
        {
            // Arrange
            var playerRepo = Substitute.For<IPlayerRepository>();
            var player = PlayerFactory.CreatePlayer1();

            // Act
            playerRepo.SavePlayer(player);

            // Assert - Verify SavePlayer was called with the player
            playerRepo.Received(1).SavePlayer(player);
        }

        [Test]
        public void Example_VerifyMethodWasNotCalled()
        {
            // Arrange
            var playerRepo = Substitute.For<IPlayerRepository>();

            // Act - Don't call anything

            // Assert - Verify SavePlayer was never called
            playerRepo.DidNotReceive().SavePlayer(Arg.Any<Player>());
        }

        #endregion

        #region Using Argument Matchers

        [Test]
        public void Example_UseArgumentMatchers()
        {
            // Arrange
            var playerRepo = Substitute.For<IPlayerRepository>();
            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();

            // Configure different returns for different arguments
            playerRepo.GetPlayer(PlayerId.Player1).Returns(player1);
            playerRepo.GetPlayer(PlayerId.Player2).Returns(player2);

            // Act & Assert
            Assert.AreEqual(player1, playerRepo.GetPlayer(PlayerId.Player1));
            Assert.AreEqual(player2, playerRepo.GetPlayer(PlayerId.Player2));
        }

        [Test]
        public void Example_UseAnyArgument()
        {
            // Arrange
            var cardRepo = Substitute.For<ICardRepository>();
            var card = CardFactory.CreateInvocation("Any Card");

            // Return the same card for any CardId
            cardRepo.GetCard(Arg.Any<CardId>()).Returns(card);

            // Act
            var result1 = cardRepo.GetCard(CardId.New());
            var result2 = cardRepo.GetCard(CardId.New());

            // Assert - Both return the same card
            Assert.AreEqual(card, result1);
            Assert.AreEqual(card, result2);
        }

        #endregion

        #region Using Test Utilities

        [Test]
        public void Example_UseTestFixtures()
        {
            // Arrange - Use CardFactory for quick card creation
            var attacker = CardFactory.CreateAttacker(attack: 5, defense: 2);
            var defender = CardFactory.CreateDefender(attack: 2, defense: 5);

            // Assert - Stats is nullable, use .Value to access
            Assert.AreEqual(5, attacker.Stats.Value.Attack);
            Assert.AreEqual(5, defender.Stats.Value.Defense);
        }

        [Test]
        public void Example_UseGameStateFixtures()
        {
            // Arrange - Use pre-configured game states
            var (player1, player2) = GameStateFixtures.CreateBasicGameSetup();

            // Assert
            Assert.IsNotNull(player1);
            Assert.IsNotNull(player2);
            Assert.AreEqual(30, player1.Deck.Count);
            Assert.AreEqual(30, player2.Deck.Count);
        }

        [Test]
        public void Example_UseCombatFixture()
        {
            // Arrange - Use combat setup fixture
            var (player1, player2, attacker, defender) = GameStateFixtures.CreateCombatSetup();

            // Assert - Players have cards on field
            Assert.AreEqual(1, player1.Field.Count);
            Assert.AreEqual(1, player2.Field.Count);
            Assert.AreEqual(attacker, player1.Field[0]);
            Assert.AreEqual(defender, player2.Field[0]);
        }

        #endregion

        #region Using NSubstitute Extensions

        [Test]
        public void Example_UseMockPlayerRepository()
        {
            // Arrange - Use extension to create pre-configured mock
            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            var playerRepo = NSubstituteExtensions.CreateMockPlayerRepository(player1, player2);

            // Act
            var retrieved1 = playerRepo.GetPlayer(PlayerId.Player1);
            var retrieved2 = playerRepo.GetPlayer(PlayerId.Player2);

            // Assert
            Assert.AreEqual(player1, retrieved1);
            Assert.AreEqual(player2, retrieved2);
        }

        [Test]
        public void Example_UseMockEventBusWithTracking()
        {
            // Arrange - Create event bus that tracks published events
            var (eventBus, publishedEvents) = NSubstituteExtensions.CreateMockEventBus();

            // Act - Publish an event (CardDrawnEvent uses Owner not PlayerId)
            eventBus.Publish(new CardDrawnEvent { Owner = CardOwner.Player1 });

            // Assert - Check event was tracked
            Assert.AreEqual(1, publishedEvents.Count);
            Assert.IsTrue(publishedEvents.WasEventPublished<CardDrawnEvent>());
        }

        [Test]
        public void Example_UseUseCaseTestContext()
        {
            // Arrange - Create all mocks for a use case test
            var (playerRepo, cardRepo, eventBus, publishedEvents, player1, player2) =
                NSubstituteExtensions.CreateUseCaseTestContext();

            // Assert - All components are properly configured
            Assert.IsNotNull(playerRepo);
            Assert.IsNotNull(cardRepo);
            Assert.IsNotNull(eventBus);
            Assert.IsNotNull(publishedEvents);
            Assert.AreEqual(player1, playerRepo.GetPlayer(PlayerId.Player1));
            Assert.AreEqual(player2, playerRepo.GetPlayer(PlayerId.Player2));
        }

        #endregion

        #region Callbacks and Actions

        [Test]
        public void Example_UseCallbacksForSideEffects()
        {
            // Arrange
            var playerRepo = Substitute.For<IPlayerRepository>();
            var savedPlayers = new List<Player>();

            // Configure callback to track saved players
            playerRepo.When(x => x.SavePlayer(Arg.Any<Player>()))
                .Do(x => savedPlayers.Add(x.Arg<Player>()));

            var player = PlayerFactory.CreatePlayer1();

            // Act
            playerRepo.SavePlayer(player);
            playerRepo.SavePlayer(player);

            // Assert
            Assert.AreEqual(2, savedPlayers.Count);
        }

        [Test]
        public void Example_ReturnDifferentValuesOnSequentialCalls()
        {
            // Arrange
            var cardRepo = Substitute.For<ICardRepository>();
            var card1 = CardFactory.CreateInvocation("First Card");
            var card2 = CardFactory.CreateInvocation("Second Card");

            // Return different cards on sequential calls
            cardRepo.GetCard(Arg.Any<CardId>()).Returns(card1, card2, (Card)null);

            // Act & Assert
            Assert.AreEqual(card1, cardRepo.GetCard(CardId.New()));
            Assert.AreEqual(card2, cardRepo.GetCard(CardId.New()));
            Assert.IsNull(cardRepo.GetCard(CardId.New()));
        }

        #endregion
    }
}

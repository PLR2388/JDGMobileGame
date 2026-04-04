using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;

namespace JDG.Domain.Tests.Entities
{
    [TestFixture]
    public class PlayerTests
    {
        private Player _player;
        private List<Card> _testDeck;

        [SetUp]
        public void SetUp()
        {
            // Create a test deck with 10 cards
            _testDeck = new List<Card>();
            for (int i = 0; i < 10; i++)
            {
                _testDeck.Add(Card.CreateInvocation(
                    CardId.New(),
                    $"Test Card {i}",
                    "Test Description",
                    "Test Detailed Description",
                    attack: 3,
                    defense: 3,
                    families: new[] { CardFamily.Human },
                    affectedByEffect: true
                ));
            }

            _player = new Player(PlayerId.Player1, _testDeck, maxHealth: 30);
        }

        #region Initialization Tests

        [Test]
        public void Constructor_InitializesPlayerCorrectly()
        {
            // Assert
            Assert.AreEqual(PlayerId.Player1, _player.Id);
            Assert.AreEqual(30, _player.MaxHealth);
            Assert.AreEqual(30, _player.Health);
            Assert.AreEqual(0, _player.Shields);
            Assert.AreEqual(false, _player.BlockAttack);
            Assert.AreEqual(false, _player.SkipCurrentDraw);
            Assert.AreEqual(10, _player.DeckCount);
            Assert.AreEqual(0, _player.HandCount);
            Assert.AreEqual(0, _player.FieldCount);
            Assert.AreEqual(0, _player.GraveyardCount);
        }

        [Test]
        public void Constructor_CopiesDeckCards()
        {
            // Assert - deck should contain all cards
            Assert.AreEqual(10, _player.Deck.Count);
        }

        #endregion

        #region Card Drawing Tests

        [Test]
        public void DrawCard_DrawsCardFromDeckToHand()
        {
            // Act
            var drawnCard = _player.DrawCard();

            // Assert
            Assert.IsNotNull(drawnCard);
            Assert.AreEqual(9, _player.DeckCount);
            Assert.AreEqual(1, _player.HandCount);
            Assert.IsTrue(_player.Hand.Contains(drawnCard));
        }

        [Test]
        public void DrawCard_DrawsFromTopOfDeck()
        {
            // Arrange
            var topCard = _player.Deck[^1];

            // Act
            var drawnCard = _player.DrawCard();

            // Assert
            Assert.AreEqual(topCard, drawnCard);
        }

        [Test]
        public void DrawCard_ReturnsNullWhenDeckEmpty()
        {
            // Arrange - draw all cards
            for (int i = 0; i < 10; i++)
            {
                _player.DrawCard();
            }

            // Act
            var drawnCard = _player.DrawCard();

            // Assert
            Assert.IsNull(drawnCard);
            Assert.AreEqual(0, _player.DeckCount);
        }

        [Test]
        public void DrawCard_MultipleDrawsWorkCorrectly()
        {
            // Act
            _player.DrawCard();
            _player.DrawCard();
            _player.DrawCard();

            // Assert
            Assert.AreEqual(7, _player.DeckCount);
            Assert.AreEqual(3, _player.HandCount);
        }

        #endregion

        #region Card Playing Tests

        [Test]
        public void PlayCard_MovesCardFromHandToField()
        {
            // Arrange
            var card = _player.DrawCard();

            // Act
            var result = _player.PlayCard(card);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _player.HandCount);
            Assert.AreEqual(1, _player.FieldCount);
            Assert.IsTrue(_player.Field.Contains(card));
        }

        [Test]
        public void PlayCard_ReturnsFalseForCardNotInHand()
        {
            // Arrange
            var cardNotInHand = Card.CreateInvocation(
                CardId.New(),
                "Not In Hand",
                "Test",
                "Test",
                attack: 1,
                defense: 1,
                families: new[] { CardFamily.Human },
                affectedByEffect: false
            );

            // Act
            var result = _player.PlayCard(cardNotInHand);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, _player.FieldCount);
        }

        #endregion

        #region Card Destruction Tests

        [Test]
        public void DestroyCardFromField_MovesCardToGraveyard()
        {
            // Arrange
            var card = _player.DrawCard();
            _player.PlayCard(card);

            // Act
            var result = _player.DestroyCardFromField(card);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _player.FieldCount);
            Assert.AreEqual(1, _player.GraveyardCount);
            Assert.IsTrue(_player.Graveyard.Contains(card));
        }

        [Test]
        public void DestroyCardFromField_ReturnsFalseForCardNotOnField()
        {
            // Arrange
            var card = _player.DrawCard();

            // Act
            var result = _player.DestroyCardFromField(card);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, _player.GraveyardCount);
        }

        #endregion

        #region Discard Tests

        [Test]
        public void DiscardCard_MovesCardFromHandToGraveyard()
        {
            // Arrange
            var card = _player.DrawCard();

            // Act
            var result = _player.DiscardCard(card);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _player.HandCount);
            Assert.AreEqual(1, _player.GraveyardCount);
            Assert.IsTrue(_player.Graveyard.Contains(card));
        }

        [Test]
        public void DiscardCard_ReturnsFalseForCardNotInHand()
        {
            // Arrange
            var cardNotInHand = Card.CreateInvocation(
                CardId.New(),
                "Not In Hand",
                "Test",
                "Test",
                attack: 1,
                defense: 1,
                families: new[] { CardFamily.Human },
                affectedByEffect: false
            );

            // Act
            var result = _player.DiscardCard(cardNotInHand);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Return Card Tests

        [Test]
        public void ReturnCardToHand_MovesCardFromGraveyardToHand()
        {
            // Arrange
            var card = _player.DrawCard();
            _player.DiscardCard(card);

            // Act
            var result = _player.ReturnCardToHand(card);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _player.HandCount);
            Assert.AreEqual(0, _player.GraveyardCount);
            Assert.IsTrue(_player.Hand.Contains(card));
        }

        [Test]
        public void ReturnCardToHand_ReturnsFalseForCardNotInGraveyard()
        {
            // Arrange
            var card = _player.DrawCard();

            // Act
            var result = _player.ReturnCardToHand(card);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Search Deck Tests

        [Test]
        public void SearchDeckAndDraw_FindsAndDrawsMatchingCard()
        {
            // Act
            var foundCard = _player.SearchDeckAndDraw(c => c.Title == "Test Card 5");

            // Assert
            Assert.IsNotNull(foundCard);
            Assert.AreEqual("Test Card 5", foundCard.Title);
            Assert.AreEqual(1, _player.HandCount);
            Assert.AreEqual(9, _player.DeckCount);
            Assert.IsTrue(_player.Hand.Contains(foundCard));
        }

        [Test]
        public void SearchDeckAndDraw_ReturnsNullWhenNoMatch()
        {
            // Act
            var foundCard = _player.SearchDeckAndDraw(c => c.Title == "Non-existent Card");

            // Assert
            Assert.IsNull(foundCard);
            Assert.AreEqual(0, _player.HandCount);
            Assert.AreEqual(10, _player.DeckCount);
        }

        #endregion

        #region Damage Tests

        [Test]
        public void TakeDamage_ReducesHealth()
        {
            // Act
            var healthDamage = _player.TakeDamage(10);

            // Assert
            Assert.AreEqual(10, healthDamage);
            Assert.AreEqual(20, _player.Health);
        }

        [Test]
        public void TakeDamage_ReducesShieldsFirst()
        {
            // Arrange
            _player.AddShields(5);

            // Act
            var healthDamage = _player.TakeDamage(10);

            // Assert
            Assert.AreEqual(5, healthDamage); // Only 5 damage to health
            Assert.AreEqual(0, _player.Shields);
            Assert.AreEqual(25, _player.Health);
        }

        [Test]
        public void TakeDamage_OnlyReducesShieldsWhenDamageLessThanShields()
        {
            // Arrange
            _player.AddShields(10);

            // Act
            var healthDamage = _player.TakeDamage(5);

            // Assert
            Assert.AreEqual(0, healthDamage); // No health damage
            Assert.AreEqual(5, _player.Shields);
            Assert.AreEqual(30, _player.Health);
        }

        [Test]
        public void TakeDamage_DoesNotReduceHealthBelowZero()
        {
            // Act
            var healthDamage = _player.TakeDamage(50);

            // Assert
            Assert.AreEqual(50, healthDamage); // Reports full damage
            Assert.AreEqual(0, _player.Health);
        }

        [Test]
        public void TakeDamage_IgnoresNegativeDamage()
        {
            // Act
            var healthDamage = _player.TakeDamage(-10);

            // Assert
            Assert.AreEqual(0, healthDamage);
            Assert.AreEqual(30, _player.Health);
        }

        #endregion

        #region Healing Tests

        [Test]
        public void Heal_IncreasesHealth()
        {
            // Arrange
            _player.TakeDamage(10);

            // Act
            var healed = _player.Heal(5);

            // Assert
            Assert.AreEqual(5, healed);
            Assert.AreEqual(25, _player.Health);
        }

        [Test]
        public void Heal_DoesNotExceedMaxHealth()
        {
            // Arrange
            _player.TakeDamage(5);

            // Act
            var healed = _player.Heal(10);

            // Assert
            Assert.AreEqual(5, healed); // Only healed 5 to reach max
            Assert.AreEqual(30, _player.Health);
        }

        [Test]
        public void Heal_IgnoresNegativeAmount()
        {
            // Arrange
            _player.TakeDamage(10);

            // Act
            var healed = _player.Heal(-5);

            // Assert
            Assert.AreEqual(0, healed);
            Assert.AreEqual(20, _player.Health);
        }

        #endregion

        #region Shield Tests

        [Test]
        public void AddShields_IncreasesShields()
        {
            // Act
            _player.AddShields(5);

            // Assert
            Assert.AreEqual(5, _player.Shields);
        }

        [Test]
        public void AddShields_IgnoresNegativeAmount()
        {
            // Act
            _player.AddShields(-5);

            // Assert
            Assert.AreEqual(0, _player.Shields);
        }

        [Test]
        public void RemoveShields_DecreasesShields()
        {
            // Arrange
            _player.AddShields(10);

            // Act
            _player.RemoveShields(5);

            // Assert
            Assert.AreEqual(5, _player.Shields);
        }

        [Test]
        public void RemoveShields_DoesNotGoBelowZero()
        {
            // Arrange
            _player.AddShields(5);

            // Act
            _player.RemoveShields(10);

            // Assert
            Assert.AreEqual(0, _player.Shields);
        }

        #endregion

        #region Block Attack Tests

        [Test]
        public void SetBlockAttack_SetsBlockAttackFlag()
        {
            // Act
            _player.SetBlockAttack(true);

            // Assert
            Assert.IsTrue(_player.BlockAttack);
        }

        [Test]
        public void SetBlockAttack_CanUnblock()
        {
            // Arrange
            _player.SetBlockAttack(true);

            // Act
            _player.SetBlockAttack(false);

            // Assert
            Assert.IsFalse(_player.BlockAttack);
        }

        #endregion

        #region IsDefeated Tests

        [Test]
        public void IsDefeated_ReturnsTrueWhenHealthZero()
        {
            // Arrange
            _player.TakeDamage(30);

            // Assert
            Assert.IsTrue(_player.IsDefeated);
        }

        [Test]
        public void IsDefeated_ReturnsFalseWhenHealthAboveZero()
        {
            // Arrange
            _player.TakeDamage(10);

            // Assert
            Assert.IsFalse(_player.IsDefeated);
        }

        #endregion

        #region Snapshot Tests

        [Test]
        public void CreateSnapshot_ReturnsCorrectSnapshot()
        {
            // Arrange
            _player.DrawCard();
            _player.DrawCard();
            _player.PlayCard(_player.Hand[0]);
            _player.TakeDamage(5);
            _player.AddShields(3);

            // Act
            var snapshot = _player.CreateSnapshot();

            // Assert
            Assert.AreEqual(PlayerId.Player1, snapshot.Id);
            Assert.AreEqual(25, snapshot.Health);
            Assert.AreEqual(30, snapshot.MaxHealth);
            Assert.AreEqual(3, snapshot.Shields);
            Assert.AreEqual(8, snapshot.DeckCount);
            Assert.AreEqual(1, snapshot.HandCount);
            Assert.AreEqual(1, snapshot.FieldCount);
            Assert.AreEqual(0, snapshot.GraveyardCount);
        }

        #endregion
    }
}

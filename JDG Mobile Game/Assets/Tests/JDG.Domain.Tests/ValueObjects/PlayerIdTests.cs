using System;
using NUnit.Framework;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;

namespace JDG.Domain.Tests.ValueObjects
{
    [TestFixture]
    public class PlayerIdTests
    {
        [Test]
        public void Player1_ReturnsPlayer1Id()
        {
            // Act
            var player1 = PlayerId.Player1;

            // Assert
            Assert.AreEqual(1, player1.ToInt());
        }

        [Test]
        public void Player2_ReturnsPlayer2Id()
        {
            // Act
            var player2 = PlayerId.Player2;

            // Assert
            Assert.AreEqual(2, player2.ToInt());
        }

        [Test]
        public void FromCardOwner_ConvertsPlayer1Correctly()
        {
            // Act
            var playerId = PlayerId.FromCardOwner(CardOwner.Player1);

            // Assert
            Assert.AreEqual(PlayerId.Player1, playerId);
            Assert.AreEqual(1, playerId.ToInt());
        }

        [Test]
        public void FromCardOwner_ConvertsPlayer2Correctly()
        {
            // Act
            var playerId = PlayerId.FromCardOwner(CardOwner.Player2);

            // Assert
            Assert.AreEqual(PlayerId.Player2, playerId);
            Assert.AreEqual(2, playerId.ToInt());
        }

        [Test]
        public void FromCardOwner_ThrowsForNotDefined()
        {
            // Assert
            Assert.Throws<ArgumentException>(() =>
                PlayerId.FromCardOwner(CardOwner.NotDefined)
            );
        }

        [Test]
        public void ToCardOwner_ConvertsPlayer1Correctly()
        {
            // Arrange
            var playerId = PlayerId.Player1;

            // Act
            var owner = playerId.ToCardOwner();

            // Assert
            Assert.AreEqual(CardOwner.Player1, owner);
        }

        [Test]
        public void ToCardOwner_ConvertsPlayer2Correctly()
        {
            // Arrange
            var playerId = PlayerId.Player2;

            // Act
            var owner = playerId.ToCardOwner();

            // Assert
            Assert.AreEqual(CardOwner.Player2, owner);
        }

        [Test]
        public void Equals_ReturnsTrueForSamePlayerId()
        {
            // Arrange
            var id1 = PlayerId.Player1;
            var id2 = PlayerId.Player1;

            // Assert
            Assert.AreEqual(id1, id2);
            Assert.IsTrue(id1.Equals(id2));
            Assert.IsTrue(id1 == id2);
            Assert.IsFalse(id1 != id2);
        }

        [Test]
        public void Equals_ReturnsFalseForDifferentPlayerId()
        {
            // Arrange
            var id1 = PlayerId.Player1;
            var id2 = PlayerId.Player2;

            // Assert
            Assert.AreNotEqual(id1, id2);
            Assert.IsFalse(id1.Equals(id2));
            Assert.IsFalse(id1 == id2);
            Assert.IsTrue(id1 != id2);
        }

        [Test]
        public void GetHashCode_ReturnsSameHashForEqualPlayerIds()
        {
            // Arrange
            var id1 = PlayerId.Player1;
            var id2 = PlayerId.Player1;

            // Assert
            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Assert
            Assert.AreEqual("Player1", PlayerId.Player1.ToString());
            Assert.AreEqual("Player2", PlayerId.Player2.ToString());
        }

        [Test]
        public void Equals_WithObject_WorksCorrectly()
        {
            // Arrange
            var id1 = PlayerId.Player1;
            object id2 = PlayerId.Player1;

            // Assert
            Assert.IsTrue(id1.Equals(id2));
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var id = PlayerId.Player1;

            // Assert
            Assert.IsFalse(id.Equals(null));
        }

        [Test]
        public void RoundTrip_CardOwnerConversion_WorksCorrectly()
        {
            // Arrange
            var originalOwner = CardOwner.Player1;

            // Act
            var playerId = PlayerId.FromCardOwner(originalOwner);
            var convertedOwner = playerId.ToCardOwner();

            // Assert
            Assert.AreEqual(originalOwner, convertedOwner);
        }
    }
}

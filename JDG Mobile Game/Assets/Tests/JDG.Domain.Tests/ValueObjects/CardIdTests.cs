using System;
using NUnit.Framework;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Tests.ValueObjects
{
    [TestFixture]
    public class CardIdTests
    {
        [Test]
        public void New_CreatesUniqueCardId()
        {
            // Act
            var id1 = CardId.New();
            var id2 = CardId.New();

            // Assert
            Assert.AreNotEqual(id1, id2);
            Assert.AreNotEqual(CardId.Empty, id1);
            Assert.AreNotEqual(CardId.Empty, id2);
        }

        [Test]
        public void FromGuid_CreatesCardIdFromGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var cardId = CardId.FromGuid(guid);

            // Assert
            Assert.AreEqual(guid, cardId.ToGuid());
        }

        [Test]
        public void Empty_ReturnsEmptyCardId()
        {
            // Act
            var empty = CardId.Empty;

            // Assert
            Assert.AreEqual(Guid.Empty, empty.ToGuid());
        }

        [Test]
        public void Equals_ReturnsTrueForSameCardId()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id1 = CardId.FromGuid(guid);
            var id2 = CardId.FromGuid(guid);

            // Assert
            Assert.AreEqual(id1, id2);
            Assert.IsTrue(id1.Equals(id2));
            Assert.IsTrue(id1 == id2);
            Assert.IsFalse(id1 != id2);
        }

        [Test]
        public void Equals_ReturnsFalseForDifferentCardId()
        {
            // Arrange
            var id1 = CardId.New();
            var id2 = CardId.New();

            // Assert
            Assert.AreNotEqual(id1, id2);
            Assert.IsFalse(id1.Equals(id2));
            Assert.IsFalse(id1 == id2);
            Assert.IsTrue(id1 != id2);
        }

        [Test]
        public void GetHashCode_ReturnsSameHashForEqualCardIds()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id1 = CardId.FromGuid(guid);
            var id2 = CardId.FromGuid(guid);

            // Assert
            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
        }

        [Test]
        public void ToString_ReturnsGuidString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var cardId = CardId.FromGuid(guid);

            // Assert
            Assert.AreEqual(guid.ToString(), cardId.ToString());
        }

        [Test]
        public void Equals_WithObject_WorksCorrectly()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var id1 = CardId.FromGuid(guid);
            object id2 = CardId.FromGuid(guid);

            // Assert
            Assert.IsTrue(id1.Equals(id2));
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var id = CardId.New();

            // Assert
            Assert.IsFalse(id.Equals(null));
        }
    }
}

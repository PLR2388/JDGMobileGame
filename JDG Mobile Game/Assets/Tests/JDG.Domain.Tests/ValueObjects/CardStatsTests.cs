using NUnit.Framework;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Tests.ValueObjects
{
    [TestFixture]
    public class CardStatsTests
    {
        [Test]
        public void Constructor_InitializesStatsCorrectly()
        {
            // Act
            var stats = new CardStats(5, 3);

            // Assert
            Assert.AreEqual(5, stats.Attack);
            Assert.AreEqual(3, stats.Defense);
        }

        [Test]
        public void Modify_ReturnsModifiedStats()
        {
            // Arrange
            var stats = new CardStats(5, 3);

            // Act
            var modified = stats.Modify(2, -1);

            // Assert
            Assert.AreEqual(7, modified.Attack);
            Assert.AreEqual(2, modified.Defense);
            // Original should be unchanged (immutable)
            Assert.AreEqual(5, stats.Attack);
            Assert.AreEqual(3, stats.Defense);
        }

        [Test]
        public void Modify_WithNegativeDeltas_WorksCorrectly()
        {
            // Arrange
            var stats = new CardStats(5, 3);

            // Act
            var modified = stats.Modify(-2, -1);

            // Assert
            Assert.AreEqual(3, modified.Attack);
            Assert.AreEqual(2, modified.Defense);
        }

        [Test]
        public void WithAttack_ReturnsStatsWithNewAttack()
        {
            // Arrange
            var stats = new CardStats(5, 3);

            // Act
            var modified = stats.WithAttack(10);

            // Assert
            Assert.AreEqual(10, modified.Attack);
            Assert.AreEqual(3, modified.Defense);
            // Original unchanged
            Assert.AreEqual(5, stats.Attack);
        }

        [Test]
        public void WithDefense_ReturnsStatsWithNewDefense()
        {
            // Arrange
            var stats = new CardStats(5, 3);

            // Act
            var modified = stats.WithDefense(8);

            // Assert
            Assert.AreEqual(5, modified.Attack);
            Assert.AreEqual(8, modified.Defense);
            // Original unchanged
            Assert.AreEqual(3, stats.Defense);
        }

        [Test]
        public void Add_AddsStatsFromAnother()
        {
            // Arrange
            var stats1 = new CardStats(5, 3);
            var stats2 = new CardStats(2, 4);

            // Act
            var result = stats1.Add(stats2);

            // Assert
            Assert.AreEqual(7, result.Attack);
            Assert.AreEqual(7, result.Defense);
        }

        [Test]
        public void Subtract_SubtractsStatsFromAnother()
        {
            // Arrange
            var stats1 = new CardStats(5, 3);
            var stats2 = new CardStats(2, 1);

            // Act
            var result = stats1.Subtract(stats2);

            // Assert
            Assert.AreEqual(3, result.Attack);
            Assert.AreEqual(2, result.Defense);
        }

        [Test]
        public void Equals_ReturnsTrueForSameStats()
        {
            // Arrange
            var stats1 = new CardStats(5, 3);
            var stats2 = new CardStats(5, 3);

            // Assert
            Assert.AreEqual(stats1, stats2);
            Assert.IsTrue(stats1.Equals(stats2));
            Assert.IsTrue(stats1 == stats2);
            Assert.IsFalse(stats1 != stats2);
        }

        [Test]
        public void Equals_ReturnsFalseForDifferentStats()
        {
            // Arrange
            var stats1 = new CardStats(5, 3);
            var stats2 = new CardStats(5, 4);

            // Assert
            Assert.AreNotEqual(stats1, stats2);
            Assert.IsFalse(stats1.Equals(stats2));
            Assert.IsFalse(stats1 == stats2);
            Assert.IsTrue(stats1 != stats2);
        }

        [Test]
        public void GetHashCode_ReturnsSameHashForEqualStats()
        {
            // Arrange
            var stats1 = new CardStats(5, 3);
            var stats2 = new CardStats(5, 3);

            // Assert
            Assert.AreEqual(stats1.GetHashCode(), stats2.GetHashCode());
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var stats = new CardStats(5, 3);

            // Assert
            Assert.AreEqual("ATK: 5, DEF: 3", stats.ToString());
        }

        [Test]
        public void Zero_ReturnsZeroStats()
        {
            // Act
            var zero = CardStats.Zero;

            // Assert
            Assert.AreEqual(0, zero.Attack);
            Assert.AreEqual(0, zero.Defense);
        }

        [Test]
        public void Equals_WithObject_WorksCorrectly()
        {
            // Arrange
            var stats1 = new CardStats(5, 3);
            object stats2 = new CardStats(5, 3);

            // Assert
            Assert.IsTrue(stats1.Equals(stats2));
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var stats = new CardStats(5, 3);

            // Assert
            Assert.IsFalse(stats.Equals(null));
        }
    }
}

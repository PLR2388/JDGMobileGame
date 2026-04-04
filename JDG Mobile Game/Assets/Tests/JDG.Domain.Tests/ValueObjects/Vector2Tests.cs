using NUnit.Framework;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Tests.ValueObjects
{
    [TestFixture]
    public class Vector2Tests
    {
        [Test]
        public void Constructor_InitializesValuesCorrectly()
        {
            // Act
            var vector = new Vector2(3.5f, 7.2f);

            // Assert
            Assert.AreEqual(3.5f, vector.X);
            Assert.AreEqual(7.2f, vector.Y);
        }

        [Test]
        public void Equals_ReturnsTrueForSameVector()
        {
            // Arrange
            var v1 = new Vector2(3.5f, 7.2f);
            var v2 = new Vector2(3.5f, 7.2f);

            // Assert
            Assert.AreEqual(v1, v2);
            Assert.IsTrue(v1.Equals(v2));
            Assert.IsTrue(v1 == v2);
            Assert.IsFalse(v1 != v2);
        }

        [Test]
        public void Equals_ReturnsFalseForDifferentVector()
        {
            // Arrange
            var v1 = new Vector2(3.5f, 7.2f);
            var v2 = new Vector2(3.5f, 8.0f);

            // Assert
            Assert.AreNotEqual(v1, v2);
            Assert.IsFalse(v1.Equals(v2));
            Assert.IsFalse(v1 == v2);
            Assert.IsTrue(v1 != v2);
        }

        [Test]
        public void GetHashCode_ReturnsSameHashForEqualVectors()
        {
            // Arrange
            var v1 = new Vector2(3.5f, 7.2f);
            var v2 = new Vector2(3.5f, 7.2f);

            // Assert
            Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var vector = new Vector2(3.5f, 7.2f);

            // Assert
            Assert.AreEqual("(3.5, 7.2)", vector.ToString());
        }

        [Test]
        public void Equals_WithObject_WorksCorrectly()
        {
            // Arrange
            var v1 = new Vector2(3.5f, 7.2f);
            object v2 = new Vector2(3.5f, 7.2f);

            // Assert
            Assert.IsTrue(v1.Equals(v2));
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var vector = new Vector2(3.5f, 7.2f);

            // Assert
            Assert.IsFalse(vector.Equals(null));
        }

        [Test]
        public void Constructor_HandlesZeroValues()
        {
            // Act
            var vector = new Vector2(0f, 0f);

            // Assert
            Assert.AreEqual(0f, vector.X);
            Assert.AreEqual(0f, vector.Y);
        }

        [Test]
        public void Constructor_HandlesNegativeValues()
        {
            // Act
            var vector = new Vector2(-3.5f, -7.2f);

            // Assert
            Assert.AreEqual(-3.5f, vector.X);
            Assert.AreEqual(-7.2f, vector.Y);
        }
    }
}

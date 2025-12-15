using JDG.Application.Cards;
using JDG.Domain;
using JDG.Domain.Enums;
using NUnit.Framework;

namespace JDG.Application.Tests.Cards
{
    /// <summary>
    /// Tests for IInGameCard interface and its implementation in InGameCard.
    /// Phase 39: Created to verify interface contract for presenter migration.
    /// </summary>
    [TestFixture]
    public class IInGameCardTests
    {
        #region Interface Contract Tests

        [Test]
        public void IInGameCard_HasTitle_Property()
        {
            // Arrange
            IInGameCard card = new TestCard("Test Title");

            // Assert
            Assert.AreEqual("Test Title", card.Title);
        }

        [Test]
        public void IInGameCard_HasCardOwner_Property()
        {
            // Arrange
            IInGameCard card = new TestCard(CardOwner.Player1);

            // Assert
            Assert.AreEqual(CardOwner.Player1, card.CardOwner);
        }

        [Test]
        public void IInGameCard_HasType_Property()
        {
            // Arrange
            IInGameCard card = new TestCard(CardType.Invocation);

            // Assert
            Assert.AreEqual(CardType.Invocation, card.Type);
        }

        [Test]
        public void IInGameCard_HasCollector_Property()
        {
            // Arrange
            IInGameCard card = new TestCard(collector: true);

            // Assert
            Assert.IsTrue(card.Collector);
        }

        [Test]
        public void IInGameCard_HasDescription_Property()
        {
            // Arrange
            IInGameCard card = new TestCard(description: "Test Description");

            // Assert
            Assert.AreEqual("Test Description", card.Description);
        }

        [Test]
        public void IInGameCard_HasDetailedDescription_Property()
        {
            // Arrange
            IInGameCard card = new TestCard(detailedDescription: "Detailed test description");

            // Assert
            Assert.AreEqual("Detailed test description", card.DetailedDescription);
        }

        [Test]
        public void IInGameCard_HasVisualId_Property()
        {
            // Arrange
            IInGameCard card = new TestCard("Card Title");

            // Assert - VisualId should be based on title
            Assert.AreEqual("Card Title", card.VisualId);
        }

        #endregion

        #region CardType Coverage Tests

        [TestCase(CardType.Invocation)]
        [TestCase(CardType.Equipment)]
        [TestCase(CardType.Field)]
        [TestCase(CardType.Effect)]
        [TestCase(CardType.Contre)]
        public void IInGameCard_SupportsAllCardTypes(CardType cardType)
        {
            // Arrange
            IInGameCard card = new TestCard(cardType);

            // Assert
            Assert.AreEqual(cardType, card.Type);
        }

        #endregion

        #region CardOwner Coverage Tests

        [TestCase(CardOwner.NotDefined)]
        [TestCase(CardOwner.Player1)]
        [TestCase(CardOwner.Player2)]
        public void IInGameCard_SupportsAllCardOwners(CardOwner owner)
        {
            // Arrange
            IInGameCard card = new TestCard(owner);

            // Assert
            Assert.AreEqual(owner, card.CardOwner);
        }

        #endregion

        #region Test Implementation

        /// <summary>
        /// Simple test implementation of IInGameCard for testing interface contract.
        /// </summary>
        private class TestCard : IInGameCard
        {
            public string Title { get; }
            public CardOwner CardOwner { get; }
            public CardType Type { get; }
            public bool Collector { get; }
            public string Description { get; }
            public string DetailedDescription { get; }
            public string VisualId => Title;

            public TestCard(
                string title = "Default",
                CardOwner owner = CardOwner.NotDefined,
                CardType type = CardType.Invocation,
                bool collector = false,
                string description = "",
                string detailedDescription = "")
            {
                Title = title;
                CardOwner = owner;
                Type = type;
                Collector = collector;
                Description = description;
                DetailedDescription = detailedDescription;
            }

            public TestCard(CardOwner owner) : this("Default", owner)
            {
            }

            public TestCard(CardType type) : this("Default", CardOwner.NotDefined, type)
            {
            }
        }

        #endregion
    }
}

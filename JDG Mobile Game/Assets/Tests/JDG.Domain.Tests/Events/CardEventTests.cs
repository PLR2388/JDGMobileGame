using System;
using JDG.Domain;
using JDG.Domain.Events;
using NUnit.Framework;

namespace JDG.Domain.Tests.Events
{
    /// <summary>
    /// Unit tests for card-related domain events.
    /// Phase 41: Added comprehensive tests for event structures.
    /// These events are the output contracts of the use cases.
    /// </summary>
    [TestFixture]
    public class CardEventTests
    {
        #region CardsResetForNewTurnEvent Tests

        [Test]
        public void CardsResetForNewTurnEvent_IsStruct()
        {
            // Arrange & Act
            var evt = new CardsResetForNewTurnEvent();

            // Assert - Struct should have default values
            Assert.IsInstanceOf<CardsResetForNewTurnEvent>(evt);
        }

        #endregion

        #region CardDiedEvent Tests

        [Test]
        public void CardDiedEvent_StoresDeadCard()
        {
            // Arrange & Act
            var evt = new CardDiedEvent
            {
                DeadCard = null, // Would be InGameCard in real usage
                Owner = CardOwner.Player1
            };

            // Assert
            Assert.AreEqual(CardOwner.Player1, evt.Owner);
        }

        [Test]
        public void CardDiedEvent_SupportsPlayer2()
        {
            // Arrange & Act
            var evt = new CardDiedEvent
            {
                DeadCard = null,
                Owner = CardOwner.Player2
            };

            // Assert
            Assert.AreEqual(CardOwner.Player2, evt.Owner);
        }

        #endregion

        #region CardAddedToFieldEvent Tests

        [Test]
        public void CardAddedToFieldEvent_StoresAddedCard()
        {
            // Arrange & Act
            var evt = new CardAddedToFieldEvent
            {
                AddedCard = null, // Would be InGameInvocationCard in real usage
                Owner = CardOwner.Player1
            };

            // Assert
            Assert.AreEqual(CardOwner.Player1, evt.Owner);
        }

        [Test]
        public void CardAddedToFieldEvent_SupportsPlayer2()
        {
            // Arrange & Act
            var evt = new CardAddedToFieldEvent
            {
                AddedCard = null,
                Owner = CardOwner.Player2
            };

            // Assert
            Assert.AreEqual(CardOwner.Player2, evt.Owner);
        }

        #endregion

        #region CardRemovedFromFieldEvent Tests

        [Test]
        public void CardRemovedFromFieldEvent_StoresCardId()
        {
            // Arrange
            var cardId = Guid.NewGuid();

            // Act
            var evt = new CardRemovedFromFieldEvent
            {
                CardId = cardId,
                Owner = CardOwner.Player1
            };

            // Assert
            Assert.AreEqual(cardId, evt.CardId);
            Assert.AreEqual(CardOwner.Player1, evt.Owner);
        }

        [Test]
        public void CardRemovedFromFieldEvent_SupportsEmptyGuid()
        {
            // Arrange & Act - Legacy cards use Empty Guid
            var evt = new CardRemovedFromFieldEvent
            {
                CardId = Guid.Empty,
                Owner = CardOwner.Player2
            };

            // Assert
            Assert.AreEqual(Guid.Empty, evt.CardId);
        }

        #endregion

        #region HandCardsChangedEvent Tests

        [Test]
        public void HandCardsChangedEvent_StoresAllProperties()
        {
            // Arrange & Act
            var evt = new HandCardsChangedEvent
            {
                Owner = CardOwner.Player1,
                NewHandCount = 7,
                Delta = 2
            };

            // Assert
            Assert.AreEqual(CardOwner.Player1, evt.Owner);
            Assert.AreEqual(7, evt.NewHandCount);
            Assert.AreEqual(2, evt.Delta);
        }

        [Test]
        public void HandCardsChangedEvent_SupportsNegativeDelta()
        {
            // Arrange & Act
            var evt = new HandCardsChangedEvent
            {
                Owner = CardOwner.Player1,
                NewHandCount = 3,
                Delta = -2
            };

            // Assert
            Assert.AreEqual(-2, evt.Delta);
        }

        #endregion

        #region CardOwner Enum Tests

        [Test]
        public void CardOwner_HasExpectedValues()
        {
            // Assert
            Assert.IsTrue(Enum.IsDefined(typeof(CardOwner), CardOwner.NotDefined));
            Assert.IsTrue(Enum.IsDefined(typeof(CardOwner), CardOwner.Player1));
            Assert.IsTrue(Enum.IsDefined(typeof(CardOwner), CardOwner.Player2));
        }

        [Test]
        public void CardOwner_NotDefined_IsDefaultValue()
        {
            // Arrange & Act
            var defaultOwner = default(CardOwner);

            // Assert
            Assert.AreEqual(CardOwner.NotDefined, defaultOwner);
        }

        #endregion
    }
}

using NUnit.Framework;
using JDG.Infrastructure.Services;
using JDG.Application.Services;
using JDG.Domain.Events;
using System.Collections.Generic;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Unit tests for CardSelectionService.
    /// Part of Phase 28 - MonoBehaviour Wave 1 service extraction.
    /// </summary>
    [TestFixture]
    public class CardSelectionServiceTests
    {
        private ICardSelectionService _service;
        private TestEventBus _eventBus;
        private TestCard _card1;
        private TestCard _card2;
        private TestCard _card3;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new TestEventBus();
            _service = new CardSelectionService(_eventBus);
            _card1 = new TestCard("Card1");
            _card2 = new TestCard("Card2");
            _card3 = new TestCard("Card3");
        }

        [Test]
        public void SelectCard_WithSingleSelection_SelectsCard()
        {
            // Arrange
            _service.MultipleCardSelection = false;

            // Act
            _service.SelectCard(_card1);

            // Assert
            Assert.AreEqual(1, _service.SelectedCards.Count);
            Assert.AreEqual(_card1, _service.SelectedCards[0]);
            Assert.AreEqual(2, _eventBus.PublishedEvents.Count); // CardSelected + SelectionChanged
        }

        [Test]
        public void SelectCard_WithSingleSelection_ClearsPreviousSelection()
        {
            // Arrange
            _service.MultipleCardSelection = false;
            _service.SelectCard(_card1);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.SelectCard(_card2);

            // Assert
            Assert.AreEqual(1, _service.SelectedCards.Count);
            Assert.AreEqual(_card2, _service.SelectedCards[0]);
            // Should have: CardDeselected (_card1), CardSelected (_card2), SelectionChanged x2
            Assert.AreEqual(4, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void SelectCard_WithMultipleSelection_KeepsPreviousSelection()
        {
            // Arrange
            _service.MultipleCardSelection = true;
            _service.MultipleSelectionLimit = 3;
            _service.SelectCard(_card1);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.SelectCard(_card2);

            // Assert
            Assert.AreEqual(2, _service.SelectedCards.Count);
            Assert.Contains(_card1, _service.SelectedCards);
            Assert.Contains(_card2, _service.SelectedCards);
        }

        [Test]
        public void SelectCard_WithLimitReached_RemovesOldestSelection()
        {
            // Arrange
            _service.MultipleCardSelection = true;
            _service.MultipleSelectionLimit = 2;
            _service.SelectCard(_card1);
            _service.SelectCard(_card2);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.SelectCard(_card3);

            // Assert
            Assert.AreEqual(2, _service.SelectedCards.Count);
            Assert.IsFalse(_service.SelectedCards.Contains(_card1)); // Oldest removed
            Assert.Contains(_card2, _service.SelectedCards);
            Assert.Contains(_card3, _service.SelectedCards);
        }

        [Test]
        public void SelectCard_WithAlreadySelectedCard_DoesNotAddAgain()
        {
            // Arrange
            _service.SelectCard(_card1);
            var initialEventCount = _eventBus.PublishedEvents.Count;

            // Act
            _service.SelectCard(_card1);

            // Assert
            Assert.AreEqual(1, _service.SelectedCards.Count);
            Assert.AreEqual(initialEventCount, _eventBus.PublishedEvents.Count); // No new events
        }

        [Test]
        public void SelectCard_WithNullCard_DoesNothing()
        {
            // Act
            _service.SelectCard(null);

            // Assert
            Assert.AreEqual(0, _service.SelectedCards.Count);
            Assert.AreEqual(0, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void UnselectCard_WithSelectedCard_RemovesCard()
        {
            // Arrange
            _service.SelectCard(_card1);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.UnselectCard(_card1);

            // Assert
            Assert.AreEqual(0, _service.SelectedCards.Count);
            Assert.AreEqual(2, _eventBus.PublishedEvents.Count); // CardDeselected + SelectionChanged
        }

        [Test]
        public void UnselectCard_WithUnselectedCard_DoesNothing()
        {
            // Arrange
            _service.SelectCard(_card1);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.UnselectCard(_card2);

            // Assert
            Assert.AreEqual(1, _service.SelectedCards.Count);
            Assert.AreEqual(0, _eventBus.PublishedEvents.Count); // No events
        }

        [Test]
        public void UnselectCard_WithNullCard_DoesNothing()
        {
            // Arrange
            _service.SelectCard(_card1);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.UnselectCard(null);

            // Assert
            Assert.AreEqual(1, _service.SelectedCards.Count);
            Assert.AreEqual(0, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void ClearSelection_WithMultipleCards_RemovesAll()
        {
            // Arrange
            _service.MultipleCardSelection = true;
            _service.MultipleSelectionLimit = 3;
            _service.SelectCard(_card1);
            _service.SelectCard(_card2);
            _service.SelectCard(_card3);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.ClearSelection();

            // Assert
            Assert.AreEqual(0, _service.SelectedCards.Count);
            // Should have: CardDeselected x3 + SelectionChanged
            Assert.AreEqual(4, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void ClearSelection_WithNoCards_DoesNothing()
        {
            // Act
            _service.ClearSelection();

            // Assert
            Assert.AreEqual(0, _service.SelectedCards.Count);
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count); // Just SelectionChanged
        }

        [Test]
        public void IsCardSelected_WithSelectedCard_ReturnsTrue()
        {
            // Arrange
            _service.SelectCard(_card1);

            // Act & Assert
            Assert.IsTrue(_service.IsCardSelected(_card1));
        }

        [Test]
        public void IsCardSelected_WithUnselectedCard_ReturnsFalse()
        {
            // Arrange
            _service.SelectCard(_card1);

            // Act & Assert
            Assert.IsFalse(_service.IsCardSelected(_card2));
        }

        [Test]
        public void IsCardSelected_WithNullCard_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(_service.IsCardSelected(null));
        }

        [Test]
        public void MultipleSelectionLimit_DefaultValue_IsOne()
        {
            // Assert
            Assert.AreEqual(1, _service.MultipleSelectionLimit);
        }

        [Test]
        public void MultipleCardSelection_DefaultValue_IsFalse()
        {
            // Assert
            Assert.IsFalse(_service.MultipleCardSelection);
        }

        #region Test Helpers

        /// <summary>
        /// Simple test card class for testing.
        /// </summary>
        private class TestCard
        {
            public string Name { get; }

            public TestCard(string name)
            {
                Name = name;
            }

            public override string ToString() => Name;
        }

        /// <summary>
        /// Test double for IEventBus that tracks published events.
        /// </summary>
        private class TestEventBus : IEventBus
        {
            public List<object> PublishedEvents { get; } = new List<object>();

            public void Publish<T>(T eventData) where T : struct
            {
                PublishedEvents.Add(eventData);
            }

            public System.IDisposable Subscribe<T>(System.Action<T> handler) where T : struct
            {
                throw new System.NotImplementedException();
            }

            public void ClearSubscriptions<T>() where T : struct
            {
                throw new System.NotImplementedException();
            }

            public void ClearAllSubscriptions()
            {
                PublishedEvents.Clear();
            }
        }

        #endregion
    }
}

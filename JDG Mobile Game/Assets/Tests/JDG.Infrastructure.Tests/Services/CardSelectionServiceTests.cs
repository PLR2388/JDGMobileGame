using NUnit.Framework;
using JDG.Application;
using JDG.Domain.Events;
using System.Collections.Generic;
using System.Linq;

// Use fully qualified names to avoid conflicts with legacy types in global namespace
using ICardSelectionServiceNew = JDG.Application.Services.ICardSelectionService;
using CardSelectionServiceNew = JDG.Infrastructure.Services.CardSelectionService;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Unit tests for CardSelectionService.
    /// Part of Phase 28 - MonoBehaviour Wave 1 service extraction.
    /// </summary>
    [TestFixture]
    public class CardSelectionServiceTests
    {
        private ICardSelectionServiceNew _service;
        private TestEventBus _eventBus;
        private TestCard _card1;
        private TestCard _card2;
        private TestCard _card3;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new TestEventBus();
            _service = new CardSelectionServiceNew(_eventBus);
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
            Assert.AreEqual(2, _eventBus.PublishedEvents.Count); // CardAddedToSelection + CardSelectionChanged
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
            // Should have: CardRemovedFromSelection (_card1), CardAddedToSelection (_card2), CardSelectionChanged x2
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
            Assert.IsTrue(_service.SelectedCards.Contains(_card1));
            Assert.IsTrue(_service.SelectedCards.Contains(_card2));
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
            Assert.IsTrue(_service.SelectedCards.Contains(_card2));
            Assert.IsTrue(_service.SelectedCards.Contains(_card3));
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
            Assert.AreEqual(2, _eventBus.PublishedEvents.Count); // CardRemovedFromSelection + CardSelectionChanged
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
            // Should have: CardRemovedFromSelection x3 + CardSelectionChanged
            Assert.AreEqual(4, _eventBus.PublishedEvents.Count);
        }

        [Test]
        public void ClearSelection_WithNoCards_DoesNothing()
        {
            // Act
            _service.ClearSelection();

            // Assert
            Assert.AreEqual(0, _service.SelectedCards.Count);
            Assert.AreEqual(1, _eventBus.PublishedEvents.Count); // Just CardSelectionChanged
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

        #region Event Publishing Tests

        [Test]
        public void SelectCard_PublishesCardAddedToSelectionEvent()
        {
            // Act
            _service.SelectCard(_card1);

            // Assert
            var addedEvents = _eventBus.PublishedEvents
                .OfType<CardAddedToSelectionEvent>()
                .ToList();
            Assert.AreEqual(1, addedEvents.Count);
            Assert.AreEqual(_card1, addedEvents[0].Card);
        }

        [Test]
        public void SelectCard_PublishesCardSelectionChangedEvent()
        {
            // Act
            _service.SelectCard(_card1);

            // Assert
            var changedEvents = _eventBus.PublishedEvents
                .OfType<CardSelectionChangedEvent>()
                .ToList();
            Assert.AreEqual(1, changedEvents.Count);
            Assert.AreEqual(1, changedEvents[0].SelectedCount);
        }

        [Test]
        public void UnselectCard_PublishesCardRemovedFromSelectionEvent()
        {
            // Arrange
            _service.SelectCard(_card1);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.UnselectCard(_card1);

            // Assert
            var removedEvents = _eventBus.PublishedEvents
                .OfType<CardRemovedFromSelectionEvent>()
                .ToList();
            Assert.AreEqual(1, removedEvents.Count);
            Assert.AreEqual(_card1, removedEvents[0].Card);
        }

        [Test]
        public void ClearSelection_PublishesCardSelectionChangedEvent_WithZeroCount()
        {
            // Arrange
            _service.SelectCard(_card1);
            _eventBus.PublishedEvents.Clear();

            // Act
            _service.ClearSelection();

            // Assert
            var changedEvents = _eventBus.PublishedEvents
                .OfType<CardSelectionChangedEvent>()
                .ToList();
            Assert.AreEqual(1, changedEvents.Count);
            Assert.AreEqual(0, changedEvents[0].SelectedCount);
        }

        [Test]
        public void SelectedCards_PersistsAcrossMultipleCalls()
        {
            // Arrange
            _service.MultipleCardSelection = true;
            _service.MultipleSelectionLimit = 5;

            // Act - Select cards in multiple calls
            _service.SelectCard(_card1);
            var after1 = _service.SelectedCards.ToList();

            _service.SelectCard(_card2);
            var after2 = _service.SelectedCards.ToList();

            _service.SelectCard(_card3);
            var after3 = _service.SelectedCards.ToList();

            // Assert - Each call should preserve previous selections
            Assert.AreEqual(1, after1.Count);
            Assert.AreEqual(2, after2.Count);
            Assert.AreEqual(3, after3.Count);
            Assert.IsTrue(after3.Contains(_card1));
            Assert.IsTrue(after3.Contains(_card2));
            Assert.IsTrue(after3.Contains(_card3));
        }

        [Test]
        public void SelectedCards_ReturnsCopy_NotOriginalList()
        {
            // Arrange
            _service.SelectCard(_card1);
            var selectedCards = _service.SelectedCards;

            // Act - Try to modify the returned collection via cast
            // IReadOnlyList doesn't have Clear, but if it's backed by a mutable list,
            // casting and clearing would affect the original
            var mutableList = selectedCards as System.Collections.IList;
            if (mutableList != null)
            {
                try
                {
                    mutableList.Clear();
                }
                catch
                {
                    // Some implementations may throw on modification
                }
            }

            // Assert - Original selection should be unchanged (proves immutability or copy)
            Assert.AreEqual(1, _service.SelectedCards.Count);
        }

        #endregion

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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using JDG.Application;
using JDG.Application.Cards;
using JDG.Application.UseCases;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.Events;

namespace JDG.Application.Tests.UseCases
{
    /// <summary>
    /// Tests for ResetCardsForNewTurnUseCase.
    /// Phase 84: Created for UseCase migration validation.
    /// </summary>
    [TestFixture]
    public class ResetCardsForNewTurnUseCaseTests
    {
        private ResetCardsForNewTurnUseCase _useCase;
        private TestEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new TestEventBus();
            _useCase = new ResetCardsForNewTurnUseCase(_eventBus);
        }

        [Test]
        public void Execute_WithValidCards_ReturnsSuccess()
        {
            // Arrange
            var cards = new List<IInGameInvocationCard>
            {
                new TestInvocationCard("Card 1"),
                new TestInvocationCard("Card 2")
            };

            // Act
            var result = _useCase.Execute(cards, CardOwner.Player1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.CardsReset);
        }

        [Test]
        public void Execute_WithNullCollection_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(null, CardOwner.Player1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invocation cards collection is null", result.Message);
        }

        [Test]
        public void Execute_WithEmptyCollection_ReturnsSuccessWithZeroReset()
        {
            // Arrange
            var cards = new List<IInGameInvocationCard>();

            // Act
            var result = _useCase.Execute(cards, CardOwner.Player1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.CardsReset);
        }

        [Test]
        public void Execute_ResetsEachCard()
        {
            // Arrange
            var card1 = new TestInvocationCard("Card 1");
            var card2 = new TestInvocationCard("Card 2");
            var cards = new List<IInGameInvocationCard> { card1, card2 };

            // Act
            _useCase.Execute(cards, CardOwner.Player1);

            // Assert
            Assert.IsTrue(card1.ResetNewTurnWasCalled);
            Assert.IsTrue(card2.ResetNewTurnWasCalled);
        }

        [Test]
        public void Execute_PublishesCardsResetEvent()
        {
            // Arrange
            var cards = new List<IInGameInvocationCard>
            {
                new TestInvocationCard("Card 1")
            };

            // Act
            _useCase.Execute(cards, CardOwner.Player1);

            // Assert
            var events = _eventBus.PublishedEvents.FindAll(e => e is CardsResetForNewTurnEvent);
            Assert.AreEqual(1, events.Count);
        }

        [Test]
        public void Execute_SkipsNullCards()
        {
            // Arrange
            var cards = new List<IInGameInvocationCard>
            {
                new TestInvocationCard("Card 1"),
                null,
                new TestInvocationCard("Card 2")
            };

            // Act
            var result = _useCase.Execute(cards, CardOwner.Player1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.CardsReset); // Only 2 non-null cards reset
        }

        [Test]
        public void Execute_LegacyOverload_Works()
        {
            // Arrange
            var card = new TestInvocationCard("Card 1");
            var cards = new List<IInGameInvocationCard> { card };

            // Act - Uses legacy overload without owner
            _useCase.Execute(cards);

            // Assert
            Assert.IsTrue(card.ResetNewTurnWasCalled);
        }

        #region Test Doubles

        private class TestEventBus : IEventBus
        {
            public List<object> PublishedEvents { get; } = new List<object>();

            public void Publish<T>(T eventData) where T : struct
            {
                PublishedEvents.Add(eventData);
            }

            public IDisposable Subscribe<T>(Action<T> handler) where T : struct
            {
                return new TestDisposable();
            }

            public void ClearSubscriptions<T>() where T : struct { }
        }

        private class TestDisposable : IDisposable
        {
            public void Dispose() { }
        }

        private class TestInvocationCard : IInGameInvocationCard
        {
            public string Title { get; }
            public CardOwner CardOwner => CardOwner.Player1;
            public CardType Type => CardType.Invocation;
            public bool Collector => false;
            public string Description => "";
            public string DetailedDescription => "";
            public string VisualId => Title;
            public int Attack { get; set; }
            public int Defense { get; set; }
            public int BaseAttack => 5;
            public int BaseDefense => 5;
            public bool CanDirectAttack => false;
            public bool CantBeAttack => false;
            public bool Aggro => false;
            public bool CancelEffect => false;
            public bool IsAffectedByEffectCard => true;
            public bool IsControlled => false;
            public int NumberOfTurnOnField => 0;
            public int NumberOfDeaths => 0;
            public IEnumerable<object> Abilities => new List<object>();
            public IInGameEquipmentCard EquipmentCard => null;
            public CardFamily[] Families => new CardFamily[0];

            public bool ResetNewTurnWasCalled { get; private set; }

            public TestInvocationCard(string title)
            {
                Title = title;
            }

            public void ResetNewTurn() => ResetNewTurnWasCalled = true;
            public void FreeCard() { }
            public void UnblockAttack() { }
            public bool CanAttack() => true;
            public void BlockAttack() { }
            public void AttackTurnDone() { }
            public void SetRemainedAttackThisTurn(int count) { }
            public bool HasAction() => false;
            public void SetEquipmentCard(IInGameEquipmentCard equipment) { }
            public void ControlCard(CardOwner newOwner) { }
            public void IncrementTurnOnField() { }
            public void IncrementDeathCount() { }
        }

        #endregion
    }
}

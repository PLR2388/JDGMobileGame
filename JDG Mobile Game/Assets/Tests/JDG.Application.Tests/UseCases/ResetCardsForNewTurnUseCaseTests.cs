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
            public void ClearAllSubscriptions() { }
        }

        private class TestDisposable : IDisposable
        {
            public void Dispose() { }
        }

        private class TestInvocationCard : IInGameInvocationCard
        {
            public string CardId => Title.ToLowerInvariant().Replace(" ", "-");
            public string Title { get; }
            public CardOwner CardOwner => CardOwner.Player1;
            public CardType Type => CardType.Invocation;
            public bool Collector => false;
            public string Description => "";
            public string DetailedDescription => "";
            public string VisualId => Title;
            public float Attack { get; set; }
            public float Defense { get; set; }
            public float BaseAttack => 5f;
            public float BaseDefense => 5f;
            public bool CanDirectAttack { get; set; }
            public bool CantBeAttack { get; set; }
            public bool Aggro { get; set; }
            public bool CancelEffect { get; set; }
            public bool IsAffectedByEffectCard { get; set; } = true;
            public bool IsControlled { get; private set; }
            public int NumberOfTurnOnField { get; private set; }
            public int NumberOfDeaths { get; private set; }
            public int TimesRevived { get; set; }
            public int BonusAttacks { get; set; }
            public IReadOnlyList<object> Abilities => new List<object>();
            public IInGameEquipmentCard EquipmentCard { get; private set; }
            public CardFamily[] Families { get; set; } = System.Array.Empty<CardFamily>();

            public bool ResetNewTurnWasCalled { get; private set; }

            public TestInvocationCard(string title)
            {
                Title = title;
            }

            public void ResetNewTurn() => ResetNewTurnWasCalled = true;
            public void FreeCard() { IsControlled = false; }
            public void UnblockAttack() { }
            public bool CanAttack() => true;
            public void BlockAttack() { }
            public void AttackTurnDone() { }
            public void SetRemainedAttackThisTurn(int count) { }
            public bool HasAction() => false;
            public void SetEquipmentCard(IInGameEquipmentCard equipment) { EquipmentCard = equipment; }
            public void ControlCard() { IsControlled = true; }
            public void IncrementNumberTurnOnField() { NumberOfTurnOnField++; }
            public void IncrementNumberDeaths() { NumberOfDeaths++; }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using NUnit.Framework;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.UseCases;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.Events;

namespace JDG.Application.Tests.UseCases
{
    /// <summary>
    /// Tests for HandleHandCardsChangeUseCase.
    /// Phase 84: Created for UseCase migration validation.
    /// </summary>
    [TestFixture]
    public class HandleHandCardsChangeUseCaseTests
    {
        private HandleHandCardsChangeUseCase _useCase;
        private TestEventBus _eventBus;
        private TestAbilityExecutor _abilityExecutor;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new TestEventBus();
            _abilityExecutor = new TestAbilityExecutor();
            _useCase = new HandleHandCardsChangeUseCase(_eventBus, _abilityExecutor);
        }

        [Test]
        public void Execute_WithValidInputs_ReturnsSuccess()
        {
            // Arrange
            var player = new TestPlayerCardCollection(CardOwner.Player1, 5);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act
            var result = _useCase.Execute(player, opponent, 4, 5);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void Execute_WithNullPlayerCards_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(null, null, 0, 1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player cards collection is null", result.Message);
        }

        [Test]
        public void Execute_PublishesHandCardsChangedEvent()
        {
            // Arrange
            var player = new TestPlayerCardCollection(CardOwner.Player1, 5);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act
            _useCase.Execute(player, opponent, 4, 5);

            // Assert
            var events = _eventBus.PublishedEvents.FindAll(e => e is HandCardsChangedEvent);
            Assert.AreEqual(1, events.Count);
            var evt = (HandCardsChangedEvent)events[0];
            Assert.AreEqual(CardOwner.Player1, evt.Owner);
            Assert.AreEqual(5, evt.NewHandCount);
            Assert.AreEqual(1, evt.Delta);
        }

        [Test]
        public void Execute_DelegatesToAbilityExecutor()
        {
            // Arrange
            var player = new TestPlayerCardCollection(CardOwner.Player1, 5);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act
            _useCase.Execute(player, opponent, 4, 5);

            // Assert
            Assert.IsTrue(_abilityExecutor.OnHandCardsChangedCalled);
        }

        [Test]
        public void Execute_LegacyOverload_CalculatesCorrectCounts()
        {
            // Arrange
            var player = new TestPlayerCardCollection(CardOwner.Player1, 5);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act - delta of +1 means went from 4 to 5
            _useCase.Execute(player, opponent, 1);

            // Assert
            var events = _eventBus.PublishedEvents.FindAll(e => e is HandCardsChangedEvent);
            Assert.AreEqual(1, events.Count);
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

        private class TestAbilityExecutor : IAbilityExecutor
        {
            public bool OnHandCardsChangedCalled { get; private set; }

            public void ExecuteOnCardAddedToField(IInGameInvocationCard addedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnCardRemovedFromField(IInGameInvocationCard removedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnCardDeath(IInGameInvocationCard deadCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnTurnStart(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnTurnEnd(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnHandCardsChanged(IPlayerCardCollection playerCards, IPlayerCardCollection opponentCards, int oldCount, int newCount)
            {
                OnHandCardsChangedCalled = true;
            }
            public void ExecuteOnFieldCardChanged(IInGameFieldCard oldFieldCard, IInGameFieldCard newFieldCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEquipmentAttached(IInGameEquipmentCard equipment, IInGameInvocationCard target, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEquipmentDetached(IInGameEquipmentCard equipment, IInGameInvocationCard previousTarget, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
        }

        private class TestPlayerCardCollection : IPlayerCardCollection
        {
            public CardOwner Owner { get; }
            public bool IsPlayerOne => Owner == CardOwner.Player1;
            public IReadOnlyList<IInGameInvocationCard> InvocationCards => new List<IInGameInvocationCard>();
            public IReadOnlyList<IInGameEffectCard> EffectCards => new List<IInGameEffectCard>();
            public IInGameFieldCard FieldCard => null;
            public IReadOnlyList<IInGameCard> HandCards => new List<IInGameCard>();
            public int HandCardCount { get; }

            public TestPlayerCardCollection(CardOwner owner, int handCount = 0)
            {
                Owner = owner;
                HandCardCount = handCount;
            }
        }

        #endregion
    }
}

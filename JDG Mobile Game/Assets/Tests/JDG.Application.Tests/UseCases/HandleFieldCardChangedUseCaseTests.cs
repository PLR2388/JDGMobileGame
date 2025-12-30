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
    /// Tests for HandleFieldCardChangedUseCase.
    /// Phase 84: Created for UseCase migration validation.
    /// </summary>
    [TestFixture]
    public class HandleFieldCardChangedUseCaseTests
    {
        private HandleFieldCardChangedUseCase _useCase;
        private TestEventBus _eventBus;
        private TestAbilityExecutor _abilityExecutor;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new TestEventBus();
            _abilityExecutor = new TestAbilityExecutor();
            _useCase = new HandleFieldCardChangedUseCase(_eventBus, _abilityExecutor);
        }

        [Test]
        public void Execute_WithValidInputs_ReturnsSuccess()
        {
            // Arrange
            var oldField = new TestFieldCard("Old Field");
            var newField = new TestFieldCard("New Field");
            var owner = new TestPlayerCardCollection(CardOwner.Player1);

            // Act
            var result = _useCase.Execute(oldField, newField, owner, null);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void Execute_WithNullOwnerCards_ReturnsFailure()
        {
            // Arrange
            var oldField = new TestFieldCard("Old Field");

            // Act
            var result = _useCase.Execute(oldField, null, null, null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Owner cards collection is null", result.Message);
        }

        [Test]
        public void Execute_PublishesFieldCardReplacedEvent()
        {
            // Arrange
            var oldField = new TestFieldCard("Old Field");
            var owner = new TestPlayerCardCollection(CardOwner.Player1);

            // Act
            _useCase.Execute(oldField, null, owner, null);

            // Assert
            var events = _eventBus.PublishedEvents.FindAll(e => e is FieldCardReplacedEvent);
            Assert.AreEqual(1, events.Count);
            var evt = (FieldCardReplacedEvent)events[0];
            Assert.AreEqual(CardOwner.Player1, evt.Owner);
        }

        [Test]
        public void Execute_DelegatesToAbilityExecutor()
        {
            // Arrange
            var oldField = new TestFieldCard("Old Field");
            var newField = new TestFieldCard("New Field");
            var owner = new TestPlayerCardCollection(CardOwner.Player1);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act
            _useCase.Execute(oldField, newField, owner, opponent);

            // Assert
            Assert.IsTrue(_abilityExecutor.OnFieldCardChangedCalled);
        }

        [Test]
        public void HandleFieldCardRemoved_CallsExecuteWithNullNewCard()
        {
            // Arrange
            var oldField = new TestFieldCard("Old Field");
            var owner = new TestPlayerCardCollection(CardOwner.Player1);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act
            _useCase.HandleFieldCardRemoved(oldField, owner, opponent);

            // Assert
            Assert.IsTrue(_abilityExecutor.OnFieldCardChangedCalled);
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
            public bool OnFieldCardChangedCalled { get; private set; }

            public void ExecuteOnCardAddedToField(IInGameInvocationCard addedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnCardRemovedFromField(IInGameInvocationCard removedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnCardDeath(IInGameInvocationCard deadCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnTurnStart(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnTurnEnd(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnHandCardsChanged(IPlayerCardCollection playerCards, IPlayerCardCollection opponentCards, int oldCount, int newCount) { }
            public void ExecuteOnFieldCardChanged(IInGameFieldCard oldFieldCard, IInGameFieldCard newFieldCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards)
            {
                OnFieldCardChangedCalled = true;
            }
            public void ExecuteOnEquipmentAttached(IInGameEquipmentCard equipment, IInGameInvocationCard target, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEquipmentDetached(IInGameEquipmentCard equipment, IInGameInvocationCard previousTarget, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEffectCardPlayed(IInGameEffectCard effectCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
        }

        private class TestFieldCard : IInGameFieldCard
        {
            public string Title { get; }
            public CardOwner CardOwner => CardOwner.Player1;
            public CardType Type => CardType.Field;
            public bool Collector => false;
            public string Description => "";
            public string DetailedDescription => "";
            public string VisualId => Title;
            public IReadOnlyList<IAbility> FieldAbilities => new List<IAbility>();

            public TestFieldCard(string title)
            {
                Title = title;
            }
        }

        private class TestPlayerCardCollection : IPlayerCardCollection
        {
            public CardOwner Owner { get; }
            public bool IsPlayerOne => Owner == CardOwner.Player1;
            public IReadOnlyList<IInGameInvocationCard> InvocationCards => new List<IInGameInvocationCard>();
            public IReadOnlyList<IInGameEffectCard> EffectCards => new List<IInGameEffectCard>();
            public IInGameFieldCard FieldCard => null;
            public IReadOnlyList<IInGameCard> HandCards => new List<IInGameCard>();
            public int HandCardCount => 0;

            public TestPlayerCardCollection(CardOwner owner)
            {
                Owner = owner;
            }
        }

        #endregion
    }
}

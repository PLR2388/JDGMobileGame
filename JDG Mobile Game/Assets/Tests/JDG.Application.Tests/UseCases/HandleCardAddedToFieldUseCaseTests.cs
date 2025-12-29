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
    /// Tests for HandleCardAddedToFieldUseCase.
    /// Phase 84: Created for UseCase migration validation.
    /// </summary>
    [TestFixture]
    public class HandleCardAddedToFieldUseCaseTests
    {
        private HandleCardAddedToFieldUseCase _useCase;
        private TestEventBus _eventBus;
        private TestAbilityExecutor _abilityExecutor;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new TestEventBus();
            _abilityExecutor = new TestAbilityExecutor();
            _useCase = new HandleCardAddedToFieldUseCase(_eventBus, _abilityExecutor);
        }

        [Test]
        public void Execute_WithValidCard_ReturnsSuccess()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", CardOwner.Player1);
            var owner = new TestPlayerCardCollection(CardOwner.Player1);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act
            var result = _useCase.Execute(card, owner, opponent);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void Execute_WithNullCard_ReturnsFailure()
        {
            // Arrange
            var owner = new TestPlayerCardCollection(CardOwner.Player1);

            // Act
            var result = _useCase.Execute(null, owner, null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Added card is null", result.Message);
        }

        [Test]
        public void Execute_WithNullOwnerCards_ReturnsFailure()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", CardOwner.Player1);

            // Act
            var result = _useCase.Execute(card, null, null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Owner cards collection is null", result.Message);
        }

        [Test]
        public void Execute_PublishesCardAddedToFieldEvent()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", CardOwner.Player1);
            var owner = new TestPlayerCardCollection(CardOwner.Player1);

            // Act
            _useCase.Execute(card, owner, null);

            // Assert
            var addedEvents = _eventBus.PublishedEvents.FindAll(e => e is CardAddedToFieldEvent);
            Assert.AreEqual(1, addedEvents.Count);
            var evt = (CardAddedToFieldEvent)addedEvents[0];
            Assert.AreEqual(CardOwner.Player1, evt.Owner);
        }

        [Test]
        public void Execute_DelegatesToAbilityExecutor()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", CardOwner.Player1);
            var owner = new TestPlayerCardCollection(CardOwner.Player1);
            var opponent = new TestPlayerCardCollection(CardOwner.Player2);

            // Act
            _useCase.Execute(card, owner, opponent);

            // Assert
            Assert.IsTrue(_abilityExecutor.OnCardAddedCalled);
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

        private class TestAbilityExecutor : IAbilityExecutor
        {
            public bool OnCardAddedCalled { get; private set; }

            public void ExecuteOnCardAddedToField(IInGameInvocationCard addedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards)
            {
                OnCardAddedCalled = true;
            }
            public void ExecuteOnCardRemovedFromField(IInGameInvocationCard removedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnCardDeath(IInGameInvocationCard deadCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnTurnStart(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnTurnEnd(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnHandCardsChanged(IPlayerCardCollection playerCards, IPlayerCardCollection opponentCards, int oldCount, int newCount) { }
            public void ExecuteOnFieldCardChanged(IInGameFieldCard oldFieldCard, IInGameFieldCard newFieldCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
        }

        private class TestInvocationCard : IInGameInvocationCard
        {
            public string Title { get; }
            public CardOwner CardOwner { get; }
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

            public TestInvocationCard(string title, CardOwner owner)
            {
                Title = title;
                CardOwner = owner;
            }

            public void ResetNewTurn() { }
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

        private class TestPlayerCardCollection : IPlayerCardCollection
        {
            public CardOwner Owner { get; }
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

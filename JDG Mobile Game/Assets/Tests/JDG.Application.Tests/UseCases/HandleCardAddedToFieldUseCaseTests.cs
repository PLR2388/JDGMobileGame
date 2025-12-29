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
            public void ClearAllSubscriptions() { }
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
            public void ExecuteOnEquipmentAttached(IInGameEquipmentCard equipment, IInGameInvocationCard target, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEquipmentDetached(IInGameEquipmentCard equipment, IInGameInvocationCard previousTarget, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
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
            public IReadOnlyList<object> Abilities => new List<object>();
            public IInGameEquipmentCard EquipmentCard { get; private set; }
            public CardFamily[] Families { get; set; } = System.Array.Empty<CardFamily>();

            public TestInvocationCard(string title, CardOwner owner)
            {
                Title = title;
                CardOwner = owner;
            }

            public void ResetNewTurn() { }
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

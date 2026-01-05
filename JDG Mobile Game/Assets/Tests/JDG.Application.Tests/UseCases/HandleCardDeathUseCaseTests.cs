using System;
using System.Collections.Generic;
using NUnit.Framework;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.UseCases;
using JDG.Domain.Enums;
using JDG.Domain.Events;

namespace JDG.Application.Tests.UseCases
{
    /// <summary>
    /// Tests for HandleCardDeathUseCase.
    /// Phase 84: Created for UseCase migration validation.
    /// </summary>
    [TestFixture]
    public class HandleCardDeathUseCaseTests
    {
        private HandleCardDeathUseCase _useCase;
        private TestEventBus _eventBus;
        private TestAbilityExecutor _abilityExecutor;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new TestEventBus();
            _abilityExecutor = new TestAbilityExecutor();
            _useCase = new HandleCardDeathUseCase(_eventBus, _abilityExecutor);
        }

        [Test]
        public void Execute_WithValidCard_ReturnsSuccess()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", JDG.Domain.CardOwner.Player1);
            var owner = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player1);
            var opponent = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player2);

            // Act
            var result = _useCase.Execute(card, owner, opponent);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void Execute_WithNullCard_ReturnsFailure()
        {
            // Arrange
            var owner = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player1);

            // Act
            var result = _useCase.Execute((IInGameInvocationCard)null, owner, null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Dead card is null", result.Message);
        }

        [Test]
        public void Execute_WithNullOwnerCards_ReturnsFailure()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", JDG.Domain.CardOwner.Player1);

            // Act
            var result = _useCase.Execute(card, null, null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Owner cards collection is null", result.Message);
        }

        [Test]
        public void Execute_ResetsCardState()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", JDG.Domain.CardOwner.Player1);
            card.Attack = 10;
            card.Defense = 10;
            var owner = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player1);

            // Act
            _useCase.Execute(card, owner, null);

            // Assert - Card state should be reset
            Assert.IsTrue(card.ResetNewTurnWasCalled);
            Assert.IsTrue(card.FreeCardWasCalled);
            Assert.IsTrue(card.UnblockAttackWasCalled);
        }

        [Test]
        public void Execute_PublishesCardDiedEvent()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", JDG.Domain.CardOwner.Player1);
            var owner = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player1);

            // Act
            _useCase.Execute(card, owner, null);

            // Assert
            var diedEvents = _eventBus.PublishedEvents.FindAll(e => e is CardDiedEvent);
            Assert.AreEqual(1, diedEvents.Count);
            var evt = (CardDiedEvent)diedEvents[0];
            Assert.AreEqual(JDG.Domain.CardOwner.Player1, evt.Owner);
        }

        [Test]
        public void Execute_DelegatesToAbilityExecutor()
        {
            // Arrange
            var card = new TestInvocationCard("Test Card", JDG.Domain.CardOwner.Player1);
            var owner = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player1);
            var opponent = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player2);

            // Act
            _useCase.Execute(card, owner, opponent);

            // Assert
            Assert.IsTrue(_abilityExecutor.OnCardDeathCalled);
        }

        [Test]
        public void Execute_WithIInGameCard_SucceedsForInvocationCard()
        {
            // Arrange
            IInGameCard card = new TestInvocationCard("Test Card", JDG.Domain.CardOwner.Player1);
            var owner = new TestPlayerCardCollection(JDG.Domain.CardOwner.Player1);

            // Act
            var result = _useCase.Execute(card, owner, null);

            // Assert
            Assert.IsTrue(result.IsSuccess);
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
            public bool OnCardDeathCalled { get; private set; }

            public void ExecuteOnCardAddedToField(IInGameInvocationCard addedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnCardRemovedFromField(IInGameInvocationCard removedCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnCardDeath(IInGameInvocationCard deadCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards)
            {
                OnCardDeathCalled = true;
            }
            public void ExecuteOnTurnStart(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnTurnEnd(IPlayerCardCollection currentPlayer, IPlayerCardCollection opponent) { }
            public void ExecuteOnHandCardsChanged(IPlayerCardCollection playerCards, IPlayerCardCollection opponentCards, int oldCount, int newCount) { }
            public void ExecuteOnFieldCardChanged(IInGameFieldCard oldFieldCard, IInGameFieldCard newFieldCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEquipmentAttached(IInGameEquipmentCard equipment, IInGameInvocationCard target, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEquipmentDetached(IInGameEquipmentCard equipment, IInGameInvocationCard previousTarget, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
            public void ExecuteOnEffectCardPlayed(IInGameEffectCard effectCard, IPlayerCardCollection ownerCards, IPlayerCardCollection opponentCards) { }
        }

        private class TestInvocationCard : IInGameInvocationCard
        {
            public string CardId => Title.ToLowerInvariant().Replace(" ", "-");
            public string Title { get; }
            public JDG.Domain.CardOwner CardOwner { get; }
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
            public bool FreeCardWasCalled { get; private set; }
            public bool UnblockAttackWasCalled { get; private set; }

            public TestInvocationCard(string title, JDG.Domain.CardOwner owner)
            {
                Title = title;
                CardOwner = owner;
                Attack = BaseAttack;
                Defense = BaseDefense;
            }

            public void ResetNewTurn() => ResetNewTurnWasCalled = true;
            public void FreeCard() { FreeCardWasCalled = true; IsControlled = false; }
            public void UnblockAttack() => UnblockAttackWasCalled = true;
            public bool CanAttack() => true;
            public void BlockAttack() { }
            public void AttackTurnDone() { }
            public void SetRemainedAttackThisTurn(int count) { }
            public bool HasAction() => false;
            public bool SetEquipmentCard(IInGameEquipmentCard equipment) { EquipmentCard = equipment; return true; }
            public void ControlCard() { IsControlled = true; }
            public void IncrementNumberTurnOnField() { NumberOfTurnOnField++; }
            public void IncrementNumberDeaths() { NumberOfDeaths++; }
        }

        private class TestPlayerCardCollection : IPlayerCardCollection
        {
            public JDG.Domain.CardOwner Owner { get; }
            public bool IsPlayerOne => Owner == JDG.Domain.CardOwner.Player1;
            public IReadOnlyList<IInGameInvocationCard> InvocationCards => new List<IInGameInvocationCard>();
            public IReadOnlyList<IInGameEffectCard> EffectCards => new List<IInGameEffectCard>();
            public IInGameFieldCard FieldCard => null;
            public IReadOnlyList<IInGameCard> HandCards => new List<IInGameCard>();
            public int HandCardCount => 0;

            public TestPlayerCardCollection(JDG.Domain.CardOwner owner)
            {
                Owner = owner;
            }
        }

        #endregion
    }
}

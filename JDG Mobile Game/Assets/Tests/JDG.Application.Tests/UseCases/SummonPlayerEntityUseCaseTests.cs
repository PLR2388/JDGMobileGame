using System;
using System.Collections.Generic;
using NUnit.Framework;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Application.UseCases;
using JDG.Domain;
using JDG.Domain.Enums;

namespace JDG.Application.Tests.UseCases
{
    /// <summary>
    /// Tests for SummonPlayerEntityUseCase.
    /// Phase 84: Created for UseCase migration validation.
    /// </summary>
    [TestFixture]
    public class SummonPlayerEntityUseCaseTests
    {
        private SummonPlayerEntityUseCase _useCase;
        private TestCardFactory _cardFactory;

        [SetUp]
        public void SetUp()
        {
            _cardFactory = new TestCardFactory();
            _useCase = new SummonPlayerEntityUseCase(_cardFactory);
        }

        [Test]
        public void Execute_WithValidCard_ReturnsSuccess()
        {
            // Arrange
            object playerCard = new object(); // Simulates InvocationCard ScriptableObject

            // Act
            var result = _useCase.Execute(playerCard, CardOwner.Player1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.EntityCard);
            Assert.AreEqual(CardOwner.Player1, result.Owner);
        }

        [Test]
        public void Execute_WithNullCard_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute(null, CardOwner.Player1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player invocation card is null", result.Message);
        }

        [Test]
        public void Execute_WithFactoryReturningNull_ReturnsFailure()
        {
            // Arrange
            _cardFactory.ShouldReturnNull = true;
            object playerCard = new object();

            // Act
            var result = _useCase.Execute(playerCard, CardOwner.Player1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to create player entity card", result.Message);
        }

        [Test]
        public void Execute_ForPlayer1_SetsCorrectOwner()
        {
            // Arrange
            object playerCard = new object();

            // Act
            var result = _useCase.Execute(playerCard, CardOwner.Player1);

            // Assert
            Assert.AreEqual(CardOwner.Player1, result.Owner);
            Assert.AreEqual(CardOwner.Player1, result.EntityCard.CardOwner);
        }

        [Test]
        public void Execute_ForPlayer2_SetsCorrectOwner()
        {
            // Arrange
            object playerCard = new object();

            // Act
            var result = _useCase.Execute(playerCard, CardOwner.Player2);

            // Assert
            Assert.AreEqual(CardOwner.Player2, result.Owner);
        }

        [Test]
        public void Execute_BoolOverload_ConvertsToCorrectOwner()
        {
            // Arrange
            object playerCard = new object();

            // Act
            var resultP1 = _useCase.Execute(playerCard, isPlayerOne: true);
            var resultP2 = _useCase.Execute(playerCard, isPlayerOne: false);

            // Assert
            Assert.AreEqual(CardOwner.Player1, resultP1.Owner);
            Assert.AreEqual(CardOwner.Player2, resultP2.Owner);
        }

        [Test]
        public void CreatePlayerEntity_ReturnsCardDirectly()
        {
            // Arrange
            object playerCard = new object();

            // Act
            var card = _useCase.CreatePlayerEntity(playerCard, CardOwner.Player1);

            // Assert
            Assert.IsNotNull(card);
        }

        [Test]
        public void CreatePlayerEntity_WithNullCard_ReturnsNull()
        {
            // Act
            var card = _useCase.CreatePlayerEntity(null, CardOwner.Player1);

            // Assert
            Assert.IsNull(card);
        }

        #region Test Doubles

        private class TestCardFactory : ICardFactory
        {
            public bool ShouldReturnNull { get; set; }
            private CardOwner _lastOwner;

            public IInGameCard CreateCard(object baseCard, CardOwner owner)
            {
                _lastOwner = owner;
                if (ShouldReturnNull) return null;
                return new TestInvocationCard("Player Entity", owner);
            }

            public IInGameInvocationCard CreateInvocationCard(object baseInvocationCard, CardOwner owner)
            {
                _lastOwner = owner;
                if (ShouldReturnNull) return null;
                return new TestInvocationCard("Player Entity", owner);
            }

            public IInGameEffectCard CreateEffectCard(object baseEffectCard, CardOwner owner)
            {
                throw new NotImplementedException();
            }

            public IInGameFieldCard CreateFieldCard(object baseFieldCard, CardOwner owner)
            {
                throw new NotImplementedException();
            }

            public IInGameEquipmentCard CreateEquipmentCard(object baseEquipmentCard, CardOwner owner)
            {
                throw new NotImplementedException();
            }
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

        #endregion
    }
}

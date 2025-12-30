using NUnit.Framework;
using JDG.Infrastructure.Services;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using System.Collections.Generic;

// Alias to resolve global namespace conflicts
using DomainCardOwner = JDG.Domain.CardOwner;
using DomainAbilityName = JDG.Domain.AbilityName;
using DomainConditionName = JDG.Domain.Enums.ConditionName;

namespace JDG.Infrastructure.Tests.Services
{
    [TestFixture]
    public class CardStateServiceTests
    {
        private CardStateService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new CardStateService();
        }

        #region Turn Management Tests

        [Test]
        public void ResetForNewTurn_WithValidState_ResetsAttackCounterAndUnblocks()
        {
            // Arrange
            var state = CreateTestInvocationState();
            state.RemainedAttacksThisTurn = 0;
            state.BlockAttackNextTurn = true;

            // Act
            _service.ResetForNewTurn(state);

            // Assert
            Assert.AreEqual(1, state.RemainedAttacksThisTurn);
            Assert.IsFalse(state.BlockAttackNextTurn);
        }

        [Test]
        public void ResetForNewTurn_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.ResetForNewTurn(null));
        }

        [Test]
        public void IncrementTurnOnField_WithValidState_IncrementsTurnCount()
        {
            // Arrange
            var state = CreateTestInvocationState();
            Assert.AreEqual(0, state.TurnsOnField);

            // Act
            _service.IncrementTurnOnField(state);

            // Assert
            Assert.AreEqual(1, state.TurnsOnField);
        }

        [Test]
        public void IncrementTurnOnField_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.IncrementTurnOnField(null));
        }

        #endregion

        #region Combat Tests

        [Test]
        public void ApplyDamage_WithValidState_ReducesDefense()
        {
            // Arrange
            var state = CreateTestInvocationState(baseDefense: 100);

            // Act
            var isDestroyed = _service.ApplyDamage(state, 30);

            // Assert
            Assert.AreEqual(70, state.CurrentDefense);
            Assert.IsFalse(isDestroyed);
        }

        [Test]
        public void ApplyDamage_WhenDefenseGoesZero_ReturnsDestroyed()
        {
            // Arrange
            var state = CreateTestInvocationState(baseDefense: 50);

            // Act
            var isDestroyed = _service.ApplyDamage(state, 50);

            // Assert
            Assert.AreEqual(0, state.CurrentDefense);
            Assert.IsTrue(isDestroyed);
        }

        [Test]
        public void ApplyDamage_WhenDefenseGoesNegative_ReturnsDestroyed()
        {
            // Arrange
            var state = CreateTestInvocationState(baseDefense: 30);

            // Act
            var isDestroyed = _service.ApplyDamage(state, 100);

            // Assert
            Assert.AreEqual(-70, state.CurrentDefense);
            Assert.IsTrue(isDestroyed);
        }

        [Test]
        public void ApplyDamage_WithNullState_ReturnsFalse()
        {
            // Act
            var result = _service.ApplyDamage(null, 50);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CanAttack_WhenHasAttacksAndNotBlocked_ReturnsTrue()
        {
            // Arrange
            var state = CreateTestInvocationState();
            state.RemainedAttacksThisTurn = 1;
            state.BlockAttackNextTurn = false;

            // Act
            var canAttack = _service.CanAttack(state);

            // Assert
            Assert.IsTrue(canAttack);
        }

        [Test]
        public void CanAttack_WhenNoAttacksRemaining_ReturnsFalse()
        {
            // Arrange
            var state = CreateTestInvocationState();
            state.RemainedAttacksThisTurn = 0;

            // Act
            var canAttack = _service.CanAttack(state);

            // Assert
            Assert.IsFalse(canAttack);
        }

        [Test]
        public void CanAttack_WhenBlocked_ReturnsFalse()
        {
            // Arrange
            var state = CreateTestInvocationState();
            state.RemainedAttacksThisTurn = 1;
            state.BlockAttackNextTurn = true;

            // Act
            var canAttack = _service.CanAttack(state);

            // Assert
            Assert.IsFalse(canAttack);
        }

        [Test]
        public void CanAttack_WithNullState_ReturnsFalse()
        {
            // Act
            var result = _service.CanAttack(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void RecordAttack_WithValidState_DecrementsAttackCounter()
        {
            // Arrange
            var state = CreateTestInvocationState();
            state.RemainedAttacksThisTurn = 2;

            // Act
            _service.RecordAttack(state);

            // Assert
            Assert.AreEqual(1, state.RemainedAttacksThisTurn);
        }

        [Test]
        public void RecordAttack_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.RecordAttack(null));
        }

        #endregion

        #region Stats Tests

        [Test]
        public void ModifyStats_WithValidState_ModifiesBothAttackAndDefense()
        {
            // Arrange
            var state = CreateTestInvocationState(baseAttack: 50, baseDefense: 100);

            // Act
            _service.ModifyStats(state, 10, -20);

            // Assert
            Assert.AreEqual(60, state.CurrentAttack);
            Assert.AreEqual(80, state.CurrentDefense);
        }

        [Test]
        public void ModifyStats_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.ModifyStats(null, 10, 10));
        }

        [Test]
        public void ResetToBaseStats_WithModifiedState_RestoresToBaseValues()
        {
            // Arrange
            var state = CreateTestInvocationState(baseAttack: 50, baseDefense: 100);
            state.CurrentAttack = 200;
            state.CurrentDefense = 300;

            // Act
            _service.ResetToBaseStats(state);

            // Assert
            Assert.AreEqual(50, state.CurrentAttack);
            Assert.AreEqual(100, state.CurrentDefense);
        }

        [Test]
        public void ResetToBaseStats_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.ResetToBaseStats(null));
        }

        [Test]
        public void SetStats_WithValidState_SetsExactValues()
        {
            // Arrange
            var state = CreateTestInvocationState();

            // Act
            _service.SetStats(state, 999, 888);

            // Assert
            Assert.AreEqual(999, state.CurrentAttack);
            Assert.AreEqual(888, state.CurrentDefense);
        }

        [Test]
        public void SetStats_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.SetStats(null, 100, 100));
        }

        #endregion

        #region Death/Field Removal Tests

        [Test]
        public void PrepareForDeath_WithValidState_ResetsAllState()
        {
            // Arrange
            var state = CreateTestInvocationState(baseAttack: 50, baseDefense: 100);
            state.CurrentAttack = 200;
            state.CurrentDefense = 0;
            state.BlockAttackNextTurn = true;
            state.IsControlled = true;
            state.RemainedAttacksThisTurn = 0;
            state.EquippedCardId = CardId.New();
            state.TurnsOnField = 5;

            // Act
            _service.PrepareForDeath(state);

            // Assert
            Assert.AreEqual(50, state.CurrentAttack);   // Reset to base
            Assert.AreEqual(100, state.CurrentDefense); // Reset to base
            Assert.IsFalse(state.BlockAttackNextTurn);  // Unblocked
            Assert.IsFalse(state.IsControlled);         // Freed
            Assert.AreEqual(1, state.RemainedAttacksThisTurn); // Reset attack counter
            Assert.AreEqual(1, state.NumberOfDeaths);   // Incremented
            Assert.IsNull(state.EquippedCardId);        // Equipment detached
            Assert.AreEqual(0, state.TurnsOnField);     // Field state reset
        }

        [Test]
        public void PrepareForDeath_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.PrepareForDeath(null));
        }

        [Test]
        public void PrepareForDeath_CalledMultipleTimes_IncrementsDeathCount()
        {
            // Arrange
            var state = CreateTestInvocationState();

            // Act
            _service.PrepareForDeath(state);
            _service.PrepareForDeath(state);
            _service.PrepareForDeath(state);

            // Assert
            Assert.AreEqual(3, state.NumberOfDeaths);
        }

        [Test]
        public void PrepareForFieldRemoval_WithInvocationState_ResetsFieldState()
        {
            // Arrange
            var state = CreateTestInvocationState(baseAttack: 50, baseDefense: 100);
            state.CurrentAttack = 200;
            state.TurnsOnField = 5;
            state.IsBlocked = true;
            state.IsControlled = true;
            state.EquippedCardId = CardId.New();

            // Act
            _service.PrepareForFieldRemoval(state);

            // Assert
            Assert.AreEqual(0, state.TurnsOnField);
            Assert.IsFalse(state.IsBlocked);
            Assert.IsFalse(state.IsControlled);
            Assert.AreEqual(50, state.CurrentAttack);   // Reset to base
            Assert.IsNull(state.EquippedCardId);
        }

        [Test]
        public void PrepareForFieldRemoval_WithGenericState_ResetsBasicFieldState()
        {
            // Arrange
            var state = InGameCardState.Create(
                CardId.New(),
                CardType.Effect,
                DomainCardOwner.Player1);
            state.TurnsOnField = 3;
            state.IsBlocked = true;
            state.IsControlled = true;

            // Act
            _service.PrepareForFieldRemoval(state);

            // Assert
            Assert.AreEqual(0, state.TurnsOnField);
            Assert.IsFalse(state.IsBlocked);
            Assert.IsFalse(state.IsControlled);
        }

        [Test]
        public void PrepareForFieldRemoval_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.PrepareForFieldRemoval(null));
        }

        #endregion

        #region Control Tests

        [Test]
        public void TakeControl_WithValidState_SetsControlledTrue()
        {
            // Arrange
            var state = CreateTestInvocationState();
            Assert.IsFalse(state.IsControlled);

            // Act
            _service.TakeControl(state);

            // Assert
            Assert.IsTrue(state.IsControlled);
        }

        [Test]
        public void TakeControl_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.TakeControl(null));
        }

        [Test]
        public void ReleaseControl_WithControlledState_SetsControlledFalse()
        {
            // Arrange
            var state = CreateTestInvocationState();
            state.IsControlled = true;

            // Act
            _service.ReleaseControl(state);

            // Assert
            Assert.IsFalse(state.IsControlled);
        }

        [Test]
        public void ReleaseControl_WithNullState_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.ReleaseControl(null));
        }

        #endregion

        #region Helper Methods

        private InvocationCardState CreateTestInvocationState(
            float baseAttack = 50,
            float baseDefense = 100)
        {
            return new InvocationCardState(
                cardDefinitionId: CardId.New(),
                owner: DomainCardOwner.Player1,
                baseAttack: baseAttack,
                baseDefense: baseDefense,
                families: new List<CardFamily> { CardFamily.Comics },
                abilities: new List<DomainAbilityName>(),
                conditions: new List<DomainConditionName>(),
                isAffectedByEffect: true);
        }

        #endregion
    }
}

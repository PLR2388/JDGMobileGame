using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using JDG.Application.Services;

namespace JDG.Application.Tests.Services
{
    /// <summary>
    /// Unit tests for CombatLogic.
    /// Tests pure combat calculations without Unity dependencies.
    /// Phase: Test coverage extraction.
    /// </summary>
    [TestFixture]
    public class CombatLogicTests
    {
        private ICombatLogic _combatLogic;

        [SetUp]
        public void SetUp()
        {
            _combatLogic = new CombatLogic();
        }

        #region ComputeDamage Tests

        [Test]
        public void ComputeDamage_WhenAttackEqualsDefense_ReturnsZero()
        {
            // Arrange
            float attack = 5f;
            float defense = 5f;

            // Act
            var result = _combatLogic.ComputeDamage(attack, defense);

            // Assert
            Assert.AreEqual(0f, result);
        }

        [Test]
        public void ComputeDamage_WhenAttackGreaterThanDefense_ReturnsNegative()
        {
            // Arrange
            float attack = 7f;
            float defense = 3f;

            // Act
            var result = _combatLogic.ComputeDamage(attack, defense);

            // Assert
            Assert.AreEqual(-4f, result);
        }

        [Test]
        public void ComputeDamage_WhenAttackLessThanDefense_ReturnsPositive()
        {
            // Arrange
            float attack = 2f;
            float defense = 5f;

            // Act
            var result = _combatLogic.ComputeDamage(attack, defense);

            // Assert
            Assert.AreEqual(3f, result);
        }

        [Test]
        public void ComputeDamage_WithZeroAttack_ReturnsDefenseUnchanged()
        {
            // Arrange
            float attack = 0f;
            float defense = 5f;

            // Act
            var result = _combatLogic.ComputeDamage(attack, defense);

            // Assert
            Assert.AreEqual(5f, result);
        }

        [Test]
        public void ComputeDamage_WithZeroDefense_ReturnsNegativeAttack()
        {
            // Arrange
            float attack = 5f;
            float defense = 0f;

            // Act
            var result = _combatLogic.ComputeDamage(attack, defense);

            // Assert
            Assert.AreEqual(-5f, result);
        }

        #endregion

        #region HasAggroCard Tests

        [Test]
        public void HasAggroCard_WithAggroCard_ReturnsTrue()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal", 3, 3),
                CombatCardInfo.Create("Aggro", 4, 4, aggro: true),
                CombatCardInfo.Create("Another", 2, 2)
            };

            // Act
            var result = _combatLogic.HasAggroCard(cards);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAggroCard_WithNoAggroCards_ReturnsFalse()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal1", 3, 3),
                CombatCardInfo.Create("Normal2", 4, 4)
            };

            // Act
            var result = _combatLogic.HasAggroCard(cards);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void HasAggroCard_WithEmptyList_ReturnsFalse()
        {
            // Arrange
            var cards = new List<CombatCardInfo>();

            // Act
            var result = _combatLogic.HasAggroCard(cards);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void HasAggroCard_WithNullList_ReturnsFalse()
        {
            // Act
            var result = _combatLogic.HasAggroCard(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void HasAggroCard_WithNullCards_SkipsNulls()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                null,
                CombatCardInfo.Create("Aggro", 4, 4, aggro: true),
                null
            };

            // Act
            var result = _combatLogic.HasAggroCard(cards);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region GetOnlyAggroCards Tests

        [Test]
        public void GetOnlyAggroCards_ReturnsOnlyAggroCards()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal", 3, 3),
                CombatCardInfo.Create("Aggro1", 4, 4, aggro: true),
                CombatCardInfo.Create("Another", 2, 2),
                CombatCardInfo.Create("Aggro2", 5, 5, aggro: true)
            };

            // Act
            var result = _combatLogic.GetOnlyAggroCards(cards);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(c => c.Aggro));
        }

        [Test]
        public void GetOnlyAggroCards_WithNoAggroCards_ReturnsEmptyList()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal1", 3, 3),
                CombatCardInfo.Create("Normal2", 4, 4)
            };

            // Act
            var result = _combatLogic.GetOnlyAggroCards(cards);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetOnlyAggroCards_WithNullList_ReturnsEmptyList()
        {
            // Act
            var result = _combatLogic.GetOnlyAggroCards(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region RemoveCantBeAttackedCards Tests

        [Test]
        public void RemoveCantBeAttackedCards_RemovesProtectedCards()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal", 3, 3),
                CombatCardInfo.Create("Protected", 4, 4, cantBeAttacked: true),
                CombatCardInfo.Create("Another", 2, 2)
            };

            // Act
            var result = _combatLogic.RemoveCantBeAttackedCards(cards);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsFalse(result.Any(c => c.CantBeAttacked));
        }

        [Test]
        public void RemoveCantBeAttackedCards_WithAllProtected_ReturnsEmptyList()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Protected1", 3, 3, cantBeAttacked: true),
                CombatCardInfo.Create("Protected2", 4, 4, cantBeAttacked: true)
            };

            // Act
            var result = _combatLogic.RemoveCantBeAttackedCards(cards);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void RemoveCantBeAttackedCards_WithNullList_ReturnsEmptyList()
        {
            // Act
            var result = _combatLogic.RemoveCantBeAttackedCards(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region ShouldAddPlayerToTarget Tests

        [Test]
        public void ShouldAddPlayerToTarget_WhenNoValidTargets_ReturnsTrue()
        {
            // Act
            var result = _combatLogic.ShouldAddPlayerToTarget(0, hasDirectAttackAbility: false);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldAddPlayerToTarget_WhenHasDirectAttackAbility_ReturnsTrue()
        {
            // Act
            var result = _combatLogic.ShouldAddPlayerToTarget(3, hasDirectAttackAbility: true);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldAddPlayerToTarget_WhenHasTargetsAndNoDirectAttack_ReturnsFalse()
        {
            // Act
            var result = _combatLogic.ShouldAddPlayerToTarget(3, hasDirectAttackAbility: false);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region BuildValidTargets Tests

        [Test]
        public void BuildValidTargets_WithAggroCards_ReturnsOnlyAggroCards()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal", 3, 3),
                CombatCardInfo.Create("Aggro", 4, 4, aggro: true),
                CombatCardInfo.Create("Another", 2, 2)
            };

            // Act
            var result = _combatLogic.BuildValidTargets(cards, attackerCanDirectAttack: false, hasDirectAttackEffect: false);

            // Assert
            Assert.IsTrue(result.HasAggroCards);
            Assert.AreEqual(1, result.ValidTargets.Count);
            Assert.AreEqual("Aggro", result.ValidTargets[0].Title);
            Assert.IsFalse(result.IncludesPlayer);
        }

        [Test]
        public void BuildValidTargets_WithEmptyField_IncludesPlayer()
        {
            // Arrange
            var cards = new List<CombatCardInfo>();

            // Act
            var result = _combatLogic.BuildValidTargets(cards, attackerCanDirectAttack: false, hasDirectAttackEffect: false);

            // Assert
            Assert.IsTrue(result.IncludesPlayer);
            Assert.AreEqual(0, result.ValidTargets.Count);
        }

        [Test]
        public void BuildValidTargets_WithNullCards_IncludesPlayer()
        {
            // Act
            var result = _combatLogic.BuildValidTargets(null, attackerCanDirectAttack: false, hasDirectAttackEffect: false);

            // Assert
            Assert.IsTrue(result.IncludesPlayer);
        }

        [Test]
        public void BuildValidTargets_WithDirectAttackEffect_IncludesPlayer()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal", 3, 3)
            };

            // Act
            var result = _combatLogic.BuildValidTargets(cards, attackerCanDirectAttack: false, hasDirectAttackEffect: true);

            // Assert
            Assert.IsTrue(result.IncludesPlayer);
        }

        [Test]
        public void BuildValidTargets_WithCanDirectAttack_IncludesPlayer()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal", 3, 3)
            };

            // Act
            var result = _combatLogic.BuildValidTargets(cards, attackerCanDirectAttack: true, hasDirectAttackEffect: false);

            // Assert
            Assert.IsTrue(result.IncludesPlayer);
        }

        [Test]
        public void BuildValidTargets_WithProtectedCards_RemovesThem()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Protected", 3, 3, cantBeAttacked: true),
                CombatCardInfo.Create("Normal", 4, 4)
            };

            // Act
            var result = _combatLogic.BuildValidTargets(cards, attackerCanDirectAttack: false, hasDirectAttackEffect: false);

            // Assert
            Assert.AreEqual(1, result.ValidTargets.Count);
            Assert.AreEqual("Normal", result.ValidTargets[0].Title);
        }

        [Test]
        public void BuildValidTargets_WithAllProtected_IncludesPlayer()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Protected1", 3, 3, cantBeAttacked: true),
                CombatCardInfo.Create("Protected2", 4, 4, cantBeAttacked: true)
            };

            // Act
            var result = _combatLogic.BuildValidTargets(cards, attackerCanDirectAttack: false, hasDirectAttackEffect: false);

            // Assert
            Assert.AreEqual(0, result.ValidTargets.Count);
            Assert.IsTrue(result.IncludesPlayer, "Player should be targetable when all cards are protected");
        }

        [Test]
        public void BuildValidTargets_NormalScenario_ReturnsAllCards()
        {
            // Arrange
            var cards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Card1", 3, 3),
                CombatCardInfo.Create("Card2", 4, 4),
                CombatCardInfo.Create("Card3", 2, 2)
            };

            // Act
            var result = _combatLogic.BuildValidTargets(cards, attackerCanDirectAttack: false, hasDirectAttackEffect: false);

            // Assert
            Assert.AreEqual(3, result.ValidTargets.Count);
            Assert.IsFalse(result.IncludesPlayer);
            Assert.IsFalse(result.HasAggroCards);
        }

        #endregion
    }
}

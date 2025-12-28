using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// PlayMode tests for CombatService.
    /// Tests combat validation, targeting, and damage calculation.
    /// Phase: Test coverage for critical paths.
    /// </summary>
    [TestFixture]
    public class CombatServicePlayTests
    {
        private CombatService _combatService;
        private ICardCollectionService _mockCardCollectionService;
        private IPlayerStatusProvider _mockPlayerStatusProvider;
        private PlayerCards _currentPlayerCards;
        private PlayerCards _opponentPlayerCards;
        private PlayerStatus _currentPlayerStatus;
        private PlayerStatus _opponentPlayerStatus;
        private GameObject _testGameObject;
        private Transform _canvas;

        [SetUp]
        public void SetUp()
        {
            // Create test GameObjects
            _testGameObject = new GameObject("TestContainer");
            _canvas = new GameObject("Canvas").transform;

            // Create PlayerCards MonoBehaviours (but don't Start them)
            var currentPlayerObj = new GameObject("CurrentPlayer");
            var opponentPlayerObj = new GameObject("OpponentPlayer");

            // Note: PlayerCards.Start() requires injected dependencies, so we use mocks instead
            _mockCardCollectionService = Substitute.For<ICardCollectionService>();
            _mockPlayerStatusProvider = Substitute.For<IPlayerStatusProvider>();

            // Create mock PlayerStatus (MonoBehaviour required)
            var currentStatusObj = new GameObject("CurrentStatus");
            var opponentStatusObj = new GameObject("OpponentStatus");
            _currentPlayerStatus = currentStatusObj.AddComponent<PlayerStatus>();
            _opponentPlayerStatus = opponentStatusObj.AddComponent<PlayerStatus>();

            _mockPlayerStatusProvider.GetCurrentPlayerStatus().Returns(_currentPlayerStatus);
            _mockPlayerStatusProvider.GetOpponentPlayerStatus().Returns(_opponentPlayerStatus);

            _combatService = new CombatService(
                _mockCardCollectionService,
                _mockPlayerStatusProvider,
                _canvas);
        }

        [TearDown]
        public void TearDown()
        {
            if (_testGameObject != null)
                Object.DestroyImmediate(_testGameObject);
            if (_canvas != null)
                Object.DestroyImmediate(_canvas.gameObject);

            // Clean up all test objects
            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in allObjects)
            {
                if (obj.name.Contains("Player") || obj.name.Contains("Status") || obj.name.Contains("Canvas"))
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }

        #region CanAttackerAttack Tests

        [UnityTest]
        public IEnumerator CanAttackerAttack_WithNullAttacker_ReturnsFalse()
        {
            // Arrange
            _combatService.Attacker = null;

            yield return null;

            // Act
            var result = _combatService.CanAttackerAttack();

            // Assert
            Assert.IsFalse(result);
        }

        [UnityTest]
        public IEnumerator HasAttackerAction_WithNullAttacker_ReturnsFalse()
        {
            // Arrange
            _combatService.Attacker = null;

            yield return null;

            // Act
            var result = _combatService.HasAttackerAction();

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region ComputeDamageAttack Tests

        [UnityTest]
        public IEnumerator ComputeDamageAttack_WithNullAttacker_ReturnsZero()
        {
            // Arrange
            _combatService.Attacker = null;
            _combatService.Opponent = null;

            yield return null;

            // Act
            var damage = _combatService.ComputeDamageAttack();

            // Assert
            Assert.AreEqual(0f, damage);
        }

        [UnityTest]
        public IEnumerator ComputeDamageAttack_WithNullOpponent_ReturnsZero()
        {
            // Arrange
            var mockAttacker = Substitute.For<InGameInvocationCard>();
            _combatService.Attacker = mockAttacker;
            _combatService.Opponent = null;

            yield return null;

            // Act
            var damage = _combatService.ComputeDamageAttack();

            // Assert
            Assert.AreEqual(0f, damage);
        }

        #endregion

        #region BuildValidTargets Tests

        [UnityTest]
        public IEnumerator BuildValidTargets_WithNullAttacker_ReturnsEmptyList()
        {
            // Arrange
            _combatService.Attacker = null;

            yield return null;

            // Act
            var targets = _combatService.BuildValidTargets();

            // Assert
            Assert.IsNotNull(targets);
            Assert.AreEqual(0, targets.Count);
        }

        #endregion

        #region HandleAttack Tests

        [UnityTest]
        public IEnumerator HandleAttack_WithNullAttacker_DoesNothing()
        {
            // Arrange
            _combatService.Attacker = null;
            _combatService.Opponent = null;

            yield return null;

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _combatService.HandleAttack());
        }

        [UnityTest]
        public IEnumerator HandleAttack_WithNullOpponent_DoesNothing()
        {
            // Arrange
            var mockAttacker = Substitute.For<InGameInvocationCard>();
            _combatService.Attacker = mockAttacker;
            _combatService.Opponent = null;

            yield return null;

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _combatService.HandleAttack());
        }

        #endregion

        #region UseSpecialAction Tests

        [UnityTest]
        public IEnumerator UseSpecialAction_WithNullAttacker_DoesNothing()
        {
            // Arrange
            _combatService.Attacker = null;

            yield return null;

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _combatService.UseSpecialAction());
        }

        #endregion

        #region IsSpecialActionPossible Tests

        [UnityTest]
        public IEnumerator IsSpecialActionPossible_WithNullAttacker_ReturnsFalse()
        {
            // Arrange
            _combatService.Attacker = null;

            yield return null;

            // Act
            var result = _combatService.IsSpecialActionPossible();

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region State Management Tests

        [UnityTest]
        public IEnumerator Attacker_CanBeSetAndRetrieved()
        {
            // Arrange
            var mockAttacker = Substitute.For<InGameInvocationCard>();

            yield return null;

            // Act
            _combatService.Attacker = mockAttacker;

            // Assert
            Assert.AreSame(mockAttacker, _combatService.Attacker);
        }

        [UnityTest]
        public IEnumerator Opponent_CanBeSetAndRetrieved()
        {
            // Arrange
            var mockOpponent = Substitute.For<InGameInvocationCard>();

            yield return null;

            // Act
            _combatService.Opponent = mockOpponent;

            // Assert
            Assert.AreSame(mockOpponent, _combatService.Opponent);
        }

        #endregion
    }
}

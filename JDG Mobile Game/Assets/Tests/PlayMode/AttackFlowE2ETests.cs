using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using JDG.PlayMode.Tests.TestHelpers;
using Phase = JDG.Domain.Phase;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// E2E tests for attack flow.
    /// Tests the complete path from selecting attacker to executing attack on opponent.
    /// Focus: Integration between GameStateService, CombatLogic, and target building.
    /// Phase 137: Added after fixing Player entity creation bug.
    /// </summary>
    [TestFixture]
    public class AttackFlowE2ETests
    {
        private GameStateService _gameStateService;
        private TestEventBus _eventBus;
        private TestGameStateRepository _gameStateRepository;
        private ICombatLogic _combatLogic;
        private GameObject _testContainer;

        [SetUp]
        public void SetUp()
        {
            _testContainer = new GameObject("TestContainer");
            _eventBus = new TestEventBus();
            _gameStateRepository = new TestGameStateRepository();
            _gameStateService = new GameStateService(_gameStateRepository, _eventBus);
            _combatLogic = new CombatLogic();
        }

        [TearDown]
        public void TearDown()
        {
            if (_testContainer != null)
                Object.DestroyImmediate(_testContainer);
        }

        #region Attack Phase Entry Tests

        [UnityTest]
        public IEnumerator AttackPhase_CanBeReachedAfterChoosePhase()
        {
            // Arrange - Start at Draw phase
            Assert.AreEqual(Phase.Draw, _gameStateService.CurrentPhase);

            // Act - Transition through phases
            _gameStateService.NextPhase(); // Draw -> Choose
            _gameStateService.NextPhase(); // Choose -> Attack

            yield return null;

            // Assert - Now in Attack phase
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator AttackPhase_Player1Turn1_ShouldBeSkipped()
        {
            // Arrange - Player 1's first turn
            _gameStateService.StartNewTurn(); // Turn 1
            Assert.AreEqual(1, _gameStateService.TurnNumber);
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);

            yield return null;

            // Assert - Attack should be skipped on Turn 1 for Player 1
            Assert.IsTrue(_gameStateService.ShouldSkipAttackPhase,
                "Player 1 should not be able to attack on Turn 1");
        }

        [UnityTest]
        public IEnumerator AttackPhase_Player2Turn1_CanAttack()
        {
            // Arrange - Switch to Player 2's turn
            _gameStateService.StartNewTurn(); // Turn 1
            _gameStateService.HandleEndTurn(); // Switch to Player 2

            yield return null;

            // Assert - Player 2 CAN attack on their first turn
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer);
            Assert.IsFalse(_gameStateService.ShouldSkipAttackPhase,
                "Player 2 should be able to attack on their first turn");
        }

        [UnityTest]
        public IEnumerator AttackPhase_Player1Turn2_CanAttack()
        {
            // Arrange - Complete Turn 1, start Turn 2
            // Note: HandleEndTurn() already calls StartNewTurn() internally
            _gameStateService.StartNewTurn(); // Turn 1
            _gameStateService.HandleEndTurn(); // P1 -> P2, starts Turn 2
            _gameStateService.HandleEndTurn(); // P2 -> P1, starts Turn 3

            yield return null;

            // Assert - Player 1 CAN attack on Turn 3 (after both players have had a turn)
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
            Assert.AreEqual(3, _gameStateService.TurnNumber);
            Assert.IsFalse(_gameStateService.ShouldSkipAttackPhase,
                "Player 1 should be able to attack after Turn 1");
        }

        #endregion

        #region Target Building Tests (CombatLogic)

        [UnityTest]
        public IEnumerator BuildValidTargets_EmptyOpponentField_PlayerIsValidTarget()
        {
            // Arrange - Empty opponent field (no invocation cards)
            var opponentCards = new List<CombatCardInfo>();

            yield return null;

            // Act - Build valid targets
            var result = _combatLogic.BuildValidTargets(
                opponentCards,
                attackerCanDirectAttack: false,
                hasDirectAttackEffect: false);

            // Assert - Player should be a valid target when field is empty
            Assert.IsTrue(result.IncludesPlayer,
                "When opponent has no invocation cards, Player should be a valid target");
            Assert.AreEqual(0, result.ValidTargets.Count,
                "Should have no invocation card targets");
        }

        [UnityTest]
        public IEnumerator BuildValidTargets_WithBlockingCards_PlayerNotTargetable()
        {
            // Arrange - Opponent has blocking cards
            var opponentCards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Blocker1", 3, 4),
                CombatCardInfo.Create("Blocker2", 2, 5)
            };

            yield return null;

            // Act
            var result = _combatLogic.BuildValidTargets(
                opponentCards,
                attackerCanDirectAttack: false,
                hasDirectAttackEffect: false);

            // Assert
            Assert.IsFalse(result.IncludesPlayer,
                "Player should NOT be targetable when blockers exist");
            Assert.AreEqual(2, result.ValidTargets.Count);
        }

        [UnityTest]
        public IEnumerator BuildValidTargets_WithDirectAttack_PlayerAlwaysTargetable()
        {
            // Arrange - Opponent has cards, but attacker can direct attack
            var opponentCards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Blocker", 3, 4)
            };

            yield return null;

            // Act - Attacker has CanDirectAttack ability
            var result = _combatLogic.BuildValidTargets(
                opponentCards,
                attackerCanDirectAttack: true,
                hasDirectAttackEffect: false);

            // Assert
            Assert.IsTrue(result.IncludesPlayer,
                "Direct attack ability should allow targeting Player even with blockers");
            Assert.AreEqual(1, result.ValidTargets.Count,
                "Invocation card should also be targetable");
        }

        [UnityTest]
        public IEnumerator BuildValidTargets_AllCardsProtected_PlayerTargetable()
        {
            // Arrange - All opponent cards have CantBeAttacked
            var opponentCards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Protected1", 3, 3, cantBeAttacked: true),
                CombatCardInfo.Create("Protected2", 4, 4, cantBeAttacked: true)
            };

            yield return null;

            // Act
            var result = _combatLogic.BuildValidTargets(
                opponentCards,
                attackerCanDirectAttack: false,
                hasDirectAttackEffect: false);

            // Assert - Player should be targetable when all cards are protected
            Assert.IsTrue(result.IncludesPlayer,
                "Player should be targetable when all opponent cards are protected");
            Assert.AreEqual(0, result.ValidTargets.Count,
                "Protected cards should not be valid targets");
        }

        [UnityTest]
        public IEnumerator BuildValidTargets_WithAggroCard_MustTargetAggro()
        {
            // Arrange - One aggro card among normal cards
            var opponentCards = new List<CombatCardInfo>
            {
                CombatCardInfo.Create("Normal", 3, 3),
                CombatCardInfo.Create("Aggro", 4, 4, aggro: true),
                CombatCardInfo.Create("Another", 2, 2)
            };

            yield return null;

            // Act
            var result = _combatLogic.BuildValidTargets(
                opponentCards,
                attackerCanDirectAttack: false,
                hasDirectAttackEffect: false);

            // Assert - Only aggro card should be targetable
            Assert.IsTrue(result.HasAggroCards);
            Assert.AreEqual(1, result.ValidTargets.Count);
            Assert.AreEqual("Aggro", result.ValidTargets[0].Title);
            Assert.IsFalse(result.IncludesPlayer,
                "Cannot target Player when aggro card exists");
        }

        #endregion

        #region Damage Calculation Tests

        [UnityTest]
        public IEnumerator ComputeDamage_AttackerStronger_DefenderDestroyed()
        {
            // Arrange
            float attackerAtk = 7f;
            float defenderDef = 3f;

            yield return null;

            // Act
            float damage = _combatLogic.ComputeDamage(attackerAtk, defenderDef);

            // Assert - Negative damage means defender takes lethal
            Assert.Less(damage, 0, "Defender should be destroyed");
            Assert.AreEqual(-4f, damage, "Damage should be DEF - ATK = 3 - 7 = -4");
        }

        [UnityTest]
        public IEnumerator ComputeDamage_DefenderStronger_AttackerDestroyed()
        {
            // Arrange
            float attackerAtk = 2f;
            float defenderDef = 5f;

            yield return null;

            // Act
            float damage = _combatLogic.ComputeDamage(attackerAtk, defenderDef);

            // Assert - Positive damage means attacker takes lethal
            Assert.Greater(damage, 0, "Attacker should be destroyed");
            Assert.AreEqual(3f, damage, "Damage should be DEF - ATK = 5 - 2 = 3");
        }

        [UnityTest]
        public IEnumerator ComputeDamage_EqualStats_MutualDestruction()
        {
            // Arrange
            float attackerAtk = 4f;
            float defenderDef = 4f;

            yield return null;

            // Act
            float damage = _combatLogic.ComputeDamage(attackerAtk, defenderDef);

            // Assert - Zero means mutual destruction
            Assert.AreEqual(0f, damage, "Equal stats should result in mutual destruction");
        }

        #endregion

        #region Full Attack Flow Integration

        [UnityTest]
        public IEnumerator FullAttackFlow_Phase1Draw_To_Phase2Choose_To_Phase3Attack()
        {
            // Arrange - Start Turn 1, switch to Player 2 who CAN attack
            _gameStateService.StartNewTurn(); // Turn 1
            _gameStateService.HandleEndTurn(); // Switch to Player 2
            _eventBus.PublishedEvents.Clear();

            // Act - Progress through phases (Player 2 can attack on their first turn)
            _gameStateService.NextPhase(); // Draw -> Choose
            Assert.AreEqual(Phase.Choose, _gameStateService.CurrentPhase);

            _gameStateService.NextPhase(); // Choose -> Attack (NOT skipped for Player 2)
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase);

            yield return null;

            // Assert - Events were published
            Assert.IsTrue(_eventBus.HasEvent<PhaseChangedEvent>(),
                "PhaseChangedEvent should be published during transitions");
        }

        [UnityTest]
        public IEnumerator FullAttackFlow_AttackPhase_To_EndPhase()
        {
            // Arrange - Get to attack phase (use Player 2 who can attack)
            _gameStateService.StartNewTurn(); // Turn 1
            _gameStateService.HandleEndTurn(); // Switch to Player 2
            _gameStateService.NextPhase(); // Draw -> Choose
            _gameStateService.NextPhase(); // Choose -> Attack (Player 2 can attack)
            Assert.AreEqual(Phase.Attack, _gameStateService.CurrentPhase);

            yield return null;

            // Act - Move to end phase
            _gameStateService.NextPhase(); // Attack -> End

            // Assert
            Assert.AreEqual(Phase.End, _gameStateService.CurrentPhase);
        }

        [UnityTest]
        public IEnumerator FullTurnCycle_AllPhasesComplete_PlayerSwitches()
        {
            // Arrange
            _gameStateService.StartNewTurn();
            Assert.AreEqual(PlayerId.Player1, _gameStateService.CurrentPlayer);
            int initialTurn = _gameStateService.TurnNumber;

            // Act - Complete full turn
            _gameStateService.NextPhase(); // Draw -> Choose
            _gameStateService.NextPhase(); // Choose -> Attack
            _gameStateService.NextPhase(); // Attack -> End
            _gameStateService.HandleEndTurn(); // End turn, switch player

            yield return null;

            // Assert
            Assert.AreEqual(PlayerId.Player2, _gameStateService.CurrentPlayer,
                "Player should switch after turn ends");
        }

        #endregion

        #region Edge Cases

        [UnityTest]
        public IEnumerator BuildValidTargets_NullCardsList_HandlesGracefully()
        {
            yield return null;

            // Act
            var result = _combatLogic.BuildValidTargets(
                null,
                attackerCanDirectAttack: false,
                hasDirectAttackEffect: false);

            // Assert
            Assert.IsNotNull(result, "Should return a valid result even with null input");
            Assert.IsTrue(result.IncludesPlayer, "Player should be targetable with null cards");
            Assert.AreEqual(0, result.ValidTargets.Count);
        }

        [UnityTest]
        public IEnumerator BuildValidTargets_NullCardsInList_SkipsNulls()
        {
            // Arrange - List with nulls
            var opponentCards = new List<CombatCardInfo>
            {
                null,
                CombatCardInfo.Create("Valid", 3, 3),
                null
            };

            yield return null;

            // Act
            var result = _combatLogic.BuildValidTargets(
                opponentCards,
                attackerCanDirectAttack: false,
                hasDirectAttackEffect: false);

            // Assert
            Assert.AreEqual(1, result.ValidTargets.Count, "Should filter out null cards");
            Assert.AreEqual("Valid", result.ValidTargets[0].Title);
        }

        #endregion
    }
}

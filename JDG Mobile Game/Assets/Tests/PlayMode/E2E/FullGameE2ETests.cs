using System.Collections;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.PlayMode.Tests.Assertions;
using JDG.PlayMode.Tests.Controllers;
using JDG.PlayMode.Tests.Fixtures;
using JDG.PlayMode.Tests.TestHelpers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace JDG.PlayMode.Tests.E2E
{
    /// <summary>
    /// End-to-end tests for complete game flows.
    /// Tests the full game lifecycle from start to win/lose conditions.
    ///
    /// Run with: Tests > Run Full Game E2E Tests (Ctrl+Shift+G)
    /// Or filter by categories: E2E, FullGame
    /// </summary>
    [TestFixture]
    [Category("E2E")]
    [Category("FullGame")]
    public class FullGameE2ETests
    {
        private GameTestController _controller;
        private PlayerActionSimulator _simulator;
        private GameStateSnapshot _lastSnapshot;

        #region Setup / Teardown

        [SetUp]
        public void SetUp()
        {
            _controller = new GameTestController();
            _controller.Initialize();
            _simulator = new PlayerActionSimulator(_controller);
        }

        [TearDown]
        public void TearDown()
        {
            // Capture state on failure for debugging
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                if (_controller != null && _controller.IsInitialized)
                {
                    _lastSnapshot = _controller.CaptureState();
                    Debug.LogError($"Test Failed. State at failure:\n{_lastSnapshot?.ToDetailedString()}");
                    Debug.LogError($"Event History:\n{_controller.EventBus?.GetEventHistoryReport()}");
                }
            }

            _controller?.Dispose();
            _controller = null;
            _simulator = null;
        }

        #endregion

        #region Win/Lose Condition Tests

        [UnityTest]
        public IEnumerator Player_Wins_ByReducingOpponentHealthToZero()
        {
            // Arrange - Setup game with low opponent health
            var config = E2EGameFixtures.DefaultGame();
            config.Player2StartHealth = 10f;
            _controller.SetupGame(config);

            // Setup direct attack scenario
            E2EGameFixtures.SetupDirectAttackScenario(_simulator);

            yield return null;

            // Act - Player 1 attacks directly for 7 damage (StrongAttacker)
            _simulator.SelectAttacker("StrongAttacker", isPlayer1: true);
            yield return _simulator.ExecuteDirectAttack(isPlayer1Attacking: true);

            // First attack: 10 - 7 = 3 HP remaining
            GameStateAssert.PlayerHealthEquals(
                _controller.PlayerStatusProvider.OpponentPlayerStatus,
                3f,
                0.01f);

            // Second direct attack
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.StrongAttacker, isPlayer1: true);
            _simulator.SelectAttacker("StrongAttacker", isPlayer1: true);
            yield return _simulator.ExecuteDirectAttack(isPlayer1Attacking: true);

            // Assert - Opponent health should be <= 0
            GameStateAssert.PlayerHealthLessThanOrEqual(
                _controller.PlayerStatusProvider.OpponentPlayerStatus,
                0f);
        }

        [UnityTest]
        public IEnumerator Player_TakesDirectDamage_WhenFieldIsEmpty()
        {
            // Arrange
            var config = E2EGameFixtures.DirectAttackGame();
            _controller.SetupGame(config);

            // Only player 1 has a card, player 2 has empty field
            E2EGameFixtures.SetupDirectAttackScenario(_simulator);

            float initialHealth = _controller.PlayerStatusProvider.OpponentPlayerStatus.CurrentHealth;

            yield return null;

            // Act - Attack with StrongAttacker (7 ATK)
            _simulator.SelectAttacker("StrongAttacker", isPlayer1: true);
            yield return _simulator.ExecuteDirectAttack(isPlayer1Attacking: true);

            // Assert - Player 2 took 7 damage
            float expectedHealth = initialHealth - 7f;
            GameStateAssert.PlayerHealthEquals(
                _controller.PlayerStatusProvider.OpponentPlayerStatus,
                expectedHealth,
                0.01f);

            // Verify event was published
            GameStateAssert.EventWasPublished<PlayerHealthChangedEvent>(_controller.EventBus);
        }

        #endregion

        #region Phase Transition Tests

        [UnityTest]
        public IEnumerator FullTurn_AllPhasesExecuteInOrder()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Assert initial state
            GameStateAssert.IsInPhase(_controller.GameStateService, Phase.Draw, "Initial phase should be Draw");
            GameStateAssert.IsTurnNumber(_controller.GameStateService, 1);

            // Act - Progress through phases
            _controller.GameStateService.NextPhase(); // Draw -> Choose
            yield return null;
            GameStateAssert.IsInPhase(_controller.GameStateService, Phase.Choose, "After NextPhase from Draw");

            _controller.GameStateService.NextPhase(); // Choose -> Attack (skipped for P1 T1) or End
            yield return null;

            // Player 1 Turn 1 skips Attack phase
            if (_controller.GameStateService.ShouldSkipAttackPhase)
            {
                GameStateAssert.IsInPhase(_controller.GameStateService, Phase.End, "P1 T1 should skip to End");
            }
            else
            {
                GameStateAssert.IsInPhase(_controller.GameStateService, Phase.Attack, "Should be in Attack phase");
                _controller.GameStateService.NextPhase(); // Attack -> End
                yield return null;
                GameStateAssert.IsInPhase(_controller.GameStateService, Phase.End, "After Attack should be End");
            }
        }

        [UnityTest]
        public IEnumerator Player1Turn1_AttackPhaseIsSkipped()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Assert - Player 1, Turn 1
            GameStateAssert.IsPlayerTurn(_controller.GameStateService, PlayerId.Player1);
            GameStateAssert.IsTurnNumber(_controller.GameStateService, 1);
            GameStateAssert.AttackPhaseShouldBeSkipped(_controller.GameStateService);
        }

        [UnityTest]
        public IEnumerator Player2Turn1_CanAttack()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Act - Complete Player 1's turn
            yield return _controller.EndTurn();

            // Assert - Player 2 can attack
            GameStateAssert.IsPlayerTurn(_controller.GameStateService, PlayerId.Player2);
            GameStateAssert.AttackPhaseNotSkipped(_controller.GameStateService);
        }

        [UnityTest]
        public IEnumerator Player1Turn2_CanAttack()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Act - Complete Turn 1 for both players
            yield return _controller.EndTurn(); // P1 -> P2
            yield return _controller.EndTurn(); // P2 -> P1 (Turn 2)

            // Assert - Player 1 can now attack
            GameStateAssert.IsPlayerTurn(_controller.GameStateService, PlayerId.Player1);
            GameStateAssert.AttackPhaseNotSkipped(_controller.GameStateService);
        }

        [UnityTest]
        public IEnumerator EndTurn_SwitchesPlayer()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());
            GameStateAssert.IsPlayerTurn(_controller.GameStateService, PlayerId.Player1);

            yield return null;

            // Act
            yield return _controller.EndTurn();

            // Assert
            GameStateAssert.IsPlayerTurn(_controller.GameStateService, PlayerId.Player2);
        }

        #endregion

        #region Combat Scenario Tests

        [UnityTest]
        public IEnumerator Combat_StrongerAttacker_DestroysDefender()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            // Strong attacker (7/5) vs Weak defender (2/5)
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.StrongAttacker, isPlayer1: true);
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.WeakDefender, isPlayer1: false);

            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("StrongAttacker"));
            Assert.IsTrue(_simulator.IsCardOnPlayer2Field("WeakDefender"));

            yield return null;

            // Act - Attack
            _simulator.SelectAttacker("StrongAttacker", isPlayer1: true);
            yield return _simulator.AttackTarget("WeakDefender", isPlayer1Attacking: true);

            // Assert - Defender destroyed, attacker survives
            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("StrongAttacker"), "Attacker should survive");
            Assert.IsFalse(_simulator.IsCardOnPlayer2Field("WeakDefender"), "Defender should be destroyed");

            // Verify CardDestroyedEvent was published
            GameStateAssert.EventWasPublished<CardDestroyedEvent>(_controller.EventBus);
        }

        [UnityTest]
        public IEnumerator Combat_StrongerDefender_DestroysAttacker()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            // Weak attacker (3/3) vs Strong defender (3/8)
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.WeakAttacker, isPlayer1: true);
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.StrongDefender, isPlayer1: false);

            yield return null;

            // Act - Attack
            _simulator.SelectAttacker("WeakAttacker", isPlayer1: true);
            yield return _simulator.AttackTarget("StrongDefender", isPlayer1Attacking: true);

            // Assert - Attacker destroyed, defender survives
            Assert.IsFalse(_simulator.IsCardOnPlayer1Field("WeakAttacker"), "Attacker should be destroyed");
            Assert.IsTrue(_simulator.IsCardOnPlayer2Field("StrongDefender"), "Defender should survive");
        }

        [UnityTest]
        public IEnumerator Combat_EqualStats_MutualDestruction()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            // Card with 5 ATK vs Card with 5 DEF
            var attacker = E2EGameFixtures.CreateAttacker("Attacker5", 5f, 4f);
            var defender = E2EGameFixtures.CreateDefender("Defender5", 3f, 5f);

            yield return _simulator.PlayInvocationCard(attacker, isPlayer1: true);
            yield return _simulator.PlayInvocationCard(defender, isPlayer1: false);

            yield return null;

            // Act - Attack (5 ATK vs 5 DEF = 0 damage = mutual destruction)
            _simulator.SelectAttacker("Attacker5", isPlayer1: true);
            yield return _simulator.AttackTarget("Defender5", isPlayer1Attacking: true);

            // Assert - Both destroyed
            Assert.IsFalse(_simulator.IsCardOnPlayer1Field("Attacker5"), "Attacker should be destroyed");
            Assert.IsFalse(_simulator.IsCardOnPlayer2Field("Defender5"), "Defender should be destroyed");

            // Two CardDestroyedEvents should be published
            Assert.AreEqual(2, _controller.EventBus.CountEvents<CardDestroyedEvent>(),
                "Two cards should be destroyed");
        }

        [UnityTest]
        public IEnumerator Combat_EmptyField_AllowsDirectAttack()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DirectAttackGame());

            // Only player 1 has a card
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act - Get valid targets
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Player entity is a valid target
            Assert.AreEqual(1, targets.Count, "Should have exactly one target (player)");
            Assert.IsTrue(targets[0].IsPlayerEntity, "Target should be the player entity");
        }

        #endregion

        #region Aggro and Protection Tests

        [UnityTest]
        public IEnumerator Combat_AggroCard_MustBeAttackedFirst()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // Player 2 has aggro card and regular card
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.AggroCard, isPlayer1: false);
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.WeakDefender, isPlayer1: false);

            yield return null;

            // Act - Get valid targets
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Only aggro card is targetable
            Assert.AreEqual(1, targets.Count, "Only aggro card should be targetable");
            Assert.AreEqual("AggroCard", targets[0].Title, "Aggro card must be attacked first");
        }

        [UnityTest]
        public IEnumerator Combat_ProtectedCard_CannotBeTargeted()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // Player 2 has only protected cards
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.ProtectedCard, isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act - Get valid targets
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Protected card not targetable, player is
            Assert.AreEqual(1, targets.Count, "Only player should be targetable");
            Assert.IsTrue(targets[0].IsPlayerEntity, "Protected cards force direct attack");
        }

        [UnityTest]
        public IEnumerator Combat_DirectAttackAbility_BypassesBlockers()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            // Direct attacker can bypass blockers
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.DirectAttackerCard, isPlayer1: true);

            // Opponent has blockers
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.WeakDefender, isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act - Get valid targets
            _simulator.SelectAttacker("DirectAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Both blocker and player are valid targets
            Assert.AreEqual(2, targets.Count, "Direct attacker can target both blocker and player");
        }

        #endregion

        #region Multi-Turn Tests

        [UnityTest]
        public IEnumerator MultiTurn_5Rounds_GameProgressesCorrectly()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Play 5 full rounds (10 turns total)
            for (int round = 1; round <= 5; round++)
            {
                // Player 1 turn
                yield return _controller.SimulateTurn(TurnActions.PassTurn);

                // Player 2 turn
                yield return _controller.SimulateTurn(TurnActions.PassTurn);
            }

            // Assert - Should be at turn 11, Player 1's turn
            // Turn count: P1T1, P2T2, P1T3, P2T4, P1T5, P2T6, P1T7, P2T8, P1T9, P2T10, P1T11
            GameStateAssert.IsPlayerTurn(_controller.GameStateService, PlayerId.Player1);
            Assert.GreaterOrEqual(_controller.GameStateService.TurnNumber, 10,
                "Should have progressed through 10+ turns");
        }

        [UnityTest]
        public IEnumerator MultiTurn_PhaseEventsPublished()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());
            _controller.EventBus.ClearEvents();

            yield return null;

            // Act - Complete one full turn
            yield return _controller.SimulateTurn(TurnActions.PassTurn);

            // Assert - Phase changed events were published
            Assert.Greater(_controller.EventBus.CountEvents<PhaseChangedEvent>(), 0,
                "Phase change events should be published during turn");
        }

        #endregion

        #region Damage Calculation Tests

        [UnityTest]
        public IEnumerator DamageCalculation_AttackerWins_NegativeDamage()
        {
            // Arrange
            yield return null;

            // Act - 7 ATK vs 3 DEF
            float damage = _controller.ComputeDamage(7f, 3f);

            // Assert - Negative damage means defender loses
            Assert.Less(damage, 0f, "Damage should be negative when attacker wins");
            Assert.AreEqual(-4f, damage, "Damage should be DEF - ATK = 3 - 7 = -4");
        }

        [UnityTest]
        public IEnumerator DamageCalculation_DefenderWins_PositiveDamage()
        {
            // Arrange
            yield return null;

            // Act - 3 ATK vs 7 DEF
            float damage = _controller.ComputeDamage(3f, 7f);

            // Assert - Positive damage means attacker loses
            Assert.Greater(damage, 0f, "Damage should be positive when defender wins");
            Assert.AreEqual(4f, damage, "Damage should be DEF - ATK = 7 - 3 = 4");
        }

        [UnityTest]
        public IEnumerator DamageCalculation_Tie_ZeroDamage()
        {
            // Arrange
            yield return null;

            // Act - 5 ATK vs 5 DEF
            float damage = _controller.ComputeDamage(5f, 5f);

            // Assert - Zero damage means tie (mutual destruction)
            Assert.AreEqual(0f, damage, "Equal stats should result in zero damage (mutual destruction)");
        }

        #endregion
    }
}

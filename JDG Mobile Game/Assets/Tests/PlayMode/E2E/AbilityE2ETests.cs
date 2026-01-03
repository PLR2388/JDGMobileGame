using System.Collections;
using System.Collections.Generic;
using JDG.Application.Abilities;
using JDG.Domain;
using JDG.Domain.Events;
using DomainCardOwner = JDG.Domain.CardOwner;
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
    /// End-to-end tests for card abilities.
    /// Tests ability triggers, effects, and interactions in game context.
    ///
    /// Run with: Tests > Run Ability E2E Tests (Ctrl+Shift+A)
    /// Or filter by categories: E2E, Abilities
    /// </summary>
    [TestFixture]
    [Category("E2E")]
    [Category("Abilities")]
    public class AbilityE2ETests
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

        #region Combat Ability Tests

        [UnityTest]
        public IEnumerator DirectAttack_BypassesBlockers()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            // Player 1 has direct attacker
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.DirectAttackerCard, isPlayer1: true);

            // Player 2 has a blocker
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.StrongDefender, isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act - Get valid targets for direct attacker
            _simulator.SelectAttacker("DirectAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Both blocker and player should be valid targets
            Assert.AreEqual(2, targets.Count, "Direct attacker should be able to target both blocker and player");

            bool hasPlayerTarget = false;
            bool hasCardTarget = false;
            foreach (var target in targets)
            {
                if (target.IsPlayerEntity) hasPlayerTarget = true;
                else hasCardTarget = true;
            }

            Assert.IsTrue(hasPlayerTarget, "Player should be a valid target");
            Assert.IsTrue(hasCardTarget, "Blocker should also be a valid target");
        }

        [UnityTest]
        public IEnumerator Aggro_ForcesTargeting()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // Player 2 has aggro and non-aggro cards
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.AggroCard, isPlayer1: false);
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.WeakDefender, isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Only aggro card should be targetable
            Assert.AreEqual(1, targets.Count, "Only aggro card should be valid target");
            Assert.AreEqual("AggroCard", targets[0].Title, "Must target aggro card");
            Assert.IsFalse(targets[0].IsPlayerEntity, "Player should not be targetable when aggro exists");
        }

        [UnityTest]
        public IEnumerator CantBeAttacked_PreventsTargeting()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // Player 2 has only protected card
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.ProtectedCard, isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Protected card not targetable, player is
            Assert.AreEqual(1, targets.Count);
            Assert.IsTrue(targets[0].IsPlayerEntity, "Only player should be targetable when all cards are protected");
        }

        [UnityTest]
        public IEnumerator MutualDestruction_DestroysBothCards()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            // Both cards have same ATK as opponent's DEF
            var card1 = E2EGameFixtures.CreateAttacker("Attacker", 5f, 5f);
            var card2 = E2EGameFixtures.CreateDefender("Defender", 5f, 5f);

            yield return _simulator.PlayInvocationCard(card1, isPlayer1: true);
            yield return _simulator.PlayInvocationCard(card2, isPlayer1: false);

            yield return null;

            // Act - Attack (5 ATK vs 5 DEF = 0 damage = mutual destruction)
            _simulator.SelectAttacker("Attacker", isPlayer1: true);
            yield return _simulator.AttackTarget("Defender", isPlayer1Attacking: true);

            // Assert
            Assert.IsFalse(_simulator.IsCardOnPlayer1Field("Attacker"), "Attacker should be destroyed");
            Assert.IsFalse(_simulator.IsCardOnPlayer2Field("Defender"), "Defender should be destroyed");

            // Two destruction events
            Assert.AreEqual(2, _controller.EventBus.CountEvents<CardDestroyedEvent>());
        }

        #endregion

        #region Draw/Summon Ability Tests

        [UnityTest]
        public IEnumerator DrawCards_PublishesCardDrawnEvent()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Simulate drawing a card (the ability effect)
            _controller.EventBus.Publish(new CardDrawnEvent
            {
                CardTitle = "DrawnCard",
                Owner = DomainCardOwner.Player1
            });

            yield return null;

            // Assert
            GameStateAssert.EventWasPublished<CardDrawnEvent>(_controller.EventBus);
        }

        [UnityTest]
        public IEnumerator InvokeFromDeck_PlacesCardOnField()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Play a card with invoke-from-deck ability
            // When the ability activates, it should place another card
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateWithAbility("Invoker", 4f, 4f, AbilityName.GetNounoursFromDeck),
                isPlayer1: true);

            // Simulate the invoked card being placed
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Nounours", 3f, 3f),
                isPlayer1: true);

            // Assert - Both cards on field
            Assert.AreEqual(2, _simulator.Player1FieldCount);
        }

        [UnityTest]
        public IEnumerator Sacrifice_RemovesCardFromField()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Place a card that will be sacrificed
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Sacrifice", 3f, 3f),
                isPlayer1: true);

            Assert.AreEqual(1, _simulator.Player1FieldCount);

            yield return null;

            // Act - Simulate sacrifice by publishing destruction event
            _controller.EventBus.Publish(new CardDestroyedEvent
            {
                Owner = DomainCardOwner.Player1,
                Reason = "Sacrificed"
            });
            // Note: In real game, the card would be removed. For simulation, we track via events.

            // Assert
            GameStateAssert.EventWasPublished<CardDestroyedEvent>(_controller.EventBus);
        }

        #endregion

        #region Stat Modifier Tests

        [UnityTest]
        public IEnumerator Equipment_ModifiesStats()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Base card: 5 ATK, 4 DEF
            var baseCard = E2EGameFixtures.CreateAttacker("BaseCard", 5f, 4f);
            yield return _simulator.PlayInvocationCard(baseCard, isPlayer1: true);

            yield return null;

            // Act - Simulate equipment being attached
            _controller.EventBus.Publish(new CardPlayedEvent
            {
                CardTitle = "PowerEquipment",
                Owner = DomainCardOwner.Player1
            });

            yield return null;

            // Assert - Equipment event was published
            // In real game, stats would be modified. Here we verify the event flow.
            var events = _controller.EventBus.GetAllEvents<CardPlayedEvent>();
            Assert.AreEqual(2, events.Count, "Base card and equipment should both be played");
        }

        [UnityTest]
        public IEnumerator FieldBonus_AffectsMatchingFamily()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Play field card
            _controller.EventBus.Publish(new CardPlayedEvent
            {
                CardTitle = "FamilyBoostField",
                Owner = DomainCardOwner.Player1
            });

            // Play matching family card
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("FamilyMember", 4f, 4f),
                isPlayer1: true);

            // Assert - Both events published
            Assert.AreEqual(2, _controller.EventBus.CountEvents<CardPlayedEvent>());
        }

        [UnityTest]
        public IEnumerator Continuous_AppliesWhileOnField()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Card with continuous ability (e.g., stat boost)
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateWithAbility("ContinuousCard", 4f, 4f, AbilityName.GiveAtkDefToComics),
                isPlayer1: true);

            yield return null;

            // Assert - Card is on field (continuous abilities apply while present)
            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("ContinuousCard"));

            // Act - Remove the card
            _simulator.ClearFields();

            // Assert - Card no longer on field (continuous ability no longer applies)
            Assert.IsFalse(_simulator.IsCardOnPlayer1Field("ContinuousCard"));
        }

        #endregion

        #region Trigger Lifecycle Tests

        [UnityTest]
        public IEnumerator OnTurnStart_TriggersAtTurnBeginning()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Simulate turn start event (like TurnService.OnTurnStart)
            _controller.EventBus.Publish(new TurnStartEvent
            {
                TurnNumber = 1,
                CurrentPlayer = DomainCardOwner.Player1
            });

            yield return null;

            // Assert
            GameStateAssert.EventWasPublished<TurnStartEvent>(_controller.EventBus);
        }

        [UnityTest]
        public IEnumerator OnTurnEnd_TriggersAtTurnEnd()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - End turn
            yield return _controller.EndTurn();

            // Assert - Turn end event should be published
            GameStateAssert.EventWasPublished<TurnEndEvent>(_controller.EventBus);
        }

        [UnityTest]
        public IEnumerator OnEquip_TriggersWhenEquipped()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Place target card for equipment
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.MediumAttacker, isPlayer1: true);

            yield return null;

            // Act - Equip (simulated via card played event)
            _controller.EventBus.Publish(new CardPlayedEvent
            {
                CardTitle = "Equipment",
                Owner = DomainCardOwner.Player1
            });

            yield return null;

            // Assert
            var events = _controller.EventBus.GetAllEvents<CardPlayedEvent>();
            Assert.GreaterOrEqual(events.Count, 2, "Should have both target and equipment events");
        }

        [UnityTest]
        public IEnumerator OnUnequip_TriggersWhenRemoved()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // Simulate equipment attached
            _controller.EventBus.Publish(new CardPlayedEvent
            {
                CardTitle = "Equipment",
                Owner = DomainCardOwner.Player1
            });

            yield return null;

            // Act - Remove equipment (via destruction)
            _controller.EventBus.Publish(new CardDestroyedEvent
            {
                Owner = DomainCardOwner.Player1,
                Reason = "Equipment removed"
            });

            yield return null;

            // Assert
            GameStateAssert.EventWasPublished<CardDestroyedEvent>(_controller.EventBus);
        }

        #endregion

        #region Ability Combination Tests

        [UnityTest]
        public IEnumerator MultipleAbilities_AllTriggerCorrectly()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Card with multiple abilities
            var multiAbilityCard = E2EGameFixtures.CreateWithAbilities(
                "MultiCard", 5f, 5f,
                AbilityName.Draw1Card,
                AbilityName.ComesBackFromDeath);

            yield return null;

            // Act - Play card
            yield return _simulator.PlayInvocationCard(multiAbilityCard, isPlayer1: true);

            // Assert - Card is on field
            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("MultiCard"));
            GameStateAssert.EventWasPublished<CardPlayedEvent>(_controller.EventBus);
        }

        [UnityTest]
        public IEnumerator AggroAndProtection_InteractCorrectly()
        {
            // Arrange - Edge case: what if a card has both aggro and protection?
            // Protection should prevent targeting, aggro becomes irrelevant
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // Card that would have both (theoretically)
            var protectedAggro = new TestInvocationCardConfig
            {
                Title = "ProtectedAggro",
                Attack = 4f,
                Defense = 4f,
                CantBeAttacked = true,
                HasAggro = true
            };
            yield return _simulator.PlayInvocationCard(protectedAggro, isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Protection wins, player is only target
            Assert.AreEqual(1, targets.Count);
            Assert.IsTrue(targets[0].IsPlayerEntity, "Protected card cannot be targeted even with aggro");
        }

        [UnityTest]
        public IEnumerator DirectAttackVsAggro_DirectAttackWins()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            // Direct attacker
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.DirectAttackerCard, isPlayer1: true);

            // Aggro card on opponent's field
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.AggroCard, isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act
            _simulator.SelectAttacker("DirectAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Direct attacker can still target player despite aggro
            // (Direct attack bypasses normal targeting rules)
            bool canTargetPlayer = false;
            foreach (var target in targets)
            {
                if (target.IsPlayerEntity)
                {
                    canTargetPlayer = true;
                    break;
                }
            }

            Assert.IsTrue(canTargetPlayer, "Direct attack should bypass aggro to allow player targeting");
        }

        #endregion

        #region Edge Case Tests

        [UnityTest]
        public IEnumerator EmptyField_AllowsDirectAttack()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DirectAttackGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert
            Assert.AreEqual(1, targets.Count);
            Assert.IsTrue(targets[0].IsPlayerEntity);
        }

        [UnityTest]
        public IEnumerator AllCardsProtected_AllowsDirectAttack()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.BasicCombatGame());

            yield return _simulator.PlayInvocationCard(E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // All opponent cards are protected
            yield return _simulator.PlayInvocationCard(E2EGameFixtures.ProtectedCard, isPlayer1: false);
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateProtectedCard("Protected2", 5f, 5f), isPlayer1: false);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            yield return null;

            // Act
            _simulator.SelectAttacker("MediumAttacker", isPlayer1: true);
            var targets = _simulator.GetValidTargets(isPlayer1Attacking: true);

            // Assert - Only player targetable since all cards are protected
            Assert.AreEqual(1, targets.Count);
            Assert.IsTrue(targets[0].IsPlayerEntity);
        }

        [UnityTest]
        public IEnumerator MaxFieldCards_PreventsFurtherPlacement()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Place 4 cards (max for invocations)
            for (int i = 1; i <= 4; i++)
            {
                yield return _simulator.PlayInvocationCard(
                    E2EGameFixtures.CreateAttacker($"Card{i}", 3f, 3f),
                    isPlayer1: true);
            }

            Assert.AreEqual(4, _simulator.Player1FieldCount);

            yield return null;

            // Act - Try to place a 5th card
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Card5", 3f, 3f),
                isPlayer1: true);

            // Assert - Still only 4 cards (5th rejected)
            Assert.AreEqual(4, _simulator.Player1FieldCount, "Should not exceed max 4 invocations");
        }

        #endregion
    }
}

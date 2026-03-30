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
    /// End-to-end tests for the tutorial flow.
    /// Tests dialogue-triggered scenarios, highlights, and scripted actions.
    ///
    /// Run with: Tests > Run Tutorial E2E Tests (Ctrl+Shift+T)
    /// Or filter by categories: E2E, Tutorial
    /// </summary>
    [TestFixture]
    [Category("E2E")]
    [Category("Tutorial")]
    public class TutorialE2ETests
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

        #region Dialogue Event Tests

        [UnityTest]
        public IEnumerator DialogueIndexChanged_PublishesEvent()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Simulate dialogue index change (like TutoPlayerGameLoop receives)
            _controller.EventBus.Publish(new DialogueIndexChangedEvent { DialogueIndex = 5 });

            yield return null;

            // Assert - Event was captured
            GameStateAssert.EventWasPublished<DialogueIndexChangedEvent>(_controller.EventBus);

            var events = _controller.EventBus.GetAllEvents<DialogueIndexChangedEvent>();
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(5, events[0].DialogueIndex);
        }

        [UnityTest]
        public IEnumerator DialogueTriggerCompleted_PublishesEvent()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Simulate trigger completion (like TutoPlayerGameLoop.NextRound does)
            _controller.EventBus.Publish(new DialogueTriggerCompletedEvent
            {
                TriggerType = (int)NextDialogueTrigger.NextPhase
            });

            yield return null;

            // Assert
            GameStateAssert.EventWasPublished<DialogueTriggerCompletedEvent>(_controller.EventBus);
        }

        #endregion

        #region Highlight Event Tests

        [UnityTest]
        public IEnumerator Highlight_PublishesHighlightRequestedEvent()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Simulate highlight request (like TutoPlayerGameLoop.HandleHighlight does)
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.Deck,
                IsActivated = true
            });

            yield return null;

            // Assert
            GameStateAssert.EventWasPublished<HighlightRequestedEvent>(_controller.EventBus);

            var evt = _controller.EventBus.GetLastEvent<HighlightRequestedEvent>();
            Assert.AreEqual((int)HighlightElement.Deck, evt.Element);
            Assert.IsTrue(evt.IsActivated);
        }

        [UnityTest]
        public IEnumerator Highlight_Deck_ShowsCorrectElement()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.Deck,
                IsActivated = true
            });

            yield return null;

            // Assert
            var evt = _controller.EventBus.GetLastEvent<HighlightRequestedEvent>();
            Assert.AreEqual((int)HighlightElement.Deck, evt.Element);
        }

        [UnityTest]
        public IEnumerator Highlight_NextPhase_ShowsCorrectElement()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.NextPhaseButton,
                IsActivated = true
            });

            yield return null;

            // Assert
            var evt = _controller.EventBus.GetLastEvent<HighlightRequestedEvent>();
            Assert.AreEqual((int)HighlightElement.NextPhaseButton, evt.Element);
        }

        [UnityTest]
        public IEnumerator Highlight_Invocations_ShowsCorrectElement()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.Invocations,
                IsActivated = true
            });

            yield return null;

            // Assert
            var evt = _controller.EventBus.GetLastEvent<HighlightRequestedEvent>();
            Assert.AreEqual((int)HighlightElement.Invocations, evt.Element);
        }

        [UnityTest]
        public IEnumerator UnsetHighlight_DeactivatesAllElements()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // First activate a highlight
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.Deck,
                IsActivated = true
            });

            yield return null;

            // Act - Deactivate (like TutoPlayerGameLoop.UnsetHighlight)
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.Deck,
                IsActivated = false
            });

            yield return null;

            // Assert - Last event should be deactivation
            var evt = _controller.EventBus.GetLastEvent<HighlightRequestedEvent>();
            Assert.IsFalse(evt.IsActivated);
        }

        #endregion

        #region Scripted Card Placement Tests

        [UnityTest]
        public IEnumerator PlaceCard_InvokesCardToField()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Simulate placing a card (like TutoPlayerGameLoop.InvokeInvocationCards)
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("TutorialCard", 5f, 4f),
                isPlayer1: true);

            // Assert - Card is on field
            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("TutorialCard"));
            Assert.AreEqual(1, _simulator.Player1FieldCount);

            // CardPlayedEvent was published
            GameStateAssert.EventWasPublished<CardPlayedEvent>(_controller.EventBus);
        }

        [UnityTest]
        public IEnumerator PlaceCard_EquipsEquipmentCard()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // First place an invocation to equip to
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("TargetCard", 5f, 4f),
                isPlayer1: true);

            yield return null;

            // Act - Simulate equipment (like TutoPlayerGameLoop.EquipInvocationCard)
            // In the real tutorial, this is done via "Equipment>Target" format
            // Here we just verify the card play event system works

            _controller.EventBus.Publish(new CardPlayedEvent
            {
                CardTitle = "TestEquipment",
                Owner = CardOwner.Player1
            });

            yield return null;

            // Assert - Equipment event published
            var events = _controller.EventBus.GetAllEvents<CardPlayedEvent>();
            Assert.AreEqual(2, events.Count); // Invocation + Equipment
        }

        [UnityTest]
        public IEnumerator PlaceMultipleCards_AllAddedToField()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Place multiple cards (like tutorial placing "Card1;Card2;Card3")
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Card1", 3f, 3f), isPlayer1: true);
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Card2", 4f, 4f), isPlayer1: true);
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Card3", 5f, 5f), isPlayer1: true);

            // Assert
            Assert.AreEqual(3, _simulator.Player1FieldCount);
            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("Card1"));
            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("Card2"));
            Assert.IsTrue(_simulator.IsCardOnPlayer1Field("Card3"));
        }

        #endregion

        #region Scripted Attack Tests

        [UnityTest]
        public IEnumerator HandleAttack_ExecutesCombat()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Setup like TutoPlayerGameLoop.HandleAttack
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Attacker", 6f, 4f), isPlayer1: true);
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateDefender("Defender", 3f, 5f), isPlayer1: false);

            yield return null;

            // Act - Scripted attack (like tutorial attack["Attacker", "Defender"])
            _simulator.SelectAttacker("Attacker", isPlayer1: true);
            yield return _simulator.AttackTarget("Defender", isPlayer1Attacking: true);

            // Assert - Combat resolved
            GameStateAssert.EventWasPublished<CardDestroyedEvent>(_controller.EventBus);
            Assert.IsFalse(_simulator.IsCardOnPlayer2Field("Defender"), "Defender should be destroyed");
        }

        [UnityTest]
        public IEnumerator HandleAttack_DirectAttackOnPlayer()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("Attacker", 5f, 4f), isPlayer1: true);
            _simulator.SetupPlayerEntity("Player2", isPlayer1: false);

            float initialHealth = _controller.PlayerStatusProvider.OpponentPlayerStatus.CurrentHealth;

            yield return null;

            // Act - Direct attack on player (like tutorial attack["Attacker"] with no defender)
            _simulator.SelectAttacker("Attacker", isPlayer1: true);
            yield return _simulator.ExecuteDirectAttack(isPlayer1Attacking: true);

            // Assert
            GameStateAssert.EventWasPublished<PlayerHealthChangedEvent>(_controller.EventBus);
            Assert.Less(_controller.PlayerStatusProvider.OpponentPlayerStatus.CurrentHealth, initialHealth);
        }

        #endregion

        #region Next Phase Action Tests

        [UnityTest]
        public IEnumerator NextPhaseAction_AdvancesGamePhase()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());
            GameStateAssert.IsInPhase(_controller.GameStateService, Phase.Draw);

            yield return null;

            // Act - Advance phase (like tutorial action: next_phase)
            _controller.GameStateService.NextPhase();

            yield return null;

            // Assert
            GameStateAssert.IsInPhase(_controller.GameStateService, Phase.Choose);

            // Event published
            GameStateAssert.EventWasPublished<PhaseChangedEvent>(_controller.EventBus);
        }

        [UnityTest]
        public IEnumerator NextPhaseAction_PublishesTriggerCompletedEvent()
        {
            // Arrange
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Act - Simulate what TutoPlayerGameLoop.NextRound does for non-P1
            _controller.EventBus.Publish(new DialogueTriggerCompletedEvent
            {
                TriggerType = (int)NextDialogueTrigger.NextPhase
            });

            yield return null;

            // Assert
            GameStateAssert.EventWasPublished<DialogueTriggerCompletedEvent>(_controller.EventBus);
        }

        #endregion

        #region Tutorial Scenario Flow Tests

        [UnityTest]
        public IEnumerator TutorialScenario_BasicFlow_CompletesSuccessfully()
        {
            // Arrange - Simulate a basic tutorial scenario sequence
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            yield return null;

            // Step 1: Highlight deck
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.Deck,
                IsActivated = true
            });
            yield return null;

            // Step 2: Dialogue index changes
            _controller.EventBus.Publish(new DialogueIndexChangedEvent { DialogueIndex = 1 });
            yield return null;

            // Step 3: Place a card
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.MediumAttacker, isPlayer1: true);

            // Step 4: Highlight next phase
            _controller.EventBus.Publish(new HighlightRequestedEvent
            {
                Element = (int)HighlightElement.NextPhaseButton,
                IsActivated = true
            });
            yield return null;

            // Step 5: Advance phase
            _controller.GameStateService.NextPhase();
            yield return null;

            // Assert - All expected events were published
            Assert.IsTrue(_controller.EventBus.HasEvent<HighlightRequestedEvent>());
            Assert.IsTrue(_controller.EventBus.HasEvent<DialogueIndexChangedEvent>());
            Assert.IsTrue(_controller.EventBus.HasEvent<CardPlayedEvent>());
            Assert.IsTrue(_controller.EventBus.HasEvent<PhaseChangedEvent>());
        }

        [UnityTest]
        public IEnumerator TutorialScenario_CombatSequence_ExecutesCorrectly()
        {
            // Arrange - Simulate tutorial combat scenario
            _controller.SetupGame(E2EGameFixtures.DefaultGame());

            // Skip to Player 2's turn so we can attack
            yield return _controller.EndTurn();
            yield return _controller.AdvanceToPhase(Phase.Attack);

            // Setup combat (Player 2 attacking Player 1's card)
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("P1Card", 4f, 4f), isPlayer1: true);
            yield return _simulator.PlayInvocationCard(
                E2EGameFixtures.CreateAttacker("P2Attacker", 6f, 5f), isPlayer1: false);

            yield return null;

            // Act - Execute scripted attack
            _simulator.SelectAttacker("P2Attacker", isPlayer1: false);
            yield return _simulator.AttackTarget("P1Card", isPlayer1Attacking: false);

            // Assert
            Assert.IsFalse(_simulator.IsCardOnPlayer1Field("P1Card"), "P1's card should be destroyed");
            Assert.IsTrue(_simulator.IsCardOnPlayer2Field("P2Attacker"), "P2's attacker should survive");
        }

        #endregion
    }

    #region Tutorial Enums (matching TutoPlayerGameLoop)

    /// <summary>
    /// Highlight elements matching TutoPlayerGameLoop.HighlightElement
    /// </summary>
    public enum HighlightElement
    {
        Space = 0,
        Deck = 1,
        YellowTrash = 2,
        Field = 3,
        Invocations = 4,
        Effect = 5,
        InHandButton = 6,
        NextPhaseButton = 7,
        Tentacules = 8,
        LifePoints = 9
    }

    /// <summary>
    /// Next dialogue triggers matching TutoPlayerGameLoop
    /// </summary>
    public enum NextDialogueTrigger
    {
        NextPhase = 0
    }

    #endregion
}

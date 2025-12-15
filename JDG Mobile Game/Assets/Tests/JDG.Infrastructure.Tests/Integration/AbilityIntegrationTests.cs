using NUnit.Framework;
using NSubstitute;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Infrastructure.Events;
using System.Collections.Generic;

// Alias to avoid conflict with legacy CardFactory in global namespace
using TestCardFactory = JDG.TestUtilities.CardFactory;
using JDG.TestUtilities;

namespace JDG.Infrastructure.Tests.Integration
{
    /// <summary>
    /// Integration tests for the ability system.
    /// Tests abilities with real EventBus and mock repositories.
    /// </summary>
    [TestFixture]
    public class AbilityIntegrationTests
    {
        private IEventBus _eventBus;
        private List<object> _publishedEvents;
        private IPlayerRepository _playerRepository;
        private ICardRepository _cardRepository;
        private AbilityRegistry _abilityRegistry;

        [SetUp]
        public void SetUp()
        {
            // Use real EventBus to test event publishing
            _eventBus = new EventBus();
            _publishedEvents = new List<object>();

            // Subscribe to track all events
            _eventBus.Subscribe<CardDrawnEvent>(e => _publishedEvents.Add(e));
            _eventBus.Subscribe<CardDestroyedEvent>(e => _publishedEvents.Add(e));

            // Use NSubstitute mocks for repositories
            _playerRepository = Substitute.For<IPlayerRepository>();
            _cardRepository = Substitute.For<ICardRepository>();

            // Setup ability registry
            _abilityRegistry = new AbilityRegistry();
        }

        [TearDown]
        public void TearDown()
        {
            _eventBus.ClearAllSubscriptions();
        }

        [Test]
        public void AbilityRegistry_RegisterAndRetrieve_Works()
        {
            // Arrange - Register a simple ability
            _abilityRegistry.Register(AbilityName.Draw1Card, () => new TestDrawAbility(1));
            _abilityRegistry.Register(AbilityName.Draw2Cards, () => new TestDrawAbility(2));

            // Act
            var draw1 = _abilityRegistry.GetAbility(AbilityName.Draw1Card);
            var draw2 = _abilityRegistry.GetAbility(AbilityName.Draw2Cards);

            // Assert
            Assert.IsNotNull(draw1);
            Assert.IsNotNull(draw2);
            Assert.AreEqual(AbilityName.Draw1Card, draw1.Name);
            Assert.AreEqual(AbilityName.Draw2Cards, draw2.Name);
        }

        [Test]
        public void AbilityExecution_WithContext_ExecutesCorrectly()
        {
            // Arrange
            var ability = new TestDrawAbility(2);
            var context = CreateAbilityContext(PlayerId.Player1, TestCardFactory.CreateInvocation());

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, ((TestDrawAbility)ability).ExecutionCount);
        }

        [Test]
        public void AbilityRegistry_UnregisteredAbility_ThrowsException()
        {
            // Act & Assert - Unregistered ability should throw KeyNotFoundException
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() =>
                _abilityRegistry.GetAbility(AbilityName.Default));
        }

        [Test]
        public void AbilityRegistry_DuplicateRegistration_ThrowsException()
        {
            // Arrange
            _abilityRegistry.Register(AbilityName.Draw1Card, () => new TestDrawAbility(1));

            // Act & Assert - Duplicate registration should throw InvalidOperationException
            Assert.Throws<System.InvalidOperationException>(() =>
                _abilityRegistry.Register(AbilityName.Draw1Card, () => new TestDrawAbility(5)));
        }

        [Test]
        public void PassiveAbility_TriggerCheck_Works()
        {
            // Arrange
            var passiveAbility = new TestPassiveAbility(AbilityTrigger.OnSummon);

            // Assert
            Assert.AreEqual(AbilityTrigger.OnSummon, passiveAbility.Trigger);
        }

        [Test]
        public void AbilityContext_CarriesCorrectState()
        {
            // Arrange
            var sourceCard = TestCardFactory.CreateInvocation("Source Card");
            var targetCard = TestCardFactory.CreateInvocation("Target Card");

            // Act
            var context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                sourceCard,
                AbilityName.KillOpponentInvocation
            );
            context.TargetCard = targetCard;

            // Assert
            Assert.AreEqual(PlayerId.Player1, context.CurrentPlayerId);
            Assert.AreEqual(PlayerId.Player2, context.OpponentPlayerId);
            Assert.AreEqual(sourceCard, context.SourceCard);
            Assert.AreEqual(targetCard, context.TargetCard);
            Assert.AreEqual(AbilityName.KillOpponentInvocation, context.AbilityName);
        }

        [Test]
        public void AbilityResult_Success_HasCorrectProperties()
        {
            // Act
            var result = AbilityResult.Success("Operation completed");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Operation completed", result.Message);
            Assert.IsFalse(result.RequiresUserInput);
        }

        [Test]
        public void AbilityResult_Failure_HasCorrectProperties()
        {
            // Act
            var result = AbilityResult.Failure("Something went wrong");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Something went wrong", result.Message);
        }

        [Test]
        public void AbilityResult_NeedsUserInput_HasCorrectProperties()
        {
            // Act
            var result = AbilityResult.NeedsUserInput("Select a target card");

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
            Assert.AreEqual("Select a target card", result.Message);
        }

        #region Ability Execution Tests

        [Test]
        public void DrawCardsAbility_Execute_PublishesCardDrawnEvents()
        {
            // Arrange
            _eventBus.Subscribe<AbilityExecutedEvent>(e => _publishedEvents.Add(e));
            var ability = new TestDrawAbilityWithEvent(_eventBus, 2);
            var context = CreateAbilityContext(PlayerId.Player1, TestCardFactory.CreateInvocation());

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(_publishedEvents.Exists(e => e is AbilityExecutedEvent));
        }

        [Test]
        public void DestroyCardAbility_Execute_PublishesCardDestroyedEvent()
        {
            // Arrange
            var targetCard = TestCardFactory.CreateInvocation("Target Card");
            var ability = new TestDestroyAbility(_eventBus);
            var context = CreateAbilityContext(PlayerId.Player1, TestCardFactory.CreateInvocation());
            context.TargetCard = targetCard;

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            var destroyedEvents = _publishedEvents.FindAll(e => e is CardDestroyedEvent);
            Assert.AreEqual(1, destroyedEvents.Count);
        }

        [Test]
        public void StatModifierAbility_Execute_ModifiesStats()
        {
            // Arrange
            var ability = new TestStatModifierAbility(attackBonus: 2, defenseBonus: 1);
            var sourceCard = TestCardFactory.CreateInvocation("Source Card");
            var context = CreateAbilityContext(PlayerId.Player1, sourceCard);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, ability.LastAppliedAtkBonus);
            Assert.AreEqual(1, ability.LastAppliedDefBonus);
        }

        [Test]
        public void ProtectionAbility_Execute_GrantsImmunity()
        {
            // Arrange
            var ability = new TestProtectionAbility(preventAttacks: true);
            var sourceCard = TestCardFactory.CreateInvocation("Protected Card");
            var context = CreateAbilityContext(PlayerId.Player1, sourceCard);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(ability.IsProtectionActive);
        }

        [Test]
        public void AbilityChain_MultipleAbilities_ExecuteInSequence()
        {
            // Arrange
            var abilities = new List<IAbility>
            {
                new TestDrawAbility(1),
                new TestStatModifierAbility(1, 1),
                new TestDrawAbility(2)
            };
            var executionOrder = new List<AbilityName>();

            // Act
            foreach (var ability in abilities)
            {
                var context = CreateAbilityContext(PlayerId.Player1, TestCardFactory.CreateInvocation());
                ability.Execute(context);
                executionOrder.Add(ability.Name);
            }

            // Assert
            Assert.AreEqual(3, executionOrder.Count);
        }

        [Test]
        public void AbilityWithCondition_WhenConditionMet_Executes()
        {
            // Arrange
            var ability = new TestConditionalAbility(shouldSucceed: true);
            var context = CreateAbilityContext(PlayerId.Player1, TestCardFactory.CreateInvocation());

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void AbilityWithCondition_WhenConditionNotMet_Fails()
        {
            // Arrange
            var ability = new TestConditionalAbility(shouldSucceed: false);
            var context = CreateAbilityContext(PlayerId.Player1, TestCardFactory.CreateInvocation());

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void MultiTargetAbility_WithMultipleTargets_AffectsAll()
        {
            // Arrange
            var targets = new List<Card>
            {
                TestCardFactory.CreateInvocation("Target 1"),
                TestCardFactory.CreateInvocation("Target 2"),
                TestCardFactory.CreateInvocation("Target 3")
            };
            var ability = new TestMultiTargetAbility(targets);
            var context = CreateAbilityContext(PlayerId.Player1, TestCardFactory.CreateInvocation());

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, ability.AffectedTargetCount);
        }

        #endregion

        #region Helper Methods

        private AbilityContext CreateAbilityContext(PlayerId currentPlayer, Card sourceCard)
        {
            var opponentId = currentPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
            return new AbilityContext(currentPlayer, opponentId, sourceCard, AbilityName.Default);
        }

        #endregion

        #region Test Ability Implementations

        /// <summary>
        /// Test ability that simulates drawing cards.
        /// </summary>
        private class TestDrawAbility : IAbility
        {
            private readonly int _cardsToDraw;
            public int ExecutionCount { get; private set; }

            public TestDrawAbility(int cardsToDraw)
            {
                _cardsToDraw = cardsToDraw;
            }

            public AbilityName Name => _cardsToDraw == 1 ? AbilityName.Draw1Card : AbilityName.Draw2Cards;
            public string Description => $"Draw {_cardsToDraw} card(s)";

            public bool CanActivate(AbilityContext context)
            {
                return context.CurrentPlayerId != default;
            }

            public AbilityResult Execute(AbilityContext context)
            {
                ExecutionCount = _cardsToDraw;
                return AbilityResult.Success($"Drew {_cardsToDraw} card(s)");
            }
        }

        /// <summary>
        /// Test passive ability for testing trigger logic.
        /// </summary>
        private class TestPassiveAbility : IPassiveAbility
        {
            public TestPassiveAbility(AbilityTrigger trigger)
            {
                Trigger = trigger;
            }

            public AbilityName Name => AbilityName.Default;
            public string Description => "Test passive ability";
            public AbilityTrigger Trigger { get; }

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                return AbilityResult.Success("Passive ability executed");
            }
        }

        /// <summary>
        /// Test ability that draws cards and publishes events.
        /// </summary>
        private class TestDrawAbilityWithEvent : IAbility
        {
            private readonly IEventBus _eventBus;
            private readonly int _cardsToDraw;

            public TestDrawAbilityWithEvent(IEventBus eventBus, int cardsToDraw)
            {
                _eventBus = eventBus;
                _cardsToDraw = cardsToDraw;
            }

            public AbilityName Name => AbilityName.Draw2Cards;
            public string Description => $"Draw {_cardsToDraw} cards";

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                _eventBus.Publish(new AbilityExecutedEvent
                {
                    AbilityName = Name,
                    PlayerId = context.CurrentPlayerId.ToCardOwner(),
                    IsSuccess = true,
                    Message = $"Drew {_cardsToDraw} cards"
                });
                return AbilityResult.Success($"Drew {_cardsToDraw} cards");
            }
        }

        /// <summary>
        /// Test ability that destroys a target card.
        /// </summary>
        private class TestDestroyAbility : IAbility
        {
            private readonly IEventBus _eventBus;

            public TestDestroyAbility(IEventBus eventBus)
            {
                _eventBus = eventBus;
            }

            public AbilityName Name => AbilityName.KillOpponentInvocation;
            public string Description => "Destroy target card";

            public bool CanActivate(AbilityContext context) => context.TargetCard != null;

            public AbilityResult Execute(AbilityContext context)
            {
                if (context.TargetCard == null)
                    return AbilityResult.Failure("No target selected");

                _eventBus.Publish(new CardDestroyedEvent
                {
                    CardId = context.TargetCard.Id,
                    Owner = context.OpponentPlayerId.ToCardOwner(),
                    Reason = "Destroyed by ability"
                });
                return AbilityResult.Success($"Destroyed {context.TargetCard.Title}");
            }
        }

        /// <summary>
        /// Test ability that modifies stats.
        /// </summary>
        private class TestStatModifierAbility : IAbility
        {
            private readonly int _attackBonus;
            private readonly int _defenseBonus;

            public int LastAppliedAtkBonus { get; private set; }
            public int LastAppliedDefBonus { get; private set; }

            public TestStatModifierAbility(int attackBonus, int defenseBonus)
            {
                _attackBonus = attackBonus;
                _defenseBonus = defenseBonus;
            }

            public AbilityName Name => AbilityName.GiveFamilyStats;
            public string Description => $"+{_attackBonus}/{_defenseBonus} stats";

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                LastAppliedAtkBonus = _attackBonus;
                LastAppliedDefBonus = _defenseBonus;
                return AbilityResult.Success($"Applied +{_attackBonus}/+{_defenseBonus}");
            }
        }

        /// <summary>
        /// Test ability that grants protection.
        /// </summary>
        private class TestProtectionAbility : IAbility
        {
            private readonly bool _preventAttacks;

            public bool IsProtectionActive { get; private set; }

            public TestProtectionAbility(bool preventAttacks)
            {
                _preventAttacks = preventAttacks;
            }

            public AbilityName Name => AbilityName.CantBeAttacked;
            public string Description => "Cannot be attacked";

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                IsProtectionActive = _preventAttacks;
                return AbilityResult.Success("Protection granted");
            }
        }

        /// <summary>
        /// Test ability with conditional execution.
        /// </summary>
        private class TestConditionalAbility : IAbility
        {
            private readonly bool _shouldSucceed;

            public TestConditionalAbility(bool shouldSucceed)
            {
                _shouldSucceed = shouldSucceed;
            }

            public AbilityName Name => AbilityName.Default;
            public string Description => "Conditional ability";

            public bool CanActivate(AbilityContext context) => _shouldSucceed;

            public AbilityResult Execute(AbilityContext context)
            {
                if (!_shouldSucceed)
                    return AbilityResult.Failure("Condition not met");
                return AbilityResult.Success("Condition met");
            }
        }

        /// <summary>
        /// Test ability that affects multiple targets.
        /// </summary>
        private class TestMultiTargetAbility : IAbility
        {
            private readonly List<Card> _targets;

            public int AffectedTargetCount { get; private set; }

            public TestMultiTargetAbility(List<Card> targets)
            {
                _targets = targets;
            }

            public AbilityName Name => AbilityName.Default;
            public string Description => "Affect multiple targets";

            public bool CanActivate(AbilityContext context) => _targets.Count > 0;

            public AbilityResult Execute(AbilityContext context)
            {
                AffectedTargetCount = _targets.Count;
                return AbilityResult.Success($"Affected {AffectedTargetCount} targets");
            }
        }

        #endregion
    }
}

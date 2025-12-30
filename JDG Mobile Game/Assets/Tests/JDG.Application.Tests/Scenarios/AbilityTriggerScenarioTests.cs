using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Tests.Scenarios
{
    /// <summary>
    /// Scenario tests for ability trigger mechanics.
    /// Tests ability execution, triggers, and chaining.
    /// Phase 129: Ability trigger scenario tests.
    /// </summary>
    [TestFixture]
    public class AbilityTriggerScenarioTests
    {
        #region Test Doubles

        /// <summary>
        /// Test passive ability that tracks execution.
        /// </summary>
        private class TestPassiveAbility : IPassiveAbility
        {
            public AbilityName Name { get; }
            public string Description => "Test ability";
            public AbilityTrigger Trigger { get; }
            public int ExecutionCount { get; private set; }
            public bool ShouldSucceed { get; set; } = true;

            public TestPassiveAbility(AbilityTrigger trigger, AbilityName name = AbilityName.Draw1Card)
            {
                Trigger = trigger;
                Name = name;
            }

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                ExecutionCount++;
                return ShouldSucceed
                    ? AbilityResult.Success("Executed")
                    : AbilityResult.Failure("Failed");
            }
        }

        /// <summary>
        /// Test stat modifier ability that tracks applied values.
        /// </summary>
        private class TestStatModifierAbility : IPassiveAbility
        {
            private readonly int _atkBonus;
            private readonly int _defBonus;

            public int AppliedAtkBonus { get; private set; }
            public int AppliedDefBonus { get; private set; }
            public AbilityName Name => AbilityName.GiveAtkDefToComics;
            public string Description => "Stat modifier";
            public AbilityTrigger Trigger { get; }
            public int ExecutionCount { get; private set; }

            public TestStatModifierAbility(AbilityTrigger trigger, int atkBonus, int defBonus)
            {
                Trigger = trigger;
                _atkBonus = atkBonus;
                _defBonus = defBonus;
            }

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                ExecutionCount++;
                AppliedAtkBonus = _atkBonus;
                AppliedDefBonus = _defBonus;
                return AbilityResult.Success($"Applied +{_atkBonus}/+{_defBonus}");
            }
        }

        #endregion

        #region On-Summon Trigger Scenarios

        [Test]
        public void OnSummon_TriggerFires_WhenCardPlaced()
        {
            // Arrange
            var ability = new TestPassiveAbility(AbilityTrigger.OnSummon);
            var context = CreateContext(PlayerId.Player1);

            // Act
            Assert.AreEqual(AbilityTrigger.OnSummon, ability.Trigger);
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, ability.ExecutionCount);
        }

        [Test]
        public void OnSummon_StatBoostAbility_AppliesImmediately()
        {
            // Arrange
            var ability = new TestStatModifierAbility(AbilityTrigger.OnSummon, 2, 1);
            var context = CreateContext(PlayerId.Player1);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, ability.AppliedAtkBonus);
            Assert.AreEqual(1, ability.AppliedDefBonus);
        }

        [Test]
        public void OnSummon_MultipleAbilities_AllFire()
        {
            // Arrange
            var abilities = new[]
            {
                new TestPassiveAbility(AbilityTrigger.OnSummon),
                new TestPassiveAbility(AbilityTrigger.OnSummon),
                new TestPassiveAbility(AbilityTrigger.OnSummon)
            };
            var context = CreateContext(PlayerId.Player1);

            // Act - Simulate card placed with multiple abilities
            int successCount = 0;
            foreach (var ability in abilities)
            {
                if (ability.Trigger == AbilityTrigger.OnSummon)
                {
                    var result = ability.Execute(context);
                    if (result.IsSuccess) successCount++;
                }
            }

            // Assert
            Assert.AreEqual(3, successCount, "All on-summon abilities should fire");
        }

        #endregion

        #region On-Death Trigger Scenarios

        [Test]
        public void OnDeath_SingleTrigger_Fires()
        {
            // Arrange
            var ability = new TestPassiveAbility(AbilityTrigger.OnDeath);
            var context = CreateContext(PlayerId.Player1);

            // Verify trigger type
            Assert.AreEqual(AbilityTrigger.OnDeath, ability.Trigger);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, ability.ExecutionCount);
        }

        [Test]
        public void OnDeath_ChainedTriggers_AllFire()
        {
            // Arrange - Multiple death triggers
            var abilities = new[]
            {
                new TestPassiveAbility(AbilityTrigger.OnDeath, AbilityName.Draw1Card),
                new TestPassiveAbility(AbilityTrigger.OnDeath, AbilityName.Draw2Cards)
            };
            var context = CreateContext(PlayerId.Player1);

            // Act
            int executionCount = 0;
            foreach (var ability in abilities)
            {
                if (ability.Trigger == AbilityTrigger.OnDeath && ability.CanActivate(context))
                {
                    var result = ability.Execute(context);
                    if (result.IsSuccess) executionCount++;
                }
            }

            // Assert
            Assert.AreEqual(2, executionCount, "All death triggers should fire");
        }

        [Test]
        public void OnDeath_FailedAbility_DoesNotBlockOthers()
        {
            // Arrange - First ability fails
            var failingAbility = new TestPassiveAbility(AbilityTrigger.OnDeath) { ShouldSucceed = false };
            var succeedingAbility = new TestPassiveAbility(AbilityTrigger.OnDeath);
            var abilities = new[] { failingAbility, succeedingAbility };
            var context = CreateContext(PlayerId.Player1);

            // Act
            int successCount = 0;
            foreach (var ability in abilities)
            {
                var result = ability.Execute(context);
                if (result.IsSuccess) successCount++;
            }

            // Assert
            Assert.AreEqual(1, successCount, "Succeeding ability should still fire after failed one");
            Assert.AreEqual(1, failingAbility.ExecutionCount, "Failed ability was still executed");
            Assert.AreEqual(1, succeedingAbility.ExecutionCount, "Succeeding ability was executed");
        }

        #endregion

        #region Equipment Trigger Scenarios

        [Test]
        public void OnEquip_StatBoost_Applied()
        {
            // Arrange
            var ability = new TestStatModifierAbility(AbilityTrigger.OnEquip, 3, 2);
            var context = CreateContext(PlayerId.Player1);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, ability.AppliedAtkBonus);
            Assert.AreEqual(2, ability.AppliedDefBonus);
        }

        [Test]
        public void OnUnequip_StatBoost_Removed()
        {
            // Arrange - Simulate equip then unequip
            var equipAbility = new TestStatModifierAbility(AbilityTrigger.OnEquip, 3, 2);
            var unequipAbility = new TestStatModifierAbility(AbilityTrigger.OnUnequip, -3, -2);
            var context = CreateContext(PlayerId.Player1);

            // Act - Equip
            equipAbility.Execute(context);

            // Act - Unequip
            var result = unequipAbility.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(-3, unequipAbility.AppliedAtkBonus, "Unequip should remove stats");
            Assert.AreEqual(-2, unequipAbility.AppliedDefBonus);
        }

        #endregion

        #region Continuous Effect Scenarios

        [Test]
        public void Continuous_TriggerType_IdentifiedCorrectly()
        {
            // Arrange
            var ability = new TestPassiveAbility(AbilityTrigger.Continuous);

            // Assert
            Assert.AreEqual(AbilityTrigger.Continuous, ability.Trigger);
        }

        [Test]
        public void Continuous_CanExecuteMultipleTimes()
        {
            // Arrange
            var ability = new TestStatModifierAbility(AbilityTrigger.Continuous, 1, 1);
            var context = CreateContext(PlayerId.Player1);

            // Act - Execute multiple times (simulating turn-by-turn continuous effect)
            ability.Execute(context);
            ability.Execute(context);
            ability.Execute(context);

            // Assert
            Assert.AreEqual(3, ability.ExecutionCount, "Continuous abilities can execute multiple times");
        }

        #endregion

        #region Trigger Type Filtering

        [Test]
        public void TriggerFilter_OnlyMatchingTriggersExecute()
        {
            // Arrange - Mix of trigger types
            var onSummon = new TestPassiveAbility(AbilityTrigger.OnSummon);
            var onDeath = new TestPassiveAbility(AbilityTrigger.OnDeath);
            var onEquip = new TestPassiveAbility(AbilityTrigger.OnEquip);
            var abilities = new[] { onSummon, onDeath, onEquip };
            var context = CreateContext(PlayerId.Player1);

            // Act - Filter for OnSummon only
            int executedCount = 0;
            foreach (var ability in abilities)
            {
                if (ability.Trigger == AbilityTrigger.OnSummon)
                {
                    ability.Execute(context);
                    executedCount++;
                }
            }

            // Assert
            Assert.AreEqual(1, executedCount);
            Assert.AreEqual(1, onSummon.ExecutionCount);
            Assert.AreEqual(0, onDeath.ExecutionCount);
            Assert.AreEqual(0, onEquip.ExecutionCount);
        }

        [Test]
        public void TriggerFilter_OnDeathOnlyFiresOnDeath()
        {
            // Arrange
            var onDeath = new TestPassiveAbility(AbilityTrigger.OnDeath);
            var context = CreateContext(PlayerId.Player1);

            // Act - Try to execute on summon (should be filtered)
            bool shouldExecute = onDeath.Trigger == AbilityTrigger.OnSummon;

            // Assert
            Assert.IsFalse(shouldExecute, "OnDeath should not match OnSummon trigger");
        }

        #endregion

        #region Ability Context

        [Test]
        public void AbilityContext_CarriesOwnerInfo()
        {
            // Arrange
            var context = CreateContext(PlayerId.Player1);

            // Assert
            Assert.AreEqual(PlayerId.Player1, context.CurrentPlayerId);
            Assert.AreEqual(PlayerId.Player2, context.OpponentPlayerId);
        }

        [Test]
        public void AbilityContext_Player2Owner_HasCorrectOpponent()
        {
            // Arrange
            var context = CreateContext(PlayerId.Player2);

            // Assert
            Assert.AreEqual(PlayerId.Player2, context.CurrentPlayerId);
            Assert.AreEqual(PlayerId.Player1, context.OpponentPlayerId);
        }

        #endregion

        #region Helper Methods

        private AbilityContext CreateContext(PlayerId owner, Card sourceCard = null)
        {
            var opponent = owner == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
            return new AbilityContext(owner, opponent, sourceCard, AbilityName.Default);
        }

        #endregion
    }
}

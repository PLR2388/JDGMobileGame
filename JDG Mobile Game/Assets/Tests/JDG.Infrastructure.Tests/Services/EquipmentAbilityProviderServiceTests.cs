using System;
using NUnit.Framework;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Tests for EquipmentAbilityProviderService.
    /// Phase 84: Created to verify provider service after EquipmentAbilityLibrary deletion.
    /// </summary>
    [TestFixture]
    public class EquipmentAbilityProviderServiceTests
    {
        private EquipmentAbilityProviderService _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new EquipmentAbilityProviderService();
        }

        [Test]
        public void GetAbility_WithValidName_ReturnsAbility()
        {
            // Arrange
            var abilityName = EquipmentAbilityName.DirectAttack;

            // Act
            var ability = _provider.GetAbility(abilityName);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(abilityName, ability.Name);
        }

        [Test]
        public void GetAbility_WithMultiplyDefBy2_ReturnsCorrectAbility()
        {
            // Act
            var ability = _provider.GetAbility(EquipmentAbilityName.MultiplyDefBy2ButPreventAttack);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(EquipmentAbilityName.MultiplyDefBy2ButPreventAttack, ability.Name);
        }

        [Test]
        public void GetAbility_WithEarn1ATKAnd1DEF_ReturnsCorrectAbility()
        {
            // Act
            var ability = _provider.GetAbility(EquipmentAbilityName.Earn1ATKAnd1DEF);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(EquipmentAbilityName.Earn1ATKAnd1DEF, ability.Name);
        }

        [Test]
        public void HasAbility_WithExistingAbility_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(EquipmentAbilityName.DirectAttack);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAbility_WithProtectOneTimeFromDestruction_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(EquipmentAbilityName.ProtectOneTimeFromDestruction);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAbility_WithCancelInvocationAbility_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(EquipmentAbilityName.CancelInvocationAbility);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AllDefinedAbilities_AreAvailable()
        {
            // Verify all known equipment abilities are present
            var expectedAbilities = new[]
            {
                EquipmentAbilityName.MultiplyDefBy2ButPreventAttack,
                EquipmentAbilityName.Earn1ATKAndMinus1DEF,
                EquipmentAbilityName.DirectAttack,
                EquipmentAbilityName.EarnOneQuarterATKPerHandCards,
                EquipmentAbilityName.PreventNewOpponentToAttack,
                EquipmentAbilityName.Remove1ATKAnd1DEF,
                EquipmentAbilityName.SetATKToOne,
                EquipmentAbilityName.CantBeAttackByOtherInvocations,
                EquipmentAbilityName.MultiplyAtkBy3,
                EquipmentAbilityName.SetDefToZero,
                EquipmentAbilityName.Earn2ATK,
                EquipmentAbilityName.Earn3ATKAndMinus1DEF,
                EquipmentAbilityName.Earn1ATKAnd1DEF,
                EquipmentAbilityName.MultiplyAtkBy2AndDefByHalf,
                EquipmentAbilityName.EarnOneQuarterDEFPerHandCards,
                EquipmentAbilityName.SwitchEquipmentCard,
                EquipmentAbilityName.Loose2ATK,
                EquipmentAbilityName.ProtectOneTimeFromDestruction,
                EquipmentAbilityName.CancelInvocationAbility
            };

            foreach (var abilityName in expectedAbilities)
            {
                Assert.IsTrue(_provider.HasAbility(abilityName), $"Missing ability: {abilityName}");
                Assert.IsNotNull(_provider.GetAbility(abilityName), $"Null ability: {abilityName}");
            }
        }
    }
}

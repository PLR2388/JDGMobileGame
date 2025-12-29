using System;
using NUnit.Framework;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Tests for FieldAbilityProviderService.
    /// Phase 84: Created to verify provider service after FieldAbilityLibrary deletion.
    /// </summary>
    [TestFixture]
    public class FieldAbilityProviderServiceTests
    {
        private FieldAbilityProviderService _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new FieldAbilityProviderService();
        }

        [Test]
        public void GetAbility_WithValidName_ReturnsAbility()
        {
            // Arrange
            var abilityName = FieldAbilityName.DrawOneMoreCard;

            // Act
            var ability = _provider.GetAbility(abilityName);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(abilityName, ability.Name);
        }

        [Test]
        public void GetAbility_WithEarn1DEFForSpatialFamily_ReturnsCorrectAbility()
        {
            // Act
            var ability = _provider.GetAbility(FieldAbilityName.Earn1DEFForSpatialFamily);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(FieldAbilityName.Earn1DEFForSpatialFamily, ability.Name);
        }

        [Test]
        public void GetAbility_WithChangeJMBruitagesFamilyToDev_ReturnsCorrectAbility()
        {
            // Act
            var ability = _provider.GetAbility(FieldAbilityName.ChangeJMBruitagesFamilyToDev);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(FieldAbilityName.ChangeJMBruitagesFamilyToDev, ability.Name);
        }

        [Test]
        public void HasAbility_WithExistingAbility_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(FieldAbilityName.DrawOneMoreCard);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAbility_WithSkipDrawToGetFistilandInvocation_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(FieldAbilityName.SkipDrawToGetFistilandInvocation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAbility_WithEarnHalfHPPerWizardInvocationEachTurn_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(FieldAbilityName.EarnHalfHPPerWizardInvocationEachTurn);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AllDefinedAbilities_AreAvailable()
        {
            // Verify all known field abilities are present
            var expectedAbilities = new[]
            {
                FieldAbilityName.Earn1DEFForSpatialFamily,
                FieldAbilityName.Earn1HalfDEFAndMinusHalfATKForDevFamily,
                FieldAbilityName.ChangeJMBruitagesFamilyToDev,
                FieldAbilityName.ChangePatronInfogramFamilyToDev,
                FieldAbilityName.Earn2DEFAndMinusOneATKForIncarnationFamily,
                FieldAbilityName.EarnHalfHPPerWizardInvocationEachTurn,
                FieldAbilityName.Earn1ATKForJapanFamily,
                FieldAbilityName.Earn1HalfATKAndMinusHalfDEFForHCFamily,
                FieldAbilityName.DrawOneMoreCard,
                FieldAbilityName.EarnHalfATKAndDefForRpgFamily,
                FieldAbilityName.SkipDrawToGetFistilandInvocation,
                FieldAbilityName.Earn2ATKAndMinus1DEFForComicsFamily
            };

            foreach (var abilityName in expectedAbilities)
            {
                Assert.IsTrue(_provider.HasAbility(abilityName), $"Missing ability: {abilityName}");
                Assert.IsNotNull(_provider.GetAbility(abilityName), $"Null ability: {abilityName}");
            }
        }
    }
}

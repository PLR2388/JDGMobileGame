using JDG.Infrastructure.Services;
using System;
using JDG.Domain.Enums;
using NUnit.Framework;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Tests for ConditionProviderService.
    /// Phase 84: Created to verify provider service after ConditionLibrary deletion.
    /// </summary>
    [TestFixture]
    public class ConditionProviderServiceTests
    {
        private ConditionProviderService _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new ConditionProviderService();
        }

        [Test]
        public void GetCondition_WithValidName_ReturnsCondition()
        {
            // Arrange
            var conditionName = ConditionName.BenzaieJeuneOrBenzaieOnField;

            // Act
            var condition = _provider.GetCondition(conditionName);

            // Assert
            Assert.IsNotNull(condition);
        }

        [Test]
        public void GetConditionTyped_WithValidName_ReturnsCondition()
        {
            // Arrange
            var conditionName = ConditionName.ZozanKebabOnField;

            // Act
            var condition = _provider.GetConditionTyped(conditionName);

            // Assert
            Assert.IsNotNull(condition);
            Assert.AreEqual(conditionName, condition.Name);
        }

        [Test]
        public void GetConditionTyped_WithJoueurDuGrenierOnField_ReturnsCorrectCondition()
        {
            // Act
            var condition = _provider.GetConditionTyped(ConditionName.JoueurDuGrenierOnFieldCondition);

            // Assert
            Assert.IsNotNull(condition);
            Assert.AreEqual(ConditionName.JoueurDuGrenierOnFieldCondition, condition.Name);
        }

        [Test]
        public void GetCondition_WithInvalidObjectType_ReturnsNull()
        {
            // Act
            var condition = _provider.GetCondition("InvalidType");

            // Assert
            Assert.IsNull(condition);
        }

        [Test]
        public void HasCondition_WithExistingCondition_ReturnsTrue()
        {
            // Act
            var result = _provider.HasCondition(ConditionName.BenzaieJeuneOrBenzaieOnField);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasCondition_WithWizardOnField_ReturnsTrue()
        {
            // Act
            var result = _provider.HasCondition(ConditionName.WizardOnField);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasCondition_WithInvalidObjectType_ReturnsFalse()
        {
            // Act
            var result = _provider.HasCondition("InvalidType");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void AllDefinedConditions_AreAvailable()
        {
            // Verify all known conditions are present
            var expectedConditions = new[]
            {
                ConditionName.BenzaieJeuneOrBenzaieOnField,
                ConditionName.ZozanKebabOnField,
                ConditionName.ArchibalVonGrenierOnField,
                ConditionName.BenzaieJeuneCassetteVhsEquiped,
                ConditionName.JoueurDuGrenierCanarangEquiped,
                ConditionName.ThreeAtk3Def,
                ConditionName.ForetDesElfesSylvainsOnField,
                ConditionName.JoueurDuGrenierOnFieldCondition,
                ConditionName.WizardOnField,
                ConditionName.LyceeMagiqueGeorgesPompidouOnField,
                ConditionName.Developer3Atk3Def2Cards,
                ConditionName.HardCorner3Atk3Def2Cards,
                ConditionName.Japan2Cards,
                ConditionName.TenDeathYellowTrash,
                ConditionName.ComicsOnField,
                ConditionName.Incarnation2Cards,
                ConditionName.GranolaxAlreadyDead,
                ConditionName.HumanOnField,
                ConditionName.SebDuGrenierMerdePlastiqueBleuEquiped,
                ConditionName.SebDuGrenierOnField,
                ConditionName.ClicheRacisteMerdeRoseEquiped,
                ConditionName.MechaGronolaxOrGranolaxOnField,
                ConditionName.JapanOnField
            };

            foreach (var conditionName in expectedConditions)
            {
                Assert.IsTrue(_provider.HasCondition(conditionName), $"Missing condition: {conditionName}");
                Assert.IsNotNull(_provider.GetConditionTyped(conditionName), $"Null condition: {conditionName}");
            }
        }
    }
}

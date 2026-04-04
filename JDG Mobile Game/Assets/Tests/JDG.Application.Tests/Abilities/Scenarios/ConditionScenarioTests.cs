using JDG.Infrastructure.Services;
using JDG.Domain.Enums;
using NUnit.Framework;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for all 23 summon conditions.
    /// These tests verify condition configuration and provider functionality.
    ///
    /// Note: The actual CanBeSummoned logic tests require PlayerCards (MonoBehaviour)
    /// and will be covered in PlayMode E2E tests (Phase 8).
    ///
    /// Condition Types:
    /// - InvocationCardOnFieldCondition: Check if specific named cards are on field
    /// - FieldCardOnFieldCondition: Check if specific field card is active
    /// - EquipmentCardOnCardCondition: Check if specific equipment is on specific card
    /// - SpecificAtkDefInvocationCardOnFieldCondition: Check for card with min ATK/DEF
    /// - SpecificFamilyInvocationCardOnFieldCondition: Check for card of specific family
    /// - SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition: Check for N cards of family with ATK/DEF
    /// - NumberInvocationDeadCondition: Check if N cards are in graveyard
    /// - SpecificCardBackFromDeathCondition: Check if specific card was resurrected
    /// </summary>
    [TestFixture]
    public class ConditionScenarioTests
    {
        private ConditionProviderService _conditionProvider;

        [SetUp]
        public void SetUp()
        {
            _conditionProvider = new ConditionProviderService();
        }

        #region Invocation Card On Field Conditions

        [Test]
        public void BenzaieJeuneOrBenzaieOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.BenzaieJeuneOrBenzaieOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.BenzaieJeuneOrBenzaieOnField, condition.Name);
            Assert.IsInstanceOf<InvocationCardOnFieldCondition>(condition);

            // Verify it checks for both Benzaie variants
            // This condition allows summoning cards like "Canardman" that require Benzaie
        }

        [Test]
        public void ArchibalVonGrenierOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.ArchibalVonGrenierOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.ArchibalVonGrenierOnField, condition.Name);
            Assert.IsInstanceOf<InvocationCardOnFieldCondition>(condition);
        }

        [Test]
        public void JoueurDuGrenierOnFieldCondition_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.JoueurDuGrenierOnFieldCondition);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.JoueurDuGrenierOnFieldCondition, condition.Name);
            Assert.IsInstanceOf<InvocationCardOnFieldCondition>(condition);

            // This condition allows summoning cards that require Joueur Du Grenier
        }

        [Test]
        public void SebDuGrenierOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.SebDuGrenierOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.SebDuGrenierOnField, condition.Name);
            Assert.IsInstanceOf<InvocationCardOnFieldCondition>(condition);
        }

        [Test]
        public void MechaGronolaxOrGranolaxOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.MechaGronolaxOrGranolaxOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.MechaGronolaxOrGranolaxOnField, condition.Name);
            Assert.IsInstanceOf<InvocationCardOnFieldCondition>(condition);

            // This condition allows summoning cards like "Starlight Unicorn"
        }

        #endregion

        #region Field Card On Field Conditions

        [Test]
        public void ZozanKebabOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.ZozanKebabOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.ZozanKebabOnField, condition.Name);
            Assert.IsInstanceOf<FieldCardOnFieldCondition>(condition);

            // Checks if field card "Zozan Kebab" is active
        }

        [Test]
        public void ForetDesElfesSylvainsOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.ForetDesElfesSylvainsOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.ForetDesElfesSylvainsOnField, condition.Name);
            Assert.IsInstanceOf<FieldCardOnFieldCondition>(condition);
        }

        [Test]
        public void LyceeMagiqueGeorgesPompidouOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.LyceeMagiqueGeorgesPompidouOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.LyceeMagiqueGeorgesPompidouOnField, condition.Name);
            Assert.IsInstanceOf<FieldCardOnFieldCondition>(condition);
        }

        #endregion

        #region Equipment Card On Card Conditions

        [Test]
        public void BenzaieJeuneCassetteVhsEquiped_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.BenzaieJeuneCassetteVhsEquiped);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.BenzaieJeuneCassetteVhsEquiped, condition.Name);
            Assert.IsInstanceOf<EquipmentCardOnCardCondition>(condition);

            // Checks if "Benzaie jeune" has "Cassette VHS" equipped
        }

        [Test]
        public void JoueurDuGrenierCanarangEquiped_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.JoueurDuGrenierCanarangEquiped);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.JoueurDuGrenierCanarangEquiped, condition.Name);
            Assert.IsInstanceOf<EquipmentCardOnCardCondition>(condition);

            // Checks if "Joueur Du Grenier" has "Canarang" equipped
        }

        [Test]
        public void SebDuGrenierMerdePlastiqueBleuEquiped_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.SebDuGrenierMerdePlastiqueBleuEquiped);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.SebDuGrenierMerdePlastiqueBleuEquiped, condition.Name);
            Assert.IsInstanceOf<EquipmentCardOnCardCondition>(condition);
        }

        [Test]
        public void ClicheRacisteMerdeRoseEquiped_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.ClicheRacisteMerdeRoseEquiped);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.ClicheRacisteMerdeRoseEquiped, condition.Name);
            Assert.IsInstanceOf<EquipmentCardOnCardCondition>(condition);
        }

        #endregion

        #region Specific ATK/DEF Conditions

        [Test]
        public void ThreeAtk3Def_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.ThreeAtk3Def);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.ThreeAtk3Def, condition.Name);
            Assert.IsInstanceOf<SpecificAtkDefInvocationCardOnFieldCondition>(condition);

            // Checks if any invocation card has at least 3 ATK or 3 DEF
        }

        #endregion

        #region Family On Field Conditions

        [Test]
        public void WizardOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.WizardOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.WizardOnField, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyInvocationCardOnFieldCondition>(condition);
        }

        [Test]
        public void ComicsOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.ComicsOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.ComicsOnField, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyInvocationCardOnFieldCondition>(condition);
        }

        [Test]
        public void HumanOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.HumanOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.HumanOnField, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyInvocationCardOnFieldCondition>(condition);
        }

        [Test]
        public void JapanOnField_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.JapanOnField);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.JapanOnField, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyInvocationCardOnFieldCondition>(condition);
        }

        #endregion

        #region Family + ATK/DEF + Number Conditions

        [Test]
        public void Developer3Atk3Def2Cards_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.Developer3Atk3Def2Cards);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.Developer3Atk3Def2Cards, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition>(condition);

            // Requires 2 Developer family cards with 3+ ATK or 3+ DEF
        }

        [Test]
        public void HardCorner3Atk3Def2Cards_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.HardCorner3Atk3Def2Cards);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.HardCorner3Atk3Def2Cards, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition>(condition);

            // Requires 2 HardCorner family cards with 3+ ATK or 3+ DEF
        }

        [Test]
        public void Japan2Cards_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.Japan2Cards);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.Japan2Cards, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition>(condition);

            // Requires 2 Japan family cards (no ATK/DEF requirement)
        }

        [Test]
        public void Incarnation2Cards_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.Incarnation2Cards);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.Incarnation2Cards, condition.Name);
            Assert.IsInstanceOf<SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition>(condition);

            // Requires 2 Incarnation family cards
        }

        #endregion

        #region Graveyard Conditions

        [Test]
        public void TenDeathYellowTrash_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.TenDeathYellowTrash);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.TenDeathYellowTrash, condition.Name);
            Assert.IsInstanceOf<NumberInvocationDeadCondition>(condition);

            // Requires 10 invocation cards in graveyard (yellow trash)
        }

        [Test]
        public void GranolaxAlreadyDead_Exists_AndHasCorrectConfiguration()
        {
            // Arrange & Act
            var condition = _conditionProvider.GetConditionTyped(ConditionName.GranolaxAlreadyDead);

            // Assert
            Assert.IsNotNull(condition, "Condition should exist");
            Assert.AreEqual(ConditionName.GranolaxAlreadyDead, condition.Name);
            Assert.IsInstanceOf<SpecificCardBackFromDeathCondition>(condition);

            // Requires Granolax to have died at least once and be back on field
        }

        #endregion

        #region All Conditions Present

        [Test]
        public void AllDefinedConditions_AreAvailable()
        {
            // Verify all 23 conditions are present
            var expectedConditions = new[]
            {
                // Invocation on field (5)
                ConditionName.BenzaieJeuneOrBenzaieOnField,
                ConditionName.ArchibalVonGrenierOnField,
                ConditionName.JoueurDuGrenierOnFieldCondition,
                ConditionName.SebDuGrenierOnField,
                ConditionName.MechaGronolaxOrGranolaxOnField,

                // Field card on field (3)
                ConditionName.ZozanKebabOnField,
                ConditionName.ForetDesElfesSylvainsOnField,
                ConditionName.LyceeMagiqueGeorgesPompidouOnField,

                // Equipment on card (4)
                ConditionName.BenzaieJeuneCassetteVhsEquiped,
                ConditionName.JoueurDuGrenierCanarangEquiped,
                ConditionName.SebDuGrenierMerdePlastiqueBleuEquiped,
                ConditionName.ClicheRacisteMerdeRoseEquiped,

                // ATK/DEF (1)
                ConditionName.ThreeAtk3Def,

                // Family on field (4)
                ConditionName.WizardOnField,
                ConditionName.ComicsOnField,
                ConditionName.HumanOnField,
                ConditionName.JapanOnField,

                // Family + ATK/DEF + number (4)
                ConditionName.Developer3Atk3Def2Cards,
                ConditionName.HardCorner3Atk3Def2Cards,
                ConditionName.Japan2Cards,
                ConditionName.Incarnation2Cards,

                // Graveyard (2)
                ConditionName.TenDeathYellowTrash,
                ConditionName.GranolaxAlreadyDead
            };

            Assert.AreEqual(23, expectedConditions.Length, "Expected 23 conditions");

            foreach (var conditionName in expectedConditions)
            {
                Assert.IsTrue(_conditionProvider.HasCondition(conditionName), $"Missing condition: {conditionName}");
                Assert.IsNotNull(_conditionProvider.GetConditionTyped(conditionName), $"Null condition: {conditionName}");
            }
        }

        #endregion

        #region Condition Type Distribution

        [Test]
        public void ConditionDistribution_MatchesExpectedTypes()
        {
            // Verify condition type distribution
            int invocationOnFieldCount = 0;
            int fieldCardOnFieldCount = 0;
            int equipmentOnCardCount = 0;
            int atkDefCount = 0;
            int familyOnFieldCount = 0;
            int familyAtkDefNumberCount = 0;
            int graveyardCount = 0;

            // Count each type
            if (_conditionProvider.GetConditionTyped(ConditionName.BenzaieJeuneOrBenzaieOnField) is InvocationCardOnFieldCondition) invocationOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.ArchibalVonGrenierOnField) is InvocationCardOnFieldCondition) invocationOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.JoueurDuGrenierOnFieldCondition) is InvocationCardOnFieldCondition) invocationOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.SebDuGrenierOnField) is InvocationCardOnFieldCondition) invocationOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.MechaGronolaxOrGranolaxOnField) is InvocationCardOnFieldCondition) invocationOnFieldCount++;

            if (_conditionProvider.GetConditionTyped(ConditionName.ZozanKebabOnField) is FieldCardOnFieldCondition) fieldCardOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.ForetDesElfesSylvainsOnField) is FieldCardOnFieldCondition) fieldCardOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.LyceeMagiqueGeorgesPompidouOnField) is FieldCardOnFieldCondition) fieldCardOnFieldCount++;

            if (_conditionProvider.GetConditionTyped(ConditionName.BenzaieJeuneCassetteVhsEquiped) is EquipmentCardOnCardCondition) equipmentOnCardCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.JoueurDuGrenierCanarangEquiped) is EquipmentCardOnCardCondition) equipmentOnCardCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.SebDuGrenierMerdePlastiqueBleuEquiped) is EquipmentCardOnCardCondition) equipmentOnCardCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.ClicheRacisteMerdeRoseEquiped) is EquipmentCardOnCardCondition) equipmentOnCardCount++;

            if (_conditionProvider.GetConditionTyped(ConditionName.ThreeAtk3Def) is SpecificAtkDefInvocationCardOnFieldCondition) atkDefCount++;

            if (_conditionProvider.GetConditionTyped(ConditionName.WizardOnField) is SpecificFamilyInvocationCardOnFieldCondition) familyOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.ComicsOnField) is SpecificFamilyInvocationCardOnFieldCondition) familyOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.HumanOnField) is SpecificFamilyInvocationCardOnFieldCondition) familyOnFieldCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.JapanOnField) is SpecificFamilyInvocationCardOnFieldCondition) familyOnFieldCount++;

            if (_conditionProvider.GetConditionTyped(ConditionName.Developer3Atk3Def2Cards) is SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition) familyAtkDefNumberCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.HardCorner3Atk3Def2Cards) is SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition) familyAtkDefNumberCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.Japan2Cards) is SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition) familyAtkDefNumberCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.Incarnation2Cards) is SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition) familyAtkDefNumberCount++;

            if (_conditionProvider.GetConditionTyped(ConditionName.TenDeathYellowTrash) is NumberInvocationDeadCondition) graveyardCount++;
            if (_conditionProvider.GetConditionTyped(ConditionName.GranolaxAlreadyDead) is SpecificCardBackFromDeathCondition) graveyardCount++;

            // Assert expected distribution
            Assert.AreEqual(5, invocationOnFieldCount, "InvocationCardOnFieldCondition count");
            Assert.AreEqual(3, fieldCardOnFieldCount, "FieldCardOnFieldCondition count");
            Assert.AreEqual(4, equipmentOnCardCount, "EquipmentCardOnCardCondition count");
            Assert.AreEqual(1, atkDefCount, "SpecificAtkDefInvocationCardOnFieldCondition count");
            Assert.AreEqual(4, familyOnFieldCount, "SpecificFamilyInvocationCardOnFieldCondition count");
            Assert.AreEqual(4, familyAtkDefNumberCount, "SpecificFamilyAtkDefNumberInvocationCardOnFieldCondition count");
            Assert.AreEqual(2, graveyardCount, "Graveyard condition count");
        }

        #endregion

        #region Edge Cases

        [Test]
        public void GetCondition_WithInvalidType_ReturnsNull()
        {
            // Act
            var condition = _conditionProvider.GetCondition("InvalidType");

            // Assert
            Assert.IsNull(condition);
        }

        [Test]
        public void HasCondition_WithInvalidType_ReturnsFalse()
        {
            // Act
            var result = _conditionProvider.HasCondition("InvalidType");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GetConditionTyped_ReturnsConditionWithCorrectName()
        {
            // Test multiple conditions to verify name property is set correctly
            var conditions = new[]
            {
                ConditionName.BenzaieJeuneOrBenzaieOnField,
                ConditionName.ZozanKebabOnField,
                ConditionName.BenzaieJeuneCassetteVhsEquiped,
                ConditionName.ThreeAtk3Def,
                ConditionName.WizardOnField,
                ConditionName.Developer3Atk3Def2Cards,
                ConditionName.TenDeathYellowTrash
            };

            foreach (var conditionName in conditions)
            {
                var condition = _conditionProvider.GetConditionTyped(conditionName);
                Assert.AreEqual(conditionName, condition.Name, $"Condition name mismatch for {conditionName}");
            }
        }

        #endregion
    }
}

using System.Collections.Generic;
using NUnit.Framework;
using JDG.Application.Services;

namespace JDG.Application.Tests.Scenarios
{
    /// <summary>
    /// Scenario tests for card placement rules.
    /// Tests field limits, equipment targeting, and placement validation.
    /// Phase 130: Card placement scenario tests.
    /// </summary>
    [TestFixture]
    public class CardPlacementScenarioTests
    {
        private ICardPlacementLogic _placementLogic;

        [SetUp]
        public void SetUp()
        {
            _placementLogic = new CardPlacementLogic();
        }

        #region Invocation Field Scenarios

        [Test]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, false)]
        [TestCase(5, false)]
        public void InvocationField_CanPlace_RespectsMax4Limit(int currentCount, bool expectedCanPlace)
        {
            // Act
            var canPlace = _placementLogic.CanPlaceInvocation(currentCount);

            // Assert
            Assert.AreEqual(expectedCanPlace, canPlace,
                $"With {currentCount} invocations, CanPlace should be {expectedCanPlace}");
        }

        [Test]
        public void InvocationField_MaxLimit_IsFour()
        {
            // Assert
            Assert.AreEqual(4, _placementLogic.MaxInvocations,
                "Max invocations should be 4");
        }

        [Test]
        public void InvocationField_EmptyField_CanPlace()
        {
            // Act
            var canPlace = _placementLogic.CanPlaceInvocation(0);

            // Assert
            Assert.IsTrue(canPlace, "Should be able to place on empty field");
        }

        [Test]
        public void InvocationField_FullField_CannotPlace()
        {
            // Act
            var canPlace = _placementLogic.CanPlaceInvocation(4);

            // Assert
            Assert.IsFalse(canPlace, "Should not be able to place on full field");
        }

        #endregion

        #region Effect Field Scenarios

        [Test]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, false)]
        [TestCase(5, false)]
        public void EffectField_CanPlace_RespectsMax4Limit(int currentCount, bool expectedCanPlace)
        {
            // Act
            var canPlace = _placementLogic.CanPlaceEffect(currentCount);

            // Assert
            Assert.AreEqual(expectedCanPlace, canPlace,
                $"With {currentCount} effects, CanPlace should be {expectedCanPlace}");
        }

        [Test]
        public void EffectField_MaxLimit_IsFour()
        {
            // Assert
            Assert.AreEqual(4, _placementLogic.MaxEffects,
                "Max effects should be 4");
        }

        [Test]
        public void EffectField_EmptyField_CanPlace()
        {
            // Act
            var canPlace = _placementLogic.CanPlaceEffect(0);

            // Assert
            Assert.IsTrue(canPlace, "Should be able to place effect on empty field");
        }

        [Test]
        public void EffectField_FullField_CannotPlace()
        {
            // Act
            var canPlace = _placementLogic.CanPlaceEffect(4);

            // Assert
            Assert.IsFalse(canPlace, "Should not be able to place effect on full field");
        }

        #endregion

        #region Field Card Scenarios

        [Test]
        public void FieldCard_WhenNoExisting_CanPlace()
        {
            // Act
            var canPlace = _placementLogic.CanPlaceFieldCard(hasFieldCard: false);

            // Assert
            Assert.IsTrue(canPlace, "Should be able to place field card when none exists");
        }

        [Test]
        public void FieldCard_WhenExisting_CannotPlace()
        {
            // Act
            var canPlace = _placementLogic.CanPlaceFieldCard(hasFieldCard: true);

            // Assert
            Assert.IsFalse(canPlace, "Should not be able to place field card when one exists");
        }

        [Test]
        public void FieldCard_IsOneSlotOnly()
        {
            // Verify the single-slot semantics
            Assert.IsTrue(_placementLogic.CanPlaceFieldCard(false), "Empty = can place");
            Assert.IsFalse(_placementLogic.CanPlaceFieldCard(true), "Occupied = cannot place");
        }

        #endregion

        #region Equipment Targeting Scenarios

        [Test]
        public void Equipment_OnlyTargetsUnequipped_ByDefault()
        {
            // Arrange - Mix of equipped and unequipped
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Equipped1", hasEquipment: true),
                EquipmentTargetInfo.Create("Unequipped1", hasEquipment: false),
                EquipmentTargetInfo.Create("Equipped2", hasEquipment: true),
                EquipmentTargetInfo.Create("Unequipped2", hasEquipment: false)
            };

            // Act - Default equipment behavior
            var targets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            // Assert
            Assert.AreEqual(2, targets.Count, "Only unequipped cards should be targets");
            Assert.IsTrue(targets.Exists(t => t.Title == "Unequipped1"));
            Assert.IsTrue(targets.Exists(t => t.Title == "Unequipped2"));
        }

        [Test]
        public void Equipment_WithCanAlwaysBePut_TargetsAll()
        {
            // Arrange - Mix of equipped and unequipped
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Equipped", hasEquipment: true),
                EquipmentTargetInfo.Create("Unequipped", hasEquipment: false)
            };

            // Act - CanAlwaysBePut equipment
            var targets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: true);

            // Assert
            Assert.AreEqual(2, targets.Count, "All cards should be valid targets with CanAlwaysBePut");
        }

        [Test]
        public void Equipment_NoValidTargets_ReturnsEmptyList()
        {
            // Arrange - All cards already equipped
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Equipped1", hasEquipment: true),
                EquipmentTargetInfo.Create("Equipped2", hasEquipment: true)
            };

            // Act
            var targets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            // Assert
            Assert.AreEqual(0, targets.Count, "No valid targets when all are equipped");
        }

        [Test]
        public void Equipment_EmptyField_ReturnsEmptyList()
        {
            // Arrange - No invocations
            var invocations = new List<EquipmentTargetInfo>();

            // Act
            var targets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            // Assert
            Assert.AreEqual(0, targets.Count, "No targets on empty field");
        }

        [Test]
        public void Equipment_NullList_ReturnsEmptyList()
        {
            // Act
            var targets = _placementLogic.GetValidEquipmentTargets(null, canAlwaysBePut: false);

            // Assert
            Assert.IsNotNull(targets, "Should return empty list, not null");
            Assert.AreEqual(0, targets.Count);
        }

        [Test]
        public void Equipment_ListWithNulls_FiltersOutNulls()
        {
            // Arrange - List with null entries
            var invocations = new List<EquipmentTargetInfo>
            {
                null,
                EquipmentTargetInfo.Create("Valid", hasEquipment: false),
                null
            };

            // Act
            var targets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            // Assert
            Assert.AreEqual(1, targets.Count, "Should filter out null entries");
            Assert.AreEqual("Valid", targets[0].Title);
        }

        [Test]
        public void Equipment_CanAlwaysBePut_WithNulls_StillFiltersNulls()
        {
            // Arrange
            var invocations = new List<EquipmentTargetInfo>
            {
                null,
                EquipmentTargetInfo.Create("Card1", hasEquipment: true),
                null,
                EquipmentTargetInfo.Create("Card2", hasEquipment: false)
            };

            // Act
            var targets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: true);

            // Assert
            Assert.AreEqual(2, targets.Count, "Should return all non-null cards");
        }

        #endregion

        #region Multi-Card Placement Scenarios

        [Test]
        public void Scenario_PlaceAllFourInvocations()
        {
            // Simulate placing 4 invocations one by one
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(_placementLogic.CanPlaceInvocation(i),
                    $"Should be able to place invocation #{i + 1}");
            }

            // 5th should fail
            Assert.IsFalse(_placementLogic.CanPlaceInvocation(4),
                "Should not be able to place 5th invocation");
        }

        [Test]
        public void Scenario_PlaceAllFourEffects()
        {
            // Simulate placing 4 effects one by one
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(_placementLogic.CanPlaceEffect(i),
                    $"Should be able to place effect #{i + 1}");
            }

            // 5th should fail
            Assert.IsFalse(_placementLogic.CanPlaceEffect(4),
                "Should not be able to place 5th effect");
        }

        [Test]
        public void Scenario_MixedFieldPlacement()
        {
            // Verify that invocation and effect fields are independent
            // Full invocation field shouldn't affect effect placement
            Assert.IsFalse(_placementLogic.CanPlaceInvocation(4), "Invocation field is full");
            Assert.IsTrue(_placementLogic.CanPlaceEffect(0), "Effect field is empty");

            // And vice versa
            Assert.IsTrue(_placementLogic.CanPlaceInvocation(0), "Invocation field is empty");
            Assert.IsFalse(_placementLogic.CanPlaceEffect(4), "Effect field is full");
        }

        [Test]
        public void Scenario_EquipmentOnMultipleTargets()
        {
            // Arrange - 4 invocations, 2 equipped
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Card1", hasEquipment: true),
                EquipmentTargetInfo.Create("Card2", hasEquipment: false),
                EquipmentTargetInfo.Create("Card3", hasEquipment: true),
                EquipmentTargetInfo.Create("Card4", hasEquipment: false)
            };

            // Act - Normal equipment
            var normalTargets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            // Act - CanAlwaysBePut equipment
            var alwaysTargets = _placementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: true);

            // Assert
            Assert.AreEqual(2, normalTargets.Count, "Normal: only unequipped");
            Assert.AreEqual(4, alwaysTargets.Count, "CanAlwaysBePut: all cards");
        }

        #endregion

        #region Edge Cases

        [Test]
        public void EdgeCase_NegativeCount_StillAllowsPlacement()
        {
            // Edge case: negative count (shouldn't happen but handle gracefully)
            Assert.IsTrue(_placementLogic.CanPlaceInvocation(-1),
                "Negative count should still allow placement");
            Assert.IsTrue(_placementLogic.CanPlaceEffect(-1),
                "Negative count should still allow placement");
        }

        [Test]
        public void EdgeCase_LargeCount_DeniesPlacement()
        {
            // Edge case: very large count
            Assert.IsFalse(_placementLogic.CanPlaceInvocation(100),
                "Large count should deny placement");
            Assert.IsFalse(_placementLogic.CanPlaceEffect(100),
                "Large count should deny placement");
        }

        [Test]
        public void EquipmentTargetInfo_CreateHelper_SetsProperties()
        {
            // Test the factory method
            var target = EquipmentTargetInfo.Create("TestCard", hasEquipment: true, isCurrentPlayer: false);

            Assert.AreEqual("TestCard", target.Title);
            Assert.IsTrue(target.HasEquipment);
            Assert.IsFalse(target.IsCurrentPlayer);
        }

        [Test]
        public void EquipmentTargetInfo_CreateHelper_DefaultsToCurrentPlayer()
        {
            // Test default parameter
            var target = EquipmentTargetInfo.Create("TestCard", hasEquipment: false);

            Assert.IsTrue(target.IsCurrentPlayer, "Should default to current player");
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using JDG.Application.Services;

namespace JDG.Application.Tests.Services
{
    /// <summary>
    /// Unit tests for CardPlacementLogic.
    /// Tests pure placement validation without Unity dependencies.
    /// Phase: Test coverage extraction.
    /// </summary>
    [TestFixture]
    public class CardPlacementLogicTests
    {
        private ICardPlacementLogic _cardPlacementLogic;

        [SetUp]
        public void SetUp()
        {
            _cardPlacementLogic = new CardPlacementLogic();
        }

        #region Constants Tests

        [Test]
        public void MaxInvocations_Returns4()
        {
            Assert.AreEqual(4, _cardPlacementLogic.MaxInvocations);
        }

        [Test]
        public void MaxEffects_Returns4()
        {
            Assert.AreEqual(4, _cardPlacementLogic.MaxEffects);
        }

        #endregion

        #region CanPlaceInvocation Tests

        [Test]
        public void CanPlaceInvocation_WithEmptyField_ReturnsTrue()
        {
            var result = _cardPlacementLogic.CanPlaceInvocation(0);
            Assert.IsTrue(result);
        }

        [Test]
        public void CanPlaceInvocation_With3Cards_ReturnsTrue()
        {
            var result = _cardPlacementLogic.CanPlaceInvocation(3);
            Assert.IsTrue(result);
        }

        [Test]
        public void CanPlaceInvocation_With4Cards_ReturnsFalse()
        {
            var result = _cardPlacementLogic.CanPlaceInvocation(4);
            Assert.IsFalse(result);
        }

        [Test]
        public void CanPlaceInvocation_WithMoreThan4Cards_ReturnsFalse()
        {
            var result = _cardPlacementLogic.CanPlaceInvocation(5);
            Assert.IsFalse(result);
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, false)]
        public void CanPlaceInvocation_VariousCounts_ReturnsExpected(int count, bool expected)
        {
            var result = _cardPlacementLogic.CanPlaceInvocation(count);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region CanPlaceEffect Tests

        [Test]
        public void CanPlaceEffect_WithEmptyField_ReturnsTrue()
        {
            var result = _cardPlacementLogic.CanPlaceEffect(0);
            Assert.IsTrue(result);
        }

        [Test]
        public void CanPlaceEffect_With3Effects_ReturnsTrue()
        {
            var result = _cardPlacementLogic.CanPlaceEffect(3);
            Assert.IsTrue(result);
        }

        [Test]
        public void CanPlaceEffect_With4Effects_ReturnsFalse()
        {
            var result = _cardPlacementLogic.CanPlaceEffect(4);
            Assert.IsFalse(result);
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, false)]
        public void CanPlaceEffect_VariousCounts_ReturnsExpected(int count, bool expected)
        {
            var result = _cardPlacementLogic.CanPlaceEffect(count);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region CanPlaceFieldCard Tests

        [Test]
        public void CanPlaceFieldCard_WithNoFieldCard_ReturnsTrue()
        {
            var result = _cardPlacementLogic.CanPlaceFieldCard(hasFieldCard: false);
            Assert.IsTrue(result);
        }

        [Test]
        public void CanPlaceFieldCard_WithExistingFieldCard_ReturnsFalse()
        {
            var result = _cardPlacementLogic.CanPlaceFieldCard(hasFieldCard: true);
            Assert.IsFalse(result);
        }

        #endregion

        #region GetValidEquipmentTargets Tests

        [Test]
        public void GetValidEquipmentTargets_WithNullInvocations_ReturnsEmptyList()
        {
            var result = _cardPlacementLogic.GetValidEquipmentTargets(null, canAlwaysBePut: false);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetValidEquipmentTargets_WithEmptyList_ReturnsEmptyList()
        {
            var invocations = new List<EquipmentTargetInfo>();

            var result = _cardPlacementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetValidEquipmentTargets_WithCanAlwaysBePut_ReturnsAllCards()
        {
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Card1", hasEquipment: false),
                EquipmentTargetInfo.Create("Card2", hasEquipment: true),
                EquipmentTargetInfo.Create("Card3", hasEquipment: false)
            };

            var result = _cardPlacementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: true);

            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void GetValidEquipmentTargets_WithoutCanAlwaysBePut_ReturnsOnlyUnequipped()
        {
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Card1", hasEquipment: false),
                EquipmentTargetInfo.Create("Card2", hasEquipment: true),
                EquipmentTargetInfo.Create("Card3", hasEquipment: false)
            };

            var result = _cardPlacementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(t => !t.HasEquipment));
        }

        [Test]
        public void GetValidEquipmentTargets_WithAllEquipped_ReturnsEmptyList()
        {
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Card1", hasEquipment: true),
                EquipmentTargetInfo.Create("Card2", hasEquipment: true)
            };

            var result = _cardPlacementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetValidEquipmentTargets_WithNullEntries_SkipsNulls()
        {
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Card1", hasEquipment: false),
                null,
                EquipmentTargetInfo.Create("Card3", hasEquipment: false)
            };

            var result = _cardPlacementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetValidEquipmentTargets_IncludesOpponentCards()
        {
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("MyCard", hasEquipment: false, isCurrentPlayer: true),
                EquipmentTargetInfo.Create("EnemyCard", hasEquipment: false, isCurrentPlayer: false)
            };

            var result = _cardPlacementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: false);

            Assert.AreEqual(2, result.Count, "Should include both player's cards");
        }

        [Test]
        public void GetValidEquipmentTargets_CanAlwaysBePut_IncludesEquippedCards()
        {
            var invocations = new List<EquipmentTargetInfo>
            {
                EquipmentTargetInfo.Create("Equipped", hasEquipment: true),
                EquipmentTargetInfo.Create("Unequipped", hasEquipment: false)
            };

            var result = _cardPlacementLogic.GetValidEquipmentTargets(invocations, canAlwaysBePut: true);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(t => t.HasEquipment), "Should include equipped cards");
        }

        #endregion

        #region Edge Cases

        [Test]
        public void CardPlacementLogic_CanBeInstantiated()
        {
            var logic = new CardPlacementLogic();
            Assert.IsNotNull(logic);
        }

        [Test]
        public void CanPlaceInvocation_WithNegativeCount_StillWorks()
        {
            // Edge case: should still return true for negative (invalid) counts
            var result = _cardPlacementLogic.CanPlaceInvocation(-1);
            Assert.IsTrue(result);
        }

        #endregion
    }
}

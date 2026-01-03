using NUnit.Framework;
using JDG.Application.Services;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using Services;
using System.Linq;

// Alias to avoid namespace conflicts
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Unit tests for CardSyncService.
    /// Phase 141: Tests the domain Card → InGameInvocationCard synchronization.
    ///
    /// Note: Full integration tests require PlayMode because CardSyncService
    /// depends on concrete InGameInvocationCard (MonoBehaviour). These tests
    /// verify the service interface and domain Card creation logic.
    /// </summary>
    [TestFixture]
    public class CardSyncServiceTests
    {
        private CardSyncService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new CardSyncService();
        }

        #region CreateLinkedCard Tests

        [Test]
        public void CreateLinkedCard_WithNullCard_ReturnsNull()
        {
            // Act
            var result = _service.CreateLinkedCard(null);

            // Assert
            Assert.IsNull(result);
        }

        // Note: Full CreateLinkedCard tests require PlayMode because
        // InGameInvocationCard is a MonoBehaviour

        #endregion

        #region SyncCardState Tests

        [Test]
        public void SyncCardState_WithNullDomainCard_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.SyncCardState(null));
        }

        [Test]
        public void SyncCardState_WithUnmappedCard_DoesNotThrow()
        {
            // Arrange
            var domainCard = CreateTestDomainCard();

            // Act & Assert - should not throw for unmapped card
            Assert.DoesNotThrow(() => _service.SyncCardState(domainCard));
        }

        #endregion

        #region ClearMapping Tests

        [Test]
        public void ClearMapping_WithNullCard_DoesNotThrow()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => _service.ClearMapping(null));
        }

        [Test]
        public void ClearMapping_WithUnmappedCard_DoesNotThrow()
        {
            // Arrange
            var domainCard = CreateTestDomainCard();

            // Act & Assert - should not throw for unmapped card
            Assert.DoesNotThrow(() => _service.ClearMapping(domainCard));
        }

        #endregion

        #region Domain Card Verification Tests

        [Test]
        public void DomainCard_CreateInvocation_HasCorrectStats()
        {
            // Arrange & Act
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "Test Description",
                detailedDescription: "Detailed",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Assert
            Assert.IsNotNull(domainCard);
            Assert.IsNotNull(domainCard.Stats);
            Assert.AreEqual(100, domainCard.Stats.Value.Attack);
            Assert.AreEqual(50, domainCard.Stats.Value.Defense);
        }

        [Test]
        public void DomainCard_ModifyStats_UpdatesValues()
        {
            // Arrange
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "",
                detailedDescription: "",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Act
            var result = domainCard.ModifyStats(10, 5);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(110, domainCard.Stats.Value.Attack);
            Assert.AreEqual(55, domainCard.Stats.Value.Defense);
        }

        [Test]
        public void DomainCard_SetCancelEffect_UpdatesFlag()
        {
            // Arrange
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "",
                detailedDescription: "",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Act
            domainCard.SetCancelEffect(true);

            // Assert
            Assert.IsTrue(domainCard.CancelEffect);
        }

        [Test]
        public void DomainCard_EnableDirectAttack_UpdatesFlag()
        {
            // Arrange
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "",
                detailedDescription: "",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Act
            domainCard.EnableDirectAttack();

            // Assert
            Assert.IsTrue(domainCard.CanDirectAttack);
        }

        [Test]
        public void DomainCard_SetCantBeAttacked_UpdatesFlag()
        {
            // Arrange
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "",
                detailedDescription: "",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Act
            domainCard.SetCantBeAttacked(true);

            // Assert
            Assert.IsTrue(domainCard.CantBeAttacked);
        }

        [Test]
        public void DomainCard_IncrementTimesRevived_UpdatesCounter()
        {
            // Arrange
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "",
                detailedDescription: "",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Act
            domainCard.IncrementTimesRevived();
            domainCard.IncrementTimesRevived();

            // Assert
            Assert.AreEqual(2, domainCard.TimesRevived);
        }

        [Test]
        public void DomainCard_SetBonusAttacks_UpdatesValue()
        {
            // Arrange
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "",
                detailedDescription: "",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Act
            domainCard.SetBonusAttacks(2);

            // Assert
            Assert.AreEqual(2, domainCard.BonusAttacks);
        }

        [Test]
        public void DomainCard_BlockAttack_UpdatesFlag()
        {
            // Arrange
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "",
                detailedDescription: "",
                attack: 100,
                defense: 50,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );

            // Act
            domainCard.BlockAttack();

            // Assert
            Assert.IsTrue(domainCard.AttackBlocked);
        }

        #endregion

        #region Helper Methods

        private static Card CreateTestDomainCard(int attack = 100, int defense = 50)
        {
            return Card.CreateInvocation(
                id: CardId.New(),
                title: "Test Card",
                description: "Test Description",
                detailedDescription: "Detailed Description",
                attack: attack,
                defense: defense,
                families: new[] { DomainCardFamily.Developer },
                affectedByEffect: true
            );
        }

        #endregion
    }
}

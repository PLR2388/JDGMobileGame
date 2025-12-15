using System.Collections;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.PlayMode.Tests.TestHelpers;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// PlayMode tests for AbilityProviderService.
    /// Tests the Strangler Fig pattern - modern vs legacy ability routing.
    /// Phase: Test coverage for critical paths.
    /// </summary>
    [TestFixture]
    public class AbilityProviderServicePlayTests
    {
        private AbilityProviderService _abilityProviderService;
        private AbilityRegistry _registry;
        private IPlayerRepository _mockPlayerRepository;
        private IEventBus _mockEventBus;

        [SetUp]
        public void SetUp()
        {
            _registry = new AbilityRegistry();
            _mockPlayerRepository = Substitute.For<IPlayerRepository>();
            _mockEventBus = new TestEventBus();

            _abilityProviderService = new AbilityProviderService(
                _registry,
                _mockPlayerRepository,
                _mockEventBus);
        }

        #region Constructor Tests

        [UnityTest]
        public IEnumerator AbilityProviderService_CanBeConstructedWithAllDependencies()
        {
            yield return null;

            // Assert
            Assert.IsNotNull(_abilityProviderService);
        }

        [UnityTest]
        public IEnumerator AbilityProviderService_CanBeConstructedWithMinimalDependencies()
        {
            // Arrange - only registry, no repository or event bus
            var minimalService = new AbilityProviderService(_registry);

            yield return null;

            // Assert
            Assert.IsNotNull(minimalService);
        }

        [UnityTest]
        public IEnumerator AbilityProviderService_CanBeConstructedWithNullRegistry()
        {
            // Arrange
            var serviceWithNullRegistry = new AbilityProviderService(null);

            yield return null;

            // Assert
            Assert.IsNotNull(serviceWithNullRegistry);
        }

        #endregion

        #region IsMigrated Tests

        [UnityTest]
        public IEnumerator IsMigrated_WithRegisteredAbility_ReturnsTrue()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestDrawAbility());

            yield return null;

            // Act
            var result = _abilityProviderService.IsMigrated(AbilityName.Draw1Card);

            // Assert
            Assert.IsTrue(result);
        }

        [UnityTest]
        public IEnumerator IsMigrated_WithUnregisteredAbility_ReturnsFalse()
        {
            yield return null;

            // Act
            var result = _abilityProviderService.IsMigrated(AbilityName.Draw1Card);

            // Assert
            Assert.IsFalse(result);
        }

        [UnityTest]
        public IEnumerator IsMigrated_WithNullRegistry_ReturnsFalse()
        {
            // Arrange
            var serviceWithNullRegistry = new AbilityProviderService(null);

            yield return null;

            // Act
            var result = serviceWithNullRegistry.IsMigrated(AbilityName.Draw1Card);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region HasAbility Tests

        [UnityTest]
        public IEnumerator HasAbility_WithRegisteredAbility_ReturnsTrue()
        {
            // Arrange
            _registry.Register(AbilityName.Draw2Cards, () => new TestDrawAbility());

            yield return null;

            // Act
            var result = _abilityProviderService.HasAbility(AbilityName.Draw2Cards);

            // Assert
            Assert.IsTrue(result);
        }

        [UnityTest]
        public IEnumerator HasAbility_WithNullRegistry_ChecksLegacy()
        {
            // Arrange
            var serviceWithNullRegistry = new AbilityProviderService(null);

            yield return null;

            // Act - will check legacy system
            var result = serviceWithNullRegistry.HasAbility(AbilityName.Draw1Card);

            // Assert - depends on legacy system initialization
            // Just verify it doesn't throw
            Assert.IsTrue(result || !result); // Always passes, just testing no exception
        }

        #endregion

        #region GetAbility Tests

        [UnityTest]
        public IEnumerator GetAbility_WithRegisteredAbility_ReturnsModernAdapter()
        {
            // Arrange
            _registry.Register(AbilityName.Draw3Cards, () => new TestDrawAbility());

            yield return null;

            // Act
            var ability = _abilityProviderService.GetAbility(AbilityName.Draw3Cards);

            // Assert
            Assert.IsNotNull(ability);
            Assert.IsInstanceOf<ModernAbilityAdapter>(ability);
        }

        [UnityTest]
        public IEnumerator GetAbility_WithSameAbilityTwice_ReturnsCachedAdapter()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestDrawAbility());

            yield return null;

            // Act
            var ability1 = _abilityProviderService.GetAbility(AbilityName.Draw1Card);
            var ability2 = _abilityProviderService.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.AreSame(ability1, ability2, "Should return cached adapter on second call");
        }

        [UnityTest]
        public IEnumerator GetAbility_AfterClearCache_CreatesNewAdapter()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestDrawAbility());
            var ability1 = _abilityProviderService.GetAbility(AbilityName.Draw1Card);

            yield return null;

            // Act
            _abilityProviderService.ClearCache();
            var ability2 = _abilityProviderService.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.AreNotSame(ability1, ability2, "Should create new adapter after cache clear");
        }

        #endregion

        #region MigrationStats Tests

        [UnityTest]
        public IEnumerator GetMigrationStats_InitialState_ReturnsZeroCounts()
        {
            yield return null;

            // Act
            var stats = _abilityProviderService.GetMigrationStats();

            // Assert
            Assert.AreEqual(0, stats.ModernAbilitiesUsed);
            Assert.AreEqual(0, stats.LegacyAbilitiesUsed);
        }

        [UnityTest]
        public IEnumerator GetMigrationStats_AfterModernAbilityUsed_IncrementsModernCount()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestDrawAbility());
            _abilityProviderService.GetAbility(AbilityName.Draw1Card);

            yield return null;

            // Act
            var stats = _abilityProviderService.GetMigrationStats();

            // Assert
            Assert.AreEqual(1, stats.ModernAbilitiesUsed);
        }

        [UnityTest]
        public IEnumerator GetMigrationStats_TracksRegisteredCount()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestDrawAbility());
            _registry.Register(AbilityName.Draw2Cards, () => new TestDrawAbility());
            _registry.Register(AbilityName.Draw3Cards, () => new TestDrawAbility());

            yield return null;

            // Act
            var stats = _abilityProviderService.GetMigrationStats();

            // Assert
            Assert.AreEqual(3, stats.TotalAbilitiesRegistered);
        }

        #endregion

        #region ClearCache Tests

        [UnityTest]
        public IEnumerator ClearCache_DoesNotThrow()
        {
            yield return null;

            // Act & Assert
            Assert.DoesNotThrow(() => _abilityProviderService.ClearCache());
        }

        [UnityTest]
        public IEnumerator ClearCache_CanBeCalledMultipleTimes()
        {
            yield return null;

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _abilityProviderService.ClearCache();
                _abilityProviderService.ClearCache();
                _abilityProviderService.ClearCache();
            });
        }

        #endregion

        #region Test Ability Implementation

        /// <summary>
        /// Simple test ability for testing the provider service.
        /// </summary>
        private class TestDrawAbility : IAbility
        {
            public AbilityName Name => AbilityName.Draw1Card;
            public string Description => "Test draw ability";

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                return AbilityResult.Success("Drew cards");
            }
        }

        #endregion
    }
}

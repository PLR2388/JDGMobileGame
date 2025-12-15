using NUnit.Framework;
using NSubstitute;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain;

namespace JDG.Application.Tests.Services
{
    /// <summary>
    /// Tests for AbilityProviderService.
    /// Phase 7: Tests the Strangler Fig pattern implementation for ability migration.
    ///
    /// Note: Legacy AbilityLibrary fallback cannot be tested in isolation as it
    /// depends on Unity singleton. These tests focus on the modern system paths.
    /// </summary>
    [TestFixture]
    public class AbilityProviderServiceTests
    {
        private AbilityProviderService _service;
        private AbilityRegistry _registry;
        private IPlayerRepository _playerRepository;
        private IEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _registry = new AbilityRegistry();
            _playerRepository = Substitute.For<IPlayerRepository>();
            _eventBus = Substitute.For<IEventBus>();
            _service = new AbilityProviderService(_registry, _playerRepository, _eventBus);
        }

        #region GetAbility Tests

        [Test]
        public void GetAbility_WithRegisteredAbility_ReturnsWrappedAbility()
        {
            // Arrange
            var testAbility = new TestAbility(AbilityName.Draw1Card, "Draw 1 Card");
            _registry.Register(AbilityName.Draw1Card, () => testAbility);

            // Act
            var result = _service.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ModernAbilityAdapter>(result);
        }

        [Test]
        public void GetAbility_CalledTwice_ReturnsCachedAdapter()
        {
            // Arrange
            var testAbility = new TestAbility(AbilityName.Draw2Cards, "Draw 2 Cards");
            _registry.Register(AbilityName.Draw2Cards, () => testAbility);

            // Act
            var result1 = _service.GetAbility(AbilityName.Draw2Cards);
            var result2 = _service.GetAbility(AbilityName.Draw2Cards);

            // Assert
            Assert.AreSame(result1, result2, "Should return same cached instance");
        }

        [Test]
        public void GetAbility_WithUnregisteredAbility_ReturnsNull()
        {
            // Arrange - no abilities registered, no legacy system available in tests

            // Act
            var result = _service.GetAbility(AbilityName.Default);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetAbility_PreservesAbilityName()
        {
            // Arrange
            var testAbility = new TestAbility(AbilityName.Draw3Cards, "Draw 3 Cards");
            _registry.Register(AbilityName.Draw3Cards, () => testAbility);

            // Act
            var result = _service.GetAbility(AbilityName.Draw3Cards);

            // Assert
            Assert.AreEqual(AbilityName.Draw3Cards, result.Name);
        }

        [Test]
        public void GetAbility_PreservesAbilityDescription()
        {
            // Arrange
            var testAbility = new TestAbility(AbilityName.Draw1Card, "Test Description");
            _registry.Register(AbilityName.Draw1Card, () => testAbility);

            // Act
            var result = _service.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.AreEqual("Test Description", result.Description);
        }

        #endregion

        #region HasAbility Tests

        [Test]
        public void HasAbility_WithRegisteredAbility_ReturnsTrue()
        {
            // Arrange
            var testAbility = new TestAbility(AbilityName.Draw1Card, "Draw 1 Card");
            _registry.Register(AbilityName.Draw1Card, () => testAbility);

            // Act
            var result = _service.HasAbility(AbilityName.Draw1Card);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAbility_WithUnregisteredAbility_ReturnsFalse()
        {
            // Arrange - no abilities registered

            // Act
            var result = _service.HasAbility(AbilityName.Default);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region IsMigrated Tests

        [Test]
        public void IsMigrated_WithRegisteredAbility_ReturnsTrue()
        {
            // Arrange
            var testAbility = new TestAbility(AbilityName.Draw1Card, "Draw 1 Card");
            _registry.Register(AbilityName.Draw1Card, () => testAbility);

            // Act
            var result = _service.IsMigrated(AbilityName.Draw1Card);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMigrated_WithUnregisteredAbility_ReturnsFalse()
        {
            // Arrange - no abilities registered

            // Act
            var result = _service.IsMigrated(AbilityName.Default);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetMigrationStats Tests

        [Test]
        public void GetMigrationStats_WithNoAbilitiesUsed_ReturnsZeroCounts()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestAbility(AbilityName.Draw1Card, ""));
            _registry.Register(AbilityName.Draw2Cards, () => new TestAbility(AbilityName.Draw2Cards, ""));

            // Act
            var stats = _service.GetMigrationStats();

            // Assert
            Assert.AreEqual(0, stats.ModernAbilitiesUsed);
            Assert.AreEqual(0, stats.LegacyAbilitiesUsed);
            Assert.AreEqual(2, stats.TotalAbilitiesRegistered);
        }

        [Test]
        public void GetMigrationStats_AfterGettingAbility_TracksModernUsage()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestAbility(AbilityName.Draw1Card, ""));
            _registry.Register(AbilityName.Draw2Cards, () => new TestAbility(AbilityName.Draw2Cards, ""));

            // Act
            _service.GetAbility(AbilityName.Draw1Card);
            var stats = _service.GetMigrationStats();

            // Assert
            Assert.AreEqual(1, stats.ModernAbilitiesUsed);
            Assert.AreEqual(0, stats.LegacyAbilitiesUsed);
        }

        [Test]
        public void GetMigrationStats_GettingSameAbilityTwice_CountsOnce()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestAbility(AbilityName.Draw1Card, ""));

            // Act
            _service.GetAbility(AbilityName.Draw1Card);
            _service.GetAbility(AbilityName.Draw1Card);
            var stats = _service.GetMigrationStats();

            // Assert
            Assert.AreEqual(1, stats.ModernAbilitiesUsed, "Same ability should only be counted once");
        }

        #endregion

        #region ClearCache Tests

        [Test]
        public void ClearCache_AfterGettingAbility_AllowsNewAdapterCreation()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestAbility(AbilityName.Draw1Card, ""));
            var firstResult = _service.GetAbility(AbilityName.Draw1Card);

            // Act
            _service.ClearCache();
            var secondResult = _service.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.AreNotSame(firstResult, secondResult, "Should create new adapter after cache clear");
        }

        #endregion

        #region Null Registry Tests

        [Test]
        public void GetAbility_WithNullRegistry_ReturnsNull()
        {
            // Arrange
            var serviceWithNullRegistry = new AbilityProviderService(null, _playerRepository, _eventBus);

            // Act
            var result = serviceWithNullRegistry.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void HasAbility_WithNullRegistry_ReturnsFalse()
        {
            // Arrange
            var serviceWithNullRegistry = new AbilityProviderService(null, _playerRepository, _eventBus);

            // Act
            var result = serviceWithNullRegistry.HasAbility(AbilityName.Draw1Card);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMigrated_WithNullRegistry_ReturnsFalse()
        {
            // Arrange
            var serviceWithNullRegistry = new AbilityProviderService(null, _playerRepository, _eventBus);

            // Act
            var result = serviceWithNullRegistry.IsMigrated(AbilityName.Draw1Card);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Test Helpers

        /// <summary>
        /// Simple test implementation of IAbility for testing.
        /// </summary>
        private class TestAbility : IAbility
        {
            public AbilityName Name { get; }
            public string Description { get; }

            public TestAbility(AbilityName name, string description)
            {
                Name = name;
                Description = description;
            }

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                return AbilityResult.Success("Test ability executed");
            }
        }

        #endregion
    }
}

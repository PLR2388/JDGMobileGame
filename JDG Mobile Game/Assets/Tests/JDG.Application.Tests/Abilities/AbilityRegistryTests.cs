using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Domain;

namespace JDG.Application.Tests.Abilities
{
    [TestFixture]
    public class AbilityRegistryTests
    {
        private AbilityRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _registry = new AbilityRegistry();
        }

        [Test]
        public void Register_AddsAbilityFactory()
        {
            // Arrange
            var ability = new TestAbility(AbilityName.Draw1Card);

            // Act
            _registry.Register(AbilityName.Draw1Card, () => ability);

            // Assert
            Assert.IsTrue(_registry.IsRegistered(AbilityName.Draw1Card));
        }

        [Test]
        public void Register_DuplicateAbility_ThrowsException()
        {
            // Arrange
            var ability = new TestAbility(AbilityName.Draw1Card);
            _registry.Register(AbilityName.Draw1Card, () => ability);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                _registry.Register(AbilityName.Draw1Card, () => ability));
        }

        [Test]
        public void GetAbility_ReturnsRegisteredAbility()
        {
            // Arrange
            var ability = new TestAbility(AbilityName.Draw2Cards);
            _registry.Register(AbilityName.Draw2Cards, () => ability);

            // Act
            var result = _registry.GetAbility(AbilityName.Draw2Cards);

            // Assert
            Assert.AreSame(ability, result);
        }

        [Test]
        public void GetAbility_UnregisteredAbility_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                _registry.GetAbility(AbilityName.Draw1Card));
        }

        [Test]
        public void GetAbility_CachesAbility()
        {
            // Arrange
            int factoryCallCount = 0;
            _registry.Register(AbilityName.Draw1Card, () =>
            {
                factoryCallCount++;
                return new TestAbility(AbilityName.Draw1Card);
            });

            // Act
            var ability1 = _registry.GetAbility(AbilityName.Draw1Card);
            var ability2 = _registry.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.AreEqual(1, factoryCallCount);
            Assert.AreSame(ability1, ability2);
        }

        [Test]
        public void IsRegistered_ReturnsFalseForUnregisteredAbility()
        {
            // Act & Assert
            Assert.IsFalse(_registry.IsRegistered(AbilityName.Draw3Cards));
        }

        [Test]
        public void GetRegisteredAbilities_ReturnsAllRegisteredNames()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestAbility(AbilityName.Draw1Card));
            _registry.Register(AbilityName.Draw2Cards, () => new TestAbility(AbilityName.Draw2Cards));

            // Act
            var registeredAbilities = _registry.GetRegisteredAbilities().ToList();

            // Assert
            Assert.AreEqual(2, registeredAbilities.Count);
            Assert.Contains(AbilityName.Draw1Card, registeredAbilities);
            Assert.Contains(AbilityName.Draw2Cards, registeredAbilities);
        }

        [Test]
        public void ClearCache_RemovesCachedAbilities()
        {
            // Arrange
            int factoryCallCount = 0;
            _registry.Register(AbilityName.Draw1Card, () =>
            {
                factoryCallCount++;
                return new TestAbility(AbilityName.Draw1Card);
            });

            _registry.GetAbility(AbilityName.Draw1Card); // Cache the ability

            // Act
            _registry.ClearCache();
            _registry.GetAbility(AbilityName.Draw1Card); // Should call factory again

            // Assert
            Assert.AreEqual(2, factoryCallCount);
        }

        [Test]
        public void Unregister_RemovesAbility()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestAbility(AbilityName.Draw1Card));

            // Act
            _registry.Unregister(AbilityName.Draw1Card);

            // Assert
            Assert.IsFalse(_registry.IsRegistered(AbilityName.Draw1Card));
        }

        [Test]
        public void Unregister_RemovesCachedAbility()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestAbility(AbilityName.Draw1Card));
            _registry.GetAbility(AbilityName.Draw1Card); // Cache it

            // Act
            _registry.Unregister(AbilityName.Draw1Card);

            // Assert
            Assert.Throws<KeyNotFoundException>(() => _registry.GetAbility(AbilityName.Draw1Card));
        }
    }

    // Test double for IAbility
    public class TestAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description => $"Test ability: {Name}";

        public TestAbility(AbilityName name)
        {
            Name = name;
        }

        public bool CanActivate(AbilityContext context) => true;

        public AbilityResult Execute(AbilityContext context) => AbilityResult.Success("Test executed");
    }
}

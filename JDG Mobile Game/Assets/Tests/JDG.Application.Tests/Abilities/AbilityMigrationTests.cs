using NUnit.Framework;
using NSubstitute;
using JDG.Application.Abilities;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.TestUtilities;

namespace JDG.Application.Tests.Abilities
{
    /// <summary>
    /// Tests for the ability migration infrastructure.
    /// Phase 7.1: Tests for LegacyAbilityAdapter and AbilityMigrationService.
    /// </summary>
    [TestFixture]
    public class AbilityMigrationTests
    {
        [Test]
        public void AbilityResult_NeedsLegacyExecution_SetsCorrectFlags()
        {
            // Act
            var result = AbilityResult.NeedsLegacyExecution("Test message");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.RequiresLegacyExecution);
            Assert.AreEqual("Test message", result.Message);
        }

        [Test]
        public void AbilityResult_Success_DoesNotRequireLegacy()
        {
            // Act
            var result = AbilityResult.Success("Success!");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.RequiresLegacyExecution);
        }

        [Test]
        public void AbilityResult_Failure_DoesNotRequireLegacy()
        {
            // Act
            var result = AbilityResult.Failure("Failed");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsFalse(result.RequiresLegacyExecution);
        }

        [Test]
        public void AbilityResult_NeedsUserInput_HasCorrectFlags()
        {
            // Act
            var result = AbilityResult.NeedsUserInput("Select target");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.RequiresUserInput);
            Assert.IsFalse(result.RequiresLegacyExecution);
        }
    }

    /// <summary>
    /// Tests for AbilityRegistry.
    /// </summary>
    [TestFixture]
    public class AbilityRegistryAdvancedTests
    {
        private AbilityRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _registry = new AbilityRegistry();
        }

        [Test]
        public void Register_WithFactory_CachesAfterFirstCall()
        {
            // Arrange
            int callCount = 0;
            _registry.Register(AbilityName.Draw1Card, () =>
            {
                callCount++;
                return new TestAbility(AbilityName.Draw1Card);
            });

            // Act
            var ability1 = _registry.GetAbility(AbilityName.Draw1Card);
            var ability2 = _registry.GetAbility(AbilityName.Draw1Card);

            // Assert - Factory should be called only once due to caching
            Assert.AreEqual(1, callCount);
            Assert.AreSame(ability1, ability2);
        }

        [Test]
        public void GetAbility_Unregistered_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() =>
                _registry.GetAbility(AbilityName.Draw1Card));
        }

        [Test]
        public void IsRegistered_Registered_ReturnsTrue()
        {
            // Arrange
            _registry.Register(AbilityName.Draw2Cards, () => new TestAbility(AbilityName.Draw2Cards));

            // Act & Assert
            Assert.IsTrue(_registry.IsRegistered(AbilityName.Draw2Cards));
        }

        [Test]
        public void IsRegistered_Unregistered_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(_registry.IsRegistered(AbilityName.Draw2Cards));
        }

        private class TestAbility : IAbility
        {
            public TestAbility(AbilityName name)
            {
                Name = name;
            }

            public AbilityName Name { get; }
            public string Description => "Test ability";

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                return AbilityResult.Success("Test executed");
            }
        }
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Domain;
using JDG.Domain.ValueObjects;
// Alias to avoid conflict with legacy CardFactory in global namespace
using TestCardFactory = JDG.TestUtilities.CardFactory;
using JDG.TestUtilities;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// Play Mode tests for the Ability system.
    /// Tests ability registration, retrieval, and execution in Unity runtime.
    /// Phase 11: Play Mode test implementation.
    /// </summary>
    [TestFixture]
    public class AbilitySystemPlayTests
    {
        private AbilityRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _registry = new AbilityRegistry();
        }

        [UnityTest]
        public IEnumerator AbilityRegistry_RegisterAndRetrieve_WorksInPlayMode()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestDrawAbility(1));

            yield return null; // Wait one frame

            // Act
            var ability = _registry.GetAbility(AbilityName.Draw1Card);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.Draw1Card, ability.Name);
        }

        [UnityTest]
        public IEnumerator AbilityRegistry_IsRegistered_ReturnsCorrectValue()
        {
            // Arrange
            _registry.Register(AbilityName.Draw2Cards, () => new TestDrawAbility(2));

            yield return null;

            // Assert
            Assert.IsTrue(_registry.IsRegistered(AbilityName.Draw2Cards));
            Assert.IsFalse(_registry.IsRegistered(AbilityName.Draw3Cards));
        }

        [UnityTest]
        public IEnumerator AbilityExecution_WithValidContext_Succeeds()
        {
            // Arrange
            var ability = new TestDrawAbility(2);
            var sourceCard = TestCardFactory.CreateInvocation("Test Card");
            var context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                sourceCard,
                AbilityName.Draw2Cards
            );

            yield return null;

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Drew 2 card(s)", result.Message);
        }

        [UnityTest]
        public IEnumerator AbilityResult_Success_HasCorrectProperties()
        {
            yield return null;

            // Act
            var result = AbilityResult.Success("Operation completed");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Operation completed", result.Message);
            Assert.IsFalse(result.RequiresUserInput);
            Assert.IsFalse(result.RequiresLegacyExecution);
        }

        [UnityTest]
        public IEnumerator AbilityResult_Failure_HasCorrectProperties()
        {
            yield return null;

            // Act
            var result = AbilityResult.Failure("Something went wrong");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Something went wrong", result.Message);
        }

        [UnityTest]
        public IEnumerator AbilityResult_NeedsUserInput_HasCorrectProperties()
        {
            yield return null;

            // Act
            var result = AbilityResult.NeedsUserInput("Select a target");

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
            Assert.AreEqual("Select a target", result.Message);
        }

        [UnityTest]
        public IEnumerator AbilityContext_CarriesCorrectState()
        {
            // Arrange
            var sourceCard = TestCardFactory.CreateInvocation("Source");
            var targetCard = TestCardFactory.CreateInvocation("Target");

            yield return null;

            // Act
            var context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                sourceCard,
                AbilityName.KillOpponentInvocation
            );
            context.TargetCard = targetCard;

            // Assert
            Assert.AreEqual(PlayerId.Player1, context.CurrentPlayerId);
            Assert.AreEqual(PlayerId.Player2, context.OpponentPlayerId);
            Assert.AreEqual(sourceCard, context.SourceCard);
            Assert.AreEqual(targetCard, context.TargetCard);
            Assert.AreEqual(AbilityName.KillOpponentInvocation, context.AbilityName);
        }

        [UnityTest]
        public IEnumerator AbilityRegistry_FactoryIsCached_ReturnsSameInstance()
        {
            // Arrange
            var callCount = 0;
            _registry.Register(AbilityName.Draw1Card, () =>
            {
                callCount++;
                return new TestDrawAbility(1);
            });

            yield return null;

            // Act - Get ability twice
            var ability1 = _registry.GetAbility(AbilityName.Draw1Card);
            var ability2 = _registry.GetAbility(AbilityName.Draw1Card);

            // Assert - Factory should only be called once
            Assert.AreEqual(1, callCount);
            Assert.AreSame(ability1, ability2);
        }

        [UnityTest]
        public IEnumerator MultipleAbilities_RegisteredAndRetrieved_InPlayMode()
        {
            // Arrange
            _registry.Register(AbilityName.Draw1Card, () => new TestDrawAbility(1));
            _registry.Register(AbilityName.Draw2Cards, () => new TestDrawAbility(2));
            _registry.Register(AbilityName.Draw3Cards, () => new TestDrawAbility(3));

            yield return null;

            // Act & Assert
            var draw1 = _registry.GetAbility(AbilityName.Draw1Card);
            var draw2 = _registry.GetAbility(AbilityName.Draw2Cards);
            var draw3 = _registry.GetAbility(AbilityName.Draw3Cards);

            Assert.AreEqual(AbilityName.Draw1Card, draw1.Name);
            Assert.AreEqual(AbilityName.Draw2Cards, draw2.Name);
            Assert.AreEqual(AbilityName.Draw3Cards, draw3.Name);
        }

        #region Test Ability Implementation

        /// <summary>
        /// Test ability for drawing cards.
        /// </summary>
        private class TestDrawAbility : IAbility
        {
            private readonly int _cardsToDraw;

            public TestDrawAbility(int cardsToDraw)
            {
                _cardsToDraw = cardsToDraw;
            }

            public AbilityName Name => _cardsToDraw switch
            {
                1 => AbilityName.Draw1Card,
                2 => AbilityName.Draw2Cards,
                3 => AbilityName.Draw3Cards,
                _ => AbilityName.Default
            };

            public string Description => $"Draw {_cardsToDraw} card(s)";

            public bool CanActivate(AbilityContext context) => true;

            public AbilityResult Execute(AbilityContext context)
            {
                return AbilityResult.Success($"Drew {_cardsToDraw} card(s)");
            }
        }

        #endregion
    }
}

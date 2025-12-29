using System;
using NUnit.Framework;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Tests for EffectAbilityProviderService.
    /// Phase 84: Created to verify provider service after EffectAbilityLibrary deletion.
    /// </summary>
    [TestFixture]
    public class EffectAbilityProviderServiceTests
    {
        private EffectAbilityProviderService _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new EffectAbilityProviderService();
        }

        [Test]
        public void GetAbility_WithValidName_ReturnsAbility()
        {
            // Arrange
            var abilityName = EffectAbilityName.LimitHandCardTo5;

            // Act
            var ability = _provider.GetAbility(abilityName);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(abilityName, ability.Name);
        }

        [Test]
        public void GetAbility_WithSwitchAtkDef_ReturnsCorrectAbility()
        {
            // Act
            var ability = _provider.GetAbility(EffectAbilityName.SwitchAtkDef);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(EffectAbilityName.SwitchAtkDef, ability.Name);
        }

        [Test]
        public void GetAbility_WithDirectAttackIfUnder5HP_ReturnsCorrectAbility()
        {
            // Act
            var ability = _provider.GetAbility(EffectAbilityName.DirectAttackIfUnder5HP);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(EffectAbilityName.DirectAttackIfUnder5HP, ability.Name);
        }

        [Test]
        public void HasAbility_WithExistingAbility_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(EffectAbilityName.LimitHandCardTo5);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAbility_WithControl1OpponentInvocationCard_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(EffectAbilityName.Control1OpponentInvocationCard);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasAbility_WithDoubleAttackPerTurn_ReturnsTrue()
        {
            // Act
            var result = _provider.HasAbility(EffectAbilityName.DoubleAttackPerTurn);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AllDefinedAbilities_AreAvailable()
        {
            // Verify all known effect abilities are present
            var expectedAbilities = new[]
            {
                EffectAbilityName.LimitHandCardTo5,
                EffectAbilityName.Lose2Point5StarsByInvocations,
                EffectAbilityName.ApplyFamilyFieldToInvocations,
                EffectAbilityName.DestroyAllCardsUnderManyConditions,
                EffectAbilityName.GetHPFor1Sacrifice3ATKDEFCondition,
                EffectAbilityName.DirectAttackIfUnder5HP,
                EffectAbilityName.ChangeFieldCardFromDeck,
                EffectAbilityName.DestroyOneCardByRemovingOneHandCard,
                EffectAbilityName.DestroyFieldFor7HalfCost,
                EffectAbilityName.Get7HalfHPFor1Sacrifice,
                EffectAbilityName.GetCardFromYellowDeck,
                EffectAbilityName.ManiabilitePourrieSkipAttackForOpponent,
                EffectAbilityName.SwitchAtkDef,
                EffectAbilityName.LookAndOrderDeckCards,
                EffectAbilityName.LooseHPBasedOnNumberInvocation,
                EffectAbilityName.DestroyEquipmentCard,
                EffectAbilityName.LookOpponentHandCardsAndChangeIt,
                EffectAbilityName.DoubleAttackPerTurn,
                EffectAbilityName.InvokeCardFromYellowTrash,
                EffectAbilityName.DivideDEFOpponentBy2,
                EffectAbilityName.Add3ShieldsForUser,
                EffectAbilityName.DestroyOpponentInvocationCard,
                EffectAbilityName.Loose1HPPerOpponentHandCards,
                EffectAbilityName.GetBackAllHPBySacrifice5AtkDef,
                EffectAbilityName.Control1OpponentInvocationCard
            };

            foreach (var abilityName in expectedAbilities)
            {
                Assert.IsTrue(_provider.HasAbility(abilityName), $"Missing ability: {abilityName}");
                Assert.IsNotNull(_provider.GetAbility(abilityName), $"Null ability: {abilityName}");
            }
        }
    }
}

using System.Linq;
using NUnit.Framework;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;

namespace JDG.Domain.Tests.Entities
{
    [TestFixture]
    public class CardTests
    {
        #region Invocation Card Tests

        [Test]
        public void CreateInvocation_CreatesCardWithCorrectProperties()
        {
            // Arrange
            var id = CardId.New();
            var families = new[] { CardFamily.Human, CardFamily.Wizard };
            var abilities = new[] { AbilityName.DrawCard, AbilityName.GainShield };
            var conditions = new[] { ConditionName.WizardOnField };

            // Act
            var card = Card.CreateInvocation(
                id: id,
                title: "Test Wizard",
                description: "A test wizard card",
                detailedDescription: "Detailed description",
                attack: 5,
                defense: 3,
                families: families,
                affectedByEffect: true,
                conditions: conditions,
                abilities: abilities,
                isCollector: true
            );

            // Assert
            Assert.AreEqual(id, card.Id);
            Assert.AreEqual("Test Wizard", card.Title);
            Assert.AreEqual("A test wizard card", card.Description);
            Assert.AreEqual("Detailed description", card.DetailedDescription);
            Assert.AreEqual(CardType.Invocation, card.Type);
            Assert.IsTrue(card.IsCollector);
            Assert.IsTrue(card.Stats.HasValue);
            Assert.AreEqual(5, card.Stats.Value.Attack);
            Assert.AreEqual(3, card.Stats.Value.Defense);
            Assert.AreEqual(2, card.Families.Count);
            Assert.IsTrue(card.AffectedByEffect);
            Assert.AreEqual(1, card.Conditions.Count);
            Assert.AreEqual(2, card.Abilities.Count);
        }

        [Test]
        public void CreateInvocation_WithoutOptionalParameters_CreatesValidCard()
        {
            // Act
            var card = Card.CreateInvocation(
                id: CardId.New(),
                title: "Simple Card",
                description: "Desc",
                detailedDescription: "Detailed",
                attack: 3,
                defense: 3,
                families: new[] { CardFamily.Human },
                affectedByEffect: false
            );

            // Assert
            Assert.AreEqual(0, card.Conditions.Count);
            Assert.AreEqual(0, card.Abilities.Count);
            Assert.IsFalse(card.IsCollector);
        }

        #endregion

        #region Equipment Card Tests

        [Test]
        public void CreateEquipment_CreatesCardWithCorrectProperties()
        {
            // Arrange
            var id = CardId.New();
            var abilities = new[] { EquipmentAbilityName.Earn2ATK, EquipmentAbilityName.DirectAttack };

            // Act
            var card = Card.CreateEquipment(
                id: id,
                title: "Power Sword",
                description: "A powerful sword",
                detailedDescription: "Grants attack bonus",
                equipmentAbilities: abilities,
                isCollector: false
            );

            // Assert
            Assert.AreEqual(id, card.Id);
            Assert.AreEqual("Power Sword", card.Title);
            Assert.AreEqual(CardType.Equipment, card.Type);
            Assert.AreEqual(2, card.EquipmentAbilities.Count);
            Assert.IsFalse(card.Stats.HasValue);
        }

        #endregion

        #region Field Card Tests

        [Test]
        public void CreateField_CreatesCardWithCorrectProperties()
        {
            // Arrange
            var id = CardId.New();
            var abilities = new[] { FieldAbilityName.DrawOneMoreCard };

            // Act
            var card = Card.CreateField(
                id: id,
                title: "Magic Academy",
                description: "A magical field",
                detailedDescription: "Boosts wizards",
                fieldFamily: CardFamily.Wizard,
                fieldAbilities: abilities
            );

            // Assert
            Assert.AreEqual(id, card.Id);
            Assert.AreEqual("Magic Academy", card.Title);
            Assert.AreEqual(CardType.Field, card.Type);
            Assert.AreEqual(CardFamily.Wizard, card.FieldFamily);
            Assert.AreEqual(1, card.FieldAbilities.Count);
        }

        #endregion

        #region Effect Card Tests

        [Test]
        public void CreateEffect_CreatesCardWithCorrectProperties()
        {
            // Arrange
            var id = CardId.New();
            var abilities = new[] { EffectAbilityName.DestroyFieldFor7HalfCost };

            // Act
            var card = Card.CreateEffect(
                id: id,
                title: "Destroy Field",
                description: "Destroys the field",
                detailedDescription: "Costs health",
                effectAbilities: abilities
            );

            // Assert
            Assert.AreEqual(id, card.Id);
            Assert.AreEqual("Destroy Field", card.Title);
            Assert.AreEqual(CardType.Effect, card.Type);
            Assert.AreEqual(1, card.EffectAbilities.Count);
        }

        #endregion

        #region Contre Card Tests

        [Test]
        public void CreateContre_CreatesCardWithCorrectProperties()
        {
            // Arrange
            var id = CardId.New();

            // Act
            var card = Card.CreateContre(
                id: id,
                title: "Counter Strike",
                description: "Counters an attack",
                detailedDescription: "Negate effect"
            );

            // Assert
            Assert.AreEqual(id, card.Id);
            Assert.AreEqual("Counter Strike", card.Title);
            Assert.AreEqual(CardType.Contre, card.Type);
            Assert.AreEqual(0, card.Abilities.Count);
            Assert.AreEqual(0, card.EquipmentAbilities.Count);
            Assert.AreEqual(0, card.FieldAbilities.Count);
            Assert.AreEqual(0, card.EffectAbilities.Count);
        }

        #endregion

        #region Owner Tests

        [Test]
        public void SetOwner_SetsOwnerCorrectly()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            // Act
            card.SetOwner(CardOwner.Player1);

            // Assert
            Assert.AreEqual(CardOwner.Player1, card.Owner);
        }

        [Test]
        public void SetOwner_CanChangeOwner()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );
            card.SetOwner(CardOwner.Player1);

            // Act
            card.SetOwner(CardOwner.Player2);

            // Assert
            Assert.AreEqual(CardOwner.Player2, card.Owner);
        }

        #endregion

        #region Stats Modification Tests

        [Test]
        public void ModifyStats_ModifiesInvocationCardStats()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                5, 3,
                new[] { CardFamily.Human },
                false
            );

            // Act
            var result = card.ModifyStats(2, -1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(7, card.Stats.Value.Attack);
            Assert.AreEqual(2, card.Stats.Value.Defense);
        }

        [Test]
        public void ModifyStats_ReturnsFalseForNonInvocationCard()
        {
            // Arrange
            var card = Card.CreateEquipment(
                CardId.New(),
                "Test Equipment",
                "Test",
                "Test",
                new[] { EquipmentAbilityName.Earn2ATK }
            );

            // Act
            var result = card.ModifyStats(2, 2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SetStats_SetsInvocationCardStats()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                5, 3,
                new[] { CardFamily.Human },
                false
            );

            // Act
            var result = card.SetStats(10, 8);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(10, card.Stats.Value.Attack);
            Assert.AreEqual(8, card.Stats.Value.Defense);
        }

        [Test]
        public void SetStats_ReturnsFalseForNonInvocationCard()
        {
            // Arrange
            var card = Card.CreateEffect(
                CardId.New(),
                "Test Effect",
                "Test",
                "Test",
                new[] { EffectAbilityName.Add3ShieldsForUser }
            );

            // Act
            var result = card.SetStats(5, 5);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ResetStats_ResetsInvocationCardStats()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                5, 3,
                new[] { CardFamily.Human },
                false
            );
            var baseStats = new CardStats(5, 3);
            card.ModifyStats(3, 2);

            // Act
            card.ResetStats(baseStats);

            // Assert
            Assert.AreEqual(5, card.Stats.Value.Attack);
            Assert.AreEqual(3, card.Stats.Value.Defense);
        }

        #endregion

        #region Query Tests

        [Test]
        public void IsInvocation_ReturnsTrueForInvocationCard()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            // Assert
            Assert.IsTrue(card.IsInvocation);
            Assert.IsFalse(card.IsEquipment);
            Assert.IsFalse(card.IsField);
            Assert.IsFalse(card.IsEffect);
            Assert.IsFalse(card.IsContre);
        }

        [Test]
        public void IsEquipment_ReturnsTrueForEquipmentCard()
        {
            // Arrange
            var card = Card.CreateEquipment(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                new[] { EquipmentAbilityName.Earn2ATK }
            );

            // Assert
            Assert.IsFalse(card.IsInvocation);
            Assert.IsTrue(card.IsEquipment);
            Assert.IsFalse(card.IsField);
            Assert.IsFalse(card.IsEffect);
            Assert.IsFalse(card.IsContre);
        }

        [Test]
        public void HasFamily_ReturnsTrueWhenCardHasFamily()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human, CardFamily.Wizard },
                false
            );

            // Assert
            Assert.IsTrue(card.HasFamily(CardFamily.Human));
            Assert.IsTrue(card.HasFamily(CardFamily.Wizard));
            Assert.IsFalse(card.HasFamily(CardFamily.Monster));
        }

        [Test]
        public void HasFamily_ReturnsTrueForAnyFamily()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Any },
                false
            );

            // Assert
            Assert.IsTrue(card.HasFamily(CardFamily.Human));
            Assert.IsTrue(card.HasFamily(CardFamily.Monster));
        }

        [Test]
        public void HasAbility_ReturnsTrueWhenCardHasAbility()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false,
                abilities: new[] { AbilityName.DrawCard, AbilityName.GainShield }
            );

            // Assert
            Assert.IsTrue(card.HasAbility(AbilityName.DrawCard));
            Assert.IsTrue(card.HasAbility(AbilityName.GainShield));
            Assert.IsFalse(card.HasAbility(AbilityName.DestroyCard));
        }

        [Test]
        public void HasCondition_ReturnsTrueWhenCardHasCondition()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false,
                conditions: new[] { ConditionName.WizardOnField }
            );

            // Assert
            Assert.IsTrue(card.HasCondition(ConditionName.WizardOnField));
            Assert.IsFalse(card.HasCondition(ConditionName.HumanOnField));
        }

        [Test]
        public void IsDestroyed_ReturnsTrueWhenDefenseZeroOrLess()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            // Act
            card.ModifyStats(0, -3);

            // Assert
            Assert.IsTrue(card.IsDestroyed);
        }

        [Test]
        public void IsDestroyed_ReturnsFalseWhenDefenseAboveZero()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            // Assert
            Assert.IsFalse(card.IsDestroyed);
        }

        [Test]
        public void IsDestroyed_ReturnsFalseForNonInvocationCard()
        {
            // Arrange
            var card = Card.CreateEquipment(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                new[] { EquipmentAbilityName.Earn2ATK }
            );

            // Assert
            Assert.IsFalse(card.IsDestroyed);
        }

        #endregion

        #region Snapshot Tests

        [Test]
        public void CreateSnapshot_ReturnsCorrectSnapshot()
        {
            // Arrange
            var id = CardId.New();
            var card = Card.CreateInvocation(
                id: id,
                title: "Test Card",
                description: "Test",
                detailedDescription: "Test",
                attack: 5,
                defense: 3,
                families: new[] { CardFamily.Human, CardFamily.Wizard },
                affectedByEffect: true,
                abilities: new[] { AbilityName.DrawCard }
            );
            card.SetOwner(CardOwner.Player1);

            // Act
            var snapshot = card.CreateSnapshot();

            // Assert
            Assert.AreEqual(id, snapshot.Id);
            Assert.AreEqual("Test Card", snapshot.Title);
            Assert.AreEqual(CardType.Invocation, snapshot.Type);
            Assert.AreEqual(CardOwner.Player1, snapshot.Owner);
            Assert.AreEqual(5, snapshot.Attack);
            Assert.AreEqual(3, snapshot.Defense);
            Assert.AreEqual(2, snapshot.FamilyCount);
            Assert.AreEqual(1, snapshot.AbilityCount);
            Assert.IsFalse(snapshot.IsDestroyed);
        }

        [Test]
        public void CreateSnapshot_ShowsDestroyedState()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );
            card.ModifyStats(0, -3);

            // Act
            var snapshot = card.CreateSnapshot();

            // Assert
            Assert.IsTrue(snapshot.IsDestroyed);
        }

        #endregion

        #region ToString Tests

        [Test]
        public void ToString_ReturnsCorrectFormatForInvocationCard()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Dragon",
                "Test",
                "Test",
                8, 6,
                new[] { CardFamily.Monster },
                false
            );

            // Act
            var result = card.ToString();

            // Assert
            Assert.AreEqual("Dragon (Invocation) - ATK: 8, DEF: 6", result);
        }

        [Test]
        public void ToString_ReturnsCorrectFormatForNonInvocationCard()
        {
            // Arrange
            var card = Card.CreateEquipment(
                CardId.New(),
                "Sword",
                "Test",
                "Test",
                new[] { EquipmentAbilityName.Earn2ATK }
            );

            // Act
            var result = card.ToString();

            // Assert
            Assert.AreEqual("Sword (Equipment)", result);
        }

        #endregion
    }
}

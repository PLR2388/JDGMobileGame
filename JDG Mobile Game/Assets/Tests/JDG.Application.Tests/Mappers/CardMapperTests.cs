using NUnit.Framework;
using JDG.Application.Mappers;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;

namespace JDG.Application.Tests.Mappers
{
    [TestFixture]
    public class CardMapperTests
    {
        [Test]
        public void ToDTO_WithInvocationCard_MapsCorrectly()
        {
            // Arrange
            var cardId = CardId.New();
            var card = Card.CreateInvocation(
                id: cardId,
                title: "Test Dragon",
                description: "A test dragon",
                detailedDescription: "Detailed desc",
                attack: 8,
                defense: 6,
                families: new[] { CardFamily.Monster, CardFamily.Wizard },
                affectedByEffect: true,
                conditions: new[] { ConditionName.WizardOnField },
                abilities: new[] { AbilityName.Draw2Cards },
                isCollector: true
            );

            card.SetOwner(CardOwner.Player1);

            // Act
            var dto = card.ToDTO(isInHand: true);

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(cardId.ToGuid(), dto.Id);
            Assert.AreEqual("Test Dragon", dto.Title);
            Assert.AreEqual("A test dragon", dto.Description);
            Assert.AreEqual(CardType.Invocation, dto.Type);
            Assert.AreEqual(CardOwner.Player1, dto.Owner);
            Assert.IsTrue(dto.IsCollector);
            Assert.AreEqual(8, dto.Attack);
            Assert.AreEqual(6, dto.Defense);
            Assert.AreEqual(2, dto.Families.Length);
            Assert.IsTrue(dto.AffectedByEffect);
            Assert.AreEqual(1, dto.Conditions.Length);
            Assert.AreEqual(1, dto.Abilities.Length);
            Assert.IsTrue(dto.IsInHand);
            Assert.IsFalse(dto.IsOnField);
            Assert.IsFalse(dto.IsDestroyed);
        }

        [Test]
        public void ToDTO_WithEquipmentCard_MapsCorrectly()
        {
            // Arrange
            var card = Card.CreateEquipment(
                CardId.New(),
                "Power Sword",
                "Test",
                "Test",
                new[] { EquipmentAbilityName.Earn2ATK, EquipmentAbilityName.DirectAttack }
            );

            // Act
            var dto = card.ToDTO(isOnField: true);

            // Assert
            Assert.AreEqual(CardType.Equipment, dto.Type);
            Assert.AreEqual(2, dto.EquipmentAbilities.Length);
            Assert.AreEqual(0, dto.Attack);
            Assert.AreEqual(0, dto.Defense);
            Assert.IsTrue(dto.IsOnField);
        }

        [Test]
        public void ToDTO_WithNullCard_ReturnsNull()
        {
            // Act
            var dto = CardMapper.ToDTO(null);

            // Assert
            Assert.IsNull(dto);
        }

        [Test]
        public void ToDTO_WithDestroyedCard_SetsIsDestroyedTrue()
        {
            // Arrange
            var card = Card.CreateInvocation(
                CardId.New(),
                "Weak Card",
                "", "",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            card.ModifyStats(0, -3); // Reduce defense to 0

            // Act
            var dto = card.ToDTO(isInGraveyard: true);

            // Assert
            Assert.IsTrue(dto.IsDestroyed);
            Assert.IsTrue(dto.IsInGraveyard);
            Assert.AreEqual(0, dto.Defense);
        }
    }
}

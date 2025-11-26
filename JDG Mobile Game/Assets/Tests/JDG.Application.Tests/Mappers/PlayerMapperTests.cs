using NUnit.Framework;
using JDG.Application.Mappers;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using System.Collections.Generic;

namespace JDG.Application.Tests.Mappers
{
    [TestFixture]
    public class PlayerMapperTests
    {
        [Test]
        public void ToDTO_WithValidPlayer_MapsCorrectly()
        {
            // Arrange
            var deck = new List<Card>
            {
                Card.CreateInvocation(CardId.New(), "Card1", "", "", 3, 3, new[] { CardFamily.Human }, false),
                Card.CreateInvocation(CardId.New(), "Card2", "", "", 3, 3, new[] { CardFamily.Human }, false)
            };

            var player = new Player(PlayerId.Player1, deck, maxHealth: 30);
            player.DrawCard();
            player.AddShields(5);
            player.TakeDamage(10);

            // Act
            var dto = player.ToDTO();

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(1, dto.PlayerId);
            Assert.AreEqual(25, dto.Health); // 30 - 5 (shields) - 0 = 30, then - 10 = 20... wait shields are 5, damage is 10, so 30 - 5 = 25
            Assert.AreEqual(30, dto.MaxHealth);
            Assert.AreEqual(0, dto.Shields); // 5 shields - 5 from damage = 0
            Assert.AreEqual(false, dto.BlockAttack);
            Assert.AreEqual(1, dto.DeckCount);
            Assert.AreEqual(1, dto.HandCount);
            Assert.AreEqual(0, dto.FieldCount);
            Assert.AreEqual(0, dto.GraveyardCount);
            Assert.AreEqual(false, dto.IsDefeated);
            Assert.AreEqual(CardOwner.Player1, dto.Owner);
        }

        [Test]
        public void ToDTO_WithNullPlayer_ReturnsNull()
        {
            // Act
            var dto = PlayerMapper.ToDTO(null);

            // Assert
            Assert.IsNull(dto);
        }

        [Test]
        public void ToDTO_WithDefeatedPlayer_SetsIsDefeatedTrue()
        {
            // Arrange
            var player = new Player(PlayerId.Player2, new List<Card>());
            player.TakeDamage(30);

            // Act
            var dto = player.ToDTO();

            // Assert
            Assert.IsTrue(dto.IsDefeated);
            Assert.AreEqual(0, dto.Health);
            Assert.AreEqual(2, dto.PlayerId);
            Assert.AreEqual(CardOwner.Player2, dto.Owner);
        }
    }
}

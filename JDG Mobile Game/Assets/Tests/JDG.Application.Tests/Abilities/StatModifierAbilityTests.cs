using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace JDG.Application.Tests.Abilities
{
    /// <summary>
    /// Tests for stat modifier abilities.
    /// Sprint 4: Added tests for GiveFamilyStatsAbility, ConditionalStatsAbility, and CopyStatsAbility.
    /// </summary>
    [TestFixture]
    public class StatModifierAbilityTests
    {
        private TestPlayerRepositoryForStats _playerRepository;
        private Player _player1;
        private Card _sourceCard;
        private AbilityContext _context;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepositoryForStats();

            _sourceCard = Card.CreateInvocation(
                CardId.New(),
                "Source Card",
                "Test",
                "Test description",
                5, 5,
                new[] { CardFamily.Human },
                false
            );

            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard });
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);
            _playerRepository.AddPlayer(_player1);

            _context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                _sourceCard,
                AbilityName.GiveAtkDefToComics
            );
        }

        #region GiveFamilyStatsAbility Tests

        [Test]
        public void GiveFamilyStatsAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new GiveFamilyStatsAbility(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                1, 1,
                _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.GiveAtkDefToComics, ability.Name);
        }

        [Test]
        public void GiveFamilyStatsAbility_Description_ContainsFamily()
        {
            // Arrange
            var ability = new GiveFamilyStatsAbility(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                1, 1,
                _playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Comics"));
        }

        [Test]
        public void GiveFamilyStatsAbility_Description_ContainsStats()
        {
            // Arrange
            var ability = new GiveFamilyStatsAbility(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                2, 3,
                _playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("+2"));
            Assert.IsTrue(ability.Description.Contains("+3"));
        }

        [Test]
        public void GiveFamilyStatsAbility_CanActivate_WithFamilyOnField_ReturnsTrue()
        {
            // Arrange
            var comicsCard = Card.CreateInvocation(
                CardId.New(),
                "Comics Card",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Comics },
                false
            );
            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard, comicsCard });
            _player1.DrawCard();
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);
            _player1.PlayCard(comicsCard);
            _playerRepository.AddPlayer(_player1);

            var ability = new GiveFamilyStatsAbility(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                1, 1,
                _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void GiveFamilyStatsAbility_CanActivate_WithoutFamily_ReturnsFalse()
        {
            // Arrange - player only has Human card
            var ability = new GiveFamilyStatsAbility(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                1, 1,
                _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GiveFamilyStatsAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var comicsCard = Card.CreateInvocation(
                CardId.New(),
                "Comics Card",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Comics },
                false
            );
            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard, comicsCard });
            _player1.DrawCard();
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);
            _player1.PlayCard(comicsCard);
            _playerRepository.AddPlayer(_player1);

            var ability = new GiveFamilyStatsAbility(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                1, 1,
                _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void GiveFamilyStatsAbility_Execute_NoFamilyCards_ReturnsFailure()
        {
            // Arrange - player only has Human card
            var ability = new GiveFamilyStatsAbility(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                1, 1,
                _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        #endregion

        #region ConditionalStatsAbility Tests

        [Test]
        public void ConditionalStatsAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new ConditionalStatsAbility(
                AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition,
                CardFamily.Japan,
                2, 2, 1, 1,
                _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition, ability.Name);
        }

        [Test]
        public void ConditionalStatsAbility_Description_ContainsCondition()
        {
            // Arrange
            var ability = new ConditionalStatsAbility(
                AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition,
                CardFamily.Japan,
                2, 2, 1, 1,
                _playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Japan"));
            Assert.IsTrue(ability.Description.Contains("2"));
        }

        [Test]
        public void ConditionalStatsAbility_CanActivate_WithSourceCard_ReturnsTrue()
        {
            // Arrange
            var ability = new ConditionalStatsAbility(
                AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition,
                CardFamily.Japan,
                2, 2, 1, 1,
                _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ConditionalStatsAbility_Execute_ConditionNotMet_ReturnsFailure()
        {
            // Arrange - source card is Human, not Japon
            var ability = new ConditionalStatsAbility(
                AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition,
                CardFamily.Japan,
                2, 2, 1, 1,
                _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void ConditionalStatsAbility_Execute_ConditionMet_ReturnsSuccess()
        {
            // Arrange
            var japonCard = Card.CreateInvocation(
                CardId.New(),
                "Japon Card",
                "Test",
                "Test",
                5, 5, // Meets 2 ATK / 2 DEF condition
                new[] { CardFamily.Japan },
                false
            );
            _player1 = new Player(PlayerId.Player1, new List<Card> { japonCard });
            _player1.DrawCard();
            _player1.PlayCard(japonCard);
            _playerRepository.AddPlayer(_player1);

            var context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                japonCard,
                AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition
            );

            var ability = new ConditionalStatsAbility(
                AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition,
                CardFamily.Japan,
                2, 2, 1, 1,
                _playerRepository);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region CopyStatsAbility Tests

        [Test]
        public void CopyStatsAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new CopyStatsAbility(
                AbilityName.CopyBenzaieJeune,
                "Benzaie jeune",
                _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.CopyBenzaieJeune, ability.Name);
        }

        [Test]
        public void CopyStatsAbility_Description_ContainsTargetCard()
        {
            // Arrange
            var ability = new CopyStatsAbility(
                AbilityName.CopyBenzaieJeune,
                "Benzaie jeune",
                _playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Benzaie jeune"));
        }

        [Test]
        public void CopyStatsAbility_CanActivate_WithTargetOnField_ReturnsTrue()
        {
            // Arrange
            var benzaie = Card.CreateInvocation(
                CardId.New(),
                "Benzaie jeune",
                "Test",
                "Test",
                10, 8,
                new[] { CardFamily.Human },
                false
            );
            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard, benzaie });
            _player1.DrawCard();
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);
            _player1.PlayCard(benzaie);
            _playerRepository.AddPlayer(_player1);

            var ability = new CopyStatsAbility(
                AbilityName.CopyBenzaieJeune,
                "Benzaie jeune",
                _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CopyStatsAbility_CanActivate_WithoutTarget_ReturnsFalse()
        {
            // Arrange - no Benzaie jeune on field
            var ability = new CopyStatsAbility(
                AbilityName.CopyBenzaieJeune,
                "Benzaie jeune",
                _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CopyStatsAbility_Execute_WithTarget_ReturnsSuccess()
        {
            // Arrange
            var benzaie = Card.CreateInvocation(
                CardId.New(),
                "Benzaie jeune",
                "Test",
                "Test",
                10, 8,
                new[] { CardFamily.Human },
                false
            );
            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard, benzaie });
            _player1.DrawCard();
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);
            _player1.PlayCard(benzaie);
            _playerRepository.AddPlayer(_player1);

            var ability = new CopyStatsAbility(
                AbilityName.CopyBenzaieJeune,
                "Benzaie jeune",
                _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void CopyStatsAbility_Execute_WithoutTarget_ReturnsFailure()
        {
            // Arrange - no Benzaie jeune on field
            var ability = new CopyStatsAbility(
                AbilityName.CopyBenzaieJeune,
                "Benzaie jeune",
                _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        #endregion

        #region StatModifierAbilityFactory Tests

        [Test]
        public void StatModifierAbilityFactory_CreateGiveFamilyStats_ReturnsAbility()
        {
            // Arrange
            var factory = new StatModifierAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateGiveFamilyStats(
                AbilityName.GiveAtkDefToComics,
                CardFamily.Comics,
                1, 1);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.GiveAtkDefToComics, ability.Name);
        }

        [Test]
        public void StatModifierAbilityFactory_CreateConditionalStats_ReturnsAbility()
        {
            // Arrange
            var factory = new StatModifierAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateConditionalStats(
                AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition,
                CardFamily.Japan,
                2, 2, 1, 1);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition, ability.Name);
        }

        [Test]
        public void StatModifierAbilityFactory_CreateCopyStats_ReturnsAbility()
        {
            // Arrange
            var factory = new StatModifierAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateCopyStats(
                AbilityName.CopyBenzaieJeune,
                "Benzaie jeune");

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.CopyBenzaieJeune, ability.Name);
        }

        #endregion
    }

    #region Test Doubles

    public class TestPlayerRepositoryForStats : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();

        public void AddPlayer(Player player)
        {
            _players[player.Id] = player;
        }

        public Player GetPlayer(PlayerId playerId)
        {
            return _players.TryGetValue(playerId, out var player) ? player : null;
        }

        public void SavePlayer(Player player)
        {
            _players[player.Id] = player;
        }

        public Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30)
        {
            var player = new Player(playerId, new List<Card>());
            _players[playerId] = player;
            return player;
        }

        public void ResetPlayer(PlayerId playerId)
        {
            // No-op for testing
        }
    }

    #endregion
}

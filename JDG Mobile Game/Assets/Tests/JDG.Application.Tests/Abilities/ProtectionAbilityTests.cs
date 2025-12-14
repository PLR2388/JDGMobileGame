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
    /// Tests for protection-related abilities.
    /// Sprint 4: Added tests for CantBeAttackedAbility, ProtectBehindAbility,
    /// CanOnlyAttackItselfAbility, LimitedLifetimeAbility, and DependencyAbility.
    /// </summary>
    [TestFixture]
    public class ProtectionAbilityTests
    {
        private TestPlayerRepositoryForProtection _playerRepository;
        private Player _player1;
        private Card _sourceCard;
        private AbilityContext _context;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepositoryForProtection();

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
                AbilityName.CanOnlyAttackItself
            );
        }

        #region CantBeAttackedAbility Tests

        [Test]
        public void CantBeAttackedAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new CantBeAttackedAbility(AbilityName.CantBeAttackIfComics, _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.CantBeAttackIfComics, ability.Name);
        }

        [Test]
        public void CantBeAttackedAbility_Description_UnconditionalProtection()
        {
            // Arrange
            var ability = new CantBeAttackedAbility(AbilityName.CantBeAttackIfComics, _playerRepository, null);

            // Assert
            Assert.AreEqual("Cannot be attacked", ability.Description);
        }

        [Test]
        public void CantBeAttackedAbility_Description_ConditionalProtection()
        {
            // Arrange
            var ability = new CantBeAttackedAbility(AbilityName.CantBeAttackIfComics, _playerRepository, CardFamily.Comics);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Comics"));
        }

        [Test]
        public void CantBeAttackedAbility_CanActivate_UnconditionalAlwaysTrue()
        {
            // Arrange
            var ability = new CantBeAttackedAbility(AbilityName.CantBeAttackIfComics, _playerRepository, null);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CantBeAttackedAbility_CanActivate_ConditionalWithFamilyOnField()
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

            var ability = new CantBeAttackedAbility(AbilityName.CantBeAttackIfComics, _playerRepository, CardFamily.Comics);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CantBeAttackedAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new CantBeAttackedAbility(AbilityName.CantBeAttackIfComics, _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("protected"));
        }

        #endregion

        #region ProtectBehindAbility Tests

        [Test]
        public void ProtectBehindAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new ProtectBehindAbility(AbilityName.ProtectedBehindStarlightUnicorn, _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.ProtectedBehindStarlightUnicorn, ability.Name);
        }

        [Test]
        public void ProtectBehindAbility_Description_AllProtection()
        {
            // Arrange
            var ability = new ProtectBehindAbility(AbilityName.ProtectedBehindStarlightUnicorn, _playerRepository, null);

            // Assert
            Assert.IsTrue(ability.Description.Contains("all cards"));
        }

        [Test]
        public void ProtectBehindAbility_Description_WithMinDefense()
        {
            // Arrange
            var ability = new ProtectBehindAbility(AbilityName.ProtectedBehindStarlightUnicorn, _playerRepository, 5);

            // Assert
            Assert.IsTrue(ability.Description.Contains("5"));
            Assert.IsTrue(ability.Description.Contains("DEF"));
        }

        [Test]
        public void ProtectBehindAbility_CanActivate_WithSourceCard_ReturnsTrue()
        {
            // Arrange
            var ability = new ProtectBehindAbility(AbilityName.ProtectedBehindStarlightUnicorn, _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ProtectBehindAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new ProtectBehindAbility(AbilityName.ProtectedBehindStarlightUnicorn, _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region CanOnlyAttackItselfAbility Tests

        [Test]
        public void CanOnlyAttackItselfAbility_Name_ReturnsCanOnlyAttackItself()
        {
            // Arrange
            var ability = new CanOnlyAttackItselfAbility();

            // Assert
            Assert.AreEqual(AbilityName.CanOnlyAttackItself, ability.Name);
        }

        [Test]
        public void CanOnlyAttackItselfAbility_Description_IsCorrect()
        {
            // Arrange
            var ability = new CanOnlyAttackItselfAbility();

            // Assert
            Assert.IsTrue(ability.Description.Contains("attack itself"));
        }

        [Test]
        public void CanOnlyAttackItselfAbility_CanActivate_AlwaysReturnsTrue()
        {
            // Arrange
            var ability = new CanOnlyAttackItselfAbility();

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanOnlyAttackItselfAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new CanOnlyAttackItselfAbility();

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region LimitedLifetimeAbility Tests

        [Test]
        public void LimitedLifetimeAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new LimitedLifetimeAbility(AbilityName.SurviveOneTurn, 1);

            // Assert
            Assert.AreEqual(AbilityName.SurviveOneTurn, ability.Name);
        }

        [Test]
        public void LimitedLifetimeAbility_Description_ContainsTurnCount()
        {
            // Arrange
            var ability = new LimitedLifetimeAbility(AbilityName.SurviveOneTurn, 2);

            // Assert
            Assert.IsTrue(ability.Description.Contains("2"));
            Assert.IsTrue(ability.Description.Contains("turn"));
        }

        [Test]
        public void LimitedLifetimeAbility_CanActivate_WithSourceCard_ReturnsTrue()
        {
            // Arrange
            var ability = new LimitedLifetimeAbility(AbilityName.SurviveOneTurn, 1);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void LimitedLifetimeAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new LimitedLifetimeAbility(AbilityName.SurviveOneTurn, 1);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region DependencyAbility Tests

        [Test]
        public void DependencyAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new DependencyAbility(
                AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
                _playerRepository,
                "Benzaie", "Benzaie jeune");

            // Assert
            Assert.AreEqual(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune, ability.Name);
        }

        [Test]
        public void DependencyAbility_Description_ListsRequiredCards()
        {
            // Arrange
            var ability = new DependencyAbility(
                AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
                _playerRepository,
                "Benzaie", "Benzaie jeune");

            // Assert
            Assert.IsTrue(ability.Description.Contains("Benzaie"));
            Assert.IsTrue(ability.Description.Contains("Benzaie jeune"));
        }

        [Test]
        public void DependencyAbility_CanActivate_WithDependencyMet_ReturnsTrue()
        {
            // Arrange
            var benzaie = Card.CreateInvocation(
                CardId.New(),
                "Benzaie",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );
            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard, benzaie });
            _player1.DrawCard();
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);
            _player1.PlayCard(benzaie);
            _playerRepository.AddPlayer(_player1);

            var ability = new DependencyAbility(
                AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
                _playerRepository,
                "Benzaie", "Benzaie jeune");

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DependencyAbility_CanActivate_WithoutDependency_ReturnsFalse()
        {
            // Arrange - player only has source card, no Benzaie
            var ability = new DependencyAbility(
                AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
                _playerRepository,
                "Benzaie", "Benzaie jeune");

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DependencyAbility_Execute_WithDependency_ReturnsSuccess()
        {
            // Arrange
            var benzaie = Card.CreateInvocation(
                CardId.New(),
                "Benzaie",
                "Test",
                "Test",
                3, 3,
                new[] { CardFamily.Human },
                false
            );
            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard, benzaie });
            _player1.DrawCard();
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);
            _player1.PlayCard(benzaie);
            _playerRepository.AddPlayer(_player1);

            var ability = new DependencyAbility(
                AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
                _playerRepository,
                "Benzaie", "Benzaie jeune");

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("satisfied"));
        }

        #endregion

        #region ProtectionAbilityFactory Tests

        [Test]
        public void ProtectionAbilityFactory_CreateCantBeAttacked_ReturnsAbility()
        {
            // Arrange
            var factory = new ProtectionAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateCantBeAttacked(AbilityName.CantBeAttackIfComics, CardFamily.Comics);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.CantBeAttackIfComics, ability.Name);
        }

        [Test]
        public void ProtectionAbilityFactory_CreateProtectBehind_ReturnsAbility()
        {
            // Arrange
            var factory = new ProtectionAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateProtectBehind(AbilityName.ProtectedBehindStarlightUnicorn, 5);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.ProtectedBehindStarlightUnicorn, ability.Name);
        }

        [Test]
        public void ProtectionAbilityFactory_CreateCanOnlyAttackItself_ReturnsAbility()
        {
            // Arrange
            var factory = new ProtectionAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateCanOnlyAttackItself();

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.CanOnlyAttackItself, ability.Name);
        }

        [Test]
        public void ProtectionAbilityFactory_CreateLimitedLifetime_ReturnsAbility()
        {
            // Arrange
            var factory = new ProtectionAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateLimitedLifetime(AbilityName.SurviveOneTurn, 1);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.SurviveOneTurn, ability.Name);
        }

        [Test]
        public void ProtectionAbilityFactory_CreateDependency_ReturnsAbility()
        {
            // Arrange
            var factory = new ProtectionAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateDependency(
                AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
                "Benzaie", "Benzaie jeune");

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune, ability.Name);
        }

        #endregion
    }

    #region Test Doubles

    public class TestPlayerRepositoryForProtection : IPlayerRepository
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

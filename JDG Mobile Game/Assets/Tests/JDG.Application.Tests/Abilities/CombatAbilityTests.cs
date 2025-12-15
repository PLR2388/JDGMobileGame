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
    /// Tests for combat-related abilities.
    /// Phase 41: Added tests for MutualDestructionAbility, SkipAttackAbility,
    /// DirectAttackAbility, ResurrectionAbility, DeathTriggerAbility,
    /// DeathRewardAbility, and DestroyOpponentCardAbility.
    /// </summary>
    [TestFixture]
    public class CombatAbilityTests
    {
        private TestPlayerRepositoryForCombat _playerRepository;
        private Player _player1;
        private Player _player2;
        private Card _sourceCard;
        private Card _targetCard;
        private AbilityContext _context;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepositoryForCombat();

            _sourceCard = Card.CreateInvocation(
                CardId.New(),
                "Attacker Card",
                "Test",
                "Test description",
                5, 5,
                new[] { CardFamily.Human },
                false
            );

            _targetCard = Card.CreateInvocation(
                CardId.New(),
                "Defender Card",
                "Test",
                "Test description",
                3, 3,
                new[] { CardFamily.Human },
                false
            );

            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard });
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);

            _player2 = new Player(PlayerId.Player2, new List<Card> { _targetCard });
            _player2.DrawCard();
            _player2.PlayCard(_targetCard);

            _playerRepository.AddPlayer(_player1);
            _playerRepository.AddPlayer(_player2);

            _context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                _sourceCard,
                AbilityName.KillEnemyIfDestroy
            )
            {
                TargetCard = _targetCard
            };
        }

        #region MutualDestructionAbility Tests

        [Test]
        public void MutualDestructionAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new MutualDestructionAbility(AbilityName.KillEnemyIfDestroy, _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.KillEnemyIfDestroy, ability.Name);
        }

        [Test]
        public void MutualDestructionAbility_Description_ContainsDestroyBoth()
        {
            // Arrange
            var ability = new MutualDestructionAbility(AbilityName.KillEnemyIfDestroy, _playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("destroy both"));
        }

        [Test]
        public void MutualDestructionAbility_Trigger_IsOnAttack()
        {
            // Arrange
            var ability = new MutualDestructionAbility(AbilityName.KillEnemyIfDestroy, _playerRepository);

            // Assert
            Assert.AreEqual(AbilityTrigger.OnAttack, ability.Trigger);
        }

        [Test]
        public void MutualDestructionAbility_CanActivate_WithBothCards_ReturnsTrue()
        {
            // Arrange
            var ability = new MutualDestructionAbility(AbilityName.KillEnemyIfDestroy, _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void MutualDestructionAbility_CanActivate_WithoutTargetCard_ReturnsFalse()
        {
            // Arrange
            var ability = new MutualDestructionAbility(AbilityName.KillEnemyIfDestroy, _playerRepository);
            var contextWithoutTarget = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                _sourceCard,
                AbilityName.KillEnemyIfDestroy
            );

            // Act
            var result = ability.CanActivate(contextWithoutTarget);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MutualDestructionAbility_Execute_DestroysBothCards()
        {
            // Arrange
            var ability = new MutualDestructionAbility(AbilityName.KillEnemyIfDestroy, _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("destroyed"));
        }

        #endregion

        #region SkipAttackAbility Tests

        [Test]
        public void SkipAttackAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new SkipAttackAbility(AbilityName.SkipOpponentAttackEveryTurn, false);

            // Assert
            Assert.AreEqual(AbilityName.SkipOpponentAttackEveryTurn, ability.Name);
        }

        [Test]
        public void SkipAttackAbility_Description_SingleSkip()
        {
            // Arrange
            var ability = new SkipAttackAbility(AbilityName.SkipOpponentAttackEveryTurn, false);

            // Assert
            Assert.AreEqual("Skip opponent's attack", ability.Description);
        }

        [Test]
        public void SkipAttackAbility_Description_EveryTurn()
        {
            // Arrange
            var ability = new SkipAttackAbility(AbilityName.SkipOpponentAttackEveryTurn, true);

            // Assert
            Assert.IsTrue(ability.Description.Contains("every turn"));
        }

        [Test]
        public void SkipAttackAbility_CanActivate_AlwaysReturnsTrue()
        {
            // Arrange
            var ability = new SkipAttackAbility(AbilityName.SkipOpponentAttackEveryTurn, false);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SkipAttackAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new SkipAttackAbility(AbilityName.SkipOpponentAttackEveryTurn, false);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("skipped"));
        }

        #endregion

        #region DirectAttackAbility Tests

        [Test]
        public void DirectAttackAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new DirectAttackAbility();

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void DirectAttackAbility_Description_ContainsDirectAttack()
        {
            // Arrange
            var ability = new DirectAttackAbility();

            // Assert
            Assert.IsTrue(ability.Description.Contains("directly"));
        }

        [Test]
        public void DirectAttackAbility_CanActivate_WithSourceCard_ReturnsTrue()
        {
            // Arrange
            var ability = new DirectAttackAbility();

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DirectAttackAbility_CanActivate_WithoutSourceCard_ReturnsFalse()
        {
            // Arrange
            var ability = new DirectAttackAbility();
            var contextWithoutSource = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                null,
                AbilityName.Default
            );

            // Act
            var result = ability.CanActivate(contextWithoutSource);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DirectAttackAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new DirectAttackAbility();

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("enabled"));
        }

        #endregion

        #region ResurrectionAbility Tests

        [Test]
        public void ResurrectionAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new ResurrectionAbility(AbilityName.ComesBackFromDeath, _playerRepository, 1, false);

            // Assert
            Assert.AreEqual(AbilityName.ComesBackFromDeath, ability.Name);
        }

        [Test]
        public void ResurrectionAbility_Description_ToField()
        {
            // Arrange
            var ability = new ResurrectionAbility(AbilityName.ComesBackFromDeath, _playerRepository, 2, false);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Revives"));
            Assert.IsTrue(ability.Description.Contains("2"));
        }

        [Test]
        public void ResurrectionAbility_Description_ToHand()
        {
            // Arrange
            var ability = new ResurrectionAbility(AbilityName.ComesBackFromDeath, _playerRepository, 1, true);

            // Assert
            Assert.IsTrue(ability.Description.Contains("hand"));
        }

        [Test]
        public void ResurrectionAbility_Trigger_IsOnDeath()
        {
            // Arrange
            var ability = new ResurrectionAbility(AbilityName.ComesBackFromDeath, _playerRepository, 1, false);

            // Assert
            Assert.AreEqual(AbilityTrigger.OnDeath, ability.Trigger);
        }

        [Test]
        public void ResurrectionAbility_CanActivate_UnderMaxRevives_ReturnsTrue()
        {
            // Arrange
            var ability = new ResurrectionAbility(AbilityName.ComesBackFromDeath, _playerRepository, 3, false);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region DeathTriggerAbility Tests

        [Test]
        public void DeathTriggerAbility_Name_ReturnsGiveDeathWhenDie()
        {
            // Arrange
            var ability = new DeathTriggerAbility(_playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.GiveDeathWhenDie, ability.Name);
        }

        [Test]
        public void DeathTriggerAbility_Description_ContainsDestroy()
        {
            // Arrange
            var ability = new DeathTriggerAbility(_playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Destroy"));
        }

        [Test]
        public void DeathTriggerAbility_Trigger_IsOnDeath()
        {
            // Arrange
            var ability = new DeathTriggerAbility(_playerRepository);

            // Assert
            Assert.AreEqual(AbilityTrigger.OnDeath, ability.Trigger);
        }

        [Test]
        public void DeathTriggerAbility_CanActivate_WithOpponentCards_ReturnsTrue()
        {
            // Arrange
            var ability = new DeathTriggerAbility(_playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeathTriggerAbility_Execute_NeedsUserInput()
        {
            // Arrange
            var ability = new DeathTriggerAbility(_playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region DeathRewardAbility Tests

        [Test]
        public void DeathRewardAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new DeathRewardAbility(AbilityName.Default, "Bonus Card", _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void DeathRewardAbility_Description_ContainsCardName()
        {
            // Arrange
            var ability = new DeathRewardAbility(AbilityName.Default, "Bonus Card", _playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Bonus Card"));
        }

        [Test]
        public void DeathRewardAbility_Trigger_IsOnDeath()
        {
            // Arrange
            var ability = new DeathRewardAbility(AbilityName.Default, "Bonus Card", _playerRepository);

            // Assert
            Assert.AreEqual(AbilityTrigger.OnDeath, ability.Trigger);
        }

        [Test]
        public void DeathRewardAbility_CanActivate_WithCardInDeck_ReturnsTrue()
        {
            // Arrange
            var bonusCard = Card.CreateInvocation(
                CardId.New(),
                "Bonus Card",
                "Test",
                "Test",
                2, 2,
                new[] { CardFamily.Human },
                false
            );
            // bonusCard first, _sourceCard last - DrawCard draws from END of deck
            _player1 = new Player(PlayerId.Player1, new List<Card> { bonusCard, _sourceCard });
            _player1.DrawCard(); // Draws _sourceCard (last element)
            _player1.PlayCard(_sourceCard);
            // bonusCard remains in deck (at index 0)
            _playerRepository.AddPlayer(_player1);

            var ability = new DeathRewardAbility(AbilityName.Default, "Bonus Card", _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeathRewardAbility_CanActivate_WithoutCardInDeck_ReturnsFalse()
        {
            // Arrange - player has no "Bonus Card" in deck
            var ability = new DeathRewardAbility(AbilityName.Default, "Bonus Card", _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region DestroyOpponentCardAbility Tests

        [Test]
        public void DestroyOpponentCardAbility_Name_ReturnsCorrectAbilityName()
        {
            // Arrange
            var ability = new DestroyOpponentCardAbility(AbilityName.KillOpponentInvocation, _playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.KillOpponentInvocation, ability.Name);
        }

        [Test]
        public void DestroyOpponentCardAbility_Description_Generic()
        {
            // Arrange
            var ability = new DestroyOpponentCardAbility(AbilityName.KillOpponentInvocation, _playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Destroy opponent's card"));
        }

        [Test]
        public void DestroyOpponentCardAbility_Description_WithCardType()
        {
            // Arrange
            var ability = new DestroyOpponentCardAbility(
                AbilityName.KillOpponentInvocation,
                _playerRepository,
                CardType.Invocation);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Invocation"));
        }

        [Test]
        public void DestroyOpponentCardAbility_CanActivate_WithOpponentCards_ReturnsTrue()
        {
            // Arrange
            var ability = new DestroyOpponentCardAbility(AbilityName.KillOpponentInvocation, _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DestroyOpponentCardAbility_CanActivate_EmptyOpponentField_ReturnsFalse()
        {
            // Arrange
            _player2 = new Player(PlayerId.Player2, new List<Card>());
            _playerRepository.AddPlayer(_player2);

            var ability = new DestroyOpponentCardAbility(AbilityName.KillOpponentInvocation, _playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DestroyOpponentCardAbility_Execute_SingleTarget_DestroysCard()
        {
            // Arrange
            var ability = new DestroyOpponentCardAbility(AbilityName.KillOpponentInvocation, _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("Destroyed"));
        }

        [Test]
        public void DestroyOpponentCardAbility_Execute_MultipleTargets_NeedsUserInput()
        {
            // Arrange
            var extraCard = Card.CreateInvocation(
                CardId.New(),
                "Extra Card",
                "Test",
                "Test",
                2, 2,
                new[] { CardFamily.Human },
                false
            );
            _player2 = new Player(PlayerId.Player2, new List<Card> { _targetCard, extraCard });
            _player2.DrawCard();
            _player2.DrawCard();
            _player2.PlayCard(_targetCard);
            _player2.PlayCard(extraCard);
            _playerRepository.AddPlayer(_player2);

            var ability = new DestroyOpponentCardAbility(AbilityName.KillOpponentInvocation, _playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region CombatAbilityFactory Tests

        [Test]
        public void CombatAbilityFactory_CreateMutualDestruction_ReturnsAbility()
        {
            // Arrange
            var factory = new CombatAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateMutualDestruction(AbilityName.KillEnemyIfDestroy);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.KillEnemyIfDestroy, ability.Name);
        }

        [Test]
        public void CombatAbilityFactory_CreateSkipAttack_ReturnsAbility()
        {
            // Arrange
            var factory = new CombatAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateSkipAttack(AbilityName.SkipOpponentAttackEveryTurn, true);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.SkipOpponentAttackEveryTurn, ability.Name);
        }

        [Test]
        public void CombatAbilityFactory_CreateDirectAttack_ReturnsAbility()
        {
            // Arrange
            var factory = new CombatAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateDirectAttack();

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void CombatAbilityFactory_CreateResurrection_ReturnsAbility()
        {
            // Arrange
            var factory = new CombatAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateResurrection(AbilityName.ComesBackFromDeath, 5, false);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.ComesBackFromDeath, ability.Name);
        }

        [Test]
        public void CombatAbilityFactory_CreateDeathTrigger_ReturnsAbility()
        {
            // Arrange
            var factory = new CombatAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateDeathTrigger();

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.GiveDeathWhenDie, ability.Name);
        }

        [Test]
        public void CombatAbilityFactory_CreateDeathReward_ReturnsAbility()
        {
            // Arrange
            var factory = new CombatAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateDeathReward(AbilityName.Default, "Reward Card");

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void CombatAbilityFactory_CreateDestroyOpponentCard_ReturnsAbility()
        {
            // Arrange
            var factory = new CombatAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateDestroyOpponentCard(AbilityName.KillOpponentInvocation, CardType.Invocation);

            // Assert
            Assert.IsNotNull(ability);
            Assert.AreEqual(AbilityName.KillOpponentInvocation, ability.Name);
        }

        #endregion
    }

    #region Test Doubles

    public class TestPlayerRepositoryForCombat : IPlayerRepository
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
            _players.Remove(playerId);
        }
    }

    #endregion
}

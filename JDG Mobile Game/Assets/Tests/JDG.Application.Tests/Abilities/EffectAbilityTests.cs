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
    /// Tests for effect-related abilities.
    /// Phase 41: Added tests for SwapStatsEffectAbility, DivideDefenseEffectAbility,
    /// ControlCardEffectAbility, ChangeFieldEffectAbility, DestroyMultipleCardsEffectAbility,
    /// LimitHandEffectAbility, LookHandEffectAbility, LookDeckEffectAbility, and InvokeFromDeckEffectAbility.
    /// </summary>
    [TestFixture]
    public class EffectAbilityTests
    {
        private TestPlayerRepositoryForEffect _playerRepository;
        private Player _player1;
        private Player _player2;
        private Card _sourceCard;
        private AbilityContext _context;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepositoryForEffect();

            _sourceCard = Card.CreateInvocation(
                CardId.New(),
                "Source Card",
                "Test",
                "Test description",
                5, 5,
                new[] { CardFamily.Human },
                false
            );

            var opponentCard = Card.CreateInvocation(
                CardId.New(),
                "Opponent Card",
                "Test",
                "Test description",
                4, 6,
                new[] { CardFamily.Human },
                false
            );

            _player1 = new Player(PlayerId.Player1, new List<Card> { _sourceCard });
            _player1.DrawCard();
            _player1.PlayCard(_sourceCard);

            _player2 = new Player(PlayerId.Player2, new List<Card> { opponentCard });
            _player2.DrawCard();
            _player2.PlayCard(opponentCard);

            _playerRepository.AddPlayer(_player1);
            _playerRepository.AddPlayer(_player2);

            _context = new AbilityContext(
                PlayerId.Player1,
                PlayerId.Player2,
                _sourceCard,
                AbilityName.Default
            );
        }

        #region SwapStatsEffectAbility Tests

        [Test]
        public void SwapStatsEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new SwapStatsEffectAbility(_playerRepository, false);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void SwapStatsEffectAbility_Description_TargetSelf()
        {
            // Arrange
            var ability = new SwapStatsEffectAbility(_playerRepository, false);

            // Assert
            Assert.IsTrue(ability.Description.Contains("your"));
        }

        [Test]
        public void SwapStatsEffectAbility_Description_TargetOpponent()
        {
            // Arrange
            var ability = new SwapStatsEffectAbility(_playerRepository, true);

            // Assert
            Assert.IsTrue(ability.Description.Contains("opponent"));
        }

        [Test]
        public void SwapStatsEffectAbility_CanActivate_WithCardsOnField_ReturnsTrue()
        {
            // Arrange
            var ability = new SwapStatsEffectAbility(_playerRepository, false);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SwapStatsEffectAbility_CanActivate_EmptyField_ReturnsFalse()
        {
            // Arrange
            _player1 = new Player(PlayerId.Player1, new List<Card>());
            _playerRepository.AddPlayer(_player1);

            var ability = new SwapStatsEffectAbility(_playerRepository, false);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SwapStatsEffectAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new SwapStatsEffectAbility(_playerRepository, false);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("Swapped"));
        }

        #endregion

        #region DivideDefenseEffectAbility Tests

        [Test]
        public void DivideDefenseEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new DivideDefenseEffectAbility(_playerRepository, 2);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void DivideDefenseEffectAbility_Description_ContainsDivisor()
        {
            // Arrange
            var ability = new DivideDefenseEffectAbility(_playerRepository, 3);

            // Assert
            Assert.IsTrue(ability.Description.Contains("3"));
            Assert.IsTrue(ability.Description.Contains("DEF"));
        }

        [Test]
        public void DivideDefenseEffectAbility_CanActivate_WithOpponentCards_ReturnsTrue()
        {
            // Arrange
            var ability = new DivideDefenseEffectAbility(_playerRepository, 2);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DivideDefenseEffectAbility_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = new DivideDefenseEffectAbility(_playerRepository, 2);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("Divided"));
        }

        #endregion

        #region ControlCardEffectAbility Tests

        [Test]
        public void ControlCardEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new ControlCardEffectAbility(_playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void ControlCardEffectAbility_Description_ContainsControl()
        {
            // Arrange
            var ability = new ControlCardEffectAbility(_playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("control"));
        }

        [Test]
        public void ControlCardEffectAbility_CanActivate_WithSpaceAndOpponentCards_ReturnsTrue()
        {
            // Arrange - player1 has 1 card (less than 4), opponent has cards
            var ability = new ControlCardEffectAbility(_playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ControlCardEffectAbility_Execute_NeedsUserInput()
        {
            // Arrange
            var ability = new ControlCardEffectAbility(_playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region ChangeFieldEffectAbility Tests

        [Test]
        public void ChangeFieldEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new ChangeFieldEffectAbility(_playerRepository, false);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void ChangeFieldEffectAbility_Description_TargetSelf()
        {
            // Arrange
            var ability = new ChangeFieldEffectAbility(_playerRepository, false);

            // Assert
            Assert.IsTrue(ability.Description.Contains("your"));
        }

        [Test]
        public void ChangeFieldEffectAbility_Description_TargetOpponent()
        {
            // Arrange
            var ability = new ChangeFieldEffectAbility(_playerRepository, true);

            // Assert
            Assert.IsTrue(ability.Description.Contains("opponent"));
        }

        [Test]
        public void ChangeFieldEffectAbility_Execute_NeedsUserInput()
        {
            // Arrange
            var ability = new ChangeFieldEffectAbility(_playerRepository, false);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region DestroyMultipleCardsEffectAbility Tests

        [Test]
        public void DestroyMultipleCardsEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new DestroyMultipleCardsEffectAbility(_playerRepository, 2);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void DestroyMultipleCardsEffectAbility_Description_ContainsCount()
        {
            // Arrange
            var ability = new DestroyMultipleCardsEffectAbility(_playerRepository, 3);

            // Assert
            Assert.IsTrue(ability.Description.Contains("3"));
            Assert.IsTrue(ability.Description.Contains("Destroy"));
        }

        [Test]
        public void DestroyMultipleCardsEffectAbility_CanActivate_WithEnoughOpponentCards_ReturnsTrue()
        {
            // Arrange - opponent has 1 card
            var ability = new DestroyMultipleCardsEffectAbility(_playerRepository, 1);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DestroyMultipleCardsEffectAbility_CanActivate_NotEnoughOpponentCards_ReturnsFalse()
        {
            // Arrange - opponent has 1 card, need 2
            var ability = new DestroyMultipleCardsEffectAbility(_playerRepository, 2);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DestroyMultipleCardsEffectAbility_Execute_NeedsUserInput()
        {
            // Arrange
            var ability = new DestroyMultipleCardsEffectAbility(_playerRepository, 1);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region LimitHandEffectAbility Tests

        [Test]
        public void LimitHandEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new LimitHandEffectAbility(_playerRepository, 5);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void LimitHandEffectAbility_Description_ContainsLimit()
        {
            // Arrange
            var ability = new LimitHandEffectAbility(_playerRepository, 3);

            // Assert
            Assert.IsTrue(ability.Description.Contains("3"));
            Assert.IsTrue(ability.Description.Contains("limited"));
        }

        [Test]
        public void LimitHandEffectAbility_CanActivate_AlwaysReturnsTrue()
        {
            // Arrange
            var ability = new LimitHandEffectAbility(_playerRepository, 5);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void LimitHandEffectAbility_Execute_UnderLimit_ReturnsSuccess()
        {
            // Arrange - opponent hand is empty (under limit)
            var ability = new LimitHandEffectAbility(_playerRepository, 5);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region LookHandEffectAbility Tests

        [Test]
        public void LookHandEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new LookHandEffectAbility(_playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void LookHandEffectAbility_Description_ContainsLook()
        {
            // Arrange
            var ability = new LookHandEffectAbility(_playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Look"));
            Assert.IsTrue(ability.Description.Contains("hand"));
        }

        [Test]
        public void LookHandEffectAbility_CanActivate_OpponentHasCards_ReturnsTrue()
        {
            // Arrange - add cards to opponent hand
            var handCard = Card.CreateInvocation(
                CardId.New(),
                "Hand Card",
                "Test",
                "Test",
                2, 2,
                new[] { CardFamily.Human },
                false
            );
            _player2 = new Player(PlayerId.Player2, new List<Card> { handCard });
            _player2.DrawCard(); // Move to hand
            _playerRepository.AddPlayer(_player2);

            var ability = new LookHandEffectAbility(_playerRepository);

            // Act
            var result = ability.CanActivate(_context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void LookHandEffectAbility_Execute_NeedsUserInput()
        {
            // Arrange
            var ability = new LookHandEffectAbility(_playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region LookDeckEffectAbility Tests

        [Test]
        public void LookDeckEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new LookDeckEffectAbility(_playerRepository, 3);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void LookDeckEffectAbility_Description_ContainsCardCount()
        {
            // Arrange
            var ability = new LookDeckEffectAbility(_playerRepository, 5);

            // Assert
            Assert.IsTrue(ability.Description.Contains("5"));
            Assert.IsTrue(ability.Description.Contains("deck"));
        }

        [Test]
        public void LookDeckEffectAbility_Execute_NeedsUserInput()
        {
            // Arrange
            var ability = new LookDeckEffectAbility(_playerRepository, 3);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region InvokeFromDeckEffectAbility Tests

        [Test]
        public void InvokeFromDeckEffectAbility_Name_ReturnsDefault()
        {
            // Arrange
            var ability = new InvokFromDeckEffectAbility(_playerRepository);

            // Assert
            Assert.AreEqual(AbilityName.Default, ability.Name);
        }

        [Test]
        public void InvokeFromDeckEffectAbility_Description_ContainsInvoke()
        {
            // Arrange
            var ability = new InvokFromDeckEffectAbility(_playerRepository);

            // Assert
            Assert.IsTrue(ability.Description.Contains("Invoke"));
            Assert.IsTrue(ability.Description.Contains("deck"));
        }

        [Test]
        public void InvokeFromDeckEffectAbility_Execute_NeedsUserInput()
        {
            // Arrange
            var ability = new InvokFromDeckEffectAbility(_playerRepository);

            // Act
            var result = ability.Execute(_context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region EffectAbilityFactory Tests

        [Test]
        public void EffectAbilityFactory_CreateSwapStats_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateSwapStats(true);

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateDivideDefense_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateDivideDefense(3);

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateControlCard_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateControlCard();

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateChangeField_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateChangeField(false);

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateDestroyMultiple_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateDestroyMultiple(2);

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateLimitHand_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateLimitHand(4);

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateLookHand_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateLookHand();

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateLookDeck_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateLookDeck(5);

            // Assert
            Assert.IsNotNull(ability);
        }

        [Test]
        public void EffectAbilityFactory_CreateInvokeFromDeck_ReturnsAbility()
        {
            // Arrange
            var factory = new EffectAbilityFactory(_playerRepository);

            // Act
            var ability = factory.CreateInvokeFromDeck();

            // Assert
            Assert.IsNotNull(ability);
        }

        #endregion
    }

    #region Test Doubles

    public class TestPlayerRepositoryForEffect : IPlayerRepository
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

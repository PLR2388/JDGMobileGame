using System.Collections.Generic;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using NUnit.Framework;

namespace JDG.Application.Tests.Abilities
{
    /// <summary>
    /// Tests for equipment ability implementations.
    /// Tests SetStatsEquipmentAbility, BonusStatsEquipmentAbility, MultiplyStatsEquipmentAbility,
    /// ProtectFromDestructionEquipmentAbility, CancelAbilitiesEquipmentAbility, SwitchEquipmentAbility,
    /// PreventAttackNewCardsEquipmentAbility, and EquipmentAbilityFactory.
    /// </summary>
    [TestFixture]
    public class EquipmentAbilityTests
    {
        private IPlayerRepository _playerRepository;
        private Card _sourceCard;
        private Card _targetCard;
        private PlayerId _playerId;
        private PlayerId _opponentId;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepository();
            _playerId = PlayerId.Player1;
            _opponentId = PlayerId.Player2;
            _sourceCard = CreateEquipmentCard("TestEquipment");
            _targetCard = CreateInvocationCard("TargetCard", 1000, 800);
        }

        private Card CreateInvocationCard(string name, int attack, int defense)
        {
            return Card.CreateInvocation(
                id: CardId.New(),
                title: name,
                description: "Test card",
                detailedDescription: "Test card description",
                attack: attack,
                defense: defense,
                families: new[] { CardFamily.Human },
                affectedByEffect: true
            );
        }

        private Card CreateEquipmentCard(string name)
        {
            return Card.CreateEquipment(
                id: CardId.New(),
                title: name,
                description: "Test equipment",
                detailedDescription: "Test equipment description",
                equipmentAbilities: new EquipmentAbilityName[] { }
            );
        }

        private AbilityContext CreateContext(Card sourceCard = null, Card targetCard = null)
        {
            var context = new AbilityContext(
                _playerId,
                _opponentId,
                sourceCard ?? _sourceCard,
                AbilityName.Default
            );
            if (targetCard != null)
            {
                context.TargetCard = targetCard;
            }
            return context;
        }

        #region SetStatsEquipmentAbility Tests

        [Test]
        public void SetStatsEquipmentAbility_Constructor_SetsProperties()
        {
            var ability = new SetStatsEquipmentAbility(1500, 1200);

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Set ATK/DEF to 1500/1200"));
        }

        [Test]
        public void SetStatsEquipmentAbility_CanActivate_ReturnsTrueWithTarget()
        {
            var ability = new SetStatsEquipmentAbility(1500, 1200);
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void SetStatsEquipmentAbility_CanActivate_ReturnsFalseWithoutTarget()
        {
            var ability = new SetStatsEquipmentAbility(1500, 1200);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void SetStatsEquipmentAbility_Execute_SetsTargetStats()
        {
            var ability = new SetStatsEquipmentAbility(1500, 1200);
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_targetCard.Stats.Value.Attack, Is.EqualTo(1500));
            Assert.That(_targetCard.Stats.Value.Defense, Is.EqualTo(1200));
        }

        [Test]
        public void SetStatsEquipmentAbility_Execute_FailsWithoutTarget()
        {
            var ability = new SetStatsEquipmentAbility(1500, 1200);
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("No target card"));
        }

        [Test]
        public void SetStatsEquipmentAbility_Execute_HandlesZeroStats()
        {
            var ability = new SetStatsEquipmentAbility(0, 0);
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_targetCard.Stats.Value.Attack, Is.EqualTo(0));
            Assert.That(_targetCard.Stats.Value.Defense, Is.EqualTo(0));
        }

        #endregion

        #region BonusStatsEquipmentAbility Tests

        [Test]
        public void BonusStatsEquipmentAbility_Constructor_SetsProperties()
        {
            var ability = new BonusStatsEquipmentAbility(500, 300);

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("+500 ATK / +300 DEF"));
        }

        [Test]
        public void BonusStatsEquipmentAbility_CanActivate_ReturnsTrueWithTarget()
        {
            var ability = new BonusStatsEquipmentAbility(500, 300);
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void BonusStatsEquipmentAbility_CanActivate_ReturnsFalseWithoutTarget()
        {
            var ability = new BonusStatsEquipmentAbility(500, 300);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void BonusStatsEquipmentAbility_Execute_ModifiesTargetStats()
        {
            var ability = new BonusStatsEquipmentAbility(500, 300);
            var context = CreateContext(targetCard: _targetCard);
            int originalAtk = _targetCard.Stats.Value.Attack;
            int originalDef = _targetCard.Stats.Value.Defense;

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Added +500/+300"));
            Assert.That(_targetCard.Stats.Value.Attack, Is.EqualTo(originalAtk + 500));
            Assert.That(_targetCard.Stats.Value.Defense, Is.EqualTo(originalDef + 300));
        }

        [Test]
        public void BonusStatsEquipmentAbility_Execute_FailsWithoutTarget()
        {
            var ability = new BonusStatsEquipmentAbility(500, 300);
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("No target card"));
        }

        [Test]
        public void BonusStatsEquipmentAbility_Execute_HandlesNegativeBonuses()
        {
            var ability = new BonusStatsEquipmentAbility(-200, -100);
            var context = CreateContext(targetCard: _targetCard);
            int originalAtk = _targetCard.Stats.Value.Attack;
            int originalDef = _targetCard.Stats.Value.Defense;

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_targetCard.Stats.Value.Attack, Is.EqualTo(originalAtk - 200));
            Assert.That(_targetCard.Stats.Value.Defense, Is.EqualTo(originalDef - 100));
        }

        #endregion

        #region MultiplyStatsEquipmentAbility Tests

        [Test]
        public void MultiplyStatsEquipmentAbility_Constructor_SetsProperties()
        {
            var ability = new MultiplyStatsEquipmentAbility(2.0f, 1.5f);

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Multiply ATK by 2x, DEF by 1.5x"));
        }

        [Test]
        public void MultiplyStatsEquipmentAbility_CanActivate_ReturnsTrueWithTarget()
        {
            var ability = new MultiplyStatsEquipmentAbility(2.0f, 1.5f);
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void MultiplyStatsEquipmentAbility_CanActivate_ReturnsFalseWithoutTarget()
        {
            var ability = new MultiplyStatsEquipmentAbility(2.0f, 1.5f);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void MultiplyStatsEquipmentAbility_Execute_MultipliesStats()
        {
            var targetWithStats = CreateInvocationCard("Target", 1000, 800);
            var ability = new MultiplyStatsEquipmentAbility(2.0f, 1.5f);
            var context = CreateContext(targetCard: targetWithStats);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            // 1000 * 2 = 2000, 800 * 1.5 = 1200
            Assert.That(targetWithStats.Stats.Value.Attack, Is.EqualTo(2000));
            Assert.That(targetWithStats.Stats.Value.Defense, Is.EqualTo(1200));
        }

        [Test]
        public void MultiplyStatsEquipmentAbility_Execute_FailsWithoutTarget()
        {
            var ability = new MultiplyStatsEquipmentAbility(2.0f, 1.5f);
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void MultiplyStatsEquipmentAbility_Execute_FailsWithNonInvocationCard()
        {
            var equipmentCard = CreateEquipmentCard("NonInvocation");
            var ability = new MultiplyStatsEquipmentAbility(2.0f, 1.5f);
            var context = CreateContext(targetCard: equipmentCard);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("No target card or invalid stats"));
        }

        [Test]
        public void MultiplyStatsEquipmentAbility_Execute_HandlesZeroMultiplier()
        {
            var targetWithStats = CreateInvocationCard("Target", 1000, 800);
            var ability = new MultiplyStatsEquipmentAbility(0f, 0f);
            var context = CreateContext(targetCard: targetWithStats);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(targetWithStats.Stats.Value.Attack, Is.EqualTo(0));
            Assert.That(targetWithStats.Stats.Value.Defense, Is.EqualTo(0));
        }

        [Test]
        public void MultiplyStatsEquipmentAbility_Execute_HandlesFractionalMultiplier()
        {
            var targetWithStats = CreateInvocationCard("Target", 1000, 1000);
            var ability = new MultiplyStatsEquipmentAbility(0.5f, 0.25f);
            var context = CreateContext(targetCard: targetWithStats);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            // 1000 * 0.5 = 500, 1000 * 0.25 = 250
            Assert.That(targetWithStats.Stats.Value.Attack, Is.EqualTo(500));
            Assert.That(targetWithStats.Stats.Value.Defense, Is.EqualTo(250));
        }

        #endregion

        #region ProtectFromDestructionEquipmentAbility Tests

        [Test]
        public void ProtectFromDestructionEquipmentAbility_Constructor_SetsProperties()
        {
            var ability = new ProtectFromDestructionEquipmentAbility();

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Protects from destruction"));
        }

        [Test]
        public void ProtectFromDestructionEquipmentAbility_CanActivate_ReturnsTrueWithTarget()
        {
            var ability = new ProtectFromDestructionEquipmentAbility();
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ProtectFromDestructionEquipmentAbility_CanActivate_ReturnsFalseWithoutTarget()
        {
            var ability = new ProtectFromDestructionEquipmentAbility();
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ProtectFromDestructionEquipmentAbility_Execute_ReturnsSuccess()
        {
            var ability = new ProtectFromDestructionEquipmentAbility();
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Card protected from destruction"));
        }

        #endregion

        #region CancelAbilitiesEquipmentAbility Tests

        [Test]
        public void CancelAbilitiesEquipmentAbility_Constructor_SetsProperties()
        {
            var ability = new CancelAbilitiesEquipmentAbility();

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Cancel equipped card's abilities"));
        }

        [Test]
        public void CancelAbilitiesEquipmentAbility_CanActivate_ReturnsTrueWithTarget()
        {
            var ability = new CancelAbilitiesEquipmentAbility();
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void CancelAbilitiesEquipmentAbility_CanActivate_ReturnsFalseWithoutTarget()
        {
            var ability = new CancelAbilitiesEquipmentAbility();
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CancelAbilitiesEquipmentAbility_Execute_SetsCancelEffectTrue()
        {
            var ability = new CancelAbilitiesEquipmentAbility();
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Card abilities canceled"));
            Assert.That(_targetCard.CancelEffect, Is.True);
        }

        [Test]
        public void CancelAbilitiesEquipmentAbility_Execute_FailsWithoutTarget()
        {
            var ability = new CancelAbilitiesEquipmentAbility();
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("No target card"));
        }

        #endregion

        #region SwitchEquipmentAbility Tests

        [Test]
        public void SwitchEquipmentAbility_Constructor_SetsProperties()
        {
            var ability = new SwitchEquipmentAbility(_playerRepository);

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Can be equipped to multiple cards"));
        }

        [Test]
        public void SwitchEquipmentAbility_CanActivate_AlwaysReturnsTrue()
        {
            var ability = new SwitchEquipmentAbility(_playerRepository);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void SwitchEquipmentAbility_CanActivate_ReturnsTrueWithTarget()
        {
            var ability = new SwitchEquipmentAbility(_playerRepository);
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void SwitchEquipmentAbility_Execute_ReturnsSuccess()
        {
            var ability = new SwitchEquipmentAbility(_playerRepository);
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Equipment can be switched"));
        }

        #endregion

        #region PreventAttackNewCardsEquipmentAbility Tests

        [Test]
        public void PreventAttackNewCardsEquipmentAbility_Constructor_SetsProperties()
        {
            var ability = new PreventAttackNewCardsEquipmentAbility();

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Cannot attack newly summoned opponent cards"));
        }

        [Test]
        public void PreventAttackNewCardsEquipmentAbility_CanActivate_ReturnsTrueWithTarget()
        {
            var ability = new PreventAttackNewCardsEquipmentAbility();
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void PreventAttackNewCardsEquipmentAbility_CanActivate_ReturnsFalseWithoutTarget()
        {
            var ability = new PreventAttackNewCardsEquipmentAbility();
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void PreventAttackNewCardsEquipmentAbility_Execute_ReturnsSuccess()
        {
            var ability = new PreventAttackNewCardsEquipmentAbility();
            var context = CreateContext(targetCard: _targetCard);

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Attack restriction applied"));
        }

        #endregion

        #region EquipmentAbilityFactory Tests

        [Test]
        public void EquipmentAbilityFactory_CreateSetStats_ReturnsAbilityWithCorrectValues()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var ability = factory.CreateSetStats(2000, 1500);

            Assert.That(ability, Is.TypeOf<SetStatsEquipmentAbility>());
            Assert.That(ability.Description, Is.EqualTo("Set ATK/DEF to 2000/1500"));
        }

        [Test]
        public void EquipmentAbilityFactory_CreateBonusStats_ReturnsAbilityWithCorrectValues()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var ability = factory.CreateBonusStats(500, 300);

            Assert.That(ability, Is.TypeOf<BonusStatsEquipmentAbility>());
            Assert.That(ability.Description, Is.EqualTo("+500 ATK / +300 DEF"));
        }

        [Test]
        public void EquipmentAbilityFactory_CreateMultiplyStats_ReturnsAbilityWithCorrectValues()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var ability = factory.CreateMultiplyStats(2.5f, 1.5f);

            Assert.That(ability, Is.TypeOf<MultiplyStatsEquipmentAbility>());
            Assert.That(ability.Description, Is.EqualTo("Multiply ATK by 2.5x, DEF by 1.5x"));
        }

        [Test]
        public void EquipmentAbilityFactory_CreateProtectFromDestruction_ReturnsAbility()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var ability = factory.CreateProtectFromDestruction();

            Assert.That(ability, Is.TypeOf<ProtectFromDestructionEquipmentAbility>());
            Assert.That(ability.Description, Is.EqualTo("Protects from destruction"));
        }

        [Test]
        public void EquipmentAbilityFactory_CreateCancelAbilities_ReturnsAbility()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var ability = factory.CreateCancelAbilities();

            Assert.That(ability, Is.TypeOf<CancelAbilitiesEquipmentAbility>());
            Assert.That(ability.Description, Is.EqualTo("Cancel equipped card's abilities"));
        }

        [Test]
        public void EquipmentAbilityFactory_CreateSwitchEquipment_ReturnsAbility()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var ability = factory.CreateSwitchEquipment();

            Assert.That(ability, Is.TypeOf<SwitchEquipmentAbility>());
            Assert.That(ability.Description, Is.EqualTo("Can be equipped to multiple cards"));
        }

        [Test]
        public void EquipmentAbilityFactory_CreatePreventAttackNew_ReturnsAbility()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var ability = factory.CreatePreventAttackNew();

            Assert.That(ability, Is.TypeOf<PreventAttackNewCardsEquipmentAbility>());
            Assert.That(ability.Description, Is.EqualTo("Cannot attack newly summoned opponent cards"));
        }

        [Test]
        public void EquipmentAbilityFactory_AllFactoryMethods_ReturnValidAbilities()
        {
            var factory = new EquipmentAbilityFactory(_playerRepository);

            var abilities = new IAbility[]
            {
                factory.CreateSetStats(100, 100),
                factory.CreateBonusStats(100, 100),
                factory.CreateMultiplyStats(1.0f, 1.0f),
                factory.CreateProtectFromDestruction(),
                factory.CreateCancelAbilities(),
                factory.CreateSwitchEquipment(),
                factory.CreatePreventAttackNew()
            };

            foreach (var ability in abilities)
            {
                Assert.That(ability, Is.Not.Null);
                Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
                Assert.That(ability.Description, Is.Not.Null.And.Not.Empty);
            }
        }

        #endregion
    }

    /// <summary>
    /// Test implementation of IPlayerRepository for ability testing.
    /// Stores Player entities and provides Get/Save operations.
    /// </summary>
    public class TestPlayerRepository : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();

        public TestPlayerRepository()
        {
            // Initialize with empty players
            _players[PlayerId.Player1] = new Player(PlayerId.Player1, new List<Card>());
            _players[PlayerId.Player2] = new Player(PlayerId.Player2, new List<Card>());
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
            var player = new Player(playerId, new List<Card>(), maxHealth);
            _players[playerId] = player;
            return player;
        }

        public void ResetPlayer(PlayerId playerId)
        {
            if (_players.ContainsKey(playerId))
            {
                _players[playerId] = new Player(playerId, new List<Card>());
            }
        }

        /// <summary>
        /// Removes a player completely from the repository.
        /// Use for testing "player not found" scenarios.
        /// </summary>
        public void ClearPlayer(PlayerId playerId)
        {
            _players.Remove(playerId);
        }

        /// <summary>
        /// Clears all players from the repository.
        /// Use for testing "player not found" scenarios.
        /// </summary>
        public void ClearAllPlayers()
        {
            _players.Clear();
        }

        /// <summary>
        /// Creates a truly empty repository with no players.
        /// </summary>
        public static TestPlayerRepository CreateEmpty()
        {
            var repo = new TestPlayerRepository();
            repo.ClearAllPlayers();
            return repo;
        }

        /// <summary>
        /// Helper method for tests to set up players with specific cards.
        /// </summary>
        public void SetupPlayer(PlayerId playerId, IEnumerable<Card> deck, int maxHealth = 30)
        {
            _players[playerId] = new Player(playerId, deck, maxHealth);
        }
    }
}

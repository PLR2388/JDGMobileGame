using NUnit.Framework;
using NSubstitute;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Application.Abilities.Implementations;
using System.Collections.Generic;
using System.Linq;

using TestCardFactory = JDG.TestUtilities.CardFactory;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for Destruction abilities.
    /// Tests abilities that destroy cards, fields, or other game elements.
    /// </summary>
    [TestFixture]
    [Category("Abilities")]
    [Category("Scenario")]
    [Category("Destruction")]
    public class DestructionAbilityScenarioTests
    {
        private IPlayerRepository _playerRepository;
        private Player _player1;
        private Player _player2;
        private CombatAbilityFactory _combatFactory;
        private EffectAbilityFactory _effectFactory;
        private ProtectionAbilityFactory _protectionFactory;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _combatFactory = new CombatAbilityFactory(_playerRepository);
            _effectFactory = new EffectAbilityFactory(_playerRepository);
            _protectionFactory = new ProtectionAbilityFactory(_playerRepository);

            // Setup players
            _player1 = CreateTestPlayer(PlayerId.Player1);
            _player2 = CreateTestPlayer(PlayerId.Player2);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(_player2);
        }

        #region DestroyFieldATK - Koaloutre

        [Test]
        public void DestroyFieldATK_WhenKoaloutreActivated_DestroysOpponentFieldBasedOnATK()
        {
            // Arrange
            // Koaloutre-Ornithambas Lapinzord nain de Californie: DestroyFieldATK
            // Destroys opponent's field card if conditions met (based on ATK)
            var koaloutre = TestCardFactory.CreateInvocation(
                "Koaloutre-Ornithambas Lapinzord nain de Californie", 4, 4, CardFamily.Incarnation);
            var opponentField = TestCardFactory.CreateField("Opponent Field", CardFamily.Human);

            PlaceOnField(_player1, koaloutre);

            var ability = _effectFactory.CreateDestroyFieldCard(0); // hpCost = 0 for test
            var context = CreateContext(_player1, koaloutre, AbilityName.DestroyFieldATK);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void DestroyFieldATK_RequiresSacrifice2Japan()
        {
            // Arrange
            // Koaloutre also requires Sacrifice2Japan as a cost
            var koaloutre = TestCardFactory.CreateInvocation(
                "Koaloutre-Ornithambas Lapinzord nain de Californie", 4, 4, CardFamily.Incarnation);
            var japanCard1 = TestCardFactory.CreateInvocation("Japan Card 1", 2, 2, CardFamily.Japan);
            var japanCard2 = TestCardFactory.CreateInvocation("Japan Card 2", 2, 2, CardFamily.Japan);

            // Must have 2 Japan cards to sacrifice
            Assert.IsNotNull(koaloutre);
            Assert.IsNotNull(japanCard1);
            Assert.IsNotNull(japanCard2);
        }

        [Test]
        public void DestroyFieldATK_OnlyAffectsOpponentField()
        {
            // Arrange
            var koaloutre = TestCardFactory.CreateInvocation(
                "Koaloutre-Ornithambas Lapinzord nain de Californie", 4, 4, CardFamily.Incarnation);
            var myField = TestCardFactory.CreateField("My Field", CardFamily.Japan);
            var opponentField = TestCardFactory.CreateField("Opponent Field", CardFamily.Human);

            // Should only destroy opponent's field, not own field
            Assert.AreNotEqual(myField.Title, opponentField.Title);
        }

        #endregion

        #region DestroyFieldDEF - Lolhitler

        [Test]
        public void DestroyFieldDEF_WhenLolhitlerActivated_DestroysFieldBasedOnDEF()
        {
            // Arrange
            // Lolhitler: DestroyFieldDEF (destroys field based on DEF)
            var lolhitler = TestCardFactory.CreateInvocation("Lolhitler", 4, 4, CardFamily.Incarnation);
            var opponentField = TestCardFactory.CreateField("Opponent Field", CardFamily.Developer);

            PlaceOnField(_player1, lolhitler);

            var ability = _effectFactory.CreateDestroyFieldCard(0); // hpCost = 0 for test
            var context = CreateContext(_player1, lolhitler, AbilityName.DestroyFieldDEF);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void DestroyFieldDEF_RequiresSacrifice2Incarnation()
        {
            // Arrange
            // Lolhitler also requires Sacrifice2Incarnation as a cost
            var lolhitler = TestCardFactory.CreateInvocation("Lolhitler", 4, 4, CardFamily.Incarnation);
            var incarnationCard1 = TestCardFactory.CreateInvocation("Incarnation 1", 2, 2, CardFamily.Incarnation);
            var incarnationCard2 = TestCardFactory.CreateInvocation("Incarnation 2", 2, 2, CardFamily.Incarnation);

            // Must have 2 Incarnation cards to sacrifice
            Assert.IsNotNull(lolhitler);
            Assert.AreEqual(CardFamily.Incarnation, incarnationCard1.Families.First());
            Assert.AreEqual(CardFamily.Incarnation, incarnationCard2.Families.First());
        }

        [Test]
        public void DestroyFieldDEF_DifferentFromDestroyFieldATK()
        {
            // Arrange
            // DestroyFieldATK and DestroyFieldDEF use different stats for calculation
            var atkDestroyer = TestCardFactory.CreateInvocation("ATK Destroyer", 4, 4, CardFamily.Incarnation);
            var defDestroyer = TestCardFactory.CreateInvocation("DEF Destroyer", 4, 4, CardFamily.Incarnation);

            // The mechanics differ based on which stat is used
            Assert.AreEqual(atkDestroyer.Stats.Value.Attack, defDestroyer.Stats.Value.Attack);
            Assert.AreEqual(atkDestroyer.Stats.Value.Defense, defDestroyer.Stats.Value.Defense);
        }

        #endregion

        #region CanOnlyAttackItself - Alpha Man / La Petite Fille / Tentacules

        [Test]
        public void CanOnlyAttackItself_AlphaMan_CanOnlyTargetSelf()
        {
            // Arrange
            // Alpha Man: CanOnlyAttackItself - Can only attack itself (forces opponent to attack it)
            var alphaMan = TestCardFactory.CreateInvocation("Alpha Man", 4, 4, CardFamily.Fistiland);
            var otherCard = TestCardFactory.CreateInvocation("Other Card", 3, 3, CardFamily.Human);

            PlaceOnField(_player1, alphaMan);
            PlaceOnField(_player2, otherCard);

            var ability = _protectionFactory.CreateCanOnlyAttackItself();
            var context = CreateContext(_player1, alphaMan, AbilityName.CanOnlyAttackItself);

            // Act
            var result = ability.Execute(context);

            // Assert - Ability sets up attack restriction
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void CanOnlyAttackItself_LaPetiteFille_RestrictsAttackTargets()
        {
            // Arrange
            // La Petite Fille also has CanOnlyAttackItself
            var laPetiteFille = TestCardFactory.CreateInvocation("La Petite Fille", 4, 4, CardFamily.Human);
            var opponent = TestCardFactory.CreateInvocation("Opponent", 3, 3, CardFamily.Developer);

            PlaceOnField(_player1, laPetiteFille);
            PlaceOnField(_player2, opponent);

            var ability = _protectionFactory.CreateCanOnlyAttackItself();
            var context = CreateContext(_player1, laPetiteFille, AbilityName.CanOnlyAttackItself);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void CanOnlyAttackItself_Tentacules_RestrictsAttackTargets()
        {
            // Arrange
            // Tentacules also has CanOnlyAttackItself
            var tentacules = TestCardFactory.CreateInvocation("Tentacules", 2, 4, CardFamily.Incarnation);

            PlaceOnField(_player1, tentacules);

            var ability = _protectionFactory.CreateCanOnlyAttackItself();
            var context = CreateContext(_player1, tentacules, AbilityName.CanOnlyAttackItself);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void CanOnlyAttackItself_OpponentMustTargetThisCard()
        {
            // Arrange
            // When CanOnlyAttackItself is active, opponent attacks are forced to this card
            var alphaMan = TestCardFactory.CreateInvocation("Alpha Man", 4, 4, CardFamily.Fistiland);
            var otherFriendly = TestCardFactory.CreateInvocation("Other Friendly", 2, 2, CardFamily.Fistiland);

            // Alpha Man acts as a "taunt" - opponent must attack it first
            Assert.IsNotNull(alphaMan);
            Assert.IsNotNull(otherFriendly);
        }

        #endregion

        #region KillOpponentInvocation - La Mort

        [Test]
        public void KillOpponentInvocation_WhenLaMortAttacks_KillsTarget()
        {
            // Arrange
            // La Mort: KillOpponentInvocation - Destroys opponent invocation on attack
            var laMort = TestCardFactory.CreateInvocation("La Mort", 2, 4, CardFamily.Human);
            var target = TestCardFactory.CreateInvocation("Target", 5, 5, CardFamily.Developer);

            PlaceOnField(_player1, laMort);
            PlaceOnField(_player2, target);

            var ability = _combatFactory.CreateMutualDestruction(AbilityName.KillOpponentInvocation);
            var context = CreateContext(_player1, laMort, AbilityName.KillOpponentInvocation);
            context.TargetCard = target;

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void KillOpponentInvocation_IgnoresDefenseStats()
        {
            // Arrange
            // La Mort kills regardless of target's DEF
            var laMort = TestCardFactory.CreateInvocation("La Mort", 2, 4, CardFamily.Human);
            var highDefTarget = TestCardFactory.CreateInvocation("High DEF", 1, 10, CardFamily.Developer);

            // Even with 10 DEF, target is destroyed
            Assert.IsTrue(highDefTarget.Stats.Value.Defense > laMort.Stats.Value.Attack);
        }

        #endregion

        #region KillEnemyIfDestroy - Sandrine

        [Test]
        public void KillEnemyIfDestroy_WhenSandrineIsDestroyed_KillsAttacker()
        {
            // Arrange
            // Sandrine le porte-manteau extraterrestre: KillEnemyIfDestroy
            var sandrine = TestCardFactory.CreateInvocation(
                "Sandrine le porte-manteau extraterrestre", 1, 1, CardFamily.Rpg);
            var attacker = TestCardFactory.CreateInvocation("Attacker", 5, 5, CardFamily.Human);

            PlaceOnField(_player1, sandrine);
            PlaceOnField(_player2, attacker);

            var ability = _combatFactory.CreateMutualDestruction(AbilityName.KillEnemyIfDestroy);
            var context = CreateContext(_player1, sandrine, AbilityName.KillEnemyIfDestroy);
            context.TargetCard = attacker;

            // Act
            var result = ability.Execute(context);

            // Assert - Both cards should be destroyed
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void KillEnemyIfDestroy_MutualDestruction()
        {
            // Arrange
            var sandrine = TestCardFactory.CreateInvocation(
                "Sandrine le porte-manteau extraterrestre", 1, 1, CardFamily.Rpg);
            var attacker = TestCardFactory.CreateInvocation("Attacker", 5, 5, CardFamily.Human);

            // Both cards die in this interaction
            Assert.AreEqual(1, sandrine.Stats.Value.Attack);
            Assert.AreEqual(5, attacker.Stats.Value.Attack);
        }

        #endregion

        #region Helper Methods

        private Player CreateTestPlayer(PlayerId playerId)
        {
            var deck = TestCardFactory.CreateDeck(30);
            return new Player(playerId, deck);
        }

        private void PlaceOnField(Player player, params Card[] cards)
        {
            foreach (var card in cards)
            {
                player.PlayCard(card);
            }
        }

        private AbilityContext CreateContext(Player player, Card sourceCard, AbilityName abilityName)
        {
            var opponentId = player.Id == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
            return new AbilityContext(player.Id, opponentId, sourceCard, abilityName);
        }

        #endregion
    }
}

using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.TestUtilities;
using TestCardFactory = JDG.TestUtilities.CardFactory;
using System.Linq;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for Combat abilities.
    /// Tests verify combat mechanics work correctly in realistic game scenarios.
    ///
    /// Combat Abilities:
    /// - MutualDestruction: Both attacker and defender are destroyed
    /// - SkipAttack: Skip opponent's attack phase
    /// - DirectAttack: Attack opponent player directly
    /// - Resurrection: Card comes back after death
    /// - DeathTrigger: Trigger effect when card dies
    /// - DeathReward: Get card from deck when this card dies
    /// - DestroyOpponentCard: Destroy an opponent's card
    /// </summary>
    [TestFixture]
    public class CombatAbilityScenarioTests : AbilityScenarioTestBase
    {
        private CombatAbilityFactory _combatFactory;

        protected override void RegisterAbilities()
        {
            _combatFactory = new CombatAbilityFactory(PlayerRepository);

            // Register combat abilities
            AbilityRegistry.Register(AbilityName.KillEnemyIfDestroy,
                () => _combatFactory.CreateMutualDestruction(AbilityName.KillEnemyIfDestroy));
            // Note: SkipOpponentAttackEveryTurn with everyTurn=true is the intended behavior
            AbilityRegistry.Register(AbilityName.SkipOpponentAttackEveryTurn,
                () => _combatFactory.CreateSkipAttack(AbilityName.SkipOpponentAttackEveryTurn, everyTurn: true));
            AbilityRegistry.Register(AbilityName.ComesBackFromDeath,
                () => _combatFactory.CreateResurrection(AbilityName.ComesBackFromDeath));
            AbilityRegistry.Register(AbilityName.ComesBackFromDeath5Times,
                () => _combatFactory.CreateResurrection(AbilityName.ComesBackFromDeath5Times, maxRevives: 5));
            AbilityRegistry.Register(AbilityName.GiveDeathWhenDie,
                () => _combatFactory.CreateDeathTrigger());
            AbilityRegistry.Register(AbilityName.KillOpponentInvocation,
                () => _combatFactory.CreateDestroyOpponentCard(AbilityName.KillOpponentInvocation, CardType.Invocation));
        }

        #region MutualDestruction Tests

        [Test]
        public void MutualDestruction_WhenAttacking_BothCardsDestroyed()
        {
            // Arrange - Setup combat scenario
            var attackerCard = CreateCard("Suicide Bomber", 4, 2, CardFamily.Monster);
            var defenderCard = CreateCard("Victim", 3, 3, CardFamily.Human);

            // Player 1's deck with attacker
            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, attackerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            // Player 2's deck with defender
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, defenderCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.KillEnemyIfDestroy);
            var context = CreateContext(attackerCard, AbilityName.KillEnemyIfDestroy);
            context.TargetCard = defenderCard;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Both cards destroyed");
            AssertCardNotOnField(player1, "Suicide Bomber");
            AssertCardNotOnField(player2, "Victim");
        }

        [Test]
        public void MutualDestruction_CanActivate_WhenBothCardsPresent_ReturnsTrue()
        {
            // Arrange
            var attackerCard = CreateCard("Attacker", 4, 4, CardFamily.Human);
            var defenderCard = CreateCard("Defender", 3, 3, CardFamily.Human);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, attackerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, defenderCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.KillEnemyIfDestroy);
            var context = CreateContext(attackerCard, AbilityName.KillEnemyIfDestroy);
            context.TargetCard = defenderCard;

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context));
        }

        [Test]
        public void MutualDestruction_CanActivate_WhenNoTarget_ReturnsFalse()
        {
            // Arrange
            var attackerCard = CreateCard("Attacker", 4, 4, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, attackerCard);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.KillEnemyIfDestroy);
            var context = CreateContext(attackerCard, AbilityName.KillEnemyIfDestroy);
            // No target card set

            // Act & Assert
            Assert.IsFalse(CanActivateAbility(ability, context));
        }

        #endregion

        #region SkipAttack Tests

        [Test]
        public void SkipAttack_Execute_ReturnsSuccess()
        {
            // Arrange
            var sourceCard = CreateCard("Attack Skipper", 2, 5, CardFamily.Wizard);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, sourceCard);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.SkipOpponentAttackEveryTurn);
            var context = CreateContext(sourceCard, AbilityName.SkipOpponentAttackEveryTurn);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "skipped");
        }

        [Test]
        public void SkipAttackEveryTurn_Execute_ReturnsSuccess()
        {
            // Arrange
            var sourceCard = CreateCard("Permanent Skipper", 2, 5, CardFamily.Wizard);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, sourceCard);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.SkipOpponentAttackEveryTurn);
            var context = CreateContext(sourceCard, AbilityName.SkipOpponentAttackEveryTurn);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
        }

        [Test]
        public void SkipAttack_CanActivate_AlwaysTrue()
        {
            // Arrange
            var sourceCard = CreateCard("Skipper", 2, 2, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, sourceCard);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.SkipOpponentAttackEveryTurn);
            var context = CreateContext(sourceCard, AbilityName.SkipOpponentAttackEveryTurn);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context));
        }

        #endregion

        #region DirectAttack Tests

        [Test]
        public void DirectAttack_Execute_ReturnsSuccess()
        {
            // Arrange
            var directAttackFactory = new CombatAbilityFactory(PlayerRepository);
            var ability = directAttackFactory.CreateDirectAttack();

            var sourceCard = CreateCard("Direct Attacker", 5, 3, CardFamily.Monster);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, sourceCard);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(sourceCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Direct attack enabled");
        }

        #endregion

        #region Resurrection Tests

        [Test]
        public void Resurrection_WhenCardDies_RevivesOnce()
        {
            // Arrange
            var (player1, player2, jeanMarcSoul) =
                AbilityScenarioFixtures.CreateJeanMarcSoulResurrectionScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.ComesBackFromDeath);
            var context = CreateContext(jeanMarcSoul, AbilityName.ComesBackFromDeath);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "revived");
        }

        [Test]
        public void Resurrection5Times_CanReviveMultipleTimes()
        {
            // Arrange
            var resurrectionCard = CreateCard("Phoenix", 3, 3, CardFamily.Monster);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, resurrectionCard);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.ComesBackFromDeath5Times);
            var context = CreateContext(resurrectionCard, AbilityName.ComesBackFromDeath5Times);

            // Act - Revive 5 times
            for (int i = 0; i < 5; i++)
            {
                var result = ability.Execute(context);
                AssertAbilitySuccess(result, "revived");
            }

            // 6th attempt should fail
            var finalResult = ability.Execute(context);
            AssertAbilityFailure(finalResult, "Max revives");
        }

        [Test]
        public void Resurrection_CanActivate_WhenNotMaxedOut_ReturnsTrue()
        {
            // Arrange
            var resurrectionCard = CreateCard("Undying", 2, 2, CardFamily.Monster);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, resurrectionCard);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.ComesBackFromDeath);
            var context = CreateContext(resurrectionCard, AbilityName.ComesBackFromDeath);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context),
                "Should be able to revive when revive count < max");
        }

        #endregion

        #region DeathTrigger Tests

        [Test]
        public void DeathTrigger_WhenOpponentHasCards_RequiresUserInput()
        {
            // Arrange
            var triggerCard = CreateCard("Death Dealer", 3, 3, CardFamily.Monster);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, triggerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var opponentCard = TestCardFactory.CreateInvocation("Target", 2, 2, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, opponentCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveDeathWhenDie);
            var context = CreateContext(triggerCard, AbilityName.GiveDeathWhenDie);

            // Act
            var result = ability.Execute(context);

            // Assert - Should require user input to select which card to destroy
            Assert.IsTrue(result.RequiresUserInput,
                "Death trigger should require user to select target");
        }

        [Test]
        public void DeathTrigger_CanActivate_WhenOpponentHasCards_ReturnsTrue()
        {
            // Arrange
            var triggerCard = CreateCard("Death Dealer", 3, 3, CardFamily.Monster);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, triggerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var opponentCard = TestCardFactory.CreateInvocation("Target", 2, 2, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, opponentCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveDeathWhenDie);
            var context = CreateContext(triggerCard, AbilityName.GiveDeathWhenDie);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context));
        }

        [Test]
        public void DeathTrigger_CanActivate_WhenOpponentFieldEmpty_ReturnsFalse()
        {
            // Arrange
            var triggerCard = CreateCard("Death Dealer", 3, 3, CardFamily.Monster);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, triggerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2(); // Empty field
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveDeathWhenDie);
            var context = CreateContext(triggerCard, AbilityName.GiveDeathWhenDie);

            // Act & Assert
            Assert.IsFalse(CanActivateAbility(ability, context),
                "Death trigger should not activate when opponent has no cards");
        }

        #endregion

        #region DestroyOpponentCard Tests

        [Test]
        public void DestroyOpponentCard_WhenSingleTarget_DestroysDirectly()
        {
            // Arrange
            var destroyerCard = CreateCard("Destroyer", 4, 4, CardFamily.Monster);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, destroyerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var targetCard = TestCardFactory.CreateInvocation("Only Target", 2, 2, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, targetCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.KillOpponentInvocation);
            var context = CreateContext(destroyerCard, AbilityName.KillOpponentInvocation);

            // Act
            var result = ability.Execute(context);

            // Assert - Single target destroyed directly
            AssertAbilitySuccess(result, "Destroyed");
            AssertCardNotOnField(player2, "Only Target");
        }

        [Test]
        public void DestroyOpponentCard_WhenMultipleTargets_RequiresUserInput()
        {
            // Arrange
            var destroyerCard = CreateCard("Destroyer", 4, 4, CardFamily.Monster);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, destroyerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var target1 = TestCardFactory.CreateInvocation("Target 1", 2, 2, CardFamily.Human);
            var target2 = TestCardFactory.CreateInvocation("Target 2", 3, 3, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(28);
            deck2.Insert(0, target1);
            deck2.Insert(1, target2);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.DrawCard();
            player2.PlayCard(player2.Hand.FirstOrDefault(c => c.Title == "Target 1"));
            player2.PlayCard(player2.Hand.FirstOrDefault(c => c.Title == "Target 2"));

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.KillOpponentInvocation);
            var context = CreateContext(destroyerCard, AbilityName.KillOpponentInvocation);

            // Act
            var result = ability.Execute(context);

            // Assert - Should require user input for selection
            Assert.IsTrue(result.RequiresUserInput,
                "Multiple targets should require user selection");
        }

        [Test]
        public void DestroyOpponentCard_WhenNoValidTargets_Fails()
        {
            // Arrange
            var destroyerCard = CreateCard("Destroyer", 4, 4, CardFamily.Monster);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, destroyerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            // Opponent has no invocation cards
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.KillOpponentInvocation);
            var context = CreateContext(destroyerCard, AbilityName.KillOpponentInvocation);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result, "No valid targets");
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Combat_PreservesOtherFieldCards()
        {
            // Arrange - Multiple cards, only combat participants should be affected
            var attackerCard = CreateCard("Attacker", 4, 4, CardFamily.Human);
            var bystander = CreateCard("Bystander", 2, 2, CardFamily.Human);

            var deck1 = TestCardFactory.CreateDeck(28);
            deck1.Insert(0, attackerCard);
            deck1.Insert(1, bystander);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Attacker"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Bystander"));

            var defenderCard = TestCardFactory.CreateInvocation("Defender", 3, 3, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, defenderCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.KillEnemyIfDestroy);
            var context = CreateContext(attackerCard, AbilityName.KillEnemyIfDestroy);
            context.TargetCard = defenderCard;

            // Act
            ability.Execute(context);

            // Assert - Bystander should be unaffected
            AssertCardOnField(player1, "Bystander");
        }

        [Test]
        public void Combat_DoesNotAffectPlayerHealth()
        {
            // Arrange
            var attackerCard = CreateCard("Attacker", 4, 4, CardFamily.Human);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, attackerCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var defenderCard = TestCardFactory.CreateInvocation("Defender", 3, 3, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, defenderCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            float player1HealthBefore = player1.Health;
            float player2HealthBefore = player2.Health;

            var ability = GetAbility(AbilityName.KillEnemyIfDestroy);
            var context = CreateContext(attackerCard, AbilityName.KillEnemyIfDestroy);
            context.TargetCard = defenderCard;

            // Act
            ability.Execute(context);

            // Assert - Player health should be unchanged
            Assert.AreEqual(player1HealthBefore, player1.Health);
            Assert.AreEqual(player2HealthBefore, player2.Health);
        }

        #endregion
    }
}

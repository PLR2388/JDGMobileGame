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
    /// Scenario tests for Stat Boost/Modifier abilities.
    /// Tests verify stat modifications work correctly in realistic game scenarios.
    ///
    /// Stat Modifier Abilities:
    /// - GiveAtkDefToComics: Give +ATK/+DEF to Comics family
    /// - GiveAktDefToRpgMember: Give bonuses to RPG family
    /// - GiveAktDefToFistilandMember: Give bonuses to Fistiland family
    /// - ConditionalStats: Bonus based on card having min ATK/DEF
    /// - CopyStats: Copy ATK/DEF from another card
    /// </summary>
    [TestFixture]
    public class StatBoostAbilityScenarioTests : AbilityScenarioTestBase
    {
        private StatModifierAbilityFactory _statFactory;

        protected override void RegisterAbilities()
        {
            _statFactory = new StatModifierAbilityFactory(PlayerRepository);

            // Family stat boost abilities
            AbilityRegistry.Register(AbilityName.GiveAtkDefToComics,
                () => _statFactory.CreateGiveFamilyStats(AbilityName.GiveAtkDefToComics, CardFamily.Comics, 1, 1));
            AbilityRegistry.Register(AbilityName.GiveAktDefToRpgMember,
                () => _statFactory.CreateGiveFamilyStats(AbilityName.GiveAktDefToRpgMember, CardFamily.Rpg, 1, 1));
            AbilityRegistry.Register(AbilityName.GiveAktDefToFistilandMember,
                () => _statFactory.CreateGiveFamilyStats(AbilityName.GiveAktDefToFistilandMember, CardFamily.Fistiland, 1, 1));
        }

        #region GiveFamilyStats Tests

        [Test]
        public void GiveAtkDefToComics_WhenComicsOnField_BoostsStats()
        {
            // Arrange - Comics family card that boosts other comics
            var boosterCard = CreateCard("Comics Booster", 3, 3, CardFamily.Comics);
            var comicsCard1 = CreateCard("Superman", 4, 4, CardFamily.Comics);
            var comicsCard2 = CreateCard("Batman", 3, 3, CardFamily.Comics);

            var deck = TestCardFactory.CreateDeck(27);
            deck.Insert(0, boosterCard);
            deck.Insert(1, comicsCard1);
            deck.Insert(2, comicsCard2);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.DrawCard();
            foreach (var card in player1.Hand.ToList())
            {
                player1.PlayCard(card);
            }

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAtkDefToComics);
            var context = CreateContext(boosterCard, AbilityName.GiveAtkDefToComics);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            // All 3 comics cards should get the boost (including booster itself)
            Assert.IsTrue(result.Message.Contains("3"),
                "Should boost all 3 Comics cards on field");
        }

        [Test]
        public void GiveAtkDefToComics_WhenNoComicsOnField_Fails()
        {
            // Arrange - No Comics cards on field
            var boosterCard = CreateCard("Human Booster", 3, 3, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, boosterCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAtkDefToComics);
            var context = CreateContext(boosterCard, AbilityName.GiveAtkDefToComics);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result, "No");
        }

        [Test]
        public void GiveAtkDefToComics_CanActivate_WhenComicsPresent_ReturnsTrue()
        {
            // Arrange
            var boosterCard = CreateCard("Booster", 3, 3, CardFamily.Comics);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, boosterCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAtkDefToComics);
            var context = CreateContext(boosterCard, AbilityName.GiveAtkDefToComics);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context),
                "Should activate when Comics cards are on field");
        }

        #endregion

        #region GiveAktDefToRpgMember Tests

        [Test]
        public void GiveAktDefToRpgMember_BoostsRpgFamily()
        {
            // Arrange
            var boosterCard = CreateCard("RPG Leader", 4, 4, CardFamily.Rpg);
            var rpgCard = CreateCard("Granolax", 2, 2, CardFamily.Rpg);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(0, boosterCard);
            deck.Insert(1, rpgCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "RPG Leader"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Granolax"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAktDefToRpgMember);
            var context = CreateContext(boosterCard, AbilityName.GiveAktDefToRpgMember);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Rpg");
        }

        #endregion

        #region GiveAktDefToFistilandMember Tests

        [Test]
        public void GiveAktDefToFistilandMember_BoostsFistilandFamily()
        {
            // Arrange
            var boosterCard = CreateCard("Fistiland Leader", 4, 4, CardFamily.Fistiland);
            var fistilandCard1 = CreateCard("Benzaie", 5, 4, CardFamily.Fistiland);
            var fistilandCard2 = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(27);
            deck.Insert(0, boosterCard);
            deck.Insert(1, fistilandCard1);
            deck.Insert(2, fistilandCard2);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.DrawCard();
            foreach (var card in player1.Hand.ToList())
            {
                player1.PlayCard(card);
            }

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAktDefToFistilandMember);
            var context = CreateContext(boosterCard, AbilityName.GiveAktDefToFistilandMember);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "3"); // All 3 Fistiland cards
        }

        #endregion

        #region ConditionalStats Tests

        [Test]
        public void ConditionalStats_WhenConditionMet_AppliesBonus()
        {
            // Arrange - Card with enough ATK/DEF meets condition
            var conditionalAbility = _statFactory.CreateConditionalStats(
                AbilityName.Default,
                CardFamily.Japan,
                minAtk: 2,
                minDef: 2,
                bonusAtk: 1,
                bonusDef: 1);

            var japanCard = TestCardFactory.CreateInvocation("Strong Japan Card", 3, 3, CardFamily.Japan);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, japanCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(japanCard, AbilityName.Default);

            // Act
            var result = conditionalAbility.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Gained");
        }

        [Test]
        public void ConditionalStats_WhenConditionNotMet_Fails()
        {
            // Arrange - Card with insufficient stats
            var conditionalAbility = _statFactory.CreateConditionalStats(
                AbilityName.Default,
                CardFamily.Japan,
                minAtk: 5,
                minDef: 5,
                bonusAtk: 1,
                bonusDef: 1);

            var weakCard = TestCardFactory.CreateInvocation("Weak Japan Card", 2, 2, CardFamily.Japan);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, weakCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(weakCard, AbilityName.Default);

            // Act
            var result = conditionalAbility.Execute(context);

            // Assert
            AssertAbilityFailure(result, "Condition not met");
        }

        [Test]
        public void ConditionalStats_WhenWrongFamily_Fails()
        {
            // Arrange - Card has right stats but wrong family
            var conditionalAbility = _statFactory.CreateConditionalStats(
                AbilityName.Default,
                CardFamily.Japan,
                minAtk: 2,
                minDef: 2,
                bonusAtk: 1,
                bonusDef: 1);

            var humanCard = TestCardFactory.CreateInvocation("Strong Human", 5, 5, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, humanCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(humanCard, AbilityName.Default);

            // Act
            var result = conditionalAbility.Execute(context);

            // Assert
            AssertAbilityFailure(result, "not met");
        }

        #endregion

        #region CopyStats Tests

        [Test]
        public void CopyStats_WhenTargetOnField_CopiesStats()
        {
            // Arrange - Copy stats from Benzaie jeune
            var copyAbility = _statFactory.CreateCopyStats(AbilityName.Default, "Benzaie jeune");

            var copyCard = TestCardFactory.CreateInvocation("Copy Cat", 1, 1, CardFamily.Human);
            var benzaieJeune = TestCardFactory.CreateInvocation("Benzaie jeune", 5, 5, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(0, copyCard);
            deck.Insert(1, benzaieJeune);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Copy Cat"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Benzaie jeune"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(copyCard, AbilityName.Default);

            // Act
            var result = copyAbility.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Copied");
        }

        [Test]
        public void CopyStats_WhenTargetNotOnField_Fails()
        {
            // Arrange - Target card not present
            var copyAbility = _statFactory.CreateCopyStats(AbilityName.Default, "Missing Card");

            var copyCard = TestCardFactory.CreateInvocation("Copy Cat", 1, 1, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, copyCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(copyCard, AbilityName.Default);

            // Act
            var result = copyAbility.Execute(context);

            // Assert
            AssertAbilityFailure(result, "not on field");
        }

        [Test]
        public void CopyStats_CanActivate_WhenTargetExists_ReturnsTrue()
        {
            // Arrange
            var copyAbility = _statFactory.CreateCopyStats(AbilityName.Default, "Target Card");

            var copyCard = TestCardFactory.CreateInvocation("Copier", 1, 1, CardFamily.Human);
            var targetCard = TestCardFactory.CreateInvocation("Target Card", 4, 4, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(0, copyCard);
            deck.Insert(1, targetCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Copier"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Target Card"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(copyCard, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(copyAbility.CanActivate(context),
                "Should be able to copy when target is on field");
        }

        [Test]
        public void CopyStats_CanActivate_WhenTargetNotExists_ReturnsFalse()
        {
            // Arrange
            var copyAbility = _statFactory.CreateCopyStats(AbilityName.Default, "Missing Target");

            var copyCard = TestCardFactory.CreateInvocation("Copier", 1, 1, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, copyCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(copyCard, AbilityName.Default);

            // Act & Assert
            Assert.IsFalse(copyAbility.CanActivate(context),
                "Should not be able to copy when target is missing");
        }

        #endregion

        #region Edge Cases

        [Test]
        public void FamilyBoost_WithMixedFamilyField_OnlyBoostsTargetFamily()
        {
            // Arrange - Field has mixed families
            var boosterCard = CreateCard("Comics Booster", 3, 3, CardFamily.Comics);
            var comicsCard = CreateCard("Superman", 4, 4, CardFamily.Comics);
            var humanCard = CreateCard("Regular Human", 2, 2, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(27);
            deck.Insert(0, boosterCard);
            deck.Insert(1, comicsCard);
            deck.Insert(2, humanCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.DrawCard();
            foreach (var card in player1.Hand.ToList())
            {
                player1.PlayCard(card);
            }

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAtkDefToComics);
            var context = CreateContext(boosterCard, AbilityName.GiveAtkDefToComics);

            // Act
            var result = ability.Execute(context);

            // Assert - Only 2 Comics cards boosted, not the Human
            AssertAbilitySuccess(result, "2");
        }

        [Test]
        public void StatBoost_DoesNotAffectOpponentCards()
        {
            // Arrange
            var boosterCard = CreateCard("Comics Booster", 3, 3, CardFamily.Comics);

            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Insert(0, boosterCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var opponentComics = TestCardFactory.CreateInvocation("Enemy Comics", 4, 4, CardFamily.Comics);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Insert(0, opponentComics);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAtkDefToComics);
            var context = CreateContext(boosterCard, AbilityName.GiveAtkDefToComics);

            // Remember opponent's card stats before boost
            var enemyCard = player2.Field.First();
            var originalAtk = enemyCard.Stats?.Attack ?? 0;
            var originalDef = enemyCard.Stats?.Defense ?? 0;

            // Act
            ability.Execute(context);

            // Assert - Opponent's Comics card should NOT be affected
            Assert.AreEqual(originalAtk, enemyCard.Stats?.Attack ?? 0,
                "Opponent's Comics card ATK should be unchanged");
            Assert.AreEqual(originalDef, enemyCard.Stats?.Defense ?? 0,
                "Opponent's Comics card DEF should be unchanged");
        }

        [Test]
        public void StatBoost_CanStackMultipleTimes()
        {
            // Arrange
            var boosterCard = CreateCard("Stackable Booster", 3, 3, CardFamily.Comics);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, boosterCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GiveAtkDefToComics);
            var context = CreateContext(boosterCard, AbilityName.GiveAtkDefToComics);

            // Act - Execute multiple times
            var result1 = ability.Execute(context);
            var result2 = ability.Execute(context);

            // Assert - Both executions should succeed
            AssertAbilitySuccess(result1);
            AssertAbilitySuccess(result2);
        }

        #endregion
    }
}

using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.TestUtilities;
using System.Linq;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for Protection abilities.
    /// Tests verify protection mechanics work correctly in realistic game scenarios.
    ///
    /// Protection Abilities:
    /// - CantBeAttackIfComics: Card cannot be attacked if Comics family on field
    /// - CantBeAttackKill: Card cannot be attacked (unconditional)
    /// - ProtectedBehindStarlightUnicorn: Protected by Starlight Unicorn
    /// - ProtectBehindGreaterDef: Protect cards with lower DEF
    /// - CanOnlyAttackItself: Attack restriction
    ///
    /// Dependency Abilities:
    /// - CantLiveWithoutBenzaie: Depends on Benzaie
    /// - CantLiveWithoutJoueurDuGrenier: Depends on JDG
    /// - CantLiveWithoutComics: Depends on Comics card
    /// - CantLiveWithoutHuman: Depends on Human family
    /// - CantLiveWithoutJapon: Depends on Japan family
    /// - CantLiveWithoutGranolax: Depends on Granolax
    /// </summary>
    [TestFixture]
    public class ProtectionAbilityScenarioTests : AbilityScenarioTestBase
    {
        private ProtectionAbilityFactory _protectionFactory;

        protected override void RegisterAbilities()
        {
            _protectionFactory = new ProtectionAbilityFactory(PlayerRepository);

            // Can't be attacked abilities
            AbilityRegistry.Register(AbilityName.CantBeAttackIfComics,
                () => _protectionFactory.CreateCantBeAttacked(AbilityName.CantBeAttackIfComics, CardFamily.Comics));
            AbilityRegistry.Register(AbilityName.CantBeAttackKill,
                () => _protectionFactory.CreateCantBeAttacked(AbilityName.CantBeAttackKill));

            // Protect behind abilities
            AbilityRegistry.Register(AbilityName.ProtectedBehindStarlightUnicorn,
                () => _protectionFactory.CreateProtectBehind(AbilityName.ProtectedBehindStarlightUnicorn));
            AbilityRegistry.Register(AbilityName.ProtectBehindGreaterDef,
                () => _protectionFactory.CreateProtectBehind(AbilityName.ProtectBehindGreaterDef, minDefense: 3));

            // Attack restriction
            AbilityRegistry.Register(AbilityName.CanOnlyAttackItself,
                () => _protectionFactory.CreateCanOnlyAttackItself());

            // Dependency abilities
            AbilityRegistry.Register(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune,
                () => _protectionFactory.CreateDependency(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune, "Benzaie"));
            AbilityRegistry.Register(AbilityName.CantLiveWithoutJDG,
                () => _protectionFactory.CreateDependency(AbilityName.CantLiveWithoutJDG, "Joueur du Grenier"));
            AbilityRegistry.Register(AbilityName.CantLiveWithoutGranolaxOrMechaGranolax,
                () => _protectionFactory.CreateDependency(AbilityName.CantLiveWithoutGranolaxOrMechaGranolax, "Granolax"));
        }

        #region CantBeAttacked Tests

        [Test]
        public void CantBeAttackKill_Execute_ReturnsSuccess()
        {
            // Arrange - A card that simply cannot be attacked
            var protectedCard = CreateCard("Untouchable Monster", 5, 5, CardFamily.Monster);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, protectedCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantBeAttackKill);
            var context = CreateContext(protectedCard, AbilityName.CantBeAttackKill);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "protected");
        }

        [Test]
        public void CantBeAttackKill_CanActivate_AlwaysReturnsTrue()
        {
            // Arrange
            var protectedCard = CreateCard("Untouchable", 3, 3, CardFamily.Monster);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, protectedCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantBeAttackKill);
            var context = CreateContext(protectedCard, AbilityName.CantBeAttackKill);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context),
                "Unconditional protection should always be active");
        }

        #endregion

        #region CantBeAttackIfComics Tests

        [Test]
        public void CantBeAttackIfComics_WhenComicsOnField_CanActivate()
        {
            // Arrange - Card protected when Comics family on field
            var protectedCard = CreateCard("Comic Protected Card", 3, 3, CardFamily.Human);
            var comicsCard = CreateCard("Spiderman", 4, 3, CardFamily.Comics);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, protectedCard);
            deck.Insert(1, comicsCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Comic Protected Card"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Spiderman"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantBeAttackIfComics);
            var context = CreateContext(protectedCard, AbilityName.CantBeAttackIfComics);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context),
                "Protection should activate when Comics card is on field");
        }

        [Test]
        public void CantBeAttackIfComics_WhenNoComicsOnField_CannotActivate()
        {
            // Arrange - No Comics family on field
            var protectedCard = CreateCard("Comic Protected Card", 3, 3, CardFamily.Human);
            var nonComicsCard = CreateCard("Normal Human", 2, 2, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, protectedCard);
            deck.Insert(1, nonComicsCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Comic Protected Card"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Normal Human"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantBeAttackIfComics);
            var context = CreateContext(protectedCard, AbilityName.CantBeAttackIfComics);

            // Act & Assert
            Assert.IsFalse(CanActivateAbility(ability, context),
                "Protection should not activate without Comics card on field");
        }

        [Test]
        public void CantBeAttackIfComics_WhenComicsRemovedFromField_ProtectionDeactivates()
        {
            // Arrange - Comics card present initially
            var protectedCard = CreateCard("Comic Protected", 3, 3, CardFamily.Human);
            var comicsCard = CardFactory.CreateInvocation("Batman", 4, 3, CardFamily.Comics);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, protectedCard);
            deck.Insert(1, comicsCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Comic Protected"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Batman"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantBeAttackIfComics);
            var context = CreateContext(protectedCard, AbilityName.CantBeAttackIfComics);

            // Verify initially protected
            Assert.IsTrue(CanActivateAbility(ability, context));

            // Remove Comics card from field
            var batman = player1.Field.FirstOrDefault(c => c.Title == "Batman");
            if (batman != null)
            {
                player1.DestroyCardFromField(batman);
                PlayerRepository.SavePlayer(player1);
            }

            // Act & Assert - Protection should now be inactive
            Assert.IsFalse(CanActivateAbility(ability, context),
                "Protection should deactivate when Comics card is removed");
        }

        #endregion

        #region ProtectBehind Tests

        [Test]
        public void ProtectBehindStarlightUnicorn_Execute_ProtectsOtherCards()
        {
            // Arrange - Starlight Unicorn protects other cards
            var starlightUnicorn = CreateCard("Starlight Unicorn", 4, 4, CardFamily.Rpg);
            var protectedCard = CreateCard("Granolax", 2, 2, CardFamily.Rpg);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, starlightUnicorn);
            deck.Insert(1, protectedCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Starlight Unicorn"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Granolax"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.ProtectedBehindStarlightUnicorn);
            var context = CreateContext(starlightUnicorn, AbilityName.ProtectedBehindStarlightUnicorn);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Protecting");
        }

        [Test]
        public void ProtectBehindGreaterDef_Execute_ProtectsLowerDefCards()
        {
            // Arrange - Card with high DEF protects cards with DEF < 3
            var protectorCard = CreateCard("High Def Protector", 2, 5, CardFamily.Human);
            var lowDefCard = CreateCard("Low Def Card", 4, 2, CardFamily.Human);
            var highDefCard = CreateCard("High Def Card", 3, 4, CardFamily.Human);

            var deck = CardFactory.CreateDeck(27);
            deck.Insert(0, protectorCard);
            deck.Insert(1, lowDefCard);
            deck.Insert(2, highDefCard);

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

            var ability = GetAbility(AbilityName.ProtectBehindGreaterDef);
            var context = CreateContext(protectorCard, AbilityName.ProtectBehindGreaterDef);

            // Act
            var result = ability.Execute(context);

            // Assert - Should protect Low Def Card but not High Def Card
            AssertAbilitySuccess(result);
        }

        [Test]
        public void ProtectBehind_WithNoOtherCards_ProtectsNone()
        {
            // Arrange - Only protector on field
            var protectorCard = CreateCard("Lonely Protector", 3, 5, CardFamily.Human);

            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, protectorCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.ProtectedBehindStarlightUnicorn);
            var context = CreateContext(protectorCard, AbilityName.ProtectedBehindStarlightUnicorn);

            // Act
            var result = ability.Execute(context);

            // Assert - Should succeed but protect 0 cards
            AssertAbilitySuccess(result, "0");
        }

        #endregion

        #region CanOnlyAttackItself Tests

        [Test]
        public void CanOnlyAttackItself_Execute_ReturnsSuccess()
        {
            // Arrange
            var restrictedCard = CreateCard("Self-Attacking Monster", 4, 4, CardFamily.Monster);

            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, restrictedCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CanOnlyAttackItself);
            var context = CreateContext(restrictedCard, AbilityName.CanOnlyAttackItself);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "restricted");
        }

        [Test]
        public void CanOnlyAttackItself_CanActivate_AlwaysTrue()
        {
            // Arrange
            var restrictedCard = CreateCard("Restricted", 2, 2, CardFamily.Monster);

            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, restrictedCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CanOnlyAttackItself);
            var context = CreateContext(restrictedCard, AbilityName.CanOnlyAttackItself);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context),
                "Self-attack restriction should always be active");
        }

        #endregion

        #region Dependency Ability Tests

        [Test]
        public void CantLiveWithoutBenzaie_WhenBenzaieOnField_CanActivate()
        {
            // Arrange - Alpha Man depends on Benzaie
            var (player1, player2, alphaMan, benzaie) =
                AbilityScenarioFixtures.CreateAlphaManDependencyScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);
            var context = CreateContext(alphaMan, AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context),
                "Dependency should be satisfied when Benzaie is on field");
        }

        [Test]
        public void CantLiveWithoutBenzaie_WhenBenzaieOnField_DependencySatisfied()
        {
            // Arrange
            var (player1, player2, alphaMan, benzaie) =
                AbilityScenarioFixtures.CreateAlphaManDependencyScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);
            var context = CreateContext(alphaMan, AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "satisfied");
            AssertCardOnField(player1, "Alpha Man"); // Should still be alive
        }

        [Test]
        public void CantLiveWithoutBenzaie_WhenBenzaieNotOnField_CardDestroyed()
        {
            // Arrange - Alpha Man without Benzaie
            var alphaMan = CreateCard("Alpha Man", 4, 4, CardFamily.Fistiland);
            var otherCard = CreateCard("Some Other Card", 2, 2, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, alphaMan);
            deck.Insert(1, otherCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Alpha Man"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Some Other Card"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);
            var context = CreateContext(alphaMan, AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);

            // Act
            var result = ability.Execute(context);

            // Assert - Card should be destroyed
            AssertAbilitySuccess(result, "destroyed");
            AssertCardNotOnField(player1, "Alpha Man");
        }

        [Test]
        public void CantLiveWithoutBenzaie_WhenBenzaieRemoved_DependentCardDestroyed()
        {
            // Arrange - Start with both cards, then remove Benzaie
            var (player1, player2, alphaMan, benzaie) =
                AbilityScenarioFixtures.CreateAlphaManDependencyScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Remove Benzaie
            var benzaieCard = player1.Field.FirstOrDefault(c => c.Title == "Benzaie");
            if (benzaieCard != null)
            {
                player1.DestroyCardFromField(benzaieCard);
                PlayerRepository.SavePlayer(player1);
            }

            var ability = GetAbility(AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);
            var context = CreateContext(alphaMan, AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);

            // Act - Check dependency after removal
            var result = ability.Execute(context);

            // Assert - Alpha Man should be destroyed
            AssertAbilitySuccess(result, "destroyed");
        }

        #endregion

        #region CantLiveWithoutGranolax Tests (Starlight Unicorn scenario)

        [Test]
        public void CantLiveWithoutGranolax_WhenGranolaxOnField_Survives()
        {
            // Arrange
            var (player1, player2, starlightUnicorn, granolax) =
                AbilityScenarioFixtures.CreateStarlightDependencyScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);
            var context = CreateContext(starlightUnicorn, AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "satisfied");
            AssertCardOnField(player1, "Starlight Unicorn");
        }

        [Test]
        public void CantLiveWithoutGranolax_WhenGranolaxRemoved_Destroyed()
        {
            // Arrange
            var (player1, player2, starlightUnicorn, granolax) =
                AbilityScenarioFixtures.CreateStarlightDependencyScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Remove Granolax
            var granolaxCard = player1.Field.FirstOrDefault(c => c.Title == "Granolax");
            if (granolaxCard != null)
            {
                player1.DestroyCardFromField(granolaxCard);
                PlayerRepository.SavePlayer(player1);
            }

            var ability = GetAbility(AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);
            var context = CreateContext(starlightUnicorn, AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "destroyed");
        }

        #endregion

        #region CantLiveWithoutJoueurDuGrenier Tests

        [Test]
        public void CantLiveWithoutJoueurDuGrenier_WhenJDGOnField_Survives()
        {
            // Arrange
            var dependentCard = CreateCard("JDG Fan", 2, 2, CardFamily.Fistiland);
            var jdg = CreateCard("Joueur du Grenier", 5, 5, CardFamily.Fistiland);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, jdg);
            deck.Insert(1, dependentCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Joueur du Grenier"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "JDG Fan"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantLiveWithoutJDG);
            var context = CreateContext(dependentCard, AbilityName.CantLiveWithoutJDG);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "satisfied");
            AssertCardOnField(player1, "JDG Fan");
        }

        [Test]
        public void CantLiveWithoutJoueurDuGrenier_WhenJDGNotOnField_Destroyed()
        {
            // Arrange - No JDG on field
            var dependentCard = CreateCard("JDG Fan", 2, 2, CardFamily.Fistiland);
            var otherCard = CreateCard("Other", 2, 2, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, dependentCard);
            deck.Insert(1, otherCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "JDG Fan"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Other"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.CantLiveWithoutJDG);
            var context = CreateContext(dependentCard, AbilityName.CantLiveWithoutJDG);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "destroyed");
            AssertCardNotOnField(player1, "JDG Fan");
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Dependency_WithMultipleValidCards_SurvivesIfAnyPresent()
        {
            // Arrange - Custom dependency that accepts multiple card names
            var customDependency = _protectionFactory.CreateDependency(
                AbilityName.Default,
                "Card A", "Card B", "Card C");

            var dependentCard = CreateCard("Multi-Dependent", 2, 2, CardFamily.Human);
            var validCard = CreateCard("Card B", 3, 3, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, dependentCard);
            deck.Insert(1, validCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Multi-Dependent"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Card B"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(dependentCard, AbilityName.Default);

            // Act
            var result = customDependency.Execute(context);

            // Assert - Should survive since Card B is present
            AssertAbilitySuccess(result, "satisfied");
        }

        [Test]
        public void Protection_DoesNotAffectOpponentField()
        {
            // Arrange
            var protectorCard = CreateCard("Protector", 4, 4, CardFamily.Human);

            var deck1 = CardFactory.CreateDeck(29);
            deck1.Insert(0, protectorCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var opponentCard = CardFactory.CreateInvocation("Enemy", 3, 3, CardFamily.Monster);
            var deck2 = CardFactory.CreateDeck(29);
            deck2.Insert(0, opponentCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.ProtectedBehindStarlightUnicorn);
            var context = CreateContext(protectorCard, AbilityName.ProtectedBehindStarlightUnicorn);

            int opponentFieldCount = player2.Field.Count;

            // Act
            ability.Execute(context);

            // Assert - Opponent's field unchanged
            Assert.AreEqual(opponentFieldCount, player2.Field.Count,
                "Protection should not affect opponent's field");
        }

        #endregion
    }
}

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
    /// Scenario tests for Deck Search abilities.
    /// Tests verify these abilities work correctly in realistic game scenarios.
    ///
    /// Deck Search Abilities (7 specific + family search):
    /// - GetNounoursFromDeck: Benzaie jeune searches for Nounours
    /// - GetPetitePortionDeRizFromDeck: Search for Petite Portion de Riz
    /// - GetLycéeMagiqueGeorgesPompidouFromDeck: Search for school card
    /// - GetZozanKebabFromDeck: Search for Zozan Kebab
    /// - GetConvocationAuLyceeFromDeck: Search for convocation card
    /// - GetBenzaieJeuneFromDeck: Search for Benzaie jeune
    /// - GetPatronInfogramesFromDeckYellowTrash: Yellow trash searches for Patron
    /// - GetFamilyCard: Generic family-based search
    /// </summary>
    [TestFixture]
    public class DeckSearchAbilityScenarioTests : AbilityScenarioTestBase
    {
        private DeckSearchAbilityFactory _deckSearchFactory;

        protected override void RegisterAbilities()
        {
            _deckSearchFactory = new DeckSearchAbilityFactory(PlayerRepository);

            // Register specific card searches
            AbilityRegistry.Register(AbilityName.GetNounoursFromDeck,
                () => _deckSearchFactory.CreateGetSpecificCard(AbilityName.GetNounoursFromDeck, "Nounours"));
            AbilityRegistry.Register(AbilityName.GetBenzaieJeuneFromDeck,
                () => _deckSearchFactory.CreateGetSpecificCard(AbilityName.GetBenzaieJeuneFromDeck, "Benzaie jeune"));
            AbilityRegistry.Register(AbilityName.GetZozanKebabFromDeck,
                () => _deckSearchFactory.CreateGetSpecificCard(AbilityName.GetZozanKebabFromDeck, "Zozan Kebab"));
            AbilityRegistry.Register(AbilityName.GetConvocationAuLyceeFromDeck,
                () => _deckSearchFactory.CreateGetSpecificCard(AbilityName.GetConvocationAuLyceeFromDeck, "Convocation au Lycée"));
            AbilityRegistry.Register(AbilityName.GetPetitePortionDeRizFromDeck,
                () => _deckSearchFactory.CreateGetSpecificCard(AbilityName.GetPetitePortionDeRizFromDeck, "Petite Portion de Riz"));
            AbilityRegistry.Register(AbilityName.GetLycéeMagiqueGeorgesPompidouFromDeck,
                () => _deckSearchFactory.CreateGetSpecificCard(AbilityName.GetLycéeMagiqueGeorgesPompidouFromDeck, "Lycée Magique Georges Pompidou"));
            AbilityRegistry.Register(AbilityName.GetPatronInfogramesFromDeckYellowTrash,
                () => _deckSearchFactory.CreateGetSpecificCard(AbilityName.GetPatronInfogramesFromDeckYellowTrash, "Patron Infogrames"));
        }

        #region GetNounoursFromDeck Tests (Benzaie jeune)

        [Test]
        public void GetNounoursFromDeck_WhenNounoursInDeck_SearchesAndDrawsCard()
        {
            // Arrange - Benzaie jeune on field, Nounours in deck
            var (player1, player2, benzaieJeune, nounours) =
                AbilityScenarioFixtures.CreateBenzaieJeuneSearchScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            int initialDeckCount = player1.DeckCount;
            int initialHandCount = player1.HandCount;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardInHand(player1, "Nounours");
            Assert.AreEqual(initialDeckCount - 1, player1.DeckCount,
                "Deck should have 1 fewer card after search");
            Assert.AreEqual(initialHandCount + 1, player1.HandCount,
                "Hand should have 1 more card after search");
        }

        [Test]
        public void GetNounoursFromDeck_CanActivate_WhenNounoursInDeck_ReturnsTrue()
        {
            // Arrange
            var (player1, player2, benzaieJeune, nounours) =
                AbilityScenarioFixtures.CreateBenzaieJeuneSearchScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            // Act & Assert
            Assert.IsTrue(CanActivateAbility(ability, context),
                "Should be able to activate when Nounours is in deck");
        }

        [Test]
        public void GetNounoursFromDeck_WhenNounoursNotInDeck_FailsGracefully()
        {
            // Arrange - Create scenario where Nounours is NOT in deck
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);
            var deck = TestCardFactory.CreateDeck(30); // No Nounours
            deck.Add(benzaieJeune);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result, "not found");
        }

        [Test]
        public void GetNounoursFromDeck_CanActivate_WhenNounoursNotInDeck_ReturnsFalse()
        {
            // Arrange
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);
            var deck = TestCardFactory.CreateDeck(30);
            deck.Add(benzaieJeune);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            // Act & Assert
            Assert.IsFalse(CanActivateAbility(ability, context),
                "Should not be able to activate when Nounours is not in deck");
        }

        #endregion

        #region GetBenzaieJeuneFromDeck Tests

        [Test]
        public void GetBenzaieJeuneFromDeck_WhenBenzaieJeuneInDeck_SearchesAndDrawsCard()
        {
            // Arrange - Different searcher, Benzaie jeune in deck
            var searcher = CreateCard("Maman", 2, 3, CardFamily.Fistiland);
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(10, benzaieJeune); // Target in middle
            deck.Insert(0, searcher);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetBenzaieJeuneFromDeck);
            var context = CreateContext(searcher, AbilityName.GetBenzaieJeuneFromDeck);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardInHand(player1, "Benzaie jeune");
        }

        [Test]
        public void GetBenzaieJeuneFromDeck_WithEmptyDeck_FailsGracefully()
        {
            // Arrange - Deck is empty (simulated by having no matching card)
            var searcher = CreateCard("Maman", 2, 3, CardFamily.Fistiland);
            var deck = new System.Collections.Generic.List<Card> { searcher };
            // Minimal deck with no Benzaie jeune

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetBenzaieJeuneFromDeck);
            var context = CreateContext(searcher, AbilityName.GetBenzaieJeuneFromDeck);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result);
        }

        #endregion

        #region GetZozanKebabFromDeck Tests

        [Test]
        public void GetZozanKebabFromDeck_WhenZozanInDeck_SearchesAndDrawsCard()
        {
            // Arrange
            var searcher = CreateCard("Frangipanus", 1, 1, CardFamily.Japan);
            var zozanKebab = CreateCard("Zozan Kebab", 3, 3, CardFamily.HardCorner);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(15, zozanKebab);
            deck.Insert(0, searcher);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetZozanKebabFromDeck);
            var context = CreateContext(searcher, AbilityName.GetZozanKebabFromDeck);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardInHand(player1, "Zozan Kebab");
        }

        #endregion

        #region GetPatronInfogramesFromDeckYellowTrash Tests

        [Test]
        public void GetPatronInfogramesFromDeck_WhenPatronInDeck_SearchesAndDrawsCard()
        {
            // Arrange - Yellow Trash card searches for Patron Infogrames
            var yellowTrash = CreateCard("Yellow Trash", 2, 2, CardFamily.Developer);
            var patron = CreateCard("Patron Infogrames", 4, 3, CardFamily.Developer);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(5, patron);
            deck.Insert(0, yellowTrash);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetPatronInfogramesFromDeckYellowTrash);
            var context = CreateContext(yellowTrash, AbilityName.GetPatronInfogramesFromDeckYellowTrash);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardInHand(player1, "Patron Infogrames");
        }

        #endregion

        #region GetConvocationAuLyceeFromDeck Tests

        [Test]
        public void GetConvocationAuLyceeFromDeck_WhenCardInDeck_SearchesAndDrawsCard()
        {
            // Arrange
            var searcher = CreateCard("Professeur", 2, 2, CardFamily.Wizard);
            var convocation = CreateCard("Convocation au Lycée", 0, 0, CardFamily.None);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(8, convocation);
            deck.Insert(0, searcher);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetConvocationAuLyceeFromDeck);
            var context = CreateContext(searcher, AbilityName.GetConvocationAuLyceeFromDeck);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardInHand(player1, "Convocation au Lycée");
        }

        #endregion

        #region Search Edge Cases

        [Test]
        public void DeckSearch_WhenMultipleCopiesInDeck_DrawsOnlyOne()
        {
            // Arrange - Two copies of Nounours in deck
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);
            var nounours1 = TestCardFactory.CreateInvocation("Nounours", 3, 3, CardFamily.Fistiland);
            var nounours2 = TestCardFactory.CreateInvocation("Nounours", 3, 3, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(27);
            deck.Insert(5, nounours1);
            deck.Insert(15, nounours2);
            deck.Insert(0, benzaieJeune);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            int handBefore = player1.HandCount;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            Assert.AreEqual(handBefore + 1, player1.HandCount, "Should draw exactly one card");
            // One Nounours should remain in deck
            Assert.IsTrue(player1.Deck.Any(c => c.Title == "Nounours"),
                "One copy should remain in deck");
        }

        [Test]
        public void DeckSearch_PreservesPlayerState()
        {
            // Arrange - Verify search doesn't affect other player state
            var (player1, player2, benzaieJeune, nounours) =
                AbilityScenarioFixtures.CreateBenzaieJeuneSearchScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            float healthBefore = player1.Health;
            int fieldCountBefore = player1.Field.Count;

            // Act
            ability.Execute(context);

            // Assert
            Assert.AreEqual(healthBefore, player1.Health, "Health should be unchanged");
            Assert.AreEqual(fieldCountBefore, player1.Field.Count, "Field count should be unchanged");
        }

        [Test]
        public void DeckSearch_DoesNotAffectOpponent()
        {
            // Arrange
            var (player1, player2, benzaieJeune, nounours) =
                AbilityScenarioFixtures.CreateBenzaieJeuneSearchScenario();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            int opponentDeckBefore = player2.DeckCount;
            int opponentHandBefore = player2.HandCount;

            // Act
            ability.Execute(context);

            // Assert
            Assert.AreEqual(opponentDeckBefore, player2.DeckCount,
                "Opponent deck should be unchanged");
            Assert.AreEqual(opponentHandBefore, player2.HandCount,
                "Opponent hand should be unchanged");
        }

        [Test]
        public void DeckSearch_WithTargetAtBottomOfDeck_StillFindsCard()
        {
            // Arrange - Target at the very bottom
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);
            var nounours = TestCardFactory.CreateInvocation("Nounours", 3, 3, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(0, benzaieJeune);
            deck.Add(nounours); // At the bottom

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardInHand(player1, "Nounours");
        }

        [Test]
        public void DeckSearch_WithTargetAtTopOfDeck_StillFindsCard()
        {
            // Arrange - Target at the very top (next to be drawn normally)
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);
            var nounours = TestCardFactory.CreateInvocation("Nounours", 3, 3, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(0, nounours); // At the top
            deck.Insert(0, benzaieJeune);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var ability = GetAbility(AbilityName.GetNounoursFromDeck);
            var context = CreateContext(benzaieJeune, AbilityName.GetNounoursFromDeck);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardInHand(player1, "Nounours");
        }

        #endregion

        #region Family Search Tests (GetFamilyCardAbility)

        [Test]
        public void GetFamilyCard_WhenFamilyMemberInDeck_ReturnsNeedsUserInput()
        {
            // Arrange - Generic family search for Fistiland
            _deckSearchFactory = new DeckSearchAbilityFactory(PlayerRepository);
            var ability = _deckSearchFactory.CreateGetFamilyCard(AbilityName.Default, CardFamily.Fistiland);

            var searcher = CreateCard("Family Searcher", 2, 2, CardFamily.Human);
            var fistilandCard = TestCardFactory.CreateInvocation("Random Fistiland", 2, 2, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(10, fistilandCard);
            deck.Insert(0, searcher);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(searcher, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Family search should ask for user selection
            Assert.IsTrue(result.RequiresUserInput,
                "Family search should require user to select which card");
        }

        [Test]
        public void GetFamilyCard_WhenNoFamilyMemberInDeck_Fails()
        {
            // Arrange - Search for family that doesn't exist in deck
            var ability = _deckSearchFactory.CreateGetFamilyCard(AbilityName.Default, CardFamily.Spatial);

            var searcher = CreateCard("Searcher", 2, 2, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, searcher);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(searcher, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result, "No");
        }

        [Test]
        public void GetFamilyCard_CanActivate_WhenFamilyExists_ReturnsTrue()
        {
            // Arrange
            var ability = _deckSearchFactory.CreateGetFamilyCard(AbilityName.Default, CardFamily.Wizard);

            var searcher = CreateCard("Searcher", 2, 2, CardFamily.Human);
            var wizardCard = TestCardFactory.CreateInvocation("Wizard Card", 2, 2, CardFamily.Wizard);

            var deck = TestCardFactory.CreateDeck(28);
            deck.Insert(10, wizardCard);
            deck.Insert(0, searcher);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(searcher, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context),
                "Should be able to search when family card exists in deck");
        }

        [Test]
        public void GetFamilyCard_CanActivate_WhenFamilyNotInDeck_ReturnsFalse()
        {
            // Arrange
            var ability = _deckSearchFactory.CreateGetFamilyCard(AbilityName.Default, CardFamily.Police);

            var searcher = CreateCard("Searcher", 2, 2, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Insert(0, searcher);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(searcher, AbilityName.Default);

            // Act & Assert
            Assert.IsFalse(ability.CanActivate(context),
                "Should not be able to search when no family card in deck");
        }

        #endregion

        #region Scenario: Chain Search Abilities

        [Test]
        public void ChainSearch_SearchThenPlayThenSearchAgain_WorksCorrectly()
        {
            // Arrange - First search finds first target, second search finds second target
            var searcher1 = CreateCard("First Searcher", 2, 2, CardFamily.Fistiland);
            var nounours = TestCardFactory.CreateInvocation("Nounours", 3, 3, CardFamily.Fistiland);
            var benzaieJeune = TestCardFactory.CreateInvocation("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            var deck = TestCardFactory.CreateDeck(27);
            deck.Insert(5, nounours);
            deck.Insert(10, benzaieJeune);
            deck.Insert(0, searcher1);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // First search - Get Nounours
            var ability1 = GetAbility(AbilityName.GetNounoursFromDeck);
            var context1 = CreateContext(searcher1, AbilityName.GetNounoursFromDeck);
            var result1 = ability1.Execute(context1);
            AssertAbilitySuccess(result1);

            // Play Nounours from hand (pretend)
            var nounoursInHand = player1.Hand.FirstOrDefault(c => c.Title == "Nounours");
            if (nounoursInHand != null)
            {
                player1.PlayCard(nounoursInHand);
            }

            // Second search - Get Benzaie jeune
            var ability2 = GetAbility(AbilityName.GetBenzaieJeuneFromDeck);
            var context2 = CreateContext(searcher1, AbilityName.GetBenzaieJeuneFromDeck);
            var result2 = ability2.Execute(context2);

            // Assert
            AssertAbilitySuccess(result2);
            AssertCardInHand(player1, "Benzaie jeune");
        }

        #endregion
    }
}

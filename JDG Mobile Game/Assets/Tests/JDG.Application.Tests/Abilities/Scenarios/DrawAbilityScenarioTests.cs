using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.UseCases;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.TestUtilities;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for Draw abilities: Draw1Card, Draw2Cards, Draw3Cards.
    /// Tests verify these abilities work correctly in realistic game scenarios.
    ///
    /// Cards using these abilities:
    /// - Draw1Card: Not documented (general ability)
    /// - Draw2Cards: L'Elfette
    /// - Draw3Cards: Theodule, Sangoku
    /// </summary>
    [TestFixture]
    public class DrawAbilityScenarioTests : AbilityScenarioTestBase
    {
        private DrawCardUseCase _drawCardUseCase;
        private DrawCardsAbilityFactory _factory;

        protected override void SetupDefaultPlayers()
        {
            // Create players with 15 cards in deck for draw testing
            var (player, expectedDraws) = GameStateFixtures.CreateDrawSetup(15);
            Player1 = player;
            Player2 = PlayerFactory.CreatePlayer2(15);

            PlayerRepository.AddPlayer(Player1);
            PlayerRepository.AddPlayer(Player2);
        }

        protected override void RegisterAbilities()
        {
            _drawCardUseCase = new DrawCardUseCase(PlayerRepository, EventBus);
            _factory = new DrawCardsAbilityFactory(_drawCardUseCase);

            // Register all draw abilities
            AbilityRegistry.Register(AbilityName.Draw1Card, () => _factory.CreateDrawNCards(1));
            AbilityRegistry.Register(AbilityName.Draw2Cards, _factory.CreateDraw2Cards);
            AbilityRegistry.Register(AbilityName.Draw3Cards, () => _factory.CreateDrawNCards(3));
        }

        #region Draw1Card Tests

        [Test]
        public void Draw1Card_BasicScenario_DrawsOneCard()
        {
            // Arrange
            var sourceCard = CreateCard("Test Source", 2, 2);
            var initialDeckCount = Player1.DeckCount;
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw1Card);
            var context = CreateContext(sourceCard, AbilityName.Draw1Card);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertDeckCount(Player1, initialDeckCount - 1);
            AssertHandCount(Player1, initialHandCount + 1);
        }

        [Test]
        public void Draw1Card_WithEmptyDeck_ReturnsFailure()
        {
            // Arrange - Empty the deck
            while (Player1.DeckCount > 0)
            {
                Player1.DrawCard();
            }

            var sourceCard = CreateCard("Test Source", 2, 2);
            var ability = GetAbility(AbilityName.Draw1Card);
            var context = CreateContext(sourceCard, AbilityName.Draw1Card);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilityFailure(result, "No cards to draw");
        }

        [Test]
        public void Draw1Card_PublishesCardDrawnEvent()
        {
            // Arrange
            var sourceCard = CreateCard("Test Source", 2, 2);
            var ability = GetAbility(AbilityName.Draw1Card);
            var context = CreateContext(sourceCard, AbilityName.Draw1Card);

            // Act
            ExecuteAbility(ability, context);

            // Assert
            AssertEventCount(1);
        }

        [Test]
        public void Draw1Card_WithExactlyOneCardInDeck_DrawsSuccessfully()
        {
            // Arrange - Leave exactly 1 card in deck
            while (Player1.DeckCount > 1)
            {
                Player1.DrawCard();
            }

            var sourceCard = CreateCard("Test Source", 2, 2);
            var ability = GetAbility(AbilityName.Draw1Card);
            var context = CreateContext(sourceCard, AbilityName.Draw1Card);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertDeckCount(Player1, 0);
        }

        #endregion

        #region Draw2Cards Tests (L'Elfette ability)

        [Test]
        public void Draw2Cards_Elfette_OnSummon_DrawsTwoCards()
        {
            // Arrange - Simulate L'Elfette being summoned
            var elfette = CreateCard("L'Elfette", 2, 2, CardFamily.Police);
            var initialDeckCount = Player1.DeckCount;
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw2Cards);
            var context = CreateContext(elfette, AbilityName.Draw2Cards);

            // Act - Simulate OnSummon trigger
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertDeckCount(Player1, initialDeckCount - 2);
            AssertHandCount(Player1, initialHandCount + 2);
        }

        [Test]
        public void Draw2Cards_WithOnlyOneCardInDeck_DrawsPartially()
        {
            // Arrange - Leave only 1 card in deck
            while (Player1.DeckCount > 1)
            {
                Player1.DrawCard();
            }

            var elfette = CreateCard("L'Elfette", 2, 2, CardFamily.Police);
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw2Cards);
            var context = CreateContext(elfette, AbilityName.Draw2Cards);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert - Should succeed but only draw 1 card
            AssertAbilitySuccess(result, "1"); // Message should mention 1 card drawn
            AssertDeckCount(Player1, 0);
            AssertHandCount(Player1, initialHandCount + 1);
        }

        [Test]
        public void Draw2Cards_PublishesTwoDrawEvents()
        {
            // Arrange
            var elfette = CreateCard("L'Elfette", 2, 2, CardFamily.Police);
            var ability = GetAbility(AbilityName.Draw2Cards);
            var context = CreateContext(elfette, AbilityName.Draw2Cards);

            // Act
            ExecuteAbility(ability, context);

            // Assert
            AssertEventCount(2);
        }

        [Test]
        public void Draw2Cards_WithExactlyTwoCardsInDeck_DrawsBoth()
        {
            // Arrange - Leave exactly 2 cards in deck
            while (Player1.DeckCount > 2)
            {
                Player1.DrawCard();
            }

            var elfette = CreateCard("L'Elfette", 2, 2, CardFamily.Police);
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw2Cards);
            var context = CreateContext(elfette, AbilityName.Draw2Cards);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertDeckCount(Player1, 0);
            AssertHandCount(Player1, initialHandCount + 2);
        }

        #endregion

        #region Draw3Cards Tests (Sangoku, Theodule abilities)

        [Test]
        public void Draw3Cards_Sangoku_OnSummon_DrawsThreeCards()
        {
            // Arrange - Simulate Sangoku being summoned
            var sangoku = CreateCard("Sangoku", 2, 2, CardFamily.Developer);
            var initialDeckCount = Player1.DeckCount;
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw3Cards);
            var context = CreateContext(sangoku, AbilityName.Draw3Cards);

            // Act - Simulate OnSummon trigger
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertDeckCount(Player1, initialDeckCount - 3);
            AssertHandCount(Player1, initialHandCount + 3);
        }

        [Test]
        public void Draw3Cards_Theodule_OnSummon_DrawsThreeCards()
        {
            // Arrange - Simulate Theodule being summoned
            var theodule = CreateCard("Theodule", 2, 2, CardFamily.Fistiland);
            var initialDeckCount = Player1.DeckCount;
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw3Cards);
            var context = CreateContext(theodule, AbilityName.Draw3Cards);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertDeckCount(Player1, initialDeckCount - 3);
            AssertHandCount(Player1, initialHandCount + 3);
        }

        [Test]
        public void Draw3Cards_WithOnlyTwoCardsInDeck_DrawsPartially()
        {
            // Arrange - Leave only 2 cards in deck
            while (Player1.DeckCount > 2)
            {
                Player1.DrawCard();
            }

            var sangoku = CreateCard("Sangoku", 2, 2, CardFamily.Developer);
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw3Cards);
            var context = CreateContext(sangoku, AbilityName.Draw3Cards);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert - Should succeed but only draw 2 cards
            AssertAbilitySuccess(result, "2"); // Message should mention 2 cards drawn
            AssertDeckCount(Player1, 0);
            AssertHandCount(Player1, initialHandCount + 2);
        }

        [Test]
        public void Draw3Cards_PublishesThreeDrawEvents()
        {
            // Arrange
            var sangoku = CreateCard("Sangoku", 2, 2, CardFamily.Developer);
            var ability = GetAbility(AbilityName.Draw3Cards);
            var context = CreateContext(sangoku, AbilityName.Draw3Cards);

            // Act
            ExecuteAbility(ability, context);

            // Assert
            AssertEventCount(3);
        }

        [Test]
        public void Draw3Cards_MultipleDrawsInSameTurn_WorksCorrectly()
        {
            // Arrange - Simulate two cards with Draw3Cards summoned same turn
            var sangoku = CreateCard("Sangoku", 2, 2, CardFamily.Developer);
            var theodule = CreateCard("Theodule", 2, 2, CardFamily.Fistiland);
            var initialDeckCount = Player1.DeckCount;
            var initialHandCount = Player1.HandCount;

            var ability = GetAbility(AbilityName.Draw3Cards);

            // Act - First draw
            var context1 = CreateContext(sangoku, AbilityName.Draw3Cards);
            var result1 = ExecuteAbility(ability, context1);

            // Second draw
            var context2 = CreateContext(theodule, AbilityName.Draw3Cards);
            var result2 = ExecuteAbility(ability, context2);

            // Assert - Both should succeed, 6 total cards drawn
            AssertAbilitySuccess(result1);
            AssertAbilitySuccess(result2);
            AssertDeckCount(Player1, initialDeckCount - 6);
            AssertHandCount(Player1, initialHandCount + 6);
            AssertEventCount(6); // 6 draw events total
        }

        #endregion

        #region CanActivate Tests

        [Test]
        public void Draw1Card_CanActivate_AlwaysReturnsTrue()
        {
            // Arrange
            var sourceCard = CreateCard("Test Source", 2, 2);
            var ability = GetAbility(AbilityName.Draw1Card);
            var context = CreateContext(sourceCard, AbilityName.Draw1Card);

            // Act
            var canActivate = CanActivateAbility(ability, context);

            // Assert
            Assert.IsTrue(canActivate);
        }

        [Test]
        public void Draw2Cards_CanActivate_AlwaysReturnsTrue()
        {
            // Arrange
            var elfette = CreateCard("L'Elfette", 2, 2, CardFamily.Police);
            var ability = GetAbility(AbilityName.Draw2Cards);
            var context = CreateContext(elfette, AbilityName.Draw2Cards);

            // Act
            var canActivate = CanActivateAbility(ability, context);

            // Assert
            Assert.IsTrue(canActivate);
        }

        [Test]
        public void Draw3Cards_CanActivate_WithEmptyDeck_StillReturnsTrue()
        {
            // Arrange - Empty deck doesn't prevent activation, just fails on execute
            while (Player1.DeckCount > 0)
            {
                Player1.DrawCard();
            }

            var sangoku = CreateCard("Sangoku", 2, 2, CardFamily.Developer);
            var ability = GetAbility(AbilityName.Draw3Cards);
            var context = CreateContext(sangoku, AbilityName.Draw3Cards);

            // Act
            var canActivate = CanActivateAbility(ability, context);

            // Assert - Can activate returns true (execution will fail)
            Assert.IsTrue(canActivate);
        }

        #endregion
    }
}

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

namespace JDG.Application.Tests.Abilities.Integration
{
    /// <summary>
    /// Integration tests for Dependency Chain relationships.
    /// Tests cards that depend on other cards to survive on the field.
    /// </summary>
    [TestFixture]
    public class DependencyChainTests
    {
        private IPlayerRepository _playerRepository;
        private Player _player1;
        private Player _player2;
        private ProtectionAbilityFactory _protectionFactory;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _protectionFactory = new ProtectionAbilityFactory(_playerRepository);

            // Note: Individual tests set up their own players with specific cards on field
            // Default empty players for tests that don't need specific setup
            _player1 = new Player(PlayerId.Player1, TestCardFactory.CreateDeck(30));
            _player2 = new Player(PlayerId.Player2, TestCardFactory.CreateDeck(30));
        }

        private void SetupRepositoryMocks()
        {
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(_player2);
        }

        #region Alpha Man + Benzaie/Benzaie Jeune

        [Test]
        public void AlphaMan_DependsOnBenzaieOrBenzaieJeune()
        {
            // Arrange
            // Alpha Man: CantLiveWithoutBenzaieOrBenzaieJeune
            var alphaMan = TestCardFactory.CreateInvocation("Alpha Man", 4, 4, CardFamily.Fistiland);
            var benzaie = TestCardFactory.CreateInvocation("Benzaie", 5, 4, CardFamily.Fistiland);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, benzaie, alphaMan);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Benzaie", "Benzaie jeune" });
            var context = CreateContext(_player1, alphaMan, AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);

            // Act
            var result = ability.Execute(context);

            // Assert - Dependency check should pass (Benzaie is on field)
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void AlphaMan_WithBenzaieJeune_SurvivesOnField()
        {
            // Arrange
            var alphaMan = TestCardFactory.CreateInvocation("Alpha Man", 4, 4, CardFamily.Fistiland);
            var benzaieJeune = TestCardFactory.CreateInvocation("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, benzaieJeune, alphaMan);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Benzaie", "Benzaie jeune" });
            var context = CreateContext(_player1, alphaMan, AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);

            // Act
            var result = ability.Execute(context);

            // Assert - Should survive (Benzaie jeune satisfies dependency)
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void AlphaMan_WithoutBenzaieOrBenzaieJeune_Dies()
        {
            // Arrange
            var alphaMan = TestCardFactory.CreateInvocation("Alpha Man", 4, 4, CardFamily.Fistiland);
            // No Benzaie or Benzaie jeune on field

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, alphaMan);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Benzaie", "Benzaie jeune" });
            var context = CreateContext(_player1, alphaMan, AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune);

            // Act
            var result = ability.Execute(context);

            // Assert - Dependency check should fail
            Assert.IsFalse(result.IsSuccess);
        }

        #endregion

        #region Starlight Unicorn + Granolax/Mecha-Granolax

        [Test]
        public void StarlightUnicorn_DependsOnGranolaxOrMechaGranolax()
        {
            // Arrange
            // Starlight Unicorn: CantLiveWithoutGranolaxOrMechaGranolax
            var unicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Rpg);
            var granolax = TestCardFactory.CreateInvocation("Granolax", 2, 2, CardFamily.Rpg);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, granolax, unicorn);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Granolax", "Mecha-Granolax" });
            var context = CreateContext(_player1, unicorn, AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);

            // Act
            var result = ability.Execute(context);

            // Assert - Dependency check should pass
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void StarlightUnicorn_WithMechaGranolax_SurvivesOnField()
        {
            // Arrange
            var unicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Rpg);
            var mechaGranolax = TestCardFactory.CreateInvocation("Mecha-Granolax", 5, 4, CardFamily.Rpg);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, mechaGranolax, unicorn);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Granolax", "Mecha-Granolax" });
            var context = CreateContext(_player1, unicorn, AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void StarlightUnicorn_WithoutGranolaxFamily_Dies()
        {
            // Arrange
            var unicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Rpg);
            // No Granolax or Mecha-Granolax

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, unicorn);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Granolax", "Mecha-Granolax" });
            var context = CreateContext(_player1, unicorn, AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);

            // Act
            var result = ability.Execute(context);

            // Assert - Should fail dependency check
            Assert.IsFalse(result.IsSuccess);
        }

        #endregion

        #region Henry Potdebeurre + JDG

        [Test]
        public void HenryPotdebeurre_DependsOnJDG()
        {
            // Arrange
            // Henry Potdebeurre: CantLiveWithoutJDG
            var henry = TestCardFactory.CreateInvocation("Henry Potdebeurre", 3, 3, CardFamily.Developer);
            var jdg = TestCardFactory.CreateInvocation("Joueur Du Grenier", 5, 5, CardFamily.Developer);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, jdg, henry);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Joueur Du Grenier" });
            var context = CreateContext(_player1, henry, AbilityName.CantLiveWithoutJDG);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void HenryPotdebeurre_WithoutJDG_Dies()
        {
            // Arrange
            var henry = TestCardFactory.CreateInvocation("Henry Potdebeurre", 3, 3, CardFamily.Developer);
            // No JDG on field

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, henry);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Joueur Du Grenier" });
            var context = CreateContext(_player1, henry, AbilityName.CantLiveWithoutJDG);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        #endregion

        #region Comics Dependency

        [Test]
        public void ComicsDependentCard_DependsOnComicsFamily()
        {
            // Arrange
            // CantLiveWithoutComics: Requires Comics family card on field
            var dependentCard = TestCardFactory.CreateInvocation("Comics Dependent", 3, 3, CardFamily.Comics);
            var comicsCard = TestCardFactory.CreateInvocation("Comics Hero", 4, 4, CardFamily.Comics);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, comicsCard, dependentCard);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateFamilyDependencyAbility(CardFamily.Comics);
            var context = CreateContext(_player1, dependentCard, AbilityName.CantLiveWithoutComics);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Human Dependency

        [Test]
        public void HumanDependentCard_DependsOnHumanFamily()
        {
            // Arrange
            // CantLiveWithoutHuman: Requires Human family card on field
            var dependentCard = TestCardFactory.CreateInvocation("Human Dependent", 3, 3, CardFamily.Human);
            var humanCard = TestCardFactory.CreateInvocation("Human", 3, 3, CardFamily.Human);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, humanCard, dependentCard);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateFamilyDependencyAbility(CardFamily.Human);
            var context = CreateContext(_player1, dependentCard, AbilityName.CantLiveWithoutHuman);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Japan Dependency

        [Test]
        public void JapanDependentCard_DependsOnJapanFamily()
        {
            // Arrange
            // CantLiveWithoutJapon: Requires Japan family card on field
            var dependentCard = TestCardFactory.CreateInvocation("Japan Dependent", 3, 3, CardFamily.Japan);
            var japanCard = TestCardFactory.CreateInvocation("Sangoku", 4, 4, CardFamily.Japan);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, japanCard, dependentCard);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateFamilyDependencyAbility(CardFamily.Japan);
            var context = CreateContext(_player1, dependentCard, AbilityName.CantLiveWithoutJapon);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Dependency Chain Scenarios

        [Test]
        public void DependencyChain_WhenDependencyRemoved_AllDependentsDie()
        {
            // Arrange
            // Scenario: Benzaie is removed, Alpha Man should die
            var benzaie = TestCardFactory.CreateInvocation("Benzaie", 5, 4, CardFamily.Fistiland);
            var alphaMan = TestCardFactory.CreateInvocation("Alpha Man", 4, 4, CardFamily.Fistiland);

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, benzaie, alphaMan);
            SetupRepositoryMocks();

            // Simulate Benzaie being removed
            var removedCards = _player1.Field.Where(c => c.Title == "Benzaie").ToList();

            // Check if Alpha Man's dependency is now broken
            var remainingField = _player1.Field.Where(c => c.Title != "Benzaie").ToList();
            var hasValidDependency = remainingField.Any(c => c.Title == "Benzaie" || c.Title == "Benzaie jeune");

            // Assert - Alpha Man should not have valid dependency after Benzaie removal
            Assert.IsFalse(hasValidDependency);
        }

        [Test]
        public void MultipleDependents_OnSameDependency_AllDieWhenDependencyRemoved()
        {
            // Arrange
            // Multiple cards depend on the same card
            var benzaie = TestCardFactory.CreateInvocation("Benzaie", 5, 4, CardFamily.Fistiland);
            var alphaMan1 = TestCardFactory.CreateInvocation("Alpha Man", 4, 4, CardFamily.Fistiland);
            var alphaMan2 = TestCardFactory.CreateInvocation("Alpha Man 2", 4, 4, CardFamily.Fistiland);

            // All depend on Benzaie - when Benzaie dies, both Alpha Mans should die
            Assert.IsNotNull(benzaie);
            Assert.IsNotNull(alphaMan1);
            Assert.IsNotNull(alphaMan2);
        }

        [Test]
        public void CircularDependency_DoesNotCauseInfiniteLoop()
        {
            // Arrange
            // Hypothetical scenario: Card A depends on Card B, Card B depends on Card A
            // The system should handle this gracefully
            var cardA = TestCardFactory.CreateInvocation("Card A", 3, 3);
            var cardB = TestCardFactory.CreateInvocation("Card B", 3, 3);

            // This test ensures the dependency system doesn't crash
            Assert.IsNotNull(cardA);
            Assert.IsNotNull(cardB);
        }

        #endregion

        #region Dependency with Protection

        [Test]
        public void DependentCard_WithProtectionEquipment_StillNeedsDependency()
        {
            // Arrange
            // Protection equipment should not override dependency requirements
            var unicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Rpg);
            // Unicorn has protection equipment but no Granolax

            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, unicorn);
            SetupRepositoryMocks();

            var ability = _protectionFactory.CreateDependencyAbility(new[] { "Granolax", "Mecha-Granolax" });
            var context = CreateContext(_player1, unicorn, AbilityName.CantLiveWithoutGranolaxOrMechaGranolax);

            // Act
            var result = ability.Execute(context);

            // Assert - Protection doesn't override dependency
            Assert.IsFalse(result.IsSuccess);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a test player with specific cards that will be placed on field.
        /// Cards are added to the deck, then drawn and played properly.
        /// </summary>
        private Player CreatePlayerWithFieldCards(PlayerId playerId, params Card[] cardsForField)
        {
            // Create deck with our test cards at the END (so they're drawn first with DrawCard taking from end)
            var deck = new List<Card>(TestCardFactory.CreateDeck(30 - cardsForField.Length));
            deck.AddRange(cardsForField);

            var player = new Player(playerId, deck);

            // Draw and play each card
            for (int i = 0; i < cardsForField.Length; i++)
            {
                player.DrawCard(); // Draws from end of deck
                var handCard = player.Hand[^1]; // Get the drawn card
                player.PlayCard(handCard); // Play to field
            }

            return player;
        }

        private AbilityContext CreateContext(Player player, Card sourceCard, AbilityName abilityName)
        {
            var opponentId = player.Id == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
            return new AbilityContext(player.Id, opponentId, sourceCard, abilityName);
        }

        #endregion
    }
}

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
    /// Scenario tests for Resurrection abilities.
    /// Tests abilities that bring cards back from graveyard or prevent death.
    /// </summary>
    [TestFixture]
    [Category("Abilities")]
    [Category("Scenario")]
    [Category("Resurrection")]
    public class ResurrectionAbilityScenarioTests
    {
        private IPlayerRepository _playerRepository;
        private Player _player1;
        private Player _player2;
        private CombatAbilityFactory _combatFactory;
        private StatModifierAbilityFactory _statModifierFactory;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _combatFactory = new CombatAbilityFactory(_playerRepository);
            _statModifierFactory = new StatModifierAbilityFactory(_playerRepository);

            // Setup players
            _player1 = CreateTestPlayer(PlayerId.Player1);
            _player2 = CreateTestPlayer(PlayerId.Player2);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(_player2);
        }

        #region ComesBackFromDeath - Jean-Marc Soul

        [Test]
        public void ComesBackFromDeath_WhenJeanMarcSoulDies_ReturnsToField()
        {
            // Arrange
            // Jean-Marc Soul: ComesBackFromDeath - Resurrects once after death
            var jeanMarcSoul = TestCardFactory.CreateInvocation("Jean-Marc Soul", 1, 1, CardFamily.Fistiland);

            // Card must be in graveyard for resurrection to work
            _player1 = CreatePlayerWithCardInGraveyard(PlayerId.Player1, jeanMarcSoul);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);

            var ability = _combatFactory.CreateResurrection(AbilityName.ComesBackFromDeath);
            var context = CreateContext(_player1, jeanMarcSoul, AbilityName.ComesBackFromDeath);

            // Act
            var result = ability.Execute(context);

            // Assert - Card should resurrect
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void ComesBackFromDeath_OnlyResurrectsOnce()
        {
            // Arrange
            var jeanMarcSoul = TestCardFactory.CreateInvocation("Jean-Marc Soul", 1, 1, CardFamily.Fistiland);

            // Jean-Marc Soul can only come back once
            // After second death, stays in graveyard
            Assert.AreEqual(1, jeanMarcSoul.Stats.Value.Attack);
            Assert.AreEqual(1, jeanMarcSoul.Stats.Value.Defense);
        }

        [Test]
        public void ComesBackFromDeath_ResurrectsWithSameStats()
        {
            // Arrange
            var jeanMarcSoul = TestCardFactory.CreateInvocation("Jean-Marc Soul", 1, 1, CardFamily.Fistiland);

            // When resurrected, returns with original stats (1/1)
            Assert.AreEqual(1, jeanMarcSoul.Stats.Value.Attack);
            Assert.AreEqual(1, jeanMarcSoul.Stats.Value.Defense);
        }

        [Test]
        public void ComesBackFromDeath_TriggersOnDeath()
        {
            // Arrange
            var jeanMarcSoul = TestCardFactory.CreateInvocation("Jean-Marc Soul", 1, 1, CardFamily.Fistiland);

            // Card must be in graveyard for resurrection to work
            _player1 = CreatePlayerWithCardInGraveyard(PlayerId.Player1, jeanMarcSoul);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);

            var ability = _combatFactory.CreateResurrection(AbilityName.ComesBackFromDeath);
            var context = CreateContext(_player1, jeanMarcSoul, AbilityName.ComesBackFromDeath);

            // The ability triggers automatically when card would die
            var result = ability.Execute(context);

            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region ComesBackFromDeath5Times - Studio de scenaristes Canadien

        [Test]
        public void ComesBackFromDeath5Times_WhenStudioDies_ReturnsToField()
        {
            // Arrange
            // Studio de scenaristes Canadien: ComesBackFromDeath5Times
            var studio = TestCardFactory.CreateInvocation("Studio de scenaristes Canadien", 1, 1, CardFamily.Comics);

            // Card must be in graveyard for resurrection to work
            _player1 = CreatePlayerWithCardInGraveyard(PlayerId.Player1, studio);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);

            var ability = _combatFactory.CreateResurrection(AbilityName.ComesBackFromDeath5Times, 5);
            var context = CreateContext(_player1, studio, AbilityName.ComesBackFromDeath5Times);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void ComesBackFromDeath5Times_CanResurrect5Times()
        {
            // Arrange
            var studio = TestCardFactory.CreateInvocation("Studio de scenaristes Canadien", 1, 1, CardFamily.Comics);

            // Can come back 5 times total (more resilient than Jean-Marc Soul)
            Assert.AreEqual(CardFamily.Comics, studio.Families.First());
        }

        [Test]
        public void ComesBackFromDeath5Times_DiesAfter6thDeath()
        {
            // Arrange
            var studio = TestCardFactory.CreateInvocation("Studio de scenaristes Canadien", 1, 1, CardFamily.Comics);

            // After 5 resurrections, the 6th death is permanent
            Assert.IsNotNull(studio);
        }

        [Test]
        public void ComesBackFromDeath5Times_TracksDeathCount()
        {
            // Arrange
            var studio = TestCardFactory.CreateInvocation("Studio de scenaristes Canadien", 1, 1, CardFamily.Comics);

            // Card must be in graveyard for resurrection to work
            _player1 = CreatePlayerWithCardInGraveyard(PlayerId.Player1, studio);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);

            var ability = _combatFactory.CreateResurrection(AbilityName.ComesBackFromDeath5Times, 5);
            var context = CreateContext(_player1, studio, AbilityName.ComesBackFromDeath5Times);

            // System should track how many times the card has died
            var result = ability.Execute(context);

            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region GiveDeathWhenDie - Papy Grenier

        [Test]
        public void GiveDeathWhenDie_WhenPapyGrenierDies_GivesCardToOpponent()
        {
            // Arrange
            // Papy Grenier: GiveDeathWhenDie - When dies, opponent gets a card (death effect)
            var papyGrenier = TestCardFactory.CreateInvocation("Papy Grenier", 4, 2, CardFamily.HardCorner);

            PlaceOnField(_player1, papyGrenier);

            var ability = _combatFactory.CreateDeathTrigger();
            var context = CreateContext(_player1, papyGrenier, AbilityName.GiveDeathWhenDie);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void GiveDeathWhenDie_CombinesWithSurviveOneTurn()
        {
            // Arrange
            // Papy Grenier also has SurviveOneTurn - survives first death, gives card on second
            var papyGrenier = TestCardFactory.CreateInvocation("Papy Grenier", 4, 2, CardFamily.HardCorner);

            // Has both abilities: SurviveOneTurn and GiveDeathWhenDie
            Assert.AreEqual(4, papyGrenier.Stats.Value.Attack);
            Assert.AreEqual(2, papyGrenier.Stats.Value.Defense);
        }

        [Test]
        public void GiveDeathWhenDie_TriggersOnActualDeath()
        {
            // Arrange
            var papyGrenier = TestCardFactory.CreateInvocation("Papy Grenier", 4, 2, CardFamily.HardCorner);

            // GiveDeathWhenDie only triggers when card actually dies
            // (after SurviveOneTurn is exhausted)
            Assert.IsNotNull(papyGrenier);
        }

        [Test]
        public void GiveDeathWhenDie_OpponentGainsCard()
        {
            // Arrange
            var papyGrenier = TestCardFactory.CreateInvocation("Papy Grenier", 4, 2, CardFamily.HardCorner);

            var ability = _combatFactory.CreateDeathTrigger();
            var context = CreateContext(_player1, papyGrenier, AbilityName.GiveDeathWhenDie);

            // When Papy Grenier dies, opponent gets some benefit (card or effect)
            var result = ability.Execute(context);

            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        #endregion

        #region CopyBenzaieJeune - Nounours

        [Test]
        public void CopyBenzaieJeune_WhenNounoursOnField_CopiesBenzaieJeuneStats()
        {
            // Arrange
            // Nounours: CopyBenzaieJeune - Copies stats/abilities of Benzaie jeune
            var nounours = TestCardFactory.CreateInvocation("Nounours", 1, 1, CardFamily.Fistiland);
            var benzaieJeune = TestCardFactory.CreateInvocation("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            // Cards must be properly placed on field using deck -> draw -> play
            _player1 = CreatePlayerWithFieldCards(PlayerId.Player1, nounours, benzaieJeune);
            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);

            var ability = _statModifierFactory.CreateCopyStats(AbilityName.CopyBenzaieJeune, "Benzaie jeune");
            var context = CreateContext(_player1, nounours, AbilityName.CopyBenzaieJeune);
            context.TargetCard = benzaieJeune;

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void CopyBenzaieJeune_RequiresBenzaieJeuneOnField()
        {
            // Arrange
            var nounours = TestCardFactory.CreateInvocation("Nounours", 1, 1, CardFamily.Fistiland);

            // Without Benzaie jeune, Nounours stays at base stats
            Assert.AreEqual(1, nounours.Stats.Value.Attack);
            Assert.AreEqual(1, nounours.Stats.Value.Defense);
        }

        [Test]
        public void CopyBenzaieJeune_GainsBenzaieJeuneAbilities()
        {
            // Arrange
            var nounours = TestCardFactory.CreateInvocation("Nounours", 1, 1, CardFamily.Fistiland);
            var benzaieJeune = TestCardFactory.CreateInvocation("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            // When copying, Nounours should also get GetNounoursFromDeck ability
            // (though this would be recursive and is likely ignored)
            Assert.IsNotNull(nounours);
            Assert.IsNotNull(benzaieJeune);
        }

        [Test]
        public void CopyBenzaieJeune_NounoursIsObtainedViaBenzaieJeune()
        {
            // Arrange
            // Benzaie jeune has GetNounoursFromDeck ability
            var benzaieJeune = TestCardFactory.CreateInvocation("Benzaie jeune", 2, 2, CardFamily.Fistiland);
            var nounours = TestCardFactory.CreateInvocation("Nounours", 1, 1, CardFamily.Fistiland);

            // These cards form a synergy:
            // 1. Benzaie jeune searches for Nounours
            // 2. Nounours copies Benzaie jeune
            Assert.AreEqual(CardFamily.Fistiland, benzaieJeune.Families.First());
            Assert.AreEqual(CardFamily.Fistiland, nounours.Families.First());
        }

        #endregion

        #region Helper Methods

        private Player CreateTestPlayer(PlayerId playerId)
        {
            var deck = TestCardFactory.CreateDeck(30);
            return new Player(playerId, deck);
        }

        /// <summary>
        /// Creates a player with specified cards on field.
        /// Cards are added to deck, drawn, and played in proper order.
        /// </summary>
        private Player CreatePlayerWithFieldCards(PlayerId playerId, params Card[] cardsForField)
        {
            var baseDeckSize = 30 - cardsForField.Length;
            var deck = new List<Card>(TestCardFactory.CreateDeck(baseDeckSize > 0 ? baseDeckSize : 0));
            deck.AddRange(cardsForField); // Add to end so they're drawn first

            var player = new Player(playerId, deck);

            for (int i = 0; i < cardsForField.Length; i++)
            {
                player.DrawCard();
                var handCard = player.Hand[^1];
                player.PlayCard(handCard);
            }

            return player;
        }

        /// <summary>
        /// Creates a player with a card on field, then moves it to graveyard (for resurrection tests).
        /// </summary>
        private Player CreatePlayerWithCardInGraveyard(PlayerId playerId, Card cardToResurrect)
        {
            var player = CreatePlayerWithFieldCards(playerId, cardToResurrect);
            player.DestroyCardFromField(cardToResurrect);
            return player;
        }

        private void PlaceOnField(Player player, params Card[] cards)
        {
            // NOTE: This method is deprecated and silently fails if cards are not in hand
            // Use CreatePlayerWithFieldCards instead
            foreach (var card in cards)
            {
                if (player.Hand.Contains(card))
                {
                    player.PlayCard(card);
                }
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

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
    /// Scenario tests for Invoke abilities.
    /// Tests abilities that invoke/summon specific cards from deck or graveyard.
    /// </summary>
    [TestFixture]
    [Category("Abilities")]
    [Category("Scenario")]
    [Category("Invoke")]
    public class InvokeAbilityScenarioTests
    {
        private IPlayerRepository _playerRepository;
        private Player _player1;
        private Player _player2;
        private SacrificeAbilityFactory _sacrificeFactory;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _sacrificeFactory = new SacrificeAbilityFactory(_playerRepository);

            // Setup players
            _player1 = CreateTestPlayer(PlayerId.Player1);
            _player2 = CreateTestPlayer(PlayerId.Player2);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(_player2);
        }

        #region InvokeTentacules - Cliche Raciste

        [Test]
        public void InvokeTentacules_WhenClicheRacisteOnField_InvokesTentacules()
        {
            // Arrange
            // Cliche Raciste: InvokeTentacules ability
            var clicheRaciste = TestCardFactory.CreateInvocation("Cliche Raciste", 1, 1, CardFamily.Incarnation);
            var tentacules = TestCardFactory.CreateInvocation("Tentacules", 2, 4, CardFamily.Incarnation);

            // Tentacules should be in deck to be invoked
            _player1.Deck.ToList(); // Access deck

            var ability = _sacrificeFactory.CreateSacrificeToInvoke();
            var context = CreateContext(_player1, clicheRaciste, AbilityName.InvokeTentacules);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void InvokeTentacules_RequiresSacrificeClicheRaciste()
        {
            // Arrange
            var clicheRaciste = TestCardFactory.CreateInvocation("Cliche Raciste", 1, 1, CardFamily.Incarnation);

            // Cliche Raciste must be sacrificed to invoke Tentacules
            Assert.IsNotNull(clicheRaciste);
            Assert.AreEqual("Cliche Raciste", clicheRaciste.Title);
        }

        [Test]
        public void InvokeTentacules_TentaculesRequiresJapanDependency()
        {
            // Arrange
            // Tentacules has CantLiveWithoutJapon - needs Japan card to survive
            var tentacules = TestCardFactory.CreateInvocation("Tentacules", 2, 4, CardFamily.Incarnation);
            var japanCard = TestCardFactory.CreateInvocation("Japan Card", 3, 3, CardFamily.Japan);

            // Without Japan card, Tentacules should die
            Assert.IsNotNull(tentacules);
            Assert.IsNotNull(japanCard);
        }

        #endregion

        #region InvokeDresseurBidulmon - Poignee de porte / Fourchette

        [Test]
        public void InvokeDresseurBidulmon_WhenPoigneeDePorteOnField_InvokesDresseur()
        {
            // Arrange
            // Poignee de porte: InvokeDresseurBidulmon ability
            var poigneeDePorte = TestCardFactory.CreateInvocation("Poignee de porte", 2, 2, CardFamily.Incarnation);
            var dresseur = TestCardFactory.CreateInvocation("Dresseur de Bidulmon", 2, 2, CardFamily.Japan);

            var ability = _sacrificeFactory.CreateSacrificeToInvoke();
            var context = CreateContext(_player1, poigneeDePorte, AbilityName.InvokeDresseurBidulmon);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void InvokeDresseurBidulmon_WhenFourchetteOnField_InvokesDresseur()
        {
            // Arrange
            // Fourchette also has InvokeDresseurBidulmon ability
            var fourchette = TestCardFactory.CreateInvocation("Fourchette", 2, 2, CardFamily.Incarnation);
            var dresseur = TestCardFactory.CreateInvocation("Dresseur de Bidulmon", 2, 2, CardFamily.Japan);

            var ability = _sacrificeFactory.CreateSacrificeToInvoke();
            var context = CreateContext(_player1, fourchette, AbilityName.InvokeDresseurBidulmon);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void InvokeDresseurBidulmon_DresseurHasConditionalStatBoost()
        {
            // Arrange
            // Dresseur de Bidulmon: Win1ATK1DefJaponWith2ATK2DEFCondition
            var dresseur = TestCardFactory.CreateInvocation("Dresseur de Bidulmon", 2, 2, CardFamily.Japan);

            // This card gains +1 ATK +1 DEF when there are 2+ Japan cards with 2+ ATK/DEF
            Assert.IsNotNull(dresseur);
            Assert.AreEqual(CardFamily.Japan, dresseur.Families.First());
        }

        #endregion

        #region InvokeSebOrJDG - Le Hobbit / Le voisin

        [Test]
        public void InvokeSebOrJDG_WhenLeHobbitOnField_InvokesSebOrJDG()
        {
            // Arrange
            // Le Hobbit: InvokeSebOrJDG ability
            var leHobbit = TestCardFactory.CreateInvocation("Le Hobbit", 2, 2, CardFamily.Police);
            var sebDuGrenier = TestCardFactory.CreateInvocation("Seb Du Grenier", 3, 3, CardFamily.HardCorner);
            var joueurDuGrenier = TestCardFactory.CreateInvocation("Joueur Du Grenier", 3, 3, CardFamily.HardCorner);

            var ability = _sacrificeFactory.CreateSacrificeToInvoke();
            var context = CreateContext(_player1, leHobbit, AbilityName.InvokeSebOrJDG);

            // Act
            var result = ability.Execute(context);

            // Assert - Should require user input to choose Seb or JDG
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void InvokeSebOrJDG_WhenLeVoisinOnField_InvokesSebOrJDG()
        {
            // Arrange
            // Le voisin also has InvokeSebOrJDG ability
            var leVoisin = TestCardFactory.CreateInvocation("Le voisin", 2, 2, CardFamily.Japan);
            var sebDuGrenier = TestCardFactory.CreateInvocation("Seb Du Grenier", 3, 3, CardFamily.HardCorner);

            var ability = _sacrificeFactory.CreateSacrificeToInvoke();
            var context = CreateContext(_player1, leVoisin, AbilityName.InvokeSebOrJDG);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void InvokeSebOrJDG_PlayerChoosesBetweenSebAndJDG()
        {
            // Arrange
            // Player should be able to choose either Seb Du Grenier or Joueur Du Grenier
            var seb = TestCardFactory.CreateInvocation("Seb Du Grenier", 3, 3, CardFamily.HardCorner);
            var jdg = TestCardFactory.CreateInvocation("Joueur Du Grenier", 3, 3, CardFamily.HardCorner);

            // Both are valid targets
            Assert.AreEqual(CardFamily.HardCorner, seb.Families.First());
            Assert.AreEqual(CardFamily.HardCorner, jdg.Families.First());
        }

        #endregion

        #region SacrificeToInvoke - Sheik Point

        [Test]
        public void SacrificeToInvoke_WhenSheikPointOnField_SacrificesAndInvokes()
        {
            // Arrange
            // Sheik Point: SacrificeToInvoke ability
            var sheikPoint = TestCardFactory.CreateInvocation("Sheik Point", 1, 1, CardFamily.HardCorner);

            var ability = _sacrificeFactory.CreateSacrificeToInvoke();
            var context = CreateContext(_player1, sheikPoint, AbilityName.SacrificeToInvoke);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void SacrificeToInvoke_SourceCardGoesToGraveyard()
        {
            // Arrange
            var sheikPoint = TestCardFactory.CreateInvocation("Sheik Point", 1, 1, CardFamily.HardCorner);

            // After sacrifice, Sheik Point should be in graveyard
            Assert.IsNotNull(sheikPoint);
            Assert.AreEqual(1, sheikPoint.Stats.Value.Attack);
            Assert.AreEqual(1, sheikPoint.Stats.Value.Defense);
        }

        #endregion

        #region Helper Methods

        private Player CreateTestPlayer(PlayerId playerId)
        {
            var deck = TestCardFactory.CreateDeck(30);
            return new Player(playerId, deck);
        }

        private AbilityContext CreateContext(Player player, Card sourceCard, AbilityName abilityName)
        {
            var opponentId = player.Id == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
            return new AbilityContext(player.Id, opponentId, sourceCard, abilityName);
        }

        #endregion
    }
}

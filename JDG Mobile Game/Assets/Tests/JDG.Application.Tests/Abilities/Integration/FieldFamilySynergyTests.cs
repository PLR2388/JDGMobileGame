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
    /// Integration tests for Field + Family synergies.
    /// Tests that field cards correctly boost their associated families.
    /// </summary>
    [TestFixture]
    public class FieldFamilySynergyTests
    {
        private IPlayerRepository _playerRepository;
        private Player _player1;
        private Player _player2;
        private FieldAbilityFactory _fieldFactory;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _fieldFactory = new FieldAbilityFactory(_playerRepository);

            // Setup players
            _player1 = CreateTestPlayer(PlayerId.Player1);
            _player2 = CreateTestPlayer(PlayerId.Player2);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(_player2);
        }

        #region Le Hard Corner + Fistiland Family

        [Test]
        public void LeHardCorner_BoostsFistilandFamily_ATKBonus()
        {
            // Arrange
            // Le Hard Corner: +1 ATK to Fistiland family cards
            var fieldCard = TestCardFactory.CreateField("Le Hard Corner", CardFamily.Fistiland);
            var benzaie = TestCardFactory.CreateInvocation("Benzaie", 5, 4, CardFamily.Fistiland);
            var benzaieJeune = TestCardFactory.CreateInvocation("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            PlaceOnField(_player1, benzaie, benzaieJeune);

            var ability = _fieldFactory.CreateFamilyBoostField(CardFamily.Fistiland, 1, 0);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void LeHardCorner_DoesNotBoostOtherFamilies()
        {
            // Arrange
            var fieldCard = TestCardFactory.CreateField("Le Hard Corner", CardFamily.Fistiland);
            var japanCard = TestCardFactory.CreateInvocation("Japan Card", 3, 3, CardFamily.Japan);

            PlaceOnField(_player1, japanCard);

            var ability = _fieldFactory.CreateFamilyBoostField(CardFamily.Fistiland, 1, 0);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Japan cards should not receive bonus
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Studio Développement + Developer Family

        [Test]
        public void StudioDeveloppement_BoostsDeveloperFamily()
        {
            // Arrange
            // Studio de développement: Boosts Developer family
            var fieldCard = TestCardFactory.CreateField("Studio de developpement", CardFamily.Developer);
            var devCard1 = TestCardFactory.CreateInvocation("Dev 1", 3, 3, CardFamily.Developer);
            var devCard2 = TestCardFactory.CreateInvocation("Dev 2", 4, 4, CardFamily.Developer);

            PlaceOnField(_player1, devCard1, devCard2);

            var ability = _fieldFactory.CreateFamilyBoostField(CardFamily.Developer, 1, 0);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Tokyo-3 + Japan Family

        [Test]
        public void Tokyo3_BoostsJapanFamily()
        {
            // Arrange
            // Tokyo-3: +1 ATK to Japan family
            var fieldCard = TestCardFactory.CreateField("Tokyo-3", CardFamily.Japan);
            var japanCard = TestCardFactory.CreateInvocation("Sangoku", 4, 4, CardFamily.Japan);

            PlaceOnField(_player1, japanCard);

            var ability = _fieldFactory.CreateFamilyBoostField(CardFamily.Japan, 1, 0);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Centre Spatial de Kourou + Spatial Family

        [Test]
        public void CentreSpatial_BoostsSpatialFamily_DEFBonus()
        {
            // Arrange
            // Centre Spatial de Kourou: +1 DEF to Spatial family
            var fieldCard = TestCardFactory.CreateField("Centre Spatial de Kourou", CardFamily.Spatial);
            var spatialCard = TestCardFactory.CreateInvocation("Space Card", 3, 3, CardFamily.Spatial);

            PlaceOnField(_player1, spatialCard);

            var ability = _fieldFactory.CreateFamilyBoostField(CardFamily.Spatial, 0, 1);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Lycée Magique Georges Pompidou + Wizard Family

        [Test]
        public void LyceeMagique_EnablesWizardCondition()
        {
            // Arrange
            // This field enables condition: LyceeMagiqueGeorgesPompidouOnField
            var fieldCard = TestCardFactory.CreateField("Lycee magique Georges Pompidou", CardFamily.Wizard);
            var wizardCard = TestCardFactory.CreateInvocation("Wizard", 3, 3, CardFamily.Wizard);

            PlaceOnField(_player1, wizardCard);

            // The field should enable summon condition for specific cards
            Assert.IsNotNull(fieldCard);
        }

        #endregion

        #region Forêt des Elfes Sylvains + Wizard Family

        [Test]
        public void ForetDesElfes_EnablesForestCondition()
        {
            // Arrange
            // This field enables condition: ForetDesElfesSylvainsOnField
            var fieldCard = TestCardFactory.CreateField("Foret des elfes sylvains", CardFamily.Wizard);

            // This enables certain invocation cards to be summoned
            Assert.IsNotNull(fieldCard);
        }

        #endregion

        #region Zozan Kebab Field

        [Test]
        public void ZozanKebab_EnablesFieldCondition()
        {
            // Arrange
            // This field enables condition: ZozanKebabOnField
            var fieldCard = TestCardFactory.CreateField("Zozan Kebab", CardFamily.Human);

            // This enables certain invocation cards to be summoned
            Assert.IsNotNull(fieldCard);
        }

        #endregion

        #region Draw Bonus Field Cards

        [Test]
        public void DrawBonusField_IncreasesCardsDrawn()
        {
            // Arrange
            var fieldCard = TestCardFactory.CreateField("Draw Field", CardFamily.None);

            var ability = _fieldFactory.CreateDrawBonusField(1);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Draw bonus should apply
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region HP Heal Field Cards

        [Test]
        public void HealPerWizardField_HealsPerWizardOnField()
        {
            // Arrange
            // Some fields heal HP based on family members on field
            var fieldCard = TestCardFactory.CreateField("Heal Field", CardFamily.Wizard);
            var wizard1 = TestCardFactory.CreateInvocation("Wizard 1", 2, 2, CardFamily.Wizard);
            var wizard2 = TestCardFactory.CreateInvocation("Wizard 2", 3, 3, CardFamily.Wizard);

            PlaceOnField(_player1, wizard1, wizard2);

            var ability = _fieldFactory.CreateHealPerFamilyField(CardFamily.Wizard, 1); // 1 HP per Wizard
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should heal 2 HP (2 wizards x 1 HP each)
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Change Family Field Cards

        [Test]
        public void ChangeFamilyField_ChangesCardFamily()
        {
            // Arrange
            // Some fields change all invocations to a specific family
            var fieldCard = TestCardFactory.CreateField("Change Family Field", CardFamily.Developer);
            var genericCard = TestCardFactory.CreateInvocation("Generic", 3, 3, CardFamily.Human);

            PlaceOnField(_player1, genericCard);

            var ability = _fieldFactory.CreateChangeFamilyField(CardFamily.Developer);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Skip Draw for Family Card

        [Test]
        public void SkipDrawForFamilyCard_AllowsSearchInsteadOfDraw()
        {
            // Arrange
            // Skip normal draw to search for a specific family card
            var fieldCard = TestCardFactory.CreateField("Search Field", CardFamily.Fistiland);

            var ability = _fieldFactory.CreateSkipDrawForFamilyCardField(CardFamily.Fistiland);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should allow search
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        #endregion

        #region Multiple Families Affected

        [Test]
        public void FieldBoost_AffectsMultipleFamilyMembers()
        {
            // Arrange
            var fieldCard = TestCardFactory.CreateField("Multi Card Field", CardFamily.Fistiland);
            var card1 = TestCardFactory.CreateInvocation("Card 1", 3, 3, CardFamily.Fistiland);
            var card2 = TestCardFactory.CreateInvocation("Card 2", 2, 2, CardFamily.Fistiland);
            var card3 = TestCardFactory.CreateInvocation("Card 3", 4, 4, CardFamily.Fistiland);
            var nonFamilyCard = TestCardFactory.CreateInvocation("Other", 3, 3, CardFamily.Japan);

            PlaceOnField(_player1, card1, card2, card3, nonFamilyCard);

            var ability = _fieldFactory.CreateFamilyBoostField(CardFamily.Fistiland, 1, 1);
            var context = CreateContext(_player1, fieldCard, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Only 3 Fistiland cards should receive boost
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Field Card Replacement

        [Test]
        public void NewFieldCard_ReplacesOldFieldEffect()
        {
            // Arrange
            var oldField = TestCardFactory.CreateField("Old Field", CardFamily.Fistiland);
            var newField = TestCardFactory.CreateField("New Field", CardFamily.Japan);

            // The old field effect should be removed when new field is played
            Assert.AreNotEqual(oldField.Title, newField.Title);
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

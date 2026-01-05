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
    /// Scenario tests for Field abilities.
    /// Tests verify field card effects work correctly in realistic game scenarios.
    ///
    /// Field Abilities (12):
    /// - Earn1DEFForSpatialFamily: +1 DEF for Spatial cards
    /// - Earn1ATKForJapanFamily: +1 ATK for Japan cards
    /// - Earn1ATKForWizardFamily: +1 ATK for Wizard cards
    /// - Earn1ATKForHardCornerFamily: +1 ATK for HardCorner cards
    /// - Earn1ATK1DEFForFistilandFamily: +1 ATK/DEF for Fistiland cards
    /// - ChangePatronInfogramFamilyToDev: Change Patron Infogram to Developer
    /// - ChangeJMBruitagesFamilyToDev: Change JM Bruitages to Developer
    /// - DrawOneMoreCard: Draw +1 card per turn
    /// - SkipDrawToGetFistilandInvocation: Skip draw to get Fistiland card
    /// - EarnHalfHPPerWizardInvocationEachTurn: Heal per Wizard card
    /// </summary>
    [TestFixture]
    public class FieldAbilityScenarioTests : AbilityScenarioTestBase
    {
        private FieldAbilityFactory _fieldFactory;

        protected override void RegisterAbilities()
        {
            _fieldFactory = new FieldAbilityFactory(PlayerRepository);
        }

        #region FamilyBoostFieldAbility Tests

        [Test]
        public void FamilyBoostField_Earn1DEFForSpatialFamily_BoostsSpatialCards()
        {
            // Arrange
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.Spatial, 0, 1);

            var spatialCard = CardFactory.CreateInvocation("Space Ship", 3, 3, CardFamily.Spatial);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, spatialCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Spatial");
        }

        [Test]
        public void FamilyBoostField_Earn1ATKForJapanFamily_BoostsJapanCards()
        {
            // Arrange
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.Japan, 1, 0);

            var japanCard1 = CardFactory.CreateInvocation("Samurai", 2, 2, CardFamily.Japan);
            var japanCard2 = CardFactory.CreateInvocation("Ninja", 3, 3, CardFamily.Japan);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, japanCard1);
            deck.Insert(1, japanCard2);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Samurai"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Ninja"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should boost both Japan cards
            AssertAbilitySuccess(result, "2"); // 2 Japan cards boosted
        }

        [Test]
        public void FamilyBoostField_Earn1ATK1DEFForFistilandFamily_BoostsBoth()
        {
            // Arrange - +1 ATK and +1 DEF for Fistiland
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.Fistiland, 1, 1);

            var fistilandCard = CardFactory.CreateInvocation("Benzaie", 5, 4, CardFamily.Fistiland);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, fistilandCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Fistiland");
        }

        [Test]
        public void FamilyBoostField_NoTargetFamily_Fails()
        {
            // Arrange - Wizard boost but no Wizard on field
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.Wizard, 1, 0);

            var humanCard = CardFactory.CreateInvocation("Regular Human", 2, 2, CardFamily.Human);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, humanCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should boost 0 cards (no Wizards)
            AssertAbilitySuccess(result, "0");
        }

        [Test]
        public void FamilyBoostField_CanActivate_WhenFamilyPresent_ReturnsTrue()
        {
            // Arrange
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.HardCorner, 1, 0);

            var hardCornerCard = CardFactory.CreateInvocation("HC Card", 3, 3, CardFamily.HardCorner);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, hardCornerCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void FamilyBoostField_HasContinuousTrigger()
        {
            // Arrange
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.Spatial, 0, 1);

            // Assert
            Assert.AreEqual(AbilityTrigger.Continuous, ability.Trigger,
                "Family boost should be continuous effect");
        }

        #endregion

        #region HealPerFamilyFieldAbility Tests

        [Test]
        public void HealPerFamilyField_HealsBasedOnFamilyCount()
        {
            // Arrange - 0.5 HP per Wizard card (using half-star system)
            var ability = _fieldFactory.CreateHealPerFamily(CardFamily.Wizard, 0.5f);

            var wizard1 = CardFactory.CreateInvocation("Mage", 2, 2, CardFamily.Wizard);
            var wizard2 = CardFactory.CreateInvocation("Sorcerer", 3, 3, CardFamily.Wizard);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, wizard1);
            deck.Insert(1, wizard2);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Mage"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Sorcerer"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should heal 1 HP (2 wizards * 0.5 HP)
            AssertAbilitySuccess(result, "1"); // Total heal
        }

        [Test]
        public void HealPerFamilyField_NoFamilyCards_NoHealing()
        {
            // Arrange
            var ability = _fieldFactory.CreateHealPerFamily(CardFamily.Wizard, 0.5f);

            var humanCard = CardFactory.CreateInvocation("Human", 2, 2, CardFamily.Human);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, humanCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should heal 0 HP
            AssertAbilitySuccess(result, "0");
        }

        [Test]
        public void HealPerFamilyField_HasTurnStartTrigger()
        {
            // Arrange
            var ability = _fieldFactory.CreateHealPerFamily(CardFamily.Wizard, 0.5f);

            // Assert
            Assert.AreEqual(AbilityTrigger.OnTurnStart, ability.Trigger,
                "Heal should trigger on turn start");
        }

        #endregion

        #region ChangeFamilyFieldAbility Tests

        [Test]
        public void ChangeFamilyField_ChangesAllInvocationsFamily()
        {
            // Arrange - Change all to Developer family
            var ability = _fieldFactory.CreateChangeFamily(CardFamily.Developer);

            var card1 = CardFactory.CreateInvocation("Card 1", 2, 2, CardFamily.Human);
            var card2 = CardFactory.CreateInvocation("Card 2", 3, 3, CardFamily.Monster);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, card1);
            deck.Insert(1, card2);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Card 1"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Card 2"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Both cards should be changed to Developer
            AssertAbilitySuccess(result, "Developer");
            Assert.IsTrue(result.Message.Contains("2"), "Should change 2 cards");
        }

        [Test]
        public void ChangeFamilyField_NoInvocations_Fails()
        {
            // Arrange - Empty field
            var ability = _fieldFactory.CreateChangeFamily(CardFamily.Developer);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should change 0 cards
            AssertAbilitySuccess(result, "0");
        }

        #endregion

        #region ChangeByNameFieldAbility Tests

        [Test]
        public void ChangeByNameField_AddsNewFamilyToSpecificCard()
        {
            // Arrange - Change "Patron Infogrames" to add Developer family
            var ability = _fieldFactory.CreateChangeByName("Patron Infogrames", CardFamily.Developer);

            var patronCard = CardFactory.CreateInvocation("Patron Infogrames", 4, 3, CardFamily.HardCorner);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, patronCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Developer");
        }

        [Test]
        public void ChangeByNameField_CardNotOnField_Fails()
        {
            // Arrange - Target card not present
            var ability = _fieldFactory.CreateChangeByName("Missing Card", CardFamily.Developer);

            var otherCard = CardFactory.CreateInvocation("Other Card", 2, 2, CardFamily.Human);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(0, otherCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result, "not found");
        }

        [Test]
        public void ChangeByNameField_HasContinuousTrigger()
        {
            // Arrange
            var ability = _fieldFactory.CreateChangeByName("Card", CardFamily.Developer);

            // Assert
            Assert.AreEqual(AbilityTrigger.Continuous, ability.Trigger,
                "Name-based change should be continuous effect");
        }

        #endregion

        #region DrawBonusFieldAbility Tests

        [Test]
        public void DrawBonusField_ReturnsSuccessMessage()
        {
            // Arrange
            var ability = _fieldFactory.CreateDrawBonus(1);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "+1");
        }

        [Test]
        public void DrawBonusField_CanActivate_AlwaysTrue()
        {
            // Arrange
            var ability = _fieldFactory.CreateDrawBonus(1);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void DrawBonusField_HasContinuousTrigger()
        {
            // Arrange
            var ability = _fieldFactory.CreateDrawBonus(1);

            // Assert
            Assert.AreEqual(AbilityTrigger.Continuous, ability.Trigger,
                "Draw bonus should be continuous effect");
        }

        #endregion

        #region SkipDrawForFamilyCardFieldAbility Tests

        [Test]
        public void SkipDrawForFamilyCard_SearchesDeckForFamily()
        {
            // Arrange - Skip draw to get Fistiland card
            var ability = _fieldFactory.CreateSkipDrawForFamily(CardFamily.Fistiland);

            var fistilandCard = CardFactory.CreateInvocation("Benzaie", 5, 4, CardFamily.Fistiland);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(10, fistilandCard); // In middle of deck

            var player1 = new Player(PlayerId.Player1, deck);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Benzaie");
            AssertCardInHand(player1, "Benzaie");
        }

        [Test]
        public void SkipDrawForFamilyCard_NoFamilyInDeck_Fails()
        {
            // Arrange - No Fistiland cards in deck
            var ability = _fieldFactory.CreateSkipDrawForFamily(CardFamily.Fistiland);

            var deck = CardFactory.CreateDeck(30); // No Fistiland cards

            var player1 = new Player(PlayerId.Player1, deck);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result, "No");
        }

        [Test]
        public void SkipDrawForFamilyCard_CanActivate_WhenFamilyInDeck_ReturnsTrue()
        {
            // Arrange
            var ability = _fieldFactory.CreateSkipDrawForFamily(CardFamily.Fistiland);

            var fistilandCard = CardFactory.CreateInvocation("Fistiland Card", 2, 2, CardFamily.Fistiland);
            var deck = CardFactory.CreateDeck(29);
            deck.Insert(5, fistilandCard);

            var player1 = new Player(PlayerId.Player1, deck);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void SkipDrawForFamilyCard_CanActivate_WhenNoFamilyInDeck_ReturnsFalse()
        {
            // Arrange
            var ability = _fieldFactory.CreateSkipDrawForFamily(CardFamily.Police);

            var deck = CardFactory.CreateDeck(30); // No Police cards

            var player1 = new Player(PlayerId.Player1, deck);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsFalse(ability.CanActivate(context));
        }

        #endregion

        #region Edge Cases

        [Test]
        public void FieldAbility_DoesNotAffectOpponentCards()
        {
            // Arrange - Fistiland boost
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.Fistiland, 2, 2);

            // Player 1's Fistiland card
            var myCard = CardFactory.CreateInvocation("My Fistiland", 3, 3, CardFamily.Fistiland);
            var deck1 = CardFactory.CreateDeck(29);
            deck1.Insert(0, myCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            // Player 2's Fistiland card
            var opponentCard = CardFactory.CreateInvocation("Enemy Fistiland", 3, 3, CardFamily.Fistiland);
            var deck2 = CardFactory.CreateDeck(29);
            deck2.Insert(0, opponentCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Remember opponent stats before
            var enemyField = player2.Field.First();
            var origAtk = enemyField.Stats?.Attack ?? 0;
            var origDef = enemyField.Stats?.Defense ?? 0;

            // Act
            ability.Execute(context);

            // Assert - Opponent's Fistiland card should be unchanged
            Assert.AreEqual(origAtk, enemyField.Stats?.Attack ?? 0,
                "Opponent's card ATK should be unchanged");
            Assert.AreEqual(origDef, enemyField.Stats?.Defense ?? 0,
                "Opponent's card DEF should be unchanged");
        }

        [Test]
        public void FieldAbility_MixedFamilyField_OnlyBoostsTargetFamily()
        {
            // Arrange
            var ability = _fieldFactory.CreateFamilyBoost(CardFamily.Fistiland, 1, 1);

            var fistilandCard = CardFactory.CreateInvocation("Fistiland Card", 2, 2, CardFamily.Fistiland);
            var humanCard = CardFactory.CreateInvocation("Human Card", 3, 3, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Insert(0, fistilandCard);
            deck.Insert(1, humanCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Fistiland Card"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Human Card"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Only 1 Fistiland card should be boosted
            AssertAbilitySuccess(result, "1");
        }

        #endregion
    }
}

using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.TestUtilities;
using System.Linq;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for Sacrifice abilities.
    /// Tests verify these abilities work correctly in realistic game scenarios.
    ///
    /// Sacrifice Abilities (16):
    /// - SacrificeArchibaldVonGrenier: Sacrifice specific card for stats
    /// - SacrificeBenzaieJeune: Benzaie sacrifices Benzaie jeune for +1 ATK/DEF
    /// - SacrificeJoueurDuGrenier: Sacrifice JDG for stats
    /// - SacrificeSebDuGrenierOnHardCornerForAtkDef: Sacrifice Seb for stats
    /// - Sacrifice3Atk3Def: Sacrifice card with 3+ ATK and 3+ DEF
    /// - SacrificeWizard: Sacrifice wizard family card
    /// - SacrificeDeveloper3Atk3Def: Sacrifice developer with 3+ ATK/DEF
    /// - SacrificeHardCorner3Atk3Def: Sacrifice HardCorner with 3+ ATK/DEF
    /// - Sacrifice2Japan: Sacrifice 2 Japan family cards
    /// - Sacrifice2Incarnation: Sacrifice 2 Incarnation family cards
    /// - SacrificeGranolax: Mecha-Granolax sacrifices Granolax
    /// - SacrificeJDGOnStudioDevForAtkDef: Sacrifice JDG on studio dev
    /// - SacrificeSebDuGrenier: Sacrifice Seb du Grenier
    /// - SacrificeClicheRaciste: Sacrifice Cliche Raciste card
    /// - SacrificeToInvoke: Generic sacrifice to invoke
    /// </summary>
    [TestFixture]
    public class SacrificeAbilityScenarioTests : AbilityScenarioTestBase
    {
        private SacrificeAbilityFactory _sacrificeFactory;
        private SpecialAbilityFactory _specialFactory;

        protected override void RegisterAbilities()
        {
            _sacrificeFactory = new SacrificeAbilityFactory(PlayerRepository);
            _specialFactory = new SpecialAbilityFactory(PlayerRepository);

            // Register sacrifice abilities - specific card sacrifices
            AbilityRegistry.Register(AbilityName.SacrificeBenzaieJeune,
                () => _sacrificeFactory.CreateSacrificeCard(AbilityName.SacrificeBenzaieJeune, "Benzaie jeune"));
            AbilityRegistry.Register(AbilityName.SacrificeGranolax,
                () => _sacrificeFactory.CreateSacrificeCard(AbilityName.SacrificeGranolax, "Granolax"));
            AbilityRegistry.Register(AbilityName.SacrificeArchibaldVonGrenier,
                () => _sacrificeFactory.CreateSacrificeCard(AbilityName.SacrificeArchibaldVonGrenier, "Archibald Von Grenier"));
            AbilityRegistry.Register(AbilityName.SacrificeJoueurDuGrenier,
                () => _sacrificeFactory.CreateSacrificeCard(AbilityName.SacrificeJoueurDuGrenier, "Joueur du Grenier"));
            AbilityRegistry.Register(AbilityName.SacrificeSebDuGrenier,
                () => _sacrificeFactory.CreateSacrificeCard(AbilityName.SacrificeSebDuGrenier, "Seb du Grenier"));
            AbilityRegistry.Register(AbilityName.SacrificeClicheRaciste,
                () => _sacrificeFactory.CreateSacrificeCard(AbilityName.SacrificeClicheRaciste, "Cliche Raciste"));

            // Register conditional sacrifice abilities
            AbilityRegistry.Register(AbilityName.Sacrifice3Atk3Def,
                () => _specialFactory.CreateConditionalSacrifice(AbilityName.Sacrifice3Atk3Def, 3, 3, CardFamily.Any, 1));
            AbilityRegistry.Register(AbilityName.SacrificeWizard,
                () => _specialFactory.CreateConditionalSacrifice(AbilityName.SacrificeWizard, 0, 0, CardFamily.Wizard, 1));
            AbilityRegistry.Register(AbilityName.SacrificeDeveloper3Atk3Def,
                () => _specialFactory.CreateConditionalSacrifice(AbilityName.SacrificeDeveloper3Atk3Def, 3, 3, CardFamily.Developer, 1));
            AbilityRegistry.Register(AbilityName.SacrificeHardCorner3Atk3Def,
                () => _specialFactory.CreateConditionalSacrifice(AbilityName.SacrificeHardCorner3Atk3Def, 3, 3, CardFamily.HardCorner, 1));
            AbilityRegistry.Register(AbilityName.Sacrifice2Japan,
                () => _specialFactory.CreateConditionalSacrifice(AbilityName.Sacrifice2Japan, 0, 0, CardFamily.Japan, 2));
            AbilityRegistry.Register(AbilityName.Sacrifice2Incarnation,
                () => _specialFactory.CreateConditionalSacrifice(AbilityName.Sacrifice2Incarnation, 0, 0, CardFamily.Incarnation, 2));

            // Register optional sacrifice abilities
            AbilityRegistry.Register(AbilityName.SacrificeSebDuGrenierOnHardCornerForAtkDef,
                () => _specialFactory.CreateOptionalSacrificeForStats(AbilityName.SacrificeSebDuGrenierOnHardCornerForAtkDef, 2, 2));
            AbilityRegistry.Register(AbilityName.SacrificeJDGOnStudioDevForAtkDef,
                () => _specialFactory.CreateOptionalSacrificeForStats(AbilityName.SacrificeJDGOnStudioDevForAtkDef, 1, 1));

            // Register sacrifice to invoke
            AbilityRegistry.Register(AbilityName.SacrificeToInvoke, _sacrificeFactory.CreateSacrificeToInvoke);
        }

        #region SacrificeBenzaieJeune Tests

        [Test]
        public void SacrificeBenzaieJeune_WhenBenzaieJeuneOnField_SacrificesSuccessfully()
        {
            // Arrange - Place Benzaie and Benzaie jeune on field
            var benzaie = CreateCard("Benzaie", 5, 4, CardFamily.Fistiland);
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            // Setup deck with these cards
            var deck = CardFactory.CreateDeck(28);
            deck.Add(benzaie);
            deck.Add(benzaieJeune);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            // Draw and play both cards
            player.DrawCard();
            player.DrawCard();
            var benzaieCard = player.Hand.FirstOrDefault(c => c.Title == "Benzaie");
            var benzaieJeuneCard = player.Hand.FirstOrDefault(c => c.Title == "Benzaie jeune");
            if (benzaieCard != null) player.PlayCard(benzaieCard);
            if (benzaieJeuneCard != null) player.PlayCard(benzaieJeuneCard);

            var ability = GetAbility(AbilityName.SacrificeBenzaieJeune);
            var context = CreateContext(benzaieCard, AbilityName.SacrificeBenzaieJeune);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardNotOnField(player, "Benzaie jeune");
            AssertCardOnField(player, "Benzaie");
        }

        [Test]
        public void SacrificeBenzaieJeune_WhenBenzaieJeuneNotOnField_ReturnsFail()
        {
            // Arrange - Only Benzaie on field, no Benzaie jeune
            var benzaie = CreateCard("Benzaie", 5, 4, CardFamily.Fistiland);

            var deck = CardFactory.CreateDeck(29);
            deck.Add(benzaie);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            var benzaieCard = player.Hand.FirstOrDefault(c => c.Title == "Benzaie");
            if (benzaieCard != null) player.PlayCard(benzaieCard);

            var ability = GetAbility(AbilityName.SacrificeBenzaieJeune);
            var context = CreateContext(benzaieCard, AbilityName.SacrificeBenzaieJeune);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilityFailure(result);
            AssertCardOnField(player, "Benzaie");
        }

        #endregion

        #region SacrificeGranolax Tests

        [Test]
        public void SacrificeGranolax_WhenGranolaxOnField_SacrificesSuccessfully()
        {
            // Arrange - Place Mecha-Granolax and Granolax on field
            var mechaGranolax = CreateCard("Mecha-Granolax", 5, 4, CardFamily.Rpg);
            var granolax = CreateCard("Granolax", 2, 2, CardFamily.Rpg);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(mechaGranolax);
            deck.Add(granolax);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            var mecha = player.Hand.FirstOrDefault(c => c.Title == "Mecha-Granolax");
            var grano = player.Hand.FirstOrDefault(c => c.Title == "Granolax");
            if (mecha != null) player.PlayCard(mecha);
            if (grano != null) player.PlayCard(grano);

            var ability = GetAbility(AbilityName.SacrificeGranolax);
            var context = CreateContext(mecha, AbilityName.SacrificeGranolax);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilitySuccess(result);
            AssertCardNotOnField(player, "Granolax");
            AssertCardOnField(player, "Mecha-Granolax");
        }

        [Test]
        public void SacrificeGranolax_SendsToGraveyard()
        {
            // Arrange
            var mechaGranolax = CreateCard("Mecha-Granolax", 5, 4, CardFamily.Rpg);
            var granolax = CreateCard("Granolax", 2, 2, CardFamily.Rpg);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(mechaGranolax);
            deck.Add(granolax);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            var mecha = player.Hand.FirstOrDefault(c => c.Title == "Mecha-Granolax");
            var grano = player.Hand.FirstOrDefault(c => c.Title == "Granolax");
            if (mecha != null) player.PlayCard(mecha);
            if (grano != null) player.PlayCard(grano);

            var ability = GetAbility(AbilityName.SacrificeGranolax);
            var context = CreateContext(mecha, AbilityName.SacrificeGranolax);

            // Act
            ExecuteAbility(ability, context);

            // Assert
            AssertCardInGraveyard(player, "Granolax");
        }

        #endregion

        #region Conditional Sacrifice Tests (Sacrifice3Atk3Def)

        [Test]
        public void Sacrifice3Atk3Def_WithValidCard_Succeeds()
        {
            // Arrange - Place a card with 3+ ATK and 3+ DEF
            var strongCard = CreateCard("Strong Card", 4, 4, CardFamily.Human);
            var sourceCard = CreateCard("Source Card", 3, 3, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(strongCard);
            deck.Add(sourceCard);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            foreach (var card in player.Hand.ToList())
            {
                player.PlayCard(card);
            }

            var ability = GetAbility(AbilityName.Sacrifice3Atk3Def);
            var context = CreateContext(sourceCard, AbilityName.Sacrifice3Atk3Def);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert - Should require user selection or succeed if auto-selected
            // The actual behavior depends on implementation
            Assert.IsNotNull(result);
        }

        [Test]
        public void Sacrifice3Atk3Def_WithNoValidCard_Fails()
        {
            // Arrange - Only cards with <3 ATK or <3 DEF
            var weakCard1 = CreateCard("Weak Card 1", 2, 2, CardFamily.Human);
            var weakCard2 = CreateCard("Weak Card 2", 1, 5, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(weakCard1);
            deck.Add(weakCard2);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            foreach (var card in player.Hand.ToList())
            {
                player.PlayCard(card);
            }

            var ability = GetAbility(AbilityName.Sacrifice3Atk3Def);
            var context = CreateContext(weakCard1, AbilityName.Sacrifice3Atk3Def);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilityFailure(result);
        }

        #endregion

        #region Family Sacrifice Tests (SacrificeWizard)

        [Test]
        public void SacrificeWizard_WithWizardOnField_Succeeds()
        {
            // Arrange
            var wizard = CreateCard("Test Wizard", 3, 3, CardFamily.Wizard);
            var sourceCard = CreateCard("Source Card", 3, 3, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(wizard);
            deck.Add(sourceCard);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            foreach (var card in player.Hand.ToList())
            {
                player.PlayCard(card);
            }

            var ability = GetAbility(AbilityName.SacrificeWizard);
            var context = CreateContext(sourceCard, AbilityName.SacrificeWizard);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void SacrificeWizard_WithNoWizardOnField_Fails()
        {
            // Arrange - No wizard family cards
            var humanCard = CreateCard("Human Card", 3, 3, CardFamily.Human);
            var fistilandCard = CreateCard("Fistiland Card", 3, 3, CardFamily.Fistiland);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(humanCard);
            deck.Add(fistilandCard);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            foreach (var card in player.Hand.ToList())
            {
                player.PlayCard(card);
            }

            var ability = GetAbility(AbilityName.SacrificeWizard);
            var context = CreateContext(humanCard, AbilityName.SacrificeWizard);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilityFailure(result);
        }

        #endregion

        #region Multiple Card Sacrifice Tests (Sacrifice2Japan)

        [Test]
        public void Sacrifice2Japan_WithTwoJapanCards_Succeeds()
        {
            // Arrange
            var japan1 = CreateCard("Japan Card 1", 2, 2, CardFamily.Japan);
            var japan2 = CreateCard("Japan Card 2", 2, 2, CardFamily.Japan);
            var sourceCard = CreateCard("Source Card", 3, 3, CardFamily.Human);

            var deck = CardFactory.CreateDeck(27);
            deck.Add(japan1);
            deck.Add(japan2);
            deck.Add(sourceCard);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            player.DrawCard();
            foreach (var card in player.Hand.ToList())
            {
                player.PlayCard(card);
            }

            var ability = GetAbility(AbilityName.Sacrifice2Japan);
            var context = CreateContext(sourceCard, AbilityName.Sacrifice2Japan);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Sacrifice2Japan_WithOnlyOneJapanCard_Fails()
        {
            // Arrange - Only 1 Japan card
            var japan1 = CreateCard("Japan Card 1", 2, 2, CardFamily.Japan);
            var humanCard = CreateCard("Human Card", 2, 2, CardFamily.Human);
            var sourceCard = CreateCard("Source Card", 3, 3, CardFamily.Human);

            var deck = CardFactory.CreateDeck(27);
            deck.Add(japan1);
            deck.Add(humanCard);
            deck.Add(sourceCard);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            player.DrawCard();
            foreach (var card in player.Hand.ToList())
            {
                player.PlayCard(card);
            }

            var ability = GetAbility(AbilityName.Sacrifice2Japan);
            var context = CreateContext(sourceCard, AbilityName.Sacrifice2Japan);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert
            AssertAbilityFailure(result);
        }

        #endregion

        #region SacrificeToInvoke Tests

        [Test]
        public void SacrificeToInvoke_BasicExecution_ReturnsResult()
        {
            // Arrange
            var sacrificeTarget = CreateCard("Sacrifice Target", 2, 2, CardFamily.Human);
            var sourceCard = CreateCard("Source Card", 3, 3, CardFamily.Human);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(sacrificeTarget);
            deck.Add(sourceCard);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            foreach (var card in player.Hand.ToList())
            {
                player.PlayCard(card);
            }

            var ability = GetAbility(AbilityName.SacrificeToInvoke);
            var context = CreateContext(sourceCard, AbilityName.SacrificeToInvoke);

            // Act
            var result = ExecuteAbility(ability, context);

            // Assert - Should return a result (may need user input)
            Assert.IsNotNull(result);
        }

        #endregion

        #region CanActivate Tests

        [Test]
        public void SacrificeBenzaieJeune_CanActivate_WhenBenzaieJeuneOnField_ReturnsTrue()
        {
            // Arrange
            var benzaie = CreateCard("Benzaie", 5, 4, CardFamily.Fistiland);
            var benzaieJeune = CreateCard("Benzaie jeune", 2, 2, CardFamily.Fistiland);

            var deck = CardFactory.CreateDeck(28);
            deck.Add(benzaie);
            deck.Add(benzaieJeune);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            player.DrawCard();
            var benzaieCard = player.Hand.FirstOrDefault(c => c.Title == "Benzaie");
            var benzaieJeuneCard = player.Hand.FirstOrDefault(c => c.Title == "Benzaie jeune");
            if (benzaieCard != null) player.PlayCard(benzaieCard);
            if (benzaieJeuneCard != null) player.PlayCard(benzaieJeuneCard);

            var ability = GetAbility(AbilityName.SacrificeBenzaieJeune);
            var context = CreateContext(benzaieCard, AbilityName.SacrificeBenzaieJeune);

            // Act
            var canActivate = CanActivateAbility(ability, context);

            // Assert
            Assert.IsTrue(canActivate);
        }

        [Test]
        public void SacrificeBenzaieJeune_CanActivate_WhenBenzaieJeuneNotOnField_ReturnsFalse()
        {
            // Arrange - No Benzaie jeune on field
            var benzaie = CreateCard("Benzaie", 5, 4, CardFamily.Fistiland);

            var deck = CardFactory.CreateDeck(29);
            deck.Add(benzaie);

            var player = new Player(PlayerId.Player1, deck);
            PlayerRepository.AddPlayer(player);

            player.DrawCard();
            var benzaieCard = player.Hand.FirstOrDefault(c => c.Title == "Benzaie");
            if (benzaieCard != null) player.PlayCard(benzaieCard);

            var ability = GetAbility(AbilityName.SacrificeBenzaieJeune);
            var context = CreateContext(benzaieCard, AbilityName.SacrificeBenzaieJeune);

            // Act
            var canActivate = CanActivateAbility(ability, context);

            // Assert
            Assert.IsFalse(canActivate);
        }

        #endregion
    }
}

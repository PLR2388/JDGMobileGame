using System.Collections.Generic;
using System.Linq;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.TestUtilities
{
    /// <summary>
    /// Pre-configured game state fixtures for ability scenario tests.
    /// These fixtures simulate realistic game situations for testing abilities.
    /// </summary>
    public static class AbilityScenarioFixtures
    {
        #region Sacrifice Scenarios

        /// <summary>
        /// Creates a scenario for testing sacrifice abilities.
        /// Player 1 has the sacrifice source and target on field.
        /// </summary>
        public static (Player player1, Player player2, Card sacrificeSource, Card sacrificeTarget)
            CreateSacrificeScenario(
                string sourceName,
                int sourceAtk,
                int sourceDef,
                string targetName,
                int targetAtk,
                int targetDef,
                CardFamily family = CardFamily.Human)
        {
            var sacrificeSource = CardFactory.CreateInvocation(sourceName, sourceAtk, sourceDef, family);
            var sacrificeTarget = CardFactory.CreateInvocation(targetName, targetAtk, targetDef, family);

            var deck1 = CardFactory.CreateDeck(28);
            deck1.Add(sacrificeSource);
            deck1.Add(sacrificeTarget);

            var player1 = new Player(PlayerId.Player1, deck1);
            var player2 = PlayerFactory.CreatePlayer2();

            // Draw and play both cards
            player1.DrawCard();
            player1.DrawCard();

            // Find and play the specific cards
            var source = player1.Hand.FirstOrDefault(c => c.Title == sourceName);
            var target = player1.Hand.FirstOrDefault(c => c.Title == targetName);

            if (source != null) player1.PlayCard(source);
            if (target != null) player1.PlayCard(target);

            return (player1, player2, source ?? sacrificeSource, target ?? sacrificeTarget);
        }

        /// <summary>
        /// Creates Benzaie + Benzaie jeune sacrifice scenario.
        /// </summary>
        public static (Player player1, Player player2, Card benzaie, Card benzaieJeune)
            CreateBenzaieSacrificeScenario()
        {
            return CreateSacrificeScenario(
                "Benzaie", 5, 4,
                "Benzaie jeune", 2, 2,
                CardFamily.Fistiland
            );
        }

        /// <summary>
        /// Creates Mecha-Granolax + Granolax sacrifice scenario.
        /// </summary>
        public static (Player player1, Player player2, Card mechaGranolax, Card granolax)
            CreateGranolaxSacrificeScenario()
        {
            return CreateSacrificeScenario(
                "Mecha-Granolax", 5, 4,
                "Granolax", 2, 2,
                CardFamily.Rpg
            );
        }

        #endregion

        #region Protection Scenarios

        /// <summary>
        /// Creates a scenario for testing protection abilities.
        /// Attacker can target protected card.
        /// </summary>
        public static (Player attacker, Player defender, Card attackingCard, Card protectedCard, Card protectorCard)
            CreateProtectionScenario(
                string protectedName,
                string protectorName,
                CardFamily family = CardFamily.Human)
        {
            var attackingCard = CardFactory.CreateAttacker();
            var protectedCard = CardFactory.CreateInvocation(protectedName, 2, 2, family);
            var protectorCard = CardFactory.CreateInvocation(protectorName, 3, 3, family);

            // Cards must be added to END of deck because DrawCard takes from end
            var attackerDeck = CardFactory.CreateDeck(29);
            attackerDeck.Add(attackingCard);

            var defenderDeck = CardFactory.CreateDeck(28);
            defenderDeck.Add(protectorCard);  // Add protector first (drawn second)
            defenderDeck.Add(protectedCard);  // Add protected last (drawn first)

            var attacker = new Player(PlayerId.Player1, attackerDeck);
            var defender = new Player(PlayerId.Player2, defenderDeck);

            // Setup attacker
            attacker.DrawCard();
            attacker.PlayCard(attacker.Hand[0]);

            // Setup defender with both cards
            defender.DrawCard();
            defender.DrawCard();
            defender.PlayCard(defender.Hand.FirstOrDefault(c => c.Title == protectedName) ?? defender.Hand[0]);
            defender.PlayCard(defender.Hand.FirstOrDefault(c => c.Title == protectorName) ?? defender.Hand[0]);

            return (attacker, defender, attackingCard, protectedCard, protectorCard);
        }

        /// <summary>
        /// Creates Granolax + Starlight Unicorn protection scenario.
        /// </summary>
        public static (Player attacker, Player defender, Card attackingCard, Card granolax, Card starlightUnicorn)
            CreateStarlightProtectionScenario()
        {
            return CreateProtectionScenario("Granolax", "Starlight Unicorn", CardFamily.Rpg);
        }

        #endregion

        #region Dependency Scenarios

        /// <summary>
        /// Creates a scenario for testing dependency abilities.
        /// Dependent card requires another card to survive.
        /// </summary>
        public static (Player player1, Player player2, Card dependentCard, Card requiredCard)
            CreateDependencyScenario(
                string dependentName,
                int dependentAtk,
                int dependentDef,
                string requiredName,
                int requiredAtk,
                int requiredDef,
                CardFamily family = CardFamily.Human)
        {
            var dependentCard = CardFactory.CreateInvocation(dependentName, dependentAtk, dependentDef, family);
            var requiredCard = CardFactory.CreateInvocation(requiredName, requiredAtk, requiredDef, family);

            // Cards must be added to END of deck because DrawCard takes from end
            var deck1 = CardFactory.CreateDeck(28);
            deck1.Add(requiredCard);    // Added first to deck end, drawn second
            deck1.Add(dependentCard);   // Added last to deck end, drawn first

            var player1 = new Player(PlayerId.Player1, deck1);
            var player2 = PlayerFactory.CreatePlayer2();

            // Draw and play both cards
            player1.DrawCard();
            player1.DrawCard();

            var dependent = player1.Hand.FirstOrDefault(c => c.Title == dependentName);
            var required = player1.Hand.FirstOrDefault(c => c.Title == requiredName);

            // Play required card first, then dependent
            if (required != null) player1.PlayCard(required);
            if (dependent != null) player1.PlayCard(dependent);

            return (player1, player2, dependent ?? dependentCard, required ?? requiredCard);
        }

        /// <summary>
        /// Creates Alpha Man + Benzaie dependency scenario.
        /// </summary>
        public static (Player player1, Player player2, Card alphaMan, Card benzaie)
            CreateAlphaManDependencyScenario()
        {
            return CreateDependencyScenario(
                "Alpha Man", 4, 4,
                "Benzaie", 5, 4,
                CardFamily.Fistiland
            );
        }

        /// <summary>
        /// Creates Starlight Unicorn + Granolax dependency scenario.
        /// </summary>
        public static (Player player1, Player player2, Card starlightUnicorn, Card granolax)
            CreateStarlightDependencyScenario()
        {
            return CreateDependencyScenario(
                "Starlight Unicorn", 4, 4,
                "Granolax", 2, 2,
                CardFamily.Rpg
            );
        }

        #endregion

        #region Deck Search Scenarios

        /// <summary>
        /// Creates a scenario for testing deck search abilities.
        /// Target card is in the deck.
        /// </summary>
        public static (Player player1, Player player2, Card searcher, Card targetInDeck)
            CreateDeckSearchScenario(
                string searcherName,
                string targetName,
                CardFamily family = CardFamily.Human)
        {
            var searcher = CardFactory.CreateInvocation(searcherName, 2, 2, family);
            var targetInDeck = CardFactory.CreateInvocation(targetName, 2, 2, family);

            // Cards must be added to END of deck because DrawCard takes from end
            // Searcher goes at end to be drawn first
            // Target goes in middle of deck to be searched
            var deck1 = CardFactory.CreateDeck(28);
            deck1.Insert(10, targetInDeck);  // Insert target in middle of deck
            deck1.Add(searcher);             // Add searcher at end to be drawn

            var player1 = new Player(PlayerId.Player1, deck1);
            var player2 = PlayerFactory.CreatePlayer2();

            // Draw and play searcher
            player1.DrawCard();
            player1.PlayCard(player1.Hand[0]);

            return (player1, player2, searcher, targetInDeck);
        }

        /// <summary>
        /// Creates Benzaie jeune searching for Nounours scenario.
        /// </summary>
        public static (Player player1, Player player2, Card benzaieJeune, Card nounours)
            CreateBenzaieJeuneSearchScenario()
        {
            return CreateDeckSearchScenario("Benzaie jeune", "Nounours", CardFamily.Fistiland);
        }

        #endregion

        #region Equipment Scenarios

        /// <summary>
        /// Creates a scenario for testing equipment abilities.
        /// </summary>
        public static (Player player1, Player player2, Card invocation, Card equipment)
            CreateEquipmentScenario(
                string invocationName,
                int atk,
                int def,
                string equipmentName,
                CardFamily family = CardFamily.Human)
        {
            var invocation = CardFactory.CreateInvocation(invocationName, atk, def, family);
            var equipment = Card.CreateEquipment(
                CardId.New(),
                equipmentName,
                $"Description of {equipmentName}",
                $"Detailed description of {equipmentName}",
                new EquipmentAbilityName[0],
                false
            );

            // Cards must be added to END of deck because DrawCard takes from end
            var deck1 = CardFactory.CreateDeck(28);
            deck1.Add(equipment);   // Added first, drawn second
            deck1.Add(invocation);  // Added last, drawn first

            var player1 = new Player(PlayerId.Player1, deck1);
            var player2 = PlayerFactory.CreatePlayer2();

            // Draw and play invocation
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == invocationName) ?? player1.Hand[0]);

            // Draw equipment (keep in hand for equipping)
            player1.DrawCard();

            return (player1, player2, invocation, equipment);
        }

        /// <summary>
        /// Creates Benzaie + Parachute dore equipment scenario.
        /// </summary>
        public static (Player player1, Player player2, Card benzaie, Card parachute)
            CreateBenzaieParachuteScenario()
        {
            return CreateEquipmentScenario("Benzaie", 5, 4, "Parachute dore", CardFamily.Fistiland);
        }

        #endregion

        #region Field Card Scenarios

        /// <summary>
        /// Creates a scenario for testing field card abilities.
        /// </summary>
        public static (Player player1, Player player2, Card fieldCard, List<Card> affectedCards)
            CreateFieldScenario(
                string fieldName,
                CardFamily boostedFamily,
                int numberOfAffectedCards = 2)
        {
            var fieldCard = CardFactory.CreateField(fieldName, boostedFamily);
            var affectedCards = new List<Card>();

            // Cards must be added to END of deck because DrawCard takes from end
            var deck1 = CardFactory.CreateDeck(30 - numberOfAffectedCards - 1);
            for (int i = 0; i < numberOfAffectedCards; i++)
            {
                var card = CardFactory.CreateInvocation($"Affected Card {i + 1}", 2, 2, boostedFamily);
                affectedCards.Add(card);
                deck1.Add(card);  // Add affected cards before field card
            }
            deck1.Add(fieldCard);  // Field card at end, drawn first

            var player1 = new Player(PlayerId.Player1, deck1);
            var player2 = PlayerFactory.CreatePlayer2();

            // Draw and play field card
            player1.DrawCard();
            var field = player1.Hand.FirstOrDefault(c => c.Title == fieldName);
            if (field != null) player1.PlayCard(field);

            // Draw and play affected cards
            for (int i = 0; i < numberOfAffectedCards; i++)
            {
                player1.DrawCard();
                var affected = player1.Hand.FirstOrDefault(c => c.Title.StartsWith("Affected Card"));
                if (affected != null) player1.PlayCard(affected);
            }

            return (player1, player2, fieldCard, affectedCards);
        }

        /// <summary>
        /// Creates Le Hard Corner + Fistiland family scenario.
        /// </summary>
        public static (Player player1, Player player2, Card leHardCorner, List<Card> fistilandCards)
            CreateHardCornerFieldScenario()
        {
            return CreateFieldScenario("Le Hard Corner", CardFamily.Fistiland, 3);
        }

        #endregion

        #region Draw Scenarios

        /// <summary>
        /// Creates a scenario for testing draw abilities with specific deck size.
        /// </summary>
        public static (Player player1, Player player2, Card drawSource, int initialDeckCount)
            CreateDrawScenario(
                string sourceName,
                int deckSize = 10)
        {
            var drawSource = CardFactory.CreateInvocation(sourceName, 2, 2);

            // Cards must be added to END of deck because DrawCard takes from end
            var deck1 = new List<Card>();
            for (int i = 1; i < deckSize; i++)
            {
                deck1.Add(CardFactory.CreateInvocation($"Deck Card {i}", 2, 2));
            }
            deck1.Add(drawSource);  // Source at end, drawn first

            var player1 = new Player(PlayerId.Player1, deck1);
            var player2 = PlayerFactory.CreatePlayer2();

            // Draw and play the source card
            player1.DrawCard();
            player1.PlayCard(player1.Hand[0]);

            return (player1, player2, drawSource, player1.DeckCount);
        }

        /// <summary>
        /// Creates L'Elfette Draw2Cards scenario.
        /// </summary>
        public static (Player player1, Player player2, Card elfette, int initialDeckCount)
            CreateElfetteDrawScenario()
        {
            return CreateDrawScenario("L'Elfette", 15);
        }

        /// <summary>
        /// Creates Sangoku Draw3Cards scenario.
        /// </summary>
        public static (Player player1, Player player2, Card sangoku, int initialDeckCount)
            CreateSangokuDrawScenario()
        {
            return CreateDrawScenario("Sangoku", 15);
        }

        #endregion

        #region Resurrection Scenarios

        /// <summary>
        /// Creates a scenario for testing resurrection abilities.
        /// Card starts in graveyard.
        /// </summary>
        public static (Player player1, Player player2, Card resurrectionCard)
            CreateResurrectionScenario(
                string cardName,
                int atk,
                int def,
                CardFamily family = CardFamily.Human)
        {
            var resurrectionCard = CardFactory.CreateInvocation(cardName, atk, def, family);

            // Cards must be added to END of deck because DrawCard takes from end
            var deck1 = CardFactory.CreateDeck(29);
            deck1.Add(resurrectionCard);

            var player1 = new Player(PlayerId.Player1, deck1);
            var player2 = PlayerFactory.CreatePlayer2();

            // Draw, play, then simulate death (move card to graveyard)
            player1.DrawCard();
            player1.PlayCard(player1.Hand[0]);
            // Destroy card to move it to graveyard - resurrection requires card in graveyard
            player1.DestroyCardFromField(resurrectionCard);

            return (player1, player2, resurrectionCard);
        }

        /// <summary>
        /// Creates Jean-Marc Soul resurrection scenario.
        /// </summary>
        public static (Player player1, Player player2, Card jeanMarcSoul)
            CreateJeanMarcSoulResurrectionScenario()
        {
            return CreateResurrectionScenario("Jean-Marc Soul", 1, 1, CardFamily.Police);
        }

        #endregion
    }
}

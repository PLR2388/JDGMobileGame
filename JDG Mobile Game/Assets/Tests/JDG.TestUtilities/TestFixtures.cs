using System.Collections.Generic;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.TestUtilities
{
    /// <summary>
    /// Factory methods for creating test cards with sensible defaults.
    /// Use these to reduce boilerplate in test setup.
    /// </summary>
    public static class CardFactory
    {
        /// <summary>
        /// Creates a basic invocation card with specified stats.
        /// </summary>
        public static Card CreateInvocation(
            string name = "Test Invocation",
            int attack = 3,
            int defense = 3,
            CardFamily family = CardFamily.Human,
            bool isShiny = false,
            CardId? id = null)
        {
            return Card.CreateInvocation(
                id ?? CardId.New(),
                name,
                $"Description of {name}",
                "TestMaterial",
                attack,
                defense,
                new[] { family },
                isShiny
            );
        }

        /// <summary>
        /// Creates an attacker card with high ATK.
        /// </summary>
        public static Card CreateAttacker(int attack = 5, int defense = 2)
        {
            return CreateInvocation("Attacker", attack, defense);
        }

        /// <summary>
        /// Creates a defender card with high DEF.
        /// </summary>
        public static Card CreateDefender(int attack = 2, int defense = 5)
        {
            return CreateInvocation("Defender", attack, defense);
        }

        /// <summary>
        /// Creates a strong card for testing edge cases.
        /// </summary>
        public static Card CreateStrongCard(int attack = 5, int defense = 5)
        {
            return CreateInvocation("Strong Card", attack, defense);
        }

        /// <summary>
        /// Creates a weak card for testing edge cases.
        /// </summary>
        public static Card CreateWeakCard(int attack = 1, int defense = 1)
        {
            return CreateInvocation("Weak Card", attack, defense);
        }

        /// <summary>
        /// Creates an effect card for testing.
        /// </summary>
        public static Card CreateEffect(
            string name = "Test Effect",
            EffectAbilityName[] abilities = null)
        {
            return Card.CreateEffect(
                CardId.New(),
                name,
                $"Description of {name}",
                "TestMaterial",
                abilities ?? new EffectAbilityName[0]
            );
        }

        /// <summary>
        /// Creates a field card for testing.
        /// </summary>
        public static Card CreateField(
            string name = "Test Field",
            CardFamily family = CardFamily.Human)
        {
            return Card.CreateField(
                CardId.New(),
                name,
                $"Description of {name}",
                "TestMaterial",
                family,
                new FieldAbilityName[0]
            );
        }

        /// <summary>
        /// Creates multiple invocation cards for deck testing.
        /// </summary>
        public static List<Card> CreateDeck(int count = 30)
        {
            var deck = new List<Card>();
            for (int i = 0; i < count; i++)
            {
                deck.Add(CreateInvocation($"Card {i + 1}", attack: 2 + (i % 4), defense: 2 + (i % 4)));
            }
            return deck;
        }
    }

    /// <summary>
    /// Factory methods for creating test players with sensible defaults.
    /// </summary>
    public static class PlayerFactory
    {
        /// <summary>
        /// Creates a player with a deck of cards.
        /// </summary>
        public static Player CreatePlayer(
            PlayerId playerId,
            int deckSize = 30,
            int maxHealth = 30)
        {
            var deck = CardFactory.CreateDeck(deckSize);
            return new Player(playerId, deck, maxHealth);
        }

        /// <summary>
        /// Creates Player 1 with default settings.
        /// </summary>
        public static Player CreatePlayer1(int deckSize = 30, int maxHealth = 30)
        {
            return CreatePlayer(PlayerId.Player1, deckSize, maxHealth);
        }

        /// <summary>
        /// Creates Player 2 with default settings.
        /// </summary>
        public static Player CreatePlayer2(int deckSize = 30, int maxHealth = 30)
        {
            return CreatePlayer(PlayerId.Player2, deckSize, maxHealth);
        }

        /// <summary>
        /// Creates a player with specific cards.
        /// </summary>
        public static Player CreatePlayerWithCards(PlayerId playerId, List<Card> cards)
        {
            return new Player(playerId, cards);
        }

        /// <summary>
        /// Creates a player with cards already on the field.
        /// </summary>
        public static Player CreatePlayerWithFieldCards(
            PlayerId playerId,
            int fieldCardCount = 2,
            int deckSize = 30)
        {
            var player = CreatePlayer(playerId, deckSize);

            // Draw and play cards to field
            for (int i = 0; i < fieldCardCount && player.Deck.Count > 0; i++)
            {
                player.DrawCard();
                if (player.Hand.Count > 0)
                {
                    player.PlayCard(player.Hand[0]);
                }
            }

            return player;
        }
    }

    /// <summary>
    /// Pre-configured game state fixtures for common test scenarios.
    /// </summary>
    public static class GameStateFixtures
    {
        /// <summary>
        /// Creates a basic game setup with two players, each with a full deck.
        /// </summary>
        public static (Player player1, Player player2) CreateBasicGameSetup()
        {
            return (PlayerFactory.CreatePlayer1(), PlayerFactory.CreatePlayer2());
        }

        /// <summary>
        /// Creates a game state ready for combat testing.
        /// Both players have cards on the field.
        /// </summary>
        public static (Player player1, Player player2, Card attacker, Card defender) CreateCombatSetup()
        {
            var attacker = CardFactory.CreateAttacker();
            var defender = CardFactory.CreateDefender();

            var player1 = PlayerFactory.CreatePlayerWithCards(
                PlayerId.Player1,
                new List<Card> { attacker }
            );
            var player2 = PlayerFactory.CreatePlayerWithCards(
                PlayerId.Player2,
                new List<Card> { defender }
            );

            // Draw and play cards
            player1.DrawCard();
            player2.DrawCard();
            player1.PlayCard(attacker);
            player2.PlayCard(defender);

            return (player1, player2, attacker, defender);
        }

        /// <summary>
        /// Creates a game state for testing direct attacks.
        /// Player 1 has cards, Player 2 has empty field.
        /// </summary>
        public static (Player player1, Player player2, Card attacker) CreateDirectAttackSetup()
        {
            var attacker = CardFactory.CreateAttacker();

            var player1 = PlayerFactory.CreatePlayerWithCards(
                PlayerId.Player1,
                new List<Card> { attacker }
            );
            var player2 = PlayerFactory.CreatePlayer2(deckSize: 10);

            // Draw and play attacker
            player1.DrawCard();
            player1.PlayCard(attacker);

            return (player1, player2, attacker);
        }

        /// <summary>
        /// Creates a game state for testing card drawing.
        /// </summary>
        public static (Player player, List<Card> expectedDraws) CreateDrawSetup(int deckSize = 5)
        {
            var player = PlayerFactory.CreatePlayer1(deckSize);
            var expectedDraws = new List<Card>(player.Deck);
            return (player, expectedDraws);
        }
    }
}

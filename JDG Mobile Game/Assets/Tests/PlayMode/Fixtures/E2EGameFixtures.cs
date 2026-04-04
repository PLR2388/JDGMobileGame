using System.Collections.Generic;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.PlayMode.Tests.Controllers;

namespace JDG.PlayMode.Tests.Fixtures
{
    /// <summary>
    /// Factory methods for creating E2E game configurations and test cards.
    /// </summary>
    public static class E2EGameFixtures
    {
        #region Game Configurations

        /// <summary>
        /// Creates a default game configuration for basic testing.
        /// Both players start with 30 HP, 30 cards in deck, 5 cards in hand.
        /// </summary>
        public static E2EGameConfig DefaultGame()
        {
            return new E2EGameConfig
            {
                Player1DeckSize = 30,
                Player2DeckSize = 30,
                Player1StartHealth = 30f,
                Player2StartHealth = 30f,
                Player1StartHandSize = 5,
                Player2StartHandSize = 5,
                StartAtTurn1 = true,
                StartingPlayer = PlayerId.Player1
            };
        }

        /// <summary>
        /// Creates a game configuration for combat testing.
        /// Standard setup with both players ready to fight.
        /// </summary>
        public static E2EGameConfig BasicCombatGame()
        {
            return new E2EGameConfig
            {
                Player1DeckSize = 20,
                Player2DeckSize = 20,
                Player1StartHealth = 30f,
                Player2StartHealth = 30f,
                Player1StartHandSize = 5,
                Player2StartHandSize = 5,
                StartAtTurn1 = true,
                StartingPlayer = PlayerId.Player1
            };
        }

        /// <summary>
        /// Creates a game configuration for testing direct attacks.
        /// Player 2 has no blockers, allowing direct player damage.
        /// </summary>
        public static E2EGameConfig DirectAttackGame()
        {
            return new E2EGameConfig
            {
                Player1DeckSize = 20,
                Player2DeckSize = 20,
                Player1StartHealth = 30f,
                Player2StartHealth = 30f,
                Player1StartHandSize = 5,
                Player2StartHandSize = 0, // No hand cards = no blockers
                StartAtTurn1 = true,
                StartingPlayer = PlayerId.Player1
            };
        }

        /// <summary>
        /// Creates a game configuration for testing deck depletion.
        /// One player has a very small deck to trigger empty deck loss condition.
        /// </summary>
        public static E2EGameConfig DeckDepletionGame()
        {
            return new E2EGameConfig
            {
                Player1DeckSize = 30,
                Player2DeckSize = 3, // Will run out quickly
                Player1StartHealth = 30f,
                Player2StartHealth = 30f,
                Player1StartHandSize = 5,
                Player2StartHandSize = 5,
                StartAtTurn1 = true,
                StartingPlayer = PlayerId.Player1
            };
        }

        /// <summary>
        /// Creates a game starting on Player 2's turn.
        /// Useful for testing scenarios where Player 2 needs to act first.
        /// </summary>
        public static E2EGameConfig Player2StartsGame()
        {
            return new E2EGameConfig
            {
                Player1DeckSize = 30,
                Player2DeckSize = 30,
                Player1StartHealth = 30f,
                Player2StartHealth = 30f,
                Player1StartHandSize = 5,
                Player2StartHandSize = 5,
                StartAtTurn1 = true,
                StartingPlayer = PlayerId.Player2
            };
        }

        /// <summary>
        /// Creates a game with low health for both players.
        /// Useful for testing win/lose conditions quickly.
        /// </summary>
        public static E2EGameConfig LowHealthGame()
        {
            return new E2EGameConfig
            {
                Player1DeckSize = 30,
                Player2DeckSize = 30,
                Player1StartHealth = 10f,
                Player2StartHealth = 10f,
                Player1StartHandSize = 5,
                Player2StartHandSize = 5,
                StartAtTurn1 = true,
                StartingPlayer = PlayerId.Player1
            };
        }

        #endregion

        #region Card Factories

        /// <summary>
        /// Creates a basic attacker card configuration.
        /// </summary>
        public static TestInvocationCardConfig CreateAttacker(string title, float atk, float def)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                CanDirectAttack = false,
                CantBeAttacked = false,
                HasAggro = false
            };
        }

        /// <summary>
        /// Creates a defender card with higher defense than attack.
        /// </summary>
        public static TestInvocationCardConfig CreateDefender(string title, float atk, float def)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                CanDirectAttack = false,
                CantBeAttacked = false,
                HasAggro = false
            };
        }

        /// <summary>
        /// Creates a card that can attack the player directly.
        /// </summary>
        public static TestInvocationCardConfig CreateDirectAttacker(string title, float atk, float def)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                CanDirectAttack = true,
                CantBeAttacked = false,
                HasAggro = false
            };
        }

        /// <summary>
        /// Creates a protected card that can't be attacked.
        /// </summary>
        public static TestInvocationCardConfig CreateProtectedCard(string title, float atk, float def)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                CanDirectAttack = false,
                CantBeAttacked = true,
                HasAggro = false
            };
        }

        /// <summary>
        /// Creates a provoke/aggro card that must be attacked first.
        /// </summary>
        public static TestInvocationCardConfig CreateAggroCard(string title, float atk, float def)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                CanDirectAttack = false,
                CantBeAttacked = false,
                HasAggro = true
            };
        }

        /// <summary>
        /// Creates a card with a specific ability.
        /// </summary>
        public static TestInvocationCardConfig CreateWithAbility(string title, float atk, float def, AbilityName ability)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                CanDirectAttack = false,
                CantBeAttacked = false,
                HasAggro = false,
                Abilities = new List<AbilityName> { ability }
            };
        }

        /// <summary>
        /// Creates a card with multiple abilities.
        /// </summary>
        public static TestInvocationCardConfig CreateWithAbilities(string title, float atk, float def, params AbilityName[] abilities)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                CanDirectAttack = false,
                CantBeAttacked = false,
                HasAggro = false,
                Abilities = new List<AbilityName>(abilities)
            };
        }

        #endregion

        #region Common Test Cards

        /// <summary>
        /// Standard weak attacker (3/3).
        /// </summary>
        public static TestInvocationCardConfig WeakAttacker => CreateAttacker("WeakAttacker", 3f, 3f);

        /// <summary>
        /// Standard medium attacker (5/4).
        /// </summary>
        public static TestInvocationCardConfig MediumAttacker => CreateAttacker("MediumAttacker", 5f, 4f);

        /// <summary>
        /// Standard strong attacker (7/5).
        /// </summary>
        public static TestInvocationCardConfig StrongAttacker => CreateAttacker("StrongAttacker", 7f, 5f);

        /// <summary>
        /// Standard weak defender (2/5).
        /// </summary>
        public static TestInvocationCardConfig WeakDefender => CreateDefender("WeakDefender", 2f, 5f);

        /// <summary>
        /// Standard strong defender (3/8).
        /// </summary>
        public static TestInvocationCardConfig StrongDefender => CreateDefender("StrongDefender", 3f, 8f);

        /// <summary>
        /// Card that can attack player directly (4/3).
        /// </summary>
        public static TestInvocationCardConfig DirectAttackerCard => CreateDirectAttacker("DirectAttacker", 4f, 3f);

        /// <summary>
        /// Protected card that can't be attacked (6/6).
        /// </summary>
        public static TestInvocationCardConfig ProtectedCard => CreateProtectedCard("ProtectedCard", 6f, 6f);

        /// <summary>
        /// Aggro card that must be attacked first (4/4).
        /// </summary>
        public static TestInvocationCardConfig AggroCard => CreateAggroCard("AggroCard", 4f, 4f);

        #endregion

        #region Preset Field Configurations

        /// <summary>
        /// Sets up a basic combat scenario with one attacker vs one defender.
        /// Uses synchronous methods to place cards immediately.
        /// </summary>
        /// <param name="simulator">The player action simulator.</param>
        public static void SetupBasicCombat(PlayerActionSimulator simulator)
        {
            // Player 1: One medium attacker
            simulator.PlayInvocationCardSync(MediumAttacker, isPlayer1: true);

            // Player 2: One weak defender
            simulator.PlayInvocationCardSync(WeakDefender, isPlayer1: false);

            // Setup player entities for direct attacks
            simulator.SetupPlayerEntity("Player1Entity", isPlayer1: true);
            simulator.SetupPlayerEntity("Player2Entity", isPlayer1: false);
        }

        /// <summary>
        /// Sets up a scenario for testing direct attacks (empty opponent field).
        /// Uses synchronous methods to place cards immediately.
        /// </summary>
        /// <param name="simulator">The player action simulator.</param>
        public static void SetupDirectAttackScenario(PlayerActionSimulator simulator)
        {
            // Player 1: One strong attacker
            simulator.PlayInvocationCardSync(StrongAttacker, isPlayer1: true);

            // Player 2: No invocation cards, just player entity
            simulator.SetupPlayerEntity("Player1Entity", isPlayer1: true);
            simulator.SetupPlayerEntity("Player2Entity", isPlayer1: false);
        }

        /// <summary>
        /// Sets up a scenario with aggro card that must be attacked first.
        /// Uses synchronous methods to place cards immediately.
        /// </summary>
        /// <param name="simulator">The player action simulator.</param>
        public static void SetupAggroScenario(PlayerActionSimulator simulator)
        {
            // Player 1: One attacker
            simulator.PlayInvocationCardSync(MediumAttacker, isPlayer1: true);

            // Player 2: Aggro card + regular card
            simulator.PlayInvocationCardSync(AggroCard, isPlayer1: false);
            simulator.PlayInvocationCardSync(WeakDefender, isPlayer1: false);
            simulator.SetupPlayerEntity("Player2Entity", isPlayer1: false);
        }

        /// <summary>
        /// Sets up a scenario with protected cards.
        /// Uses synchronous methods to place cards immediately.
        /// </summary>
        /// <param name="simulator">The player action simulator.</param>
        public static void SetupProtectionScenario(PlayerActionSimulator simulator)
        {
            // Player 1: One attacker
            simulator.PlayInvocationCardSync(StrongAttacker, isPlayer1: true);

            // Player 2: Only protected cards
            simulator.PlayInvocationCardSync(ProtectedCard, isPlayer1: false);
            simulator.SetupPlayerEntity("Player2Entity", isPlayer1: false);
        }

        /// <summary>
        /// Sets up multiple cards on both sides for complex combat testing.
        /// Uses synchronous methods to place cards immediately.
        /// </summary>
        /// <param name="simulator">The player action simulator.</param>
        public static void SetupFullFieldScenario(PlayerActionSimulator simulator)
        {
            // Player 1: 3 cards
            simulator.PlayInvocationCardSync(WeakAttacker, isPlayer1: true);
            simulator.PlayInvocationCardSync(MediumAttacker, isPlayer1: true);
            simulator.PlayInvocationCardSync(StrongAttacker, isPlayer1: true);

            // Player 2: 3 cards
            simulator.PlayInvocationCardSync(WeakDefender, isPlayer1: false);
            simulator.PlayInvocationCardSync(CreateDefender("MediumDefender", 4f, 6f), isPlayer1: false);
            simulator.PlayInvocationCardSync(StrongDefender, isPlayer1: false);

            // Setup player entities
            simulator.SetupPlayerEntity("Player1Entity", isPlayer1: true);
            simulator.SetupPlayerEntity("Player2Entity", isPlayer1: false);
        }

        #endregion
    }
}

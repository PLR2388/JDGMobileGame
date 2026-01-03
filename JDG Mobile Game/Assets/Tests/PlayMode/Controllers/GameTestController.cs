using System;
using System.Collections;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using JDG.PlayMode.Tests.Assertions;
using JDG.PlayMode.Tests.TestHelpers;
using UnityEngine;

namespace JDG.PlayMode.Tests.Controllers
{
    /// <summary>
    /// Configuration for setting up a test game.
    /// </summary>
    public class E2EGameConfig
    {
        public int Player1DeckSize { get; set; } = 30;
        public int Player2DeckSize { get; set; } = 30;
        public float Player1StartHealth { get; set; } = 30f;
        public float Player2StartHealth { get; set; } = 30f;
        public int Player1StartHandSize { get; set; } = 5;
        public int Player2StartHandSize { get; set; } = 5;
        public bool StartAtTurn1 { get; set; } = true;
        public PlayerId StartingPlayer { get; set; } = PlayerId.Player1;
    }

    /// <summary>
    /// Represents a set of actions for a simulated turn.
    /// </summary>
    public class TurnActions
    {
        public bool DrawCard { get; set; } = true;
        public bool EndTurn { get; set; } = true;

        public static TurnActions PassTurn => new TurnActions { DrawCard = true, EndTurn = true };
        public static TurnActions DrawOnly => new TurnActions { DrawCard = true, EndTurn = false };
    }

    /// <summary>
    /// Controls game simulation for E2E tests.
    /// Provides methods to advance game state and simulate player actions.
    /// </summary>
    public class GameTestController : IDisposable
    {
        #region Dependencies

        private GameStateService _gameStateService;
        private TestEventBus _eventBus;
        private TestCardCollectionService _cardCollectionService;
        private TestPlayerStatusProvider _playerStatusProvider;
        private TestGameStateRepository _gameStateRepository;
        private ICombatLogic _combatLogic;

        private GameObject _testContainer;
        private bool _isInitialized;

        #endregion

        #region Public Properties

        /// <summary>
        /// The game state service for this test.
        /// </summary>
        public GameStateService GameStateService => _gameStateService;

        /// <summary>
        /// The test event bus capturing all published events.
        /// </summary>
        public TestEventBus EventBus => _eventBus;

        /// <summary>
        /// The card collection service for accessing player cards.
        /// </summary>
        public TestCardCollectionService CardCollectionService => _cardCollectionService;

        /// <summary>
        /// The player status provider for health/shields.
        /// </summary>
        public TestPlayerStatusProvider PlayerStatusProvider => _playerStatusProvider;

        /// <summary>
        /// Combat logic for damage calculations.
        /// </summary>
        public ICombatLogic CombatLogic => _combatLogic;

        /// <summary>
        /// Whether the controller has been initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new GameTestController. Call Initialize() before use.
        /// </summary>
        public GameTestController()
        {
        }

        /// <summary>
        /// Initializes the controller with test dependencies.
        /// Creates a fresh game state for testing.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("GameTestController already initialized. Call Dispose() first to reinitialize.");
                return;
            }

            _testContainer = new GameObject("E2E_TestContainer");

            // Create test dependencies
            _eventBus = new TestEventBus();
            _gameStateRepository = new TestGameStateRepository();
            _gameStateService = new GameStateService(_gameStateRepository, _eventBus);
            _cardCollectionService = new TestCardCollectionService();
            _playerStatusProvider = new TestPlayerStatusProvider();
            _combatLogic = new CombatLogic();

            _isInitialized = true;
        }

        /// <summary>
        /// Sets up the game with the given configuration.
        /// </summary>
        /// <param name="config">Game configuration.</param>
        public void SetupGame(E2EGameConfig config)
        {
            EnsureInitialized();

            // Setup player 1 cards
            var p1Cards = _cardCollectionService.CurrentPlayerCards;
            SetupPlayerCards(p1Cards, config.Player1DeckSize, config.Player1StartHandSize);

            // Setup player 2 cards
            var p2Cards = _cardCollectionService.OpponentPlayerCards;
            SetupPlayerCards(p2Cards, config.Player2DeckSize, config.Player2StartHandSize);

            // Setup player status
            _playerStatusProvider.CurrentPlayerStatus.CurrentHealth = config.Player1StartHealth;
            _playerStatusProvider.OpponentPlayerStatus.CurrentHealth = config.Player2StartHealth;

            // Start the game
            if (config.StartAtTurn1)
            {
                _gameStateService.StartNewTurn();
            }

            // Switch to starting player if needed
            if (config.StartingPlayer == PlayerId.Player2 && _gameStateService.CurrentPlayer == PlayerId.Player1)
            {
                _gameStateService.HandleEndTurn();
            }
        }

        private void SetupPlayerCards(TestPlayerCards cards, int deckSize, int handSize)
        {
            // Clear existing cards
            cards.Deck.Clear();
            cards.HandCards.Clear();
            cards.InvocationCards.Clear();
            cards.EffectCards.Clear();
            cards.YellowCards.Clear();

            // We don't create actual InGameCard instances here since they require ScriptableObjects.
            // Instead, we just set up the counts. Tests that need actual cards should use
            // the PlayerActionSimulator or load real cards from Resources.
        }

        #endregion

        #region Phase Control

        /// <summary>
        /// Advances the game to the target phase.
        /// Automatically progresses through intermediate phases.
        /// </summary>
        /// <param name="targetPhase">The phase to advance to.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator AdvanceToPhase(Phase targetPhase)
        {
            EnsureInitialized();

            int maxIterations = 10;
            int iterations = 0;

            while (_gameStateService.CurrentPhase != targetPhase && iterations < maxIterations)
            {
                _gameStateService.NextPhase();
                yield return null;
                iterations++;
            }

            if (_gameStateService.CurrentPhase != targetPhase)
            {
                throw new InvalidOperationException(
                    $"Failed to advance to phase {targetPhase}. Current phase: {_gameStateService.CurrentPhase}");
            }
        }

        /// <summary>
        /// Simulates a turn with the given actions.
        /// </summary>
        /// <param name="actions">Actions to perform during the turn.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator SimulateTurn(TurnActions actions)
        {
            EnsureInitialized();

            // Draw phase - just advance
            if (_gameStateService.CurrentPhase == Phase.Draw)
            {
                _gameStateService.NextPhase(); // -> Choose
                yield return null;
            }

            // Choose phase - advance to attack (or end if skipped)
            if (_gameStateService.CurrentPhase == Phase.Choose)
            {
                _gameStateService.NextPhase(); // -> Attack (or End if skipped)
                yield return null;
            }

            // Attack phase - advance to end
            if (_gameStateService.CurrentPhase == Phase.Attack)
            {
                _gameStateService.NextPhase(); // -> End
                yield return null;
            }

            // End turn if requested
            if (actions.EndTurn && _gameStateService.CurrentPhase == Phase.End)
            {
                yield return EndTurn();
            }
        }

        /// <summary>
        /// Ends the current turn and switches to the next player.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator EndTurn()
        {
            EnsureInitialized();

            // Advance to End phase if not there
            while (_gameStateService.CurrentPhase != Phase.End && _gameStateService.CurrentPhase != Phase.GameOver)
            {
                _gameStateService.NextPhase();
                yield return null;
            }

            // Handle end of turn (switches player, starts new turn)
            if (_gameStateService.CurrentPhase == Phase.End)
            {
                _gameStateService.HandleEndTurn();
                yield return null;
            }
        }

        /// <summary>
        /// Waits until the game reaches the specified phase or times out.
        /// </summary>
        /// <param name="phase">The phase to wait for.</param>
        /// <param name="timeout">Maximum time to wait in seconds.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator WaitForPhase(Phase phase, float timeout = 5f)
        {
            EnsureInitialized();

            yield return TestAwaiters.WaitForCondition(
                () => _gameStateService.CurrentPhase == phase,
                timeout,
                $"Timeout waiting for phase {phase}. Current phase: {_gameStateService.CurrentPhase}"
            );
        }

        #endregion

        #region State Capture

        /// <summary>
        /// Captures the current game state as a snapshot.
        /// Useful for debugging test failures.
        /// </summary>
        /// <returns>A snapshot of the current game state.</returns>
        public GameStateSnapshot CaptureState()
        {
            EnsureInitialized();

            return GameStateSnapshot.Capture(
                _gameStateService,
                _playerStatusProvider,
                _cardCollectionService,
                _eventBus
            );
        }

        /// <summary>
        /// Logs the current game state to the console.
        /// </summary>
        public void LogCurrentState()
        {
            var snapshot = CaptureState();
            Debug.Log(snapshot.ToDetailedString());
        }

        #endregion

        #region Combat Helpers

        /// <summary>
        /// Computes damage using the combat logic.
        /// </summary>
        /// <param name="attackerAtk">Attacker's attack value.</param>
        /// <param name="defenderDef">Defender's defense value.</param>
        /// <returns>Damage result (negative = defender dies, positive = attacker dies).</returns>
        public float ComputeDamage(float attackerAtk, float defenderDef)
        {
            return _combatLogic.ComputeDamage(attackerAtk, defenderDef);
        }

        /// <summary>
        /// Deals damage to the current player.
        /// </summary>
        /// <param name="damage">Amount of damage (negative value).</param>
        public void DealDamageToCurrentPlayer(float damage)
        {
            _playerStatusProvider.CurrentPlayerStatus.ChangePv(damage);
        }

        /// <summary>
        /// Deals damage to the opponent player.
        /// </summary>
        /// <param name="damage">Amount of damage (negative value).</param>
        public void DealDamageToOpponent(float damage)
        {
            _playerStatusProvider.OpponentPlayerStatus.ChangePv(damage);
        }

        #endregion

        #region Cleanup

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException(
                    "GameTestController is not initialized. Call Initialize() first.");
            }
        }

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        public void Dispose()
        {
            if (_testContainer != null)
            {
                UnityEngine.Object.DestroyImmediate(_testContainer);
                _testContainer = null;
            }

            _gameStateService = null;
            _eventBus = null;
            _cardCollectionService = null;
            _playerStatusProvider = null;
            _gameStateRepository = null;
            _combatLogic = null;
            _isInitialized = false;
        }

        #endregion
    }

    /// <summary>
    /// Test implementation of IGameStateRepository for E2E tests.
    /// </summary>
    public class TestGameStateRepository : IGameStateRepository
    {
        private Phase _currentPhase = Phase.Draw;
        private int _turnNumber = 0;
        private PlayerId _currentPlayer = PlayerId.Player1;
        private bool _isGameOver = false;

        public Phase CurrentPhase => _currentPhase;
        public void SetPhase(Phase phase) => _currentPhase = phase;

        public int TurnNumber => _turnNumber;
        public void IncrementTurn() => _turnNumber++;

        public PlayerId CurrentPlayer => _currentPlayer;
        public void SetCurrentPlayer(PlayerId player) => _currentPlayer = player;

        public void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        }

        public void ResetGameState()
        {
            _currentPhase = Phase.Draw;
            _turnNumber = 0;
            _currentPlayer = PlayerId.Player1;
            _isGameOver = false;
        }

        public bool IsGameOver => _isGameOver;
        public void SetGameOver(bool isGameOver) => _isGameOver = isGameOver;
    }
}

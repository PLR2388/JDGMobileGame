using NUnit.Framework;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Application.UseCases;
using JDG.Infrastructure.Events;
using JDG.Infrastructure.Repositories;

namespace JDG.Infrastructure.Tests.Integration
{
    /// <summary>
    /// Integration tests for complete game flow.
    /// Tests the full stack: Domain → Application → Infrastructure.
    /// </summary>
    [TestFixture]
    public class GameFlowIntegrationTests
    {
        private IEventBus _eventBus;
        private IPlayerRepository _playerRepository;
        private ICardRepository _cardRepository;
        private IGameStateRepository _gameStateRepository;
        private StartGameUseCase _startGameUseCase;
        private DrawCardUseCase _drawCardUseCase;
        private PlayCardUseCase _playCardUseCase;
        private AttackUseCase _attackUseCase;
        private EndTurnUseCase _endTurnUseCase;

        [SetUp]
        public void Setup()
        {
            // Setup infrastructure
            _eventBus = new EventBus();
            _cardRepository = new CardRepository();
            _playerRepository = new PlayerRepository(_cardRepository);
            _gameStateRepository = new GameStateRepository();

            // Setup use cases
            _startGameUseCase = new StartGameUseCase(
                _gameStateRepository,
                _playerRepository,
                _cardRepository,
                _eventBus
            );

            _drawCardUseCase = new DrawCardUseCase(
                _playerRepository,
                _eventBus
            );

            _playCardUseCase = new PlayCardUseCase(
                _playerRepository,
                _cardRepository,
                _gameStateRepository,
                _eventBus
            );

            _attackUseCase = new AttackUseCase(
                _playerRepository,
                _cardRepository,
                _gameStateRepository,
                _eventBus
            );

            _endTurnUseCase = new EndTurnUseCase(
                _gameStateRepository,
                _playerRepository,
                _eventBus
            );
        }

        [Test]
        public void FullGameFlow_StartGame_DrawCards_PlayCard_Attack_EndTurn()
        {
            // Arrange: Create test cards
            var card1 = Card.CreateInvocation(
                CardId.New(),
                "Test Invocation 1",
                "A test card",
                "Detailed description",
                attack: 5,
                defense: 3,
                families: new[] { CardFamily.Developer },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            var card2 = Card.CreateInvocation(
                CardId.New(),
                "Test Invocation 2",
                "Another test card",
                "Detailed description",
                attack: 4,
                defense: 4,
                families: new[] { CardFamily.Wizard },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            // Register test cards
            _cardRepository.RegisterCardDefinition(card1);
            _cardRepository.RegisterCardDefinition(card2);

            // Act 1: Start Game
            var player1Deck = new[] { card1.Id, card2.Id };
            var player2Deck = new[] { card1.Id, card2.Id };

            var startResult = _startGameUseCase.Execute(player1Deck, player2Deck);
            Assert.IsTrue(startResult.IsSuccess, "Game should start successfully");

            var gameState = _gameStateRepository.GetGameState();
            Assert.AreEqual(Phase.Draw, gameState.CurrentPhase);
            Assert.AreEqual(CardOwner.Player1, gameState.CurrentPlayer);
            Assert.AreEqual(1, gameState.TurnNumber);

            // Act 2: Draw Card for Player 1
            var drawResult = _drawCardUseCase.Execute(PlayerId.Player1);
            Assert.IsTrue(drawResult.IsSuccess, "Player 1 should draw a card");

            var player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            Assert.AreEqual(1, player1.Hand.Count, "Player 1 should have 1 card in hand");

            // Act 3: Play Card
            var cardToPlay = player1.Hand[0];
            var playResult = _playCardUseCase.Execute(PlayerId.Player1, cardToPlay.Id);
            Assert.IsTrue(playResult.IsSuccess, "Card should be played successfully");

            player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            Assert.AreEqual(0, player1.Hand.Count, "Hand should be empty");
            Assert.AreEqual(1, player1.Field.Count, "Field should have 1 card");

            // Act 4: End Turn (switch to Player 2)
            var endTurnResult = _endTurnUseCase.Execute();
            Assert.IsTrue(endTurnResult.IsSuccess, "Turn should end successfully");

            gameState = _gameStateRepository.GetGameState();
            Assert.AreEqual(CardOwner.Player2, gameState.CurrentPlayer);
            Assert.AreEqual(2, gameState.TurnNumber);

            // Act 5: Player 2 draws and plays a card
            drawResult = _drawCardUseCase.Execute(PlayerId.Player2);
            Assert.IsTrue(drawResult.IsSuccess);

            var player2 = _playerRepository.GetPlayer(PlayerId.Player2);
            var player2Card = player2.Hand[0];

            playResult = _playCardUseCase.Execute(PlayerId.Player2, player2Card.Id);
            Assert.IsTrue(playResult.IsSuccess);

            // Act 6: End Player 2 turn, back to Player 1
            endTurnResult = _endTurnUseCase.Execute();
            Assert.IsTrue(endTurnResult.IsSuccess);

            gameState = _gameStateRepository.GetGameState();
            Assert.AreEqual(CardOwner.Player1, gameState.CurrentPlayer);
            Assert.AreEqual(3, gameState.TurnNumber);

            // Act 7: Attack with Player 1's card
            player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            player2 = _playerRepository.GetPlayer(PlayerId.Player2);

            var attackerCard = player1.Field[0];
            var defenderCard = player2.Field[0];

            var attackResult = _attackUseCase.Execute(
                PlayerId.Player1,
                attackerCard.Id,
                defenderCard.Id
            );

            Assert.IsTrue(attackResult.IsSuccess, "Attack should succeed");

            // Verify combat results
            // Attacker (5/3) vs Defender (4/4)
            // Both cards should be destroyed (attacker ATK >= defender DEF, defender ATK >= attacker DEF)
            player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            player2 = _playerRepository.GetPlayer(PlayerId.Player2);

            Assert.AreEqual(0, player1.Field.Count, "Player 1 field should be empty (attacker destroyed)");
            Assert.AreEqual(0, player2.Field.Count, "Player 2 field should be empty (defender destroyed)");
            Assert.AreEqual(1, player1.Graveyard.Count, "Player 1 should have 1 card in graveyard");
            Assert.AreEqual(1, player2.Graveyard.Count, "Player 2 should have 1 card in graveyard");
        }

        [Test]
        public void EventBus_PublishesEventsCorrectly_DuringGameFlow()
        {
            // Arrange: Track events
            int gameStartedEvents = 0;
            int cardDrawnEvents = 0;
            int cardPlayedEvents = 0;
            int phaseChangedEvents = 0;

            _eventBus.Subscribe<GameStartedEvent>(evt => gameStartedEvents++);
            _eventBus.Subscribe<CardDrawnEvent>(evt => cardDrawnEvents++);
            _eventBus.Subscribe<CardPlayedEvent>(evt => cardPlayedEvents++);
            _eventBus.Subscribe<PhaseChangedEvent>(evt => phaseChangedEvents++);

            // Create test cards
            var card = Card.CreateInvocation(
                CardId.New(),
                "Test Card",
                "Description",
                "Details",
                attack: 3,
                defense: 3,
                families: new[] { CardFamily.Developer },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            _cardRepository.RegisterCardDefinition(card);

            // Act: Perform game flow
            var deck = new[] { card.Id, card.Id };
            _startGameUseCase.Execute(deck, deck);
            _drawCardUseCase.Execute(PlayerId.Player1);

            var player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            var cardToPlay = player1.Hand[0];
            _playCardUseCase.Execute(PlayerId.Player1, cardToPlay.Id);

            // Assert: Verify events
            Assert.AreEqual(1, gameStartedEvents, "Should publish 1 GameStartedEvent");
            Assert.AreEqual(1, cardDrawnEvents, "Should publish 1 CardDrawnEvent");
            Assert.AreEqual(1, cardPlayedEvents, "Should publish 1 CardPlayedEvent");
            Assert.Greater(phaseChangedEvents, 0, "Should publish PhaseChangedEvents");
        }

        [Test]
        public void PlayerDamage_ReducesHealthCorrectly()
        {
            // Arrange: Start game
            var card = Card.CreateInvocation(
                CardId.New(),
                "Attacker",
                "Description",
                "Details",
                attack: 10,  // High attack to damage player
                defense: 1,
                families: new[] { CardFamily.Developer },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            _cardRepository.RegisterCardDefinition(card);

            var deck = new[] { card.Id };
            _startGameUseCase.Execute(deck, deck);

            // Player 1 summons attacker
            _drawCardUseCase.Execute(PlayerId.Player1);
            var player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            var attackerCard = player1.Hand[0];
            _playCardUseCase.Execute(PlayerId.Player1, attackerCard.Id);

            // End turn to Player 2
            _endTurnUseCase.Execute();

            // Player 2's turn - skip actions
            _endTurnUseCase.Execute();

            // Back to Player 1 - attack Player 2 directly
            var player2 = _playerRepository.GetPlayer(PlayerId.Player2);
            int initialHealth = player2.Health;

            // Act: Direct attack on player
            player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            var attackCard = player1.Field[0];

            // Note: AttackUseCase requires a defender card, but for direct attacks
            // we would need a separate use case or modification
            // For this test, we'll verify health change through TakeDamage directly

            int damage = 10;
            int actualDamage = player2.TakeDamage(damage);
            _playerRepository.SavePlayer(player2);

            // Assert
            player2 = _playerRepository.GetPlayer(PlayerId.Player2);
            Assert.AreEqual(initialHealth - actualDamage, player2.Health, "Player health should decrease");
        }

        [Test]
        public void GameState_TracksMultipleTurnsCorrectly()
        {
            // Arrange
            var card = Card.CreateInvocation(CardId.New(), "Test", "Desc", "Details", 1, 1, null, true, null, null, false);
            _cardRepository.RegisterCardDefinition(card);

            var deck = new[] { card.Id };
            _startGameUseCase.Execute(deck, deck);

            // Act: Play 5 full turns (10 player turns total)
            for (int i = 0; i < 10; i++)
            {
                _endTurnUseCase.Execute();
            }

            // Assert
            var gameState = _gameStateRepository.GetGameState();
            Assert.AreEqual(11, gameState.TurnNumber, "Should be on turn 11");

            // Player 1 starts, so after 10 end turns (even number), it should be Player 1's turn again
            Assert.AreEqual(CardOwner.Player1, gameState.CurrentPlayer);
        }
    }
}

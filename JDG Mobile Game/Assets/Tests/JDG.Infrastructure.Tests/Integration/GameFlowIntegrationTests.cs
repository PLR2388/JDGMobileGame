using NUnit.Framework;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.Events;
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
        private IDeckRepository _deckRepository;
        private IGameStateRepository _gameStateRepository;
        private StartGameUseCase _startGameUseCase;
        private DrawCardUseCase _drawCardUseCase;
        private EndTurnUseCase _endTurnUseCase;

        [SetUp]
        public void Setup()
        {
            // Setup infrastructure
            _eventBus = new EventBus();
            _cardRepository = new CardRepository();
            _deckRepository = new DeckRepository(_cardRepository);
            _playerRepository = new PlayerRepository(_cardRepository);
            _gameStateRepository = new GameStateRepository();

            // Setup use cases
            _startGameUseCase = new StartGameUseCase(
                _playerRepository,
                _deckRepository,
                _gameStateRepository,
                _eventBus
            );

            _drawCardUseCase = new DrawCardUseCase(
                _playerRepository,
                _eventBus
            );

            _endTurnUseCase = new EndTurnUseCase(
                _gameStateRepository,
                _eventBus
            );
        }

        [Test]
        public void SimpleGameFlow_StartGame_DrawCards_EndTurn()
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

            // Register test cards in repository
            ((CardRepository)_cardRepository).RegisterCardDefinition(card1);
            ((CardRepository)_cardRepository).RegisterCardDefinition(card2);

            // Create a test deck
            _deckRepository.SaveDeck("TestDeck", new[] { card1.Id, card2.Id, card1.Id, card2.Id });

            // Act 1: Start Game
            var startResult = _startGameUseCase.Execute("TestDeck", "TestDeck");
            Assert.IsTrue(startResult.IsSuccess, "Game should start successfully");

            Assert.AreEqual(Phase.Draw, _gameStateRepository.CurrentPhase);
            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer);
            Assert.AreEqual(1, _gameStateRepository.TurnNumber);

            // Act 2: Draw Card for Player 1
            var drawResult = _drawCardUseCase.Execute(PlayerId.Player1);
            Assert.IsTrue(drawResult.IsSuccess, "Player 1 should draw a card");

            var player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            Assert.AreEqual(1, player1.HandCount, "Player 1 should have 1 card in hand");

            // Act 3: End Turn (switch to Player 2)
            var endTurnResult = _endTurnUseCase.Execute();
            Assert.IsTrue(endTurnResult.IsSuccess, "Turn should end successfully");

            Assert.AreEqual(PlayerId.Player2, _gameStateRepository.CurrentPlayer);
            Assert.AreEqual(2, _gameStateRepository.TurnNumber);

            // Act 4: Player 2 draws a card
            drawResult = _drawCardUseCase.Execute(PlayerId.Player2);
            Assert.IsTrue(drawResult.IsSuccess);

            var player2 = _playerRepository.GetPlayer(PlayerId.Player2);
            Assert.AreEqual(1, player2.HandCount, "Player 2 should have 1 card in hand");

            // Act 5: End Player 2 turn, back to Player 1
            endTurnResult = _endTurnUseCase.Execute();
            Assert.IsTrue(endTurnResult.IsSuccess);

            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer);
            Assert.AreEqual(3, _gameStateRepository.TurnNumber);
        }

        [Test]
        public void EventBus_PublishesEventsCorrectly_DuringGameFlow()
        {
            // Arrange: Track events
            int gameStartedEvents = 0;
            int cardDrawnEvents = 0;
            int phaseChangedEvents = 0;

            _eventBus.Subscribe<GameStartedEvent>(evt => gameStartedEvents++);
            _eventBus.Subscribe<CardDrawnEvent>(evt => cardDrawnEvents++);
            _eventBus.Subscribe<PhaseChangedEvent>(evt => phaseChangedEvents++);

            // Create test card
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

            ((CardRepository)_cardRepository).RegisterCardDefinition(card);
            _deckRepository.SaveDeck("TestDeck", new[] { card.Id, card.Id });

            // Act: Perform game flow
            _startGameUseCase.Execute("TestDeck", "TestDeck");
            _drawCardUseCase.Execute(PlayerId.Player1);

            // Assert: Verify events
            Assert.AreEqual(1, gameStartedEvents, "Should publish 1 GameStartedEvent");
            Assert.AreEqual(1, cardDrawnEvents, "Should publish 1 CardDrawnEvent");
            Assert.Greater(phaseChangedEvents, 0, "Should publish PhaseChangedEvents");
        }

        [Test]
        public void PlayerDamage_ReducesHealthCorrectly()
        {
            // Arrange
            var player = _playerRepository.GetPlayer(PlayerId.Player1);
            int initialHealth = player.Health;

            // Act: Direct damage through domain entity
            int damage = 10;
            int actualDamage = player.TakeDamage(damage);
            _playerRepository.SavePlayer(player);

            // Assert
            player = _playerRepository.GetPlayer(PlayerId.Player1);
            Assert.AreEqual(initialHealth - actualDamage, player.Health, "Player health should decrease");
            Assert.AreEqual(damage, actualDamage, "Should take full damage (no shields)");
        }

        [Test]
        public void GameState_TracksMultipleTurnsCorrectly()
        {
            // Arrange
            var card = Card.CreateInvocation(CardId.New(), "Test", "Desc", "Details", 1, 1, null, true, null, null, false);
            ((CardRepository)_cardRepository).RegisterCardDefinition(card);
            _deckRepository.SaveDeck("TestDeck", new[] { card.Id, card.Id, card.Id, card.Id });
            _startGameUseCase.Execute("TestDeck", "TestDeck");

            // Act: Play 10 turns
            for (int i = 0; i < 10; i++)
            {
                _endTurnUseCase.Execute();
            }

            // Assert
            Assert.AreEqual(11, _gameStateRepository.TurnNumber, "Should be on turn 11");

            // Player 1 starts, so after 10 end turns (even number), it should be Player 1's turn again
            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer);
        }
    }
}

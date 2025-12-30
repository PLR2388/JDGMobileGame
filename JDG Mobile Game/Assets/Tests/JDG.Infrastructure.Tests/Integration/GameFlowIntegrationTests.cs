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
            // Arrange: Create a test card and player
            var card = Card.CreateInvocation(CardId.New(), "Test", "Desc", "Details", 1, 1, null, true, null, null, false);
            ((CardRepository)_cardRepository).RegisterCardDefinition(card);

            // Create player with a deck
            var player = _playerRepository.CreatePlayer(PlayerId.Player1, new[] { card.Id }, maxHealth: 30);
            Assert.IsNotNull(player, "Player should be created");

            float initialHealth = player.Health;

            // Act: Direct damage through domain entity
            float damage = 10;
            float actualDamage = player.TakeDamage(damage);
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

        [Test]
        public void DirectAttack_WhenOpponentFieldEmpty_DamagesPlayer()
        {
            // Arrange: Create an attacker card
            var attackerCard = Card.CreateInvocation(
                CardId.New(),
                "Strong Attacker",
                "A strong attacker",
                "Details",
                attack: 10,
                defense: 5,
                families: new[] { CardFamily.Developer },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            ((CardRepository)_cardRepository).RegisterCardDefinition(attackerCard);
            _deckRepository.SaveDeck("AttackerDeck", new[] { attackerCard.Id, attackerCard.Id });
            _deckRepository.SaveDeck("EmptyDeck", new[] { attackerCard.Id, attackerCard.Id });

            _startGameUseCase.Execute("AttackerDeck", "EmptyDeck");

            // Player 1 draws a card and plays it (puts it on field)
            _drawCardUseCase.Execute(PlayerId.Player1);
            var player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            var cardToPlay = player1.Hand[0];
            player1.PlayCard(cardToPlay);
            _playerRepository.SavePlayer(player1);

            // Setup attack use case
            var attackUseCase = new AttackUseCase(_playerRepository, _cardRepository, _eventBus);

            // Act: Direct attack on Player 2
            var player2Before = _playerRepository.GetPlayer(PlayerId.Player2);
            float healthBefore = player2Before.Health;

            var result = attackUseCase.ExecuteDirectAttack(PlayerId.Player1, cardToPlay.Id, PlayerId.Player2);

            // Assert
            Assert.IsTrue(result.IsSuccess, "Direct attack should succeed");
            Assert.IsTrue(result.IsDirectAttack, "Should be a direct attack");
            Assert.AreEqual(10, result.Damage, "Should deal 10 damage");

            var player2After = _playerRepository.GetPlayer(PlayerId.Player2);
            Assert.AreEqual(healthBefore - 10, player2After.Health, "Player 2 should lose 10 health");
        }

        [Test]
        public void PlayerDefeated_WhenHealthReachesZero()
        {
            // Arrange: Create a very strong attacker
            var strongCard = Card.CreateInvocation(
                CardId.New(),
                "Lethal Attacker",
                "One hit kill",
                "Details",
                attack: 30,  // Enough to one-shot player (default health is 30)
                defense: 1,
                families: new[] { CardFamily.Developer },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            ((CardRepository)_cardRepository).RegisterCardDefinition(strongCard);
            _deckRepository.SaveDeck("StrongDeck", new[] { strongCard.Id, strongCard.Id });

            _startGameUseCase.Execute("StrongDeck", "StrongDeck");

            // Player 1 draws and plays
            _drawCardUseCase.Execute(PlayerId.Player1);
            var player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            var cardToPlay = player1.Hand[0];
            player1.PlayCard(cardToPlay);
            _playerRepository.SavePlayer(player1);

            var attackUseCase = new AttackUseCase(_playerRepository, _cardRepository, _eventBus);

            // Act: Direct attack to kill player 2
            var result = attackUseCase.ExecuteDirectAttack(PlayerId.Player1, cardToPlay.Id, PlayerId.Player2);

            // Assert
            Assert.IsTrue(result.IsSuccess, "Attack should succeed");
            Assert.IsTrue(result.PlayerDefeated, "Player 2 should be defeated");

            var player2 = _playerRepository.GetPlayer(PlayerId.Player2);
            Assert.IsTrue(player2.IsDefeated, "Player 2 IsDefeated should be true");
            Assert.AreEqual(0, player2.Health, "Player 2 health should be 0");
        }

        [Test]
        public void PhaseTransition_StartGame_BeginsWithDrawPhase()
        {
            // Arrange
            var card = Card.CreateInvocation(CardId.New(), "Test", "Desc", "Details", 1, 1, null, true, null, null, false);
            ((CardRepository)_cardRepository).RegisterCardDefinition(card);
            _deckRepository.SaveDeck("TestDeck", new[] { card.Id, card.Id });

            // Track phase changes
            var phases = new System.Collections.Generic.List<Phase>();
            _eventBus.Subscribe<PhaseChangedEvent>(evt => phases.Add(evt.NewPhase));

            // Act
            _startGameUseCase.Execute("TestDeck", "TestDeck");

            // Assert
            Assert.AreEqual(Phase.Draw, _gameStateRepository.CurrentPhase, "Should start with Draw phase");
            Assert.IsTrue(phases.Contains(Phase.Draw), "PhaseChangedEvent should indicate Draw phase");
        }

        [Test]
        public void CombatResolution_DestroysBothCards_WhenBothHaveLowDefense()
        {
            // Arrange: Create two cards that will destroy each other
            var card1 = Card.CreateInvocation(
                CardId.New(),
                "Glass Cannon",
                "High attack, low defense",
                "Details",
                attack: 5,
                defense: 2,
                families: new[] { CardFamily.Developer },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            var card2 = Card.CreateInvocation(
                CardId.New(),
                "Glass Defender",
                "Moderate attack, low defense",
                "Details",
                attack: 3,
                defense: 3,
                families: new[] { CardFamily.Wizard },
                affectedByEffect: true,
                conditions: null,
                abilities: null,
                isCollector: false
            );

            ((CardRepository)_cardRepository).RegisterCardDefinition(card1);
            ((CardRepository)_cardRepository).RegisterCardDefinition(card2);

            // Create players with specific cards
            _playerRepository.CreatePlayer(PlayerId.Player1, new[] { card1.Id, card1.Id }, maxHealth: 30);
            _playerRepository.CreatePlayer(PlayerId.Player2, new[] { card2.Id, card2.Id }, maxHealth: 30);

            _gameStateRepository.ResetGameState();
            _gameStateRepository.SetPhase(Phase.Attack);
            _gameStateRepository.SetCurrentPlayer(PlayerId.Player1);

            // Put cards on field
            var player1 = _playerRepository.GetPlayer(PlayerId.Player1);
            var player2 = _playerRepository.GetPlayer(PlayerId.Player2);

            player1.DrawCard();
            player2.DrawCard();
            player1.PlayCard(player1.Hand[0]);
            player2.PlayCard(player2.Hand[0]);
            _playerRepository.SavePlayer(player1);
            _playerRepository.SavePlayer(player2);

            var attackUseCase = new AttackUseCase(_playerRepository, _cardRepository, _eventBus);

            // Act: Card1 attacks Card2
            // Card1 has 5 ATK vs Card2's 3 DEF -> Card2 takes 5 damage, destroyed (3-5=-2)
            // Card2 has 3 ATK vs Card1's 2 DEF -> Card1 takes 3 damage, destroyed (2-3=-1)
            var result = attackUseCase.Execute(PlayerId.Player1, card1.Id, card2.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess, "Attack should succeed");
            Assert.IsTrue(result.DefenderDestroyed, "Defender should be destroyed");
            Assert.IsTrue(result.AttackerDestroyed, "Attacker should be destroyed");
        }

        [Test]
        public void DrawCard_WhenDeckEmpty_ReturnsFailure()
        {
            // Arrange: Create a deck with only 1 card
            var card = Card.CreateInvocation(CardId.New(), "Only Card", "Desc", "Details", 1, 1, null, true, null, null, false);
            ((CardRepository)_cardRepository).RegisterCardDefinition(card);
            _deckRepository.SaveDeck("TinyDeck", new[] { card.Id });

            _startGameUseCase.Execute("TinyDeck", "TinyDeck");

            // Draw the only card
            var result1 = _drawCardUseCase.Execute(PlayerId.Player1);
            Assert.IsTrue(result1.IsSuccess, "First draw should succeed");

            // Act: Try to draw again
            var result2 = _drawCardUseCase.Execute(PlayerId.Player1);

            // Assert
            Assert.IsFalse(result2.IsSuccess, "Second draw should fail - deck is empty");
            Assert.AreEqual("Deck is empty", result2.Message);
        }

        [Test]
        public void TurnAlternation_AlwaysAlternatesBetweenPlayers()
        {
            // Arrange
            var card = Card.CreateInvocation(CardId.New(), "Test", "Desc", "Details", 1, 1, null, true, null, null, false);
            ((CardRepository)_cardRepository).RegisterCardDefinition(card);
            _deckRepository.SaveDeck("TestDeck", new[] { card.Id, card.Id, card.Id, card.Id });
            _startGameUseCase.Execute("TestDeck", "TestDeck");

            // Assert initial state
            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer, "Should start as Player 1");

            // Act & Assert: End turns and verify alternation
            _endTurnUseCase.Execute();
            Assert.AreEqual(PlayerId.Player2, _gameStateRepository.CurrentPlayer, "After turn 1 end, should be Player 2");

            _endTurnUseCase.Execute();
            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer, "After turn 2 end, should be Player 1");

            _endTurnUseCase.Execute();
            Assert.AreEqual(PlayerId.Player2, _gameStateRepository.CurrentPlayer, "After turn 3 end, should be Player 2");

            _endTurnUseCase.Execute();
            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer, "After turn 4 end, should be Player 1");
        }
    }
}

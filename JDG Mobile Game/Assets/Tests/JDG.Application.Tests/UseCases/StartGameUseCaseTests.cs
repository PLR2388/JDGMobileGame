using NUnit.Framework;
using JDG.Application.UseCases;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Domain.Events;
using System.Collections.Generic;
using System.Linq;

namespace JDG.Application.Tests.UseCases
{
    [TestFixture]
    public class StartGameUseCaseTests
    {
        private StartGameUseCase _useCase;
        private TestPlayerRepositoryForStartGame _playerRepository;
        private TestDeckRepository _deckRepository;
        private TestGameStateRepository _gameStateRepository;
        private TestEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepositoryForStartGame();
            _deckRepository = new TestDeckRepository();
            _gameStateRepository = new TestGameStateRepository();
            _eventBus = new TestEventBus();
            _useCase = new StartGameUseCase(_playerRepository, _deckRepository, _gameStateRepository, _eventBus);

            // Setup default decks
            var deck1 = CreateTestDeck(20);
            var deck2 = CreateTestDeck(20);
            _deckRepository.SetDeck("TestDeck1", deck1);
            _deckRepository.SetDeck("TestDeck2", deck2);
            _deckRepository.SetDefaultDeck(PlayerId.Player1, deck1);
            _deckRepository.SetDefaultDeck(PlayerId.Player2, deck2);
        }

        private CardId[] CreateTestDeck(int size)
        {
            var deck = new CardId[size];
            for (int i = 0; i < size; i++)
            {
                deck[i] = CardId.New();
            }
            return deck;
        }

        [Test]
        public void Execute_WithValidDecks_StartsGameSuccessfully()
        {
            // Act
            var result = _useCase.Execute("TestDeck1", "TestDeck2");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Game started successfully", result.Message);
        }

        [Test]
        public void Execute_CreatesPlayers()
        {
            // Act
            var result = _useCase.Execute("TestDeck1", "TestDeck2");

            // Assert
            Assert.IsNotNull(result.Player1);
            Assert.IsNotNull(result.Player2);
        }

        [Test]
        public void Execute_ResetsGameState()
        {
            // Arrange
            _gameStateRepository.SetPhase(Phase.Attack);
            _gameStateRepository.SetCurrentPlayer(PlayerId.Player2);
            _gameStateRepository.IncrementTurn();

            // Act
            _useCase.Execute("TestDeck1", "TestDeck2");

            // Assert
            Assert.AreEqual(Phase.Draw, _gameStateRepository.CurrentPhase);
            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer);
        }

        [Test]
        public void Execute_PublishesGameStartedEvent()
        {
            // Act
            _useCase.Execute("TestDeck1", "TestDeck2");

            // Assert
            var gameStartedEvents = _eventBus.PublishedEvents.FindAll(e => e is GameStartedEvent);
            Assert.AreEqual(1, gameStartedEvents.Count);
        }

        [Test]
        public void Execute_PublishesPhaseChangedEvent()
        {
            // Act
            _useCase.Execute("TestDeck1", "TestDeck2");

            // Assert
            var phaseChangedEvents = _eventBus.PublishedEvents.FindAll(e => e is PhaseChangedEvent);
            Assert.AreEqual(1, phaseChangedEvents.Count);
        }

        [Test]
        public void Execute_WithEmptyDeckNames_UsesDefaultDecks()
        {
            // Act
            var result = _useCase.Execute("", "");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, _deckRepository.DefaultDeckAccessCount);
        }

        [Test]
        public void Execute_WithNullDeckNames_UsesDefaultDecks()
        {
            // Act
            var result = _useCase.Execute(null, null);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void Execute_WithInvalidPlayer1Deck_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute("NonExistentDeck", "TestDeck2");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player 1 deck not found or empty", result.Message);
        }

        [Test]
        public void Execute_WithInvalidPlayer2Deck_ReturnsFailure()
        {
            // Act
            var result = _useCase.Execute("TestDeck1", "NonExistentDeck");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player 2 deck not found or empty", result.Message);
        }

        [Test]
        public void Execute_WithEmptyPlayer1Deck_ReturnsFailure()
        {
            // Arrange
            _deckRepository.SetDeck("EmptyDeck", new CardId[0]);

            // Act
            var result = _useCase.Execute("EmptyDeck", "TestDeck2");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Player 1 deck not found or empty", result.Message);
        }

        [Test]
        public void Execute_ResetsPlayersBeforeCreating()
        {
            // Act
            _useCase.Execute("TestDeck1", "TestDeck2");

            // Assert
            Assert.IsTrue(_playerRepository.Player1Reset);
            Assert.IsTrue(_playerRepository.Player2Reset);
        }

        [Test]
        public void Execute_SetsStartingPlayerToPlayer1()
        {
            // Act
            _useCase.Execute("TestDeck1", "TestDeck2");

            // Assert
            Assert.AreEqual(PlayerId.Player1, _gameStateRepository.CurrentPlayer);
        }
    }

    // Test double for IPlayerRepository specific to StartGameUseCase tests
    public class TestPlayerRepositoryForStartGame : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();
        public bool Player1Reset { get; private set; }
        public bool Player2Reset { get; private set; }

        public Player GetPlayer(PlayerId playerId) =>
            _players.ContainsKey(playerId) ? _players[playerId] : null;

        public void SavePlayer(Player player) => _players[player.Id] = player;

        public Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30)
        {
            // Create cards from CardIds
            var cards = deckCardIds.Select(id => Card.CreateInvocation(
                id,
                $"Card {id}",
                "Test",
                "Test",
                3, 3,
                new[] { JDG.Domain.Enums.CardFamily.Human },
                false
            )).ToList();

            var player = new Player(playerId, cards, maxHealth);
            _players[playerId] = player;
            return player;
        }

        public void ResetPlayer(PlayerId playerId)
        {
            if (playerId == PlayerId.Player1) Player1Reset = true;
            if (playerId == PlayerId.Player2) Player2Reset = true;
            _players.Remove(playerId);
        }
    }

    // Test double for IDeckRepository
    public class TestDeckRepository : IDeckRepository
    {
        private readonly Dictionary<string, CardId[]> _decks = new Dictionary<string, CardId[]>();
        private readonly Dictionary<PlayerId, CardId[]> _defaultDecks = new Dictionary<PlayerId, CardId[]>();
        public int DefaultDeckAccessCount { get; private set; }

        public void SetDeck(string name, CardId[] deck) => _decks[name] = deck;
        public void SetDefaultDeck(PlayerId playerId, CardId[] deck) => _defaultDecks[playerId] = deck;

        public CardId[] GetDeck(string deckName) =>
            _decks.ContainsKey(deckName) ? _decks[deckName] : null;

        public void SaveDeck(string deckName, CardId[] cardIds) => _decks[deckName] = cardIds;

        public IEnumerable<string> GetAllDeckNames() => _decks.Keys;

        public void DeleteDeck(string deckName) => _decks.Remove(deckName);

        public CardId[] GetDefaultDeck(PlayerId playerId)
        {
            DefaultDeckAccessCount++;
            return _defaultDecks.ContainsKey(playerId) ? _defaultDecks[playerId] : null;
        }
    }
}

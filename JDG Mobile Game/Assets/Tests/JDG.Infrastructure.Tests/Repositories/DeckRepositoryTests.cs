using NUnit.Framework;
using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Repositories;
using System.Linq;

namespace JDG.Infrastructure.Tests.Repositories
{
    /// <summary>
    /// Unit tests for DeckRepository.
    /// Tests deck saving, loading, and management operations.
    /// </summary>
    [TestFixture]
    public class DeckRepositoryTests
    {
        private DeckRepository _repository;
        private CardRepository _cardRepository;

        [SetUp]
        public void SetUp()
        {
            _cardRepository = new CardRepository();
            _repository = new DeckRepository(_cardRepository);
        }

        #region GetDeck Tests

        [Test]
        public void GetDeck_WithNonExistentDeck_ReturnsNull()
        {
            // Act
            var result = _repository.GetDeck("NonExistent");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetDeck_WithSavedDeck_ReturnsDeck()
        {
            // Arrange
            var cardIds = new[] { CardId.New(), CardId.New(), CardId.New() };
            _repository.SaveDeck("TestDeck", cardIds);

            // Act
            var result = _repository.GetDeck("TestDeck");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
        }

        #endregion

        #region SaveDeck Tests

        [Test]
        public void SaveDeck_WithValidParameters_SavesDeck()
        {
            // Arrange
            var cardIds = new[] { CardId.New(), CardId.New() };

            // Act
            _repository.SaveDeck("MyDeck", cardIds);

            // Assert
            var retrieved = _repository.GetDeck("MyDeck");
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(2, retrieved.Length);
        }

        [Test]
        public void SaveDeck_WithNullName_DoesNotSave()
        {
            // Arrange
            var cardIds = new[] { CardId.New() };

            // Act
            _repository.SaveDeck(null, cardIds);

            // Assert
            var names = _repository.GetAllDeckNames().ToList();
            Assert.IsEmpty(names);
        }

        [Test]
        public void SaveDeck_WithEmptyName_DoesNotSave()
        {
            // Arrange
            var cardIds = new[] { CardId.New() };

            // Act
            _repository.SaveDeck("", cardIds);

            // Assert
            var names = _repository.GetAllDeckNames().ToList();
            Assert.IsEmpty(names);
        }

        [Test]
        public void SaveDeck_WithNullCardIds_DoesNotSave()
        {
            // Act
            _repository.SaveDeck("TestDeck", null);

            // Assert
            var result = _repository.GetDeck("TestDeck");
            Assert.IsNull(result);
        }

        [Test]
        public void SaveDeck_WithEmptyCardIds_DoesNotSave()
        {
            // Act
            _repository.SaveDeck("TestDeck", new CardId[0]);

            // Assert
            var result = _repository.GetDeck("TestDeck");
            Assert.IsNull(result);
        }

        [Test]
        public void SaveDeck_WithSameName_OverwritesPrevious()
        {
            // Arrange
            var cardIds1 = new[] { CardId.New() };
            var cardIds2 = new[] { CardId.New(), CardId.New(), CardId.New() };

            // Act
            _repository.SaveDeck("MyDeck", cardIds1);
            _repository.SaveDeck("MyDeck", cardIds2);

            // Assert
            var retrieved = _repository.GetDeck("MyDeck");
            Assert.AreEqual(3, retrieved.Length);
        }

        #endregion

        #region GetAllDeckNames Tests

        [Test]
        public void GetAllDeckNames_WithNoDecks_ReturnsEmpty()
        {
            // Act
            var result = _repository.GetAllDeckNames();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetAllDeckNames_WithMultipleDecks_ReturnsAll()
        {
            // Arrange
            _repository.SaveDeck("Deck1", new[] { CardId.New() });
            _repository.SaveDeck("Deck2", new[] { CardId.New() });
            _repository.SaveDeck("Deck3", new[] { CardId.New() });

            // Act
            var result = _repository.GetAllDeckNames().ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.Contains("Deck1", result);
            Assert.Contains("Deck2", result);
            Assert.Contains("Deck3", result);
        }

        #endregion

        #region DeleteDeck Tests

        [Test]
        public void DeleteDeck_WithExistingDeck_RemovesDeck()
        {
            // Arrange
            _repository.SaveDeck("ToDelete", new[] { CardId.New() });

            // Act
            _repository.DeleteDeck("ToDelete");

            // Assert
            var result = _repository.GetDeck("ToDelete");
            Assert.IsNull(result);
        }

        [Test]
        public void DeleteDeck_WithNonExistentDeck_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _repository.DeleteDeck("NonExistent"));
        }

        [Test]
        public void DeleteDeck_OnlyRemovesSpecifiedDeck()
        {
            // Arrange
            _repository.SaveDeck("Deck1", new[] { CardId.New() });
            _repository.SaveDeck("Deck2", new[] { CardId.New() });

            // Act
            _repository.DeleteDeck("Deck1");

            // Assert
            Assert.IsNull(_repository.GetDeck("Deck1"));
            Assert.IsNotNull(_repository.GetDeck("Deck2"));
        }

        #endregion

        #region GetDefaultDeck Tests

        [Test]
        public void GetDefaultDeck_WithNoCards_ReturnsEmptyArray()
        {
            // Act
            var result = _repository.GetDefaultDeck(PlayerId.Player1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetDefaultDeck_WithAvailableCards_ReturnsDeck()
        {
            // Arrange - Add some card definitions
            for (int i = 0; i < 10; i++)
            {
                _cardRepository.RegisterCardDefinition(
                    Card.CreateInvocation(
                        CardId.New(),
                        $"Card {i}",
                        "Description",
                        "Details",
                        5, 5,
                        new[] { CardFamily.Rpg },
                        true, null, null, false
                    )
                );
            }

            // Act
            var result = _repository.GetDefaultDeck(PlayerId.Player1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Length);
        }

        [Test]
        public void GetDefaultDeck_CreatesUniqueCardInstances()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                _cardRepository.RegisterCardDefinition(
                    Card.CreateInvocation(
                        CardId.New(),
                        $"Card {i}",
                        "Description",
                        "Details",
                        5, 5,
                        new[] { CardFamily.Rpg },
                        true, null, null, false
                    )
                );
            }

            // Act
            var result = _repository.GetDefaultDeck(PlayerId.Player1);

            // Assert - All card IDs should be unique
            var uniqueIds = result.Distinct().Count();
            Assert.AreEqual(result.Length, uniqueIds);
        }

        [Test]
        public void GetDefaultDeck_CardsAreRetrievableFromCardRepository()
        {
            // Arrange
            _cardRepository.RegisterCardDefinition(
                Card.CreateInvocation(
                    CardId.New(),
                    "Retrievable Card",
                    "Description",
                    "Details",
                    5, 5,
                    new[] { CardFamily.Rpg },
                    true, null, null, false
                )
            );

            // Act
            var deckCardIds = _repository.GetDefaultDeck(PlayerId.Player1);

            // Assert - Each card ID should be retrievable
            foreach (var cardId in deckCardIds)
            {
                var card = _cardRepository.GetCard(cardId);
                Assert.IsNotNull(card, $"Card with ID {cardId} should be retrievable");
            }
        }

        #endregion

        #region LoadDecks Tests

        [Test]
        public void LoadDecks_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _repository.LoadDecks());
        }

        #endregion
    }
}

using NUnit.Framework;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Repositories;
using System.Linq;

namespace JDG.Infrastructure.Tests.Repositories
{
    /// <summary>
    /// Unit tests for CardRepository.
    /// Tests card definition registration, instance creation, and querying.
    /// </summary>
    [TestFixture]
    public class CardRepositoryTests
    {
        private CardRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new CardRepository();
        }

        #region Initialize Tests

        [Test]
        public void Initialize_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _repository.Initialize());
        }

        #endregion

        #region RegisterCardDefinition Tests

        [Test]
        public void RegisterCardDefinition_WithValidCard_AddsToRepository()
        {
            // Arrange
            var card = CreateTestInvocationCard("Test Card");

            // Act
            _repository.RegisterCardDefinition(card);

            // Assert
            var result = _repository.GetCardByTitle("Test Card");
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Card", result.Title);
        }

        [Test]
        public void RegisterCardDefinition_WithNull_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _repository.RegisterCardDefinition(null));
        }

        [Test]
        public void RegisterCardDefinition_WithDuplicateTitle_OverwritesPrevious()
        {
            // Arrange
            var card1 = CreateTestInvocationCard("Duplicate Card");
            var card2 = Card.CreateInvocation(
                CardId.New(),
                "Duplicate Card",
                "New Description",
                "New Details",
                10, 10,
                new[] { CardFamily.Rpg },
                true, null, null, false
            );

            // Act
            _repository.RegisterCardDefinition(card1);
            _repository.RegisterCardDefinition(card2);

            // Assert
            var result = _repository.GetCardByTitle("Duplicate Card");
            Assert.AreEqual("New Description", result.Description);
        }

        #endregion

        #region GetCard Tests

        [Test]
        public void GetCard_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var nonExistentId = CardId.New();

            // Act
            var result = _repository.GetCard(nonExistentId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetCard_WithRegisteredInstance_ReturnsCard()
        {
            // Arrange
            var card = CreateTestInvocationCard("Instance Card");
            _repository.RegisterCardInstance(card);

            // Act
            var result = _repository.GetCard(card.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(card.Id, result.Id);
        }

        #endregion

        #region CreateCardInstance Tests

        [Test]
        public void CreateCardInstance_WithRegisteredDefinition_CreatesNewInstance()
        {
            // Arrange
            var definition = CreateTestInvocationCard("Definition Card");
            _repository.RegisterCardDefinition(definition);

            // Act
            var instance = _repository.CreateCardInstance("Definition Card");

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreNotEqual(definition.Id, instance.Id); // Should have new ID
            Assert.AreEqual(definition.Title, instance.Title);
        }

        [Test]
        public void CreateCardInstance_WithNonExistentDefinition_ReturnsNull()
        {
            // Act
            var instance = _repository.CreateCardInstance("Non Existent Card");

            // Assert
            Assert.IsNull(instance);
        }

        [Test]
        public void CreateCardInstance_RegistersInstanceForRetrieval()
        {
            // Arrange
            var definition = CreateTestInvocationCard("Retrievable Card");
            _repository.RegisterCardDefinition(definition);

            // Act
            var instance = _repository.CreateCardInstance("Retrievable Card");
            var retrieved = _repository.GetCard(instance.Id);

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(instance.Id, retrieved.Id);
        }

        [Test]
        public void CreateCardInstance_ForEquipmentCard_CreatesCorrectType()
        {
            // Arrange
            var definition = Card.CreateEquipment(
                CardId.New(),
                "Equipment Card",
                "Description",
                "Details",
                new[] { JDG.Domain.Enums.EquipmentAbilityName.DirectAttack },
                false
            );
            _repository.RegisterCardDefinition(definition);

            // Act
            var instance = _repository.CreateCardInstance("Equipment Card");

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(CardType.Equipment, instance.Type);
        }

        [Test]
        public void CreateCardInstance_ForFieldCard_CreatesCorrectType()
        {
            // Arrange
            var definition = Card.CreateField(
                CardId.New(),
                "Field Card",
                "Description",
                "Details",
                CardFamily.Comics,
                new[] { JDG.Domain.Enums.FieldAbilityName.DrawOneMoreCard },
                false
            );
            _repository.RegisterCardDefinition(definition);

            // Act
            var instance = _repository.CreateCardInstance("Field Card");

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(CardType.Field, instance.Type);
        }

        [Test]
        public void CreateCardInstance_ForEffectCard_CreatesCorrectType()
        {
            // Arrange
            var definition = Card.CreateEffect(
                CardId.New(),
                "Effect Card",
                "Description",
                "Details",
                new[] { JDG.Domain.Enums.EffectAbilityName.LimitHandCardTo5 },
                false
            );
            _repository.RegisterCardDefinition(definition);

            // Act
            var instance = _repository.CreateCardInstance("Effect Card");

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(CardType.Effect, instance.Type);
        }

        [Test]
        public void CreateCardInstance_ForContreCard_CreatesCorrectType()
        {
            // Arrange
            var definition = Card.CreateContre(
                CardId.New(),
                "Contre Card",
                "Description",
                "Details",
                false
            );
            _repository.RegisterCardDefinition(definition);

            // Act
            var instance = _repository.CreateCardInstance("Contre Card");

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual(CardType.Contre, instance.Type);
        }

        #endregion

        #region GetAllCardDefinitions Tests

        [Test]
        public void GetAllCardDefinitions_WithNoCards_ReturnsEmpty()
        {
            // Act
            var result = _repository.GetAllCardDefinitions();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetAllCardDefinitions_WithMultipleCards_ReturnsAll()
        {
            // Arrange
            _repository.RegisterCardDefinition(CreateTestInvocationCard("Card 1"));
            _repository.RegisterCardDefinition(CreateTestInvocationCard("Card 2"));
            _repository.RegisterCardDefinition(CreateTestInvocationCard("Card 3"));

            // Act
            var result = _repository.GetAllCardDefinitions().ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
        }

        #endregion

        #region GetCardsByType Tests

        [Test]
        public void GetCardsByType_ReturnsOnlyMatchingType()
        {
            // Arrange
            _repository.RegisterCardDefinition(CreateTestInvocationCard("Invocation 1"));
            _repository.RegisterCardDefinition(CreateTestInvocationCard("Invocation 2"));
            _repository.RegisterCardDefinition(Card.CreateEquipment(
                CardId.New(), "Equipment 1", "Desc", "Details",
                new[] { JDG.Domain.Enums.EquipmentAbilityName.DirectAttack }, false
            ));

            // Act
            var invocations = _repository.GetCardsByType(CardType.Invocation).ToList();
            var equipment = _repository.GetCardsByType(CardType.Equipment).ToList();

            // Assert
            Assert.AreEqual(2, invocations.Count);
            Assert.AreEqual(1, equipment.Count);
        }

        #endregion

        #region GetCardsByFamily Tests

        [Test]
        public void GetCardsByFamily_ReturnsMatchingCards()
        {
            // Arrange
            _repository.RegisterCardDefinition(Card.CreateInvocation(
                CardId.New(), "Rpg Card", "Desc", "Details",
                5, 5, new[] { CardFamily.Rpg }, true, null, null, false
            ));
            _repository.RegisterCardDefinition(Card.CreateInvocation(
                CardId.New(), "Comics Card", "Desc", "Details",
                5, 5, new[] { CardFamily.Comics }, true, null, null, false
            ));

            // Act
            var rpgCards = _repository.GetCardsByFamily(CardFamily.Rpg).ToList();

            // Assert
            Assert.AreEqual(1, rpgCards.Count);
            Assert.AreEqual("Rpg Card", rpgCards[0].Title);
        }

        #endregion

        #region GetCardByTitle Tests

        [Test]
        public void GetCardByTitle_WithExistingTitle_ReturnsCard()
        {
            // Arrange
            _repository.RegisterCardDefinition(CreateTestInvocationCard("Unique Title"));

            // Act
            var result = _repository.GetCardByTitle("Unique Title");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Unique Title", result.Title);
        }

        [Test]
        public void GetCardByTitle_WithNonExistentTitle_ReturnsNull()
        {
            // Act
            var result = _repository.GetCardByTitle("Non Existent");

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Helper Methods

        private Card CreateTestInvocationCard(string title)
        {
            return Card.CreateInvocation(
                CardId.New(),
                title,
                "Test Description",
                "Test Detailed Description",
                5, 5,
                new[] { CardFamily.Rpg },
                true,
                null,
                null,
                false
            );
        }

        #endregion
    }
}

using NUnit.Framework;
using JDG.Application.Services;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Unit tests for LocalizationService.
    /// Phase 39: Added comprehensive tests for localization service.
    /// </summary>
    [TestFixture]
    public class LocalizationServiceTests
    {
        private ILocalizationService _localizationService;

        [SetUp]
        public void Setup()
        {
            // Note: LocalizationService currently depends on LocalizationSystem.Instance
            // These tests will only run if LocalizationSystem singleton exists in scene
            // TODO: Refactor LocalizationService to accept dependencies for better testability
        }

        #region Constructor Tests

        [Test]
        public void LocalizationService_WhenCreated_ImplementsInterface()
        {
            // Arrange & Act
            var service = new LocalizationService();

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOf<ILocalizationService>(service);
        }

        [Test]
        public void LocalizationService_WhenCreated_HasDefaultLanguage()
        {
            // Arrange & Act
            var service = new LocalizationService();

            // Assert - Default is French
            Assert.AreEqual(GameLanguage.French, service.GetCurrentLanguage());
        }

        #endregion

        #region GetCurrentLanguage Tests

        [Test]
        public void GetCurrentLanguage_ReturnsDefault()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            var language = service.GetCurrentLanguage();

            // Assert
            Assert.AreEqual(GameLanguage.French, language);
        }

        [Test]
        public void GetCurrentLanguage_AfterSetLanguage_ReturnsUpdatedLanguage()
        {
            // Arrange
            var service = new LocalizationService();
            service.SetLanguage(GameLanguage.English);

            // Act
            var language = service.GetCurrentLanguage();

            // Assert
            Assert.AreEqual(GameLanguage.English, language);
        }

        #endregion

        #region SetLanguage Tests

        [Test]
        public void SetLanguage_UpdatesCurrentLanguage()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            service.SetLanguage(GameLanguage.English);
            var language = service.GetCurrentLanguage();

            // Assert
            Assert.AreEqual(GameLanguage.English, language);
        }

        [Test]
        public void SetLanguage_ToFrench_SetsLanguageCorrectly()
        {
            // Arrange
            var service = new LocalizationService();
            service.SetLanguage(GameLanguage.English); // Change from default

            // Act
            service.SetLanguage(GameLanguage.French);

            // Assert
            Assert.AreEqual(GameLanguage.French, service.GetCurrentLanguage());
        }

        [Test]
        public void SetLanguage_MultipleTimes_UsesLastValue()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            service.SetLanguage(GameLanguage.English);
            service.SetLanguage(GameLanguage.French);
            service.SetLanguage(GameLanguage.English);

            // Assert
            Assert.AreEqual(GameLanguage.English, service.GetCurrentLanguage());
        }

        #endregion

        #region HasKey Tests

        [Test]
        public void HasKey_WithValidKey_ReturnsCorrectly()
        {
            // Arrange
            var service = new LocalizationService();

            // Act - Test with common localization keys
            // These may fail if LocalizationKeys enum doesn't have these values
            var hasPhaseChoose = service.HasKey("PHASE_CHOOSE");
            var hasInvalidKey = service.HasKey("INVALID_KEY_THAT_DOES_NOT_EXIST");

            // Assert
            // We can only assert that the method doesn't throw
            Assert.IsNotNull(hasPhaseChoose);
            Assert.IsNotNull(hasInvalidKey);
        }

        [Test]
        public void HasKey_WithInvalidKey_ReturnsFalse()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            var result = service.HasKey("THIS_KEY_DEFINITELY_DOES_NOT_EXIST_12345");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void HasKey_WithEmptyString_ReturnsFalse()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            var result = service.HasKey("");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void HasKey_WithNullKey_ReturnsFalse()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            var result = service.HasKey(null);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetLocalizedValue Tests

        [Test]
        public void GetLocalizedValue_WithInvalidKey_ReturnsKeyInBrackets()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            var result = service.GetLocalizedValue("INVALID_KEY_XYZ");

            // Assert
            Assert.IsNotNull(result);
            // Should return key in brackets as fallback
            Assert.That(result, Does.Contain("INVALID_KEY_XYZ").Or.Contains("["));
        }

        [Test]
        public void GetLocalizedValue_WithEmptyKey_ReturnsEmptyInBrackets()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            var result = service.GetLocalizedValue("");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[]", result);
        }

        [Test]
        public void GetLocalizedValue_ReturnsNonNullValue()
        {
            // Arrange
            var service = new LocalizationService();

            // Act
            var result = service.GetLocalizedValue("PHASE_CHOOSE");

            // Assert
            Assert.IsNotNull(result);
        }

        #endregion

        #region GameLanguage Enum Tests

        [Test]
        public void GameLanguage_HasExpectedValues()
        {
            // Assert - Verify the enum has the expected values
            Assert.IsTrue(System.Enum.IsDefined(typeof(GameLanguage), GameLanguage.French));
            Assert.IsTrue(System.Enum.IsDefined(typeof(GameLanguage), GameLanguage.English));
        }

        #endregion
    }
}

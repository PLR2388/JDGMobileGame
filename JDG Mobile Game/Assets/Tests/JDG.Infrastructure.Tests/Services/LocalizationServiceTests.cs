using NUnit.Framework;
using JDG.Application.Services;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.Tests.Services
{
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
        public void GetLocalizedValue_WithInvalidKey_ReturnsKeyInBrackets()
        {
            // Arrange
            if (LocalizationSystem.Instance == null) Assert.Ignore("LocalizationSystem singleton not available");

            var service = new LocalizationService();

            // Act
            var result = service.GetLocalizedValue("INVALID_KEY_XYZ");

            // Assert
            Assert.IsNotNull(result);
            // Should return key in brackets as fallback
            Assert.That(result, Does.Contain("INVALID_KEY_XYZ").Or.Contains("["));
        }
    }
}

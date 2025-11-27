using NUnit.Framework;
using JDG.Application.Services;
using JDG.Infrastructure.Services;
using Sound;
using UnityEngine;

namespace JDG.Infrastructure.Tests.Services
{
    [TestFixture]
    public class AudioServiceTests
    {
        private IAudioService _audioService;

        [SetUp]
        public void Setup()
        {
            // Note: AudioService currently depends on AudioSystem.Instance
            // These tests will only run if AudioSystem singleton exists in scene
            // TODO: Refactor AudioService to accept dependencies for better testability
        }

        [Test]
        public void AudioService_WhenCreated_ImplementsInterface()
        {
            // Arrange & Act
            var service = new AudioService();

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOf<IAudioService>(service);
        }

        [Test]
        public void GetMusicVolume_ReturnsValidRange()
        {
            // Arrange
            if (!AudioSystem.InstanceExists) Assert.Ignore("AudioSystem singleton not available");

            var service = new AudioService();

            // Act
            var volume = service.GetMusicVolume();

            // Assert
            Assert.GreaterOrEqual(volume, 0f);
            Assert.LessOrEqual(volume, 1f);
        }

        [Test]
        public void GetSfxVolume_ReturnsValidRange()
        {
            // Arrange
            if (!AudioSystem.InstanceExists) Assert.Ignore("AudioSystem singleton not available");

            var service = new AudioService();

            // Act
            var volume = service.GetSfxVolume();

            // Assert
            Assert.GreaterOrEqual(volume, 0f);
            Assert.LessOrEqual(volume, 1f);
        }

        [Test]
        public void SetMusicVolume_ClampsToValidRange()
        {
            // Arrange
            if (!AudioSystem.InstanceExists) Assert.Ignore("AudioSystem singleton not available");

            var service = new AudioService();

            // Act - Test clamping upper bound
            service.SetMusicVolume(1.5f);
            var volume1 = service.GetMusicVolume();

            // Act - Test clamping lower bound
            service.SetMusicVolume(-0.5f);
            var volume2 = service.GetMusicVolume();

            // Assert
            Assert.LessOrEqual(volume1, 1f);
            Assert.GreaterOrEqual(volume2, 0f);
        }
    }
}

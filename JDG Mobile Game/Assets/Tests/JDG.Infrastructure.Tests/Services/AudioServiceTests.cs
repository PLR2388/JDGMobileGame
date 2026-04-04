using NUnit.Framework;
using JDG.Application.Services;
using JDG.Infrastructure.Services;
using Sound;
using UnityEngine;

namespace JDG.Infrastructure.Tests.Services
{
    /// <summary>
    /// Tests for AudioService.
    /// Phase 88: Improved test isolation with proper categorization.
    ///
    /// Unit Tests: Tests that don't require AudioSystem (instantiation, interface checks)
    /// Integration Tests: Tests that require AudioSystem singleton for audio playback
    ///
    /// Integration tests use Assert.Ignore when AudioSystem is unavailable,
    /// allowing them to run in Unity Play Mode when the singleton exists.
    /// </summary>
    [TestFixture]
    public class AudioServiceTests
    {
        #region Unit Tests (No AudioSystem Required)

        [Test]
        [Category("Unit")]
        public void AudioService_WhenCreated_ImplementsInterface()
        {
            // Arrange & Act
            var service = new AudioService();

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOf<IAudioService>(service);
        }

        [Test]
        [Category("Unit")]
        public void AudioService_WhenCreated_CanBeInstantiated()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => new AudioService());
        }

        [Test]
        [Category("Unit")]
        public void GetMusicVolume_WithoutAudioSystem_ReturnsZero()
        {
            // AudioService returns 0 when AudioSystem is null (graceful degradation)
            #pragma warning disable CS0618
            if (AudioSystem.Instance != null)
            {
                Assert.Ignore("This test requires AudioSystem to be unavailable");
            }
            #pragma warning restore CS0618

            // Arrange
            var service = new AudioService();

            // Act
            var volume = service.GetMusicVolume();

            // Assert - Returns 0 when AudioSystem is null
            Assert.AreEqual(0f, volume);
        }

        [Test]
        [Category("Unit")]
        public void GetSfxVolume_WithoutAudioSystem_ReturnsZero()
        {
            #pragma warning disable CS0618
            if (AudioSystem.Instance != null)
            {
                Assert.Ignore("This test requires AudioSystem to be unavailable");
            }
            #pragma warning restore CS0618

            // Arrange
            var service = new AudioService();

            // Act
            var volume = service.GetSfxVolume();

            // Assert - Returns 0 when AudioSystem is null
            Assert.AreEqual(0f, volume);
        }

        #endregion

        #region Integration Tests (Require AudioSystem)

        [Test]
        [Category("Integration")]
        public void GetMusicVolume_WithAudioSystem_ReturnsValidRange()
        {
            // Arrange
            #pragma warning disable CS0618
            if (AudioSystem.Instance == null)
            {
                Assert.Ignore("AudioSystem singleton not available - run in Unity Play Mode");
            }
            #pragma warning restore CS0618

            var service = new AudioService();

            // Act
            var volume = service.GetMusicVolume();

            // Assert
            Assert.GreaterOrEqual(volume, 0f);
            Assert.LessOrEqual(volume, 1f);
        }

        [Test]
        [Category("Integration")]
        public void GetSfxVolume_WithAudioSystem_ReturnsValidRange()
        {
            // Arrange
            #pragma warning disable CS0618
            if (AudioSystem.Instance == null)
            {
                Assert.Ignore("AudioSystem singleton not available - run in Unity Play Mode");
            }
            #pragma warning restore CS0618

            var service = new AudioService();

            // Act
            var volume = service.GetSfxVolume();

            // Assert
            Assert.GreaterOrEqual(volume, 0f);
            Assert.LessOrEqual(volume, 1f);
        }

        [Test]
        [Category("Integration")]
        public void SetMusicVolume_WithAudioSystem_ClampsToValidRange()
        {
            // Arrange
            #pragma warning disable CS0618
            if (AudioSystem.Instance == null)
            {
                Assert.Ignore("AudioSystem singleton not available - run in Unity Play Mode");
            }
            #pragma warning restore CS0618

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

        #endregion
    }
}

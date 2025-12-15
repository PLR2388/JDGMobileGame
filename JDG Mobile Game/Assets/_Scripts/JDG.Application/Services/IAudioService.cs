namespace JDG.Application.Services
{
    /// <summary>
    /// Service for playing audio (music and sound effects).
    /// Replaces AudioSystem singleton.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Plays background music by name.
        /// </summary>
        void PlayMusic(string musicName);

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        void StopMusic();

        /// <summary>
        /// Plays a sound effect by name.
        /// </summary>
        void PlaySoundEffect(string sfxName);

        /// <summary>
        /// Sets the master volume (0.0 to 1.0).
        /// </summary>
        void SetMasterVolume(float volume);

        /// <summary>
        /// Sets the music volume (0.0 to 1.0).
        /// </summary>
        void SetMusicVolume(float volume);

        /// <summary>
        /// Sets the sound effects volume (0.0 to 1.0).
        /// </summary>
        void SetSfxVolume(float volume);

        /// <summary>
        /// Gets the current music volume.
        /// </summary>
        float GetMusicVolume();

        /// <summary>
        /// Gets the current sound effects volume.
        /// </summary>
        float GetSfxVolume();

        /// <summary>
        /// Plays music associated with a card family.
        /// Phase 37: Added to support CardPlacementService DI migration.
        /// </summary>
        /// <param name="family">The card family to play music for.</param>
        void PlayFamilyMusic(object family);

        // ============================================
        // Phase 8: Convenience methods for singleton migration
        // These match the AudioSystem API to ease migration
        // ============================================

        /// <summary>
        /// Plays the transition sound effect.
        /// </summary>
        void PlayTransitionSound();

        /// <summary>
        /// Plays the back navigation sound effect.
        /// </summary>
        void PlayBackSound();
    }
}

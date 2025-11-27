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
    }
}

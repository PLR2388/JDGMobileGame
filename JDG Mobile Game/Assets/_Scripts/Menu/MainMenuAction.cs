using JDG.Application.Services;
using Sound;
using UnityEngine;
using VContainer;

/// <summary>
/// Represents actions in the main menu related to sound and music playback.
/// Provides methods to play different themes or sounds for various menu sections.
/// Phase 8: Migrated from AudioSystem.Instance to IAudioService DI.
/// </summary>
public class MainMenuAction : MonoBehaviour
{
    private IAudioService _audioService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 8: Inject IAudioService instead of using AudioSystem.Instance.
    /// </summary>
    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Invoked after all Awake calls complete.
    /// Phase 84 Fix: Changed from Awake to Start to ensure VContainer injection completes first.
    /// Automatically plays the main theme music.
    /// </summary>
    private void Start()
    {
        PlayMainTheme();
    }

    /// <summary>
    /// Plays the main theme music.
    /// </summary>
    public void PlayMainTheme()
    {
        GetAudioService().PlayMusic(nameof(Music.MainTheme));
    }

    /// <summary>
    /// Plays the music for the one player menu section.
    /// </summary>
    public void PlayOnePlayerMenuMusic()
    {
        GetAudioService().PlayMusic(nameof(Music.OnePlayerMenu));
    }

    /// <summary>
    /// Plays the music for the two player menu section.
    /// </summary>
    public void PlayTwoPlayerMenuMusic()
    {
        GetAudioService().PlayMusic(nameof(Music.TwoPlayerMenu));
    }

    /// <summary>
    /// Plays the music for the options menu section.
    /// </summary>
    public void PlayOptionMenuMusic()
    {
        GetAudioService().PlayMusic(nameof(Music.OptionMenu));
    }

    /// <summary>
    /// Plays the transition sound effect.
    /// </summary>
    public void PlayTransitionSound()
    {
        GetAudioService().PlayTransitionSound();
    }

    /// <summary>
    /// Plays the back navigation sound effect.
    /// </summary>
    public void PlayBackSound()
    {
        GetAudioService().PlayBackSound();
    }

    /// <summary>
    /// Gets the audio service, falling back to singleton wrapper if DI not available.
    /// Phase 8: Temporary fallback during migration - allows use in scenes without VContainer.
    /// </summary>
    private IAudioService GetAudioService()
    {
        if (_audioService != null)
            return _audioService;

        // Fallback for scenes without VContainer scope
        // This allows gradual migration without breaking non-DI scenes
        return new JDG.Infrastructure.Services.AudioService();
    }
}

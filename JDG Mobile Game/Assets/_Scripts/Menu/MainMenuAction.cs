using Sound;
using UnityEngine;

/// <summary>
/// Represents actions in the main menu related to sound and music playback.
/// Provides methods to play different themes or sounds for various menu sections.
/// </summary>
public class MainMenuAction : MonoBehaviour
{

    /// <summary>
    /// Invoked when the script instance is being loaded.
    /// Automatically plays the main theme music.
    /// </summary>
    private void Awake()
    {
        PlayMainTheme();
    }

    /// <summary>
    /// Plays the main theme music.
    /// </summary>
    public void PlayMainTheme()
    {
        AudioSystem.Instance.PlayMusic(Music.MainTheme);
    }

    /// <summary>
    /// Plays the music for the one player menu section.
    /// </summary>
    public void PlayOnePlayerMenuMusic()
    {
        AudioSystem.Instance.PlayMusic(Music.OnePlayerMenu);
    }

    /// <summary>
    /// Plays the music for the two player menu section.
    /// </summary>
    public void PlayTwoPlayerMenuMusic()
    {
        AudioSystem.Instance.PlayMusic(Music.TwoPlayerMenu);
    }

    /// <summary>
    /// Plays the music for the options menu section.
    /// </summary>
    public void PlayOptionMenuMusic()
    {
        AudioSystem.Instance.PlayMusic(Music.OptionMenu);
    }
    
    /// <summary>
    /// Plays the transition sound effect.
    /// </summary>
    public void PlayTransitionSound()
    {
        AudioSystem.Instance.PlayTransitionSound();
    }

    /// <summary>
    /// Plays the back navigation sound effect.
    /// </summary>
    public void PlayBackSound()
    {
        AudioSystem.Instance.PlayBackSound();
    }
}

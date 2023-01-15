using Sound;
using UnityEngine;

public class MainMenuAction : MonoBehaviour
{

    private void Awake()
    {
        SoundManager.Instance.PlayMusic(Music.MainTheme);
    }

    public void PlayMainTheme()
    {
        SoundManager.Instance.PlayMusic(Music.MainTheme);
    }

    public void PlayOnePlayerMenuMusic()
    {
        SoundManager.Instance.PlayMusic(Music.OnePlayerMenu);
    }

    public void PlayTwoPlayerMenuMusic()
    {
        SoundManager.Instance.PlayMusic(Music.TwoPlayerMenu);
    }

    public void PlayOptionMenuMusic()
    {
        SoundManager.Instance.PlayMusic(Music.OptionMenu);
    }
    
    public void PlayTransitionSound()
    {
        SoundManager.Instance.PlayTransitionSound();
    }

    public void PlayBackSound()
    {
        SoundManager.Instance.PlayBackSound();
    }


}

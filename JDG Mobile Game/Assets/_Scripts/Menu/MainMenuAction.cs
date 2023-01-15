using Sound;
using UnityEngine;

public class MainMenuAction : MonoBehaviour
{

    private void Awake()
    {
        AudioSystem.Instance.PlayMusic(Music.MainTheme);
    }

    public void PlayMainTheme()
    {
        AudioSystem.Instance.PlayMusic(Music.MainTheme);
    }

    public void PlayOnePlayerMenuMusic()
    {
        AudioSystem.Instance.PlayMusic(Music.OnePlayerMenu);
    }

    public void PlayTwoPlayerMenuMusic()
    {
        AudioSystem.Instance.PlayMusic(Music.TwoPlayerMenu);
    }

    public void PlayOptionMenuMusic()
    {
        AudioSystem.Instance.PlayMusic(Music.OptionMenu);
    }
    
    public void PlayTransitionSound()
    {
        AudioSystem.Instance.PlayTransitionSound();
    }

    public void PlayBackSound()
    {
        AudioSystem.Instance.PlayBackSound();
    }


}

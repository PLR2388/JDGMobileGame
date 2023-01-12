using UnityEngine;

public class MainMenuAction : MonoBehaviour
{

    private void Awake()
    {
        SoundManager.Instance.PlayMusic(Music.MainTheme);
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

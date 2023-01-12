using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [SerializeField] private Music[] musicNames;
    [SerializeField] private AudioClip[] musics;

    [SerializeField] private SoundEffect[] soundEffectName;

    [SerializeField] private AudioClip[] soundEffect;
    // Start is called before the first frame update

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    public float MusicVolume => musicAudioSource.volume;
    public float SoundEffectVolume => soundEffectAudioSource.volume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int IndexMusic(Music music)
    {
        return Array.IndexOf(musicNames, music);
    }

    private int IndexSound(SoundEffect soundEffect)
    {
        return Array.IndexOf(soundEffectName, soundEffect);
    }

    public void PlayMusic(Music music)
    {
        musicAudioSource.clip = musics[IndexMusic(music)];
        musicAudioSource.Play();
    }

    public void ChangeSoundEffectVolume(float value)
    {
        soundEffectAudioSource.volume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        musicAudioSource.volume = value;
    }

    public void PlayTransitionSound()
    {
        soundEffectAudioSource.PlayOneShot(soundEffect[IndexSound(SoundEffect.Transition)]);
    }

    public void PlayBackSound()
    {
        soundEffectAudioSource.PlayOneShot(soundEffect[IndexSound(SoundEffect.Transition)]);
    }
}
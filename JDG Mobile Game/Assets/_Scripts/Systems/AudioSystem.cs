using System;
using UnityEngine;

namespace Sound
{
    public class AudioSystem : StaticInstance<AudioSystem>
    {
        [SerializeField] private Music[] musicNames;
        [SerializeField] private AudioClip[] musics;

        [SerializeField] private SoundEffect[] soundEffectName;
        [SerializeField] private AudioClip[] soundEffect;

        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource soundEffectAudioSource;

        public float MusicVolume => musicAudioSource.volume;
        public float SoundEffectVolume => soundEffectAudioSource.volume;

        private int IndexMusic(Music music)
        {
            return Array.IndexOf(musicNames, music);
        }

        private int IndexSound(SoundEffect soundEffectSound)
        {
            return Array.IndexOf(soundEffectName, soundEffectSound);
        }

        public void StopMusic()
        {
            musicAudioSource.Stop();
        }

        public void PlayMusic(Music music)
        {
            var audioClipToPlay = musics[IndexMusic(music)];
            if (musicAudioSource.clip != audioClipToPlay)
            {
                musicAudioSource.clip = musics[IndexMusic(music)];
                musicAudioSource.Play();
            }
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
}
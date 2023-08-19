using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
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

        public Dictionary<CardFamily, Music> FamilyMusic = new Dictionary<CardFamily, Music>
        {
            [CardFamily.Any] = Music.DrawPhase,
           [CardFamily.Comics] = Music.CanardCity,
           [CardFamily.Developer] = Music.DrawPhase,
           [CardFamily.Fistiland] = Music.DrawPhase,
           [CardFamily.Human] = Music.DrawPhase,
           [CardFamily.Incarnation] = Music.DrawPhase,
           [CardFamily.Japan] = Music.DrawPhase,
           [CardFamily.Monster] = Music.DrawPhase,
           [CardFamily.Police] = Music.DrawPhase,
           [CardFamily.Rpg] = Music.Rpg,
           [CardFamily.Spatial] = Music.DrawPhase,
           [CardFamily.Wizard] =Music.Wizard,
           [CardFamily.HardCorner] = Music.DrawPhase
        };

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

        public void PlayFamilyMusic(CardFamily family)
        {
            PlayMusic(FamilyMusic[family]);
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
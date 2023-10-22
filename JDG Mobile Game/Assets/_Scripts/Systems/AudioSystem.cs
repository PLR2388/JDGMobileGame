using System;
using System.Collections.Generic;
using Cards;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// Manages the playback and control of audio, including both music and sound effects.
    /// </summary>
    public class AudioSystem : StaticInstance<AudioSystem>
    {
        [SerializeField] private Music[] musicNames;
        [SerializeField] private AudioClip[] musics;

        [SerializeField] private SoundEffect[] soundEffectName;
        [SerializeField] private AudioClip[] soundEffect;

        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource soundEffectAudioSource;

        /// <summary>
        /// Gets the volume level for the currently playing music.
        /// </summary>
        public float MusicVolume => musicAudioSource.volume;
        
        /// <summary>
        /// Gets the volume level for the currently playing sound effect.
        /// </summary>
        public float SoundEffectVolume => soundEffectAudioSource.volume;

        private readonly Dictionary<CardFamily, Music> familyMusic = new Dictionary<CardFamily, Music>
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

        /// <summary>
        /// Retrieves the index of the specified music from the musicNames array.
        /// </summary>
        /// <param name="music">The music for which the index is required.</param>
        /// <returns>The index of the specified music in the musicNames array, or -1 if not found.</returns>
        private int IndexMusic(Music music)
        {
            return Array.IndexOf(musicNames, music);
        }

        /// <summary>
        /// Retrieves the index of the specified sound effect from the soundEffectName array.
        /// </summary>
        /// <param name="soundEffectSound">The sound effect for which the index is required.</param>
        /// <returns>The index of the specified sound effect in the soundEffectName array, or -1 if not found.</returns>
        private int IndexSound(SoundEffect soundEffectSound)
        {
            return Array.IndexOf(soundEffectName, soundEffectSound);
        }

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public void StopMusic()
        {
            musicAudioSource.Stop();
        }

        /// <summary>
        /// Plays the music associated with a given card family.
        /// </summary>
        /// <param name="family">The card family to play the music for.</param>
        public void PlayFamilyMusic(CardFamily family)
        {
            PlayMusic(familyMusic[family]);
        }

        /// <summary>
        /// Plays a specific music track.
        /// </summary>
        /// <param name="music">The music to play.</param>
        public void PlayMusic(Music music)
        {
            var audioClipToPlay = musics[IndexMusic(music)];
            if (musicAudioSource.clip != audioClipToPlay)
            {
                musicAudioSource.clip = musics[IndexMusic(music)];
                musicAudioSource.Play();
            }
        }

        /// <summary>
        /// Adjusts the volume of sound effects.
        /// </summary>
        /// <param name="value">The desired volume level.</param>
        public void ChangeSoundEffectVolume(float value)
        {
            soundEffectAudioSource.volume = value;
        }

        /// <summary>
        /// Adjusts the volume of the music.
        /// </summary>
        /// <param name="value">The desired volume level.</param>
        public void ChangeMusicVolume(float value)
        {
            musicAudioSource.volume = value;
        }

        /// <summary>
        /// Plays the transition sound effect.
        /// </summary>
        public void PlayTransitionSound()
        {
            soundEffectAudioSource.PlayOneShot(soundEffect[IndexSound(SoundEffect.Transition)]);
        }
        
        /// <summary>
        /// Plays the back sound effect.
        /// </summary>
        public void PlayBackSound()
        {
            soundEffectAudioSource.PlayOneShot(soundEffect[IndexSound(SoundEffect.Transition)]);
        }
    }
}
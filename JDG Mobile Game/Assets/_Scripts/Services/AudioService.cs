using Cards;
using JDG.Application.Services;
using Sound;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IAudioService.
    /// Wraps the existing AudioSystem singleton during migration.
    /// Uses Strangler Fig pattern - delegates to AudioSystem.Instance temporarily.
    /// </summary>
    public class AudioService : IAudioService
    {
        private readonly AudioSystem _audioSystem;

        public AudioService()
        {
            // During migration, get the existing singleton
            // TODO: Later, inject AudioSystem dependencies directly
            _audioSystem = AudioSystem.Instance;
        }

        public void PlayMusic(string musicName)
        {
            if (System.Enum.TryParse<Music>(musicName, true, out var music))
            {
                _audioSystem.PlayMusic(music);
            }
            else
            {
                Debug.LogWarning($"AudioService: Music '{musicName}' not found");
            }
        }

        public void StopMusic()
        {
            _audioSystem.StopMusic();
        }

        public void PlaySoundEffect(string sfxName)
        {
            // Map common sound effects
            switch (sfxName.ToLower())
            {
                case "transition":
                    _audioSystem.PlayTransitionSound();
                    break;
                case "back":
                    _audioSystem.PlayBackSound();
                    break;
                default:
                    Debug.LogWarning($"AudioService: Sound effect '{sfxName}' not found");
                    break;
            }
        }

        public void SetMasterVolume(float volume)
        {
            // AudioSystem doesn't have separate master volume
            // Apply to both music and sfx
            SetMusicVolume(volume);
            SetSfxVolume(volume);
        }

        public void SetMusicVolume(float volume)
        {
            _audioSystem.ChangeMusicVolume(Mathf.Clamp01(volume));
        }

        public void SetSfxVolume(float volume)
        {
            _audioSystem.ChangeSoundEffectVolume(Mathf.Clamp01(volume));
        }

        public float GetMusicVolume()
        {
            return _audioSystem.MusicVolume;
        }

        public float GetSfxVolume()
        {
            return _audioSystem.SoundEffectVolume;
        }

        /// <summary>
        /// Plays music associated with a card family.
        /// Phase 37: Added to support CardPlacementService DI migration.
        /// </summary>
        /// <param name="family">The card family to play music for.</param>
        public void PlayFamilyMusic(object family)
        {
            if (family is CardFamily cardFamily)
            {
                _audioSystem.PlayFamilyMusic(cardFamily);
            }
            else
            {
                Debug.LogWarning($"AudioService: Invalid family type: {family?.GetType().Name ?? "null"}");
            }
        }

        // ============================================
        // Phase 8: Convenience methods for singleton migration
        // ============================================

        /// <summary>
        /// Plays the transition sound effect.
        /// </summary>
        public void PlayTransitionSound()
        {
            _audioSystem.PlayTransitionSound();
        }

        /// <summary>
        /// Plays the back navigation sound effect.
        /// </summary>
        public void PlayBackSound()
        {
            _audioSystem.PlayBackSound();
        }
    }
}

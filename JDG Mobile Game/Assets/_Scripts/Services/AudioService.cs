using Cards;
using JDG.Application.Services;
using Sound;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IAudioService.
    /// Phase 8: Created to replace AudioSystem.Instance direct access.
    /// Phase 84: Uses lazy access to AudioSystem for proper initialization timing.
    /// Note: AudioSystem remains as a MonoBehaviour because AudioSource requires it.
    /// Once all direct AudioSystem.Instance calls are removed, AudioSystem can be
    /// converted to a non-singleton scene component.
    /// </summary>
    public class AudioService : IAudioService
    {
        // Lazy access to AudioSystem - avoids constructor timing issues
        // AudioSystem must exist in scene for audio to work
        private AudioSystem AudioSystem
        {
            get
            {
#pragma warning disable CS0618 // Suppress obsolete warning - AudioSystem needed for AudioSource
                return AudioSystem.Instance;
#pragma warning restore CS0618
            }
        }

        public AudioService()
        {
            // Empty constructor - lazy access handles timing
        }

        public void PlayMusic(string musicName)
        {
            var audioSystem = AudioSystem;
            if (audioSystem == null)
            {
                Debug.LogWarning($"AudioService: AudioSystem not available");
                return;
            }

            if (System.Enum.TryParse<Music>(musicName, true, out var music))
            {
                audioSystem.PlayMusic(music);
            }
            else
            {
                Debug.LogWarning($"AudioService: Music '{musicName}' not found");
            }
        }

        public void StopMusic()
        {
            var audioSystem = AudioSystem;
            if (audioSystem != null)
            {
                audioSystem.StopMusic();
            }
        }

        public void PlaySoundEffect(string sfxName)
        {
            var audioSystem = AudioSystem;
            if (audioSystem == null)
            {
                Debug.LogWarning($"AudioService: AudioSystem not available");
                return;
            }

            switch (sfxName.ToLower())
            {
                case "transition":
                    audioSystem.PlayTransitionSound();
                    break;
                case "back":
                    audioSystem.PlayBackSound();
                    break;
                default:
                    Debug.LogWarning($"AudioService: Sound effect '{sfxName}' not found");
                    break;
            }
        }

        public void SetMasterVolume(float volume)
        {
            SetMusicVolume(volume);
            SetSfxVolume(volume);
        }

        public void SetMusicVolume(float volume)
        {
            var audioSystem = AudioSystem;
            if (audioSystem != null)
            {
                audioSystem.ChangeMusicVolume(Mathf.Clamp01(volume));
            }
        }

        public void SetSfxVolume(float volume)
        {
            var audioSystem = AudioSystem;
            if (audioSystem != null)
            {
                audioSystem.ChangeSoundEffectVolume(Mathf.Clamp01(volume));
            }
        }

        public float GetMusicVolume()
        {
            var audioSystem = AudioSystem;
            return audioSystem != null ? audioSystem.MusicVolume : 0f;
        }

        public float GetSfxVolume()
        {
            var audioSystem = AudioSystem;
            return audioSystem != null ? audioSystem.SoundEffectVolume : 0f;
        }

        /// <summary>
        /// Plays music associated with a card family.
        /// Phase 37: Added to support CardPlacementService DI migration.
        /// </summary>
        public void PlayFamilyMusic(object family)
        {
            var audioSystem = AudioSystem;
            if (audioSystem == null)
            {
                Debug.LogWarning($"AudioService: AudioSystem not available");
                return;
            }

            if (family is CardFamily cardFamily)
            {
                audioSystem.PlayFamilyMusic(cardFamily);
            }
            else
            {
                Debug.LogWarning($"AudioService: Invalid family type: {family?.GetType().Name ?? "null"}");
            }
        }

        /// <summary>
        /// Plays the transition sound effect.
        /// </summary>
        public void PlayTransitionSound()
        {
            var audioSystem = AudioSystem;
            if (audioSystem != null)
            {
                audioSystem.PlayTransitionSound();
            }
        }

        /// <summary>
        /// Plays the back navigation sound effect.
        /// </summary>
        public void PlayBackSound()
        {
            var audioSystem = AudioSystem;
            if (audioSystem != null)
            {
                audioSystem.PlayBackSound();
            }
        }
    }
}

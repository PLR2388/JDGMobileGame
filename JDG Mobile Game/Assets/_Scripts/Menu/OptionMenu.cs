using JDG.Application.Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Menu
{
    /// <summary>
    /// Manages the options menu for audio settings.
    /// Phase 8: Migrated from AudioSystem.Instance to IAudioService DI.
    /// </summary>
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private Slider volumeMusicSlider;
        [SerializeField] private Slider soundEffectSlider;

        private IAudioService _audioService;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Phase 8: Inject IAudioService instead of using AudioSystem.Instance.
        /// </summary>
        [Inject]
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        // Start is called before the first frame update
        private void Start()
        {
            InitializeSliders();
        }

        /// <summary>
        /// Initializes the sliders with the current audio system values and adds listeners.
        /// </summary>
        private void InitializeSliders()
        {
            var audioService = GetAudioService();
            volumeMusicSlider.value = audioService.GetMusicVolume();
            soundEffectSlider.value = audioService.GetSfxVolume();

            volumeMusicSlider.onValueChanged.AddListener(MusicVolumeChanged);
            soundEffectSlider.onValueChanged.AddListener(SoundEffectVolumeChanged);
        }

        /// <summary>
        /// Updates the music volume in the audio system.
        /// </summary>
        /// <param name="value">The new volume value.</param>
        private void MusicVolumeChanged(float value)
        {
            GetAudioService().SetMusicVolume(value);
        }

        /// <summary>
        /// Updates the sound effect volume in the audio system.
        /// </summary>
        /// <param name="value">The new volume value.</param>
        private void SoundEffectVolumeChanged(float value)
        {
            GetAudioService().SetSfxVolume(value);
        }

        /// <summary>
        /// Gets the audio service. Throws if DI not configured.
        /// Phase 144: Removed fallback - DI must be properly configured.
        /// </summary>
        private IAudioService GetAudioService()
        {
            if (_audioService == null)
            {
                throw new System.InvalidOperationException(
                    "[OptionMenu] IAudioService not injected. " +
                    "Ensure VContainer is configured and OptionMenu is injected via LifetimeScope.");
            }
            return _audioService;
        }
    }
}
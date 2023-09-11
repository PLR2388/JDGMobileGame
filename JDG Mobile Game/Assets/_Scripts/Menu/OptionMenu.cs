using Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private Slider volumeMusicSlider;
        [SerializeField] private Slider soundEffectSlider;
        
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
            volumeMusicSlider.value = AudioSystem.Instance.MusicVolume;
            soundEffectSlider.value = AudioSystem.Instance.SoundEffectVolume;

            volumeMusicSlider.onValueChanged.AddListener(MusicVolumeChanged);
            soundEffectSlider.onValueChanged.AddListener(SoundEffectVolumeChanged);
        }

        
        /// <summary>
        /// Updates the music volume in the audio system.
        /// </summary>
        /// <param name="value">The new volume value.</param>
        private void MusicVolumeChanged(float value)
        {
            AudioSystem.Instance.ChangeMusicVolume(value);
        }
        
        /// <summary>
        /// Updates the sound effect volume in the audio system.
        /// </summary>
        /// <param name="value">The new volume value.</param>
        private void SoundEffectVolumeChanged(float value)
        {
            AudioSystem.Instance.ChangeSoundEffectVolume(value);
        }
    }
}
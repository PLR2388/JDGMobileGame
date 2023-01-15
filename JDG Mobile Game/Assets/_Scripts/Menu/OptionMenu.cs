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
            volumeMusicSlider.value = AudioSystem.Instance.MusicVolume;
            soundEffectSlider.value = AudioSystem.Instance.SoundEffectVolume;
            Slider.SliderEvent sliderMusicEvent = new Slider.SliderEvent();
            Slider.SliderEvent sliderSoundEffectEvent = new Slider.SliderEvent();
            
            sliderMusicEvent.AddListener(MusicVolumeChanged);
            sliderSoundEffectEvent.AddListener(SoundEffectVolumeChanged);

            volumeMusicSlider.onValueChanged = sliderMusicEvent;
            soundEffectSlider.onValueChanged = sliderSoundEffectEvent;
        }

        private void MusicVolumeChanged(float value)
        {
            AudioSystem.Instance.ChangeMusicVolume(value);
        }
        
        private void SoundEffectVolumeChanged(float value)
        {
            AudioSystem.Instance.ChangeSoundEffectVolume(value);
        }
    }
}
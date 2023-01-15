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
            volumeMusicSlider.value = SoundManager.Instance.MusicVolume;
            soundEffectSlider.value = SoundManager.Instance.SoundEffectVolume;
            Slider.SliderEvent sliderMusicEvent = new Slider.SliderEvent();
            Slider.SliderEvent sliderSoundEffectEvent = new Slider.SliderEvent();
            
            sliderMusicEvent.AddListener(MusicVolumeChanged);
            sliderSoundEffectEvent.AddListener(SoundEffectVolumeChanged);

            volumeMusicSlider.onValueChanged = sliderMusicEvent;
            soundEffectSlider.onValueChanged = sliderSoundEffectEvent;
        }

        private void MusicVolumeChanged(float value)
        {
            SoundManager.Instance.ChangeMusicVolume(value);
        }
        
        private void SoundEffectVolumeChanged(float value)
        {
            SoundManager.Instance.ChangeSoundEffectVolume(value);
        }
    }
}
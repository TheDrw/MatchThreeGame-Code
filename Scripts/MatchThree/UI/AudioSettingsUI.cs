using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using MatchThree.Helpers;
using MatchThree.Settings;
using MatchThree.Audio;

namespace MatchThree.UI
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] GameSettings gameSettings;
        [SerializeField] AudioMixer audioMixer;

        [Header("SFX settings")]
        [SerializeField] Image soundImage;
        [SerializeField] Sprite soundOnSprite;
        [SerializeField] Sprite soundOffSprite;
        [SerializeField] Slider soundSlider;

        [Header("Music settings")]
        [SerializeField] Image musicImage;
        [SerializeField] Sprite musicOnSprite;
        [SerializeField] Sprite musicOffSprite;
        [SerializeField] Slider musicSlider;

        static readonly float ON_VALUE = 10f, OFF_VALUE = 0f;

        private void OnEnable()
        {
            LoadVolumeSettings();
        }

        public void ToggleSound()
        {
            if(gameSettings.IsSFXEnabled)
            {
                gameSettings.SetSFXVolume(OFF_VALUE);
                soundSlider.value = OFF_VALUE;
            }
            else
            {
                gameSettings.SetSFXVolume(ON_VALUE);
                soundSlider.value = ON_VALUE;

            }

            soundImage.sprite = gameSettings.IsSFXEnabled ? soundOnSprite : soundOffSprite;
        }

        public void ToggleMusic()
        {
            if(gameSettings.IsMusicEnabled)
            {
                gameSettings.SetMusicVolume(OFF_VALUE);
                musicSlider.value = OFF_VALUE;
            }
            else
            {
                gameSettings.SetMusicVolume(ON_VALUE);
                musicSlider.value = ON_VALUE;
            }

            musicImage.sprite = gameSettings.IsMusicEnabled ? musicOnSprite : musicOffSprite;
        }

        public void SetSoundVolumeFromSlider(float val)
        {
            gameSettings.SetSFXVolume(val);
            soundImage.sprite =gameSettings.IsSFXEnabled ?  soundOnSprite : soundOffSprite;
            MenuSFX.Play?.ButtonSelected();
        }

        public void SetMusicVolumeFromSlider(float val)
        {
            gameSettings.SetMusicVolume(val);
            musicImage.sprite = gameSettings.IsMusicEnabled ? musicOnSprite : musicOffSprite;
            MenuSFX.Play?.ButtonSelected();
        }

        void LoadVolumeSettings()
        {
            soundImage.sprite = gameSettings.IsSFXEnabled ? soundOnSprite : soundOffSprite;
            soundSlider.value = gameSettings.GetSFXVolume();

            musicImage.sprite = gameSettings.IsMusicEnabled ? musicOnSprite : musicOffSprite;
            musicSlider.value = gameSettings.GetMusicVolume();
        }
    }
}
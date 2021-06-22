using MatchThree.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MatchThree.Data;
using System.IO;

namespace MatchThree.Settings
{
    [CreateAssetMenu(fileName = "Data", menuName = "Extra/Settings")]
    public class GameSettings : ScriptableObject, ISaveAndLoad, ILoadedAsync
    {
        [SerializeField] AudioMixer mixer;
        [SerializeField] SettingsData settings;

        readonly string file = "settings";
        static readonly string soundVolumeStr = "soundVolume", musicVolumeStr = "musicVolume";

        public void Load()
        {
            DataSave.Load(file, settings, this);

            SetSFXVolume(settings.VolumeSFX);
            SetMusicVolume(settings.VolumeMusic);
        }

        public void LoadAsyncSuccess()
        {
            SetSFXVolume(settings.VolumeSFX);
            SetMusicVolume(settings.VolumeMusic);
        }

        public void Save()
        {
            DataSave.Save(file, settings);
        }

        public void Reset()
        {
            settings.Reset();

            SetSFXVolume(settings.VolumeSFX);
            SetMusicVolume(settings.VolumeMusic);
        }

        /// <summary>
        /// Value is teh slider value from the UI, mainly whole numbers from 0-whatever
        /// </summary>
        /// <param name="val"></param>
        public void SetSFXVolume(float val)
        {
            settings.VolumeSFX = (int)val;
            settings.SFXEnabled = settings.VolumeSFX > 0;

            float dec = GeneralHelp.ValueToDecibelConversion(val);
            mixer.SetFloat(soundVolumeStr, dec);
        }

        public float GetSFXVolume() => settings.VolumeSFX;

        public void SetMusicVolume(float val)
        {
            settings.VolumeMusic = (int)val;
            settings.MusicEnabled = settings.VolumeMusic > 0;

            float dec = GeneralHelp.ValueToDecibelConversion(val);
            mixer.SetFloat(musicVolumeStr, dec);
        }

        public float GetMusicVolume() => settings.VolumeMusic;

        public bool IsMusicEnabled => settings.MusicEnabled;
        public bool IsSFXEnabled => settings.SFXEnabled;

        public int Difficulty
        {
            get { return settings.Difficulty; }
            set { settings.Difficulty = value; }
        }

        public bool EnableBoardAssist
        {
            get { return settings.EnableBoardAssist; }
            set { settings.EnableBoardAssist = value; }
        }

        public bool EnableVFX
        {
            get { return settings.EnableVFX; }
            set { settings.EnableVFX = value; }
        }

        public int Quality
        {
            get { return settings.Quality; }
            set { settings.Quality = value; }
        }

        public string PlayerName
        {
            get { return settings.PlayerName; }
            set { settings.PlayerName = value; }
        }

        public bool IsOnline
        {
            get { return settings.IsOnline; }
            set { settings.IsOnline = value; }
        }
    }
}
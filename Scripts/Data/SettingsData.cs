using System;
using UnityEngine;

namespace MatchThree.Data
{
    [Serializable]
    public class SettingsData
    {
        [Header("Gameplay")]
        public bool EnableBoardAssist = true;
        [Range(0, 10)]
        public int Difficulty = 5;

        [Header("Audio")]
        [Range(0, 10)]
        public int VolumeSFX = 10;
        public bool SFXEnabled = true;

        [Space(8)]
        [Range(0, 10)]
        public int VolumeMusic = 10;
        public bool MusicEnabled = true;

        [Header("Graphics")]
        public bool EnableVFX = true;
        [Range(0, 4)]
        public int Quality = 2;

        [Header("Online")]
        public string PlayerName = "";
        public bool IsOnline = false;

        public void Reset()
        {
            EnableBoardAssist = true;
            Difficulty = 5;

            VolumeSFX = 10;
            SFXEnabled = true;

            VolumeMusic = 10;
            MusicEnabled = true;

            EnableVFX = true;
            Quality = 2;
        }
    }
}
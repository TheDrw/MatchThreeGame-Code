using MatchThree.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree.UI
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] GameSettings settings;
        [SerializeField] Slider difficultySlider;
        [SerializeField] Toggle boardAssistToggle;


        private void OnEnable()
        {
            difficultySlider.value = settings.Difficulty;
            boardAssistToggle.isOn = settings.EnableBoardAssist;
        }

        public void SetDifficulty(float val)
        {
            settings.Difficulty = (int)val;
        }

        public void EnableBoardAssist(bool val)
        {
            settings.EnableBoardAssist = val;
        }

        public void ResetUI()
        {
            difficultySlider.value = settings.Difficulty;
            boardAssistToggle.isOn = settings.EnableBoardAssist;
        }
    }
}
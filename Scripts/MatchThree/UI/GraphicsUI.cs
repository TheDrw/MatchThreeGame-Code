using MatchThree.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree.UI
{
    public class GraphicsUI : MonoBehaviour
    {
        [SerializeField] GameSettings settings;
        [SerializeField] Toggle VisualEffectsToggle;
        [SerializeField] Toggle BackgroundToggle;

        private void OnEnable()
        {
            VisualEffectsToggle.isOn = settings.EnableVFX;
            BackgroundToggle.isOn = settings.EnableBackground;
        }

        public void EnableVFX(bool val)
        {
            settings.EnableVFX = val;
        }

        public void EnableBackground(bool val)
        {
            settings.EnableBackground = val;
        }
    }
}
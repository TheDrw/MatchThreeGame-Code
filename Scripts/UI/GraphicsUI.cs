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

        private void OnEnable()
        {
            VisualEffectsToggle.isOn = settings.EnableVFX;
        }

        public void EnableVFX(bool val)
        {
            settings.EnableVFX = val;
        }
    }
}
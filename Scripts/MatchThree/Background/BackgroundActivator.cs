using MatchThree.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Background
{
    public class BackgroundActivator : MonoBehaviour
    {
        [SerializeField] GameSettings settings;
        [SerializeField] GameObject rootScene;

        private void Start()
        {
            rootScene.SetActive(settings.EnableBackground);

            settings.OnBackgroundActiveChanged += Activator;
        }

        private void OnDestroy()
        {
            settings.OnBackgroundActiveChanged -= Activator;
        }

        void Activator(bool active)
        {
            rootScene.SetActive(active);
        }
    }
}
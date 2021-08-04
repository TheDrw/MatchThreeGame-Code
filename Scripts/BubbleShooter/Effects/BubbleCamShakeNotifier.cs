using BubbleShooter.Controller;
using MatchThree.Cam;
using MatchThree.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Effects
{
    public class BubbleCamShakeNotifier : MonoBehaviour
    {
        [SerializeField] GameSettings settings;
        [SerializeField] MainCamera mainCam;
        [SerializeField] BubbleShooterController[] controllers;

        private void Start()
        {
            if (controllers.Length == 0)
            {
                controllers = FindObjectsOfType<BubbleShooterController>();
            }

            ConnectControllerEvents();
        }

        private void OnDestroy()
        {
            DisconnectControllerEvents();
        }

        private void ConnectControllerEvents()
        {
            foreach (var controller in controllers)
            {
                controller.Board.OnMatchFound += NotifyShake;
            }
        }

        private void DisconnectControllerEvents()
        {
            foreach (var controller in controllers)
            {
                controller.Board.OnMatchFound -= NotifyShake;
            }
        }

        void NotifyShake(int count)
        {
            if (!settings.EnableVFX) return;

            float intensity = IntensityCaclulator(count);
            mainCam.Shake(intensity);
        }

        private  float IntensityCaclulator(int count)
        {
            float intensity = 0;

            if (count >= 4 && count < 7) intensity = 1f;
            else if (count >= 7 && count < 10) intensity = 5f;
            else if (count >= 10) intensity = 10f;
            return intensity;
        }
    }
}
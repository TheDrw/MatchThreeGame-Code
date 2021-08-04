using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Effects;

namespace MatchThree.Cam
{
    [RequireComponent(typeof(Camera))]
    public class UICamera : MonoBehaviour
    {
        [SerializeField] TransitionsEffect transitions;

        public Camera Cam { get; private set; } = null;

        private void Awake()
        {
            Cam = GetComponent<Camera>();
            Cam.enabled = false;
        }

        private void Start()
        {
            transitions.OnFadeOutFinished += EnableUICamera;
            transitions.OnFadeInStarted += DisableUICamera;
        }

        private void OnDestroy()
        {
            transitions.OnFadeOutFinished -= EnableUICamera;
            transitions.OnFadeInStarted -= DisableUICamera;
        }

        void EnableUICamera()
        {
            Cam.enabled = true;
        }

        void DisableUICamera()
        {
            Cam.enabled = false;
        }
    }
}
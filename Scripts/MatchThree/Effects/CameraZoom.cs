using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MatchThree.Cam;

namespace MatchThree.Effects
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] MainCamera cam;
        InputActions inputActions = null;


        private void Awake()
        {
            inputActions = new InputActions();
        }

        private void Start()
        {
            if(cam == null)
            {
                cam = Camera.main.GetComponent<MainCamera>();
            }

            inputActions.Player.CameraZoom.performed += ctx => ToggleZoom();
            inputActions.Enable();
        }

        private void OnDestroy()
        {
            inputActions.Player.CameraZoom.performed -= ctx => ToggleZoom();
            inputActions.Dispose();
        }

        void ToggleZoom()
        {
            cam.Zoom();
        }

    }
}
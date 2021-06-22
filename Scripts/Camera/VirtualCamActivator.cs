using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace MatchThree.Cam
{
    public class VirtualCamActivator : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera vcam;

        static readonly int ON = 100, OFF = -100;


        private void Awake()
        {
            if(vcam == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("WRN : Missing vcam. Disabling this script.", this);
#endif
                enabled = false;
            }
        }


        private void OnEnable()
        {
            if (vcam == null) return;

            vcam.Priority = ON;
        }

        private void OnDisable()
        {
            if (vcam == null) return;

            vcam.Priority = OFF;
        }
    }
}
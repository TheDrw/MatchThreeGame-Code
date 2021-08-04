using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Core
{
    public class FramerateLockInEditor : MonoBehaviour
    {
#if UNITY_EDITOR
        float fps = 0;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            fps = (int)(1f / Time.unscaledDeltaTime);
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 20, 150, 20), $"FPS : {fps:00.00}");
        }
#endif
    }
}
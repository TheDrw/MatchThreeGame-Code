using DG.Tweening;
using UnityEngine;

namespace MatchThree.Cam
{
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] Camera camUI;

        [Header("Zoom")]
        [SerializeField] float duration = .25f;
        [SerializeField] float minZoom = 6f;
        [SerializeField] float maxZoom = 12f;

        Camera cam;

        float zoomTarget = 0f;
        bool zoomFlag = false;

        bool canZoom = true;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            zoomTarget = minZoom;
        }
        
        public void Zoom()
        {
            if (!canZoom) return;

            camUI.DOOrthoSize(zoomTarget, duration);
            cam.DOOrthoSize(zoomTarget, duration)
                .OnComplete(() => ZoomFinished());
            
        }

        void ZoomFinished()
        {
            canZoom = true;
            zoomTarget = zoomFlag ? minZoom : maxZoom;
            zoomFlag = !zoomFlag;
        }
    }
}
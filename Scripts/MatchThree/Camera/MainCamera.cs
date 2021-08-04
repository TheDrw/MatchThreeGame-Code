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

        Vector3 initPos;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            zoomTarget = minZoom;

            initPos = transform.position;
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

        public void Shake(float intensityMultiplier)
        {
            var vectorIntensity = new Vector3(.1f, .1f, 0) * intensityMultiplier;
            transform
                .DOShakePosition(0.15f, vectorIntensity, 100)
                .OnComplete(() => transform.position = initPos);
        }
    }
}
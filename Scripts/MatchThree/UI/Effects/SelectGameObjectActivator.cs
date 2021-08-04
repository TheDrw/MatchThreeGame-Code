using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MatchThree.UI
{
    public class SelectGameObjectActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GameObject[] gameObjects;

        private void Start()
        {
            if(gameObjects.Length > 0)
            {
                TurnOffObjects();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("ERR - There are no gameObjects to activate. Just delete/disable this since it isn't being used.", this);
#endif
            }
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (gameObjects.Length == 0) return;

            TurnOnObjects();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (gameObjects.Length == 0) return;

            TurnOffObjects();
        }

        void TurnOnObjects()
        {
            foreach (var go in gameObjects)
            {
                go.SetActive(true);
                go.GetComponent<RectTransform>().DOScale(1, 0.15f).SetUpdate(true);
            }
        }

        void TurnOffObjects()
        {
            foreach (var go in gameObjects)
            {
                go.SetActive(false);
                go.GetComponent<RectTransform>().localScale = Vector3.zero;
            }
        }
    }
}

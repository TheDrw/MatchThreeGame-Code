using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MatchThree.UI.Effects
{
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(Image))]
    public class SelectorImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image image = null;

        private void Start()
        {
            image.color = new Color(1, 1, 1, 0);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = new Color(1, 1, 1, 0.7f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.color = new Color(1, 1, 1, 0);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using MatchThree.Audio;

namespace MatchThree.UI.Effects
{
    public class ButtonSelectEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
        IPointerDownHandler, IPointerUpHandler
    {

        [Tooltip("Words pop up on button when being highlighted - not required if not needed")]
        [SerializeField] GameObject textPopup;
        [Tooltip("Explanation words pop up on button when being highlighted - not required if not needed")]
        [SerializeField] GameObject explanationPopup;

        static readonly float endScale = 1.25f;
        static readonly float duration = 0.2f;

        private void Awake()
        {
            HidePopupText();
            HideExplanationText();
        }

        private void OnDisable()
        {
            HidePopupText();
            HideExplanationText();
            GetComponent<RectTransform>().localScale = Vector3.one;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PopupButton();
            ShowPopupText();
            ShowExplanationText();

            MenuSFX.Play?.ButtonSelected();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PopdownButton();
            HidePopupText();
            HideExplanationText();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MenuSFX.Play?.ButtonPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MenuSFX.Play?.ButtonReleased();
        }

        private void PopupButton()
        {
            GetComponent<RectTransform>()
                .DOScale(endScale, duration)
                .SetUpdate(true);
        }

        private void PopdownButton()
        {
            GetComponent<RectTransform>()
                .DOScale(1, duration)
                .SetUpdate(true);
        }

        private void HidePopupText()
        {
            if (textPopup != null)
            {
                textPopup.SetActive(false);
                textPopup.GetComponent<RectTransform>().localScale = Vector3.zero;
            }
        }

        private void ShowPopupText()
        {
            if (textPopup != null)
            {
                textPopup.SetActive(true);
                textPopup.GetComponent<RectTransform>().DOScale(1, 0.2f).SetUpdate(true);
            }
        }

        private void HideExplanationText()
        {
            if (explanationPopup != null)
            {
                explanationPopup.SetActive(false);
                explanationPopup.GetComponent<RectTransform>().localScale = Vector3.zero;
            }
        }

        private void ShowExplanationText()
        {
            if(explanationPopup != null)
            {
                explanationPopup.SetActive(true);
                explanationPopup.GetComponent<RectTransform>().DOScale(1, 0.2f).SetUpdate(true);
            }
        }
    }
}
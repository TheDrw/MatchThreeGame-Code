using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace BubbleShooter.UI
{
    public class BubbleControllerUI : MonoBehaviour
    {
        [SerializeField] GameObject infoCanvasObject;
        [SerializeField] GameObject scoreCanvasObject;

        [SerializeField] Image characterImage;

        [Header("Result info")]
        [SerializeField] GameObject resultImageEffectObject;
        [SerializeField] TMP_Text resultText;
        [SerializeField] Sprite loseSprite;

        private void Start()
        {
            resultImageEffectObject.SetActive(false);
            resultText.gameObject.SetActive(false);
        }

        public void Show()
        {
            infoCanvasObject.SetActive(true);
            infoCanvasObject.transform.localScale = Vector3.zero;
            infoCanvasObject.transform
                .DOScale(Vector3.one, .5f)
                .SetEase(Ease.OutBack);

            scoreCanvasObject.SetActive(true);
            scoreCanvasObject.transform.localScale = Vector3.zero;
            scoreCanvasObject.transform
                .DOScale(Vector3.one, .5f)
                .SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            infoCanvasObject.SetActive(false);
            scoreCanvasObject.SetActive(false);
        }

        public void DisplayWon()
        {
            infoCanvasObject.transform.DOPunchScale(Vector3.one * 1.15f, .5f, 2 ,.05f);
            resultImageEffectObject.SetActive(true);

            resultText.gameObject.SetActive(true);
            resultText.text = "Won!";
        }

        public void DisplayLost()
        {
            infoCanvasObject.transform.DOShakeRotation(.25f, 90, 100, 90);

            characterImage.sprite = loseSprite;

            resultText.gameObject.SetActive(true);
            resultText.text = "Lost!";
        }
    }
}
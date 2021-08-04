using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using BubbleShooter.Core;
using DG.Tweening;

namespace BubbleShooter.UI
{
    public class BubbleGameCountdownUI : MonoBehaviour
    {
        [Header("Countdown Section")]
        [SerializeField] TMP_Text countdownText;


        private void Start()
        {
            BubbleGameManager.OnGameCountdownStart += PerformCountdownText;
            countdownText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            BubbleGameManager.OnGameCountdownStart -= PerformCountdownText;
        }

        void PerformCountdownText()
        {
            countdownText.transform.localScale = Vector3.zero;
            countdownText.gameObject.SetActive(true);

            StartCoroutine(CountdownTextCoroutine());
        }

        IEnumerator CountdownTextCoroutine()
        {
            countdownText.text = "Ready?!";
            countdownText.transform.DOScale(Vector3.one, .25f).SetEase(Ease.OutBack);

            var wait = new WaitForSeconds(1f);
            yield return wait;

            for (int i = 3; i > 0; i--)
            {
                countdownText.text = $"{i}";
                countdownText.transform.DOPunchScale(Vector3.one * 1.25f, 0.2f);
                yield return wait;
            }

            countdownText.text = "Go!";
            countdownText.transform.DOPunchScale(Vector3.one * 1.5f, 0.2f);
            countdownText.transform.DOShakeRotation(.5f, 50, 5, 45);
            yield return new WaitForSeconds(.5f);

            countdownText.transform.DOLocalMoveY(Screen.height, 0.35f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(.5f);

            gameObject.SetActive(false);
        }
    }
}
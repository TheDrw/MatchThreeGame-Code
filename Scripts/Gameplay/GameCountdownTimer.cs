using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;
using MatchThree.Controllers;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using MatchThree.Settings;

namespace MatchThree.Gameplay
{
    public class GameCountdownTimer : MonoBehaviour
    {
        [SerializeField] Controller playerController;
        [SerializeField] GameManager GM;
        [SerializeField] Image countdownImg;
        [SerializeField] GameSettings settings;

        [Header("Texts")]
        [SerializeField] TMP_Text incrementedTimeText;
        [SerializeField] TMP_Text decrementedTimeText;

        [Header("Bonus")]
        [SerializeField] GameObject bonusAsWhole;
        [SerializeField] TMP_Text bonusTimeText;
        [SerializeField] GameObject bonusVFX;

        const float MAX_TIME = 10, INCREMENT_TIME = 2, DECREMENT_TIME = 1;
        float currTime = 0;

        Sequence decrementSeq = null, incrementSeq = null, bonusSeq = null;

        private IEnumerator Start()
        {
            incrementedTimeText.gameObject.SetActive(false);
            decrementedTimeText.gameObject.SetActive(false);
            bonusAsWhole.SetActive(false);

            playerController.OnControllerMadeGoodMove += IncrementTimer;
            playerController.OnControllerMadeBadMove += DecrementTimer;

            playerController.Scoreboard.OnLoopCombo += IncrementTimer;
            playerController.Gameboard.OnNoMorePossibleMatches += MaxOutTimer;
           

            GameManager.OnGameStart += BeginTimer;
            GameManager.OnGameFinished += EndTimer;

            countdownImg.fillAmount = 0;
            yield return new WaitForSeconds(1f);
            countdownImg.DOFillAmount(1, .5f).SetEase(Ease.OutCirc);
        }

        private void OnDisable()
        {
            playerController.OnControllerMadeGoodMove -= IncrementTimer;
            playerController.OnControllerMadeBadMove -= DecrementTimer;

            playerController.Scoreboard.OnLoopCombo -= IncrementTimer;
            playerController.Gameboard.OnNoMorePossibleMatches -= MaxOutTimer;

            GameManager.OnGameStart -= BeginTimer;
            GameManager.OnGameFinished -= EndTimer;
        }

        void BeginTimer()
        {
            StartCoroutine(TimerCoroutine());
        }

        IEnumerator TimerCoroutine()
        {
            currTime = MAX_TIME;
            while (currTime > 0)
            {
                currTime -= Time.deltaTime;

                countdownImg.fillAmount = currTime / MAX_TIME;
                yield return null;
            }
            countdownImg.fillAmount = 0;
            GM.EndGameEarly();
        }

        void IncrementTimer()
        {
            currTime = Mathf.Clamp(currTime + INCREMENT_TIME, 0, MAX_TIME);

            incrementedTimeText.gameObject.SetActive(true);

            incrementedTimeText.rectTransform.localScale = Vector3.zero;
            incrementedTimeText.text = $"+{INCREMENT_TIME}";


            if (incrementSeq != null)
            {
                incrementSeq.Kill();
            }

            incrementSeq = DOTween.Sequence();
            incrementSeq.Append(incrementedTimeText.rectTransform.DOScale(1f, .2f).SetEase(Ease.OutBounce));
            incrementSeq.AppendInterval(.35f);
            incrementSeq.Append(incrementedTimeText.rectTransform.DOScale(0f, .2f));
            incrementSeq.AppendCallback(() => incrementedTimeText.gameObject.SetActive(false));
            incrementSeq.AppendCallback(() => incrementSeq = null);
        }

        void DecrementTimer()
        {
            currTime = Mathf.Clamp(currTime - DECREMENT_TIME, 0, MAX_TIME);

            decrementedTimeText.gameObject.SetActive(true);
            decrementedTimeText.rectTransform.localScale = Vector3.zero;
            decrementedTimeText.text = $"-{DECREMENT_TIME}";

            if (decrementSeq != null)
            {
                decrementSeq.Kill();
            }

            decrementSeq = DOTween.Sequence();
            decrementSeq.Append(decrementedTimeText.rectTransform.DOScale(1f, .2f).SetEase(Ease.OutBounce));
            decrementSeq.AppendInterval(.35f);
            decrementSeq.Append(decrementedTimeText.rectTransform.DOScale(0f, .2f));
            decrementSeq.AppendCallback(() => decrementedTimeText.gameObject.SetActive(false));
            decrementSeq.AppendCallback(() => decrementSeq = null);
        }

        void EndTimer()
        {
            currTime = 0;
        }

        void MaxOutTimer()
        {
            const float gameboardNoMatchEffectsCompensation = 3;
            currTime = MAX_TIME + gameboardNoMatchEffectsCompensation;

            if(bonusSeq != null)
            {
                bonusSeq.Kill();
            }

            
            bonusAsWhole.SetActive(true);
            bonusVFX.SetActive(settings.EnableVFX);
            bonusTimeText.text = $"Max Time Refill!";

            var bonusAsWholeRect = bonusAsWhole.GetComponent<RectTransform>();
            bonusAsWholeRect.localScale = Vector3.zero;

            bonusSeq = DOTween.Sequence();
            bonusSeq.Append(bonusAsWholeRect.DOScale(1f, .2f).SetEase(Ease.OutBounce));
            bonusSeq.AppendInterval(1.25f);
            bonusSeq.Append(bonusAsWholeRect.DOScale(0f, .2f));
            bonusSeq.AppendCallback(() => bonusAsWhole.SetActive(false));
            bonusSeq.AppendCallback(() => bonusSeq = null);
        }
    }
}
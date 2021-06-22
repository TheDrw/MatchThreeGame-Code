using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using MatchThree.Core;
using System.Collections;
using System.Collections.Generic;
using MatchThree.Data;
using MatchThree.Audio;

namespace MatchThree.UI
{
    public class GameManagerUI : MonoBehaviour
    {
        [Header("Countdown Section")]
        [SerializeField] GameObject countdownRoot;
        [SerializeField] TMP_Text countdownText;

        [Header("Audio")]
        [SerializeField] GameObject menuAudio;

        [Header("Results")]
        [SerializeField] GameObject resultsRoot;

        [Header("Placements Section")]
        [SerializeField] GameObject placementsRoot;
        [SerializeField] PlacementResult placementsTextPrefab;
        [SerializeField] GameObject placementsContentsObject;

        [Header("Analytics Section")]
        [SerializeField] GameObject analyticsRoot;
        [SerializeField] AnalyticResult analyticsTextPrefab;
        [SerializeField] GameObject analyticsContentsObject;
 

        private void Awake()
        {
            if (countdownRoot.activeSelf)
            {
                countdownRoot.SetActive(false);
            }

            if (resultsRoot.activeSelf)
            {
                resultsRoot.SetActive(false);
            }
        }

        private void Start()
        {
            GameManager.OnGameStart += HandleGameStart;
            GameManager.OnGameCountdownStart += PerformCountdownText;
            GameManager.OnGameFinishedWithFinalGameResults += ShowGameResults;
            GameManager.OnGameExit += DisableMenu;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStart -= HandleGameStart;
            GameManager.OnGameCountdownStart -= PerformCountdownText;
            GameManager.OnGameFinishedWithFinalGameResults -= ShowGameResults;
            GameManager.OnGameExit -= DisableMenu;
        }

        void HandleGameStart()
        {
            SetCanvasActivation(false);
            countdownRoot.SetActive(false);
        }

        void PerformCountdownText()
        {
            countdownRoot.transform.localScale = Vector3.zero;
            countdownRoot.SetActive(true);

            StartCoroutine(CountdownTextCoroutine());
        }

        IEnumerator CountdownTextCoroutine()
        {
            countdownText.text = "Ready?!";
            countdownRoot.transform.DOScale(Vector3.one, .25f).SetEase(Ease.OutBack);

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
            yield return wait;
        }

        void ShowGameResults(List<FinishedGameResult> finishedGameResults)
        {
            SetCanvasActivation(true);

            menuAudio.SetActive(true);

            int spot = 1;
            foreach (var gameResult in finishedGameResults)
            {
                var placementText = Instantiate(placementsTextPrefab, placementsContentsObject.transform)
                    .GetComponent<PlacementResult>();
                placementText.SetPlacementText(gameResult, spot);

                var analyticText = Instantiate(analyticsTextPrefab, analyticsContentsObject.transform)
                    .GetComponent<AnalyticResult>();
                analyticText.SetAnalyticText(gameResult, spot);
                spot++;
            }

            StartCoroutine(ShowGameResultsCoroutine());
        }

        IEnumerator ShowGameResultsCoroutine()
        {
            yield return new WaitForSeconds(2f);
            resultsRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -Screen.height);
            resultsRoot.SetActive(true);
            resultsRoot.transform.DOLocalMoveY(0f, .15f);


            MenuSFX.Play?.MenuIn();
        }

        void SetCanvasActivation(bool active)
        {
            GetComponent<Canvas>().enabled = active;
        }

        void DisableMenu()
        {
            gameObject.SetActive(false);
        }
    }
}
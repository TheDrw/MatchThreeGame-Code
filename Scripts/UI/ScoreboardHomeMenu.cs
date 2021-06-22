using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Data;
using MatchThree.Audio;
using System.Linq;

namespace MatchThree.UI
{
    public class ScoreboardHomeMenu : MonoBehaviour
    {
        [SerializeField] ScoreboardHomeResult resultPrefab;
        [SerializeField] GameObject initRoot;
        [SerializeField] GameObject noneText;

        List<ScoreboardHomeResult> resultsRefs = null;
        RectTransform rectTransform;

        static readonly int MAX_RESULTS = 10; 

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            resultsRefs = new List<ScoreboardHomeResult>();

            gameObject.SetActive(false);
        }

        private void Start()
        {
            for (int i = 0; i < MAX_RESULTS; i++)
            {
                var resultText = Instantiate(resultPrefab, initRoot.transform)
                    .GetComponent<ScoreboardHomeResult>();

                resultText.gameObject.SetActive(false);
                resultsRefs.Add(resultText);
            }
        }

        private void OnDisable()
        {
            if (resultsRefs == null || resultsRefs.Count == 0) return;

            StopAllCoroutines();
            foreach (var refs in resultsRefs)
            {
                refs.gameObject.SetActive(false);
            }
        }

        public void InitializeScoreboardMenu(List<FinishedGameResult> results)
        {
            if (results.Count == 0)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 125);
                noneText.SetActive(true);
                return;
            }

            noneText.SetActive(false);
            var topTenResults = results.Take(MAX_RESULTS).ToList();

            StartCoroutine(PlaceScoreboardResults(topTenResults));
        }

        private void ResizeMenuHeightByResultCount(int count)
        {
            // 95 for 1
            // 195 for 5
            // 315 for 10
            // ~23.5 for each result

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 95 + (25 * count));
        }

        IEnumerator PlaceScoreboardResults(List<FinishedGameResult> topTenResults)
        {
            WaitForSeconds waitPause = new WaitForSeconds(.05f);
            
            for (int i = 0; i < topTenResults.Count; i++)
            {
                ResizeMenuHeightByResultCount(i);
                yield return waitPause;

                bool enableHighlightAlternate = i % 2 == 0;
                resultsRefs[i].gameObject.SetActive(true);
                resultsRefs[i].InitializeTextResult(topTenResults[i], enableHighlightAlternate);
                MenuSFX.Play?.Pop();
            }
        }
    }
}
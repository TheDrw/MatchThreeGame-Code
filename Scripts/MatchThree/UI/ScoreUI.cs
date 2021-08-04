using UnityEngine;
using TMPro;
using MatchThree.Core;
using System.Collections;
using DG.Tweening;

namespace MatchThree.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] Score scoreboard;
        [SerializeField] TMP_Text totalScoreText;
        [SerializeField] TMP_Text accumulatedScoreText;
        [SerializeField] TMP_Text loopComboMultiplierText;
        [SerializeField] TMP_Text loopBonusMultiplierText;

        static string format = "000000";

        private void Awake()
        {
            totalScoreText.text = "";
            accumulatedScoreText.text = "";
            loopComboMultiplierText.text = "";
            loopBonusMultiplierText.text = "";
        }

        private void Start()
        {
            scoreboard.OnScoreTotaled += UpdateTotalScoreText;
            scoreboard.OnScoreInit += InitializeScoreText;
            scoreboard.OnScoredCurrentAccumulatedPoints += UpdateAccumulatedPointsText;
            scoreboard.OnScoredCurrentLoopComboMultiplier += UpdateLoopComboMultiplierText;
            scoreboard.OnScoredLoopBonusMultiplier += UpdateLoopBonusMultiplierText;
            scoreboard.OnScoreTotaled += InactivateProgressValues;
        }


        private void OnDestroy()
        {
            scoreboard.OnScoreTotaled -= UpdateTotalScoreText;
            scoreboard.OnScoreInit -= InitializeScoreText;
            scoreboard.OnScoredCurrentAccumulatedPoints -= UpdateAccumulatedPointsText;
            scoreboard.OnScoredCurrentLoopComboMultiplier -= UpdateLoopComboMultiplierText;
            scoreboard.OnScoredLoopBonusMultiplier -= UpdateLoopBonusMultiplierText;
            scoreboard.OnScoreTotaled -= InactivateProgressValues;
        }

        private void InitializeScoreText()
        {
            totalScoreText.text = format;
            StartCoroutine(TypewriterEffect());
        }

        IEnumerator TypewriterEffect()
        {
            int maxCharCount = totalScoreText.text.Length,
                currCharCount = 0;

            float typewritterSpeed = 0.1f / ((float)maxCharCount);
            var wait = new WaitForSeconds(typewritterSpeed);
            while (currCharCount <= maxCharCount)
            {
                totalScoreText.maxVisibleCharacters = currCharCount;
                currCharCount++;
                yield return wait;
            }
        }

        void UpdateTotalScoreText(int totalPts, int deltaPts)
        {
            StartCoroutine(UpdateTotalScoreTextCoroutine(totalPts, deltaPts));
        }

        IEnumerator UpdateTotalScoreTextCoroutine(int totalPts, int deltaPts)
        {
            int increment = GetIncrementationByDeltaPoints(deltaPts);

            // just increments up to or even beyond it to make it look like it is going to the totalpts.
            // it will snap to the real value afterwards anyways.
            for (int i = totalPts - deltaPts; i < totalPts; i += increment)
            {
                totalScoreText.text = i.ToString(format);
                yield return null;
            }

            totalScoreText.text = totalPts.ToString(format);
        }

        // increments by numbers that i found to help increment fast enough to seem decent.
        private int GetIncrementationByDeltaPoints(int deltaPts)
        {
            int increment = 1;
            if (deltaPts >= 100 && deltaPts <= 1000)
            {
                increment = 13;
            }
            else if (deltaPts >= 1000 && deltaPts <= 10000)
            {
                increment = 137;
            }
            else if (deltaPts >= 10000 && deltaPts <= 100000)
            {
                increment = 1379;
            }
            else if (deltaPts > 100000)
            {
                increment = 13793;
            }

            return increment;
        }

        void UpdateAccumulatedPointsText(int accumulatedPts, int bonusAccumulatedPts)
        {
            if (bonusAccumulatedPts > 0)
            {
                accumulatedScoreText.text = $"{bonusAccumulatedPts}+{accumulatedPts}";
            }
            else
            {
                accumulatedScoreText.text = $"+{accumulatedPts}";
            }
        }

        void UpdateLoopComboMultiplierText(int multiplier)
        {
            loopComboMultiplierText.text = $"x{multiplier}";
        }

        void UpdateLoopBonusMultiplierText(int multiplier)
        {
            loopBonusMultiplierText.text = $"x{multiplier}!";
            loopBonusMultiplierText.rectTransform.DOPunchScale(Vector3.one * 1.2f, .25f);
        }

        void InactivateProgressValues(int notUsed, int notUsedEither)
        {
            loopComboMultiplierText.text = "";
            accumulatedScoreText.text = "";
            loopBonusMultiplierText.text = "";
        }
    }
}
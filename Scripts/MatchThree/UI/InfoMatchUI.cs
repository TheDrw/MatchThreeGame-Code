using MatchThree.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MatchThree.UI
{
    public class TotalResults
    {
        public float Time = 0f;
        public int Moves = 0;
        public int GoodMoves = 0;
        public int BadMoves = 0;
        public int NoMatches = 0;
        public float GoodRatio => Moves == 0 ? 0f : 100 * (float)GoodMoves / Moves;
        public float BadRatio => Moves == 0 ? 0f : 100 * (float)BadMoves / Moves;


        public int LargestSingleMatch = 0;
        public int LargestLoopMatch = 0;
        public int LongestComboLoop = 0;
    }


    public class InfoMatchUI : MonoBehaviour
    {
        [SerializeField] AllScores scores;

        [Header("Info Texts")]
        [SerializeField] TMP_Text timeText;
        [SerializeField] TMP_Text movesText;
        [SerializeField] TMP_Text goodMovesText;
        [SerializeField] TMP_Text badMovesText;
        [Space(5)]
        [SerializeField] TMP_Text goodRatioText;
        [SerializeField] TMP_Text badRatioText;
        [SerializeField] TMP_Text noMatchesText;
        [Space(5)]
        [SerializeField] TMP_Text largestSingleMatchText;
        [SerializeField] TMP_Text largestLoopMatchText;
        [SerializeField] TMP_Text LongestLoopComboText;

        TotalResults total = null;

        private void OnEnable()
        {
            total = new TotalResults();
            CalculateTotalResults();
            SetTotalResultsTexts();
        }

        private void CalculateTotalResults()
        {
            foreach (var score in scores.ScoresList)
            {
                foreach (var result in score.Results)
                {
                    total.Time += result.TimeFinished;
                    total.Moves += result.MoveCount;
                    total.GoodMoves += result.RightMovesCount;
                    total.BadMoves += result.WrongMovesCount;
                    total.NoMatches += result.NumberOfNoMatches;

                    total.LargestSingleMatch = Mathf.Max(total.LargestSingleMatch, result.LargestSingleMatch);
                    total.LargestLoopMatch = Mathf.Max(total.LargestLoopMatch, result.LargestLoopMatch);
                    total.LongestComboLoop = Mathf.Max(total.LongestComboLoop, result.LongestLoopCombo);
                }
            }
        }

        void SetTotalResultsTexts()
        {
            //print($"{total.Time}");
            int seconds = (int)(total.Time  % 60);
            int minutes = (int)(total.Time / 60f) % 60;
            int hours = (int)(total.Time / 3600f) % 24;
            int days = (int)(total.Time / 86400f);

            timeText.text = $"{days}d {hours}h {minutes}m {seconds}s";
            movesText.text = $"{total.Moves}";
            goodMovesText.text = $"{total.GoodMoves}";
            badMovesText.text = $"{total.BadMoves}";

            goodRatioText.text = $"{total.GoodRatio : 0.00}%";
            badRatioText.text = $"{total.BadRatio : 0.00}%";
            noMatchesText.text = $"{total.NoMatches}";

            largestSingleMatchText.text = $"{total.LargestSingleMatch}";
            largestLoopMatchText.text = $"{total.LargestLoopMatch}";
            LongestLoopComboText.text = $"{total.LongestComboLoop}";
        }
    }
}
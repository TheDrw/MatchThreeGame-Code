using UnityEngine;
using TMPro;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class ScoreboardResultLoopsScore : ScoreboardHomeResult
    {
        [SerializeField] TMP_Text loops;
        [SerializeField] TMP_Text score;

        public override void InitializeTextResult(FinishedGameResult res, bool enableHighlight)
        {
            base.InitializeTextResult(res, enableHighlight);

            loops.text = $"{res.LongestLoopCombo}";
            score.text = $"{res.TotalPoints}";
        }
    }

}
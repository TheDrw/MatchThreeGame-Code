using UnityEngine;
using TMPro;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class ScoreboardResultTimeScoreMoves : ScoreboardHomeResult
    {
        [SerializeField] TMP_Text time;
        [SerializeField] TMP_Text score;
        [SerializeField] TMP_Text moves;

        public override void InitializeTextResult(FinishedGameResult res, bool enableHighlight)
        {
            base.InitializeTextResult(res, enableHighlight);

            time.text = res.TimeFinished.ToString(".000");
            score.text = $"{res.TotalPoints}";
            moves.text = $"{res.RightMovesCount}";
        }
    }
}
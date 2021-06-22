using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MatchThree.Data;
using UnityEngine.UI;

namespace MatchThree.UI
{
    public class ScoreboardResultRankBoTimeResult : ScoreboardHomeResult
    {
        [SerializeField] TMP_Text rank;
        [SerializeField] TMP_Text boNumber;
        [SerializeField] TMP_Text time;

        public override void InitializeTextResult(FinishedGameResult res, bool enableHighlight)
        {
            base.InitializeTextResult(res, enableHighlight);

            rank.text = $"{res.Rank}";
            boNumber.text = $"{res.BoardType}";
            time.text = res.TimeFinished.ToString(".000");
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class ScoreboardHomeTimeResult : ScoreboardHomeResult
    {
        [SerializeField] TMP_Text time;

        public override void InitializeTextResult(FinishedGameResult res, bool enableHighlight)
        {
            base.InitializeTextResult(res, enableHighlight);

            time.text = res.TimeFinished.ToString(".000");
        }
    }
}
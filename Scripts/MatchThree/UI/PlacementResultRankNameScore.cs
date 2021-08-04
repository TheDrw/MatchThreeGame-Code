using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class PlacementResultRankNameScore : PlacementResult
    {
        [SerializeField] TMP_Text rank;
        [SerializeField] TMP_Text controllerName;
        [SerializeField] TMP_Text time;

        public override void SetPlacementText(FinishedGameResult result, int spotVal)
        {
            base.SetPlacementText(result, spotVal);

            rank.text = $"{result.Rank}";
            controllerName.text = $"{result.Name}";
            time.text = result.TimeFinished.ToString(".000");
        }
    }
}
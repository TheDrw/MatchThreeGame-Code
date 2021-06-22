using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class PlacementResultTimeScore : PlacementResult
    {
        [SerializeField] TMP_Text time;
        [SerializeField] TMP_Text points;

        public override void SetPlacementText(FinishedGameResult result, int spotVal)
        {
            base.SetPlacementText(result, spotVal);

            time.text = result.TimeFinished.ToString(".000");
            points.text = $"{result.TotalPoints}";
        }
    }
}
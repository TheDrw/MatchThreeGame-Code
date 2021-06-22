using UnityEngine;
using TMPro;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class PlacementResultScoreLoop : PlacementResult
    {
        [SerializeField] TMP_Text points;
        [SerializeField] TMP_Text loop;

        public override void SetPlacementText(FinishedGameResult result, int spotVal)
        {
            base.SetPlacementText(result, spotVal);

            points.text = $"{result.TotalPoints}";
            loop.text = $"{result.LongestLoopCombo}";
        }
    }

}
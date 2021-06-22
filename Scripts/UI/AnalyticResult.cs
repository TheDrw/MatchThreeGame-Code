using TMPro;
using UnityEngine;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class AnalyticResult : MonoBehaviour
    {
        [SerializeField] TMP_Text spotRank;
        [SerializeField] TMP_Text controllerName;
        [SerializeField] TMP_Text numMoves;
        [SerializeField] TMP_Text wrongMoves;
        [SerializeField] TMP_Text rightMoves;
        [SerializeField] TMP_Text singleMatches;
        [SerializeField] TMP_Text loopMatches;
        [SerializeField] TMP_Text longestLoopCombo;
        [SerializeField] TMP_Text noMatches;

        public void SetAnalyticText(FinishedGameResult result, int spot)
        {
            spotRank.text = $"{spot}";
            controllerName.text = $"{result.Name}";
            numMoves.text = $"{result.MoveCount}";
            wrongMoves.text = $"{result.WrongMovesCount}";
            rightMoves.text = $"{result.RightMovesCount}";
            singleMatches.text = $"{result.LargestSingleMatch}";
            loopMatches.text = $"{result.LargestLoopMatch}";
            longestLoopCombo.text = $"{result.LongestLoopCombo}";
            noMatches.text = $"{result.NumberOfNoMatches}";
        }
    }
}
using UnityEngine;
using System.Linq;

namespace MatchThree.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "HighScores/One Move")]
    public class HighScoresOneMove : HighScores
    {
        public override void Record(FinishedGameResult result)
        {
            results.Add(result);

            results = results.OrderByDescending(x => x.LongestLoopCombo)
                .ThenBy(y => y.TotalPoints)
                .ToList();

            Save();
        }
    }
}
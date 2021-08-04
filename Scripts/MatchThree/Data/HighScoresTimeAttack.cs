using UnityEngine;
using System.Linq;

namespace MatchThree.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "HighScores/Time Attack")]
    public class HighScoresTimeAttack : HighScores
    {
        public override void Record(FinishedGameResult result)
        {
            results.Add(result);

            results = results.OrderByDescending(x => x.TimeFinished)
                .ToList();

            Save();
        }
    }
}
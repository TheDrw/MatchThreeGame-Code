using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MatchThree.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "HighScores/Versus AI")]
    public class HighScoresVersusAI : HighScores
    {
        public override void Record(FinishedGameResult result)
        {
            results.Add(result);

            results = results.OrderBy(x => x.Rank)
                .ThenBy(y => y.TimeFinished)
                .ToList();

            Save();
        }
    }
}
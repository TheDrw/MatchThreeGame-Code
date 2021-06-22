using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MatchThree.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "HighScores/Solo")]
    public class HighScoresSolo : HighScores
    {
        public override void Record(FinishedGameResult result)
        {
            results.Add(result);

            results = results.OrderBy(x => x.TimeFinished)
                .ToList();

            Save();
        }
    }
}
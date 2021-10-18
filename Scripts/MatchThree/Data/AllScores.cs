using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MatchThree.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Match3/Extra/All Scores Holder")]
    public class AllScores : ScriptableObject
    {
        [SerializeField] HighScores[] allScores;

        public HighScores[] ScoresArray => allScores;
        public List<HighScores> ScoresList => allScores.ToList();

        public void Reset()
        {
            foreach(var score in allScores)
            {
                score.Delete();
            }
        }

        public void Load()
        {
            foreach (var score in allScores)
            {
                score.Load();
            }
        }


    }
}
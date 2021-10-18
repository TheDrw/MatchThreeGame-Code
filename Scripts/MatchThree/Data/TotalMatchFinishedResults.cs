using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Data
{

    public class Total
    { 
        
    }


    public class TotalMatchFinishedResults : MonoBehaviour
    {
        [SerializeField] AllScores scores;

        // Start is called before the first frame update
        void OnEnable()
        {
            foreach(var score in scores.ScoresList)
            {
                foreach(var result in score.Results)
                {
                    //result.TimeFinished
                }
            }
        }
    }
}
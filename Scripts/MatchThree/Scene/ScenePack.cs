using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Data;

namespace MatchThree.Scene
{
    /// <summary>
    /// Whenever I renamed scenes, the references to those string assets
    /// would have to be rewired. This prevents the rewiring from the 
    /// UI elements and you just do it on its SO instead.
    /// </summary>
    [CreateAssetMenu(fileName = "Data", menuName = "Extra/Scene Pack")]
    public class ScenePack : ScriptableObject
    {
        [Scene] [SerializeField] string sceneName;
        [SerializeField] HighScores scores;

        public string Scene => sceneName;
        public HighScores Scores => scores;
    }
}
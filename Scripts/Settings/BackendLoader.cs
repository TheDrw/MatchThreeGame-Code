using UnityEngine;
using MatchThree.Settings;

namespace MatchThree.Data
{
    public class BackendLoader : MonoBehaviour
    {
        [SerializeField] GameSettings gameSettings;
        [SerializeField] AllScores allScores;

        void Start()
        {
            allScores.Load();
            gameSettings.Load();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Settings;

namespace MatchThree.Data
{
    public class NewSessionResetPrevious : MonoBehaviour
    {
        [SerializeField] GameSettings settings;
        [SerializeField] AllScores allScores;

        private void Awake()
        {
            settings.Reset();
            allScores.Reset();
        }
    }
}
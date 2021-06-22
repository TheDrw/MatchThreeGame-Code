using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Data
{
    public abstract class HighScores : ScriptableObject, ISaveAndLoad
    {
        [SerializeField] protected List<FinishedGameResult> results;


        public void Load()
        {
            DataSave.Load($"{name}", this);
        }

        public void Save()
        {
            DataSave.Save($"{name}", this);
        }

        public void Delete()
        {
            results.Clear();
            DataSave.Delete($"{name}");
        }

        public abstract void Record(FinishedGameResult result);

        public List<FinishedGameResult> Results => results;
    }
}
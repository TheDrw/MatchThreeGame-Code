using System.Collections;
using System;
using UnityEngine;


namespace MatchThree.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Match3/Player Choice")]
    public class PlayerChoice : ScriptableObject, ISaveAndLoad
    {
        [SerializeField] int currentChoices = 3;
        [SerializeField] int currIdx = 0;

        int[] boChoices = { 3, 5, 7, 9 };
        
        readonly string file = "choice.wot";

        private void Awake()
        {
            currentChoices = boChoices[0];
        }

        public int CurrentChoice => currentChoices;

        public int NextChoice()
        {
            currIdx = (currIdx + 1) % boChoices.Length;
            currentChoices = boChoices[currIdx];
            return currentChoices;
        }

        public void Load()
        {
            DataSave.Load(file, this);
        }

        public void Save()
        {
            DataSave.Save(file, this);
        }
    }
}
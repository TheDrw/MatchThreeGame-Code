using System;
using MatchThree.Controllers;
using UnityEngine;

namespace MatchThree.Data
{
    [Serializable]
    public class FinishedGameResult
    {
        public FinishedGameResult(Controller controller, int currRank, float timeFinishedGame)
        {
            name = controller.name;
            moveCount = controller.MoveCount;
            wrongMovesCount = controller.NumberOfBadMoves;
            rightMovesCount = controller.NumberOfGoodMoves;
            largestSingleMatch = controller.LargestSingleMatch;
            largestLoopMatch = controller.LargestLoopMatch;
            longestLoopCombo = controller.LongestLoopCombo;
            numberOfNoMatches = controller.NumberOfNoMatches;
            boardType = controller.BoardType;
            totalPoints = controller.TotalPoints;

            rank = currRank;
            timeFinished = timeFinishedGame;
        }

        [SerializeField] string name = "b00ty";
        [SerializeField] int boardType = 0;
        [SerializeField] int rank = 0;
        [SerializeField] float timeFinished = 420.696f;
        [SerializeField] int moveCount = 100;
        [SerializeField] int wrongMovesCount = 420;
        [SerializeField] int rightMovesCount = 69;
        [SerializeField] int largestSingleMatch = 69;
        [SerializeField] int longestLoopCombo = 420;
        [SerializeField] int largestLoopMatch = 69;
        [SerializeField] int numberOfNoMatches = 0;
        [SerializeField] int totalPoints = 999999;

        public string Name => name;
        public int BoardType => boardType;
        public int Rank => rank;
        public float TimeFinished => timeFinished;
        public int MoveCount => moveCount;
        public int WrongMovesCount => wrongMovesCount;
        public int RightMovesCount => rightMovesCount;
        public int LargestSingleMatch => largestSingleMatch;
        public int LongestLoopCombo => longestLoopCombo;
        public int LargestLoopMatch => largestLoopMatch;
        public int NumberOfNoMatches => numberOfNoMatches;
        public int TotalPoints => totalPoints;
    }
}
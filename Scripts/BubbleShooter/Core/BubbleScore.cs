using UnityEngine;
using System;

namespace BubbleShooter.Core
{
    public class BubbleScore : MonoBehaviour
    {
        [SerializeField] BubbleShooterBoard board;

        int totalScore = 0;

        const int DROP_SCORE = 100;
        const int MATCH_SCORE = 10;

        public event Action<int> OnScoreUpdate = delegate { };

        private void OnEnable()
        {
            RegisterBoardEvents();
        }

        private void OnDisable()
        {
            UnregisterBoardEvents();
        }
        private void RegisterBoardEvents()
        {
            board.OnMatchFound += AddMatchScore;
            board.OnMatchDroppingsFell += AddDropScore;
        }

        private void UnregisterBoardEvents()
        {
            board.OnMatchFound -= AddMatchScore;
            board.OnMatchDroppingsFell -= AddDropScore;
        }

        void AddMatchScore(int count)
        {
            int bonus = CalculateMultiplierBonus(count);

            totalScore += count * MATCH_SCORE * bonus;
            OnScoreUpdate(totalScore);
        }

        int CalculateMultiplierBonus(int count)
        {
            if (count == 3) return 1;

            if (count >= 4 && count < 7)        return 2;
            else if (count >= 7 && count < 10)  return 3;
            else                                return 5;
        }

        void AddDropScore()
        {
            totalScore += DROP_SCORE;
            OnScoreUpdate(totalScore);
        }
    }
}
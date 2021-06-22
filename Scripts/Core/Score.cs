using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Core
{
    [RequireComponent(typeof(Board))]
    public class Score : MonoBehaviour
    {
        Board gameboard = null;

        public Action<int, int> OnScoredCurrentAccumulatedPoints = delegate { };
        public Action OnLoopCombo = delegate { };
        public Action<int> OnScoredCurrentLoopComboMultiplier = delegate { };
        public Action<int> OnScoredLoopBonusMultiplier = delegate { };
        public Action<int, int> OnScoreTotaled = delegate { };
        public Action OnScoreInit = delegate { };
        public Action OnMaxScoreReached = delegate { };

        static readonly int MAX_POINTS = 999999;

        int accumulatedBonus = 0;
        int accumulatedPoints = 0;
        int loopComboMultiplier = 1;
        int loopBonusMultiplier = 1;

        int totalLoopMatches = 0;

        public int TotalPoints { get; private set; } = 0;
        public int LargestSingleMatch { get; private set; } = 0;
        public int LargestLoopMatch { get; private set; } = 0;
        public int LongestLoopCombo { get; private set; } = 0;
        public int NumberOfNoMatches { get; private set; } = 0;

        private void Awake()
        {
            if(gameboard == null)
            {
                gameboard = GetComponent<Board>();
            }
        }

        private void Start()
        {
            gameboard.OnMatchScored += AccumulatePoints;
            gameboard.OnGameBoardInitialized += ShowScore;
            gameboard.OnGameBoardInitialized += TotalUpPointsFromResetBonus;
            gameboard.OnExhaustedAllMatchesInLoop += TotalUpPoints;

            gameboard.OnMatchLoopCombo += SetMultiplier;
            gameboard.OnNoMorePossibleMatches += AccumulateBoardResetBonus;
        }

        private void OnDestroy()
        {
            gameboard.OnMatchScored -= AccumulatePoints;
            gameboard.OnGameBoardInitialized -= ShowScore;
            gameboard.OnGameBoardInitialized -= TotalUpPointsFromResetBonus;
            gameboard.OnExhaustedAllMatchesInLoop -= TotalUpPoints;
            gameboard.OnMatchLoopCombo -= SetMultiplier;
            gameboard.OnNoMorePossibleMatches += AccumulateBoardResetBonus;
        }

        void AccumulatePoints(int pts)
        {
            LargestSingleMatch = Mathf.Max(LargestSingleMatch, pts);
            totalLoopMatches += pts;

            accumulatedPoints += pts;
            if (accumulatedPoints >= 50 && accumulatedPoints < 100)
            {
                accumulatedBonus = 0;
            }
            else if (accumulatedPoints >= 100)
            {
                accumulatedBonus = 0;
            }

            OnScoredCurrentAccumulatedPoints(accumulatedPoints, accumulatedBonus);
        }

        void SetMultiplier(int mult)
        {
            OnLoopCombo();
            HandleLoopBonusMultiplier(mult);
            HandleLoopComboMultiplier(mult);
        }

        private void HandleLoopBonusMultiplier(int mult)
        {
            int firstThreeDigits = 1000;
            int tempBonusMult = TotalPoints / firstThreeDigits;
            if (tempBonusMult <= 9)
            {
                if (mult == 5)
                {
                    loopBonusMultiplier = tempBonusMult;
                    loopBonusMultiplier += 2;
                    OnScoredLoopBonusMultiplier(loopBonusMultiplier);
                }
                else if (mult == 10)
                {
                    loopBonusMultiplier = tempBonusMult;
                    loopBonusMultiplier += 3;
                    OnScoredLoopBonusMultiplier(loopBonusMultiplier);
                }
                else if (mult == 15)
                {
                    loopBonusMultiplier = tempBonusMult;
                    loopBonusMultiplier += 5;
                    OnScoredLoopBonusMultiplier(loopBonusMultiplier);
                }
                else if (mult == 20)
                {
                    loopBonusMultiplier = tempBonusMult;
                    loopBonusMultiplier += 10;
                    OnScoredLoopBonusMultiplier(loopBonusMultiplier);
                }
            }
            else if (mult == 5)
            {
                loopBonusMultiplier = tempBonusMult;
                OnScoredLoopBonusMultiplier(loopBonusMultiplier);
            }
        }

        private void HandleLoopComboMultiplier(int mult)
        {
            loopComboMultiplier = mult;
            OnScoredCurrentLoopComboMultiplier(loopComboMultiplier);
        }

        void TotalUpPoints()
        {
            int prevTotal = TotalPoints;
            int cheetsMultiplier = 1;// 99999;
            TotalPoints = Mathf.Clamp(TotalPoints + 
                accumulatedPoints * loopComboMultiplier * loopBonusMultiplier * cheetsMultiplier, 
                0, 
                MAX_POINTS);

            //totalPoints = MAX_POINTS;

            LargestLoopMatch = Mathf.Max(LargestLoopMatch, totalLoopMatches);
            LongestLoopCombo = Mathf.Max(LongestLoopCombo, loopComboMultiplier);

            if (TotalPoints >= MAX_POINTS)
            {
                OnMaxScoreReached();
            }

            int deltaPts = TotalPoints - prevTotal;
            OnScoreTotaled(TotalPoints, deltaPts);

            ResetAccruedValues();
        }

        private void ResetAccruedValues()
        {
            loopBonusMultiplier = 1;
            loopComboMultiplier = 1; 
            accumulatedPoints = 0;
            accumulatedBonus = 0;

            totalLoopMatches = 0;
        }

        void ShowScore()
        {
            if (TotalPoints > 0) return;

            OnScoreInit();
        }

        void AccumulateBoardResetBonus()
        {
            NumberOfNoMatches++;
            accumulatedPoints = TotalPoints;
            loopComboMultiplier = 1;

            OnScoredCurrentAccumulatedPoints(accumulatedPoints, 0);
            OnScoredCurrentLoopComboMultiplier(loopComboMultiplier);
        }

        void TotalUpPointsFromResetBonus()
        {
            if(TotalPoints > 0)
            {
                TotalUpPoints();
            }
        }
    }
}
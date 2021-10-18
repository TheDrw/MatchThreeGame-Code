using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Core
{
    [RequireComponent(typeof(Board))]
    public class Score : MonoBehaviour
    {
        Board gameboard = null;

        public event Action<int, int> OnScoredCurrentAccumulatedPoints = delegate { };
        public event Action OnLoopCombo = delegate { };
        public event Action<int> OnScoredCurrentLoopComboMultiplier = delegate { };
        public event Action<int> OnScoredLoopBonusMultiplier = delegate { };
        public event Action<int, int> OnScoreTotaled = delegate { };
        public event Action OnScoreInit = delegate { };
        public event Action OnMaxScoreReached = delegate { };

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
            TotalPoints = Mathf.Clamp(TotalPoints + 
                accumulatedPoints * loopComboMultiplier * loopBonusMultiplier,
                0, 
                MAX_POINTS);

            HandlePossibleOverflow();
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

        /// <summary>
        ///  this is teh specific case when playing one move 2Bo.
        ///  it has the potential to loop for a real long time,
        ///  so the easiest way to handle the possibility of going to the
        ///  overflow side is to check around 500 loops and just set it to max.
        ///  according to some tests, it is around 80m on average.
        ///  the score should only go up to 999,999 anyways, so this will do. If the loopCombo ever
        ///  overflows, then uhh oh well.¯\_(ツ)_/¯
        ///  it is very very very unlikely, though.
        ///  https://i.redd.it/pbg999hfa0j31.png
        ///  </summary>
        void HandlePossibleOverflow()
        {
            if(loopComboMultiplier > 500)
            {
                TotalPoints = MAX_POINTS;
            }
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
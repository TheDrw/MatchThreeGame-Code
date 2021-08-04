using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Analytics
{
    [Serializable]
    public class BubbleAnalytics
    {
        public int MoveCount { get; private set; } = 0;
        public int NumberOfMatches { get; private set; } = 0;
        public int NumberOfDrops { get; private set; } = 0;
        public int LargestNumberOfMatches { get; private set; } = 0;
        public int LargestNumberOfDrops { get; private set; } = 0;
        public int NumberOfWallBounces { get; private set; } = 0;
        public int NumberOfGoodMoves { get; private set; } = 0;
        public int NumberOfBadMoves { get; private set; } = 0;
        public int TotalPoints { get; private set; } = 0;
    }
}
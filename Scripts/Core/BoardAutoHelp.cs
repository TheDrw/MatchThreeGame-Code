using UnityEngine;

namespace MatchThree.Core
{

    public class BoardAutoHelp
    {
        public BoardAutoHelp(GamePieceSwapDirection dir, Vector2Int posIndex)
        {
            SwapDirection = dir;
            PositionIndex = posIndex;
        }

        public BoardAutoHelp GetAutoHelp() => this;
        public Vector2Int PositionIndex { get; private set; }
        public GamePieceSwapDirection SwapDirection { get; private set; }
    }
}
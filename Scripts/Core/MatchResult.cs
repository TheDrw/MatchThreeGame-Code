using UnityEngine;

namespace MatchThree.Core
{
    public class MatchResult
    {
        public MatchResult(GamePiece gamePiece, Vector2Int position)
        {
            GamePiece = gamePiece;
            Position = position;
        }

        public GamePiece GamePiece { get; private set; }
        public Vector2Int Position { get; private set; }

    }
}
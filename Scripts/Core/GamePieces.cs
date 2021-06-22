using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Core
{
    [CreateAssetMenu(menuName = "Board/Game Pieces")]
    public class GamePieces : ScriptableObject
    {
        [Tooltip("Required 9 Game Pieces. Order of game pieces matter, so keep that in mind.")]
        [SerializeField] GamePiece[] gamePieces;

        private void OnValidate()
        {
            if (gamePieces.Length != 9)
            {
                Debug.LogError("ERR: Not enough game pieces to be used! You need 9.", this);
                int i = 0;
                foreach(var piece in gamePieces)
                {
                    if(piece == null)
                    {
                        Debug.LogError($"ERR: Missing a game piece in element {i++}.", this);
                    }
                }
            }
        }

        public GamePiece[] GetGamePieces => gamePieces;
    }
}
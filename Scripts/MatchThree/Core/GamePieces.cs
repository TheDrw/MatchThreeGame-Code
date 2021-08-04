using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Core
{
    [CreateAssetMenu(menuName = "Match3/Board/Game Pieces")]
    public class GamePieces : ScriptableObject
    {
        [Tooltip("Required 9 Game Pieces. Order of game pieces matter, so keep that in mind.")]
        [SerializeField] GamePiece[] gamePieces;


        /// <summary>
        /// A dictionary of the already inserted pieces to make look-up times quicker
        /// </summary>
        Dictionary<GamePieceType, GamePiece> gamePiecesDict = null;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            gamePiecesDict = new Dictionary<GamePieceType, GamePiece>();
            foreach (var gp in gamePieces)
            {
                gamePiecesDict.Add(gp.GetGamePieceType, gp);
            }
        }

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

            InitializeDictionary();
        }

        public GamePiece[] GetGamePieces => gamePieces;

        public GamePiece GetRandomGamePiece()
        {
            return gamePieces[UnityEngine.Random.Range(0, 9)];
        }

        public GamePiece GetGamePieceByType(GamePieceType type)
        {
            return gamePiecesDict[type];
        }
    }
}
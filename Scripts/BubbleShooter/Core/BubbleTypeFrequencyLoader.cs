using MatchThree.Core;
using UnityEngine;
using System.Collections.Generic;

namespace BubbleShooter.Core
{
    public class BubbleTypeFrequencyLoader : MonoBehaviour
    {
        [SerializeField] GamePieces gamePieces = null;
        [SerializeField] BubbleShooterBoard board = null;

        //[SerializeField] Firework firework;

        [Header("Testing")]
        [Tooltip ("Enable to just give random colored game pieces"),
         SerializeField] bool isTesting = false;

        Dictionary<GamePieceType, int> frequencyCount = null;

        private void Awake()
        {
            frequencyCount = new Dictionary<GamePieceType, int>();
        }

        private void OnEnable()
        {
            board.OnGamePieceTypeAdded += IncreaseFrequencyType;
            board.OnGamePieceTypeRemoved += DecreaseFrequencyType;

            board.OnBoardClear += DeployFireworks;
        }

        private void OnDestroy()
        {
            board.OnGamePieceTypeAdded -= IncreaseFrequencyType;
            board.OnGamePieceTypeRemoved -= DecreaseFrequencyType;

            board.OnBoardClear -= DeployFireworks;
        }

        void DeployFireworks()
        {

        }

        public GamePiece GetBubbleFromFrequencyLoader()
        {
            if (frequencyCount.Count > 0 && !isTesting)
            {
                foreach (var bucket in frequencyCount)
                {
                    float chance = (bucket.Value / (float)board.TotalNumberOfGamePiecesOnBoard) * 100f;
                    chance = Mathf.Min(chance, 95f);
                    //print($"chance: {bucket.Key} {chance} , {bucket.Value}, {board.TotalNumberOfGamePiecesOnBoard}");
                   
                    if (IsPossibleByChance((int)chance))
                    {
                        //print($"sending {bucket.Key}");
                        return gamePieces.GetGamePieceByType(bucket.Key);
                    }
                }

                if (IsPossibleByChance(75))
                {
                    //print("finding failed so sending in first key");
                    var enumerator = frequencyCount.Keys.GetEnumerator();
                    enumerator.MoveNext();
                    return gamePieces.GetGamePieceByType(enumerator.Current);
                }
            }

            //print($"sending random");
            return gamePieces.GetRandomGamePiece();
        }

        /// <summary>
        /// Enter a number val from 0 to 100 where val represents the percentage 
        /// of chance of returning true.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        bool IsPossibleByChance(int val)
        {
            val = Mathf.Clamp(val, 0, 100);
            return UnityEngine.Random.Range(1, 101) <= val;
        }

        void IncreaseFrequencyType(GamePieceType type)
        {
            if(!frequencyCount.ContainsKey(type))
            {
                frequencyCount[type] = 1;
            }
            else
            {
                frequencyCount[type]++;
            }
            
            //PrintTypeFrequency();
        }

        void DecreaseFrequencyType(GamePieceType type)
        {
            frequencyCount[type] = Mathf.Max(frequencyCount[type] - 1, 0);
            if(frequencyCount[type] == 0)
            {
                frequencyCount.Remove(type);
            }
            //PrintTypeFrequency();
        }

        void PrintTypeFrequency()
        {
            string s = "";
            foreach(var bucket in frequencyCount)
            {
                s += $"{bucket.Key} = {bucket.Value}\n";
            }

            print(s);
        }
    }
}
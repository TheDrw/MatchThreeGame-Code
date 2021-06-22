using MatchThree.Controllers;
using MatchThree.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Gameplay
{
    public class GameOneMoveChecker : MonoBehaviour
    {
        [SerializeField] GameManager GM;
        [SerializeField] PlayerController player;

        private void Start()
        {
            player.OnControllerMadeGoodMove += EndGame;
        }

        private void OnDestroy()
        {
            player.OnControllerMadeGoodMove -= EndGame;
        }

        void EndGame()
        {
            player.Deactivate();
            StartCoroutine(EndGameCO());
            
        }

        IEnumerator EndGameCO()
        {
            var waitUntilPlayerDone = new WaitUntil(() => player.Gameboard.CurrentBoardState == BoardState.Ready);
            yield return waitUntilPlayerDone;
            print("PLATYER DUNE");
            GM.EndGameEarly();
        }
    }
}
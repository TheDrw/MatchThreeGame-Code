using System.Collections;
using UnityEngine;
using MatchThree.Core;
using MatchThree.Settings;


namespace MatchThree.Controllers
{
    public class AIController : Controller
    {
        [SerializeField] GameSettings gameSettings;

        [Tooltip("Delay time for each move. The lower, the slower. The higher, the faster.")]
        [SerializeField] float waitSelectionTime = 0.1f;
        WaitForSeconds waitOneSecond = null,
            waitBeforeSelecting = null;

        protected override void Awake()
        {
            base.Awake();

            waitOneSecond = new WaitForSeconds(1f);
            waitBeforeSelecting = new WaitForSeconds(waitSelectionTime);
        }

        private void OnValidate()
        {
            waitSelectionTime = Mathf.Clamp(waitSelectionTime, 0.01f, 10f);
        }

        protected override IEnumerator Start()
        {
            yield return base.Start();

            var waitBeforeSelecting = new WaitForSeconds(waitSelectionTime);
            var waitBetweenSelecting = new WaitForSeconds(.25f);
            var waitForBoardReady = new WaitUntil(() => gameboard.CurrentBoardState == BoardState.Ready);
            var waitUntilActive = new WaitUntil(() => isActive);

            yield return waitForBoardReady;
            ControllerReadyToStart();

            yield return waitUntilActive;

            while (isActive)
            {
                yield return waitForBoardReady;
                yield return waitBetweenSelecting; // stops bug from selecting again

                if (!isActive) break;

                yield return PerformMove();
            }

            Dispose();
        }

        IEnumerator PerformMove()
        {
            int rand = Random.Range(0, 1 + gameSettings.Difficulty);

            if (rand == 0)
            {
                Vector2Int randomSpot = GetRandomPositionIndex();
                GamePieceSwapDirection randomDir = GetRandomSwapDirection();

                gameboard.SelectGamePieceOnBoard(randomSpot);
                yield return waitBeforeSelecting;
                gameboard.UnselectGamePieceOnBoard(randomDir, randomSpot);
                MoveConfirmed();
                yield return waitOneSecond;
            }
            else
            {
                BoardAutoHelp cheetz = gameboard.FindFirstAutoMatch();
                if (cheetz != null)
                {
                    gameboard.SelectGamePieceOnBoard(cheetz.PositionIndex);
                    yield return waitBeforeSelecting;
                    gameboard.UnselectGamePieceOnBoard(cheetz.SwapDirection, cheetz.PositionIndex);
                    MoveConfirmed();
                }
            }
        }
    }
}
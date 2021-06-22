using System;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;
using MatchThree.UI;
using System.Collections;

namespace MatchThree.Controllers
{
    public abstract class Controller : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] protected Board gameboard;
        [SerializeField] protected Score scoreboard;
        [SerializeField] protected ControllerUI controllerUI;

        [Space(10)]
        [Tooltip("The number of colors on the board. You can't go lower than 2 or more than 9.")]
        [SerializeField] protected int boardType = 3;

        public Board Gameboard => gameboard;
        public Score Scoreboard => scoreboard;

        public Action OnControllerMadeFirstMove = delegate { };
        public Action<Controller> OnControllerFinishedGame = delegate { };
        public Action OnControllerMadeGoodMove = delegate { };
        public Action OnControllerMadeBadMove = delegate { };
        public Action OnControllerMaxScoreReached = delegate { };
        public Action OnControllerReadyToStart = delegate { };

        public bool IsReadyToStart { get; protected set; } = false;
        protected bool isActive = false;
        public bool IsDisposed { get; private set; } = false;

        public int BoardType => boardType;
        public int MoveCount { get; private set; } = 0;
        public int LargestSingleMatch => scoreboard.LargestSingleMatch;
        public int LargestLoopMatch => scoreboard.LargestLoopMatch;
        public int LongestLoopCombo => scoreboard.LongestLoopCombo;
        public int NumberOfNoMatches => scoreboard.NumberOfNoMatches;
        public int NumberOfGoodMoves { get; private set; } = 0;
        public int NumberOfBadMoves { get; private set; } = 0;
        public int TotalPoints => scoreboard.TotalPoints;

        protected virtual void Awake()
        {
            if (gameboard == null)
            {
                gameboard = GetComponentInChildren<Board>();
            }

            if (scoreboard == null)
            {
                scoreboard = GetComponentInChildren<Score>();
            }

            if (controllerUI == null)
            {
                controllerUI = GetComponentInChildren<ControllerUI>();
            }
        }

        protected virtual IEnumerator Start()
        {
            gameboard.SetBoardType(boardType);
            yield return null;
        }

        private void OnValidate()
        {
            boardType = Mathf.Clamp(boardType, 2, 9);
        }

        private void OnEnable()
        {
            scoreboard.OnMaxScoreReached += Dispose;
            gameboard.OnMatchMadeGood += IncrementGoodMoves;
            gameboard.OnMatchMadeBad += IncrementBadMoves;
        }


        private void OnDestroy()
        {
            scoreboard.OnMaxScoreReached -= Dispose;
            gameboard.OnMatchMadeGood -= IncrementGoodMoves;
            gameboard.OnMatchMadeBad -= IncrementBadMoves;
        }

        protected void ControllerReadyToStart()
        {
            IsReadyToStart = true;
            controllerUI.HideInitializingBoardText();
            OnControllerReadyToStart();
        }

        public virtual void Activate()
        {
            isActive = true;
        }

        public virtual void Deactivate()
        {
            isActive = false;
        }

        public virtual void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            StartCoroutine(DisposeCoroutine());
        }

        public virtual void DisposeImmediate()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            Deactivate();
            OnControllerFinishedGame(this);
            gameboard.StopAndEliminateBoard();
        }


        IEnumerator DisposeCoroutine()
        {
            WaitUntil waitForGameboard = new WaitUntil(() => gameboard.CurrentBoardState == BoardState.Ready ||
                gameboard.CurrentBoardState == BoardState.Selecting
            );

            yield return waitForGameboard;

            Deactivate();
            OnControllerFinishedGame(this);
            gameboard.StopAndEliminateBoard();
        }

        public void EnableAssist() => gameboard.ActivateAssist();

        public void DisableAssist() => gameboard.DeactivateAssist();

        public void HandleControllerFinished(float timeFinished, int rank)
        {
            controllerUI.ShowFinishedOverlay(timeFinished, rank);
        }

        void IncrementGoodMoves()
        {
            NumberOfGoodMoves++;
            OnControllerMadeGoodMove();
        }

        void IncrementBadMoves()
        {
            NumberOfBadMoves++;
            OnControllerMadeBadMove();
        }

        protected void MoveConfirmed()
        {
            MoveCount++;

            if (MoveCount == 1)
            {
                OnControllerMadeFirstMove();
            }
        }

        protected Vector2Int GetRandomPositionIndex()
        {
            return new Vector2Int(
                    UnityEngine.Random.Range(0, gameboard.BoardSize.x),
                    UnityEngine.Random.Range(0, gameboard.BoardSize.y)
                    );
        }

        protected GamePieceSwapDirection GetRandomSwapDirection()
        {
            int random = UnityEngine.Random.Range(0, 4);

            if (random == 0) return GamePieceSwapDirection.Down;
            else if (random == 1) return GamePieceSwapDirection.Left;
            else if (random == 2) return GamePieceSwapDirection.Right;
            else return GamePieceSwapDirection.Up;
        }
    }
}
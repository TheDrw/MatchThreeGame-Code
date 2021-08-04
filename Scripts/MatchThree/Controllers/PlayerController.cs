using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;
using MatchThree.Data;

namespace MatchThree.Controllers
{
    interface IPlayer { }

    public class PlayerController : Controller, IPlayer
    {
        [SerializeField] Camera mainCam;

        Vector2 begin, end;
        readonly Vector3 screenPointOffsetToTranslateForPositionIndex = Vector3.one * 0.5f;

        protected override IEnumerator Start()
        {
            //yield return base.Start();

            gameboard.SetBoardType(boardType);

            GameManager.OnGamePaused +=(() => base.Deactivate());
            GameManager.OnGameUnpaused += (() => base.Activate());

            var boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.size = gameboard.BoardSize;
            boxCollider.offset = new Vector2((gameboard.BoardSize.x / 2f) - 0.5f, (gameboard.BoardSize.y / 2f) - 0.5f);

            if (mainCam == null)
            {
                mainCam = Camera.main;
            }

            yield return new WaitUntil(() => gameboard.CurrentBoardState == BoardState.Ready);
            ControllerReadyToStart();
        }

        private void OnDestroy()
        {
            GameManager.OnGamePaused -= (() => base.Deactivate());
            GameManager.OnGameUnpaused -= (() => base.Activate());
        }

        private void OnMouseDown()
        {
            if (!isActive ||
                gameboard.CurrentBoardState != BoardState.Ready) return;

            begin = mainCam.ScreenToWorldPoint(Input.mousePosition) + screenPointOffsetToTranslateForPositionIndex - transform.position;
            gameboard.SelectGamePieceOnBoard(GetPositionIndex());

        }

        private void OnMouseUp()
        {
            if (!isActive || 
                gameboard.CurrentBoardState != BoardState.Selecting) return;

            
            end = mainCam.ScreenToWorldPoint(Input.mousePosition) + screenPointOffsetToTranslateForPositionIndex - transform.position;
            float dist = Vector2.Distance(begin, end);

            gameboard.UnselectGamePieceOnBoard(GetMoveDirection(), GetPositionIndex(), dist);

            if (dist >= 0.5f)
            {
                MoveConfirmed();
            }
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Dispose()
        {
            base.Dispose();

            GetComponent<BoxCollider2D>().enabled = false;
        }

        private GamePieceSwapDirection GetMoveDirection()
        {
            float angle = Mathf.Atan2((end.y - begin.y), (end.x - begin.x)) * (180 / Mathf.PI);

            GamePieceSwapDirection moveDirection = GamePieceSwapDirection.NA;
            if (angle > -45f && angle <= 45f)
            {
                moveDirection = GamePieceSwapDirection.Right;
            }
            else if (angle > 45f && angle <= 135f)
            {
                moveDirection = GamePieceSwapDirection.Up;
            }
            else if (angle > 135f || angle <= -135f)
            {
                moveDirection = GamePieceSwapDirection.Left;
            }
            else if (angle < -45f && angle >= -135f)
            {
                moveDirection = GamePieceSwapDirection.Down;
            }

            return moveDirection;
        }

        Vector2Int GetPositionIndex()
        {
            return new Vector2Int((int)begin.x, (int)begin.y);
        }

        public void OverrideBoardType(int boardType)
        {
            this.boardType = boardType;
            gameboard.SetBoardType(boardType);
        }
    }
}
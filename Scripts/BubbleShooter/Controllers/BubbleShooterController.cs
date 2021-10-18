using BubbleShooter.Audio;
using BubbleShooter.Core;
using UnityEngine;
using System;
using DG.Tweening;
using BubbleShooter.UI;

namespace BubbleShooter.Controller
{
    public enum ShootDirection
    {
        Right = -1,
        Idle = 0, 
        Left = 1
    }

    [SelectionBase]
    public abstract class BubbleShooterController : MonoBehaviour
    {
        [SerializeField] protected BubbleShooterBoard board;
        [SerializeField] protected BubbleTypeFrequencyLoader loader;
        [SerializeField] protected Shooter shooter;
        [SerializeField] BubbleControllerUI controllerUI;

        public event Action OnControllerMadeFirstMove = delegate { };
        public event Action<BubbleShooterController> OnControllerFinishedGame = delegate { };
        public event Action<BubbleShooterController> OnControllerDied = delegate { };
        public event Action OnControllerMadeGoodMove = delegate { };
        public event Action OnControllerMadeBadMove = delegate { };
        public event Action OnControllerMaxScoreReached = delegate { };
        public event Action OnControllerReadyToStart = delegate { };
        

        public BubbleShooterBoard Board => board;

        protected ShootDirection shooterDirection = ShootDirection.Idle;

        public bool IsReadyToStart { get; protected set; } = false;
        protected bool isActive = false;
        public bool IsDisposed { get; private set; } = false;

        protected virtual void Awake()
        {
            shooter.enabled = false;
            shooter.transform.localScale = Vector3.zero;
        }

        protected virtual void Start()
        {
            controllerUI.Hide();
        }

        protected virtual void OnEnable()
        {
            RegisterBoardEvents();
        }


        protected virtual void Update()
        {
            RotateShooter(shooterDirection);
        }

        protected virtual void OnDisable()
        {
            UnregisterBoardEvents();
        }

        private void RegisterBoardEvents()
        {
            board.OnReady += LoadBubble;
            board.OnBoardSetupStart += ShowShooter;
            board.OnBoardSetupFinished += ControllerReadyToStart;
            board.OnBubbleLandedAtBottomAndNoMatchFoundSoGameOver += ControllerHitGameOverOnBoard;
            board.OnBoardClear += ControllerClearedBoard;
        }

        private void UnregisterBoardEvents()
        {
            board.OnReady -= LoadBubble;
            board.OnBoardSetupStart -= ShowShooter;
            board.OnBoardSetupFinished -= ControllerReadyToStart;
            board.OnBubbleLandedAtBottomAndNoMatchFoundSoGameOver -= ControllerHitGameOverOnBoard;
            board.OnBoardClear -= ControllerClearedBoard;
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
            shooterDirection = ShootDirection.Idle;
        }

        public void Won()
        {
            controllerUI.DisplayWon();
        }

        public void Lost()
        {
            controllerUI.DisplayLost();
        }

        public void Dispose(bool destroyBoard = true)
        {
            if (IsDisposed) return;

            isActive = false;
            IsDisposed = true;

            board.StopTrackingBubble();
            if (destroyBoard)
            {
                board.EndBoard();
            }
        }

        public void SetupInitialization()
        {
            board.SetupBoardInitialization();
        }

        void ControllerClearedBoard()
        {
            OnControllerFinishedGame(this);
        }

        void ControllerHitGameOverOnBoard()
        {
            OnControllerDied(this);
        }

        void ShowShooter()
        {
            shooter.enabled = true;
            shooter.transform.DOScale(Vector3.one, .5f).SetEase(Ease.InBounce);
        }

        public void HandleControllerFinished(float time, int rank)
        { }

        protected void ControllerReadyToStart()
        {
            controllerUI.Show();
            IsReadyToStart = true;
            OnControllerReadyToStart();
        }


        protected void SetShootDirection(ShootDirection dir)
        {
            shooterDirection = dir;
        }

        protected void SetShootDirection(int val)
        {
            if (val == -1) SetShootDirection(ShootDirection.Right);
            else if (val == 0) SetShootDirection(ShootDirection.Idle);
            else if (val == 1) SetShootDirection(ShootDirection.Left);
            else Debug.LogError("ERR : Calculation for setting shoot direction isn't right.", this);
        }

        protected void LoadBubble()
        {
            var bubble = Instantiate(loader.GetBubbleFromFrequencyLoader().gameObject,
                shooter.LoadLocation.position,
                Quaternion.identity)
                .AddComponent<Bubble>();

            shooter.LoadBubble(bubble);
            board.EnqueueTrackingBubble(bubble);
        }

        protected void ShootBubble()
        {
            if (!isActive) return;

            shooter.Shoot();
        }

        /// <summary>
        /// +1 for left and -1 for right.
        /// </summary>
        /// <param name="dir"></param>
        private void RotateShooter(ShootDirection dir)
        {
            shooter.Rotate(dir);
        }
    }
}
using DG.Tweening;
using MatchThree.Helpers;
using UnityEngine;
using Game;

namespace MatchThree.UI
{
    public class PauseManager : MenuManager
    {
        [Header("Secondary Menus")]
        [SerializeField] GameObject settings;
        [SerializeField] GameObject exit;

        IGameManager gameManage = null;

        protected override void Awake()
        {
            base.Awake();

            front.SetActive(false);
            settings.SetActive(false);
            exit.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();

            var currMenu = menuStack.Peek();
            TweenLocalMoveMenu(currMenu, Vector3.up * -500f, Vector3.zero, 0.15f, Ease.OutBack);

            gameManage = GeneralHelp.GetIGameManagerFromActiveScene();
        }

        protected override void HandleBackOnLastMenuInStack()
        {
            base.HandleBackOnLastMenuInStack();
            GoBackToGame();
        }

        public void GoBackToGame()
        {
            var currMenu = menuStack.Peek();
            TweenLocalMoveMenu(currMenu, Vector3.zero, Vector3.up * -500f, 0.15f)
                .OnComplete(() => gameManage.UnpauseGame());
        }

        public void ReturnToHome()
        {
            var currMenu = menuStack.Peek();
            TweenLocalMoveMenu(currMenu, Vector3.zero, Vector3.up * -500f, 0.15f);

            inputActions.Disable();
            gameManage.ReturnToHome();
        }
    }
}
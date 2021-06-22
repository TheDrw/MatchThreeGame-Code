using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;
using MatchThree.Audio;
using DG.Tweening;
using MatchThree.Helpers;

namespace MatchThree.UI
{
    public class PauseManager : MenuManager
    {
        [Header("Secondary Menus")]
        [SerializeField] GameObject settings;
        [SerializeField] GameObject exit;

        GameManager GM = null;

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

            GM = GeneralHelp.GetGameManagerFromActiveScene();
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
                .OnComplete(() => GM.UnpauseGame());
        }

        public void ReturnToHome()
        {
            var currMenu = menuStack.Peek();
            TweenLocalMoveMenu(currMenu, Vector3.zero, Vector3.up * -500f, 0.15f);

            inputActions.Disable();
            GM.ReturnToHome();
        }
    }
}
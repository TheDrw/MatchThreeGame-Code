using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MatchThree.Core;
using UnityEngine.SceneManagement;

namespace MatchThree.UI
{
    public abstract class MenuManager : MonoBehaviour
    {
        [Header("Primary Menu")]
        [Tooltip("This is the main focus and first menu that is shown to the player.")]
        [SerializeField] protected GameObject front;


        protected bool isActive = false;
        protected Stack<GameObject> menuStack = null;
        protected InputActions inputActions = null;

        protected event Action OnFrontMenuEnter = delegate { };
        protected event Action OnFrontMenuExit = delegate { };

        protected virtual void Awake()
        {
            inputActions = new InputActions();
            menuStack =  new Stack<GameObject>();

            menuStack.Push(front);
        }

        protected virtual void Start()
        {
            inputActions.Player.BackUI.performed += ctx => GoBackOneMenu();
            inputActions.Enable();
            isActive = true;
        }

        protected virtual void OnDestroy()
        {
            inputActions.Player.BackUI.performed -= ctx => GoBackOneMenu();
            inputActions.Disable();
            inputActions.Dispose();
        }

        public void EnableInputBack()
        {
            inputActions.Enable();
        }

        public void DisableInputBack()
        {
            inputActions.Disable();
        }

        public void GoBackOneMenu()
        {
            if (menuStack.Count > 1)
            {
                var currMenu = menuStack.Pop();
                TweenLocalMoveMenu(currMenu, Vector3.zero, Vector3.up * -500f, 0.15f)
                    .OnComplete(()=> currMenu.SetActive(false));

                var newMenu = menuStack.Peek();
                TweenLocalMoveMenu(newMenu, Vector3.up * -500f, Vector3.zero, 0.15f);

                if (menuStack.Count == 1)
                {
                    OnFrontMenuEnter();
                }
            }
            else
            {
                HandleBackOnLastMenuInStack();
            }
        }

        public void GoToMenu(GameObject menuGO)
        {
            var currMenu = menuStack.Peek();
            TweenLocalMoveMenu(currMenu, Vector3.zero, Vector3.up * -500f, 0.15f)
                .OnComplete(() => currMenu.SetActive(false));


            menuStack.Push(menuGO);
            TweenLocalMoveMenu(menuGO, Vector3.up * -500f, Vector3.zero, 0.15f);

            if(menuStack.Count == 2)
            {
                OnFrontMenuExit();
            }
        }

        public void GoToMenuDontHidePrevious(GameObject menuGO)
        {
            menuStack.Push(menuGO);
            TweenLocalMoveMenu(menuGO, Vector3.up * -500f, Vector3.zero, 0.15f);

            if (menuStack.Count == 2)
            {
                OnFrontMenuExit();
            }
        }

        protected virtual void HandleBackOnLastMenuInStack()
        {
            
        }

        protected DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> TweenLocalMoveMenu(GameObject go, 
            Vector3 localRelocation, Vector3 direction, float duration, Ease ease = Ease.Linear)
        {
            go.SetActive(true);
            go.transform.localPosition = localRelocation;

            return go.transform
                .DOLocalMove(direction, duration)
                .SetEase(ease)
                .SetUpdate(true);
        }
    }
}
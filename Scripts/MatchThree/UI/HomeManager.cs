using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MatchThree.Effects;
using MatchThree.Audio;
using DG.Tweening;
using MatchThree.Scene;


namespace MatchThree.UI
{
    public class HomeManager : MenuManager
    {
        [Header("Secondary Menus")]
        [SerializeField] GameObject info;
        [SerializeField] GameObject settings;
        [SerializeField] GameObject gameMode;
        
        [Space(5)]
        [Header("Etc")]
        [SerializeField] TransitionManager transitionManager;
        [SerializeField] GameObject titleTextGO;

        bool isEnteringScene = false;

        protected override void Start()
        {
            StartCoroutine(ClearUnusedAssets());
            DOTween.Clear();

            front.SetActive(false);
            info.SetActive(false);
            settings.SetActive(false);
            gameMode.SetActive(false);

            transitionManager.FadeOutTransition(.15f, .75f);
            StartCoroutine(WaitForGameTitle());

            OnFrontMenuEnter += ShowTitle;
            OnFrontMenuExit += HideTitle;
        }

        IEnumerator ClearUnusedAssets()
        {
            yield return Resources.UnloadUnusedAssets();
            
        }

        IEnumerator WaitForGameTitle()
        {
            titleTextGO.SetActive(true);
            yield return new WaitForSeconds(4f);

            base.Start();
            MenuSFX.Play?.MenuIn();
            TweenLocalMoveMenu(front, Vector3.up * -500f, Vector3.zero, 0.15f);
        }

        public void EnterScene(ScenePack scenePack)
        {
            if (isEnteringScene) return;

            isEnteringScene = true;
            GoToGame(scenePack.Scene);
        }

        void GoToGame(string sceneName)
        {
            transitionManager.FadeInTransition(.15f, 0f);
            gameMode.SetActive(false);
            StartCoroutine(GoToGameCoroutine(sceneName));
        }

        IEnumerator GoToGameCoroutine(string sceneName)
        {
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadScene(sceneName);
        }

        //protected override void HandleBackOnLastMenuInStack()
        //{
        //    base.HandleBackOnLastMenuInStack();
        //    titleTextGO.SetActive(false);
        //}

        void ShowTitle()
        {
            titleTextGO.SetActive(true);
            titleTextGO.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 0.15f);
        }

        void HideTitle()
        {
            titleTextGO.GetComponent<RectTransform>()
                .DOAnchorPos(Vector3.up * 200f, 0.15f)
                .OnComplete(() => titleTextGO.SetActive(false));
        }

    }
}
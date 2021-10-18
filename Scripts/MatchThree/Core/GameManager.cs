using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using MatchThree.Controllers;
using MatchThree.Data;
using UnityEngine.SceneManagement;
using MatchThree.Effects;
using System.Diagnostics;
using Game;

namespace MatchThree.Core
{
    public enum GameState
    { 
        NA, Initializing, Playing, Paused, Finished, EndingEarly
    }

    public class GameManager : MonoBehaviour, IGameManager
    {
        [Header("Game Settings")]
        [SerializeField] bool isAssitAllowed = true;

        [Header("Scenes")]
        [Scene] [SerializeField] string pauseMenu;
        [Scene] [SerializeField] string home;

        [Header("Other stuff")]
        [SerializeField] GameObject eventSystem;
        [SerializeField] HighScores scores;
        [SerializeField] TransitionManager transitionManager;

        [Header("Audio")]
        [SerializeField] AudioClip readySFX;
        [SerializeField] AudioClip countdownSFX;
        [SerializeField] AudioClip goSFX;

        int rank = 1;
        Controller[] controllers;
        List<FinishedGameResult> finishedResults = new List<FinishedGameResult>();
        AudioSource audioSource;

        public static event Action<FinishedGameResult> OnGamePlayerFinished = delegate { };
        public static event Action OnGameFinished = delegate { };
        public static event Action OnGameStart = delegate { };
        public static event Action OnGameCountdownStart = delegate { };
        public static event Action<List<FinishedGameResult>> OnGameFinishedWithFinalGameResults = delegate { };
        public static event Action OnGamePaused = delegate { };
        public static event Action OnGameUnpaused = delegate { };
        public static event Action OnGameExit = delegate { };
        public static event Action OnGameEndingEarly = delegate { };

        public GameState CurrentGameState { get; private set; } = GameState.NA;

        InputActions inputActions = null;
        float timeActive = 0f;
        bool areControllersReady = false;

        private void Awake()
        {
            inputActions = new InputActions();
            audioSource = GetComponent<AudioSource>();
            CurrentGameState = GameState.Initializing;
        }

        private IEnumerator Start()
        {
            yield return Resources.UnloadUnusedAssets();

            transitionManager.FadeOutTransition(.15f, .5f);

            inputActions.Player.Pause.performed += ctx => PauseGame();
            controllers = FindObjectsOfType<Controller>();
            foreach(var controller in controllers)
            {
                controller.OnControllerFinishedGame += ControllerFinished;
                controller.OnControllerReadyToStart += ControllerReadyToStart;

                if (isAssitAllowed)
                {
                    controller.EnableAssist();
                }
                else
                {
                    controller.DisableAssist();
                }
            }

            var waitForControllersReady = new WaitUntil(() => areControllersReady);
            yield return waitForControllersReady;

            yield return InitiateGame();

            eventSystem.SetActive(true);
        }

        private void OnDestroy()
        {
            inputActions.Player.Pause.performed -= ctx => PauseGame();
            foreach (var controller in controllers)
            {
                controller.OnControllerFinishedGame -= ControllerFinished;
                controller.OnControllerReadyToStart -= ControllerReadyToStart;
            }
        }

        private void Update()
        {
            timeActive += Time.deltaTime;
        }

        IEnumerator InitiateGame()
        {
            var waitOneSecond = new WaitForSeconds(1f);

            yield return waitOneSecond;
            yield return CountdownGame(waitOneSecond);
            yield return waitOneSecond;

            StartGame();
        }

        IEnumerator CountdownGame(WaitForSeconds waitOneSecond)
        {
            OnGameCountdownStart();
            audioSource.PlayOneShot(readySFX);
            yield return waitOneSecond;

            for (int i = 3; i > 0; i--)
            {
                audioSource.PlayOneShot(countdownSFX);
                yield return waitOneSecond;
            }

            audioSource.PlayOneShot(goSFX);
        }

        void StartGame()
        {
            foreach (var controller in controllers)
            {
                controller.Activate();
            }

            inputActions.Enable();
            CurrentGameState = GameState.Playing;
            timeActive = 0f;
            OnGameStart();
        }

        void ControllerFinished(Controller controller)
        {
            float timeFinished = timeActive;

            controller.HandleControllerFinished(timeFinished, rank);
            var result = new FinishedGameResult(controller, rank, timeFinished);

            finishedResults.Add(result);

            if (controller is IPlayer)
            {
                scores.Record(result);
                OnGamePlayerFinished(result);
            }

            if (controllers.Length == finishedResults.Count)
            {
                OnGameFinished();

                if(finishedResults.Count == 1)
                {
                    OnGameFinishedWithFinalGameResults(scores.Results);
                }
                else
                {
                    OnGameFinishedWithFinalGameResults(finishedResults);
                }
                
                CurrentGameState = GameState.Finished;
            }

            rank++;
        }


        void ControllerReadyToStart()
        {
            int readyCount = 0;
            foreach (var controller in controllers)
            {
                if (controller.IsReadyToStart)
                {
                    readyCount++;
                }
            }

            areControllersReady = readyCount == controllers.Length;
        }

        /// <summary>
        /// Ends the game early but will wait for other boards to be in ready state first.
        /// </summary>
        public void EndGameEarly()
        {
            CurrentGameState = GameState.EndingEarly;
            foreach(var controller in controllers)
            {
                if(!controller.IsDisposed)
                {
                    controller.Dispose();
                }
            }

            OnGameEndingEarly();
        }

        public void PauseGame()
        {
            if (CurrentGameState != GameState.Playing ||
                CurrentGameState == GameState.Paused) return;

            HandlePauseDetails();
            StartCoroutine(OpenPauseMenuCoroutine());
        }

        public void UnpauseGame()
        {
            if (CurrentGameState != GameState.Paused) return;

            StartCoroutine(ClosePauseMenuCoroutine());
        }

        private void HandlePauseDetails()
        {
            CurrentGameState = GameState.Paused;
            OnGamePaused();
            inputActions.Disable();
            GC.Collect();
            Time.timeScale = 0f;
            eventSystem.SetActive(false);
        }

        private void HandleUnpauseDetails()
        {
            CurrentGameState = GameState.Playing;
            GC.Collect();
            Time.timeScale = 1f;
            OnGameUnpaused();
            inputActions.Enable();
            eventSystem.SetActive(true);
        }

        public void ReturnToHome()
        {
            GC.Collect();
            
            OnGameExit();
            StartCoroutine(ReturnHomeCoroutine());
        }

        IEnumerator ReturnHomeCoroutine()
        {
            transitionManager.FadeInTransition(.15f, .5f);
            yield return new WaitForSecondsRealtime(.5f);
            ClosePauseMenuCoroutine();

            yield return new WaitForSecondsRealtime(.5f);
            Time.timeScale = 1f;


            var homeScene = SceneManager.GetSceneByPath(home);
            SceneManager.LoadScene(home, LoadSceneMode.Single);
        }

        public void RetryGame()
        {
            GC.Collect();

            OnGameExit();
            StartCoroutine(RetryGameCoroutine());
        }

        IEnumerator RetryGameCoroutine()
        {
            transitionManager.FadeInTransition(.15f, .5f);
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        IEnumerator OpenPauseMenuCoroutine()
        {
            if (!SceneManager.GetSceneByPath(pauseMenu).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(pauseMenu, LoadSceneMode.Additive);
            }
        }

        IEnumerator ClosePauseMenuCoroutine()
        {
            if (SceneManager.GetSceneByPath(pauseMenu).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(pauseMenu);
                HandleUnpauseDetails();
            }
        }
    }
}
using UnityEngine;
using System;
using System.Collections;
using MatchThree.Effects;
using UnityEngine.SceneManagement;
using BubbleShooter.Controller;
using DG.Tweening;
using MatchThree.Controllers;
using BubbleShooter.UI;
using Game;

namespace BubbleShooter.Core
{
    public enum GameState
    {
        NA, Initializing, Playing, Paused, Finished, EndingEarly
    }

    // TODO - literally copy and pasted teh game manager from the match 3 game and changed it for the bubble controller.
    // must fix the duplicate code later on to be more flexible. Just gotta get this out soon.
    public class BubbleGameManager : MonoBehaviour, IGameManager
    {
        [Header("Scenes")]
        [Scene] [SerializeField] string pauseMenu;
        [Scene] [SerializeField] string home;

        [Header("Other stuff")]
        //[SerializeField] HighScores scores;
        [SerializeField] TransitionManager transitionManager;
        [SerializeField] BubbleEndgameUI endGameUIObject;

        [Header("Audio")]
        [SerializeField] AudioClip boardCreationSFX;
        [SerializeField] AudioClip readySFX;
        [SerializeField] AudioClip countdownSFX;
        [SerializeField] AudioClip goSFX;

        [Space(5)]
        [Tooltip("Controllers in the game should be initialized here. If not, it will scan for them in the heirarchy.")]
        [SerializeField] BubbleShooterController[] controllers;
        //List<FinishedGameResult> finishedResults = new List<FinishedGameResult>();
        AudioSource audioSource;

        //public static Action<FinishedGameResult> OnGamePlayerFinished = delegate { };
        public static Action OnGameFinished = delegate { };
        public static Action OnGameStart = delegate { };
        public static Action OnGameCountdownStart = delegate { };
        //public static Action<List<FinishedGameResult>> OnGameFinishedWithFinalGameResults = delegate { };
        public static Action OnGamePaused = delegate { };
        public static Action OnGameUnpaused = delegate { };
        public static Action OnGameExit = delegate { };
        public static Action OnGameEndingEarly = delegate { };
        public static Action OnPlayerWon = delegate { };
        public static Action OnPlayerLost = delegate { };

        public GameState CurrentGameState { get; private set; } = GameState.NA;

        int rank = 1;
        InputActions inputActions = null;
        float timeActive = 0f;

        private void Awake()
        {
            inputActions = new InputActions();
            audioSource = GetComponent<AudioSource>();
            CurrentGameState = GameState.Initializing;
        }

        private void OnEnable()
        {
            endGameUIObject.gameObject.SetActive(false);
            //HandleTransition();
        }

        private IEnumerator Start()
        {
            DOTween.Clear();
            yield return Resources.UnloadUnusedAssets();

            HandleTransition();
            RegisterInputEvents();
            HandleInitializationOfControllers();

            float delayTimeBeforeSettingUpGame = 1f;
            yield return new WaitForSeconds(delayTimeBeforeSettingUpGame);
            InitializeControllerSetups();

            yield return WaitUntilControllersReady();
            yield return InitiateGame();
        }

        private void Update()
        {
            timeActive += Time.deltaTime;
        }

        private void OnDestroy()
        {
            UnregisterInputEvents();
            UnregisterControllerEvents();
        }

        private void InitializeControllerSetups()
        {
            foreach (var controller in controllers) controller.SetupInitialization();
            audioSource.PlayOneShot(boardCreationSFX);
        }

        private void HandleInitializationOfControllers()
        {
            if (controllers.Length == 0)
            {
                controllers = FindObjectsOfType<BubbleShooterController>();
            }

            RegisterControllerEvents();
        }

        private void RegisterInputEvents()
        {
            inputActions.Player.Pause.performed += ctx => PauseGame();
        }

        private void UnregisterInputEvents()
        {
            inputActions.Player.Pause.performed -= ctx => PauseGame();
        }

        private void HandleTransition()
        {
            if (transitionManager == null)
            {
                transitionManager = FindObjectOfType<TransitionManager>();
#if UNITY_EDITOR
                Debug.LogWarning("WRN : Transition manager isn't linked through editor. Looking through heirarchy to find it.");
#endif
            }

            transitionManager.FadeOutTransition(.15f, -1f);
        }

        private void RegisterControllerEvents()
        {
            foreach (var controller in controllers)
            {
                controller.OnControllerFinishedGame += ControllerWon;
                controller.OnControllerDied += ControllerDied;
            }
        }

        private void UnregisterControllerEvents()
        {
            foreach (var controller in controllers)
            {
                controller.OnControllerFinishedGame -= ControllerWon;
                controller.OnControllerDied -= ControllerDied;
            }
        }

        // "pauses"  until each controller is ready
        IEnumerator WaitUntilControllersReady()
        {
            foreach (var controller in controllers)
            {
                var waitUntilCurrentControlReady = new WaitUntil(() => controller.IsReadyToStart);
                yield return waitUntilCurrentControlReady; 
            }
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

        // implementation is locked for 2 players
        void ControllerDied(BubbleShooterController controller)
        {
            //Debug.Log($"{controller.gameObject.name} died", controller);
            if (controller is IPlayer)
            {
                OnPlayerLost();
                HandleControllerLost(controller);
            }
            else
            {
                OnPlayerWon();
                HandleControllerLost(controller);
            }

            GameFinished();
        }

        void GameFinished()
        {
            endGameUIObject.gameObject.SetActive(true);
        }

        private void HandleControllerLost(BubbleShooterController loser)
        {
            foreach (var controller in controllers)
            {
                if (controller == loser)
                {
                    controller.Lost();
                    controller.Dispose(true);
                }
                else
                {
                    controller.Won();
                    controller.Dispose(false);
                }
            }
        }

        private void HandleControllerWon(BubbleShooterController winner)
        {
            foreach (var controller in controllers)
            {
                if (controller == winner)
                {
                    controller.Won();
                    controller.Dispose(false);
                }
                else
                {
                    controller.Lost();
                    controller.Dispose(true);
                }
            }
        }

        void ControllerWon(BubbleShooterController controller)
        {
            //Debug.Log($"{controller.gameObject.name} won", controller);

            if (controller is IPlayer)
            {
                OnPlayerWon();
                HandleControllerWon(controller);
            }
            else
            {
                HandleControllerWon(controller);
                OnPlayerLost();
            }

            GameFinished();

            float timeFinished = timeActive;

            controller.HandleControllerFinished(timeFinished, rank);
            rank++;
        }

        /// <summary>
        /// Ends the game early but will wait for other boards to be in ready state first.
        /// </summary>
        public void EndGameEarly()
        {
            CurrentGameState = GameState.EndingEarly;
            foreach (var controller in controllers)
            {
                if (!controller.IsDisposed)
                {
                    controller.Dispose();
                }
            }

            OnGameEndingEarly();
        }

        public void PauseGame()
        {
            if (CurrentGameState != GameState.Playing) return;

            OnGamePaused();
            DeactivateControls();
            GC.Collect();
            Time.timeScale = 0f;
            CurrentGameState = GameState.Paused;
            OpenPauseMenu();
        }

        private void DeactivateControls()
        {
            inputActions.Disable();
            foreach(var controller in controllers)
            {
                controller.Deactivate();
            }
        }

        public void UnpauseGame()
        {
            if (CurrentGameState != GameState.Paused) return;

            GC.Collect();
            Time.timeScale = 1f;

            OnGameUnpaused();
            ActivateControls();
            CurrentGameState = GameState.Playing;
            ClosePauseMenu();
        }

        private void ActivateControls()
        {
            inputActions.Enable();
            foreach(var controller in controllers)
            {
                controller.Activate();
            }
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
            ClosePauseMenu();

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


        void OpenPauseMenu()
        {
            if (!SceneManager.GetSceneByPath(pauseMenu).isLoaded)
            {
                SceneManager.LoadSceneAsync(pauseMenu, LoadSceneMode.Additive);
            }
        }

        void ClosePauseMenu()
        {
            if (SceneManager.GetSceneByPath(pauseMenu).isLoaded)
            {
                SceneManager.UnloadSceneAsync(pauseMenu);
            }
        }
    }
}
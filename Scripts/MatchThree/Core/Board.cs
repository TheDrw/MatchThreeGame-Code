using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using MatchThree.Settings;
using MatchThree.Audio;
using MatchThree.Effects;

namespace MatchThree.Core
{
    public class Board : MonoBehaviour
    {
        [SerializeField] GameSettings gameSettings;

        [Header("Board Setup")]
        [Tooltip("Size must at least be a 2x2")]
        [SerializeField] Vector2Int boardSize = new Vector2Int(5, 5);

        [Space(5)]
        [SerializeField] BoardSFX boardSFX = null;
        [SerializeField] BoardVFX boardVFX = null;
        [SerializeField] GamePieces pieces = null;

        GamePiece[] gamePieces = null; // could object pool it, but don't think it is really necessary.
        GamePiece[,] boardTileMap = null;
        AudioSource audioSource = null;
        List<MatchResult> resultsFromMatching = new List<MatchResult>();

        public BoardState CurrentBoardState { get; private set; } = BoardState.Initializing;
        public Vector2Int BoardSize => boardSize;

        public int GamePieceCount => gamePieces.Length;

        int boardType = 3;

        bool isUsingAssist = true;

        IEnumerator boardCheckerCO = null, showAssistCO = null;

        GamePiece hintedGamePiece = null;


        public event Action<int> OnMatchScored = delegate { };
        public event Action<int> OnMatchLoopCombo = delegate { };
        public event Action OnExhaustedAllMatchesInLoop = delegate { };
        public event Action OnGameBoardInitialized = delegate { };
        public event Action OnNoMorePossibleMatches = delegate { };
        public event Action OnMatchMadeBad = delegate { };
        public event Action OnMatchMadeGood = delegate { };

        private void Awake()
        {
            gamePieces = pieces.GetGamePieces;
            boardTileMap = new GamePiece[boardSize.x, boardSize.y];
            audioSource = GetComponent<AudioSource>();
        }

        private void OnValidate()
        {
            if (boardSize.x < 2)
            {
                boardSize.x = 2;
            }

            if(boardSize.y < 2)
            {
                boardSize.y = 2;
            }
        }

        private IEnumerator Start()
        {
            CurrentBoardState = BoardState.Initializing;
            yield return new WaitForSeconds(.5f);
            yield return InitializeBoard();
            OnGameBoardInitialized();
            SetBoardStateReady();
        }

        IEnumerator InitializeBoard()
        {
            InstantiateGamePiecesToBoard();
            yield return CollectAllMatches(); // remove any matches at start
            yield return new WaitForSeconds(.5f);
            PlayIntroEffects();
        }

        private void InstantiateGamePiecesToBoard()
        {
            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    Vector3 spotOffsetWithWorldPosition = new Vector3(i, j, 0) + transform.position;

                    boardTileMap[i, j] = Instantiate(
                        GetRandomGamePieceGameObject(),
                        spotOffsetWithWorldPosition,
                        Quaternion.identity,
                        transform).GetComponent<GamePiece>();
                }
            }
        }

        private void PlayIntroEffects()
        {
            if (gameSettings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.PopupSFX);
            }

            if (gameSettings.EnableVFX)
            {
                for (int i = 0; i < boardSize.x; i++)
                {
                    for (int j = 0; j < boardSize.y; j++)
                    {
                        boardTileMap[i, j].PunchScaleVFX(scale: 0.5f, duration: .25f, vibrato: 2, elasticity: .05f);
                    }
                }
            }
        }

        void SwapGamePieces(GamePieceSwapDirection dir, Vector2Int posIndexA)
        {
            if (CurrentBoardState != BoardState.Selecting) return;

            CurrentBoardState = BoardState.Updating;
            if (!IsMoveWithinBoard(dir, posIndexA))
            {
                OnMatchMadeBad();
                PlayInvalidMoveEffects(posIndexA);
                return;
            }

            Vector2Int swapDirection = SwapDirection(dir);
            Vector2Int posIndexB = new Vector2Int(posIndexA.x + swapDirection.x, posIndexA.y + swapDirection.y);

            SwapGamePiecesInMemory(posIndexA, posIndexB);

            StartCoroutine(HandleMatchingAfterSwapCoroutine(posIndexA, posIndexB));
        }

        private void PlayInvalidMoveEffects(Vector2Int posIndexA)
        {
            if (gameSettings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.ErrorSFX);
            }

            boardTileMap[posIndexA.x, posIndexA.y]?
                .transform.DOShakeRotation(.25f, 90, 100, 90)
                .OnComplete(() => SetBoardStateReady());
        }

        private void SwapGamePiecesInMemory(Vector2Int posIndexA, Vector2Int posIndexB)
        {
            GamePiece temp = GetGamePieceByIndex(posIndexA);

            boardTileMap[posIndexA.x, posIndexA.y] = GetGamePieceByIndex(posIndexB);
            boardTileMap[posIndexB.x, posIndexB.y] = temp;
        }

        IEnumerator SwapGamePiecesInScene(Vector2Int posIndexA, Vector2Int posIndexB)
        {
            if (gameSettings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.RandomSwapSFX());
            }

            GamePiece gamePieceA = GetGamePieceByIndex(posIndexB),
                gamePieceB = GetGamePieceByIndex(posIndexA);

            float swapDuration = 0.35f;

            gamePieceA?.transform
                .DOLocalMove(new Vector3(posIndexB.x, posIndexB.y, 0f), swapDuration)
                .SetEase(Ease.OutBounce);

            gamePieceB?.transform
                .DOLocalMove(new Vector3(posIndexA.x, posIndexA.y, 0f), swapDuration)
                .SetEase(Ease.OutBounce);

            yield return new WaitForSeconds(swapDuration);
        }

        IEnumerator HandleMatchingAfterSwapCoroutine(Vector2Int posIdxA, Vector2Int posIdxB)
        {
            yield return SwapGamePiecesInScene(posIdxA, posIdxB);

            resultsFromMatching.Clear();

            bool matchOneFound = FindMatchesAtIndex(posIdxA),
                matchTwoFound = FindMatchesAtIndex(posIdxB);

            bool noMatchesFound = !matchOneFound && !matchTwoFound;
            if (noMatchesFound)
            {
                OnMatchMadeBad();
                yield return SwapBeansBack(posIdxA, posIdxB);
            }
            else
            {
                OnMatchMadeGood();

                if (gameSettings.IsSFXEnabled)
                {
                    PlayMatchSoundByCombo(1);
                }

                yield return ParseResultsAfterMatching();
                yield return CollectAllMatches();
            }

            SetBoardStateReady();
        }

        IEnumerator SwapBeansBack(Vector2Int posIdxA, Vector2Int posIdxB)
        {
            SwapGamePiecesInMemory(posIdxA, posIdxB);
            yield return SwapGamePiecesInScene(posIdxA, posIdxB);
        }

        IEnumerator CollectAllMatches()
        {
            bool matchFoundDuringRefill = false;
            bool isInitializing = CurrentBoardState == BoardState.Initializing;
            int loopMultiplier = 1;
            do
            {
                int matchCombo = 0;
                matchFoundDuringRefill = false;
                for (int i = 0; i < boardSize.x; i++)
                {
                    for (int j = 0; j < boardSize.y; j++)
                    {
                        if (FindMatchesAtIndex(i, j))
                        {
                            matchCombo++;
                            matchFoundDuringRefill = true;
                        }
                    }
                }

                if (matchFoundDuringRefill)
                {
                    if (!isInitializing)
                    {
                        loopMultiplier++;
                        OnMatchLoopCombo(loopMultiplier);
                        PlayMatchSoundByCombo(loopMultiplier);
                    }

                    yield return ParseResultsAfterMatching();
                }

            } while (matchFoundDuringRefill);

            if(!isInitializing)
            {
                OnExhaustedAllMatchesInLoop();
            }
        }

        private void PlayMatchSoundByCombo(int combo)
        {
            if (!gameSettings.IsSFXEnabled) return;

            if(combo == 1)
            {
                audioSource.PlayOneShot(boardSFX.MatchFoundSFX);
            }
            else if (combo == 5)
            {
                audioSource.PlayOneShot(boardSFX.MatchComboOneSFX);
            }
            else if (combo == 10)
            {
                audioSource.PlayOneShot(boardSFX.MatchComboTwoSFX);
            }
            else if (combo == 20)
            {
                audioSource.PlayOneShot(boardSFX.MatchComboThreeSFX);
            }
        }

        IEnumerator ParseResultsAfterMatching()
        {
            UseResultsToRemoveGamePiecesFromBoard();
            PlayGamePieceDisappearingSFX();

            Dictionary<int, List<int>> emptySpots = EmptySpotsFromResultsByPositionsToDict();

            var wait = new WaitForSeconds(1f);

            int pieceCount = 0;
            foreach (var spot in emptySpots)
            {
                pieceCount += spot.Value.Count();
                var gamePieceAboveVertMatchQueue = new Queue<GamePiece>();
                GetAllCurrentGamePiecesAboveMissingSpot(spot, spot.Value, gamePieceAboveVertMatchQueue);
                CreateMoreGamePiecesAboveOldOnes(spot, spot.Value, gamePieceAboveVertMatchQueue);
                MoveCurrentGamePiecesDown(spot, spot.Value, gamePieceAboveVertMatchQueue);
            }

            ResetResultsFromMatching();

            if (CurrentBoardState != BoardState.Initializing)
            {
                OnMatchScored(pieceCount);
                yield return wait;
            }
        }

        private void ResetResultsFromMatching()
        {
            resultsFromMatching.Clear();
        }

        private void PlayGamePieceDisappearingSFX()
        {
            if (!gameSettings.IsSFXEnabled) return;

            if (CurrentBoardState != BoardState.Initializing)
            {
                audioSource.PlayOneShot(boardSFX.PoppingSFX);
            }
        }

        private void UseResultsToRemoveGamePiecesFromBoard()
        {
            foreach (MatchResult result in resultsFromMatching)
            {
                PlayVanishAtGamePieceParticles(result);

                Destroy(result.GamePiece.gameObject);
                boardTileMap[result.Position.x, result.Position.y] = null;
            }
        }

        private void PlayVanishAtGamePieceParticles(MatchResult result)
        {
            if (gameSettings.EnableVFX && 
                CurrentBoardState != BoardState.Initializing)
            {
                Instantiate(boardVFX.Vanish, result.GamePiece.transform.position, Quaternion.identity);
            }
        }

        /// dictionary of <x-coord , list of y-coords>
        private Dictionary<int, List<int>> EmptySpotsFromResultsByPositionsToDict()
        {
            Dictionary<int, SortedSet<int>> tempEmptySpots = OrderingEmptyPositionSet();

            var emptySpots = new Dictionary<int, List<int>>();
            foreach (var spot in tempEmptySpots)
            {
                emptySpots.Add(spot.Key, spot.Value.ToList());
            }

            return emptySpots;
        }

        /// <summary>
        /// there's a bug when collected the empty positions where it can collect an already
        /// previously marked piece. specificially on elbows or T where there's one extra piece
        /// extending on the end. This fixes it by throwing them into a sorted set.
        /// ex :       x
        ///         xxxxx
        ///            x
        ///            x
        ///  
        /// Accidentally, the xCoords are thrown in the dictionary sorted already without having
        /// to call sort on it. the yCoords have to be sorted because how it is used later.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, SortedSet<int>> OrderingEmptyPositionSet()
        {
            var tempEmptySpots = new Dictionary<int, SortedSet<int>>();
            foreach (var res in resultsFromMatching)
            {
                int xCoord = res.Position.x, yCoord = res.Position.y;
                if (!tempEmptySpots.ContainsKey(xCoord))
                {
                    tempEmptySpots.Add(xCoord, new SortedSet<int> { yCoord });
                }
                else
                {
                    tempEmptySpots[xCoord].Add(yCoord);
                }
            }
            return tempEmptySpots;
        }

        private void GetAllCurrentGamePiecesAboveMissingSpot(KeyValuePair<int, List<int>> spot,
            List<int> emptyVerticalIndices, Queue<GamePiece> gamePieceAboveVertMatchQueue)
        {
            for (int j = emptyVerticalIndices[0]; j < boardSize.y; j++)
            {
                GamePiece gamePieceAboveSpot = boardTileMap[spot.Key, j];
                if (gamePieceAboveSpot != null)
                {
                    gamePieceAboveVertMatchQueue.Enqueue(gamePieceAboveSpot);
                }
            }
        }

        private void CreateMoreGamePiecesAboveOldOnes(KeyValuePair<int, List<int>> spot,
            List<int> emptyVerticalIndices, Queue<GamePiece> gamePieceAboveVertMatchQueue)
        {
            int amountToCreate = emptyVerticalIndices.Count;
            float topPartScreenHiddenFromPlayer = 20f;
            for (int i = 0; i < amountToCreate; i++)
            {
                Vector3 spotOffsetWithWorldPosition = new Vector3(spot.Key, topPartScreenHiddenFromPlayer, 0) + transform.position;

                var gamePiece = Instantiate(
                     GetRandomGamePieceGameObject(),
                     spotOffsetWithWorldPosition,
                     Quaternion.identity,
                     transform).GetComponent<GamePiece>();

                gamePieceAboveVertMatchQueue.Enqueue(gamePiece);
            }
        }

        private void MoveCurrentGamePiecesDown(KeyValuePair<int, List<int>> spot,
            List<int> emptyVerticalIndices, Queue<GamePiece> gamePieceAboveVertMatchQueue)
        {
            float delayBouncyVisualEffect = 0f;
            for (int j = emptyVerticalIndices[0]; j < boardSize.y; j++)
            {
                if (gamePieceAboveVertMatchQueue.Count != 0)
                {
                    GamePiece gamePiece = gamePieceAboveVertMatchQueue.Dequeue();
                    boardTileMap[spot.Key, j] = gamePiece;

                    if (CurrentBoardState != BoardState.Initializing)
                    {
                        gamePiece.transform
                            .DOLocalMove(new Vector3(spot.Key, j), .75f + delayBouncyVisualEffect)
                            .SetEase(Ease.OutBounce);
                    }
                    else
                    {
                        gamePiece.transform.localPosition = new Vector3(spot.Key, j);
                    }

                    delayBouncyVisualEffect += 0.025f;
                }
            }
        }

        bool FindMatchesAtIndex(int x, int y)
        {
            return FindMatchesAtIndex(new Vector2Int(x, y));
        }

        // there's a bug where it adds an already marked piece in twice on weird shapes
        // like Ts, elbows, etc...
        // a bandaid fix is implemented above using a set but might be overkill.
        bool FindMatchesAtIndex(Vector2Int posIdx)
        {
            GamePiece focusedGamePiece = GetGamePieceByIndex(posIdx);
            if (focusedGamePiece != null && focusedGamePiece.IsVisited) return false;

            List<MatchResult> resultsFromRight = SearchRight(focusedGamePiece, posIdx),
                resultsFromLeft = SearchLeft(focusedGamePiece, posIdx),
                resultsFromUp = SearchUp(focusedGamePiece, posIdx),
                resultsFromDown = SearchDown(focusedGamePiece, posIdx);

            List<MatchResult> horizontalResult = new List<MatchResult>(resultsFromLeft);
            horizontalResult.AddRange(resultsFromRight);

            List<MatchResult> verticalResult = new List<MatchResult>(resultsFromDown);
            verticalResult.AddRange(resultsFromUp);

            bool hasFoundMatches = horizontalResult.Count >= 2 || verticalResult.Count >= 2;
            if (hasFoundMatches)
            {
                resultsFromMatching.Add(new MatchResult(focusedGamePiece,
                    new Vector2Int(posIdx.x, posIdx.y)));

                int scoreHorizontal = horizontalResult.Count >= 2 ? MarkGamePieces(horizontalResult) : 0;
                int scoreVertical = verticalResult.Count >= 2 ? MarkGamePieces(verticalResult) : 0;

                if (scoreHorizontal > 0)
                {
                    resultsFromMatching.AddRange(horizontalResult);
                }

                if (scoreVertical > 0)
                {
                    resultsFromMatching.AddRange(verticalResult);
                }

                return true;
            }

            return false;
        }

        private List<MatchResult> SearchRight(GamePiece focusedGamePiece, Vector2Int posIdx)
        {
            List<MatchResult> results = new List<MatchResult>();
            for (int i = posIdx.x + 1; i < boardSize.x; i++)
            {
                GamePiece comparingPiece = GetGamePieceByIndex(i, posIdx.y);
                if (comparingPiece != null &&
                    focusedGamePiece.IsSameAs(comparingPiece))
                {
                    Vector2Int comparingPiecePos = new Vector2Int(i, posIdx.y);
                    results.Add(new MatchResult(comparingPiece, comparingPiecePos));
                }
                else break;
            }

            return results;
        }

        private List<MatchResult> SearchLeft(GamePiece focusedGamePiece, Vector2Int posIdx)
        {
            List<MatchResult> results = new List<MatchResult>();
            for (int i = posIdx.x - 1; i >= 0; i--)
            {
                GamePiece comparingPiece = GetGamePieceByIndex(i, posIdx.y);
                if (comparingPiece != null &&
                    focusedGamePiece.IsSameAs(comparingPiece))
                {
                    Vector2Int comparingPiecePos = new Vector2Int(i, posIdx.y);
                    results.Add(new MatchResult(comparingPiece, comparingPiecePos));
                }
                else break;
            }

            return results;
        }

        private List<MatchResult> SearchUp(GamePiece focusedGamePiece, Vector2Int posIdx)
        {
            List<MatchResult> results = new List<MatchResult>();
            for (int j = posIdx.y + 1; j < boardSize.y; j++)
            {
                GamePiece comparingPiece = GetGamePieceByIndex(posIdx.x, j);
                if (comparingPiece != null &&
                    focusedGamePiece.IsSameAs(comparingPiece))
                {
                    Vector2Int comparingPiecePos = new Vector2Int(posIdx.x, j);
                    results.Add(new MatchResult(comparingPiece, comparingPiecePos));
                }
                else break;
            }

            return results;
        }

        private List<MatchResult> SearchDown(GamePiece focusedGamePiece, Vector2Int posIdx)
        {
            List<MatchResult> results = new List<MatchResult>();
            for (int j = posIdx.y - 1; j >= 0; j--)
            {
                GamePiece comparingPiece = GetGamePieceByIndex(posIdx.x, j);

                if (comparingPiece != null &&
                    focusedGamePiece.IsSameAs(comparingPiece))
                {
                    Vector2Int comparingPiecePos = new Vector2Int(posIdx.x, j);
                    results.Add(new MatchResult(comparingPiece, comparingPiecePos));
                }
                else break;
            }

            return results;
        }

        int MarkGamePieces(List<MatchResult> results)
        {
            int score = 0;
            foreach (MatchResult result in results)
            {
                if (!result.GamePiece.IsVisited)
                {
                    result.GamePiece.Visit();
                    score++;
                }
            }

            return score;
        }

        private Vector2Int SwapDirection(GamePieceSwapDirection dir)
        {
            if (dir == GamePieceSwapDirection.Right) return Vector2Int.right;
            else if (dir == GamePieceSwapDirection.Left) return Vector2Int.left;
            else if (dir == GamePieceSwapDirection.Up) return Vector2Int.up;
            else if (dir == GamePieceSwapDirection.Down) return Vector2Int.down;
            return Vector2Int.zero;
        }

        bool IsMoveWithinBoard(GamePieceSwapDirection dir, Vector2Int posIndex)
        {
            if ((dir == GamePieceSwapDirection.Down && posIndex.y == 0) ||
                (dir == GamePieceSwapDirection.Up && posIndex.y == boardSize.y - 1) ||
                (dir == GamePieceSwapDirection.Left && posIndex.x == 0) ||
                (dir == GamePieceSwapDirection.Right && posIndex.x == boardSize.x - 1))
            {
                return false;
            }

            return true;
        }

        GameObject GetRandomGamePieceGameObject()
        {
            return gamePieces[UnityEngine.Random.Range(0, boardType)].gameObject;
        }


#if UNITY_EDITOR
        void PrintBoardDebug()
        {
            string all = "";
            for (int j = boardSize.y - 1; j >= 0; j--)
            {
                string row = "";
                for (int i = 0; i < boardSize.x; i++)
                {
                    var bean = boardTileMap[i, j];
                    if (bean)
                    {
                        row += bean.GetGamePieceType.ToString()[0];
                    }
                    else
                    {
                        row += "X";
                    }
                }
                all += row + '\n';
            }

            print(all);
        }
#endif

        public void SelectGamePieceOnBoard(Vector2Int posIdx)
        {
            if (CurrentBoardState != BoardState.Ready) return;

            ResetBoardChecker();

            TurnOffHintGamePiece();

            CurrentBoardState = BoardState.Selecting;
            HighlightSelectedEffects(posIdx);
        }

        private void ResetBoardChecker()
        {
            if (boardCheckerCO != null)
            {
                StopCoroutine(boardCheckerCO);
                boardCheckerCO = null;
            }
        }

        private void TurnOffHintGamePiece()
        {
            if (hintedGamePiece)
            {
                if (gameSettings.EnableVFX)
                {
                    hintedGamePiece.HintOff();
                }

                hintedGamePiece = null;
            }
        }

        void HighlightSelectedEffects(Vector2Int posIdx)
        {
            if (CurrentBoardState != BoardState.Selecting) return;

            if (gameSettings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.SelectedSFX);
            }

            if (gameSettings.EnableVFX)
            {
                GetGamePieceByIndex(posIdx).Highlight();
            }
        }

        public void UnselectGamePieceOnBoard(GamePieceSwapDirection dir, Vector2Int posIdx, float swipeDist = 1.0f)
        {
            if (gameSettings.EnableVFX)
            {
                GetGamePieceByIndex(posIdx)?.Unhighlight();
            }

            if (CurrentBoardState != BoardState.Selecting) return;

            if (swipeDist < 0.5f)
            {
                SetBoardStateReady();
                return;
            }

            SwapGamePieces(dir, posIdx);
        }

        void SetBoardStateReady()
        {
            CurrentBoardState = BoardState.Ready;

            InitiateBoardChecker();
        }

        private void InitiateBoardChecker()
        {
            if (boardCheckerCO == null)
            {
                boardCheckerCO = BoardCheckerCoroutine();
                StartCoroutine(boardCheckerCO);
            }
        }

        IEnumerator BoardCheckerCoroutine()
        {
            if (CurrentBoardState == BoardState.Ready)
            {
                var cheets = FindFirstAutoMatch();
                if (cheets != null && isUsingAssist)
                {
                    if(showAssistCO != null)
                    {
                        StopCoroutine(showAssistCO);
                    }

                    showAssistCO = ShowAssistCoroutine(cheets);
                    yield return showAssistCO;

                }
                else
                {
                    CurrentBoardState = BoardState.Updating;
                    PlayNoMoreMatchesEffects();
                    OnNoMorePossibleMatches();

                    yield return new WaitForSeconds(1f);
                    yield return ResetBoard();
                }
            }

            boardCheckerCO = null;
        }

        IEnumerator ShowAssistCoroutine(BoardAutoHelp cheets)
        {
            yield return new WaitForSeconds(10f);

            if (gameSettings.EnableVFX)
            {
                hintedGamePiece = GetGamePieceByIndex(cheets.PositionIndex);
            }

            if (hintedGamePiece != null &&
                gameSettings.EnableVFX)
            {
                hintedGamePiece.HintOn();
            }
            showAssistCO = null;
        }

        private void PlayNoMoreMatchesEffects()
        {
            if (gameSettings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.NoMoreMatchesSFX);
            }

            if (gameSettings.EnableVFX)
            {
                for (int i = 0; i < boardSize.x; i++)
                {
                    for (int j = 0; j < boardSize.y; j++)
                    {
                        boardTileMap[i, j].PunchScaleVFX(scale: 1f, duration: .45f, vibrato: 5, elasticity: 1f);
                        boardTileMap[i, j].transform.DOShakeRotation(.25f, 10, 100, 90);
                        boardTileMap[i, j].HintOn();
                    }
                }
            }
        }

        GamePiece GetGamePieceByIndex(Vector2Int vect)
        {
            return boardTileMap[vect.x, vect.y];
        }

        GamePiece GetGamePieceByIndex(int x, int y)
        {
            return GetGamePieceByIndex(new Vector2Int(x, y));
        }

        IEnumerator ResetBoard()
        {
            boardCheckerCO = null; // if a double reset happens, this stops it from freezing
            CurrentBoardState = BoardState.Initializing;
            PlayClearBoardEffects();
            yield return new WaitForSeconds(0.5f);
            yield return Start();
        }

        private void PlayClearBoardEffects()
        {
            if (gameSettings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.ClearingBoardSFX);
            }

            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    var gp = GetGamePieceByIndex(i, j);

                    if (gameSettings.EnableVFX)
                    {
                        Instantiate(boardVFX.ClearBoard, gp.transform.position, Quaternion.identity);
                    }

                    Destroy(gp.gameObject);
                    boardTileMap[i, j] = null;
                }
            }
        }

        // not sure how to refactor this
        public BoardAutoHelp FindFirstAutoMatch()
        {
            // you need position and direction it has to go
            // scan everything from the bottom
            GamePiece focusPiece = null;
            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    focusPiece = GetGamePieceByIndex(i, j);
                    Vector2Int focusPos = new Vector2Int(i, j);
                    focusPos.x = i; focusPos.y = j;

                    if (IsMoveWithinBoard(GamePieceSwapDirection.Right, focusPos))
                    {
                        var copyMovedFocusPos = focusPos;
                        copyMovedFocusPos.x = i + 1;

                        // don't check the opposite direction of where you move to
                        // because you'll be double counting where you were. 
                        // it is also not necessary.
                        var rightResult = SearchRight(focusPiece, copyMovedFocusPos);
                        var upResult = SearchUp(focusPiece, copyMovedFocusPos);
                        var downResult = SearchDown(focusPiece, copyMovedFocusPos);

                        // two in row to that direction OR
                        // inbetween corner cases when there's one on either side.
                        // again, not needed to check direction you are moving towards
                        if ((rightResult.Count > 1 || upResult.Count > 1 || downResult.Count > 1) ||
                                (upResult.Count > 0 && downResult.Count > 0))
                        {
                            return new BoardAutoHelp(GamePieceSwapDirection.Right, focusPos);
                        }
                    }

                    if (IsMoveWithinBoard(GamePieceSwapDirection.Left, focusPos))
                    {
                        var copyMovedFocusPos = focusPos;
                        copyMovedFocusPos.x = i - 1;

                        var leftResult = SearchLeft(focusPiece, copyMovedFocusPos);
                        var upResult = SearchUp(focusPiece, copyMovedFocusPos);
                        var downResult = SearchDown(focusPiece, copyMovedFocusPos);

                        if ((leftResult.Count > 1 || upResult.Count > 1 || downResult.Count > 1) ||
                                (upResult.Count > 0 && downResult.Count > 0))
                        {
                            return new BoardAutoHelp(GamePieceSwapDirection.Left, focusPos);
                        }
                    }

                    if (IsMoveWithinBoard(GamePieceSwapDirection.Up, focusPos))
                    {
                        var copyMovedFocusPos = focusPos;
                        copyMovedFocusPos.y = j + 1;

                        var upResult = SearchUp(focusPiece, copyMovedFocusPos);
                        var leftResult = SearchLeft(focusPiece, copyMovedFocusPos);
                        var rightResult = SearchRight(focusPiece, copyMovedFocusPos);

                        if ((upResult.Count > 1 || leftResult.Count > 1 || rightResult.Count > 1) ||
                                (leftResult.Count > 0 && rightResult.Count > 0))
                        {
                            return new BoardAutoHelp(GamePieceSwapDirection.Up, focusPos);
                        }
                    }

                    if (IsMoveWithinBoard(GamePieceSwapDirection.Down, focusPos))
                    {
                        var copyMovedFocusPos = focusPos;
                        copyMovedFocusPos.y = j - 1;

                        var downResult = SearchDown(focusPiece, copyMovedFocusPos);
                        var leftResult = SearchLeft(focusPiece, copyMovedFocusPos);
                        var rightResult = SearchRight(focusPiece, copyMovedFocusPos);

                        if ((downResult.Count > 1 || leftResult.Count > 1 || rightResult.Count > 1) ||
                                (leftResult.Count > 0 && rightResult.Count > 0))
                        {
                            return new BoardAutoHelp(GamePieceSwapDirection.Down, focusPos);
                        }
                    }
                }
            }

            return null;
        }

        public void ActivateAssist()
        {
            isUsingAssist = true;
        }

        public void DeactivateAssist()
        {
            isUsingAssist = false;
        }

        public void StopAndEliminateBoard()
        {
            PlayClearBoardEffects();
            StopAllCoroutines();
        }

        public void SetBoardType(int boardType)
        {
            this.boardType = boardType;
            if (gamePieces != null)
            {
                gamePieces = gamePieces
                    .Take(boardType)
                    .ToArray();
            }
        }
    }
}
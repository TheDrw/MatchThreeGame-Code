using MatchThree.Core;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using MatchThree.Effects;
using BubbleShooter.Audio;
using MatchThree.Settings;
using DG.Tweening;
using System.Linq;

namespace BubbleShooter.Core
{
    /// <summary>
    /// Note : accessing gameboard is backwards => 
    /// HEIGHT , WIDTH so y,x.
    /// 
    /// Instead of the, probably better, way of doing collision with Colliders,
    /// the board checks the 2D distances of the game pieces in rows that relates to the height in which the 
    /// target piece (the piece we shoot) has traveled and see's if they're close enough to be recognized
    /// once it has passed it, it doesn't check for any from below. You can enable gizmos to see its visualization.
    /// 
    /// If moving the board, make sure it is not a fractional value for best/predictable results.
    /// </summary>
    public class BubbleShooterBoard : MonoBehaviour
    {
        #region Internal Class Board Collision Result
        class BoardCollisionInfo
        {
            public BoardCollisionInfo(Vector2Int idx, GamePiece gp)
            {
                BubbleLandIndex = idx; BubbleTouchedGamePiece = gp;
            }

            public Vector2Int BubbleLandIndex { get; private set; }
            public GamePiece BubbleTouchedGamePiece { get; private set; }
        }
        #endregion

        [SerializeField] GameSettings settings;
        [Space(5)]
        [SerializeField] GamePieces pieces;
        [SerializeField] BoardVFX boardVFX;
        [SerializeField] BubbleBoardSFX boardSFX;

        [Space(5)]
        [SerializeField] GamePiece wall;
        [SerializeField] GamePiece ceiling;
        [SerializeField] GameObject gameOverBottomBorder;

        [Header("Board settings")]
        [SerializeField] int numberOfTypes = 4;
        [SerializeField] int height = 8;
        [Tooltip("Has to be an odd number"),
            SerializeField] int width = 15;
        [Space(5)]
        [SerializeField] int startRowAmount = 1;
        [SerializeField] float collisionDistanceSize = 0.9f;

        [Tooltip("The amount of height the target must travel before the next row detects it. " +
            "Ex: if it's checking objects in row 2, this means it is at a height of 1.25m into it before it starts checking row 2 for" +
            "a value of 0.25f."),
            SerializeField]
        float rowDetectionAffordance = 0.25f;

        [Header("Matching")] 
        [Tooltip("Allow the game piece type: ANY to be able to match itself. Disable to fill board " +
            "with game pieces of type: ANY and enable to it test matching, dangling pieces..etc."),
            SerializeField]
        bool allowAnyTypeToMatch = false;

        /// <summary>
        /// The number of game pieces in the game that are not the walls or ceiling.
        /// </summary>
        public int TotalNumberOfGamePiecesOnBoard { get; private set; } = 0;
        public int Width => width;
        public int Height => height;

        Bubble trackingBubble = default;
        GamePiece[,] gameboard = null;
        GamePiece[] gamePieces = null;

        const float HEIGHT_OFFSET = 0f, BOARD_SIZE_IN_HALF = 0.5f,
            HEIGHT_ACTIVATOR = -0.4f;

        float widthOffset = 0f; 

        Vector3 positionOffset = Vector3.zero;

        Stack<Vector2Int> lastMoves = null;
        Vector2Int confirmedMove;
        AudioSource audioSource = default;

        public event Action OnReady = delegate { };
        public event Action OnMoveConfirmed = delegate { };
        public event Action<int> OnMatchFound = delegate { };
        public event Action OnMatchNotFound = delegate { };
        public event Action OnMatchDroppingsFell = delegate { };
        public event Action OnBubbleLandedAtBottomAndNoMatchFoundSoGameOver = delegate { };
        public event Action<GamePieceType> OnGamePieceTypeAdded = delegate { };
        public event Action<GamePieceType> OnGamePieceTypeRemoved = delegate { };
        public event Action OnBoardClear = delegate { };
        public event Action OnBoardSetupFinished = delegate { };
        public event Action OnBoardSetupStart = delegate { };

        static readonly Vector2Int FAKE_NULL = new Vector2Int(-42069, -42069); // random value to pretend it is null

        BoardCollisionInfo collisionInfo = null;

        bool isTrackingBubblePlacedAtBottom = false;
        bool hasBoardBeganInitialization = false;

        private void Awake()
        {
            gameboard = new GamePiece[height, width];
            gamePieces = pieces.GetGamePieces;
            lastMoves = new Stack<Vector2Int>();
            audioSource = GetComponent<AudioSource>();

            positionOffset.x = transform.position.x;
            positionOffset.y = transform.position.y;
        }

        private void OnValidate()
        {
            numberOfTypes = Mathf.Clamp(numberOfTypes, 1, 9);
            height = Mathf.Clamp(height, 4, 100);
            width = Mathf.Clamp(width , 3, 101);

            collisionDistanceSize = Mathf.Clamp(collisionDistanceSize, 0.1f, 2f);
            rowDetectionAffordance = Mathf.Clamp(rowDetectionAffordance, 0f, 1.5f);

            startRowAmount = Mathf.Clamp(startRowAmount, 0, height - 2);
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (width % 2 == 0)
            {
                width++;
                Debug.LogError($"ERR: width of board is an even number. It has to be an odd number." +
                    $" Changing board width to {width}", this);
            }
#endif
            widthOffset = Mathf.Floor(width / 2f) / 2f;

            InitializeGamePieceCountType();
        }

        public void SetupBoardInitialization()
        {
            if (hasBoardBeganInitialization) return;

            hasBoardBeganInitialization = true;
            StartCoroutine(SetupBoardCoroutine());
        }
        
        void VisualizeGameOverBorder()
        {
            gameOverBottomBorder.transform.DOScaleX(width / 2, .5f);
        }

        GamePiece GetRandomGamePiece() => gamePieces[UnityEngine.Random.Range(0, numberOfTypes)];

        private IEnumerator SetupBoardCoroutine()
        {
            //yield return new WaitForSeconds(1f);
            OnBoardSetupStart();
            yield return FillBoardWithRandomPieces();
            yield return CreateWalls();
            yield return CreateCeiling();
            CreateVisualsCeilingGaps();
            VisualizeGameOverBorder();

            OnBoardSetupFinished();
            OnReady();
        }

        private void InitializeGamePieceCountType()
        {
            gamePieces = gamePieces
                .Take(numberOfTypes)
                .ToArray();
        }

        private IEnumerator FillBoardWithRandomPieces()
        {
            bool alt = false;
            for (int y = height - 2; y >= height - startRowAmount - 1; y--)
            {
                int x = alt ? 2 : 1;
                int upper = alt ? width - 2 : width - 1;
                for (; x <= upper; x += 2)
                {
                    gameboard[y, x] = Instantiate(
                        GetRandomGamePiece().gameObject,
                        new Vector3((x * BOARD_SIZE_IN_HALF) - widthOffset, 
                            y + HEIGHT_OFFSET) + positionOffset,
                        Quaternion.identity).GetComponent<GamePiece>();

                    OnGamePieceTypeAdded(GetGamePieceAtGameboardIndex(x, y).GetGamePieceType);
                    TotalNumberOfGamePiecesOnBoard++;
                }

                yield return null;
                alt = !alt;
            }
        }

        private IEnumerator CreateWalls()
        {
            for (int y = height - 1; y >= 0; y--)
            {
                gameboard[y, 0] = Instantiate(
                        wall.gameObject,
                        new Vector3(-BOARD_SIZE_IN_HALF - widthOffset, 
                            y + HEIGHT_OFFSET) + positionOffset,
                        Quaternion.identity).GetComponent<GamePiece>();

                if (settings.EnableVFX)
                {
                    GetGamePieceAtGameboardIndex(0, y).PunchScaleVFX(1.5f, .5f, 1, 1f);
                }

                gameboard[y, width - 1] = Instantiate(
                        wall.gameObject,
                        new Vector3((width * BOARD_SIZE_IN_HALF) - widthOffset, 
                            y + HEIGHT_OFFSET) + positionOffset,
                        Quaternion.identity).GetComponent<GamePiece>();

                if (settings.EnableVFX)
                {
                    GetGamePieceAtGameboardIndex(width - 1, y).PunchScaleVFX(1.5f, .5f, 1, 1f);
                }

                yield return null;
            }
        }

        private IEnumerator CreateCeiling()
        {
            for (int x = 2; x < width - 1; x += 2)
            {
                gameboard[height - 1, x] = Instantiate(
                    ceiling.gameObject,
                    new Vector3((x * BOARD_SIZE_IN_HALF) - widthOffset,
                        height + HEIGHT_OFFSET - 1) + positionOffset,
                    Quaternion.identity).GetComponent<GamePiece>();
                yield return null;
            }
        }

        /// <summary>
        /// The corners have gaps because of how i have it made, this fills in those gaps.
        /// </summary>
        private void CreateVisualsCeilingGaps()
        {
            Vector3 topLeft = GetGamePieceAtGameboardIndex(0, height - 1).transform.position + Vector3.right * 0.5f, 
                topRight = GetGamePieceAtGameboardIndex(width - 1, height - 1).transform.position + Vector3.left * 0.5f;

            Instantiate(ceiling.gameObject, topLeft, Quaternion.identity);
            Instantiate(ceiling.gameObject, topRight, Quaternion.identity);
        }

        private void Update()
        {
            TrackBubble();
        }

        private void TrackBubble()
        {
            if (trackingBubble)
            {
                // TODO - if board is moved up, this ruins everything
                float currHeight = trackingBubble.transform.position.y + positionOffset.y;
                if (currHeight < HEIGHT_ACTIVATOR + positionOffset.y) return;

                int heightIdx = (int)Mathf.Clamp(currHeight - rowDetectionAffordance + 1, 0f, height - 1);
                if (HasCollidedWithGamePiecesInRow(heightIdx))
                {
                    HandleWhenBubbleCollidedWithGamePiece();
                }
            }
        }

        private void HandleWhenBubbleCollidedWithGamePiece()
        {
            trackingBubble.Stop();
            HandlePlacementBehaviours();
            HandleMatching();
        }

        public void EnqueueTrackingBubble(Bubble bubble)
        {
            trackingBubble = bubble;
        }

        bool HasCollidedWithGamePiecesInRow(int heightIdx)
        {
            for(int widthIdx = 0; widthIdx < width; widthIdx++)
            {
                var gamePiece = GetGamePieceAtGameboardIndex(widthIdx, heightIdx);
                if(gamePiece)
                {
                    float dist = Vector2.Distance(gamePiece.transform.position, trackingBubble.transform.position);

#if UNITY_EDITOR
                    Debug.DrawLine(gamePiece.transform.position, trackingBubble.transform.position, Color.green, Time.deltaTime);
                    if (dist < 1.5f)
                        Debug.DrawLine(gamePiece.transform.position, trackingBubble.transform.position, Color.red, Time.deltaTime);
#endif

                    if (dist <= collisionDistanceSize)
                    {
                        if (trackingBubble.LastHitID == gamePiece.GetInstanceID()) return false;

                        trackingBubble.CollidedWith(gamePiece.GetInstanceID());

                        if (gamePiece.IsSameAs(GamePieceType.Wall))
                        {
                            HandleTrackingBubbleBounce();
                            return false;
                        }

                        collisionInfo = new BoardCollisionInfo(new Vector2Int(widthIdx, heightIdx), gamePiece);
                        return true;
                    }
                }
            }

            return false;
        }

        private void HandleTrackingBubbleBounce()
        {
            if (settings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.BounceSFX);
            }

            if (settings.EnableVFX)
            {
                trackingBubble.GetComponent<IPunchScale>()
                    .PunchScaleVFX(-0.5f, 0.15f, 1, .5f);
            }

            trackingBubble.Bounce();
        }

        /// <summary>
        /// An iterative approach that first starts with the current game piece (target) that just landed.
        /// that target will initially check all neighbors around (left, right, top left, top right, bottom left,
        /// bottom right) and see if they are all the same type - note: there are only at most 5 neighbors. 
        /// put it in a list and repeat until there aren't anymore neighbors.
        /// 
        /// I believe this algorithm is called flood fill or somethin.
        /// </summary>
        void HandleMatching()
        {
            var target = GetGamePieceAtGameboardIndex(confirmedMove);

            if (!allowAnyTypeToMatch && 
                target.IsSameAs(GamePieceType.ANY)) return;

            target.Visit();
            var confirmedSpots = new List<Vector2Int> { confirmedMove };

            var neighbors = GetAllSurroundingNeighborsFromSpot(confirmedMove);
            while (neighbors.Count > 0)
            {
                var neighborsOfSameType = GetAllUnvisitedNeighborsOfSameType(neighbors, target.GetGamePieceType);
                confirmedSpots.AddRange(neighborsOfSameType);

                neighbors.Clear();
                neighbors = FillWithNewNeighbors(neighborsOfSameType);
                //print($"Found {neighbors.Count} more neighbors.");
            }


            //print($"Found a total of {confirmedSpots.Count} pieces of same type.");
            bool hasEnoughForMatch = confirmedSpots.Count >= 3;
            if(hasEnoughForMatch)
            {
                FoundMatches(confirmedSpots);
                BoardReady();
            }
            else
            {
                if (settings.EnableVFX)
                {
                    trackingBubble.GetComponent<IPunchScale>()
                        .PunchScaleVFX(-0.75f, 0.08f, 1, .5f);
                }

                FoundNoMatches(confirmedSpots);

                if (isTrackingBubblePlacedAtBottom)
                {
                    BoardGameOver();
                }
                else
                {
                    BoardReady();
                }
            }
        }

        void BoardGameOver()
        {
            trackingBubble = null;
            collisionInfo = null;
            OnBubbleLandedAtBottomAndNoMatchFoundSoGameOver();
        }

        void BoardReady()
        {
            trackingBubble = null;
            collisionInfo = null;
            OnReady();
        }

        private void FoundNoMatches(List<Vector2Int> confirmedSpots)
        {
            foreach (var confirmedSpot in confirmedSpots)
            {
                GetGamePieceAtGameboardIndex(confirmedSpot).Unvisit();
            }

            if (settings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.ParkedSFX);
            }
            OnMatchNotFound();
        }

        private void FoundMatches(List<Vector2Int> confirmedSpots)
        {
            foreach (var confirmedSpot in confirmedSpots)
            {
                var gamePiece = GetGamePieceAtGameboardIndex(confirmedSpot);

                if (settings.EnableVFX)
                {
                    gamePiece.PopScaleVFX(3f, .095f); // temp
                    Instantiate(boardVFX.Vanish, gamePiece.transform.position, Quaternion.identity);
                }

                Destroy(gamePiece.gameObject, .095f); // temp

                NullifyAtGameboardIndex(confirmedSpot);
            }

            FoundMatchesSFX(confirmedSpots.Count);
            OnMatchFound(confirmedSpots.Count);

            HandleDroppings(confirmedSpots);
        }

        #region Droppings Section

        private void HandleDroppings(List<Vector2Int> confirmedSpots)
        {
            var potentialDroppings = GetPotentialDroppingsFromMatchedSpots(confirmedSpots);
            var subsections = SplitDroppingAreasIntoSubsections(potentialDroppings);
            UnvisitSpotsWhenHandlingDroppings(subsections);
            DropSubsectionsThatAreNotTouchingCeiling(subsections);
        }

        private List<Vector2Int> GetPotentialDroppingsFromMatchedSpots(List<Vector2Int> confirmedSpots)
        {
            var potentialDroppings = new List<Vector2Int>();
            foreach (var spot in confirmedSpots)
            {
                potentialDroppings.AddRange(GetAllSurroundingNeighborsFromSpot(spot));
            }

            return potentialDroppings;
        }

        private void DropSubsectionsThatAreNotTouchingCeiling(List<List<Vector2Int>> subsections)
        {
            foreach (var subsection in subsections)
            {
                if (!IsAnySpotTouchingCeiling(subsection))
                {
                    DropTheSubsection(subsection);
                }
            }
        }

#if UNITY_EDITOR
        private void PrintOutSubsections(List<List<Vector2Int>> subsections)
        {
            foreach (var i in subsections)
            {
                string s = "";
                foreach (var j in i)
                {
                    s += $"{j.x},{j.y}   ";

                }
                print($"section : {s}\n");
            }
        }
#endif

        void UnvisitSpotsWhenHandlingDroppings(List<List<Vector2Int>> subsections)
        {
            foreach(var section in subsections)
            {
                foreach(var spot in section)
                {
                    GetGamePieceAtGameboardIndex(spot).Unvisit();
                }
            }
        }

        private List<List<Vector2Int>> SplitDroppingAreasIntoSubsections(List<Vector2Int> potentialDroppings)
        {
            var subsections = new List<List<Vector2Int>>();
            foreach (var dropSpot in potentialDroppings)
            {
                if (GetGamePieceAtGameboardIndex(dropSpot).IsVisited) continue;

                GetGamePieceAtGameboardIndex(dropSpot).Visit();
                var neighborSection = GetAllSurroundingNeighborsFromSpot(dropSpot);
                var sectionBuilder = new List<Vector2Int>() { dropSpot };

                while (neighborSection.Count > 0)
                {
                    var neighbors = GetAllUnvisitedNeighborsOfEveryType(neighborSection);
                    sectionBuilder.AddRange(neighbors);

                    neighborSection.Clear();
                    neighborSection = FillWithNewNeighbors(neighbors);
                }

                subsections.Add(sectionBuilder);
            }

            return subsections;
        }

        private void DropTheSubsection(List<Vector2Int> subsection)
        {
            const float LITTLE_BELOW_FLOOR = 0.75f, DROP_TIME = 0.13f;
            float floorPosition = GetGamePieceAtGameboardIndex(0, 0).transform.position.y - LITTLE_BELOW_FLOOR;
            foreach(var spot in subsection)
            {
                var gp = GetGamePieceAtGameboardIndex(spot).gameObject;
                float fallTime = (gp.transform.position.y - floorPosition) * DROP_TIME + UnityEngine.Random.Range(-0.05f, 0.05f);
                gp.transform.DOLocalMoveY(floorPosition, fallTime)
                    .SetEase(Ease.InQuad)
                    .OnComplete(()=> DroppingsEndEffects(gp));

                NullifyAtGameboardIndex(spot);
            }
        }

        /// <summary>
        /// The ending when the droppings fall to play SFX and VFX.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="skipEventCall"> will prevent the score listener being called so it wont add points to the score.</param>
        void DroppingsEndEffects(GameObject go, bool skipEventCall = false)
        {
            if (!skipEventCall)
            {
                OnMatchDroppingsFell();
            }

            if (settings.IsSFXEnabled)
            {
                audioSource.PlayOneShot(boardSFX.DropSFX);
            }

            if (settings.EnableVFX)
            {
                Instantiate(boardVFX.ClearBoard, go.transform.position, Quaternion.identity);
            }

            Destroy(go);
        }

        #endregion

        bool IsAnySpotTouchingCeiling(List<Vector2Int> spots)
        {
            foreach(var spot in spots)
            {
                if (IsSpotTouchingCeiling(spot)) return true;
            }

            return false;
        }

        void SetAllGamePiecesOnBoardToUnvisit()
        {
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    var gamePiece = GetGamePieceAtGameboardIndex(x, y);
                    if (gamePiece)
                    {
                        gamePiece.Unvisit();
                    }
                }
            }
        }


        bool IsSpotTouchingCeiling(Vector2Int spot)
        {
            return spot.y == height - 2;
        }

        void FoundMatchesSFX(int count)
        {
            if (!settings.IsSFXEnabled) return;

            if(count == 3)
            {
                audioSource.PlayOneShot(boardSFX.MatchThreeSFX);
            }
            else if(count > 3 && count <= 7)
            {
                audioSource.PlayOneShot(boardSFX.MatchFourSFX);
            }
            else if(count > 7 && count <= 10)
            {
                audioSource.PlayOneShot(boardSFX.MatchFiveSFX);
            }
            else
            {
                audioSource.PlayOneShot(boardSFX.MatchManySFX);
            }
        }

        List<Vector2Int> FillWithNewNeighbors(List<Vector2Int> neighbors)
        {
            var potentialNeighbors = new List<Vector2Int>();
            foreach(var potentialSpot in neighbors)
            {
                var foundSpots = GetAllSurroundingNeighborsFromSpot(potentialSpot);
                potentialNeighbors.AddRange(foundSpots);
            }
            return potentialNeighbors;
        }

        /// <summary>
        /// Marks the gamepiece as visited when it visits it.
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        List<Vector2Int> GetAllUnvisitedNeighborsOfSameType(List<Vector2Int> neighbors, GamePieceType targetType)
        {
            var spots = new List<Vector2Int>();
            foreach(var neighbor in neighbors)
            {
                var neighborPiece = GetGamePieceAtGameboardIndex(neighbor);
                if (!neighborPiece.IsVisited &&
                    neighborPiece.IsSameAs(targetType))
                {
                    neighborPiece.Visit();
                    spots.Add(neighbor);
                }
            }

            return spots;
        }

        /// <summary>
        /// Marks the gamepiece as visited when it visits it.
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        List<Vector2Int> GetAllUnvisitedNeighborsOfNotSameType(List<Vector2Int> neighbors, GamePieceType targetType)
        {
            var spots = new List<Vector2Int>();
            foreach (var neighbor in neighbors)
            {
                var neighborPiece = GetGamePieceAtGameboardIndex(neighbor);
                if (!neighborPiece.IsVisited &&
                    !neighborPiece.IsSameAs(targetType))
                {
                    neighborPiece.Visit();
                    spots.Add(neighbor);
                }
            }

            return spots;
        }

        /// <summary>
        /// Get all neighbors of every type besides the walls, floor and ceiling. Marks the gamepiece as visited
        /// when it visits it.
        /// </summary>
        /// <param name="neighbors"></param>
        /// <returns></returns>
        List<Vector2Int> GetAllUnvisitedNeighborsOfEveryType(List<Vector2Int> neighbors)
        {
            var spots = new List<Vector2Int>();
            foreach (var neighbor in neighbors)
            {
                var neighborPiece = GetGamePieceAtGameboardIndex(neighbor);
                if (!neighborPiece.IsVisited)
                {
                    neighborPiece.Visit();
                    spots.Add(neighbor);
                }
            }

            return spots;
        }

#if UNITY_EDITOR
        private void PrintNeighborCount(List<Vector2Int> neighbors)
        {
            if (neighbors.Count == 0)
            {
                print("Found no neighbors nearby.");
            }
            else
            {
                print($"Found {neighbors.Count} potential neighbors.");
            }
        }
#endif

        /// <summary>
        /// Check left, right, top left, top right, bottom left, bottom right in the gameboard 
        /// and returns them back into a list or spots.
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        List<Vector2Int> GetAllSurroundingNeighborsFromSpot(Vector2Int spot)
        {
            var neighbors = new List<Vector2Int>();
            Vector2Int left = CheckLeft(spot), right = CheckRight(spot),
                topLeft = CheckTopLeft(spot), topRight = CheckTopRight(spot),
                bottomLeft = CheckBottomLeft(spot), bottomRight = CheckBottomRight(spot);

            if (GetGamePieceAtGameboardIndex(left)) neighbors.Add(left);
            if (GetGamePieceAtGameboardIndex(right)) neighbors.Add(right);

            if (GetGamePieceAtGameboardIndex(topLeft)) neighbors.Add(topLeft);
            if (GetGamePieceAtGameboardIndex(topRight)) neighbors.Add(topRight);

            if (GetGamePieceAtGameboardIndex(bottomLeft)) neighbors.Add(bottomLeft);
            if (GetGamePieceAtGameboardIndex(bottomRight)) neighbors.Add(bottomRight);

            return neighbors;
        }

        #region Board Directional Checking

        Vector2Int CheckLeft(Vector2Int spot)
        {
            if (!IsWithinBoard(spot.x - 2, spot.y)) return FAKE_NULL;

            return new Vector2Int(spot.x - 2, spot.y);
        }

        Vector2Int CheckRight(Vector2Int spot)
        {
            if (!IsWithinBoard(spot.x + 2, spot.y)) return FAKE_NULL;

            return new Vector2Int(spot.x + 2, spot.y);
        }

        Vector2Int CheckTopRight(Vector2Int spot)
        {
            if (!IsWithinBoard(spot.x + 1, spot.y + 1)) return FAKE_NULL;

            return new Vector2Int(spot.x + 1, spot.y + 1);
        }

        Vector2Int CheckTopLeft(Vector2Int spot)
        {
            if (!IsWithinBoard(spot.x - 1, spot.y + 1)) return FAKE_NULL;

            return new Vector2Int(spot.x - 1, spot.y + 1);
        }

        Vector2Int CheckBottomRight(Vector2Int spot)
        {
            if (!IsWithinBoard(spot.x + 1, spot.y - 1)) return FAKE_NULL;

            return new Vector2Int(spot.x + 1, spot.y - 1);
        }

        Vector2Int CheckBottomLeft(Vector2Int spot)
        {
            if (!IsWithinBoard(spot.x - 1, spot.y - 1)) return FAKE_NULL;

            return new Vector2Int(spot.x - 1, spot.y - 1);
        }

        #endregion

        bool IsWithinBoard(Vector2Int spot)
        {
            return IsWithinBoard(spot.x, spot.y);
        }

        /// <summary>
        /// checks if x is within the playable width of the board. so if x <= 0 or x >= width, returns false.
        /// It considers walls as not within playable width.
        /// or checks if y is within the playable height of the board. if y < 0 or y >= height - 1, returns false. 
        /// It considers the floor and ceiling as not playable height.
        /// </summary>
        /// <param name="x">width check</param>
        /// <param name="y">height check</param>
        /// <returns></returns>
        bool IsWithinBoard(int x, int y)
        {
            int LEFT_WALL = 0, RIGHT_WALL = width - 1, 
                FLOOR = 0, CEILING = height - 1;

            bool isOutsideHorizontalBound = x <= LEFT_WALL || x >= RIGHT_WALL,
                isOutsideVerticalBound = y < FLOOR || y >= CEILING;

            if (isOutsideHorizontalBound || isOutsideVerticalBound) return false;

            return true;
        }

        // at some point, i reversed passing the parameters with x,y to be y,x (heightIdx, widthIdx) for some reason.
        private void HandlePlacementBehaviours()
        {
            int heightIdx = collisionInfo.BubbleLandIndex.y, widthIdx = collisionInfo.BubbleLandIndex.x;

            Vector2 dirToSpot = trackingBubble.transform.position - collisionInfo.BubbleTouchedGamePiece.transform.position;
            int verticalSide = Vector2.Dot(collisionInfo.BubbleTouchedGamePiece.transform.right, dirToSpot) < 0 ? -1 : 1; // left or right

            float horizontalSide = Vector2.Dot(collisionInfo.BubbleTouchedGamePiece.transform.up, dirToSpot);
            int adjacentPlacementValue = (int)Mathf.Abs(horizontalSide * 100f); // easier to deal with positive whole numbers

            //Debug.Log($"VERT: {verticalSide} ### ADJ : {adjacentPlacementValue}");

            if (IsIncomingAdjacent(adjacentPlacementValue))
            {
                HandleAdjacentBehaviour(heightIdx, widthIdx, verticalSide);
            }
            else
            {
                HandleBelowBehaviour(heightIdx, widthIdx, verticalSide);
            }
        }

        bool IsIncomingAdjacent(int val)
        {
            const int LOWER_HEIGHT_AFFORDANCE_LIMIT = 0, UPPER_HEIGHT_AFFORDANCE_LIMIT = 35;

            return val >= LOWER_HEIGHT_AFFORDANCE_LIMIT &&
                val < UPPER_HEIGHT_AFFORDANCE_LIMIT;
        }

        private void HandleAdjacentBehaviour(int heightIdx, int widthIdx, int verticalSide)
        {
            //print("hit side");
            int spotCheckHeight = heightIdx, 
                spotCheckWidth = verticalSide > 0 ? widthIdx + 2 : widthIdx - 2;

            bool isAdjacentSpotFree = GetGamePieceAtGameboardIndex(spotCheckWidth, spotCheckHeight) == null;
            if (isAdjacentSpotFree)
            {
                //print("parked adjacent");
                PlaceGamePieceAdjacent(heightIdx, widthIdx, verticalSide);
            }
            else
            {
                //print("side taken so going below");
                HandleBelowBehaviour(heightIdx, widthIdx, verticalSide);
            }
        }

        private void HandleBelowBehaviour(int heightIdx, int widthIdx, int verticalSide)
        {
            //print("hit below");
            int spotCheckHeight = heightIdx - 1, spotCheckWidth = widthIdx + verticalSide;

            bool isBottomSpotFree = GetGamePieceAtGameboardIndex(spotCheckWidth, spotCheckHeight) == null;

            if (isBottomSpotFree)
            {
                //print("parked below");
                PlaceGamePieceBelow(heightIdx, widthIdx, verticalSide);
            }
            else
            {
                //print("bottom was taken so parked adjacent");
                int spotCheckWithinBoard = verticalSide > 0 ? widthIdx + 2 : widthIdx - 2;
                bool isBeyondBoard = spotCheckWithinBoard < 0 || spotCheckWithinBoard >= width;
                if (isBeyondBoard)
                {
                    //print("tis beyond board");
                    const int OPPOSITE_SIDE_FROM_BEYOND_WALL = -1;
                    PlaceGamePieceBelow(heightIdx, widthIdx, OPPOSITE_SIDE_FROM_BEYOND_WALL * verticalSide);
                }
                else
                {
                    PlaceGamePieceAdjacent(heightIdx, widthIdx, verticalSide);
                }
            }
        }

        private void PlaceGamePieceAdjacent(int heightIdx, int widthIdx, int verticalSide)
        {
            int heightPlacement = heightIdx;
            int horizontalPlacement = verticalSide > 0 ? widthIdx + 2 : widthIdx - 2;

            float posX = (widthIdx + verticalSide) * BOARD_SIZE_IN_HALF - widthOffset + verticalSide * BOARD_SIZE_IN_HALF;
            trackingBubble.transform.position = new Vector3(posX, heightIdx, 0f) + positionOffset;
            SetTrackingBubbleIntoGameboardPlacementInMemory(horizontalPlacement, heightPlacement);
        }

        private void PlaceGamePieceBelow(int heightIdx, int widthIdx, int verticalSide)
        {
            int heightPlacement = heightIdx - 1;
            int horizontalPlacement = widthIdx + verticalSide;

            float posX = (widthIdx + verticalSide) * BOARD_SIZE_IN_HALF - widthOffset;
            trackingBubble.transform.position = new Vector3(posX, heightIdx - 1, 0f) + positionOffset;
            SetTrackingBubbleIntoGameboardPlacementInMemory(horizontalPlacement, heightPlacement);
        }

        void SetTrackingBubbleIntoGameboardPlacementInMemory(int x, int y)
        {
            //print($"planted at {x} {y}");
            confirmedMove = new Vector2Int(x, y);
            lastMoves.Push(confirmedMove);

            var gamePiece = trackingBubble.GetComponent<GamePiece>();
            gameboard[y, x] = gamePiece; // remember it is backwards (HEIGHT , WIDTH)
            OnGamePieceTypeAdded(gamePiece.GetGamePieceType);
            TotalNumberOfGamePiecesOnBoard++;

            isTrackingBubblePlacedAtBottom = y == 0;
        }

        /// <summary>
        /// Does not check if spot is within board. But if spot is null, returns null.
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        GamePiece GetGamePieceAtGameboardIndex(Vector2Int spot)
        {
            return spot != FAKE_NULL ? GetGamePieceAtGameboardIndex(spot.x, spot.y) :
                null;
        }


        /// <summary>
        /// Does not check if indices are within the board.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        GamePiece GetGamePieceAtGameboardIndex(int x, int y)
        {
            return gameboard[y, x]; // remember it is backwards (HEIGHT,WIDTH)
        }

        void NullifyAtGameboardIndex(Vector2Int spot)
        {
            TotalNumberOfGamePiecesOnBoard = Mathf.Max(TotalNumberOfGamePiecesOnBoard - 1, 0);
            OnGamePieceTypeRemoved(GetGamePieceAtGameboardIndex(spot).GetGamePieceType);
            gameboard[spot.y, spot.x] = null;

            if(TotalNumberOfGamePiecesOnBoard == 0)
            {
                OnBoardClear();
            }
        }

        void PrintBoard()
        {
            string all = "";
            for (int y = height - 1; y >= 0; y--)
            {
                string row = "";
                for (int x = 0; x < width; x++ )
                {
                    var spot = gameboard[y, x];
                    if (spot)
                    {
                        row += "1";
                    }
                    else
                    {
                        row += "-";
                    }
                    row += " ";
                }

                all += row + '\n';
            }

            Debug.Log($"{all}");
        }

        // for debugging
        public void UndoLastMove()
        {
            if (lastMoves.Count == 0) return;
            
            var lastMove = lastMoves.Pop();
            var gp = GetGamePieceAtGameboardIndex(lastMove);

            if (gp == null) return;

            OnGamePieceTypeRemoved(gp.GetGamePieceType);
            TotalNumberOfGamePiecesOnBoard--;

            Destroy(gp.gameObject);
            gameboard[lastMove.y, lastMove.x] = null;
            //PrintBoard();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            var bottomLeft = transform.position + Vector3.left * (Mathf.Floor((width + 3) / 2) / 2);
            var bottomRight = transform.position + Vector3.right * (Mathf.Floor((width + 3) / 2) / 2);
            var topLeft = bottomLeft + Vector3.up * (height - 0.5f);
            var topRight = bottomRight + Vector3.up * (height - 0.5f);
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topLeft, topRight);
        }

        bool HasActiveGamePiecesOnBoard => TotalNumberOfGamePiecesOnBoard > 0;

        public void StopTrackingBubble()
        {
            if (trackingBubble)
            {
                trackingBubble.Stop();
                DroppingsEndEffects(trackingBubble.gameObject, true);
                trackingBubble = null;
            }
        }

        public void EndBoard()
        {
            StartCoroutine(DropAllPieces());
        }

        IEnumerator DropAllPieces()
        {
            var wait = new WaitForSeconds(.1f);
            const float LITTLE_BELOW_FLOOR = 0.75f, DROP_TIME = 0.13f ;
            float floorPosition = GetGamePieceAtGameboardIndex(0, 0).transform.position.y - LITTLE_BELOW_FLOOR;
            for(int j = 0; j < height; j++)
            {
                for(int i = 0; i < width; i++)
                {
                    var gp = GetGamePieceAtGameboardIndex(i,j);
                    if (gp == null || 
                        !gp.IsPlayableGamePiece()) continue;


                    float fallTime = (gp.transform.position.y - floorPosition) * DROP_TIME + UnityEngine.Random.Range(-0.07f, 0.07f);
                    gp.transform.DOLocalMoveY(floorPosition, fallTime)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() => DroppingsEndEffects(gp.gameObject, true));
                }

                yield return wait;
            }
        }
    }
}
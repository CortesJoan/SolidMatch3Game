using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] tiles;
    public UnityEvent onMatchStart = new UnityEvent();
    public UnityEvent onMatch = new UnityEvent();
    public UnityEvent onBoardRefillComplete = new UnityEvent(); 
    public int refillType;

    [SerializeField] internal Tile[,] tileMatrix;

    private MatchChecker matchChecker;
    private TileManager tileManager;
    private BoardRefiller boardRefiller;
    private BoardCommandManager boardCommandManager;
    [Header("Animation Settings")]
    [SerializeField]
    internal float swapDuration = 0.5f;
    [SerializeField] private float waitForRefillDelay = 0.5f;
    [SerializeField] private float matchClearTime = 0.5f;

    [Header("Debug")] [SerializeField] internal List<Tile> prechargedTiles = new List<Tile>();

    [Header("Hint")] private Tile hintTile;

    [SerializeField] InputHandler inputHandler;

    private void OnEnable()
    {
        }

    private void OnDisable()
    {
     }

    public void Awake()
    {
        if (tileMatrix == null || tileMatrix.Length == 0)
        {
            tileMatrix = new Tile[width, height];
        }
        if (prechargedTiles.Count != 0)
            LoadPrechargedTiles();
        matchChecker = new MatchChecker(tileMatrix, width, height);
        boardCommandManager = GetComponent<BoardCommandManager>();
        if (!boardCommandManager)
        {
            boardCommandManager = gameObject.AddComponent<BoardCommandManager>();
        }
        tileManager = new TileManager(tileMatrix, tilePrefab, tiles, swapDuration, transform);
        boardRefiller = new BoardRefiller(tileMatrix, tilePrefab, tiles, width, height, transform);

        boardCommandManager.CreateNewCommandGroup("InitializeBoard");
        InitializeBoard();


        boardCommandManager.CreateNewCommandGroup("MovementInputStart");
    }

    public void LoadPrechargedTiles()
    {
        for (int row = 0; row < width; row++)
        {
            for (int column = 0; column < height; column++)
            {
                int index = row * height + column;
                if (index < prechargedTiles.Count)
                {
                    tileMatrix[row, column] = prechargedTiles[index];
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tileMatrix[i, j] == null)
                {
                    tileManager.CreateAndSetUpTile(i, j);
                }
            }
        }
    }


    public List<Tile> CheckForMatchesAt(int x, int y)
    {
        return matchChecker.CheckForMatchesAt(x, y);
    }


    public IEnumerator SwapTiles(Tile tileA, Tile tileB)
    {
        boardCommandManager.CreateNewCommandGroup("SwapTiles");
        int tileAX = tileA.x;
        int tileAY = tileA.y;
        int tileBX = tileB.x;
        int tileBY = tileB.y;

        yield return StartCoroutine(tileManager.SwapTiles(tileA, tileB));

        List<Tile> newMatches = matchChecker.CheckForMatchesAt(tileA.x, tileA.y);
        List<Tile> newMatchesB = matchChecker.CheckForMatchesAt(tileB.x, tileB.y);
        newMatches.AddRange(newMatchesB);
        if (newMatches.Count > 0)
        {
            onMatchStart.Invoke();
            ClearMatches(newMatches);
            yield return new WaitForSeconds(waitForRefillDelay);
            onBoardRefillComplete.Invoke();
            yield return StartCoroutine(RefillBoard());
        }
        else
        {
            // Swap tiles' positions in the matrix
            tileMatrix[tileAX, tileAY] = tileB;
            tileMatrix[tileBX, tileBY] = tileA;

            // Update tile coordinates
            tileA.x = tileBX;
            tileA.y = tileBY;
            tileB.x = tileAX;
            tileB.y = tileAY;


            tileA.MoveToPosition(tileAX, tileAY, swapDuration);
            tileB.MoveToPosition(tileBX, tileBY, swapDuration);

            // Wait for the reverse swap to complete
            yield return new WaitForSeconds(swapDuration);

            // Swap tiles' positions in the matrix back to the original
            tileMatrix[tileAX, tileAY] = tileA;
            tileMatrix[tileBX, tileBY] = tileB;

            // Restore the original tile coordinates
            tileA.x = tileAX;
            tileA.y = tileAY;
            tileB.x = tileBX;
            tileB.y = tileBY;
            inputHandler.UnBlockInput();
        }
    }

    public void ClearMatches(List<Tile> matches)
    {
        foreach (Tile tile in matches)
        {
            boardCommandManager.AddAndDoCommandToTheLastGroup(new CommandDestroy(tile,
                matchClearTime, delegate { }));
        }
    }

    public IEnumerator RefillBoard()
    {
        yield return StartCoroutine(boardRefiller.RefillBoard(refillType));

        List<Tile> newMatches = matchChecker.FindAllMatches();
        if (newMatches.Count > 0)
        {
            onMatch?.Invoke();
            ClearMatches(newMatches);
            yield return new WaitForSeconds(waitForRefillDelay);
            StartCoroutine(RefillBoard());
        }
        else
        {
            inputHandler.UnBlockInput();
        }
    }

    public bool AreTilesAdjacent(Tile tileA, Tile tileB)
    {
        int deltaX = Mathf.Abs(tileA.x - tileB.x);
        int deltaY = Mathf.Abs(tileA.y - tileB.y);

        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public UnityEvent<int, int> onTileMatchFound => matchChecker.OnTileMatchFound;

    public TileManager GetTileManager()
    {
        return tileManager;
    }

    public Tile[,] GetTileMatrix()
    {
        return tileMatrix;
    }

    public void SetTileMatrix(Tile[,] newMatrix)
    {
        tileMatrix = newMatrix;
    }

    

    private void SwapTilesInMatrix(Tile tileA, Tile tileB)
    {
        int tileAX = tileA.x;
        int tileAY = tileA.y;
        int tileBX = tileB.x;
        int tileBY = tileB.y;
        tileMatrix[tileAX, tileAY] = tileB;
        tileMatrix[tileBX, tileBY] = tileA;
        tileA.x = tileBX;
        tileA.y = tileBY;
        tileB.x = tileAX;
        tileB.y = tileAY;
    } 

    public List<Tile> FindHint()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile currentTile = tileMatrix[x, y];
                if (currentTile == null || !currentTile.IsFullySpawned())
                {
                    continue;
                }

                List<Vector2Int> directions = new List<Vector2Int>
                {
                    Vector2Int.up,
                    Vector2Int.right
                };

                foreach (Vector2Int direction in directions)
                {
                    int newX = x + direction.x;
                    int newY = y + direction.y;

                    if (IsValidCoordinate(newX, newY))
                    {
                        SwapTilesAt(x, y, newX, newY);
                        if (CheckForMatchesAt(x, y).Count >= 2 || CheckForMatchesAt(newX, newY).Count >= 2)
                        {
                            SwapTilesAt(x, y, newX, newY); // Swap back before returning
                            return new List<Tile> { currentTile, tileMatrix[newX, newY] };
                        }
                        SwapTilesAt(x, y, newX, newY); // Swap back if not valid hint
                    }
                }
            }
        }
        return new List<Tile>(); // Return empty list if no hints found
    }

    private void SwapTilesAt(int x1, int y1, int x2, int y2)
    {
        (tileMatrix[x1, y1], tileMatrix[x2, y2]) = (tileMatrix[x2, y2], tileMatrix[x1, y1]);
    }

    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
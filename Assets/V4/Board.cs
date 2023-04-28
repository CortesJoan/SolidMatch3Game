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
    public GameObject destructionEffectPrefab;
    public UnityEvent onMatchStart;
    public UnityEvent onBoardRefillComplete;
    public int refillType;

    internal Tile[,] tileMatrix;
    private TileDestructionEffect destructionEffect;

    private MatchChecker matchChecker;
    private TileManager tileManager;
    private BoardRefiller boardRefiller;
    private BoardCommandManager boardCommandManager;
    [Header("Animation Settings")]
    [SerializeField]
    internal float swapDuration = 0.5f;
    [SerializeField] private float waitForRefillDelay = 0.5f;
    [SerializeField] private float matchClearTime = 0.5f;
 
    void Awake()
    {
        tileMatrix = new Tile[width, height];

        // Pass proper arguments to MatchChecker constructor
        matchChecker = new MatchChecker(tileMatrix, width, height);

        // Pass proper arguments to TileManager constructor
        tileManager = new TileManager(tileMatrix, tilePrefab, tiles, swapDuration, transform);

        // Pass proper arguments to BoardRefiller constructor
        boardRefiller = new BoardRefiller(tileMatrix, tilePrefab, tiles, width, height, transform);

        InitializeBoard();
        boardCommandManager = GetComponent<BoardCommandManager>();
        GameObject effect = Instantiate(destructionEffectPrefab);
        destructionEffect = effect.GetComponent<TileDestructionEffect>();
    }


    private void InitializeBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tileManager.CreateAndSetUpTile(i, j);
            }
        }
    }

    public List<Tile> CheckForMatchesAt(int x, int y)
    {
        return matchChecker.CheckForMatchesAt(x, y);
    }


    public IEnumerator SwapTiles(Tile tileA, Tile tileB)
    {
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
            StartCoroutine(RefillBoard());
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

            // No match: reverse the swap
            // Swap back the tiles using DOTween
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
        }
    }

    public void ClearMatches(List<Tile> matches)
    {
        foreach (Tile tile in matches)
        {
            boardCommandManager.AddAndDoCommand(new CommandDestroy(tile,destructionEffect,duration: matchClearTime));
        }
    }

    public IEnumerator RefillBoard()
    {
        yield return StartCoroutine(boardRefiller.RefillBoard(refillType));

        List<Tile> newMatches = matchChecker.FindAllMatches();
        if (newMatches.Count > 0)
        {
            GameController.Instance.IncreaseCombo();
            ClearMatches(newMatches);
            yield return new WaitForSeconds(waitForRefillDelay);
            StartCoroutine(RefillBoard());
        }
    }

    public bool AreTilesAdjacent(Tile tileA, Tile tileB)
    {
        int deltaX = Mathf.Abs(tileA.x - tileB.x);
        int deltaY = Mathf.Abs(tileA.y - tileB.y);

        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public UnityEvent<int, int> onTileMatchFound => matchChecker.OnTileMatchFound;
}
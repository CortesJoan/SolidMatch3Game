using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class BoardTests
{
    // A reference to the Board instance
    private Board board;

    // A method that runs before each test and sets up the board
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Random.InitState(seed: 1);
        // Create a new Board game object
        GameObject boardGO = new GameObject("Board");
        // Add the Board component to it 
        board = boardGO.AddComponent<Board>();
        // Initialize the board with some parameters
        board.width = 8;
        board.height = 8;
        board.tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/V4/Tile.prefab");
        board.tiles = new[]
        {
            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/V4/Tile.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/V4/TileB.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/V4/TileC.prefab"),
        };
        board.refillType = 1;
        board.Awake();
        yield return null;
    }

    // A method that runs after each test and cleans up the board
    [TearDown]
    public void TearDown()
    {
        // Destroy the Board game object
        GameObject.Destroy(board.gameObject);
    }

    // A test that checks if the board is initialized correctly
    [UnityTest]
    public IEnumerator BoardIsInitialized()
    {
        var boardTileMatrix = board.GetTileMatrix();

        // Assert that the board has a tile matrix with the correct dimensions
        Assert.That(boardTileMatrix, Is.Not.Null);
        Assert.That(boardTileMatrix.GetLength(0), Is.EqualTo(board.width));
        Assert.That(boardTileMatrix.GetLength(1), Is.EqualTo(board.height));

        // Assert that each tile in the matrix is not null and has a valid tile type
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                Tile tile = boardTileMatrix[i, j];
                Assert.That(tile, Is.Not.Null);
                Assert.That(tile.tileType, Is.GreaterThanOrEqualTo(0));
                Assert.That(tile.tileType, Is.LessThan(board.tiles.Length));
            }
        }
        yield return null;
    }

    // A test that checks if two adjacent tiles can be swapped
    [UnityTest]
    public IEnumerator TilesCanBeSwapped()
    {
        var boardTileMatrix = board.GetTileMatrix();
        // Get two adjacent tiles from the board
        Tile tileA = boardTileMatrix[0, 0];
        Tile tileB = boardTileMatrix[1, 0];

        // Get their initial positions and types
        Vector2 posA = tileA.transform.position;
        Vector2 posB = tileB.transform.position;
        int typeA = tileA.tileType;
        int typeB = tileB.tileType;

        // Swap the tiles using the board's method
        yield return board.StartCoroutine(board.SwapTiles(tileA, tileB));
    }

    [UnityTest]
    public IEnumerator TilesCanBeMatched()
    {
        var boardTileMatrix = board.GetTileMatrix();
        // Get two adjacent tiles from the board
        Tile tileA = boardTileMatrix[0, 0];
        Tile tileB = boardTileMatrix[1, 0];
        tileA.tileType = tileB.tileType;
        // Get their initial positions and types
        Vector2 posA = tileA.transform.position;
        Vector2 posB = tileB.transform.position;
        int typeA = tileA.tileType;
        int typeB = tileB.tileType;

        // Swap the tiles using the board's method
        yield return board.StartCoroutine(board.SwapTiles(tileA, tileB));

        // Assert that their positions and types are swapped in the matrix
        Assert.IsNull(tileA);
        Assert.IsNull(tileB);
    }
}
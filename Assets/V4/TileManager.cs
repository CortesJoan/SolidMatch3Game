using System.Collections;
using UnityEngine;

public class TileManager
{
    private Tile[,] _tileMatrix;
    private GameObject _tilePrefab;
    private GameObject[] _tiles;
    private float _swapDuration;


    private Transform _parentTransform;
    private BoardCommandManager _boardCommandManager;

    public TileManager(Tile[,] tileMatrix, GameObject tilePrefab, GameObject[] tiles, float swapDuration,
        Transform parentTransform)
    {
        _tileMatrix = tileMatrix;
        _tilePrefab = tilePrefab;
        _tiles = tiles;
        _swapDuration = swapDuration;
        _parentTransform = parentTransform;
        _boardCommandManager =  parentTransform.GetComponent<BoardCommandManager>();
    }

    public void CreateAndSetUpTile(int x, int y)
    {
        GameObject newTile = Object.Instantiate(_tilePrefab, _parentTransform);
        newTile.transform.localPosition = new Vector2(x, y);
        int randomTile = GetRandomTileIndex(x, y);
        SetTileSprite(newTile, randomTile);

        _tileMatrix[x, y] = newTile.GetComponent<Tile>();
        _tileMatrix[x, y].Init(x, y, randomTile);
        _tileMatrix[x, y].SetIsFullySpawned(true); // Set IsFullySpawned to true for initial tiles
    }

    public IEnumerator SwapTiles(Tile tileA, Tile tileB)
    {
        int tileAX = tileA.x;
        int tileAY = tileA.y;
        int tileBX = tileB.x;
        int tileBY = tileB.y;
        _boardCommandManager.AddAndDoCommandToTheLastGroup(new CommandMove(tileA, tileBX, tileBY, _swapDuration));
        _boardCommandManager.AddAndDoCommandToTheLastGroup(new CommandMove(tileB, tileAX, tileAY, _swapDuration));
        _tileMatrix[tileAX, tileAY] = tileB;
        _tileMatrix[tileBX, tileBY] = tileA;
        tileA.x = tileBX;
        tileA.y = tileBY;
        tileB.x = tileAX;
        tileB.y = tileAY;

        yield return new WaitForSeconds(_swapDuration);
    }

    private int GetRandomTileIndex(int x, int y)
    {
        int randomTile;
        do
        {
            CommandSaveLoadRandomState commandSaveLoadRandomState =
                new CommandSaveLoadRandomState(0, _tiles.Length);
            _boardCommandManager.AddAndDoCommand(commandSaveLoadRandomState);
            randomTile = commandSaveLoadRandomState.result;
        } while (CreatesInitialMatch(x, y, randomTile));

        return randomTile;
    }

    private bool CreatesInitialMatch(int x, int y, int tileType)
    {
        if (x > 1 && tileType == _tileMatrix[x - 1, y]?.tileType && tileType == _tileMatrix[x - 2, y]?.tileType)
        {
            return true;
        }

        if (y > 1 && tileType == _tileMatrix[x, y - 1]?.tileType && tileType == _tileMatrix[x, y - 2]?.tileType)
        {
            return true;
        }

        return false;
    }

    internal void SetTileSprite(GameObject tileObject, int tileIndex)
    {
        SpriteRenderer renderer = tileObject.GetComponentInChildren<SpriteRenderer>();
        var prefabTile = _tiles[tileIndex].GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = prefabTile.sprite;
        renderer.color = prefabTile.color;
    }
}
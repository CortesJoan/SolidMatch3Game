using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class BoardRefiller
{
    private Tile[,] _tileMatrix;
    private GameObject _tilePrefab;
    private GameObject[] _tiles;
    private int _width, _height;
    private Transform _parentTransform;
    public  static UnityEvent<Tile> onTileFullSpawn = new UnityEvent<Tile>();

    public BoardRefiller(Tile[,] tileMatrix, GameObject tilePrefab, GameObject[] tiles, int width, int height,
        Transform parentTransform)
    {
        _tileMatrix = tileMatrix;
        _tilePrefab = tilePrefab;
        _tiles = tiles;
        _width = width;
        _height = height;
        _parentTransform = parentTransform;
    }

    public IEnumerator RefillBoard(int refillType)
    {
        // Wait for tiles to settle
        yield return new WaitForSeconds(0.5f);

        switch (refillType)
        {
            case 1:
                SpawnNewTiles();
                break;
            case 2:
                MakeTilesFallIntoEmptySpacesAndSpawnNewTiles();
                break;
            default:
                Debug.LogError("Invalid refillType provided.");
                break;
        }
    }

    private void SpawnNewTiles()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_tileMatrix[i, j] == null)
                {
                    int randomTile = GetRandomTileIndex(i, j);
                    BoardCommandManager.instance.AddAndDoCommand(new CommandCreateTile(this,    GameObject.FindObjectOfType<Board>().GetTileManager(), i, j, randomTile)); 
                }
            }
        }
    }

    internal Tile SpawnNewRandomTileAt(int x, int y)
    {
        GameObject newTile = Object.Instantiate(_tilePrefab,  _parentTransform);
        newTile.transform.localPosition = new Vector2(x, y);
        newTile.transform.SetParent(_parentTransform);
        int randomTile = GetRandomTileIndex(x, y);
        SetTileSprite(newTile, randomTile);

        Tile tileComponent = newTile.GetComponent<Tile>();
        tileComponent.Init(x, y, randomTile);
        _tileMatrix[x, y] = tileComponent;

        newTile.transform.DOScale(Vector3.zero, 0.5f).From().OnComplete(() =>
        {
            _tileMatrix[x, y].SetIsFullySpawned(true);
            onTileFullSpawn?.Invoke(tileComponent);
        });
        return _tileMatrix[x, y];
    }

    private int GetRandomTileIndex(int x, int y)
    {
        int randomTile;

        do
        {
            CommandSaveLoadRandomState commandSaveLoadRandomState =  new CommandSaveLoadRandomState(0, _tiles.Length);
            BoardCommandManager.instance.AddAndDoCommandToTheLastGroup(commandSaveLoadRandomState);
            randomTile =commandSaveLoadRandomState.result;
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

    private void SetTileSprite(GameObject tileObject, int tileIndex)
    {
        SpriteRenderer renderer = tileObject.GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = _tiles[tileIndex].GetComponentInChildren<SpriteRenderer>().sprite;
    }

    private void MakeTilesFallIntoEmptySpacesAndSpawnNewTiles()
    {
        for (int x = 0; x < _width; x++)
        {
            float fallDelay = 0;
            for (int y = 0; y < _height; y++)
            {
                if (_tileMatrix[x, y] == null)
                {
                    int newY = y;
                    while (newY < _height - 1 && _tileMatrix[x, newY] == null)
                    {
                        newY++;
                    }

                    if (newY < _height - 1)
                    {
                        _tileMatrix[x, newY].MoveToPosition(x, y, 0.5f);
                        _tileMatrix[x, newY].y = y;
                        _tileMatrix[x, y] = _tileMatrix[x, newY];
                        _tileMatrix[x, newY] = null;

                        fallDelay += 0.1f;
                    }
                    else
                    {
                        SpawnNewRandomTileAt(x, _height - 1);
                        _tileMatrix[x, _height - 1].transform.position = new Vector3(x, _height, 0);
                        _tileMatrix[x, _height - 1].MoveToPosition(x, y, 0.5f);
                        _tileMatrix[x, _height - 1].y = y;
                        _tileMatrix[x, y] = _tileMatrix[x, _height - 1];
                    }
                }
            }
        }
    }
}
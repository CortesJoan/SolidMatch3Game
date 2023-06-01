using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MatchChecker
{
    private Tile[,] _tileMatrix;
    private int _width;
    private int _height;
    public UnityEvent<int, int> OnTileMatchFound = new UnityEvent<int, int>();
    public MatchChecker(Tile[,] tileMatrix, int width, int height)
    {
        _tileMatrix = tileMatrix;
        _width = width;
        _height = height;
        OnTileMatchFound = new UnityEvent<int, int>();
        BoardRefiller.onTileFullSpawn.AddListener(OnTileFullSpawns);
    }

    private void OnTileFullSpawns(Tile spawnedTile)
    {
        CheckForMatchesAt(spawnedTile.x, spawnedTile.y);
        
    }

    public List<Tile> CheckForMatchesAt(int x, int y)
    {
        Tile tile = _tileMatrix[x, y];
        if (tile  == null || !tile.IsFullySpawned())
        {
            Debug.LogWarning("Not tile at pos x: " + x + " y: " + y);
            return new List<Tile>();
        }

        List<Tile> matchedTiles = new List<Tile>();

        List<Tile> horizontalMatches = CheckHorizontalMatchesAt(x, y);
        List<Tile> verticalMatches = CheckVerticalMatchesAt(x, y);

        if (horizontalMatches.Count > 0)
        {
            matchedTiles.Add(_tileMatrix[x, y]);
            matchedTiles.AddRange(horizontalMatches);
        }

        if (verticalMatches.Count > 0)
        {
            if (!matchedTiles.Contains(_tileMatrix[x, y]))
            {
                matchedTiles.Add(_tileMatrix[x, y]);
            }
            matchedTiles.AddRange(verticalMatches);
        }

        return matchedTiles;
    }

    private List<Tile> CheckHorizontalMatchesAt(int x, int y)
    {
        List<Tile> leftMatches = CheckMatchesInDirection(x, y, Vector2.left);
        List<Tile> rightMatches = CheckMatchesInDirection(x, y, Vector2.right);
        List<Tile> horizontalMatches = new List<Tile>(leftMatches);
        horizontalMatches.AddRange(rightMatches);

        if (GetTileAt(x,y).GetMinimumTileCountForMatch()<=horizontalMatches.Count+1)
        {
         
            return horizontalMatches;
            
        }
        return new List<Tile>();
    }
    
    private List<Tile> CheckVerticalMatchesAt(int x, int y)
    {
        List<Tile> upMatches = CheckMatchesInDirection(x, y, Vector2.up);
        List<Tile> downMatches = CheckMatchesInDirection(x, y, Vector2.down);
        List<Tile> verticalMatches = new List<Tile>(upMatches);
        verticalMatches.AddRange(downMatches);

        if (GetTileAt(x,y).GetMinimumTileCountForMatch()<= verticalMatches.Count+1)
        {
         
            return verticalMatches;
            
        }
        return new List<Tile>();
    }

    private List<Tile> CheckMatchesInDirection(int x, int y, Vector2 direction)
    {
        List<Tile> matchedTiles = new List<Tile>();

        Tile currentTile = _tileMatrix[x, y];
        
        if (!currentTile || !currentTile.IsFullySpawned())
        {
            return matchedTiles;
        }
        int currentType = currentTile.tileType;

        int xOffset = (int)direction.x;
        int yOffset = (int)direction.y;

        for (int i = 1; i < Mathf.Max(_width, _height); i++)
        {
            int newX = x + xOffset * i;
            int newY = y + yOffset * i;

            if (newX >= 0 && newX < _width && newY >= 0 && newY < _height)
            {
                Tile nextTile = _tileMatrix[newX, newY];
                if (nextTile != null && nextTile.tileType == currentType)
                {
                    matchedTiles.Add(nextTile);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return matchedTiles;
    }

    public List<Tile> FindAllMatches()
    {
        List<Tile> result = new List<Tile>();

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                List<Tile> matches = CheckForMatchesAt(i, j);

                if (matches.Count >= 1) // Change to work with a minimum match of two tiles
                {
                    result.AddRange(matches);
                }
            }
        }

        return result;
    }
    public Tile GetTileAt(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _tileMatrix[x, y];
        }
        else
        {
            return null;
        }
    }
}
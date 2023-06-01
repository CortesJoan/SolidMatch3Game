using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CommandBoardRefill : Command
{
    private BoardRefiller boardRefiller;
    private int refillType;
    private List<Tile> affectedTiles;
    private List<Vector2Int> originalPositions;

    public CommandBoardRefill(BoardRefiller boardRefiller, int refillType)
    {
        this.boardRefiller = boardRefiller;
        this.refillType = refillType;
        affectedTiles = new List<Tile>();
        originalPositions = new List<Vector2Int>();
        BoardRefiller.onTileFullSpawn.AddListener(OnTileFullSpawn);
    }

    public override void DoCommand(bool force = false)
    {
        boardRefiller.RefillBoard(refillType);
    }

    public override void UndoCommand(bool force = false)
    {
        for (int i = 0; i < affectedTiles.Count; i++)
        {
            Tile tile = affectedTiles[i];
            Vector2Int originalPosition = originalPositions[i];
            int currentX = tile.x;
            int currentY = tile.y;
            tile.MoveToPosition(originalPosition.x, originalPosition.y, 0.5f);
            Board board = GameObject.FindObjectOfType<Board>();
            board.tileMatrix[currentX, currentY] = null;
            board.tileMatrix[originalPosition.x, originalPosition.y] = tile;
            tile.x = originalPosition.x;
            tile.y = originalPosition.y;
        }
    }

    private void OnTileFullSpawn(Tile spawnedTile)
    {
        affectedTiles.Add(spawnedTile);
        originalPositions.Add(new Vector2Int(-1, -1));
    }
}
using System.Collections.Generic;
using UnityEngine;

public class CommandBoardRefill : Command
{
    // A field to store the reference to the board refiller
    private BoardRefiller boardRefiller;
    private int refillType; // A field to store the list of tiles that were spawned or moved during the refill
    private List<Tile> affectedTiles; // A field to store the list of original positions of the affected tiles
    private List<Vector2Int> originalPositions;

// The constructor receives the board refiller and the refill type
    public CommandBoardRefill(BoardRefiller boardRefiller, int refillType)
    {
        // Save the values received in the corresponding fields
        this.boardRefiller = boardRefiller;
        this.refillType = refillType;
        // Initialize the lists of affected tiles and original positions
        affectedTiles = new List<Tile>();
        originalPositions = new List<Vector2Int>();
        // Subscribe to the onTileFullSpawn event of the board refiller
        BoardRefiller.onTileFullSpawn.AddListener(OnTileFullSpawn);
    }

    public override void DoCommand(bool force = false)
    {
        // Call the RefillBoard method of the board refiller passing the refill type
        boardRefiller.RefillBoard(refillType);
    }

    public override void UndoCommand(bool force = false)
    {
        // Loop through the affected tiles and their original positions
        for (int i = 0; i < affectedTiles.Count; i++)
        {
            // Get the current tile and its original position
            Tile tile = affectedTiles[i];
            Vector2Int originalPosition = originalPositions[i];
            // Get the current position of the tile
            int currentX = tile.x;
            int currentY = tile.y;
            // Move the tile back to its original position using its MoveToPosition method
            tile.MoveToPosition(originalPosition.x, originalPosition.y, 0.5f);
            // Update the tile coordinates and its position in the board matrix
            Board board = GameObject.FindObjectOfType<Board>();
            board.tileMatrix[currentX, currentY] = null;
            board.tileMatrix[originalPosition.x, originalPosition.y] = tile;
            tile.x = originalPosition.x;
            tile.y = originalPosition.y;
        }
    }

    private void OnTileFullSpawn(Tile spawnedTile)
    {
        // Add the spawned tile to the list of affected tiles
        affectedTiles.Add(spawnedTile);
        // Add a dummy position (-1, -1) to the list of original positions for spawned tiles
        originalPositions.Add(new Vector2Int(-1, -1));
    }
}
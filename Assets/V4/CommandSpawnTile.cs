using System;
using DG.Tweening;
using UnityEngine;
[Serializable]

public class CommandSpawnTile :Command
{
     private Tile tileToDestroy;
     private int tileTypeBeforeDestroy;
      private float destructionEffectDuration;
 int xPos, yPos;
    public CommandSpawnTile(Tile tileToSpawn,int xPos,int yPos,  float duration, BoardRefiller boardRefiller)
    {
        boardRefiller.SpawnNewRandomTileAt(xPos, yPos);
         this.tileToDestroy = tileToDestroy;
         this.destructionEffectDuration = duration;
         this.tileTypeBeforeDestroy = tileToDestroy.tileType;
    }

    public override void DoCommand(bool force = false)
    {
        tileToDestroy.transform.DOScale(Vector3.zero, destructionEffectDuration)
            .OnComplete(() =>
            {
                GameObject.Destroy(tileToDestroy.gameObject);
                Board board = GameObject.FindObjectOfType<Board>();
                board.tileMatrix[tileToDestroy.x, tileToDestroy.y] = null;
            });
    }

    public override void UndoCommand(bool force = false)
    {
         Board board = GameObject.FindObjectOfType<Board>();
        TileManager tileManager = new TileManager(board.tileMatrix, board.tilePrefab, board.tiles, board.swapDuration,
            board.transform);
        tileManager.CreateAndSetUpTile(tileToDestroy.x, tileToDestroy.y);
         Tile newTile = board.tileMatrix[tileToDestroy.x, tileToDestroy.y];
        newTile.tileType = tileTypeBeforeDestroy;
        tileManager.SetTileSprite(newTile.gameObject, tileTypeBeforeDestroy);
         newTile.transform.localScale = Vector3.zero;
        newTile.transform.DOScale(Vector3.one, destructionEffectDuration)
            .OnComplete(() => { newTile.SetIsFullySpawned(true); });
    }
}
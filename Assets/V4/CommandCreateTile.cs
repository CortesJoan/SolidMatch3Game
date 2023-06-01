using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class CommandCreateTile : Command
{
    private BoardRefiller boardRefiller;
    private TileManager tileManager;
    private int xPos, yPos;
    private int tileType;
    private Tile createdTile;

    public CommandCreateTile(BoardRefiller boardRefiller,TileManager tileManager, int xPos, int yPos, int tileType)
    {
        this.tileManager = tileManager;
        this.boardRefiller = boardRefiller;
        this.xPos = xPos;
        this.yPos = yPos;
        this.tileType = tileType;
    }

    public override void DoCommand(bool force = false)
    {
        createdTile = boardRefiller.SpawnNewRandomTileAt(xPos, yPos);
        createdTile.tileType = tileType;
        tileManager.SetTileSprite(createdTile.gameObject, tileType);
    }

    public override void UndoCommand(bool force = false)
    {
        TileDestructionEffect destructionEffect = GameObject.FindObjectOfType<TileDestructionEffect>();
        destructionEffect.Play(createdTile.transform.position);
        createdTile.transform.DOScale(Vector3.zero, .5f)
            .OnComplete(() =>
            {
                GameObject.Destroy(createdTile.gameObject);
                Board board = GameObject.FindObjectOfType<Board>();
                board.tileMatrix[createdTile.x, createdTile.y] = null;
            });
    }
}
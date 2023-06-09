﻿using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[Serializable]

public class CommandDestroy : Command
{
    private Tile tileToDestroy;
    private int tileTypeBeforeDestroy;
    private float destructionEffectDuration;
    private Vector2Int positionBeforeDestroy;
    public CommandDestroy(Tile tileToDestroy,float duration, UnityAction onDestroyStart=null)
    {
        this.tileToDestroy = tileToDestroy;
         this.destructionEffectDuration = duration;
        this.tileTypeBeforeDestroy = tileToDestroy.tileType;
        onDoCommand.AddListener(onDestroyStart);
    }

    public override void DoCommand(bool force = false)
    {
        tileToDestroy.transform.DOScale(Vector3.zero, destructionEffectDuration)
            .OnComplete(() =>
            {
                positionBeforeDestroy= new Vector2Int(tileToDestroy.x,tileToDestroy.y);
                Board board = GameObject.FindObjectOfType<Board>();
                board.tileMatrix[tileToDestroy.x, tileToDestroy.y] = null;
                GameObject.Destroy(tileToDestroy.gameObject);

            });
    }

    public override void UndoCommand(bool force = false)
    {
        Board board = GameObject.FindObjectOfType<Board>();
        TileManager tileManager = new TileManager(board.tileMatrix, board.tilePrefab, board.tiles, board.swapDuration,
            board.transform);
        tileManager.CreateAndSetUpTile(positionBeforeDestroy.x, positionBeforeDestroy.y);
        Tile newTile = board.tileMatrix[positionBeforeDestroy.x, positionBeforeDestroy.y];
        newTile.tileType = tileTypeBeforeDestroy;
        tileManager.SetTileSprite(newTile.gameObject, tileTypeBeforeDestroy);
        newTile.transform.localScale = Vector3.zero;
        newTile.transform.DOScale(Vector3.one, destructionEffectDuration)
            .OnComplete(() => { newTile.SetIsFullySpawned(true); });
    }
}
using System;
using UnityEngine;
[Serializable]

public class CommandMove : Command
{
    private Tile tileToMove;
    private int originalX, originalY;
    private int newX, newY;
    private float moveDuration;

    public CommandMove(Tile tileToMove, int newX, int newY, float duration)
    {
        this.tileToMove = tileToMove;
        this.newX = newX;
        this.newY = newY;
        this.moveDuration = duration;
        this.originalX = tileToMove.x;
        this.originalY = tileToMove.y;
    }

    public override void DoCommand(bool force = false)
    {
        tileToMove.MoveToPosition(newX, newY, moveDuration);
        Board board = GameObject.FindObjectOfType<Board>();
        board.tileMatrix[originalX, originalY] = null;
        board.tileMatrix[newX, newY] = tileToMove;
        tileToMove.x = newX;
        tileToMove.y = newY;
    }

    public override void UndoCommand(bool force = false)
    {
        tileToMove.MoveToPosition(originalX, originalY, moveDuration);
        Board board = GameObject.FindObjectOfType<Board>();
        board.tileMatrix[newX, newY] = null;
        board.tileMatrix[originalX, originalY] = tileToMove;
        tileToMove.x = originalX;
        tileToMove.y = originalY;
    }
}
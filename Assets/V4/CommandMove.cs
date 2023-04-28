using UnityEngine;

public class CommandMove : Command
{
    // Un campo para guardar la referencia a la casilla a mover
    private Tile tileToMove;
    // Un campo para guardar las coordenadas originales de la casilla antes de moverla
    private int originalX, originalY;
    // Un campo para guardar las coordenadas nuevas de la casilla después de moverla
    private int newX, newY;
    // Un campo para guardar la duración del movimiento
    private float moveDuration;

    // El constructor recibe la casilla a mover, las coordenadas nuevas y la duración
    public CommandMove(Tile tileToMove, int newX, int newY, float duration)
    {
        // Guarda los valores recibidos en los campos correspondientes
        this.tileToMove = tileToMove;
        this.newX = newX;
        this.newY = newY;
        this.moveDuration = duration;
        // Guarda las coordenadas originales de la casilla antes de moverla
        this.originalX = tileToMove.x;
        this.originalY = tileToMove.y;
    }

    public override void DoCommand(bool force = false)
    {
        // Llama al método MoveToPosition de la casilla pasándole las coordenadas nuevas y la duración
        tileToMove.MoveToPosition(newX, newY, moveDuration);
        // Actualiza las coordenadas de la casilla y su posición en la matriz del tablero
        Board board = GameObject.FindObjectOfType<Board>();
        board.tileMatrix[originalX, originalY] = null;
        board.tileMatrix[newX, newY] = tileToMove;
        tileToMove.x = newX;
        tileToMove.y = newY;
    }

    public override void UndoCommand(bool force = false)
    {
        // Llama al método MoveToPosition de la casilla pasándole las coordenadas originales y la duración
        tileToMove.MoveToPosition(originalX, originalY, moveDuration);
        // Actualiza las coordenadas de la casilla y su posición en la matriz del tablero
        Board board = GameObject.FindObjectOfType<Board>();
        board.tileMatrix[newX, newY] = null;
        board.tileMatrix[originalX, originalY] = tileToMove;
        tileToMove.x = originalX;
        tileToMove.y = originalY;
    }
}
using DG.Tweening;
using UnityEngine;

public class CommandDestroy : Command
{
    // Un campo para guardar la referencia a la casilla a destruir
    private Tile tileToDestroy;
    // Un campo para guardar el tipo de la casilla antes de destruirla
    private int tileTypeBeforeDestroy;
    // Un campo para guardar el efecto de destrucción que se usa
    private TileDestructionEffect destructionEffect;
    // Un campo para guardar la duración del efecto de destrucción
    private float destructionEffectDuration;
    public CommandDestroy(Tile tileToDestroy, TileDestructionEffect destructionEffect, float duration)
    {
        // Guarda los valores recibidos en los campos correspondientes
        this.tileToDestroy = tileToDestroy;
        this.destructionEffect = destructionEffect;
        this.destructionEffectDuration = duration;
        // Guarda el tipo de la casilla antes de destruirla
        this.tileTypeBeforeDestroy = tileToDestroy.tileType;
    }

    public override void DoCommand(bool force = false)
    {
        // Llama al método Play del efecto de destrucción pasándole la posición de la casilla
        destructionEffect.Play(tileToDestroy.transform.position);
        // Escala la casilla a cero usando DOTween y al completarse la destruye y la pone en null en la matriz del tablero
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
        // Crea una nueva casilla en la posición de la casilla destruida usando el método CreateAndSetUpTile del TileManager
        Board board = GameObject.FindObjectOfType<Board>();
        TileManager tileManager = new TileManager(board.tileMatrix, board.tilePrefab, board.tiles, board.swapDuration, board.transform);
        tileManager.CreateAndSetUpTile(tileToDestroy.x, tileToDestroy.y);
        // Asigna el tipo guardado a la nueva casilla y actualiza su sprite
        Tile newTile = board.tileMatrix[tileToDestroy.x, tileToDestroy.y];
        newTile.tileType = tileTypeBeforeDestroy;
        tileManager.SetTileSprite(newTile.gameObject, tileTypeBeforeDestroy);
        // Escala la nueva casilla desde cero usando DOTween y al completarse le asigna el valor de isFullySpawned a true
        newTile.transform.localScale = Vector3.zero;
        newTile.transform.DOScale(Vector3.one, destructionEffectDuration)
            .OnComplete(() =>
            {
                newTile.SetIsFullySpawned(true);
            });
    }

}
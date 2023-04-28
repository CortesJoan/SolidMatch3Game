using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputHandler : MonoBehaviour
{
   
    [FormerlySerializedAs("OnTilesSwapped")] public  UnityEvent<Tile, Tile> onTilesSwapped;
    private Board board;
    private Tile selectedTile;
    private Camera mainCamera;
    private float maxTimeWithoutInput = 10f;
    private float currentTimeWithoutInput;
    public UnityEvent onNoInputForLongTime;
    void Awake()
    {
        if (onTilesSwapped == null)
        {
            onTilesSwapped = new  UnityEvent<Tile, Tile>();
        }
    }

    void Start()
    {
        board = FindObjectOfType<Board>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition =
                mainCamera.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            HandleInput(touchPosition);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            HandleInput(mousePosition);
        }
        currentTimeWithoutInput = Mathf.Min(currentTimeWithoutInput + Time.deltaTime,currentTimeWithoutInput);
        if (currentTimeWithoutInput==maxTimeWithoutInput)
        {
            onNoInputForLongTime?.Invoke();
            currentTimeWithoutInput = 0;
        }
    }

    private void HandleInput(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            Tile selectedTile = hit.collider.GetComponentInParent<Tile>();
            SelectTile(selectedTile);
        }
    }

    private void SelectTile(Tile tile)
    {
        if (tile == null || !tile.IsFullySpawned()) return;
        if (selectedTile == null)
        {
            selectedTile = tile;
        }
        else
        {
            if (board.AreTilesAdjacent(selectedTile, tile))
            {
                onTilesSwapped.Invoke(selectedTile, tile);
                selectedTile = null;
            }
            else
            {
                selectedTile = tile;
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputHandler : MonoBehaviour
{
    public UnityEvent<Tile, Tile> onTilesSwapped;
    private Board board;
    private Tile selectedTile;
    private Camera mainCamera;
    [SerializeField] private float maxTimeWithoutInput = 3f;
    private float currentTimeWithoutInput;
    public UnityEvent onNoInputForLongTime;
    public UnityEvent onInputAfterNoInputForLongTime;
    bool canInput = true; 
    void Awake()
    {
        if (onTilesSwapped == null)
        {
            onTilesSwapped = new UnityEvent<Tile, Tile>();
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

        if (canInput)
        {
            currentTimeWithoutInput = Mathf.Min(currentTimeWithoutInput + Time.deltaTime, maxTimeWithoutInput);
            if (currentTimeWithoutInput == maxTimeWithoutInput)
            {
                onNoInputForLongTime?.Invoke();
                currentTimeWithoutInput = 0;
            }   
        }
    }

    private void HandleInput(Vector2 position)
    {
        if (!canInput)
        {
            return;
        }
        if (currentTimeWithoutInput != 0)
        {
            onInputAfterNoInputForLongTime?.Invoke();
        }
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
            if (selectedTile != null)
            {
                selectedTile.Select();
            }
        }
        else
        {
            if (board.AreTilesAdjacent(selectedTile, tile))
            {
                selectedTile.UnSelect();
                onTilesSwapped.Invoke(selectedTile, tile);
                BlockInput();
                selectedTile = null;
            }
            else
            {
                tile.UnSelect();
                selectedTile = tile;
                selectedTile.Select();
            }
        }
    }

   
    public void BlockInput()
    {
        canInput = false;
    }

    public void UnBlockInput()
    {
        canInput = true;
        
    }
}
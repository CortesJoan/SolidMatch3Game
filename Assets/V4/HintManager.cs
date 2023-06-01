using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HintManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private InputHandler inputHandler;

    private List<Tile> hintedTiles;

    void OnEnable()
    {
        inputHandler.onNoInputForLongTime.AddListener(ShowHint);
        inputHandler.onInputAfterNoInputForLongTime.AddListener(HideHint);
    }

    void OnDisable()
    {
        inputHandler.onNoInputForLongTime.RemoveListener(ShowHint);
        inputHandler.onInputAfterNoInputForLongTime.RemoveListener(HideHint);
    }

    public void ShowHint()
    {
        hintedTiles = board.FindHint();
        foreach (Tile tile in hintedTiles)
        {
            tile.Highlight(true);
        }
    }

    public void HideHint()
    {
        if (hintedTiles != null)
        {
            foreach (Tile tile in hintedTiles)
            {
                tile.Highlight(false);
            }
        }
    }
} 
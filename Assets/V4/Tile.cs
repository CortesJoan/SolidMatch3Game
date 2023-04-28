using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int tileType;
    private bool isFullySpawned;
    [SerializeField] int numberOfSameTilesToMatch;

    public void Init(int xPos, int yPos, int type)
    {
        x = xPos;
        y = yPos;
        tileType = type;
    }

    public void MoveToPosition(int newX, int newY, float duration)
    {
        x = newX;
        y = newY;
        isFullySpawned = false;

        transform.DOMove(new Vector2(x, y), duration)
            .OnComplete(() => { isFullySpawned = true; });
    }


    public bool IsFullySpawned()
    {
        return isFullySpawned;
    }

    public void SetIsFullySpawned(bool value)
    {
        isFullySpawned = value;
    }

    public int GetMinimumTileCountForMatch()
    {
        return numberOfSameTilesToMatch;
    }
}